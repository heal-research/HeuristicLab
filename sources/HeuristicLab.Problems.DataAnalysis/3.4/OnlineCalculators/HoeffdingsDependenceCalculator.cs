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
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis {
  public class HoeffdingsDependenceCalculator {

    public static double Calculate(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues, out OnlineCalculatorError errorState) {
      double d = HoeffD(originalValues, estimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return d;
    }

    /// <summary>
    /// computes Hoeffding's dependence coefficient. 
    /// Source: hoeffd.r from R package hmisc http://cran.r-project.org/web/packages/Hmisc/index.html
    /// </summary>
    private static double HoeffD(IEnumerable<double> xs, IEnumerable<double> ys, out OnlineCalculatorError errorState) {
      double[] rx = TiedRank(xs);
      double[] ry = TiedRank(ys);
      if (rx.Length != ry.Length) throw new ArgumentException("The number of elements in xs and ys does not match");
      double[] rxy = TiedRank(xs, ys);

      int n = rx.Length;
      double q = 0, r = 0, s = 0;
      double scaling = 1.0 / (n * (n - 1));
      for (int i = 0; i < n; i++) {
        q += (rx[i] - 1) * (rx[i] - 2) * (ry[i] - 1) * (ry[i] - 2) * scaling;
        r += (rx[i] - 2) * (ry[i] - 2) * rxy[i] * scaling;
        s += rxy[i] * (rxy[i] - 1) * scaling;
      }
      errorState = OnlineCalculatorError.None;
      // return 30.0 * (q - 2 * (n - 2) * r + (n - 2) * (n - 3) * s) / n / (n - 1) / (n - 2) / (n - 3) / (n - 4);
      double t0 = q / (n - 2) / (n - 3) / (n - 4);
      double t1 = 2 * r / (n - 3) / (n - 4);
      double t2 = s / (n - 4);
      return 30.0 * (t0 - t1 + t2);
    }

    private static double[] TiedRank(IEnumerable<double> xs) {
      var xsArr = xs.ToArray();
      var idx = Enumerable.Range(1, xsArr.Length).ToArray();
      Array.Sort(xsArr, idx);
      CRank(xsArr);
      Array.Sort(idx, xsArr);
      return xsArr;
    }

    /// <summary>
    /// Calculates the joint rank with midranks for ties. Source: hoeffd.r from R package hmisc http://cran.r-project.org/web/packages/Hmisc/index.html
    /// </summary>
    /// <param name="xs"></param>
    /// <param name="ys"></param>
    /// <returns></returns>
    private static double[] TiedRank(IEnumerable<double> xs, IEnumerable<double> ys) {
      var xsArr = xs.ToArray();
      var ysArr = ys.ToArray();
      var r = new double[xsArr.Length];
      int n = r.Length;
      for (int i = 0; i < n; i++) {
        var xi = xsArr[i];
        var yi = ysArr[i];
        double ri = 0.0;
        for (int j = 0; j < n; j++) {
          if (i != j) {
            double cx;
            if (xsArr[j] < xi) cx = 1.0;
            else if (xsArr[j] > xi) cx = 0.0;
            else cx = 0.5;  // eq
            double cy;
            if (ysArr[j] < yi) cy = 1.0;
            else if (ysArr[j] > yi) cy = 0.0;
            else cy = 0.5; // eq
            ri = ri + cx * cy;
          }
        }
        r[i] = ri;
      }
      return r;
    }

    /// <summary>
    /// Calculates midranks. Source: Numerical Recipes in C. p 642
    /// </summary>
    /// <param name="w">Sorted array of elements, replaces the elements by their rank, including midranking of ties</param>
    /// <returns></returns>
    private static void CRank(double[] w) {
      int i = 0;
      int n = w.Length;
      while (i < n - 1) {
        if (w[i + 1] > w[i]) {    // w[i+1] must be larger or equal w[i] as w must be sorted
          // not a tie
          w[i] = i + 1;
          i++;
        } else {
          int j;
          for (j = i + 1; j < n && w[j] <= w[i]; j++) ; // how far does it go (<= effectively means == as w must be sorted, side-step equality for double values)
          double rank = 1 + 0.5 * (i + j - 1);
          int k;
          for (k = i; k < j; k++) w[k] = rank; // set the rank for all tied entries
          i = j;
        }
      }

      if (i == n - 1) w[n - 1] = n;   // if the last element was not tied, this is its rank
    }
  }
}
