#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive {

  [Item("Hive Task", "Represents a hive task.")]
  [StorableClass]
  public class HiveTask : NamedItem, IItemTree<HiveTask>, IDisposable {
    protected static object locker = new object();
    protected ReaderWriterLockSlim childHiveTasksLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    protected ReaderWriterLockSlim itemTaskLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public override Image ItemImage {
      get {
        if (task.Id == Guid.Empty) { // not yet uploaded
          return HeuristicLab.Common.Resources.VSImageLibrary.Event;
        } else {
          if (task.State == TaskState.Waiting) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePrepared;
          else if (task.State == TaskState.Calculating) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStarted;
          else if (task.State == TaskState.Transferring) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStarted;
          else if (task.State == TaskState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePaused;
          else if (task.State == TaskState.Aborted) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStopped;
          else if (task.State == TaskState.Failed) return HeuristicLab.Common.Resources.VSImageLibrary.Error;
          else if (task.State == TaskState.Finished) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStopped;
          else return base.ItemImage;
        }
      }
    }

    [Storable]
    protected Task task;
    public Task Task {
      get { return task; }
      set {
        if (task != value) {
          DeregisterTaskEvents();
          task = value;
          RegisterTaskEvents();
          IsFinishedTaskDownloaded = false;
          OnTaskChanged();
          OnToStringChanged();
          OnItemImageChanged();
        }
      }
    }

    [Storable]
    protected ItemTask itemTask;
    public ItemTask ItemTask {
      get { return itemTask; }
      set {
        if (itemTask != null && syncTasksWithOptimizers) {
          childHiveTasksLock.EnterWriteLock();
          try {
            childHiveTasks.Clear();
          }
          finally { childHiveTasksLock.ExitWriteLock(); }
        }
        if (itemTask != value) {
          itemTaskLock.EnterWriteLock();
          try {
            DeregisterItemTaskEvents();
            itemTask = value;
            RegisterItemTaskEvents();
          }
          finally { itemTaskLock.ExitWriteLock(); }
          OnItemTaskChanged();
          IsFinishedTaskDownloaded = true;
        }

      }
    }

    // task downloaded since last status change
    [Storable]
    private bool isFinishedTaskDownloaded = false;
    public bool IsFinishedTaskDownloaded {
      get { return isFinishedTaskDownloaded; }
      set {
        if (value != isFinishedTaskDownloaded) {
          this.isFinishedTaskDownloaded = value;
          OnIsFinishedJobDownloadedChanged();
        }
      }
    }

    public bool IsDownloading { get; set; }

    // if true, all control buttons should be enabled. otherwise disabled
    private bool isControllable = true;
    public bool IsControllable {
      get { return isControllable; }
      set {
        if (value != isControllable) {
          isControllable = value;
          OnIsControllableChanged();
          childHiveTasksLock.EnterReadLock();
          try {
            foreach (var hiveJob in childHiveTasks) {
              hiveJob.IsControllable = value;
            }
          }
          finally {
            childHiveTasksLock.ExitReadLock();
          }
        }
      }
    }

    [Storable]
    protected ItemList<HiveTask> childHiveTasks;
    public virtual ReadOnlyItemList<HiveTask> ChildHiveTasks {
      get {
        childHiveTasksLock.EnterReadLock();
        try {
          return childHiveTasks.AsReadOnly();
        }
        finally { childHiveTasksLock.ExitReadLock(); }
      }
    }

    [Storable]
    protected bool syncTasksWithOptimizers = true;

    public StateLogList StateLog {
      get {
        var list = new StateLogList(this.task.StateLog);
        list.ForEach(s => { s.TaskName = itemTask.Name; });
        return list;
      }
    }

    public StateLogListList ChildStateLogList {
      get { return new StateLogListList(this.childHiveTasks.Select(x => x.StateLog)); }
    }

    #region Constructors and Cloning
    [StorableConstructor]
    protected HiveTask(bool deserializing) { }

    public HiveTask()
      : base() {
      Name = "Hive Task";
      this.Task = new Task() { CoresNeeded = 1, MemoryNeeded = 128 };
      task.State = TaskState.Offline;
      this.childHiveTasks = new ItemList<HiveTask>();
      syncTasksWithOptimizers = true;
      RegisterChildHiveTasksEvents();
    }

    public HiveTask(ItemTask itemTask, bool autoCreateChildHiveTasks)
      : this() {
      this.syncTasksWithOptimizers = autoCreateChildHiveTasks;
      this.ItemTask = itemTask;
      this.syncTasksWithOptimizers = true;
    }

    public HiveTask(Task task, TaskData taskData, bool autoCreateChildHiveTasks) {
      this.syncTasksWithOptimizers = autoCreateChildHiveTasks;
      this.Task = task;
      try {
        this.ItemTask = PersistenceUtil.Deserialize<ItemTask>(taskData.Data);
      }
      catch {
        this.ItemTask = null;
      }
      this.childHiveTasks = new ItemList<HiveTask>();
      this.syncTasksWithOptimizers = true;
      RegisterChildHiveTasksEvents();
    }

    protected HiveTask(HiveTask original, Cloner cloner)
      : base(original, cloner) {
      this.Task = cloner.Clone(original.task);
      this.ItemTask = cloner.Clone(original.ItemTask);
      original.childHiveTasksLock.EnterReadLock();
      try {
        this.childHiveTasks = cloner.Clone(original.childHiveTasks);
      }
      finally { original.childHiveTasksLock.ExitReadLock(); }
      this.syncTasksWithOptimizers = original.syncTasksWithOptimizers;
      this.isFinishedTaskDownloaded = original.isFinishedTaskDownloaded;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HiveTask(this, cloner);
    }
    #endregion

    protected virtual void UpdateChildHiveTasks() { }

    protected virtual void RegisterItemTaskEvents() {
      if (ItemTask != null) {
        ItemTask.ComputeInParallelChanged += new EventHandler(ItemJob_ComputeInParallelChanged);
        ItemTask.ToStringChanged += new EventHandler(ItemJob_ToStringChanged);
      }
    }
    protected virtual void DeregisterItemTaskEvents() {
      if (ItemTask != null) {
        ItemTask.ComputeInParallelChanged -= new EventHandler(ItemJob_ComputeInParallelChanged);
        ItemTask.ToStringChanged -= new EventHandler(ItemJob_ToStringChanged);
      }
    }

    protected virtual void RegisterChildHiveTasksEvents() {
      this.childHiveTasks.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnItemsAdded);
      this.childHiveTasks.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnItemsRemoved);
      this.childHiveTasks.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnCollectionReset);
    }
    protected virtual void DeregisterChildHiveTasksEvents() {
      this.childHiveTasks.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnItemsAdded);
      this.childHiveTasks.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnItemsRemoved);
      this.childHiveTasks.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<HiveTask>>(OnCollectionReset);
    }

    protected virtual void ItemJob_ToStringChanged(object sender, EventArgs e) {
      this.OnToStringChanged();
    }

    protected virtual void ItemJob_ComputeInParallelChanged(object sender, EventArgs e) {
      if (ItemTask != null && syncTasksWithOptimizers) {
        this.UpdateChildHiveTasks();
      }
    }

    public virtual void AddChildHiveTask(HiveTask hiveTask) {
      childHiveTasksLock.EnterWriteLock();
      try {
        this.childHiveTasks.Add(hiveTask);
      }
      finally { childHiveTasksLock.ExitWriteLock(); }
    }

    public override string ToString() {
      if (itemTask != null && itemTask.Item != null) {
        return itemTask.ToString();
      } else {
        return Task.Id.ToString();
      }
    }

    public virtual void UpdateFromLightweightJob(LightweightTask lightweightJob) {
      if (lightweightJob != null) {
        task.Id = lightweightJob.Id;
        task.ParentTaskId = lightweightJob.ParentTaskId;
        task.ExecutionTime = lightweightJob.ExecutionTime;
        task.State = lightweightJob.State;
        task.StateLog = new List<StateLog>(lightweightJob.StateLog);
        task.Command = lightweightJob.Command;

        OnTaskStateChanged();
        OnToStringChanged();
        OnItemImageChanged();
        OnStateLogChanged();
      }
    }

    /// <summary>
    /// Creates a TaskData object containing the Task and the IJob-Object as byte[]
    /// </summary>
    /// <param name="withoutChildOptimizers">
    ///   if true the Child-Optimizers will not be serialized (if the task contains an Experiment)
    /// </param>
    public virtual TaskData GetAsTaskData(bool withoutChildOptimizers, out List<IPluginDescription> plugins) {
      if (ItemTask == null) {
        plugins = new List<IPluginDescription>();
        return null;
      }

      IEnumerable<Type> usedTypes;
      byte[] taskByteArray = PersistenceUtil.Serialize(ItemTask, out usedTypes);
      TaskData taskData = new TaskData() { TaskId = task.Id, Data = taskByteArray };
      plugins = PluginUtil.GetPluginsForTask(usedTypes, ItemTask);
      return taskData;
    }

    #region Event Handler
    public event EventHandler TaskChanged;
    private void OnTaskChanged() {
      EventHandler handler = TaskChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler TaskStateChanged;
    private void OnTaskStateChanged() {
      EventHandler handler = TaskStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ItemTaskChanged;
    private void OnItemTaskChanged() {
      ItemJob_ComputeInParallelChanged(this, EventArgs.Empty);
      var handler = ItemTaskChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler IsFinishedJobDownloadedChanged;
    private void OnIsFinishedJobDownloadedChanged() {
      var handler = IsFinishedJobDownloadedChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler StateLogChanged;
    private void OnStateLogChanged() {
      var handler = StateLogChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler IsControllableChanged;
    private void OnIsControllableChanged() {
      var handler = IsControllableChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void RegisterTaskEvents() {
      if (task != null)
        task.PropertyChanged += new PropertyChangedEventHandler(task_PropertyChanged);
    }

    private void DeregisterTaskEvents() {
      if (task != null)
        task.PropertyChanged += new PropertyChangedEventHandler(task_PropertyChanged);
    }

    private void task_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "State") {
        IsFinishedTaskDownloaded = false;
      }
      if (e.PropertyName == "Priority" && Task != null) {
        foreach (var task in childHiveTasks) {
          task.Task.Priority = Task.Priority;
        }
      }
    }
    #endregion

    /// <summary>
    /// Returns a list of HiveTasks including this and all its child-jobs recursively
    /// </summary>
    public IEnumerable<HiveTask> GetAllHiveTasks() {
      childHiveTasksLock.EnterReadLock();
      try {
        var tasks = new List<HiveTask>();
        tasks.Add(this);
        foreach (HiveTask child in this.childHiveTasks) {
          tasks.AddRange(child.GetAllHiveTasks());
        }
        return tasks;
      }
      finally { childHiveTasksLock.ExitReadLock(); }
    }

    public HiveTask GetParentByJobId(Guid taskId) {
      childHiveTasksLock.EnterReadLock();
      try {
        if (this.ChildHiveTasks.SingleOrDefault(j => j.task.Id == taskId) != null)
          return this;
        foreach (HiveTask child in this.childHiveTasks) {
          HiveTask result = child.GetParentByJobId(taskId);
          if (result != null)
            return result;
        }
        return null;
      }
      finally { childHiveTasksLock.ExitWriteLock(); }
    }

    /// <summary>
    /// Searches for an HiveTask object with the correct taskId recursively
    /// </summary>
    public HiveTask GetHiveTaskByTaskId(Guid jobId) {
      if (this.Task.Id == jobId) {
        return this;
      } else {
        childHiveTasksLock.EnterReadLock();
        try {
          foreach (HiveTask child in this.childHiveTasks) {
            HiveTask result = child.GetHiveTaskByTaskId(jobId);
            if (result != null)
              return result;
          }
        }
        finally { childHiveTasksLock.ExitReadLock(); }
      }
      return null;
    }

    public void RemoveByTaskId(Guid taskId) {
      childHiveTasksLock.EnterWriteLock();
      try {
        IEnumerable<HiveTask> tasks = childHiveTasks.Where(j => j.Task.Id == taskId).ToList();
        foreach (HiveTask t in tasks) {
          this.childHiveTasks.Remove(t);
        }
        foreach (HiveTask child in childHiveTasks) {
          child.RemoveByTaskId(taskId);
        }
      }
      finally { childHiveTasksLock.ExitWriteLock(); }
    }

    public IEnumerable<IItemTree<HiveTask>> GetChildItems() {
      return this.ChildHiveTasks;
    }

    #region INotifyObservableCollectionItemsChanged<IItemTree> Members

    public event CollectionItemsChangedEventHandler<IItemTree<HiveTask>> CollectionReset;
    private void OnCollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<HiveTask>> e) {
      foreach (var item in e.Items) {
        item.Value.StateLogChanged -= new EventHandler(ChildHiveJob_StateLogChanged);
      }
      var handler = CollectionReset;
      if (handler != null) handler(this, ToCollectionItemsChangedEventArgs(e));
    }

    public event CollectionItemsChangedEventHandler<IItemTree<HiveTask>> ItemsAdded;
    private void OnItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<HiveTask>> e) {
      foreach (var item in e.Items) {
        item.Value.StateLogChanged += new EventHandler(ChildHiveJob_StateLogChanged);
        item.Value.IsControllable = this.IsControllable;
      }
      var handler = ItemsAdded;
      if (handler != null) handler(this, ToCollectionItemsChangedEventArgs(e));
    }

    public event CollectionItemsChangedEventHandler<IItemTree<HiveTask>> ItemsRemoved;
    private void OnItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<HiveTask>> e) {
      foreach (var item in e.Items) {
        item.Value.StateLogChanged -= new EventHandler(ChildHiveJob_StateLogChanged);
      }
      var handler = ItemsRemoved;
      if (handler != null) handler(this, ToCollectionItemsChangedEventArgs(e));
    }

    private static CollectionItemsChangedEventArgs<IItemTree<HiveTask>> ToCollectionItemsChangedEventArgs(CollectionItemsChangedEventArgs<IndexedItem<HiveTask>> e) {
      return new CollectionItemsChangedEventArgs<IItemTree<HiveTask>>(e.Items.Select(x => x.Value), e.OldItems == null ? null : e.OldItems.Select(x => x.Value));
    }

    private void ChildHiveJob_StateLogChanged(object sender, EventArgs e) {
      OnStateLogChanged();
    }
    #endregion

    public void Pause() {
      if (this.Task.IsParentTask) {
        childHiveTasksLock.EnterReadLock();
        try {
          foreach (var child in childHiveTasks) {
            HiveServiceLocator.Instance.CallHiveService(s => s.PauseTask(child.task.Id));
          }
        }
        finally { childHiveTasksLock.ExitReadLock(); }
      } else {
        HiveServiceLocator.Instance.CallHiveService(s => s.PauseTask(this.task.Id));
      }
    }

    public void Stop() {
      if (this.Task.IsParentTask) {
        childHiveTasksLock.EnterReadLock();
        try {
          foreach (var child in childHiveTasks) {
            HiveServiceLocator.Instance.CallHiveService(s => s.StopTask(child.task.Id));
          }
        }
        finally { childHiveTasksLock.ExitReadLock(); }
      } else {
        HiveServiceLocator.Instance.CallHiveService(s => s.StopTask(this.task.Id));
      }
    }

    public void Restart() {
      HiveServiceLocator.Instance.CallHiveService(service => {
        TaskData taskData = new TaskData();
        taskData.TaskId = this.task.Id;
        taskData.Data = PersistenceUtil.Serialize(this.itemTask);
        service.UpdateTaskData(this.Task, taskData);
        service.RestartTask(this.task.Id);
        Task task = service.GetTask(this.task.Id);
        this.task.LastTaskDataUpdate = task.LastTaskDataUpdate;
      });
    }

    public ICollection<IItemTreeNodeAction<HiveTask>> Actions {
      get {
        return new List<IItemTreeNodeAction<HiveTask>>();
      }
    }

    public virtual void IntegrateChild(ItemTask task, Guid childTaskId) { }

    /// <summary>
    /// Delete ItemTask
    /// </summary>
    public virtual void ClearData() {
      this.ItemTask.Item = null;
    }

    public void Dispose() {
      DeregisterChildHiveTasksEvents();
      DeregisterTaskEvents();
      DeregisterItemTaskEvents();
      childHiveTasksLock.Dispose();
      itemTaskLock.Dispose();
      ClearData();
    }
  }

  [Item("Hive Task", "Represents a hive task.")]
  [StorableClass]
  public class HiveTask<T> : HiveTask where T : ItemTask {

    public new T ItemTask {
      get { return (T)base.ItemTask; }
      set { base.ItemTask = value; }
    }

    #region Constructors and Cloning
    public HiveTask() : base() { }
    [StorableConstructor]
    protected HiveTask(bool deserializing) { }
    public HiveTask(T itemTask) : base(itemTask, true) { }
    protected HiveTask(HiveTask<T> original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HiveTask<T>(this, cloner);
    }
    #endregion
  }
}