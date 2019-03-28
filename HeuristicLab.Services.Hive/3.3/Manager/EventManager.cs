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
using System.Linq;
using HeuristicLab.Services.Hive.DataAccess;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;

namespace HeuristicLab.Services.Hive.Manager {
  public class EventManager : IEventManager {
    private const string SlaveTimeout = "Slave timed out.";
    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public void Cleanup() {
      var pm = PersistenceManager;

      pm.UseTransaction(() => {
        FinishJobDeletion(pm);
        pm.SubmitChanges();
      });

      pm.UseTransaction(() => {
        SetTimeoutSlavesOffline(pm);
        SetTimeoutTasksWaiting(pm);
        DeleteObsoleteSlaves(pm);
        pm.SubmitChanges();
      });

      pm.UseTransaction(() => {
        FinishParentTasks(pm);
        pm.SubmitChanges();
      });
    }

    /// <summary>
    /// Deletes all jobs which are in state "DeletionPending" (this will include all corresponding tasks).
    /// The state "DeletionPending" is set by HiveJanitor > StatisticsGenerator
    /// </summary>
    private void FinishJobDeletion(IPersistenceManager pm) {
      var jobDao = pm.JobDao;
      jobDao.DeleteByState(JobState.DeletionPending);
    }

    /// <summary>
    /// Searches for slaves which are timed out, puts them and their task offline
    /// </summary>
    private void SetTimeoutSlavesOffline(IPersistenceManager pm) {
      var slaveDao = pm.SlaveDao;
      var slaves = slaveDao.GetOnlineSlaves();
      foreach (var slave in slaves) {
        if (!slave.LastHeartbeat.HasValue ||
            (DateTime.Now - slave.LastHeartbeat.Value) >
            Properties.Settings.Default.SlaveHeartbeatTimeout) {
          slave.SlaveState = SlaveState.Offline;
        }
      }
    }

    /// <summary>
    /// Looks for parent tasks which have FinishWhenChildJobsFinished and set their state to finished
    /// </summary>
    private void FinishParentTasks(IPersistenceManager pm) {
      var resourceDao = pm.ResourceDao;
      var taskDao = pm.TaskDao;
      var resourceIds = resourceDao.GetAll().Select(x => x.ResourceId).ToList();
      var parentTasksToFinish = taskDao.GetParentTasks(resourceIds, 0, true);
      foreach (var task in parentTasksToFinish) {
        task.State = TaskState.Finished;
        task.StateLogs.Add(new StateLog {
          State = task.State,
          SlaveId = null,
          UserId = null,
          Exception = string.Empty,
          DateTime = DateTime.Now
        });
      }
    }

    /// <summary>
    /// Looks for task which have not sent heartbeats for some time and reschedules them for calculation
    /// </summary>
    private void SetTimeoutTasksWaiting(IPersistenceManager pm) {
      var taskDao = pm.TaskDao;
      var tasks = taskDao.GetAll().Where(x => (x.State == TaskState.Calculating && (DateTime.Now - x.LastHeartbeat) > Properties.Settings.Default.CalculatingJobHeartbeatTimeout)
                                           || (x.State == TaskState.Transferring && (DateTime.Now - x.LastHeartbeat) > Properties.Settings.Default.TransferringJobHeartbeatTimeout));
      foreach (var task in tasks) {
        task.State = TaskState.Waiting;
        task.StateLogs.Add(new StateLog {
          State = task.State,
          SlaveId = null,
          UserId = null,
          Exception = SlaveTimeout,
          DateTime = DateTime.Now
        });
        task.Command = null;
      }
    }

    /// <summary>
    /// Searches for slaves that are disposable and deletes them if they were offline for too long
    /// </summary>
    private void DeleteObsoleteSlaves(IPersistenceManager pm) {
      var slaveDao = pm.SlaveDao;
      var downtimeDao = pm.DowntimeDao;
      var slaveIds = slaveDao.GetAll()
        .Where(x => x.IsDisposable.GetValueOrDefault()
                 && x.SlaveState == SlaveState.Offline
                 && (DateTime.Now - x.LastHeartbeat) > Properties.Settings.Default.SweepInterval)
        .Select(x => x.ResourceId)
        .ToList();
      foreach (var id in slaveIds) {
        bool downtimesAvailable = downtimeDao.GetByResourceId(id).Any();
        if (!downtimesAvailable) {
          slaveDao.Delete(id);
        }
      }
    }
  }
}
