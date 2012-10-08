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
  [Item("ResourceClass", "Represents a resource used in scheduling problems.")]
  [StorableClass]
  public class Resource : Item {
    public Resource(int index)
      : base() {
      Index = index;
      Tasks = new ItemList<ScheduledTask>();
    }
    [Storable]
    public int Index {
      get;
      set;
    }
    [Storable]
    public ItemList<ScheduledTask> Tasks {
      get;
      set;
    }
    public double TotalDuration {
      get {
        double result = 0;
        foreach (ScheduledTask t in Tasks) {
          if (t.EndTime > result)
            result = t.EndTime;
        }
        return result;
      }
    }

    [StorableConstructor]
    protected Resource(bool deserializing) : base(deserializing) { }
    protected Resource(Resource original, Cloner cloner)
      : base(original, cloner) {
      this.Index = original.Index;
      this.Tasks = cloner.Clone(original.Tasks);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Resource(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("Resource#" + Index + " [ ");
      foreach (ScheduledTask t in Tasks) {
        sb.Append(t.ToString() + " ");
      }
      sb.Append("]");
      return sb.ToString();
    }


    public override bool Equals(object obj) {
      if (obj.GetType() == typeof(Resource))
        return AreEqual(this, obj as Resource);
      else
        return false;
    }
    public override int GetHashCode() {
      if (Tasks.Count == 1)
        return Tasks[0].GetHashCode();
      if (Tasks.Count == 2)
        return Tasks[0].GetHashCode() ^ Tasks[1].GetHashCode();
      return 0;
    }
    private static bool AreEqual(Resource res1, Resource res2) {
      if (res1.Tasks.Count != res2.Tasks.Count)
        return false;
      for (int i = 0; i < res1.Tasks.Count; i++) {
        if (!res1.Tasks[i].Equals(res2.Tasks[i]))
          return false;
      }

      return true;
    }
  }
}
