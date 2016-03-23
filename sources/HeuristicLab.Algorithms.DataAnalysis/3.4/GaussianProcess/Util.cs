#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Algorithms.DataAnalysis {
  internal static class Util {
    public static double ScalarProd(double[] v, double[] u) {
      if (v.Length != u.Length) throw new InvalidOperationException();
      double prod = 0.0;
      for (int i = 0; i < v.Length; i++)
        prod += v[i] * u[i];
      return prod;
    }

    public static double SqrDist(IEnumerable<double> x, IEnumerable<double> y) {
      return x.Zip(y, (a, b) => (a - b) * (a - b)).Sum();
    }

    public static double SqrDist(double x, double y) {
      double d = x - y;
      return d * d;
    }

    public static double SqrDist(double[,] x, int i, int j, int[] columnIndices, double scale = 1.0) {
      return SqrDist(x, i, x, j, columnIndices, scale);
    }

    public static double SqrDist(double[,] x, int i, double[,] xt, int j, int[] columnIndices, double scale = 1.0) {
      double ss = 0.0;
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1)).ToArray();
      for (int c = 0; c < columnIndices.Length; c++) {
        var columnIndex = columnIndices[c];
        double d = x[i, columnIndex] - xt[j, columnIndex];
        ss += d * d;
      }
      return scale * scale * ss;
    }

    public static double SqrDist(double[,] x, int i, int j, double[] scale, int[] columnIndices) {
      return SqrDist(x, i, x, j, scale, columnIndices);
    }

    public static double SqrDist(double[,] x, int i, double[,] xt, int j, double[] scale, int[] columnIndices) {
      double ss = 0.0;
      int scaleIndex = 0;
      for (int c = 0; c < columnIndices.Length; c++) {
        var columnIndex = columnIndices[c];
        double d = x[i, columnIndex] - xt[j, columnIndex];
        ss += d * d * scale[scaleIndex] * scale[scaleIndex];
        scaleIndex++;
      }
      // must be at the end of scale after iterating over columnIndices
      if (scaleIndex != scale.Length)
        throw new ArgumentException("Lengths of scales and covariance functions does not match.");
      return ss;
    }
    public static double ScalarProd(double[,] x, int i, int j, int[] columnIndices, double scale = 1.0) {
      return ScalarProd(x, i, x, j, columnIndices, scale);
    }

    public static double ScalarProd(double[,] x, int i, double[,] xt, int j, int[] columnIndices, double scale = 1.0) {
      double sum = 0.0;
      for (int c = 0; c < columnIndices.Length; c++) {
        var columnIndex = columnIndices[c];
        sum += x[i, columnIndex] * xt[j, columnIndex];
      }
      return scale * scale * sum;
    }
    public static double ScalarProd(double[,] x, int i, int j, double[] scale, int[] columnIndices) {
      return ScalarProd(x, i, x, j, scale, columnIndices);
    }

    public static double ScalarProd(double[,] x, int i, double[,] xt, int j, double[] scale, int[] columnIndices) {
      double sum = 0.0;
      int scaleIndex = 0;
      for (int c = 0; c < columnIndices.Length; c++, scaleIndex++) {
        var columnIndex = columnIndices[c];
        sum += x[i, columnIndex] * scale[scaleIndex] * xt[j, columnIndex] * scale[scaleIndex];
      }
      // must be at the end of scale after iterating over columnIndices
      if (scaleIndex != scale.Length)
        throw new ArgumentException("Lengths of scales and covariance functions does not match.");

      return sum;
    }

    public static IEnumerable<double> GetRow(double[,] x, int r) {
      int cols = x.GetLength(1);
      return GetRow(x, r, Enumerable.Range(0, cols));
    }
    public static IEnumerable<double> GetRow(double[,] x, int r, IEnumerable<int> columnIndices) {
      return columnIndices.Select(c => x[r, c]);
    }
    public static IEnumerable<double> GetCol(double[,] x, int c) {
      int rows = x.GetLength(0);
      return Enumerable.Range(0, rows).Select(r => x[r, c]);
    }
  }
}
