#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Common {
  public class EmpiricalCumulativeDistributionFunction {
    private static readonly AbscissaComparer abscissaComparer = new AbscissaComparer();
    private static readonly OrdinateComparer ordinateComparer = new OrdinateComparer();

    private List<Point2D<double>> ecdf;
    public IEnumerable<Point2D<double>> SupportingPoints {
      get { return ecdf; }
    }
    
    public EmpiricalCumulativeDistributionFunction(IList<double> sample) {
      ecdf = new List<Point2D<double>>();

      var len = sample.Count;
      var cumulative = 0;
      var localcumulative = 0;
      var prev = double.NaN;
      foreach (var p in sample.OrderBy(x => x)) {
        if (double.IsNaN(p) || double.IsInfinity(p)) continue;
        if (!double.IsNaN(prev) && prev < p) {
          cumulative += localcumulative;
          localcumulative = 0;
          ecdf.Add(Point2D<double>.Create(prev, cumulative / (double)len));
        }
        prev = p;
        localcumulative++;
      }
      if (!double.IsNaN(prev)) {
        cumulative += localcumulative;
        ecdf.Add(Point2D<double>.Create(prev, cumulative / (double)len));
      }
    }
    public EmpiricalCumulativeDistributionFunction(IEnumerable<Point2D<double>> ecdf) {
      this.ecdf = new List<Point2D<double>>();
      var prev = Point2D<double>.Empty;
      foreach (var point in ecdf) {
        if (point.Y < 0 || point.Y > 1 || double.IsNaN(point.X) || double.IsInfinity(point.X)
          || point.IsEmpty || (!prev.IsEmpty && (point.X <= prev.X || point.Y <= prev.Y)))
          throw new ArgumentException("Invalid supporting points of a cumulative distribution function. Must be strictly monotonically increasing in both X and Y with X in R and Y in [0;1].", "ecdf");

        this.ecdf.Add(point);
        prev = point;
      }
    }

    public double Evaluate(double x) {
      if (ecdf.Count == 0) return double.NaN;
      if (x < ecdf[0].X) return 0;
      var last = ecdf[ecdf.Count - 1];
      if (x >= last.X) return last.Y;

      var index = ecdf.BinarySearch(Point2D<double>.Create(x, 0), abscissaComparer);
      if (index >= 0) return ecdf[index].Y;
      return ecdf[~index - 1].Y;
    }

    public double InterpolateLinear(double x) {
      if (ecdf.Count == 0) return double.NaN;
      if (x < ecdf[0].X) return 0;
      var last = ecdf[ecdf.Count - 1];
      if (x >= last.X) return last.Y;

      var index = ecdf.BinarySearch(Point2D<double>.Create(x, 0), abscissaComparer);
      if (index >= 0) return ecdf[index].Y;
      var prev = ecdf[~index - 1];
      var next = ecdf[~index];
      return prev.Y + (next.Y - prev.Y) * ((x - prev.X) / (next.X - prev.X));
    }

    public double InterpolateNearest(double x) {
      if (ecdf.Count == 0) return double.NaN;
      if (x < ecdf[0].X) return 0;
      var last = ecdf[ecdf.Count - 1];
      if (x >= last.X) return last.Y;

      var index = ecdf.BinarySearch(Point2D<double>.Create(x, 0), abscissaComparer);
      if (index >= 0) return ecdf[index].Y;
      var prev = ecdf[~index - 1];
      var next = ecdf[~index];
      if (x - prev.X < next.X - x) return prev.Y;
      return next.Y;
    }

    public double Inverse(double y) {
      if (ecdf.Count == 0) return double.NaN;
      if (y < 0 || y > 1) throw new ArgumentException("parameter must be in interval [0;1]", "y");
      if (ecdf[ecdf.Count - 1].Y < y) return double.PositiveInfinity;
      var index = ecdf.BinarySearch(Point2D<double>.Create(0, y), ordinateComparer);
      if (index >= 0) return ecdf[index].X;
      return ecdf[Math.Max(~index - 1, 0)].X;
    }

    private class AbscissaComparer : Comparer<Point2D<double>> {
      public override int Compare(Point2D<double> x, Point2D<double> y) {
        return x.X.CompareTo(y.X);
      }
    }

    private class OrdinateComparer : Comparer<Point2D<double>> {
      public override int Compare(Point2D<double> x, Point2D<double> y) {
        return x.Y.CompareTo(y.Y);
      }
    }
  }
}
