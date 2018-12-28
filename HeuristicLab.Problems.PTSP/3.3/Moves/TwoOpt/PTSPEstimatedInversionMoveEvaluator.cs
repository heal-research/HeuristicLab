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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.PTSP {
  [Item("PTSP Estimated Inversion Move Evaluator", "Evaluates an inversion move (2-opt) over several realizations of tours by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableType("9E418FA4-7721-40D2-9FDC-DB82723F7DBF")]
  public class PTSPEstimatedInversionMoveEvaluator : EstimatedPTSPMoveEvaluator, IPermutationInversionMoveOperator {

    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected PTSPEstimatedInversionMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PTSPEstimatedInversionMoveEvaluator(PTSPEstimatedInversionMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public PTSPEstimatedInversionMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPEstimatedInversionMoveEvaluator(this, cloner);
    }

    public static double EvaluateMove(Permutation tour, InversionMove move, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      double moveQuality = 0;
      var edges = new int[4];
      var indices = new int[4];
      edges[0] = tour.GetCircular(move.Index1 - 1);
      indices[0] = move.Index1 - 1;
      if (indices[0] == -1) indices[0] = tour.Length - 1;
      edges[1] = tour[move.Index1];
      indices[1] = move.Index1;
      edges[2] = tour[move.Index2];
      indices[2] = move.Index2;
      edges[3] = tour.GetCircular(move.Index2 + 1);
      indices[3] = move.Index2 + 1;
      if (indices[3] == tour.Length + 1) indices[3] = 0;
      var aPosteriori = new int[4];
      foreach (var realization in realizations) {
        for (var i = 0; i < edges.Length; i++) {
          if (realization[edges[i]]) {
            aPosteriori[i] = edges[i];
          } else {
            var j = 1;
            if (i % 2 == 0) {
              // find nearest predecessor in realization if source edge
              while (!realization[tour.GetCircular(indices[i] - j)]) {
                j++;
              }
              aPosteriori[i] = tour.GetCircular(indices[i] - j);
            } else {
              // find nearest successor in realization if target edge
              while (!realization[tour.GetCircular(indices[i] + j)]) {
                j++;
              }
              aPosteriori[i] = tour.GetCircular(indices[i] + j);
            }
          }
        }
        // compute cost difference between the two a posteriori solutions
        if (!(aPosteriori[0] == aPosteriori[2] && aPosteriori[1] == aPosteriori[3])) {
          moveQuality = moveQuality + distance(aPosteriori[0], aPosteriori[2]) + distance(aPosteriori[1], aPosteriori[3])
            - distance(aPosteriori[0], aPosteriori[1]) - distance(aPosteriori[2], aPosteriori[3]);
        }
        Array.Clear(aPosteriori, 0, aPosteriori.Length);
      }
      // return average of cost differences
      return moveQuality / realizations.Count;
    }

    protected override double EvaluateMove(Permutation tour, Func<int, int, double> distance, ItemList<BoolArray> realizations) {
      return EvaluateMove(tour, InversionMoveParameter.ActualValue, distance, realizations);
    }
  }
}
