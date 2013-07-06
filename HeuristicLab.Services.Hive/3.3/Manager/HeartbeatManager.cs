#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Services.Hive.DataAccess;
using Heartbeat = HeuristicLab.Services.Hive.DataTransfer.Heartbeat;

namespace HeuristicLab.Services.Hive {
  public class HeartbeatManager {
    private const string MutexName = "HiveTaskSchedulingMutex";

    private IOptimizedHiveDao dao {
      get { return ServiceLocator.Instance.OptimizedHiveDao; }
    }
    private ITaskScheduler taskScheduler {
      get { return ServiceLocator.Instance.TaskScheduler; }
    }
    private DataAccess.ITransactionManager trans {
      get { return ServiceLocator.Instance.TransactionManager; }
    }

    /// <summary>
    /// This method will be called every time a slave sends a heartbeat (-> very often; concurrency is important!)
    /// </summary>
    /// <returns>a list of actions the slave should do</returns>
    public List<MessageContainer> ProcessHeartbeat(Heartbeat heartbeat) {
      List<MessageContainer> actions = new List<MessageContainer>();

      Slave slave = null;
      trans.UseTransaction(() => {
        slave = dao.GetSlaveById(heartbeat.SlaveId);
      });
      if (slave == null) {
        actions.Add(new MessageContainer(MessageContainer.MessageType.SayHello));
      } else {
        if (heartbeat.HbInterval != slave.HbInterval) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.NewHBInterval));
        }
        if (dao.SlaveHasToShutdownComputer(slave.ResourceId)) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.ShutdownComputer));
        }

        // update slave data
        slave.FreeCores = heartbeat.FreeCores;
        slave.FreeMemory = heartbeat.FreeMemory;
        slave.CpuUtilization = heartbeat.CpuUtilization;
        slave.IsAllowedToCalculate = dao.SlaveIsAllowedToCalculate(slave.ResourceId);
        slave.SlaveState = (heartbeat.JobProgress != null && heartbeat.JobProgress.Count > 0) ? SlaveState.Calculating : SlaveState.Idle;
        slave.LastHeartbeat = DateTime.Now;

        trans.UseTransaction(() => {
          dao.UpdateSlave(slave);
        });

        // update task data
        actions.AddRange(UpdateTasks(heartbeat, slave.IsAllowedToCalculate));

        // assign new task
        if (heartbeat.AssignJob && slave.IsAllowedToCalculate && heartbeat.FreeCores > 0) {
          bool mutexAquired = false;
          var mutex = new Mutex(false, MutexName);
          try {

            mutexAquired = mutex.WaitOne(Properties.Settings.Default.SchedulingPatience);
            if (!mutexAquired)
              LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling could not be aquired.");
            else {
              trans.UseTransaction(() => {
                IEnumerable<TaskInfoForScheduler> availableTasks = null;
                availableTasks = taskScheduler.Schedule(dao.GetWaitingTasks(slave).ToArray());
                if (availableTasks.Any()) {
                  var task = availableTasks.First();
                  AssignTask(slave, task.TaskId);
                  actions.Add(new MessageContainer(MessageContainer.MessageType.CalculateTask, task.TaskId));
                }
              });
            }
          }
          catch (AbandonedMutexException) {
            LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling has been abandoned.");
          }
          catch (Exception ex) {
            LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager threw an exception in ProcessHeartbeat: " + ex.ToString());
          }
          finally {
            if (mutexAquired) mutex.ReleaseMutex();
          }
        }
      }
      return actions;
    }

    private void AssignTask(Slave slave, Guid taskId) {
      var task = dao.UpdateTaskState(taskId, TaskState.Transferring, slave.ResourceId, null, null);

      // from now on the task has some time to send the next heartbeat (ApplicationConstants.TransferringJobHeartbeatTimeout)
      task.LastHeartbeat = DateTime.Now;
      dao.UpdateTask(task);
    }

    /// <summary>
    /// Update the progress of each task
    /// Checks if all the task sent by heartbeat are supposed to be calculated by this slave
    /// </summary>
    private IEnumerable<MessageContainer> UpdateTasks(Heartbeat heartbeat, bool IsAllowedToCalculate) {
      List<MessageContainer> actions = new List<MessageContainer>();

      if (heartbeat.JobProgress == null)
        return actions;

      if (!IsAllowedToCalculate && heartbeat.JobProgress.Count != 0) {
        actions.Add(new MessageContainer(MessageContainer.MessageType.PauseAll));
      } else {
        // process the jobProgresses
        foreach (var jobProgress in heartbeat.JobProgress) {
          Tuple<Task, Guid?> taskWithLastStateLogSlaveId = null;
          trans.UseTransaction(() => {
            taskWithLastStateLogSlaveId = dao.GetTaskByIdAndLastStateLogSlaveId(jobProgress.Key);
          });
          var curTask = taskWithLastStateLogSlaveId != null ? taskWithLastStateLogSlaveId.Item1 : null;
          if (curTask == null) {
            // task does not exist in db
            actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, jobProgress.Key));
            LogFactory.GetLogger(this.GetType().Namespace).Log("Task on slave " + heartbeat.SlaveId + " does not exist in DB: " + jobProgress.Key);
          } else {
            var slaveId = taskWithLastStateLogSlaveId.Item2;
            if (slaveId == Guid.Empty || slaveId != heartbeat.SlaveId) {
              // assigned slave does not match heartbeat
              actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.TaskId));
              LogFactory.GetLogger(this.GetType().Namespace).Log("The slave " + heartbeat.SlaveId + " is not supposed to calculate task: " + curTask);
            } else if (!dao.TaskIsAllowedToBeCalculatedBySlave(curTask.TaskId, heartbeat.SlaveId)) {
              // assigned resources ids of task do not match with slaveId (and parent resourceGroupIds); this might happen when slave is moved to different group
              actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.TaskId));
            } else {
              // save task execution time
              curTask.ExecutionTimeMs = jobProgress.Value.TotalMilliseconds;
              curTask.LastHeartbeat = DateTime.Now;

              switch (curTask.Command) {
                case Command.Stop:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.StopTask, curTask.TaskId));
                  break;
                case Command.Pause:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.TaskId));
                  break;
                case Command.Abort:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.TaskId));
                  break;
              }
              trans.UseTransaction(() => {
                dao.UpdateTask(curTask);
              });
            }
          }
        }
      }
      return actions;
    }
  }
}
