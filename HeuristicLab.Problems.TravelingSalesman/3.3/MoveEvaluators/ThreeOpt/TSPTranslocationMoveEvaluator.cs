#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate a translocation or insertion move.
  /// </summary>
  [Item("TSP Translocation Move Evaluator", "Evaluates a translocation or insertion move (3-opt) by an efficient delta computation.")]
  [StorableType("3f13bc93-778e-4b9b-8285-1b807886a653")]
  public class TSPTranslocationMoveEvaluator : TSPMoveEvaluator, IPermutationTranslocationMoveOperator {
    [Storable] public ILookupParameter<TranslocationMove> MoveParameter { get; private set; }

    ILookupParameter<TranslocationMove> IPermutationTranslocationMoveOperator.TranslocationMoveParameter => MoveParameter;
    ILookupParameter<Permutation> IPermutationMoveOperator.PermutationParameter => TSPTourParameter;

    [StorableConstructor]
    protected TSPTranslocationMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPTranslocationMoveEvaluator(TSPTranslocationMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
      MoveParameter = cloner.Clone(original.MoveParameter);
    }
    public TSPTranslocationMoveEvaluator()
      : base() {
      Parameters.Add(MoveParameter = new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPTranslocationMoveEvaluator(this, cloner);
    }

    protected override double CalculateTourLengthWithMove(ITSPData tspData, Permutation tspTour, double tourLength) {
      var move = MoveParameter.ActualValue;
      return CalculateTourLengthDelta(tspData, tspTour, move);
    }

    public static double CalculateTourLengthDelta(ITSPData tspData, Permutation tspTour, TranslocationMove move) {
      if (move.Index1 == move.Index3
        || move.Index2 == tspTour.Length - 1 && move.Index3 == 0
        || move.Index1 == 0 && move.Index3 == tspTour.Length - 1 - move.Index2) return 0;

      int edge1source = tspTour.GetCircular(move.Index1 - 1);
      int edge1target = tspTour[move.Index1];
      int edge2source = tspTour[move.Index2];
      int edge2target = tspTour.GetCircular(move.Index2 + 1);
      int edge3source, edge3target;
      if (move.Index3 > move.Index1) {
        edge3source = tspTour.GetCircular(move.Index3 + move.Index2 - move.Index1);
        edge3target = tspTour.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1);
      } else {
        edge3source = tspTour.GetCircular(move.Index3 - 1);
        edge3target = tspTour[move.Index3];
      }
      double moveQuality = 0;
      // remove three edges
      moveQuality -= tspData.GetDistance(edge1source, edge1target);
      moveQuality -= tspData.GetDistance(edge2source, edge2target);
      moveQuality -= tspData.GetDistance(edge3source, edge3target);
      // add three edges
      moveQuality += tspData.GetDistance(edge3source, edge1target);
      moveQuality += tspData.GetDistance(edge2source, edge3target);
      moveQuality += tspData.GetDistance(edge1source, edge2target);
      return moveQuality;
    }
  }
}
