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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("JobSequenceMatrixEncoding", "Represents an encoding for a scheduling Problem using a list of job sequences to deliver scheduleinformation.")]
  [StorableClass]
  public class JSMEncoding : Item, ISchedule {

    [Storable]
    public ItemList<Permutation> JobSequenceMatrix { get; set; }

    [StorableConstructor]
    protected JSMEncoding(bool deserializing) : base(deserializing) { }
    protected JSMEncoding(JSMEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.JobSequenceMatrix = cloner.Clone(original.JobSequenceMatrix);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMEncoding(this, cloner);
    }
    public JSMEncoding()
      : base() {
      JobSequenceMatrix = new ItemList<Permutation>();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");

      foreach (Permutation p in JobSequenceMatrix) {
        sb.AppendLine(p.ToString());
      }

      sb.Append("]");
      return sb.ToString();
    }
  }
}
