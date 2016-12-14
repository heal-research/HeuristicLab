#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  public static class MoveGenerator {
    public static IEnumerable<Move> GenerateForItem(int i, List<int> groupItems, LinearLinkage lle, int[] lleb) {
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

    public static IEnumerable<Move> Generate(LinearLinkage lle) {
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
