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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAPSwap2MoveEvaluator", "Evaluated a swap-2 move on a QAP solution.")]
  [StorableClass]
  public class QAPSwap2MoveEvaluator : QAPMoveEvaluator, IPermutationSwap2MoveOperator {
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return (ILookupParameter<Swap2Move>)Parameters["Swap2Move"]; }
    }

    [StorableConstructor]
    protected QAPSwap2MoveEvaluator(bool deserializing) : base(deserializing) { }
    protected QAPSwap2MoveEvaluator(QAPSwap2MoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPSwap2MoveEvaluator() {
      Parameters.Add(new LookupParameter<Swap2Move>("Swap2Move", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPSwap2MoveEvaluator(this, cloner);
    }

    /// <summary>
    /// Calculates the quality of the move <paramref name="move"/> by evaluating the changes.
    /// The runtime complexity of this method is O(N) with N being the size of the permutation.
    /// </summary>
    /// <param name="assignment">The current permutation.</param>
    /// <param name="move">The move that is to be evaluated if it was applied to the current permutation.</param>
    /// <param name="weights">The weights matrix.</param>
    /// <param name="distances">The distances matrix.</param>
    /// <returns>The relative change in quality if <paramref name="move"/> was applied to <paramref name="assignment"/>.</returns>
    public static double Apply(Permutation assignment, Swap2Move move, DoubleMatrix weights, DoubleMatrix distances) {
      if (move.Index1 == move.Index2) return 0;
      double moveQuality = 0;
      int fac1 = move.Index1, fac2 = move.Index2;
      int loc1 = assignment[fac1], loc2 = assignment[fac2];

      for (int j = 0; j < assignment.Length; j++) {
        if (j == fac1) {
          moveQuality += weights[fac1, fac1] * (distances[loc2, loc2] - distances[loc1, loc1]);
          moveQuality += weights[fac1, fac2] * (distances[loc2, loc1] - distances[loc1, loc2]);
        } else if (j == fac2) {
          moveQuality += weights[fac2, fac2] * (distances[loc1, loc1] - distances[loc2, loc2]);
          moveQuality += weights[fac2, fac1] * (distances[loc1, loc2] - distances[loc2, loc1]);
        } else {
          int locJ = assignment[j];
          moveQuality += weights[fac1, j] * (distances[loc2, locJ] - distances[loc1, locJ]);
          moveQuality += weights[j, fac1] * (distances[locJ, loc2] - distances[locJ, loc1]);
          moveQuality += weights[fac2, j] * (distances[loc1, locJ] - distances[loc2, locJ]);
          moveQuality += weights[j, fac2] * (distances[locJ, loc1] - distances[locJ, loc2]);
        }
      }
      return moveQuality;
    }

    /// <summary>
    /// Is able to compute the move qualities faster O(1) in some cases if it knows the quality of
    /// performing the move <paramref name="move"/> previously. In other cases it performs a
    /// standard move quality calculation with runtime complexity O(N).
    /// </summary>
    /// <remarks>
    /// The number of cases that the calculation can be performed faster grows with N^2
    /// while the number of cases which require a larger recalculation grows linearly with N.
    /// Larger problem instances thus benefit from this faster method to a larger degree.
    /// </remarks>
    /// <param name="assignment">The current permutation.</param>
    /// <param name="move">The current move that is to be evaluated.</param>
    /// <param name="previousQuality">The quality of that move as evaluated for the previous permutation.</param>
    /// <param name="weights">The weigths matrix.</param>
    /// <param name="distances">The distances matrix.</param>
    /// <param name="lastMove">The move that was applied to transform the permutation from the previous to the current one.</param>
    /// <returns>The relative change in quality if <paramref name="move"/> was applied to <paramref name="assignment"/>.</returns>
    public static double Apply(Permutation assignment, Swap2Move move, double previousQuality,
      DoubleMatrix weights, DoubleMatrix distances, Swap2Move lastMove) {
      bool overlapsLastMove = move.Index1 == lastMove.Index1
                           || move.Index2 == lastMove.Index1
                           || move.Index1 == lastMove.Index2
                           || move.Index2 == lastMove.Index2;

      if (!overlapsLastMove) {
        int r = lastMove.Index1, u = move.Index1, s = lastMove.Index2, v = move.Index2;
        int pR = assignment[lastMove.Index1], pU = assignment[move.Index1], pS = assignment[lastMove.Index2], pV = assignment[move.Index2];

        return previousQuality
          + (weights[r, u] - weights[r, v] + weights[s, v] - weights[s, u])
            * (distances[pS, pU] - distances[pS, pV] + distances[pR, pV] - distances[pR, pU])
          + (weights[u, r] - weights[v, r] + weights[v, s] - weights[u, s])
            * (distances[pU, pS] - distances[pV, pS] + distances[pV, pR] - distances[pU, pR]);
      } else {
        return Apply(assignment, move, weights, distances);
      }
    }

    public override IOperation Apply() {
      Swap2Move move = Swap2MoveParameter.ActualValue;
      if (move == null) throw new InvalidOperationException("Swap-2 move is not found.");
      Permutation assignment = PermutationParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;
      DoubleMatrix weights = WeightsParameter.ActualValue;

      double moveQuality = QualityParameter.ActualValue.Value;
      moveQuality += Apply(assignment, move, weights, distances);
      MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      return base.Apply();
    }
  }
}
