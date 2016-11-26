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

using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.Binary.LocalSearch {
  public static class Heuristic {
    private static bool IsBetter(bool maximization, double a, double b) {
      return maximization && a > b
        || !maximization && a < b;
    }

    public static Tuple<int, int> ExhaustiveBitFlipSearch(IRandom random, BinaryVector solution, ref double quality, bool maximization, Func<BinaryVector, double> evalFunc, CancellationToken token, bool[] subspace = null) {
      if (double.IsNaN(quality)) quality = evalFunc(solution);
      var improved = false;
      var order = Enumerable.Range(0, solution.Length).Shuffle(random).ToArray();
      var lastImp = -1;
      var steps = 0;
      var evaluations = 0;
      do {
        improved = false;
        for (var i = 0; i < solution.Length; i++) {
          // in case we didn't make an improvement this round and arrived at the index of the last improvement
          // break means we don't need to try the remaining moves again as they have brought no improvement
          if (!improved && lastImp == i) break;
          var idx = order[i];
          if (subspace != null && !subspace[idx]) continue;
          // bitflip the solution
          solution[idx] = !solution[idx];
          var after = evalFunc(solution);
          evaluations++;
          if (IsBetter(maximization, after, quality)) {
            steps++;
            quality = after;
            lastImp = i;
            improved = true;
          } else {
            // undo the bitflip in case no improvement was made
            solution[idx] = !solution[idx];
          }
          token.ThrowIfCancellationRequested();
        }
      } while (improved);
      
      return Tuple.Create(evaluations, steps);
    }
  }
}
