#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("Job Sequence Matrix", "Represents the job sequence matrix solution.")]
  [StorableType("8F19A51A-45F1-4C1D-BCD4-A9F57E40DDC5")]
  public class JSM : Item, IScheduleSolution {

    [Storable]
    public ItemList<Permutation> JobSequenceMatrix { get; private set; }
    [Storable]
    public int RandomSeed { get; private set; }

    [StorableConstructor]
    protected JSM(StorableConstructorFlag _) : base(_) { }
    protected JSM(JSM original, Cloner cloner)
      : base(original, cloner) {
      this.JobSequenceMatrix = cloner.Clone(original.JobSequenceMatrix);
      this.RandomSeed = original.RandomSeed;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSM(this, cloner);
    }
    public JSM(int randomSeed)
      : base() {
      RandomSeed = randomSeed;
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
