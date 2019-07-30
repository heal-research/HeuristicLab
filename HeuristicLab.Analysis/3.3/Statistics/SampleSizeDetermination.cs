#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;

namespace HeuristicLab.Analysis.Statistics {
  public static class SampleSizeDetermination {
    /// <summary>
    /// Determines for a given sample the required sample size as described in 
    /// Göran Kauermann, Helmut Küchenhoff: Stichproben: Methoden und praktische Umsetzung mit R, section 2.27. 
    /// </summary>
    /// <param name="samples">The pilot sample.</param>
    /// <param name="conf">Confidence Interval.</param>
    /// <returns>Number of required samples for the given confidence interval. </returns>
    public static int DetermineSampleSizeByEstimatingMean(double[] samples, double conf = 0.95) {
      if (conf < 0.0 || conf > 1.0) throw new ArgumentException("The confidence interval must be between zero and one.");

      var confInterval = samples.ConfidenceIntervals(conf);
      double e = (confInterval.Item2 - confInterval.Item1) / 2;
      double s = samples.StandardDeviation();
      double z = alglib.invnormaldistribution((conf + 1) / 2);
      double n = samples.Count();

      double result = Math.Pow(s, 2) / ((Math.Pow(e, 2) / Math.Pow(z, 2)) + (Math.Pow(s, 2) / n));

      result = Math.Ceiling(result);
      if (result > int.MaxValue)
        return int.MaxValue;
      else
        return (int)result;
    }

    /// <summary>
    /// Calculates Cohen's d.
    /// </summary>
    /// <returns>Cohen's d. 
    /// d = 0.2 means small effect
    /// d = 0.5 means medium effect
    /// d = 0.8 means big effect
    /// According to Wikipedia this means: "A lower Cohen's d indicates a necessity of larger sample sizes, and vice versa."
    /// </returns>
    public static double CalculateCohensD(double[] d1, double[] d2) {
      double x1, x2, s1, s2;

      x1 = d1.Average();
      x2 = d2.Average();
      s1 = d1.Variance();
      s2 = d2.Variance();

      return Math.Abs(x1 - x2) / Math.Sqrt((s1 + s2) / 2);
    }

    /// <summary>
    /// Calculates Hedges' g. 
    /// Hedges' g works like Cohen's d but corrects for bias. 
    /// </summary>
    /// <returns>Hedges' g</returns>
    public static double CalculateHedgesG(double[] d1, double[] d2) {
      double x1, x2, s1, s2, n1, n2, s, g, c;

      x1 = d1.Average();
      x2 = d2.Average();
      s1 = d1.Variance();
      s2 = d2.Variance();
      n1 = d1.Count();
      n2 = d2.Count();

      s = Math.Sqrt(((n1 - 1) * s1 + (n2 - 1) * s2) / (n1 + n2 - 2));
      g = Math.Abs(x1 - x2) / s;
      c = (1 - (3 / (4 * (n1 + n2) - 9))) * g;

      return c;
    }
  }
}
