

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
