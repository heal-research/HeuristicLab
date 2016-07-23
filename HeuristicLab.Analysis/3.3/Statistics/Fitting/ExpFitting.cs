#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ExpFitting : IFitting {
    private void ExpFunc(double[] c, double[] x, ref double func, object obj) {
      func = Math.Exp(-c[0] * Math.Pow(x[0], 2));
    }

    private double[] GetDefaultXValues(int n) {
      var stdX = Enumerable.Range(1, n).Select(x => (double)x).ToArray();
      return stdX;
    }

    public void Calculate(double[] dataPoints, out double p0) {
      var stdX = GetDefaultXValues(dataPoints.Count());
      Calculate(dataPoints, stdX, out p0);
    }

    public void Calculate(double[] y, double[] x, out double p0) {
      if (y.Count() != x.Count()) {
        throw new ArgumentException("The lenght of x and y needs do be equal. ");
      }

      double[] c = new double[] { 0.3 };
      double epsf = 0;
      double epsx = 0.000001;
      int maxits = 0;
      int info;
      alglib.lsfitstate state;
      alglib.lsfitreport rep;
      double diffstep = 0.0001;
      double[,] xx = new double[x.Count(), 1];

      for (int i = 0; i < x.Count(); i++) {
        xx[i, 0] = x[i];
      }

      alglib.lsfitcreatef(xx, y, c, diffstep, out state);
      alglib.lsfitsetcond(state, epsf, epsx, maxits);
      alglib.lsfitfit(state, ExpFunc, null, null);
      alglib.lsfitresults(state, out info, out c, out rep);

      p0 = c[0];
    }

    public DataRow CalculateFittedLine(double[] dataPoints) {
      DataRow newRow = new DataRow();
      double c0;
      Calculate(dataPoints, out c0);
      var stdX = GetDefaultXValues(dataPoints.Count());

      for (int i = 0; i < stdX.Count(); i++) {
        newRow.Values.Add(Math.Exp(-c0 * Math.Pow(stdX[i], 2)));
      }

      return newRow;
    }

    public DataRow CalculateFittedLine(double[] y, double[] x) {
      DataRow newRow = new DataRow();
      double c0;
      Calculate(y, x, out c0);

      for (int i = 0; i < x.Count(); i++) {
        newRow.Values.Add(Math.Exp(-c0 * Math.Pow(x[i], 2)));
      }

      return newRow;
    }

    public override string ToString() {
      return "Exponential Fitting";
    }
  }
}
