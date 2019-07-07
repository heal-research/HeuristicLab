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
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive.SlaveCore {

  /// <summary>
  /// Holds a list of slave tasks and manages access to this list. 
  /// Forwards events from SlaveTask and forwards commands to SlaveTask. 
  /// Periodically sends task data to the server to avoid loss of progress when the slave crashes.
  /// </summary>
  public class TaskManager {
    private static readonly ReaderWriterLockSlim slaveTasksLocker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private readonly Dictionary<Guid, Tuple<SlaveTask, DateTime>> slaveTasks;
    private readonly ILog log;
    private readonly PluginManager pluginManager;
    private readonly CancellationTokenSource cts;
    private readonly CancellationToken ct;
    private readonly AutoResetEvent waitHandle;
    private readonly TimeSpan checkpointInterval;
    private readonly TimeSpan checkpointCheckInterval;

    public int TaskCount {
      get {
        slaveTasksLocker.EnterReadLock();
        try {
          return slaveTasks.Count;
        }
        finally { slaveTasksLocker.ExitReadLock(); }
      }
    }

    public Guid[] TaskIds {
      get {
        slaveTasksLocker.EnterReadLock();
        try {
          return slaveTasks.Keys.ToArray();
        }
        finally { slaveTasksLocker.ExitReadLock(); }
      }
    }

    public TaskManager(PluginManager pluginCache, ILog log) {
      this.pluginManager = pluginCache;
      this.log = log;
      this.slaveTasks = new Dictionary<Guid, Tuple<SlaveTask, DateTime>>();

      cts = new CancellationTokenSource();
      ct = cts.Token;
      waitHandle = new AutoResetEvent(true);
      checkpointInterval = Settings.Default.CheckpointInterval;
      checkpointCheckInterval = Settings.Default.CheckpointCheckInterval;

      System.Threading.Tasks.Task.Factory.StartNew(Checkpointing, ct);
    }

    #region Checkpointing
    private void Checkpointing() {
      while (!ct.IsCancellationRequested) {
        slaveTasksLocker.EnterUpgradeableReadLock();
        try {
          foreach (var entry in slaveTasks) {
            if (DateTime.Now - entry.Value.Item2 > checkpointInterval)
              PauseTaskAsync(entry.Key);
          }
        } finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
        waitHandle.WaitOne(checkpointCheckInterval);
      }
    }

    public void StopCheckpointing() {
      cts.Cancel();
      waitHandle.Set();
      waitHandle.Close();
    }
    #endregion

    #region Task Control methods
    public void StartTaskAsync(Task task, TaskData taskData) {
      SlaveTask slaveTask = null;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        if (slaveTasks.ContainsKey(task.Id)) {
          SlaveStatusInfo.IncrementTasksFailed();
          throw new TaskAlreadyRunningException(task.Id);
        } else {
          slaveTask = new SlaveTask(pluginManager, task.CoresNeeded, log);
          AddSlaveTask(task, slaveTask);
          SlaveStatusInfo.IncrementTasksFetched();
        }
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }

      if (slaveTask != null) {
        try {
          slaveTask.StartJobAsync(task, taskData);
        }
        catch (Exception) {
          RemoveSlaveTask(task.Id, slaveTask); // clean up and rethrow
          slaveTask.DisposeAppDomain();
          throw;
        }
      }
    }

    public void PauseTaskAsync(Guid taskId) {
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        if (!slaveTasks.ContainsKey(taskId)) throw new TaskNotRunningException(taskId);
        SlaveTask slaveTask = slaveTasks[taskId].Item1;
        slaveTask.PauseTask();
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
    }

    public void StopTaskAsync(Guid taskId) {
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        if (!slaveTasks.ContainsKey(taskId)) throw new TaskNotRunningException(taskId);
        SlaveTask slaveTask = slaveTasks[taskId].Item1;
        slaveTask.StopTask();
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
    }

    public void AbortTask(Guid taskId) {
      SlaveTask slaveTask = null;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        if (!slaveTasks.ContainsKey(taskId)) throw new TaskNotRunningException(taskId);
        slaveTask = slaveTasks[taskId].Item1;
        if (!slaveTask.IsPrepared) throw new AppDomainNotCreatedException();
        RemoveSlaveTask(taskId, slaveTask);
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
      slaveTask.DisposeAppDomain();
      SlaveStatusInfo.IncrementTasksAborted();
      OnTaskAborted(slaveTask);
    }

    public void PauseAllTasksAsync() {
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        foreach (var slaveTask in slaveTasks.Values) {
          slaveTask.Item1.PauseTask();
        }
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
    }

    public void StopAllTasksAsync() {
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        foreach (var slaveTask in slaveTasks.Values) {
          slaveTask.Item1.StopTask();
        }
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
    }

    public void AbortAllTasks() {
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        foreach (var slaveTask in slaveTasks.Values.ToArray()) {
          AbortTask(slaveTask.Item1.TaskId);
        }
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }
    }
    #endregion

    #region Add/Remove SlaveTask
    private void AddSlaveTask(Task task, SlaveTask slaveTask) {
      slaveTasksLocker.EnterWriteLock();
      try {
        slaveTasks.Add(task.Id, Tuple.Create(slaveTask, DateTime.Now));
        RegisterSlaveTaskEvents(slaveTask);
      }
      finally { slaveTasksLocker.ExitWriteLock(); }
    }

    private void RemoveSlaveTask(Guid taskId, SlaveTask slaveTask) {
      slaveTasksLocker.EnterWriteLock();
      try {
        slaveTasks.Remove(taskId);
        DeregisterSlaveTaskEvents(slaveTask);
      }
      finally { slaveTasksLocker.ExitWriteLock(); }
    }
    #endregion

    #region SlaveTask Events
    private void RegisterSlaveTaskEvents(SlaveTask slaveTask) {
      slaveTask.TaskStarted += new EventHandler<EventArgs<Guid>>(slaveTask_TaskStarted);
      slaveTask.TaskPaused += new EventHandler<EventArgs<Guid>>(slaveTask_TaskPaused);
      slaveTask.TaskStopped += new EventHandler<EventArgs<Guid>>(slaveTask_TaskStopped);
      slaveTask.TaskFailed += new EventHandler<EventArgs<Guid, Exception>>(slaveTask_TaskFailed);
    }

    private void DeregisterSlaveTaskEvents(SlaveTask slaveTask) {
      slaveTask.TaskStarted -= new EventHandler<EventArgs<Guid>>(slaveTask_TaskStarted);
      slaveTask.TaskPaused -= new EventHandler<EventArgs<Guid>>(slaveTask_TaskPaused);
      slaveTask.TaskStopped -= new EventHandler<EventArgs<Guid>>(slaveTask_TaskStopped);
      slaveTask.TaskFailed -= new EventHandler<EventArgs<Guid, Exception>>(slaveTask_TaskFailed);
    }

    private void slaveTask_TaskStarted(object sender, EventArgs<Guid> e) {
      SlaveTask slaveTask;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        slaveTask = slaveTasks[e.Value].Item1;
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }

      SlaveStatusInfo.IncrementTasksStarted();
      OnTaskStarted(slaveTask);
    }

    private void slaveTask_TaskPaused(object sender, EventArgs<Guid> e) {
      SlaveTask slaveTask;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        slaveTask = slaveTasks[e.Value].Item1;
        RemoveSlaveTask(e.Value, slaveTask);
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }

      TaskData taskData = null;
      try {
        taskData = slaveTask.GetTaskData();
        SlaveStatusInfo.IncrementTasksFinished();
        OnTaskPaused(slaveTask, taskData);
      }
      catch (Exception ex) {
        RemoveSlaveTask(e.Value, slaveTask);
        SlaveStatusInfo.IncrementTasksFailed();
        OnTaskFailed(slaveTask, taskData, ex);
      }
    }

    private void slaveTask_TaskStopped(object sender, EventArgs<Guid> e) {
      SlaveTask slaveTask;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        slaveTask = slaveTasks[e.Value].Item1;
        RemoveSlaveTask(e.Value, slaveTask);
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }

      TaskData taskData = null;
      try {
        taskData = slaveTask.GetTaskData();
        SlaveStatusInfo.IncrementTasksFinished();
        OnTaskStopped(slaveTask, taskData);
      }
      catch (Exception ex) {
        RemoveSlaveTask(e.Value, slaveTask);
        SlaveStatusInfo.IncrementTasksFailed();
        OnTaskFailed(slaveTask, taskData, ex);
      }
    }

    private void slaveTask_TaskFailed(object sender, EventArgs<Guid, Exception> e) {
      SlaveTask slaveTask;
      slaveTasksLocker.EnterUpgradeableReadLock();
      try {
        slaveTask = slaveTasks[e.Value].Item1;
        RemoveSlaveTask(e.Value, slaveTask);
      }
      finally { slaveTasksLocker.ExitUpgradeableReadLock(); }

      TaskData taskData = null;
      try {
        taskData = slaveTask.GetTaskData();
      }
      catch { /* taskData will be null */ }
      SlaveStatusInfo.IncrementTasksFailed();
      OnTaskFailed(slaveTask, taskData, e.Value2);
    }
    #endregion

    #region EventHandler
    public event EventHandler<EventArgs<SlaveTask>> TaskStarted;
    private void OnTaskStarted(SlaveTask slaveTask) {
      var handler = TaskStarted;
      if (handler != null) handler(this, new EventArgs<SlaveTask>(slaveTask));
    }

    public event EventHandler<EventArgs<SlaveTask, TaskData>> TaskStopped;
    private void OnTaskStopped(SlaveTask slaveTask, TaskData taskData) {
      var handler = TaskStopped;
      if (handler != null) handler(this, new EventArgs<SlaveTask, TaskData>(slaveTask, taskData));
    }

    public event EventHandler<EventArgs<SlaveTask, TaskData>> TaskPaused;
    private void OnTaskPaused(SlaveTask slaveTask, TaskData taskData) {
      var handler = TaskPaused;
      if (handler != null) handler(this, new EventArgs<SlaveTask, TaskData>(slaveTask, taskData));
    }

    public event EventHandler<EventArgs<Tuple<SlaveTask, TaskData, Exception>>> TaskFailed;
    private void OnTaskFailed(SlaveTask slaveTask, TaskData taskData, Exception exception) {
      var handler = TaskFailed;
      if (handler != null) handler(this, new EventArgs<Tuple<SlaveTask, TaskData, Exception>>(new Tuple<SlaveTask, TaskData, Exception>(slaveTask, taskData, exception)));
    }

    public event EventHandler<EventArgs<SlaveTask>> TaskAborted;
    private void OnTaskAborted(SlaveTask slaveTask) {
      var handler = TaskAborted;
      if (handler != null) handler(this, new EventArgs<SlaveTask>(slaveTask));
    }
    #endregion

    public Dictionary<Guid, TimeSpan> GetExecutionTimes() {
      slaveTasksLocker.EnterReadLock();
      try {
        return slaveTasks.ToDictionary(x => x.Key, x => x.Value.Item1.ExecutionTime);
      }
      finally { slaveTasksLocker.ExitReadLock(); }
    }
  }
}
