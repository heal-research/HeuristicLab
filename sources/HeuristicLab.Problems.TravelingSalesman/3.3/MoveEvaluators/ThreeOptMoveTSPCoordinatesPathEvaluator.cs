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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator to evaluate 3-opt moves.
  /// </summary>
  [Item("ThreeOptMoveTSPCoordinatesPathEvaluator", "Base class for evaluating 3-opt moves given the cities' coordinates.")]
  [StorableClass]
  public abstract class ThreeOptMoveTSPCoordinatesPathEvaluator : TSPPathMoveEvaluator, IThreeOptPermutationMoveOperator {
    public ILookupParameter<ThreeOptMove> ThreeOptMoveParameter {
      get { return (ILookupParameter<ThreeOptMove>)Parameters["ThreeOptMove"]; }
    }

    public ThreeOptMoveTSPCoordinatesPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<ThreeOptMove>("ThreeOptMove", "The move to evaluate."));
    }

    public override IOperation Apply() {
      Permutation permutation = PermutationParameter.ActualValue;
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
      double moveQuality = QualityParameter.ActualValue.Value;
      int edge1source = permutation.GetCircular(move.Index1 - 1);
      int edge1target = permutation[move.Index1];
      int edge2source = permutation[move.Index2];
      int edge2target = permutation.GetCircular(move.Index2 + 1);
      int edge3source = permutation.GetCircular(move.Index3 - 1);
      int edge3target = permutation[move.Index3];

      if (!(move.Index1 == 0 && move.Index2 == permutation.Length - 1)) { // a change like this would break the calculation (it doesn't actually change the quality anyway)
        if (UseDistanceMatrixParameter.ActualValue.Value) {
          DoubleMatrix distanceMatrix = DistanceMatrixParameter.ActualValue;
          if (distanceMatrix == null) {
            distanceMatrix = CalculateDistanceMatrix(CoordinatesParameter.ActualValue);
            DistanceMatrixParameter.ActualValue = distanceMatrix;
          }
          // remove three edges
          moveQuality -= distanceMatrix[edge1source, edge1target];
          moveQuality -= distanceMatrix[edge2source, edge2target];
          moveQuality -= distanceMatrix[edge3source, edge3target];
          // add three edges
          moveQuality += distanceMatrix[edge3source, edge1target];
          moveQuality += distanceMatrix[edge2source, edge3target];
          moveQuality += distanceMatrix[edge1source, edge2target];
        } else {
          DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
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
        }
      }
      if (MoveQualityParameter.ActualValue == null) MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      else MoveQualityParameter.ActualValue.Value = moveQuality;
      return base.Apply();
    }

    protected abstract double CalculateDistance(double x1, double y1, double x2, double y2);

    protected DoubleMatrix CalculateDistanceMatrix(DoubleMatrix c) {
      DoubleMatrix distanceMatrix = new DoubleMatrix(c.Rows, c.Rows);
      for (int i = 0; i < distanceMatrix.Rows; i++) {
        for (int j = 0; j < distanceMatrix.Columns; j++)
          distanceMatrix[i, j] = CalculateDistance(c[i, 0], c[i, 1], c[j, 0], c[j, 1]);
      }
      return distanceMatrix;
    }
  }
}
