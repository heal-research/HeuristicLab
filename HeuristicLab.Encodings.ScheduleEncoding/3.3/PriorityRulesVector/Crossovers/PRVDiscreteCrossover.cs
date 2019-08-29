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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding {
  [Item("PRVDiscreteCrossover", "Represents a crossover operation swapping sequences of the parents to generate offspring.")]
  [StorableType("79050109-0BDC-4EBE-A290-D32F99BB45D4")]
  public class PRVDiscreteCrossover : PRVCrossover {

    [StorableConstructor]
    protected PRVDiscreteCrossover(StorableConstructorFlag _) : base(_) { }
    protected PRVDiscreteCrossover(PRVDiscreteCrossover original, Cloner cloner) : base(original, cloner) { }
    public PRVDiscreteCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVDiscreteCrossover(this, cloner);
    }

    public static PRVEncoding Apply(IRandom random, PRVEncoding parent1, PRVEncoding parent2) {
      var randomSeed = random.Next();
      var integerVector = DiscreteCrossover.Apply(random, new ItemArray<IntegerVector>(new[] { parent1.PriorityRulesVector, parent2.PriorityRulesVector }));
      return new PRVEncoding(integerVector, randomSeed);
    }

    public override PRVEncoding Cross(IRandom random, PRVEncoding parent1, PRVEncoding parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
