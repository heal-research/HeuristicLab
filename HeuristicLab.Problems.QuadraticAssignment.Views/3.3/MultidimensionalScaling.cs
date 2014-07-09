#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  public static class MultidimensionalScaling {

    /// <summary>
    /// Performs the Kruskal-Shepard algorithm and applies a gradient descent method
    /// to fit the coordinates such that the difference between the fit distances
    /// and the actual distances is minimal.
    /// </summary>
    /// <param name="distances">A symmetric NxN matrix that specifies the distances between each element i and j. Diagonal elements are ignored.</param>
    /// <param name="stress">Returns the stress between the fit distances and the actual distances.</param>
    /// <returns>A Nx2 matrix where the first column represents the x- and the second column the y coordinates.</returns>
    public static DoubleMatrix MetricByDistance(DoubleMatrix distances, out double stress) {
      if (distances == null) throw new ArgumentNullException("distances");
      if (distances.Rows != distances.Columns) throw new ArgumentException("Distance matrix must be a square matrix.", "distances");
      stress = 0.0;

      int dimension = distances.Rows;
      if (dimension == 1) return new DoubleMatrix(new double[,] { { 0, 0 } });
      else if (dimension == 2) return new DoubleMatrix(new double[,] { { 0, distances[0, 1] } });

      DoubleMatrix coordinates = new DoubleMatrix(dimension, 2);
      double rad = (2 * Math.PI) / coordinates.Rows;
      for (int i = 0; i < dimension; i++) {
        coordinates[i, 0] = 10 * Math.Cos(rad * i);
        coordinates[i, 1] = 10 * Math.Sin(rad * i);
      }

      double epsg = 1e-7;
      double epsf = 0;
      double epsx = 0;
      int maxits = 0;
      alglib.mincgstate state = null;
      alglib.mincgreport rep;

      for (int iterations = 0; iterations < 10; iterations++) {
        for (int i = 0; i < dimension; i++) {
          double[] c = new double[] { coordinates[i, 0], coordinates[i, 1] };

          if (iterations == 0 && i == 0) {
            alglib.mincgcreate(c, out state);
            alglib.mincgsetcond(state, epsg, epsf, epsx, maxits);
          } else {
            alglib.mincgrestartfrom(state, c);
          }
          alglib.mincgoptimize(state, StressGradient, null, new Info(coordinates, distances, i));
          alglib.mincgresults(state, out c, out rep);

          coordinates[i, 0] = c[0];
          coordinates[i, 1] = c[1];
        }
      }
      stress = CalculateNormalizedStress(dimension, distances, coordinates);
      return coordinates;
    }

    private static void StressGradient(double[] x, ref double func, double[] grad, object obj) {
      func = 0; grad[0] = 0; grad[1] = 0;
      Info info = (obj as Info);
      for (int i = 0; i < info.Coordinates.Rows; i++) {
        double c = info.Distances[info.Row, i];
        if (i != info.Row) {
          double a = info.Coordinates[i, 0];
          double b = info.Coordinates[i, 1];
          func += Stress(x, c, a, b);
          grad[0] += ((2 * x[0] - 2 * a) * Math.Sqrt(x[1] * x[1] - 2 * b * x[1] + x[0] * x[0] - 2 * a * x[0] + b * b + a * a) - 2 * c * x[0] + 2 * a * c) / Math.Sqrt(x[1] * x[1] - 2 * b * x[1] + x[0] * x[0] - 2 * a * x[0] + b * b + a * a);
          grad[1] += ((2 * x[1] - 2 * b) * Math.Sqrt(x[1] * x[1] - 2 * b * x[1] + x[0] * x[0] - 2 * a * x[0] + b * b + a * a) - 2 * c * x[1] + 2 * b * c) / Math.Sqrt(x[1] * x[1] - 2 * b * x[1] + x[0] * x[0] - 2 * a * x[0] + b * b + a * a);
        }
      }
    }

    private static double Stress(double[] x, double distance, double xCoord, double yCoord) {
      return Stress(x[0], x[1], distance, xCoord, yCoord);
    }

    private static double Stress(double x, double y, double distance, double otherX, double otherY) {
      double d = Math.Sqrt((x - otherX) * (x - otherX)
                         + (y - otherY) * (y - otherY));
      return (d - distance) * (d - distance);
    }

    public static double CalculateNormalizedStress(int dimension, DoubleMatrix distances, DoubleMatrix coordinates) {
      double stress = 0;
      for (int i = 0; i < dimension - 1; i++) {
        for (int j = i + 1; j < dimension; j++) {
          if (distances[i, j] != 0) {
            stress += Stress(coordinates[i, 0], coordinates[i, 1], distances[i, j], coordinates[j, 0], coordinates[j, 1])
              / (distances[i, j] * distances[i, j]);
          }
        }
      }
      return stress;
    }

    private class Info {
      public DoubleMatrix Coordinates { get; set; }
      public DoubleMatrix Distances { get; set; }
      public int Row { get; set; }

      public Info(DoubleMatrix c, DoubleMatrix d, int r) {
        Coordinates = c;
        Distances = d;
        Row = r;
      }
    }
  }
}