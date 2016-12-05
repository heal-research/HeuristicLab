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
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Algorithms.MemPR.Permutation.LocalSearch {
  public static class ExhaustiveSwap2 {
    public static Tuple<int, int> HillClimb(IRandom random, Encodings.PermutationEncoding.Permutation perm,
      ref double quality, bool maximization, Func<Encodings.PermutationEncoding.Permutation, double> eval,
      CancellationToken token, bool[,] noTouch = null) {
      var current = perm;
      if (double.IsNaN(quality)) quality = eval(current);
      Swap2Move lastSuccessMove = null;
      int steps = 0, evaluations = 0;
      while (true) {
        foreach (var swap in ExhaustiveSwap2MoveGenerator.Generate(current)) {
          if (lastSuccessMove != null && swap.Index1 == lastSuccessMove.Index1 && swap.Index2 == lastSuccessMove.Index2) {
            // been there, done that
            lastSuccessMove = null;
            break;
          }
          if (noTouch != null && (noTouch[swap.Index1, 0] || noTouch[swap.Index2, 0]))
            continue;

          var h = current[swap.Index1];
          current[swap.Index1] = current[swap.Index2];
          current[swap.Index2] = h;
          var q = eval(current);
          evaluations++;
          if (FitnessComparer.IsBetter(maximization, q, quality)) {
            steps++;
            quality = q;
            lastSuccessMove = swap;
          } else {
            current[swap.Index2] = current[swap.Index1];
            current[swap.Index1] = h;
          }

          if (token.IsCancellationRequested) {
            lastSuccessMove = null;
            break;
          }
        }
        if (lastSuccessMove == null) break;
      }
      return Tuple.Create(evaluations, steps);
    }
  }
}
