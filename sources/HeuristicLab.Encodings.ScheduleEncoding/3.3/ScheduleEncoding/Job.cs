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
  [Item("Job", "Represents a composition of tasks that require processing in a scheduling problem.")]
  [StorableClass]
  public class Job : Item {
    [Storable]
    public double DueDate { get; set; }
    [Storable]
    public int Index { get; set; }
    [Storable]
    public ItemList<Task> Tasks { get; set; }

    [StorableConstructor]
    protected Job(bool deserializing) : base(deserializing) { }
    protected Job(Job original, Cloner cloner)
      : base(original, cloner) {
      this.DueDate = original.DueDate;
      this.Index = original.Index;
      this.Tasks = cloner.Clone(original.Tasks);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Job(this, cloner);
    }
    public Job(int index, double dueDate)
      : base() {
      Index = index;
      Tasks = new ItemList<Task>();

      DueDate = dueDate;

    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("Job#" + Index + " [ ");
      foreach (Task t in Tasks) {
        sb.Append(t.ToString() + " ");
      }
      sb.Append("{" + DueDate + "} ");
      sb.Append("]");
      return sb.ToString();
    }

    internal Task GetPreviousTask(Task t) {
      if (t.TaskNr == 0)
        return null;
      else
        return Tasks[t.TaskNr - 1];
    }


  }
}
