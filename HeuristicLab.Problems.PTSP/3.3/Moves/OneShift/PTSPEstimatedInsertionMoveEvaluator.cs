#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Estimated Insertion Move Evaluator", "Evaluates an insertion move (1-shift)")]
  [StorableClass]
  public class PTSPEstimatedInsertionMoveEvaluator : EstimatedPTSPMoveEvaluator, IPermutationTranslocationMoveOperator {

    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected PTSPEstimatedInsertionMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected PTSPEstimatedInsertionMoveEvaluator(PTSPEstimatedInsertionMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPEstimatedInsertionMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPEstimatedInsertionMoveEvaluator(this, cloner);
    }

    public static double EvaluateMove(Permutation tour, TranslocationMove move, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      var afterMove = (Permutation)tour.Clone();
      TranslocationManipulator.Apply(afterMove, move.Index1, move.Index1, move.Index3);
      double moveQuality = 0;
      var edges = new int[12];
      var indices = new int[12];
      edges[0] = tour.GetCircular(move.Index1 - 1);
      indices[0] = DecreaseCircularIndex(tour.Length, move.Index1);
      edges[1] = tour[move.Index1];
      indices[1] = move.Index1;
      edges[2] = tour[move.Index1];
      indices[2] = move.Index1;
      edges[3] = tour.GetCircular(move.Index1 + 1);
      indices[3] = IncreaseCircularIndex(tour.Length, move.Index1);

      edges[6] = afterMove.GetCircular(move.Index3 - 1);
      indices[6] = DecreaseCircularIndex(afterMove.Length, move.Index3);
      edges[7] = afterMove[move.Index3];
      indices[7] = move.Index3;
      edges[8] = afterMove[move.Index3];
      indices[8] = move.Index3;
      edges[9] = afterMove.GetCircular(move.Index3 + 1);
      indices[9] = IncreaseCircularIndex(afterMove.Length, move.Index3);

      if (move.Index3 > move.Index1) {
        edges[4] = tour[move.Index3];
        indices[4] = move.Index3;
        edges[5] = tour.GetCircular(move.Index3 + 1);
        indices[5] = indices[9];
        edges[10] = afterMove.GetCircular(move.Index1 - 1);
        indices[10] = indices[0];
        edges[11] = afterMove[move.Index1];
        indices[11] = move.Index1;
      } else {
        edges[4] = tour.GetCircular(move.Index3 - 1);
        indices[4] = indices[6];
        edges[5] = tour[move.Index3];
        indices[5] = move.Index3;
        edges[10] = afterMove[move.Index1];
        indices[10] = move.Index1;
        edges[11] = afterMove.GetCircular(move.Index1 + 1);
        indices[11] = indices[3];
      }
      int[] aPosteriori = new int[12];
      foreach (var realization in realizations) {
        for (int i = 0; i < edges.Length; i++) {
          Permutation tempPermutation;
          if (i < 6) {
            tempPermutation = tour;
          } else {
            tempPermutation = afterMove;
          }
          if (realization[edges[i]]) {
            aPosteriori[i] = edges[i];
          } else {
            int j = 1;
            if (i % 2 == 0) {
              // find nearest predecessor in realization if source edge
              while (!realization[tempPermutation.GetCircular(indices[i] - j)]) {
                j++;
              }
              aPosteriori[i] = tempPermutation.GetCircular(indices[i] - j);
            } else {
              // find nearest successor in realization if target edge
              while (!realization[tempPermutation.GetCircular(indices[i] + j)]) {
                j++;
              }
              aPosteriori[i] = tempPermutation.GetCircular(indices[i] + j);
            }
          }
        }
        if (!(aPosteriori[0] == aPosteriori[2] && aPosteriori[1] == aPosteriori[3]) &&
          !(aPosteriori[0] == aPosteriori[4] && aPosteriori[1] == aPosteriori[5]) &&
          !(aPosteriori[2] == aPosteriori[4] && aPosteriori[3] == aPosteriori[5])) {
          // compute cost difference between the two a posteriori solutions
          moveQuality = moveQuality + distance(aPosteriori[6], aPosteriori[7]) + distance(aPosteriori[8], aPosteriori[9]) + distance(aPosteriori[10], aPosteriori[11]);
          moveQuality = moveQuality - distance(aPosteriori[0], aPosteriori[1]) - distance(aPosteriori[2], aPosteriori[3]) - distance(aPosteriori[4], aPosteriori[5]);
        }
        Array.Clear(aPosteriori, 0, aPosteriori.Length);
      }
      // return average of cost differences
      return moveQuality / realizations.Count;
    }

    private static int DecreaseCircularIndex(int length, int index) {
      var result = index - 1;
      if (result == -1) {
        result = length - 1;
      }
      return result;
    }

    private static int IncreaseCircularIndex(int length, int index) {
      var result = index + 1;
      if (result == length + 1) {
        result = 0;
      }
      return result;
    }

    protected override double EvaluateMove(Permutation tour, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      return EvaluateMove(tour, TranslocationMoveParameter.ActualValue, distance, realizations);
    }
  }
}
