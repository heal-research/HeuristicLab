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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("ExhaustiveSwap2MoveGenerator", "Generates all possible swap-2 moves from a given lle grouping.")]
  [StorableType("1D050D36-3197-4931-9806-F8119A60E8B6")]
  public class ExhaustiveSwap2MoveGenerator : Swap2MoveGenerator, IExhaustiveMoveGenerator {
    [StorableConstructor]
    protected ExhaustiveSwap2MoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected ExhaustiveSwap2MoveGenerator(ExhaustiveSwap2MoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveSwap2MoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveSwap2MoveGenerator(this, cloner);
    }

    public static IEnumerable<Swap2Move> Generate(LinearLinkage lle) {
      int length = lle.Length;
      if (length == 1) throw new ArgumentException("ExhaustiveSwap2MoveGenerator: There cannot be an Swap move given only one item.", "lle");

      var groups = lle.GetGroups().ToList();
      if (groups.Count == 1) throw new InvalidOperationException("ExhaustiveSwap2MoveGenerator: Swap moves cannot be applied when there is only one group.");

      for (int i = 0; i < groups.Count; i++)
        for (int j = i + 1; j < groups.Count; j++)
          for (var k = 0; k < groups[i].Count; k++)
            for (var m = 0; m < groups[j].Count; m++) {
              yield return new Swap2Move(groups[i][k], groups[j][m]);
            }
    }

    protected override Swap2Move[] GenerateMoves(LinearLinkage lle) {
      return Generate(lle).ToArray();
    }
  }
}
