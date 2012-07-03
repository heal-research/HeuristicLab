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

using System;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.VehicleRouting {
  public struct DistanceMatrix {
    public DoubleMatrix Matrix { get; set; }
    public BoolValue UseDistanceMatrix { get; set; }
  }

  public sealed class VRPUtilities {
    public static double CalculateDistance(int start, int end, DoubleMatrix coordinates) {
      double distance = 0.0;

      distance =
          Math.Sqrt(
            Math.Pow(coordinates[start, 0] - coordinates[end, 0], 2) +
            Math.Pow(coordinates[start, 1] - coordinates[end, 1], 2));

      return distance;
    }

    private static DoubleMatrix CreateDistanceMatrix(DoubleMatrix coordinates) {
      DoubleMatrix distanceMatrix = new DoubleMatrix(coordinates.Rows, coordinates.Rows);

      for (int i = 0; i < distanceMatrix.Rows; i++) {
        for (int j = i; j < distanceMatrix.Columns; j++) {
          double distance = CalculateDistance(i, j, coordinates);

          distanceMatrix[i, j] = distance;
          distanceMatrix[j, i] = distance;
        }
      }

      return distanceMatrix;
    }

    public static DistanceMatrix GetDistanceMatrix(DoubleMatrix coordinates, IParameter distanceMatrix, BoolValue useDistanceMatrix) {
      DistanceMatrix result = new DistanceMatrix();
      
      if (useDistanceMatrix.Value) {
        result.UseDistanceMatrix = new BoolValue(true);
        if (distanceMatrix is IValueParameter<DoubleMatrix>) {
          if ((distanceMatrix as IValueParameter<DoubleMatrix>).Value == null) {
            (distanceMatrix as IValueParameter<DoubleMatrix>).Value = CreateDistanceMatrix(coordinates);
          }

          result.Matrix = (distanceMatrix as IValueParameter<DoubleMatrix>).Value;
        } else {
          if (distanceMatrix.ActualValue == null) {
            distanceMatrix.ActualValue = CreateDistanceMatrix(coordinates);
          }

          result.Matrix = (distanceMatrix.ActualValue as DoubleMatrix);
        }
      } else {
        result.UseDistanceMatrix = new BoolValue(false);
        result.Matrix = coordinates;
      }

      return result;
    }

    public static double GetDistance(int start, int end, DistanceMatrix distMatrix) {
      double distance = 0.0;

      if (distMatrix.UseDistanceMatrix.Value)
        distance = distMatrix.Matrix[start, end];
      else
        distance = CalculateDistance(start, end, distMatrix.Matrix);

      return distance;
    }

    public static double GetDistance(int start, int end,
      DoubleMatrix coordinates, IParameter distanceMatrix, BoolValue useDistanceMatrix) {
      double distance = 0.0;

      if (useDistanceMatrix.Value) {
        if (distanceMatrix is IValueParameter<DoubleMatrix>) {
          if ((distanceMatrix as IValueParameter<DoubleMatrix>).Value == null) {
            (distanceMatrix as IValueParameter<DoubleMatrix>).Value = CreateDistanceMatrix(coordinates);
          }
          
          distance = (distanceMatrix as IValueParameter<DoubleMatrix>).Value[start, end];
        } else {
          if (distanceMatrix.ActualValue == null) {
            distanceMatrix.ActualValue = CreateDistanceMatrix(coordinates);
          }

          distance = (distanceMatrix.ActualValue as DoubleMatrix)[start, end];
        }     
      } else {
        distance = CalculateDistance(start, end, coordinates);
      }

      return distance;
    }

    public static double GetDistance(int start, int end,
      DoubleMatrix coordinates, DoubleMatrix distanceMatrix, BoolValue useDistanceMatrix) {
      double distance = 0.0;

      if (useDistanceMatrix.Value) {
        distance = distanceMatrix[start, end];
      } else {
        distance = CalculateDistance(start, end, coordinates);
      }

      return distance;
    }
  }
}
