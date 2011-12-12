#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;


namespace HeuristicLab.Services.Hive {
  /// <summary>
  /// This class offers methods for cleaning up offline slaves and task
  /// </summary>
  public class EventManager : IEventManager {
    private IHiveDao dao {
      get { return ServiceLocator.Instance.HiveDao; }
    }
    private IAuthorizationManager auth {
      get { return ServiceLocator.Instance.AuthorizationManager; }
    }
    private ILogger log {
      get { return LogFactory.GetLogger(this.GetType().Namespace); }
    }

    public void Cleanup() {
      SetTimeoutSlavesOffline();
      SetTimeoutTasksWaiting();
      FinishParentTasks();
      UpdateStatistics();
    }

    private void UpdateStatistics() {
      var slaves = dao.GetSlaves(x => x.SlaveState == SlaveState.Calculating || x.SlaveState == SlaveState.Idle);

      var stats = new DataTransfer.Statistics();
      stats.TimeStamp = DateTime.Now;
      var slaveStats = new List<DT.SlaveStatistics>();
      foreach (var slave in slaves) {
        slaveStats.Add(new DT.SlaveStatistics() {
          SlaveId = slave.Id,
          Cores = slave.Cores.HasValue ? slave.Cores.Value : 0,
          FreeCores = slave.FreeCores.HasValue ? slave.FreeCores.Value : 0,
          Memory = slave.Memory.HasValue ? slave.Memory.Value : 0,
          FreeMemory = slave.FreeMemory.HasValue ? slave.FreeMemory.Value : 0,
          CpuUtilization = slave.CpuUtilization
        });
      }
      stats.SlaveStatistics = slaveStats;
      //collecting user statistics slows down the db and results in timeouts. 
      //we have to find another way to deal with this.  
      //until then the next line is commented out...
      //stats.UserStatistics = dao.GetUserStatistics();
      dao.AddStatistics(stats);
    }

    /// <summary>
    /// Searches for slaves which are timed out, puts them and their task offline
    /// </summary>
    private void SetTimeoutSlavesOffline() {
      var slaves = dao.GetSlaves(x => x.SlaveState != SlaveState.Offline);
      foreach (DT.Slave slave in slaves) {
        if (!slave.LastHeartbeat.HasValue || (DateTime.Now - slave.LastHeartbeat.Value) > HeuristicLab.Services.Hive.Properties.Settings.Default.SlaveHeartbeatTimeout) {
          slave.SlaveState = DT.SlaveState.Offline;
          SetTasksWaiting(slave.Id);
          dao.UpdateSlave(slave);
        }
      }
    }

    /// <summary>
    /// Looks for parent tasks which have FinishWhenChildJobsFinished and set their state to finished
    /// </summary>
    private void FinishParentTasks() {
      var parentTasksToFinish = dao.GetParentTasks(dao.GetResources(x => true).Select(x => x.Id), 0, true);
      foreach (var task in parentTasksToFinish) {
        dao.UpdateTaskState(task.Id, TaskState.Finished, null, null, string.Empty);
      }
    }

    private void SetTasksWaiting(Guid slaveId) {
      var tasks = dao.GetTasks(x => x.State == TaskState.Calculating).Where(x => x.StateLog.Last().SlaveId == slaveId);
      foreach (var j in tasks) {
        DT.Task task = dao.UpdateTaskState(j.Id, TaskState.Waiting, slaveId, null, "Slave timed out.");
        task.Command = null;
        dao.UpdateTask(task);
      }
    }

    /// <summary>
    /// Looks for task which have not sent heartbeats for some time and reschedules them for calculation
    /// </summary>
    private void SetTimeoutTasksWaiting() {
      var tasks = dao.GetTasks(x => (x.State == TaskState.Calculating && (DateTime.Now - x.LastHeartbeat) > HeuristicLab.Services.Hive.Properties.Settings.Default.CalculatingJobHeartbeatTimeout)
                               || (x.State == TaskState.Transferring && (DateTime.Now - x.LastHeartbeat) > HeuristicLab.Services.Hive.Properties.Settings.Default.TransferringJobHeartbeatTimeout));
      foreach (var j in tasks) {
        DT.Task task = dao.UpdateTaskState(j.Id, TaskState.Waiting, null, null, "Slave timed out.");
        task.Command = null;
        dao.UpdateTask(task);
      }
    }
  }
}
