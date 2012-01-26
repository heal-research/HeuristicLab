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
using HeuristicLab.Data;

namespace HeuristicLab.Analysis {
  public static class MultidimensionalScaling {

    /// <summary>
    /// Performs the Kruskal-Shepard algorithm and applies a gradient descent method
    /// to fit the coordinates such that the difference between the fit distances
    /// and the dissimilarities becomes minimal.
    /// </summary>
    /// <remarks>
    /// It will initialize the coordinates in a deterministic fashion such that all initial points are equally spaced on a circle.
    /// </remarks>
    /// <param name="dissimilarities">A symmetric NxN matrix that specifies the dissimilarities between each element i and j. Diagonal elements are ignored.</param>
    /// 
    /// <returns>A Nx2 matrix where the first column represents the x- and the second column the y coordinates.</returns>
    public static DoubleMatrix KruskalShepard(DoubleMatrix dissimilarities) {
      if (dissimilarities == null) throw new ArgumentNullException("dissimilarities");
      if (dissimilarities.Rows != dissimilarities.Columns) throw new ArgumentException("Dissimilarities must be a square matrix.", "dissimilarities");

      int dimension = dissimilarities.Rows;
      if (dimension == 1) return new DoubleMatrix(new double[,] { { 0, 0 } });
      else if (dimension == 2) return new DoubleMatrix(new double[,] { { 0, 0 }, { 0, dissimilarities[0, 1] } });

      DoubleMatrix coordinates = new DoubleMatrix(dimension, 2);
      double rad = (2 * Math.PI) / coordinates.Rows;
      for (int i = 0; i < dimension; i++) {
        coordinates[i, 0] = 10 * Math.Cos(rad * i);
        coordinates[i, 1] = 10 * Math.Sin(rad * i);
      }

      return KruskalShepard(dissimilarities, coordinates);
    }

    /// <summary>
    /// Performs the Kruskal-Shepard algorithm and applies a gradient descent method
    /// to fit the coordinates such that the difference between the fit distances
    /// and the dissimilarities is minimal.
    /// </summary>
    /// <remarks>
    /// It will use a pre-initialized x,y-coordinates matrix as a starting point of the gradient descent.
    /// </remarks>
    /// <param name="dissimilarities">A symmetric NxN matrix that specifies the dissimilarities between each element i and j. Diagonal elements are ignored.</param>
    /// <param name="coordinates">The Nx2 matrix of initial coordinates.</param>
    /// <param name="maximumIterations">The number of iterations for which the algorithm should run.
    /// In every iteration it tries to find the best location for every item.</param>
    /// <returns>A Nx2 matrix where the first column represents the x- and the second column the y coordinates.</returns>
    public static DoubleMatrix KruskalShepard(DoubleMatrix dissimilarities, DoubleMatrix coordinates, int maximumIterations = 10) {
      int dimension = dissimilarities.Rows;
      if (dimension != dissimilarities.Columns || coordinates.Rows != dimension) throw new ArgumentException("The number of coordinates and the number of rows and columns in the dissimilarities matrix do not match.");

      double epsg = 1e-7;
      double epsf = 0;
      double epsx = 0;
      int maxits = 0;

      alglib.minlmstate state;
      alglib.minlmreport rep;
      for (int iterations = 0; iterations < maximumIterations; iterations++) {
        bool changed = false;
        for (int i = 0; i < dimension; i++) {
          double[] c = new double[] { coordinates[i, 0], coordinates[i, 1] };

          try {
            alglib.minlmcreatevj(dimension - 1, c, out state);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlmoptimize(state, StressFitness, StressJacobian, null, new Info(coordinates, dissimilarities, i));
            alglib.minlmresults(state, out c, out rep);
          } catch (alglib.alglibexception) { }
          if (!double.IsNaN(c[0]) && !double.IsNaN(c[1])) {
            changed = changed || (coordinates[i, 0] != c[0]) || (coordinates[i, 1] != c[1]);
            coordinates[i, 0] = c[0];
            coordinates[i, 1] = c[1];
          }
        }
        if (!changed) break;
      }
      return coordinates;
    }

