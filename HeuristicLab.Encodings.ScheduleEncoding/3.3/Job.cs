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
using System.ComponentModel;
using System.Text;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("Job", "Represents a composition of tasks that require processing in a scheduling problem.")]
  [StorableType("77EC3F5A-5539-4090-8EB6-6599F17A32D8")]
  public sealed class Job : Item, INotifyPropertyChanged {

    [Storable(Name = "DueDate")]
    private double dueDate;
    public double DueDate {
      get { return dueDate; }
      set {
        if (dueDate == value) return;
        dueDate = value;
        OnPropertyChanged("DueDate");
        OnToStringChanged();
      }
    }

    [Storable(Name = "Index")]
    private int index;
    public int Index {
      get { return index; }
      set {
        if (index == value) return;
        index = value;
        OnPropertyChanged("Index");
        OnToStringChanged();
      }
    }

    [Storable(Name = "Tasks")]
    private ItemList<Task> tasks;
    public ItemList<Task> Tasks {
      get { return tasks; }
      private set {
        tasks = value;
        OnPropertyChanged("Tasks");
      }
    }

    [StorableConstructor]
    private Job(StorableConstructorFlag _) : base(_) { }
    private Job(Job original, Cloner cloner)
      : base(original, cloner) {
      this.dueDate = original.DueDate;
      this.index = original.Index;
      this.tasks = cloner.Clone(original.Tasks);
      RegisterEventHandlers();
    }
    public Job() : this(-1, double.MaxValue) { }
    public Job(int index, double dueDate)
      : base() {
      this.dueDate = dueDate;
      this.index = index;
      this.tasks = new ItemList<Task>();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Job(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Tasks.ItemsAdded += TasksOnItemsChanged;
      Tasks.ItemsRemoved += TasksOnItemsRemoved;
      Tasks.ItemsReplaced += TasksOnItemsChanged;
      Tasks.CollectionReset += TasksOnItemsChanged;
      foreach (var task in Tasks) {
        task.PropertyChanged += TaskOnPropertyChanged;
        task.ToStringChanged += TaskOnToStringChanged;
      }
    }

    private void TasksOnItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<Task>> e) {
      foreach (var task in e.OldItems) {
        task.Value.PropertyChanged -= TaskOnPropertyChanged;
        task.Value.ToStringChanged -= TaskOnToStringChanged;
      }
      foreach (var task in e.Items) {
        task.Value.PropertyChanged += TaskOnPropertyChanged;
        task.Value.ToStringChanged += TaskOnToStringChanged;
      }
      OnTasksChanged();
      OnToStringChanged();
    }

    private void TasksOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Task>> e) {
      foreach (var task in e.Items) {
        task.Value.PropertyChanged -= TaskOnPropertyChanged;
        task.Value.ToStringChanged -= TaskOnToStringChanged;
      }
      OnTasksChanged();
      OnToStringChanged();
    }

    private void TaskOnPropertyChanged(object sender, EventArgs e) {
      OnTasksChanged();
    }

    private void TaskOnToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }

    public override string ToString() {
      var sb = new StringBuilder();
      sb.Append("Job#" + Index + " [ ");
      foreach (Task t in Tasks) {
        sb.Append(t + " ");
      }
      sb.Append("{" + DueDate + "} ");
      sb.Append("]");
      return sb.ToString();
    }

    public event EventHandler TasksChanged;
    private void OnTasksChanged() {
      var handler = TasksChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
