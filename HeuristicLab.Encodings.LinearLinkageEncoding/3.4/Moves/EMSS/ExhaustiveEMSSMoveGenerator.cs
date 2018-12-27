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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("ExhaustiveEMSSMoveGenerator", "Generates all possible extract, merge, shift, and split (EMSS) moves from a given LLE solution.")]
  [StorableClass]
  public class ExhaustiveEMSSMoveGenerator : EMSSMoveGenerator, IExhaustiveMoveGenerator {
    [StorableConstructor]
    protected ExhaustiveEMSSMoveGenerator(bool deserializing) : base(deserializing) { }
    protected ExhaustiveEMSSMoveGenerator(ExhaustiveEMSSMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveEMSSMoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveEMSSMoveGenerator(this, cloner);
    }

    protected override EMSSMove[] GenerateMoves(LinearLinkage lle) {
      int length = lle.Length;
      if (length == 1) throw new ArgumentException("ExhaustiveEMSSMoveGenerator: There cannot be a move given only one item.", "lle");

      return Generate(lle).ToArray();
    }

    public static IEnumerable<EMSSMove> GenerateForItem(int i, List<int> groupItems, LinearLinkage lle, int[] lleb) {
      var pred = lleb[i];
      var next = lle[i];
      var isFirst = pred == i;
      var isLast = next == i;

      // First: shift i into each previous group
      foreach (var m in groupItems.Where(x => lle[x] != i)) {
        yield return new ShiftMove(i, pred, m, next, lle[m]);
      }

      if (!isLast) {
        // Second: split group at i
        yield return new SplitMove(i);

        if (isFirst) {
          // Third: merge with closed groups
          foreach (var m in groupItems.Where(x => lle[x] == x)) {
            yield return new MergeMove(i, m);
          }
        } else {
          // Fourth: extract i into group of its own (exclude first and last, because of SplitMove)
          yield return new ExtractMove(i, pred, next);
        }
      }
    }

    public static IEnumerable<EMSSMove> Generate(LinearLinkage lle) {
      var groupItems = new List<int>();
      var lleb = lle.ToBackLinks();
      for (var i = 0; i < lle.Length; i++) {
        foreach (var move in GenerateForItem(i, groupItems, lle, lleb))
          yield return move;
        if (lleb[i] != i)
          groupItems.Remove(lleb[i]);
        groupItems.Add(i);
      }
    }
  }
}
