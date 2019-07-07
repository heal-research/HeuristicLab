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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  internal static class Utilities {
    internal static double MinimumDistance(double[] point, IEnumerable<double[]> points) {
      if (point == null) throw new ArgumentNullException("Point must not be null.");
      if (points == null) throw new ArgumentNullException("Points must not be null.");
      if (!points.Any()) throw new ArgumentException("Points must not be empty.");

      double minDistance = double.MaxValue;
      foreach (double[] r in points) {
        if (r.Length != point.Length) throw new ArgumentException("Dimensions of Points and Point do not match.");

        double squaredDistance = 0;
        for (int i = 0; i < r.Length; i++) {
          squaredDistance += (point[i] - r[i]) * (point[i] - r[i]);
        }
        minDistance = Math.Min(squaredDistance, minDistance);
      }

      return Math.Sqrt(minDistance);
    }

    internal static DoubleMatrix ToMatrix(IEnumerable<double[]> source) {
      try {
        int firstDimension = source.Count();
        int secondDimension = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

        var result = new DoubleMatrix(firstDimension, secondDimension);
        var enumarator = source.GetEnumerator();
        for (int i = 0; i < firstDimension && enumarator.MoveNext(); ++i)
          for (int j = 0; j < secondDimension; ++j)
            result[i, j] = enumarator.Current[j];
        return result;
      }
      catch (InvalidOperationException) {
        throw new InvalidOperationException("The given jagged array is not rectangular.");
      }
    }

    internal class DimensionComparer : IComparer<double[]> {
      private readonly int dim;
      private readonly int descending;

      public DimensionComparer(int dimension, bool descending) {
        this.dim = dimension;
        this.descending = descending ? -1 : 1;
      }

      public int Compare(double[] x, double[] y) {
        if (x[dim] < y[dim]) return -descending;
        else if (x[dim] > y[dim]) return descending;
        else return 0;
      }
    }
  }
}
