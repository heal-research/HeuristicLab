#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.ComponentModel;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("Job", "Represents a composition of tasks that require processing in a scheduling problem.")]
  [StorableClass]
  public class Job : Item, INotifyPropertyChanged {

    [Storable(Name = "DueDate")]
    private double dueDate;
    public double DueDate {
      get { return dueDate; }
      set {
        bool changed = dueDate != value;
        dueDate = value;
        if (changed) OnPropertyChanged("DueDate");
      }
    }

    [Storable(Name = "Index")]
    private int index;
    public int Index {
      get { return index; }
      set {
        bool changed = index != value;
        index = value;
        if (changed) OnPropertyChanged("Index");
      }
    }

    [Storable(Name = "Tasks")]
    private ItemList<Task> tasks;
    public ItemList<Task> Tasks {
      get { return tasks; }
      set {
        bool changed = tasks != value;
        tasks = value;
        if (changed) OnPropertyChanged("Tasks");
      }
    }

    [StorableConstructor]
    protected Job(bool deserializing) : base(deserializing) { }
    protected Job(Job original, Cloner cloner)
      : base(original, cloner) {
      this.DueDate = original.DueDate;
      this.Index = original.Index;
      this.Tasks = cloner.Clone(original.Tasks);
    }
    public Job() : this(-1, double.MaxValue) { }
    public Job(int index, double dueDate)
      : base() {
      DueDate = dueDate;
      Index = index;
      Tasks = new ItemList<Task>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Job(this, cloner);
    }

    public override string ToString() {
      var sb = new StringBuilder();
      sb.Append("Job#" + Index + " [ ");
      foreach (Task t in Tasks) {
        sb.Append(t.ToString() + " ");
      }
      sb.Append("{" + DueDate + "} ");
      sb.Append("]");
      return sb.ToString();
    }

    internal Task GetPreviousTask(Task t) {
      if (t.TaskNr == 0) return null;
      return Tasks[t.TaskNr - 1];
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
