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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Algorithms.DataAnalysis {
  internal static class Util {
    public static double ScalarProd(IEnumerable<double> v, IEnumerable<double> u) {
      return v.Zip(u, (vi, ui) => vi * ui).Sum();
    }

    public static double SqrDist(IEnumerable<double> x, IEnumerable<double> y) {
      return x.Zip(y, (a, b) => (a - b) * (a - b)).Sum();
    }

    public static double SqrDist(double x, double y) {
      double d = x - y;
      return d * d;
    }

    public static double SqrDist(double[,] x, int i, int j, double scale = 1.0, IEnumerable<int> columnIndices = null) {
      return SqrDist(x, i, x, j, scale, columnIndices);
    }

    public static double SqrDist(double[,] x, int i, double[,] xt, int j, double scale = 1.0, IEnumerable<int> columnIndices = null) {
      double ss = 0.0;
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      foreach (int columnIndex in columnIndices) {
        double d = x[i, columnIndex] - xt[j, columnIndex];
        ss += d * d;
      }
      return scale * scale * ss;
    }

    public static double SqrDist(double[,] x, int i, int j, double[] scale, IEnumerable<int> columnIndices = null) {
      return SqrDist(x, i, x, j, scale, columnIndices);
    }

    public static double SqrDist(double[,] x, int i, double[,] xt, int j, double[] scale, IEnumerable<int> columnIndices = null) {
      double ss = 0.0;
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      int scaleIndex = 0;
      foreach (int columnIndex in columnIndices) {
        double d = x[i, columnIndex] - xt[j, columnIndex];
        ss += d * d * scale[scaleIndex] * scale[scaleIndex];
        scaleIndex++;
      }
      // must be at the end of scale after iterating over columnIndices
      if (scaleIndex != scale.Length)
        throw new ArgumentException("Lengths of scales and covariance functions does not match.");
      return ss;
    }
    public static double ScalarProd(double[,] x, int i, int j, double scale = 1.0, IEnumerable<int> columnIndices = null) {
      return ScalarProd(x, i, x, j, scale, columnIndices);
    }

    public static double ScalarProd(double[,] x, int i, double[,] xt, int j, double scale = 1.0, IEnumerable<int> columnIndices = null) {
      double sum = 0.0;
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      foreach (int columnIndex in columnIndices) {
        sum += x[i, columnIndex] * xt[j, columnIndex];
      }
      return scale * scale * sum;
    }
    public static double ScalarProd(double[,] x, int i, int j, double[] scale, IEnumerable<int> columnIndices = null) {
      return ScalarProd(x, i, x, j, scale, columnIndices);
    }

    public static double ScalarProd(double[,] x, int i, double[,] xt, int j, double[] scale, IEnumerable<int> columnIndices = null) {
      double sum = 0.0;
      if (columnIndices == null) columnIndices = Enumerable.Range(0, x.GetLength(1));
      int scaleIndex = 0;
      foreach (int columnIndex in columnIndices) {
        sum += x[i, columnIndex] * scale[scaleIndex] * xt[j, columnIndex] * scale[scaleIndex];
        scaleIndex++;
      }
      // must be at the end of scale after iterating over columnIndices
      if (scaleIndex != scale.Length)
        throw new ArgumentException("Lengths of scales and covariance functions does not match.");

      return sum;
    }

    public static IEnumerable<double> GetRow(double[,] x, int r) {
      int cols = x.GetLength(1);
      return Enumerable.Range(0, cols).Select(c => x[r, c]);
    }
    public static IEnumerable<double> GetCol(double[,] x, int c) {
      int rows = x.GetLength(0);
      return Enumerable.Range(0, rows).Select(r => x[r, c]);
    }


    public static void AttachValueChangeHandler<T, U>(IValueParameter<T> parameter, Action action)
      where T : ValueTypeValue<U>
      where U : struct {
      parameter.ValueChanged += (sender, args) => {
        if (parameter.Value != null) {
          parameter.Value.ValueChanged += (s, a) => action();
          action();
        }
      };
      if (parameter.Value != null) {
        parameter.Value.ValueChanged += (s, a) => action();
      }
    }

    public static void AttachArrayChangeHandler<T, U>(IValueParameter<T> parameter, Action action)
      where T : ValueTypeArray<U>
      where U : struct {
      parameter.ValueChanged += (sender, args) => {
        if (parameter.Value != null) {
          parameter.Value.ItemChanged += (s, a) => action();
          parameter.Value.Reset += (s, a) => action();
          action();
        }
      };
      if (parameter.Value != null) {
        parameter.Value.ItemChanged += (s, a) => action();
        parameter.Value.Reset += (s, a) => action();
      }
    }
  }
}
