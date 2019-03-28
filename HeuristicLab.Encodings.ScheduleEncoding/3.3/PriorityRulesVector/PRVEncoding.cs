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

using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PriorityRulesVectorEncoding", "Represents an encoding for a Scheduling Problem.")]
  [StorableType("D42AC200-1F87-451E-A953-803EF33BE953")]
  public class PRVEncoding : Item, ISchedule {
    [Storable]
    public IntegerVector PriorityRulesVector { get; private set; }
    [Storable]
    public int RandomSeed { get; private set; }

    [StorableConstructor]
    protected PRVEncoding(StorableConstructorFlag _) : base(_) { }
    protected PRVEncoding(PRVEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.PriorityRulesVector = cloner.Clone(original.PriorityRulesVector);
      this.RandomSeed = original.RandomSeed;
    }

    public PRVEncoding(IntegerVector iv, int randomSeed)
      : base() {
      this.RandomSeed = randomSeed;
      this.PriorityRulesVector = (IntegerVector)iv.Clone();
    }
    public PRVEncoding(int length, IRandom random, int min, int max)
      : base() {
      this.RandomSeed = random.Next();
      this.PriorityRulesVector = new IntegerVector(length, random, min, max);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVEncoding(this, cloner);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[ ");

      foreach (int i in PriorityRulesVector) {
        sb.Append(i + " ");
      }

      sb.Append("]");
      return sb.ToString();
    }
  }
}
