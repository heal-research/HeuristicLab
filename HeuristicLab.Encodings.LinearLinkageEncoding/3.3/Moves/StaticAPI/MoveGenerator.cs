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
    public static IEnumerable<Move> GenerateForItem(int i, int next, BidirectionalDictionary<int, int> links) {
      var pred = -1;
      var isFirst = !links.TryGetBySecond(i, out pred);
      var isLast = next == i;

      // First: shift i into each previous group
      foreach (var l in links.Where(x => x.Value != i)) {
        yield return new ShiftMove(i, isFirst ? i : pred, l.Key, next, l.Value);
      }

      if (!isLast) {
        // Second: split group at i
        yield return new SplitMove(i);

        if (isFirst) {
          // Third: merge with closed groups
          foreach (var l in links.Where(x => x.Key == x.Value)) {
            yield return new MergeMove(i, l.Key);
          }
        }
      }
      if (!isFirst)
        // Fourth: extract i into group of its own (exclude first, because of Second)
        yield return new ExtractMove(i, pred, next);

    }

    public static IEnumerable<Move> Generate(LinearLinkage lle) {
      var links = new BidirectionalDictionary<int, int>();
      for (var i = 0; i < lle.Length; i++) {
        foreach (var move in GenerateForItem(i, lle[i], links))
          yield return move;
        links.RemoveBySecond(i);
        links.Add(i, lle[i]);
      }
    }
  }
}
