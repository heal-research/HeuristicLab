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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate a translocation or insertion move.
  /// </summary>
  [Item("TSPTranslocationMovePathEvaluator", "Evaluates a translocation or insertion move (3-opt) by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableType("DC2FBCA3-B75F-4DA2-974E-334083159D93")]
  public abstract class TSPTranslocationMovePathEvaluator : TSPPathMoveEvaluator, IPermutationTranslocationMoveOperator {
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (ILookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected TSPTranslocationMovePathEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPTranslocationMovePathEvaluator(TSPTranslocationMovePathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPTranslocationMovePathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
    }

    public static double EvaluateByCoordinates(Permutation permutation, TranslocationMove move, DoubleMatrix coordinates, TSPTranslocationMovePathEvaluator evaluator) {
      if (move.Index1 == move.Index3
        || move.Index2 == permutation.Length - 1 && move.Index3 == 0
        || move.Index1 == 0 && move.Index3 == permutation.Length - 1 - move.Index2) return 0;

      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      int edge3source, edge3target;
      if (move.Index3 > move.Index1) {
        edge3source = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1);
        edge3target = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1);
      } else {
        edge3source = permutation.GetCircular(move.Index3 - 1);
        edge3target = permutation[move.Index3];
      }
      double moveQuality = 0;
      // remove three edges
      moveQuality -= evaluator.CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality -= evaluator.CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      moveQuality -= evaluator.CalculateDistance(coordinates[edge3source, 0], coordinates[edge3source, 1],
        coordinates[edge3target, 0], coordinates[edge3target, 1]);
      // add three edges
      moveQuality += evaluator.CalculateDistance(coordinates[edge3source, 0], coordinates[edge3source, 1],
        coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality += evaluator.CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge3target, 0], coordinates[edge3target, 1]);
      moveQuality += evaluator.CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      return moveQuality;
    }

    public static double EvaluateByDistanceMatrix(Permutation permutation, TranslocationMove move, DistanceMatrix distanceMatrix) {
      if (move.Index1 == move.Index3
        || move.Index2 == permutation.Length - 1 && move.Index3 == 0
        || move.Index1 == 0 && move.Index3 == permutation.Length - 1 - move.Index2) return 0;

      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      int edge3source, edge3target;
      if (move.Index3 > move.Index1) {
        edge3source = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1);
        edge3target = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1);
      } else {
        edge3source = permutation.GetCircular(move.Index3 - 1);
        edge3target = permutation[move.Index3];
      }
      double moveQuality = 0;
      // remove three edges
      moveQuality -= distanceMatrix[edge1source, edge1target];
      moveQuality -= distanceMatrix[edge2source, edge2target];
      moveQuality -= distanceMatrix[edge3source, edge3target];
      // add three edges
      moveQuality += distanceMatrix[edge3source, edge1target];
      moveQuality += distanceMatrix[edge2source, edge3target];
      moveQuality += distanceMatrix[edge1source, edge2target];
      return moveQuality;
    }

    protected override double EvaluateByCoordinates(Permutation permutation, DoubleMatrix coordinates) {
      return EvaluateByCoordinates(permutation, TranslocationMoveParameter.ActualValue, coordinates, this);
    }

    protected override double EvaluateByDistanceMatrix(Permutation permutation, DistanceMatrix distanceMatrix) {
      return EvaluateByDistanceMatrix(permutation, TranslocationMoveParameter.ActualValue, distanceMatrix);
    }
  }
}
