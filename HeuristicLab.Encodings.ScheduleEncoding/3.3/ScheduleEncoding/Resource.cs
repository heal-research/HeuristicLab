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

using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("ResourceClass", "Represents a resource used in scheduling problems.")]
  [StorableClass]
  public sealed class Resource : Item {

    [Storable]
    public int Index { get; private set; }
    [Storable]
    public ItemList<ScheduledTask> Tasks { get; private set; }

    public double TotalDuration {
      get {
        if (Tasks.Count == 0) return 0.0;
        return Tasks.Max(t => t.EndTime);
      }
    }

    [StorableConstructor]
    private Resource(bool deserializing) : base(deserializing) { }
    private Resource(Resource original, Cloner cloner)
      : base(original, cloner) {
      this.Index = original.Index;
      this.Tasks = cloner.Clone(original.Tasks);
    }
    public Resource(int index)
      : base() {
      Index = index;
      Tasks = new ItemList<ScheduledTask>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Resource(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("Resource#" + Index + " [ ");
      foreach (ScheduledTask t in Tasks) {
        sb.Append(t + " ");
      }
      sb.Append("]");
      return sb.ToString();
    }
  }
}
