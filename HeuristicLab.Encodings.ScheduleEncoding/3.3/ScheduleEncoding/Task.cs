#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Task", "Represents a task that has to be scheduled.")]
  [StorableClass]
  public class Task : Item, INotifyPropertyChanged {

    [Storable(Name = "TaskNr")]
    private int taskNr;
    public int TaskNr {
      get { return taskNr; }
      set {
        bool changed = taskNr != value;
        taskNr = value;
        if (changed) {
          OnPropertyChanged("TaskNr");
          OnToStringChanged();
        }
      }
    }
    [Storable(Name = "ResourceNr")]
    private int resourceNr;
    public int ResourceNr {
      get { return resourceNr; }
      set {
        bool changed = resourceNr != value;
        resourceNr = value;
        if (changed) {
          OnPropertyChanged("ResourceNr");
          OnToStringChanged();
        }
      }
    }

    [Storable(Name = "JobNr")]
    private int jobNr;
    public int JobNr {
      get { return jobNr; }
      set {
        bool changed = jobNr != value;
        jobNr = value;
        if (changed) OnPropertyChanged("JobNr");
      }
    }

    [Storable(Name = "Duration")]
    private double duration;
    public double Duration {
      get { return duration; }
      set {
        bool changed = duration != value;
        duration = value;
        if (changed) OnPropertyChanged("Duration");
      }
    }

    [Storable(Name = "IsScheduled")]
    private bool isScheduled;
    public bool IsScheduled {
      get { return isScheduled; }
      set {
        bool changed = isScheduled != value;
        isScheduled = value;
        if (changed) OnPropertyChanged("IsScheduled");
      }
    }

    [StorableConstructor]
    protected Task(bool deserializing) : base(deserializing) { }
    protected Task(Task original, Cloner cloner)
      : base(original, cloner) {
      this.resourceNr = original.ResourceNr;
      this.jobNr = original.JobNr;
      this.duration = original.Duration;
      this.taskNr = original.TaskNr;
      this.isScheduled = original.IsScheduled;
    }
    public Task() : this(-1, -1, -1, 0) { }
    public Task(int taskNr, int resNr, int jobNr, double duration)
      : base() {
      this.duration = duration;
      this.resourceNr = resNr;
      this.jobNr = jobNr;
      this.taskNr = taskNr;
      this.isScheduled = false;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Task(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[" + TaskNr + "," + ResourceNr + "]");
      return sb.ToString();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
