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

namespace HeuristicLab.Algorithms.DataAnalysis {
  public static class Util {
    public static double ScalarProd(IEnumerable<double> v, IEnumerable<double> u) {
      return v.Zip(u, (vi, ui) => vi * ui).Sum();
    }

    public static double SqrDist(double x, double y) {
      double d = x - y;
      return Math.Max(d * d, 0.0);
    }

    public static double SqrDist(IEnumerable<double> x, IEnumerable<double> y) {
      return x.Zip(y, SqrDist).Sum();
    }

    public static IEnumerable<double> GetRow(double[,] x, int r) {
      int cols = x.GetLength(1);
      return Enumerable.Range(0, cols).Select(c => x[r, c]);
    }
    public static IEnumerable<double> GetCol(double[,] x, int c) {
      int rows = x.GetLength(0);
      return Enumerable.Range(0, rows).Select(r => x[r, c]);
    }
  }
}
