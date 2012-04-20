#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate inversion moves (2-opt).
  /// </summary>
  [Item("TSPInversionMovePathEvaluator", "Evaluates an inversion move (2-opt) by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableClass]
  public abstract class TSPInversionMovePathEvaluator : TSPPathMoveEvaluator, IPermutationInversionMoveOperator {
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (ILookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected TSPInversionMovePathEvaluator(bool deserializing) : base(deserializing) { }
    protected TSPInversionMovePathEvaluator(TSPInversionMovePathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPInversionMovePathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
    }

    public static double EvaluateByCoordinates(Permutation permutation, InversionMove move, DoubleMatrix coordinates, TSPInversionMovePathEvaluator evaluator) {
      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      if (move.Index2 - move.Index1 >= permutation.Length - 2) return 0;
      double moveQuality = 0;
      // remove two edges
      moveQuality -= evaluator.CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
            coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality -= evaluator.CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      // add two edges
      moveQuality += evaluator.CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge2source, 0], coordinates[edge2source, 1]);
      moveQuality += evaluator.CalculateDistance(coordinates[edge1target, 0], coordinates[edge1target, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      return moveQuality;
    }

    public static double EvaluateByDistanceMatrix(Permutation permutation, InversionMove move, DistanceMatrix distanceMatrix) {
      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      if (move.Index2 - move.Index1 >= permutation.Length - 2) return 0;
      double moveQuality = 0;
      // remove two edges
      moveQuality -= distanceMatrix[edge1source, edge1target];
      moveQuality -= distanceMatrix[edge2source, edge2target];
      // add two edges
      moveQuality += distanceMatrix[edge1source, edge2source];
      moveQuality += distanceMatrix[edge1target, edge2target];
      return moveQuality;
    }

    protected override double EvaluateByCoordinates(Permutation permutation, DoubleMatrix coordinates) {
      return EvaluateByCoordinates(permutation, InversionMoveParameter.ActualValue, coordinates, this);
    }

    protected override double EvaluateByDistanceMatrix(Permutation permutation, DistanceMatrix distanceMatrix) {
      return EvaluateByDistanceMatrix(permutation, InversionMoveParameter.ActualValue, distanceMatrix);
    }
  }
}
