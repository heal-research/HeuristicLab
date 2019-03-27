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

namespace HeuristicLab.Problems.Instances {
  public enum DistanceMeasure { Direct, Euclidean, RoundedEuclidean, UpperEuclidean, Geo, Manhattan, Maximum, Att };

  public static class DistanceHelper {
    /// <summary>
    /// If only the coordinates are given, can calculate the distance matrix.
    /// </summary>
    /// <returns>A full distance matrix between all cities.</returns>
    public static double[,] GetDistanceMatrix(DistanceMeasure distanceMeasure, double[,] coordinates, double[,] distances, int dimension) {
      if (distances != null) return distances;

      distances = new double[dimension, dimension];
      for (int i = 0; i < dimension - 1; i++)
        for (int j = i + 1; j < dimension; j++) {
          distances[i, j] = GetDistance(i, j, distanceMeasure, coordinates, distances);
          distances[j, i] = distances[i, j];
        }

      return distances;
    }

    public static double GetDistance(DistanceMeasure distanceMeasure, double x1, double y1, double x2, double y2) {
      switch (distanceMeasure) {
        case DistanceMeasure.Att:
          return AttDistance(x1, y1, x2, y2);
        case DistanceMeasure.Direct:
          throw new ArgumentException("Direct distance measure requires distance matrix for distance calculation.");
        case DistanceMeasure.Euclidean:
          return EuclideanDistance(x1, y1, x2, y2);
        case DistanceMeasure.Geo:
          return GeoDistance(x1, y1, x2, y2);
        case DistanceMeasure.Manhattan:
          return ManhattanDistance(x1, y1, x2, y2);
        case DistanceMeasure.Maximum:
          return MaximumDistance(x1, y1, x2, y2);
        case DistanceMeasure.RoundedEuclidean:
          return Math.Round(EuclideanDistance(x1, y1, x2, y2));
        case DistanceMeasure.UpperEuclidean:
          return Math.Ceiling(EuclideanDistance(x1, y1, x2, y2));
        default:
          throw new InvalidOperationException("Distance measure is not known.");
      }
    }

    #region Private Helpers
    private static double GetDistance(int i, int j, DistanceMeasure distanceMeasure, double[,] coordinates, double[,] distances) {
      switch (distanceMeasure) {
        case DistanceMeasure.Att:
          return AttDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
        case DistanceMeasure.Direct:
          return distances[i, j];
        case DistanceMeasure.Euclidean:
          return EuclideanDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
        case DistanceMeasure.Geo:
          return GeoDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
        case DistanceMeasure.Manhattan:
          return ManhattanDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
        case DistanceMeasure.Maximum:
          return MaximumDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
        case DistanceMeasure.RoundedEuclidean:
          return Math.Round(EuclideanDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]));
        case DistanceMeasure.UpperEuclidean:
          return Math.Ceiling(EuclideanDistance(coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]));
        default:
          throw new InvalidOperationException("Distance measure is not known.");
      }
    }

    private static double AttDistance(double x1, double y1, double x2, double y2) {
      return Math.Ceiling(Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) / 10.0));
    }

    private static double EuclideanDistance(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    private const double PI = 3.141592;
    private const double RADIUS = 6378.388;
    private static double GeoDistance(double x1, double y1, double x2, double y2) {
      double latitude1, longitude1, latitude2, longitude2;
      double q1, q2, q3;
      double length;

      latitude1 = ConvertToRadian(x1);
      longitude1 = ConvertToRadian(y1);
      latitude2 = ConvertToRadian(x2);
      longitude2 = ConvertToRadian(y2);

      q1 = Math.Cos(longitude1 - longitude2);
      q2 = Math.Cos(latitude1 - latitude2);
      q3 = Math.Cos(latitude1 + latitude2);

      length = (int)(RADIUS * Math.Acos(0.5 * ((1.0 + q1) * q2 - (1.0 - q1) * q3)) + 1.0);
      return (length);
    }

    private static double ConvertToRadian(double x) {
      return PI * (Math.Truncate(x) + 5.0 * (x - Math.Truncate(x)) / 3.0) / 180.0;
    }

    private static double ManhattanDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Abs(x1 - x2) + Math.Abs(y1 - y2), MidpointRounding.AwayFromZero);
    }

    private static double MaximumDistance(double x1, double y1, double x2, double y2) {
      return Math.Max(Math.Round(Math.Abs(x1 - x2), MidpointRounding.AwayFromZero), Math.Round(Math.Abs(y1 - y2), MidpointRounding.AwayFromZero));
    }
    #endregion
  }
}
