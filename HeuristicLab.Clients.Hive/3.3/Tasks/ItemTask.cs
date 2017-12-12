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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Hive;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive {
  [Item("Item Task", "Represents a executable hive task which contains a HeuristicLab Item.")]
  [StorableClass]
  public abstract class ItemTask : NamedItem, ITask {
    public virtual HiveTask CreateHiveTask() {
      return new HiveTask(this, true);
    }

    public virtual bool IsParallelizable {
      get { return true; }
      set { }
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
    public ItemTask(IItem item) {
      Item = item;
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
    public abstract new bool CanChangeDescription { get; }

    public abstract new bool CanChangeName { get; }

    public abstract new string Description { get; set; }

    public abstract new string Name { get; set; }
    #endregion

    #region Events
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
    public override string ItemDescription {
      get {
        if (item == null)
          return string.Empty;
        else
          return item.ItemDescription;
      }
    }

    public override Image ItemImage {
      get {
        if (item == null)
          return HeuristicLab.Common.Resources.VSImageLibrary.Class;
        else
          return item.ItemImage;
      }
    }

    public override string ItemName {
      get {
        if (item == null)
          return string.Empty;
        else
          return item.ItemName;
      }
    }

    public virtual new Version ItemVersion {
      get {
        if (item == null)
          return ItemAttribute.GetVersion(this.GetType());
        else
          return item.ItemVersion;
      }
    }
    #endregion

    public override string ToString() {
      return Name;
    }

    #region Helpers
    public static bool IsTypeSupported(Type itemType) {
      var supportedHiveTaskTypes = ApplicationManager.Manager.GetTypes(typeof(ItemTask))
          .Select(t => t.GetProperties().Single(x => x.Name == "Item" && x.PropertyType != typeof(IItem)).PropertyType);
      return supportedHiveTaskTypes.Any(x => x.IsAssignableFrom(itemType));
    }

    public static Type GetHiveTaskType(Type itemType) {
      if (!IsTypeSupported(itemType)) throw new Exception("Item " + itemType + " is not supported for Hive.");

      var typeHiveTaskMap = ApplicationManager.Manager.GetTypes(typeof(ItemTask))
          .Select(t => new Tuple<Type, Type>(t.GetProperties().Single(x => x.Name == "Item" && x.PropertyType != typeof(IItem)).PropertyType, t));

      return typeHiveTaskMap.Single(x => x.Item1.IsAssignableFrom(itemType)).Item2;
    }

    public static ItemTask GetItemTaskForItem(IItem item) {
      Type itemType = item.GetType();
      Type hiveTaskType = GetHiveTaskType(itemType);
      return Activator.CreateInstance(hiveTaskType, new object[] { item }) as ItemTask;
    }
    #endregion
  }
}