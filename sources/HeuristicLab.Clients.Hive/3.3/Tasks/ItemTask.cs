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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Hive;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Clients.Hive {
  [Item("Item Task", "Represents a executable hive task which contains a HeuristicLab Item.")]
  [StorableClass]
  public abstract class ItemTask : NamedItem, ITask {
    public virtual bool IsParallelizable {
      get { return true; }
    }

    [Storable]
    protected IItem item;
    public IItem Item {
      get { return item; }
      set {
        if (value != item) {
          if (item != null) DeregisterItemEvents();
          item = value;
          if (item != null) RegisterItemEvents();
          OnItemChanged();
        }
      }
    }

    [Storable]
    protected bool computeInParallel;
    public bool ComputeInParallel {
      get { return computeInParallel; }
      set {
        if (computeInParallel != value) {
          computeInParallel = value;
          OnComputeInParallelChanged();
        }
      }
    }

    #region Constructors and Cloning
    public ItemTask() { }

    [StorableConstructor]
    protected ItemTask(bool deserializing) { }
    protected ItemTask(ItemTask original, Cloner cloner)
      : base(original, cloner) {
      this.ComputeInParallel = original.ComputeInParallel;
      this.Item = cloner.Clone(original.Item);
    }

    [StorableHook(HookType.AfterDeserialization)]
    protected virtual void AfterDeserialization() {
      RegisterItemEvents();
    }
    #endregion

    #region Item Events
    protected virtual void RegisterItemEvents() {
      item.ItemImageChanged += new EventHandler(item_ItemImageChanged);
      item.ToStringChanged += new EventHandler(item_ToStringChanged);
    }

    protected virtual void DeregisterItemEvents() {
      item.ItemImageChanged -= new EventHandler(item_ItemImageChanged);
      item.ToStringChanged -= new EventHandler(item_ToStringChanged);
    }

    protected void item_ToStringChanged(object sender, EventArgs e) {
      this.OnToStringChanged();
    }
    protected void item_ItemImageChanged(object sender, EventArgs e) {
      this.OnItemImageChanged();
    }

    #endregion

    #region ITask Members

    public abstract ExecutionState ExecutionState { get; }

    public abstract TimeSpan ExecutionTime { get; }

    public abstract void Prepare();

    public abstract void Start();

    public abstract void Pause();

    public abstract void Stop();

    public event EventHandler TaskStarted;
    protected virtual void OnTaskStarted() {
      EventHandler handler = TaskStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler TaskStopped;
    protected virtual void OnTaskStopped() {
      EventHandler handler = TaskStopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler TaskPaused;
    protected virtual void OnTaskPaused() {
      EventHandler handler = TaskPaused;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler TaskFailed;
    protected virtual void OnTaskFailed(EventArgs<Exception> e) {
      EventHandler handler = TaskFailed;
      if (handler != null) handler(this, e);
    }

    public event EventHandler ComputeInParallelChanged;
    protected virtual void OnComputeInParallelChanged() {
      EventHandler handler = ComputeInParallelChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ItemChanged;
    protected virtual void OnItemChanged() {
      EventHandler handler = ItemChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region INamedItem Members
    public abstract bool CanChangeDescription { get; }

    public abstract bool CanChangeName { get; }

    public abstract string Description { get; set; }

    public abstract string Name { get; set; }
    #endregion

    #region Events
    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      var handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      var handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<CancelEventArgs<string>> NameChanging;
    protected virtual void OnNameChanging(string value, bool cancel) {
      var handler = NameChanging;
      if (handler != null) handler(this, new CancelEventArgs<string>(value, cancel));
    }
    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionStateChanged;
    protected virtual void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region IItem Members
    public virtual string ItemDescription {
      get { return item.ItemDescription; }
    }

    public virtual Image ItemImage {
      get { return item.ItemImage; }
    }

    public virtual string ItemName {
      get { return item.ItemName; }
    }

    public virtual Version ItemVersion {
      get { return item.ItemVersion; }
    }
    #endregion

    public override string ToString() {
      return Name;
    }
  }
}
