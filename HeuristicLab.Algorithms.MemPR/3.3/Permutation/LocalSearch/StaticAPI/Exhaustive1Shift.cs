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
  public static class Exhaustive1Shift {
    public static Tuple<int, int> HillClimb(IRandom random, Encodings.PermutationEncoding.Permutation perm,
      ref double quality, bool maximization, Func<Encodings.PermutationEncoding.Permutation, double> eval,
      CancellationToken token, bool[,] noTouch = null) {
      var current = perm;
      if (double.IsNaN(quality)) quality = eval(current);
      TranslocationMove lastSuccessMove = null;
      int steps = 0, evaluations = 0;
      while (true) {
        foreach (var shift in ExhaustiveInsertionMoveGenerator.Generate(current)) {
          if (lastSuccessMove != null && shift.Index1 == lastSuccessMove.Index1 && shift.Index2 == lastSuccessMove.Index2 && shift.Index3 == lastSuccessMove.Index3) {
            // been there, done that
            lastSuccessMove = null;
            break;
          }
          var prev1 = shift.Index1 - 1;
          var next1 = (shift.Index1 + 1) % current.Length;
          if (prev1 < 0) prev1 += current.Length;
          var prev3 = shift.Index3 - 1;
          var next3 = (shift.Index3 + 1) % current.Length;
          if (prev3 < 0) prev3 += current.Length;
          if (noTouch != null && ((noTouch[current[prev1], current[shift.Index1]] || noTouch[current[shift.Index1], current[next1]]
                                || noTouch[current[prev3], current[shift.Index3]] || noTouch[current[shift.Index3], current[next3]])))
            continue;
          TranslocationManipulator.Apply(current, shift.Index1, shift.Index2, shift.Index3);
          var q = eval(current);
          evaluations++;
          if (FitnessComparer.IsBetter(maximization, q, quality)) {
            steps++;
            quality = q;
            lastSuccessMove = shift;
          } else TranslocationManipulator.Apply(current, shift.Index3, shift.Index3, shift.Index1);

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
