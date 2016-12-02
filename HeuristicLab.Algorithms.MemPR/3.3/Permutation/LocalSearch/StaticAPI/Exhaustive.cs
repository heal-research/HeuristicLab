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
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Algorithms.MemPR.Permutation.LocalSearch {
  public static class Exhaustive {
#if DEBUG
    private const bool VALIDATE = true;
#else
  private const bool VALIDATE = false;
#endif

    public static Tuple<int, int> HillClimb(IRandom random, Encodings.PermutationEncoding.Permutation perm,
      ref double quality, bool maximization, Func<Encodings.PermutationEncoding.Permutation, double> eval,
      CancellationToken token, bool[,] noTouch = null) {
      if (double.IsNaN(quality)) quality = eval(perm);
      Tuple<int, int> changes;
      switch (perm.PermutationType) {
        case PermutationTypes.Absolute:
          changes = ExhaustiveSwap2.HillClimb(random, perm, ref quality, maximization, eval, token, noTouch);
          break;
        case PermutationTypes.RelativeDirected:
          changes = Exhaustive1Shift.HillClimb(random, perm, ref quality, maximization, eval, token, noTouch);
          break;
        case PermutationTypes.RelativeUndirected:
          changes = Exhaustive2Opt.HillClimb(random, perm, ref quality, maximization, eval, token, noTouch);
          break;
        default: throw new ArgumentException(string.Format("Permutation type {0} is not known", perm.PermutationType));
      }
      if (VALIDATE && !perm.Validate()) throw new ArgumentException("HillClimb produced invalid child");
      return changes;
    }
  }
}
