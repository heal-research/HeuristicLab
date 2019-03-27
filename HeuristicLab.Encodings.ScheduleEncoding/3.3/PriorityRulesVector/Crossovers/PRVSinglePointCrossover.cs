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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding.PriorityRulesVector {
  [Item("PRVSinglePointCrossover", "Represents a crossover operation swapping sequences of the parents to generate offspring.")]
  [StorableType("7E148F4E-1993-44B5-B466-E1E441203498")]
  public class PRVSinglePointCrossover : PRVCrossover {

    [StorableConstructor]
    protected PRVSinglePointCrossover(StorableConstructorFlag _) : base(_) { }
    protected PRVSinglePointCrossover(PRVSinglePointCrossover original, Cloner cloner) : base(original, cloner) { }
    public PRVSinglePointCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PRVSinglePointCrossover(this, cloner);
    }

    public static PRVEncoding Apply(IRandom random, PRVEncoding parent1, PRVEncoding parent2) {
      return new PRVEncoding(SinglePointCrossover.Apply(random, parent1.PriorityRulesVector, parent2.PriorityRulesVector), parent1.NrOfRules);
    }

    public override PRVEncoding Cross(IRandom random, PRVEncoding parent1, PRVEncoding parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
