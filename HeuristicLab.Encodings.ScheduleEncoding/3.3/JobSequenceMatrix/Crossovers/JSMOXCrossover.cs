#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMOrderCrossover", "Represents a crossover operation swapping sequences of the parents to generate offspring.")]
  [StorableClass]
  public class JSMOXCrossover : JSMCrossover {

    [StorableConstructor]
    protected JSMOXCrossover(bool deserializing) : base(deserializing) { }
    protected JSMOXCrossover(JSMOXCrossover original, Cloner cloner) : base(original, cloner) { }
    public JSMOXCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMOXCrossover(this, cloner);
    }

    public static JSMEncoding Apply(IRandom random, JSMEncoding parent1, JSMEncoding parent2) {
      var result = new JSMEncoding();

      for (int i = 0; i < parent1.JobSequenceMatrix.Count; i++) {
        result.JobSequenceMatrix.Add(
          HeuristicLab.Encodings.PermutationEncoding.OrderCrossover.Apply(random,
          parent1.JobSequenceMatrix[i], parent2.JobSequenceMatrix[i]));
      }

      return result;
    }

    public override JSMEncoding Cross(IRandom random, JSMEncoding parent1, JSMEncoding parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
