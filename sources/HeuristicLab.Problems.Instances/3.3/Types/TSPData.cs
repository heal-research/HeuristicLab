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

namespace HeuristicLab.Problems.Instances {
  public enum TSPDistanceMeasure { Direct, Euclidean, RoundedEuclidean, UpperEuclidean, Geo, Manhattan, Maximum, Att };

  /// <summary>
  /// Describes instances of the Traveling Salesman Problem (TSP).
  /// </summary>
  public class TSPData {
    /// <summary>
    /// The name of the instance
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Optional! The description of the instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The number of cities.
    /// </summary>
    public int Dimension { get; set; }
    /// <summary>
    /// Specifies the distance measure that is to be used.
    /// </summary>
    public TSPDistanceMeasure DistanceMeasure { get; set; }
    /// <summary>
    /// Optional! The distances are given in form of a distance matrix.
    /// </summary>
    /// <remarks>
    /// Either this property or <see cref="Coordinates"/> needs to be specified.
    /// </remarks>
    public double[,] Distances { get; set; }
    /// <summary>
    /// Optional! A a matrix of dimension [N, 2] matrix where each row is one of the cities
    /// and the colmns represent x and y coordinates respectively.
    /// </summary>
    /// <remarks>
    /// Either this property or <see cref="Distances"/> needs to be specified.
    /// 
    /// If no distance matrix is given, the distances have to be calculated by the
    /// specified distance measure. If a distance matrix is given in addtion to the
    /// coordinates, the distance matrix takes precedence and the coordinates are
    /// for display only.
    /// </remarks>
    public double[,] Coordinates { get; set; }

    /// <summary>
    /// Optional! The best-known tour in path-encoding.
    /// </summary>
    public int[] BestKnownTour { get; set; }
    /// <summary>
    /// Optional! The quality of the best-known tour.
    /// </summary>
    public double? BestKnownQuality { get; set; }

    /// <summary>
    /// If only the coordinates are given, can calculate the distance matrix.
    /// </summary>
    /// <returns>A full distance matrix between all cities.</returns>
    public double[,] GetDistanceMatrix() {
      if (Distances != null) return Distances;
      Distances = new double[Dimension, Dimension];
      for (int i = 0; i < Dimension - 1; i++)
        for (int j = i + 1; j < Dimension; j++) {
          Distances[i, j] = GetDistance(i, j);
          Distances[j, i] = Distances[i, j];
        }
      return Distances;
    }

    #region Private Helpers
    private double GetDistance(int i, int j) {
      switch (DistanceMeasure) {
        case TSPDistanceMeasure.Att:
          return AttDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]);
        case TSPDistanceMeasure.Direct:
          return Distances[i, j];
        case TSPDistanceMeasure.Euclidean:
          return EuclideanDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]);
        case TSPDistanceMeasure.Geo:
          return GeoDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]);
        case TSPDistanceMeasure.Manhattan:
          return ManhattanDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]);
        case TSPDistanceMeasure.Maximum:
          return MaximumDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]);
        case TSPDistanceMeasure.RoundedEuclidean:
          return Math.Round(EuclideanDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]));
        case TSPDistanceMeasure.UpperEuclidean:
          return Math.Ceiling(EuclideanDistance(Coordinates[i, 0], Coordinates[i, 1], Coordinates[j, 0], Coordinates[j, 1]));
        default:
          throw new InvalidOperationException("Distance measure is not known.");
      }
    }

    private double AttDistance(double x1, double y1, double x2, double y2) {
      return Math.Ceiling(Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)) / 10.0));
    }

    private double EuclideanDistance(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    private const double PI = 3.141592;
    private const double RADIUS = 6378.388;
    private double GeoDistance(double x1, double y1, double x2, double y2) {
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

    private double ConvertToRadian(double x) {
      return PI * (Math.Truncate(x) + 5.0 * (x - Math.Truncate(x)) / 3.0) / 180.0;
    }

    private double ManhattanDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Abs(x1 - x2) + Math.Abs(y1 - y2), MidpointRounding.AwayFromZero);
    }

    private double MaximumDistance(double x1, double y1, double x2, double y2) {
      return Math.Max(Math.Round(Math.Abs(x1 - x2), MidpointRounding.AwayFromZero), Math.Round(Math.Abs(y1 - y2), MidpointRounding.AwayFromZero));
    }
    #endregion
  }
}
