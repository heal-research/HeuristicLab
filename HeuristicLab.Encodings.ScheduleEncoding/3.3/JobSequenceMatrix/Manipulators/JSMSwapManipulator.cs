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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;

namespace HeuristicLab.Encodings.ScheduleEncoding.JobSequenceMatrix {
  [Item("JSMSwapManipulator", "Represents a manipulation operation swapping parts of the individual.")]
  [StorableType("7BB1535F-C886-40A3-9F7C-4A54CED90A6E")]
  public class JSMSwapManipulator : JSMManipulator {

    [StorableConstructor]
    protected JSMSwapManipulator(StorableConstructorFlag _) : base(_) { }
    protected JSMSwapManipulator(JSMSwapManipulator original, Cloner cloner) : base(original, cloner) { }
    public JSMSwapManipulator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new JSMSwapManipulator(this, cloner);
    }

    public static void Apply(IRandom random, JSMEncoding individual) {
      int resourceIndex = random.Next(individual.JobSequenceMatrix.Count);
      Permutation p = individual.JobSequenceMatrix[resourceIndex];
      int seqIndex1 = random.Next(p.Length);
      int seqIndex2 = random.Next(p.Length);
      int aux = p[seqIndex1];
      p[seqIndex1] = p[seqIndex2];
      p[seqIndex2] = aux;
    }

    protected override void Manipulate(IRandom random, IScheduleEncoding individual) {
      var solution = individual as JSMEncoding;
      if (solution == null) throw new InvalidOperationException("Encoding is not of type JSMEncoding");
      Apply(random, solution);
    }
  }
}
