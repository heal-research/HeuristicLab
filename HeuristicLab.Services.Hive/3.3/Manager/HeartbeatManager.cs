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
using HeuristicLab.Services.Hive.DataTransfer;
using DA = HeuristicLab.Services.Hive.DataAccess;

namespace HeuristicLab.Services.Hive {
  public class HeartbeatManager {
    private const string MutexName = "HiveTaskSchedulingMutex";

    private IHiveDao dao {
      get { return ServiceLocator.Instance.HiveDao; }
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
      slave = trans.UseTransaction(() => { return dao.GetSlave(heartbeat.SlaveId); });

      if (slave == null) {
        actions.Add(new MessageContainer(MessageContainer.MessageType.SayHello));
      } else {
        if (heartbeat.HbInterval != slave.HbInterval) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.NewHBInterval));
        }
        if (ShutdownSlaveComputer(slave.Id)) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.ShutdownComputer));
        }

        // update slave data
        slave.FreeCores = heartbeat.FreeCores;
        slave.FreeMemory = heartbeat.FreeMemory;
        slave.CpuUtilization = heartbeat.CpuUtilization;
        slave.IsAllowedToCalculate = SlaveIsAllowedToCalculate(slave.Id);
        slave.SlaveState = (heartbeat.JobProgress != null && heartbeat.JobProgress.Count > 0) ? SlaveState.Calculating : SlaveState.Idle;
        slave.LastHeartbeat = DateTime.Now;

        trans.UseTransaction(() => { dao.UpdateSlave(slave); });

        // update task data
        actions.AddRange(UpdateTasks(heartbeat, slave.IsAllowedToCalculate));

        // assign new task
        if (heartbeat.AssignJob && slave.IsAllowedToCalculate && heartbeat.FreeCores > 0) {
          bool mutexAquired = false;
          var mutex = new Mutex(false, MutexName);
          try {
            mutexAquired = mutex.WaitOne(Properties.Settings.Default.SchedulingPatience);
            if (!mutexAquired)
              DA.LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling could not be aquired.");
            else {
              IEnumerable<TaskInfoForScheduler> availableTasks = null;
              availableTasks = trans.UseTransaction(() => { return taskScheduler.Schedule(dao.GetWaitingTasks(slave)); });
              if (availableTasks.Any()) {
                var task = availableTasks.First();
                AssignJob(slave, task.TaskId);
                actions.Add(new MessageContainer(MessageContainer.MessageType.CalculateTask, task.TaskId));
              }
            }
          }
          catch (AbandonedMutexException) {
            DA.LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling has been abandoned.");
          }
          catch (Exception ex) {
            DA.LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager threw an exception in ProcessHeartbeat: " + ex.ToString());
          }
          finally {
            if (mutexAquired) mutex.ReleaseMutex();
          }
        }
      }
      return actions;
    }

    private void AssignJob(Slave slave, Guid taskId) {
      trans.UseTransaction(() => {
        var task = dao.UpdateTaskState(taskId, DataAccess.TaskState.Transferring, slave.Id, null, null);

        // from now on the task has some time to send the next heartbeat (ApplicationConstants.TransferringJobHeartbeatTimeout)
        task.LastHeartbeat = DateTime.Now;
        dao.UpdateTask(task);
      });
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
          Task curTask = null;
          curTask = trans.UseTransaction(() => { return dao.GetTask(jobProgress.Key); });
          if (curTask == null) {
            // task does not exist in db
            actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, jobProgress.Key));
            DA.LogFactory.GetLogger(this.GetType().Namespace).Log("Task on slave " + heartbeat.SlaveId + " does not exist in DB: " + jobProgress.Key);
          } else {
            if (curTask.CurrentStateLog.SlaveId == Guid.Empty || curTask.CurrentStateLog.SlaveId != heartbeat.SlaveId) {
              // assigned slave does not match heartbeat
              actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.Id));
              DA.LogFactory.GetLogger(this.GetType().Namespace).Log("The slave " + heartbeat.SlaveId + " is not supposed to calculate task: " + curTask);
            } else if (!TaskIsAllowedToBeCalculatedBySlave(heartbeat.SlaveId, curTask)) {
              // assigned resources ids of task do not match with slaveId (and parent resourceGroupIds); this might happen when slave is moved to different group
              actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.Id));
            } else {
              // save task execution time
              curTask.ExecutionTime = jobProgress.Value;
              curTask.LastHeartbeat = DateTime.Now;

              switch (curTask.Command) {
                case Command.Stop:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.StopTask, curTask.Id));
                  break;
                case Command.Pause:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.Id));
                  break;
                case Command.Abort:
                  actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.Id));
                  break;
              }
              trans.UseTransaction(() => { dao.UpdateTask(curTask); });
            }
          }
        }
      }
      return actions;
    }

    private bool TaskIsAllowedToBeCalculatedBySlave(Guid slaveId, Task curTask) {
      return trans.UseTransaction(() => {
        var assignedResourceIds = dao.GetAssignedResources(curTask.Id).Select(x => x.Id);
        var slaveResourceIds = dao.GetParentResources(slaveId).Select(x => x.Id);
        return assignedResourceIds.Any(x => slaveResourceIds.Contains(x));
      });
    }

    private bool SlaveIsAllowedToCalculate(Guid slaveId) {
      // the slave may only calculate if there is no downtime right now. this needs to be checked for every parent resource also
      return trans.UseTransaction(() => { return dao.GetParentResources(slaveId).All(r => dao.GetDowntimes(x => x.ResourceId == r.Id && x.DowntimeType == DA.DowntimeType.Offline && (DateTime.Now >= x.StartDate) && (DateTime.Now <= x.EndDate)).Count() == 0); });
    }

    private bool ShutdownSlaveComputer(Guid slaveId) {
      return trans.UseTransaction(() => { return dao.GetParentResources(slaveId).Any(r => dao.GetDowntimes(x => x.ResourceId == r.Id && x.DowntimeType == DA.DowntimeType.Shutdown && (DateTime.Now >= x.StartDate) && (DateTime.Now <= x.EndDate)).Count() != 0); });
    }
  }
}
