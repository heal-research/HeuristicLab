#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private static readonly TaskState[] CompletedStates = { TaskState.Finished, TaskState.Aborted, TaskState.Failed };

    private IPersistenceManager PersistenceManager {
      get { return ServiceLocator.Instance.PersistenceManager; }
    }

    public void Cleanup() {
      Console.WriteLine("started cleanup");
      var pm = PersistenceManager;

      // preemptiv delete obsolete entities
      // speeds up job deletion
      BatchDelete((p, s) => p.StateLogDao.DeleteObsolete(s), 100, 100, true, pm, "DeleteObsoleteStateLogs");
      BatchDelete((p, s) => p.TaskDataDao.DeleteObsolete(s), 100, 20, true, pm, "DeleteObsoleteTaskData");
      BatchDelete((p, s) => p.TaskDao.DeleteObsolete(s), 100, 20, false, pm, "DeleteObsoleteTasks");
      BatchDelete((p, s) => p.JobDao.DeleteByState(JobState.DeletionPending, s), 100, 20, true, pm, "DeleteObsoleteJobs");

      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: SetTimeoutSlavesOffline");
      Console.WriteLine("5");
      pm.UseTransactionAndSubmit(() => { SetTimeoutSlavesOffline(pm); });
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: SetTimeoutTasksWaiting");
      Console.WriteLine("6");
      pm.UseTransactionAndSubmit(() => { SetTimeoutTasksWaiting(pm); });
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: DeleteObsoleteSlaves");
      Console.WriteLine("7");
      pm.UseTransactionAndSubmit(() => { DeleteObsoleteSlaves(pm); });
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: AbortObsoleteTasks");
      Console.WriteLine("8");
      pm.UseTransactionAndSubmit(() => { AbortObsoleteTasks(pm); });
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: FinishParentTasks");
      Console.WriteLine("9");
      pm.UseTransactionAndSubmit(() => { FinishParentTasks(pm); });
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log("HiveJanitor: DONE");
      Console.WriteLine("10");
    }

    private void BatchDelete(
      Func<IPersistenceManager, int, int> deletionFunc,
      int batchSize,
      int maxCalls,
      bool limitIsBatchSize,
      IPersistenceManager pm,
      string logMessage
    ) {
      int totalDeleted = 0;
      while (maxCalls > 0) {
        maxCalls--;
        LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log($"HiveJanitor: {logMessage}");
        Console.WriteLine($"HiveJanitor: {logMessage}");
        var deleted = pm.UseTransactionAndSubmit(() => { return deletionFunc(pm, batchSize); });
        LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log($"HiveJanitor: {logMessage} DONE (deleted {deleted}, {maxCalls} calls left)");
        Console.WriteLine($"HiveJanitor: {logMessage} DONE (deleted {deleted}, {maxCalls} calls left)");
        totalDeleted += deleted;
        if (limitIsBatchSize && deleted < batchSize || deleted <= 0) return;
      }
      LogFactory.GetLogger(typeof(HiveJanitor).Namespace).Log($"HiveJanitor: Possible rows left to delete (total deleted: {totalDeleted}).");
      Console.WriteLine($"HiveJanitor: Possible rows left to delete (total deleted: {totalDeleted}).");
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

    /// <summary>
    /// Aborts tasks whose jobs have already been marked for deletion
    /// </summary>
    /// <param name="pm"></param>
    private void AbortObsoleteTasks(IPersistenceManager pm) {
      var jobDao = pm.JobDao;
      var taskDao = pm.TaskDao;

      var obsoleteTasks = (from jobId in jobDao.GetJobIdsByState(JobState.StatisticsPending)
                           join task in taskDao.GetAll() on jobId equals task.JobId
                           where !CompletedStates.Contains(task.State) && task.Command == null
                           select task).ToList();

      foreach (var t in obsoleteTasks) {
        t.State = TaskState.Aborted;
      }
    }
  }
}
