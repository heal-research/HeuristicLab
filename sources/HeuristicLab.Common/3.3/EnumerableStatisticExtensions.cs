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

namespace HeuristicLab.Common {
  public static class EnumerableStatisticExtensions {
    /// <summary>
    /// Calculates the median element of the enumeration.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Median(this IEnumerable<double> values) {
      // iterate only once 
      double[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      Array.Sort(valuesArr);

      // return the middle element (if n is uneven) or the average of the two middle elements if n is even.
      if (n % 2 == 1) {
        return valuesArr[n / 2];
      } else {
        return (valuesArr[(n / 2) - 1] + valuesArr[n / 2]) / 2.0;
      }
    }

    /// <summary>
    /// Calculates the range (max - min) of the enumeration.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Range(this IEnumerable<double> values) {
      double min = double.PositiveInfinity;
      double max = double.NegativeInfinity;
      int i = 0;
      foreach (var e in values) {
        if (min > e) min = e;
        if (max < e) max = e;
        i++;
      }
      if (i < 1) throw new ArgumentException("The enumerable must contain at least two elements", "values");
      return max - min;
    }


    /// <summary>
    /// Calculates the standard deviation of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double StandardDeviation(this IEnumerable<double> values) {
      return Math.Sqrt(Variance(values));
    }

    /// <summary>
    /// Calculates the variance of values. (sum (x - x_mean)² / n)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Variance(this IEnumerable<double> values) {
      int m_n = 0;
      double m_oldM = 0.0;
      double m_newM = 0.0;
      double m_oldS = 0.0;
      double m_newS = 0.0;
      foreach (double x in values) {
        m_n++;
        if (m_n == 1) {
          m_oldM = m_newM = x;
          m_oldS = 0.0;
        } else {
          m_newM = m_oldM + (x - m_oldM) / m_n;
          m_newS = m_oldS + (x - m_oldM) * (x - m_newM);

          // set up for next iteration
          m_oldM = m_newM;
          m_oldS = m_newS;
        }
      }
      return ((m_n > 1) ? m_newS / (m_n - 1) : 0.0);
    }

    /// <summary>
    /// Calculates the pth percentile of the values.
    /// </summary
    public static double Percentile(this IEnumerable<double> values, double p) {
      // iterate only once 
      double[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");
      if (n == 1) return values.ElementAt(0);

      if (p.IsAlmost(0.0)) return valuesArr[0];
      if (p.IsAlmost(1.0)) return valuesArr[n - 1];

      double t = p * (n - 1);
      int index = (int)Math.Floor(t);
      double percentage = t - index;
      return valuesArr[index] * (1 - percentage) + valuesArr[index + 1] * percentage;
    }

    public static IEnumerable<double> LimitToRange(this IEnumerable<double> values, double min, double max) {
      if (min > max) throw new ArgumentException(string.Format("Minimum {0} is larger than maximum {1}.", min, max));
      foreach (var x in values) {
        if (double.IsNaN(x)) yield return (max + min) / 2.0;
        else if (x < min) yield return min;
        else if (x > max) yield return max;
        else yield return x;
      }
    }
  }
}
