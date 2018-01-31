#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public void Calculate(double[] dataPoints, out double slope, out double intercept) {
      var stdX = Enumerable.Range(0, dataPoints.Count()).Select(x => (double)x).ToArray();
      Calculate(dataPoints, stdX, out slope, out intercept);
    }

    public void Calculate(double[] y, double[] x, out double slope, out double intercept) {
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

      slope = (sxy - (n * avgx * avgy)) / (sxx - (n * avgx * avgx));
      intercept = avgy - slope * avgx;
    }

    public double CalculateError(double[] dataPoints, double slope, double intercept) {
      double r;
      double avgy = dataPoints.Average();
      double sstot = 0.0;
      double sserr = 0.0;

      for (int i = 0; i < dataPoints.Count(); i++) {
        double y = slope * i + intercept;
        sstot += Math.Pow(dataPoints[i] - avgy, 2);
        sserr += Math.Pow(dataPoints[i] - y, 2);
      }

      r = 1.0 - (sserr / sstot);
      return r;
    }

    public DataRow CalculateFittedLine(double[] y, double[] x) {
      double slope, intercept;
      Calculate(y, x, out slope, out intercept);

      DataRow newRow = new DataRow();
      for (int i = 0; i < x.Count(); i++) {
        newRow.Values.Add(slope * x[i] + intercept);
      }
      return newRow;
    }

    public DataRow CalculateFittedLine(double[] dataPoints) {
      DataRow newRow = new DataRow();
      double slope, intercept;
      Calculate(dataPoints, out slope, out intercept);
      var stdX = Enumerable.Range(0, dataPoints.Count()).Select(x => (double)x).ToArray();

      for (int i = 0; i < stdX.Count(); i++) {
        newRow.Values.Add(slope * stdX[i] + intercept);
      }

      return newRow;
    }

    public override string ToString() {
      return "Linear Fitting";
    }
  }
}