    private static void StressFitness(double[] x, double[] fi, object obj) {
      Info info = (obj as Info);
      int idx = 0;
      for (int i = 0; i < info.Coordinates.Rows; i++) {
        if (i == info.Row) continue;
        if (!double.IsNaN(info.Dissimilarities[info.Row, i]))
          fi[idx++] = Stress(x, info.Dissimilarities[info.Row, i], info.Coordinates[i, 0], info.Coordinates[i, 1]);
        else fi[idx++] = 0.0;
      }
    }

    private static void StressJacobian(double[] x, double[] fi, double[,] jac, object obj) {
      Info info = (obj as Info);
      int idx = 0;
      for (int i = 0; i < info.Coordinates.Rows; i++) {
        if (i == info.Row) continue;
        double c = info.Dissimilarities[info.Row, i];
        double a = info.Coordinates[i, 0];
        double b = info.Coordinates[i, 1];
        if (!double.IsNaN(c)) {
          fi[idx] = Stress(x, c, a, b); ;
          jac[idx, 0] = 2 * (x[0] - a) * (Math.Sqrt((a - x[0]) * (a - x[0]) + (b - x[1]) * (b - x[1])) - c) / Math.Sqrt((a - x[0]) * (a - x[0]) + (b - x[1]) * (b - x[1]));
          jac[idx, 1] = 2 * (x[1] - b) * (Math.Sqrt((a - x[0]) * (a - x[0]) + (b - x[1]) * (b - x[1])) - c) / Math.Sqrt((a - x[0]) * (a - x[0]) + (b - x[1]) * (b - x[1]));
        } else {
          fi[idx] = jac[idx, 0] = jac[idx, 1] = 0;
        }
        idx++;
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

    /// <summary>
    /// This method computes the normalized raw-stress value according to Groenen and van de Velden 2004. "Multidimensional Scaling". Technical report EI 2004-15.
    /// </summary>
    /// <remarks>
    /// Throws an ArgumentException when the <paramref name="dissimilarities"/> matrix is not symmetric.
    /// </remarks>
    /// 
    /// <param name="dissimilarities">The matrix with the dissimilarities.</param>
    /// <param name="coordinates">The actual location of the points.</param>
    /// <returns>The normalized raw-stress value that describes the goodness-of-fit between the distances in the points and the size of the dissimilarities. If the value is &lt; 0.1 the fit is generally considered good. If between 0.1 and 0.2 it is considered acceptable, but the usefulness of the scaling with higher values is doubtful.</returns>
    public static double CalculateNormalizedStress(DoubleMatrix dissimilarities, DoubleMatrix coordinates) {
      int dimension = dissimilarities.Rows;
      if (dimension != dissimilarities.Columns || dimension != coordinates.Rows) throw new ArgumentException("The number of coordinates and the number of rows and columns in the dissimilarities matrix do not match.");
      double stress = 0, normalization = 0;
      for (int i = 0; i < dimension - 1; i++) {
        for (int j = i + 1; j < dimension; j++) {
          if (dissimilarities[i, j] != dissimilarities[j, i] && !(double.IsNaN(dissimilarities[i, j]) && double.IsNaN(dissimilarities[j, i])))
            throw new ArgumentException("Dissimilarities is not a symmetric matrix.", "dissimilarities");
          if (!double.IsNaN(dissimilarities[i, j])) {
            stress += Stress(coordinates[i, 0], coordinates[i, 1], dissimilarities[i, j], coordinates[j, 0], coordinates[j, 1]);
            normalization += (dissimilarities[i, j] * dissimilarities[i, j]);
          }
        }
      }
      return stress / normalization;
    }

    private class Info {
      public DoubleMatrix Coordinates { get; set; }
      public DoubleMatrix Dissimilarities { get; set; }
      public int Row { get; set; }

      public Info(DoubleMatrix c, DoubleMatrix d, int r) {
        Coordinates = c;
        Dissimilarities = d;
        Row = r;
      }
    }
  }
}