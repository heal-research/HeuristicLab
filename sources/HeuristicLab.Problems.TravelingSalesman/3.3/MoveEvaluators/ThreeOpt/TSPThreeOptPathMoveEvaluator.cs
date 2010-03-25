#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate 3-opt moves.
  /// </summary>
  [Item("TSPThreeOptPathMoveEvaluator", "Evaluates a 3-opt move by summing up the length of all added edges and subtracting the length of all deleted edges.")]
  [StorableClass]
  public abstract class TSPThreeOptPathMoveEvaluator : TSPPathMoveEvaluator, IThreeOptPermutationMoveOperator {
    public ILookupParameter<ThreeOptMove> ThreeOptMoveParameter {
      get { return (ILookupParameter<ThreeOptMove>)Parameters["ThreeOptMove"]; }
    }

    public TSPThreeOptPathMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<ThreeOptMove>("ThreeOptMove", "The move to evaluate."));
    }

    protected override double EvaluateByCoordinates(Permutation permutation, DoubleMatrix coordinates) {
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
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
      if (move.Index1 == move.Index3
        || move.Index2 - move.Index1 >= permutation.Length - 2
        || move.Index1 == permutation.Length - 1 && move.Index3 == 0
        || move.Index1 == 0 && move.Index3 == permutation.Length - 1) return 0;
      double moveQuality = 0;
      // remove three edges
      moveQuality -= CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality -= CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      moveQuality -= CalculateDistance(coordinates[edge3source, 0], coordinates[edge3source, 1],
        coordinates[edge3target, 0], coordinates[edge3target, 1]);
      // add three edges
      moveQuality += CalculateDistance(coordinates[edge3source, 0], coordinates[edge3source, 1],
        coordinates[edge1target, 0], coordinates[edge1target, 1]);
      moveQuality += CalculateDistance(coordinates[edge2source, 0], coordinates[edge2source, 1],
        coordinates[edge3target, 0], coordinates[edge3target, 1]);
      moveQuality += CalculateDistance(coordinates[edge1source, 0], coordinates[edge1source, 1],
        coordinates[edge2target, 0], coordinates[edge2target, 1]);
      return moveQuality;
    }

    protected override double EvaluateByDistanceMatrix(Permutation permutation, DoubleMatrix distanceMatrix) {
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
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
      if (move.Index1 == move.Index3
        || move.Index2 - move.Index1 >= permutation.Length - 2
        || move.Index1 == permutation.Length - 1 && move.Index3 == 0
        || move.Index1 == 0 && move.Index3 == permutation.Length - 1) return 0;
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
  }
}
