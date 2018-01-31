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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Analysis.Statistics {
  public static class KernelDensityEstimator {
    public static double[] Density(double[] x, double mean, double stdDev) {
      return x.Select(xi => Density(xi, mean, stdDev)).ToArray();
    }

    public static double Density(double x, double mean, double stdDev) {
      return (1.0 / (stdDev * Math.Sqrt(2.0 * Math.PI))) *
                  Math.Exp(-((Math.Pow(x - mean, 2.0)) /
                             (2.0 * Math.Pow(stdDev, 2.0))));
    }

    // the scale (sigma) of the kernel is a parameter
    public static List<Tuple<double, double>> Density(double[] x, int nrOfPoints, double stepWidth, double sigma = 1.0) {
      // calculate grid for which to estimate the density
      double[] newX = new double[nrOfPoints];
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

      // for each of the points for which we want to calculate the density
      // we sum up all the densities of the observed points assuming they are at the center of a normal distribution
      var y = from xi in newX
              select (from obsX in x
                      select Density(xi, obsX, sigma)).Sum();

      return newX.Zip(y, Tuple.Create).ToList();
    }

    //Silverman's rule of thumb for bandwidth estimation (sigma)
    public static double EstimateBandwidth(double[] x) {
      return 1.06 * x.StandardDeviation() * Math.Pow(x.Length, -0.2);
    }
  }
}
