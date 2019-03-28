#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.Hive.DataAccess.Interfaces;
using HeuristicLab.Services.Hive.DataTransfer;
using DA = HeuristicLab.Services.Hive.DataAccess;

namespace HeuristicLab.Services.Hive.Manager {
  public class HeartbeatManager {
    private const string MutexName = "HiveTaskSchedulingMutex";

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    private ITaskScheduler TaskScheduler {
      get { return ServiceLocator.Instance.TaskScheduler; }
    }

    /// <summary>
    /// This method will be called every time a slave sends a heartbeat (-> very often; concurrency is important!)
    /// </summary>
    /// <returns>a list of actions the slave should do</returns>
    public List<MessageContainer> ProcessHeartbeat(Heartbeat heartbeat) {
      List<MessageContainer> actions = new List<MessageContainer>();
      var pm = PersistenceManager;
      var slaveDao = pm.SlaveDao;
      var taskDao = pm.TaskDao;
      var slave = pm.UseTransaction(() => slaveDao.GetById(heartbeat.SlaveId));
      if (slave == null) {
        actions.Add(new MessageContainer(MessageContainer.MessageType.SayHello));
      } else {
        if (heartbeat.HbInterval != slave.HbInterval) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.NewHBInterval));
        }
        if (slaveDao.SlaveHasToShutdownComputer(slave.ResourceId)) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.ShutdownComputer));
        }
        // update slave data  
        slave.FreeCores = heartbeat.FreeCores;
        slave.FreeMemory = heartbeat.FreeMemory;
        slave.CpuUtilization = heartbeat.CpuUtilization;
        slave.SlaveState = (heartbeat.JobProgress != null && heartbeat.JobProgress.Count > 0)
          ? DA.SlaveState.Calculating
          : DA.SlaveState.Idle;
        slave.LastHeartbeat = DateTime.Now;
        pm.UseTransaction(() => {
          slave.IsAllowedToCalculate = slaveDao.SlaveIsAllowedToCalculate(slave.ResourceId);
          pm.SubmitChanges();
        });

        // update task data
        actions.AddRange(UpdateTasks(pm, heartbeat, slave.IsAllowedToCalculate));

        // assign new task
        if (heartbeat.AssignJob && slave.IsAllowedToCalculate && heartbeat.FreeCores > 0) {
          bool mutexAquired = false;
          var mutex = new Mutex(false, MutexName);
          try {
            mutexAquired = mutex.WaitOne(Properties.Settings.Default.SchedulingPatience);
            if (mutexAquired) {
              var waitingTasks = pm.UseTransaction(() => taskDao.GetWaitingTasks(slave)
                  .Select(x => new TaskInfoForScheduler {
                    TaskId = x.TaskId,
                    JobId = x.JobId,
                    Priority = x.Priority
                  })
                  .ToList()
              );
              var availableTasks = TaskScheduler.Schedule(waitingTasks).ToArray();
              if (availableTasks.Any()) {
                var task = availableTasks.First();
                AssignTask(pm, slave, task.TaskId);
                actions.Add(new MessageContainer(MessageContainer.MessageType.CalculateTask, task.TaskId));
              }
            } else {
              LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling could not be aquired.");
            }
          }
          catch (AbandonedMutexException) {
            LogFactory.GetLogger(this.GetType().Namespace).Log("HeartbeatManager: The mutex used for scheduling has been abandoned.");
          }
          catch (Exception ex) {
            LogFactory.GetLogger(this.GetType().Namespace).Log(string.Format("HeartbeatManager threw an exception in ProcessHeartbeat: {0}", ex));
          }
          finally {
            if (mutexAquired) mutex.ReleaseMutex();
          }
        }
      }
      return actions;
    }

    private void AssignTask(IPersistenceManager pm, DA.Slave slave, Guid taskId) {
      const DA.TaskState transferring = DA.TaskState.Transferring;
      DateTime now = DateTime.Now;
      var taskDao = pm.TaskDao;
      var stateLogDao = pm.StateLogDao;
      pm.UseTransaction(() => {
        var task = taskDao.GetById(taskId);
        stateLogDao.Save(new DA.StateLog {
          State = transferring,
          DateTime = now,
          TaskId = taskId,
          SlaveId = slave.ResourceId,
          UserId = null,
          Exception = null
        });
        task.State = transferring;
        task.LastHeartbeat = now;
        pm.SubmitChanges();
      });
    }

    /// <summary>
    /// Update the progress of each task
    /// Checks if all the task sent by heartbeat are supposed to be calculated by this slave
    /// </summary>
    private IEnumerable<MessageContainer> UpdateTasks(IPersistenceManager pm, Heartbeat heartbeat, bool isAllowedToCalculate) {
      var taskDao = pm.TaskDao;
      var jobDao = pm.JobDao;
      var assignedJobResourceDao = pm.AssignedJobResourceDao;
      var actions = new List<MessageContainer>();
      if (heartbeat.JobProgress == null || !heartbeat.JobProgress.Any())
        return actions;

      var jobIdsWithStatisticsPending = jobDao.GetJobIdsByState(DA.JobState.StatisticsPending).ToList();

      // select all tasks and statelogs with one query
      var taskIds = heartbeat.JobProgress.Select(x => x.Key).ToList();
      var taskInfos = pm.UseTransaction(() =>
        (from task in taskDao.GetAll()
          where taskIds.Contains(task.TaskId)
          let lastStateLog = task.StateLogs.OrderByDescending(x => x.DateTime).FirstOrDefault()
          select new {
            TaskId = task.TaskId,
            JobId = task.JobId,
            State = task.State,
            Command = task.Command,
            SlaveId = lastStateLog != null ? lastStateLog.SlaveId : default(Guid)
          }).ToList()
      );

      // process the jobProgresses
      foreach (var jobProgress in heartbeat.JobProgress) {
        var progress = jobProgress;
        var curTask = taskInfos.SingleOrDefault(x => x.TaskId == progress.Key);
        if (curTask == null) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, progress.Key));
          LogFactory.GetLogger(this.GetType().Namespace).Log("Task on slave " + heartbeat.SlaveId + " does not exist in DB: " + jobProgress.Key);
        } else if (jobIdsWithStatisticsPending.Contains(curTask.JobId)) {
          // parenting job of current task has been requested for deletion (indicated by job state "Statistics Pending")
          // update task execution time
          pm.UseTransaction(() => {
            taskDao.UpdateExecutionTime(curTask.TaskId, progress.Value.TotalMilliseconds);
          });
          actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.TaskId));
          LogFactory.GetLogger(this.GetType().Namespace).Log("Abort task " + curTask.TaskId + " on slave " + heartbeat.SlaveId + ". The parenting job " + curTask.JobId + " was requested to be deleted.");
        } else if (curTask.SlaveId == Guid.Empty || curTask.SlaveId != heartbeat.SlaveId) {
          // assigned slave does not match heartbeat
          actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.TaskId));
          LogFactory.GetLogger(this.GetType().Namespace).Log("The slave " + heartbeat.SlaveId + " is not supposed to calculate task: " + curTask.TaskId);
        } else if (!isAllowedToCalculate) {
          actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.TaskId));
          LogFactory.GetLogger(this.GetType().Namespace).Log("The slave " + heartbeat.SlaveId + " is not allowed to calculate any tasks tue to a downtime. The task is paused.");
        } else if (!assignedJobResourceDao.CheckJobGrantedForResource(curTask.JobId, heartbeat.SlaveId)) {
          // slaveId (and parent resourceGroupIds) are not among the assigned resources ids for task-parenting job
          // this might happen when (a) job-resource assignment has been changed (b) slave is moved to different group
          actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.TaskId));
          LogFactory.GetLogger(this.GetType().Namespace).Log("The slave " + heartbeat.SlaveId + " is not granted to calculate task: " + curTask.TaskId + " of job: " + curTask.JobId);
        } else {
          // update task execution time
          pm.UseTransaction(() => {
            taskDao.UpdateExecutionTime(curTask.TaskId, progress.Value.TotalMilliseconds);
          });
          switch (curTask.Command) {
            case DA.Command.Stop:
              actions.Add(new MessageContainer(MessageContainer.MessageType.StopTask, curTask.TaskId));
              break;
            case DA.Command.Pause:
              actions.Add(new MessageContainer(MessageContainer.MessageType.PauseTask, curTask.TaskId));
              break;
            case DA.Command.Abort:
              actions.Add(new MessageContainer(MessageContainer.MessageType.AbortTask, curTask.TaskId));
              break;
          }
        }
        
      } 
      return actions;
    }
  }
}
