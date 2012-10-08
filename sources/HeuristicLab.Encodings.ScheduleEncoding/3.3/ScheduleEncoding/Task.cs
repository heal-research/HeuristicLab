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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("Task", "Represents a task that has to be scheduled.")]
  [StorableClass]
  public class Task : Item {
    [Storable]
    public int TaskNr { get; set; }
    [Storable]
    public int ResourceNr { get; set; }
    [Storable]
    public int JobNr { get; set; }
    [Storable]
    public double Duration { get; set; }
    [Storable]
    public bool IsScheduled { get; set; }

    [StorableConstructor]
    protected Task(bool deserializing) : base(deserializing) { }
    protected Task(Task original, Cloner cloner)
      : base(original, cloner) {
      this.ResourceNr = original.ResourceNr;
      this.JobNr = original.JobNr;
      this.Duration = original.Duration;
      this.TaskNr = original.TaskNr;
      this.IsScheduled = original.IsScheduled;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Task(this, cloner);
    }


    public Task(int taskNr, int resNr, int jobNr, double duration)
      : base() {
      Duration = duration;
      ResourceNr = resNr;
      JobNr = jobNr;
      TaskNr = taskNr;
      IsScheduled = false;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[" + TaskNr + "," + ResourceNr + "]");
      return sb.ToString();
    }

    public override bool Equals(object obj) {
      if (obj.GetType() == typeof(Task))
        return AreEqual(this, obj as Task);
      else
        return false;
    }
    public override int GetHashCode() {
      return TaskNr ^ JobNr;
    }
    public static bool AreEqual(Task task1, Task task2) {
      return (task1.Duration == task2.Duration &&
        task1.IsScheduled == task2.IsScheduled &&
        task1.JobNr == task2.JobNr &&
        task1.ResourceNr == task2.ResourceNr &&
        task1.TaskNr == task2.TaskNr);
    }
  }
}
