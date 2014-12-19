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
using System.Linq;

namespace HeuristicLab.Analysis.Statistics {
  public class LinearLeastSquaresFitting : IFitting {
    public void Calculate(double[] dataPoints, out double p0, out double p1) {
      var stdX = Enumerable.Range(0, dataPoints.Count()).Select(x => (double)x).ToArray();
      Calculate(dataPoints, stdX, out p0, out p1);
    }

    public void Calculate(double[] y, double[] x, out double p0, out double p1) {
      if (y.Count() != x.Count()) {
        throw new ArgumentException("The lenght of x and y needs do be equal. ");
      }

      double sxy = 0.0;
      double sxx = 0.0;
      int n = y.Count();
      double sy = y.Sum();
      double sx = ((n - 1) * n) / 2.0;
      double avgy = sy / n;
      double avgx = sx / n;

      for (int i = 0; i < n; i++) {
        sxy += x[i] * y[i];
        sxx += x[i] * x[i];
      }

      p0 = (sxy - (n * avgx * avgy)) / (sxx - (n * avgx * avgx));
      p1 = avgy - p0 * avgx;
    }

    public double CalculateError(double[] dataPoints, double p0, double p1) {
      double r;
      double avgy = dataPoints.Average();
      double sstot = 0.0;
      double sserr = 0.0;

      for (int i = 0; i < dataPoints.Count(); i++) {
        double y = p0 * i + p1;
        sstot += Math.Pow(dataPoints[i] - avgy, 2);
        sserr += Math.Pow(dataPoints[i] - y, 2);
      }

      r = 1.0 - (sserr / sstot);
      return r;
    }

    public DataRow CalculateFittedLine(double[] y, double[] x, string rowName) {
      double k, d;
      Calculate(y, x, out k, out d);

      DataRow newRow = new DataRow(rowName);
      for (int i = 0; i < x.Count(); i++) {
        newRow.Values.Add(k * x[i] + d);
      }
      return newRow;
    }

    public DataRow CalculateFittedLine(double[] dataPoints, string rowName) {
      DataRow newRow = new DataRow(rowName);
      double c0, c1;
      Calculate(dataPoints, out c0, out c1);
      var stdX = Enumerable.Range(0, dataPoints.Count()).Select(x => (double)x).ToArray();

      for (int i = 0; i < stdX.Count(); i++) {
        newRow.Values.Add(c0 * stdX[i] + c1);
      }

      return newRow;
    }

    public override string ToString() {
      return "Linear Fitting";
    }
  }
}
