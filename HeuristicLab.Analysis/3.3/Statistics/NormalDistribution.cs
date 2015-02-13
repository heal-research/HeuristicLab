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
using HeuristicLab.Common;

namespace HeuristicLab.Analysis.Statistics {
  public static class NormalDistribution {
    public static double[] Density(double[] x, double mean, double stdDev) {
      double[] result = new double[x.Length];

      for (int i = 0; i < x.Length; i++) {
        result[i] = (1.0 / (stdDev * Math.Sqrt(2.0 * Math.PI))) *
                    Math.Exp(-((Math.Pow(x[i] - mean, 2.0)) /
                               (2.0 * Math.Pow(stdDev, 2.0))));
      }

      return result;
    }

    // based on the idea from http://www.statmethods.net/graphs/density.html
    public static List<Tuple<double, double>> Density(double[] x, int nrOfPoints, double stepWidth) {
      double[] newX = new double[nrOfPoints];
      double mean = x.Average();
      double stdDev = x.StandardDeviation();
      double margin = stepWidth * 2;

      double dataMin = x.Min() - margin;
      double dataMax = x.Max() + margin;
      double diff = (dataMax - dataMin) / nrOfPoints;
      double cur = dataMin;
      newX[0] = cur;
      for (int i = 1; i < nrOfPoints; i++) {
        cur += diff;
        newX[i] = cur;
      }

      var y = Density(newX, mean, stdDev).Select(k => k * stepWidth * x.Length).ToList();

      var points = new List<Tuple<double, double>>();
      for (int i = 0; i < newX.Length; i++) {
        points.Add(new Tuple<double, double>(newX[i], y[i]));
      }

      return points;
    }
  }
}
