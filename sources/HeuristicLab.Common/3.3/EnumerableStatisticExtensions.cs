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
using System.Diagnostics.Contracts;
using System.Linq;

namespace HeuristicLab.Common {
  public static class EnumerableStatisticExtensions {
    /// <summary>
    /// Calculates the median element of the enumeration.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Median(this IEnumerable<double> values) {
      // See unit tests for comparison with naive implementation
      return Quantile(values, 0.5);
    }

    /// <summary>
    /// Calculates the alpha-quantile element of the enumeration.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Quantile(this IEnumerable<double> values, double alpha) {
      // See unit tests for comparison with naive implementation
      double[] valuesArr = values.ToArray();
      int n = valuesArr.Length;
      if (n == 0) throw new InvalidOperationException("Enumeration contains no elements.");

      // "When N is even, statistics books define the median as the arithmetic mean of the elements k = N/2 
      // and k = N/2 + 1 (that is, N/2 from the bottom and N/2 from the top). 
      // If you accept such pedantry, you must perform two separate selections to find these elements."

      // return the element at Math.Ceiling (if n*alpha is fractional) or the average of two elements if n*alpha is integer.
      var pos = n * alpha;
      Contract.Assert(pos >= 0);
      Contract.Assert(pos < n);
      bool isInteger = Math.Round(pos).IsAlmost(pos);
      if (isInteger) {
        return 0.5 * (Select((int)pos - 1, valuesArr) + Select((int)pos, valuesArr));
      } else {
        return Select((int)Math.Ceiling(pos) - 1, valuesArr);
      }
    }

    // Numerical Recipes in C++, §8.5 Selecting the Mth Largest, O(n)
    // Given k in [0..n-1] returns an array value from array arr[0..n-1] such that k array values are 
    // lee than or equal to the one returned. The input array will be rearranged to hav this value in 
    // location arr[k], with all smaller elements moved to arr[0..k-1] (in arbitrary order) and all 
    // larger elements in arr[k+1..n-1] (also in arbitrary order).
    // 
    // Could be changed to Select<T> where T is IComparable but in this case is significantly slower for double values
    private static double Select(int k, double[] arr) {
      Contract.Assert(arr.GetLowerBound(0) == 0);
      Contract.Assert(k >= 0 && k < arr.Length);
      int i, ir, j, l, mid, n = arr.Length;
      double a;
      l = 0;
      ir = n - 1;
      for (; ; ) {
        if (ir <= l + 1) {
          // Active partition contains 1 or 2 elements.
          if (ir == l + 1 && arr[ir] < arr[l]) {
            // if (ir == l + 1 && arr[ir].CompareTo(arr[l]) < 0) {
            // Case of 2 elements.
            // SWAP(arr[l], arr[ir]);
            double temp = arr[l];
            arr[l] = arr[ir];
            arr[ir] = temp;
          }
          return arr[k];
        } else {
          mid = (l + ir) >> 1; // Choose median of left, center, and right elements
          {
            // SWAP(arr[mid], arr[l + 1]); // as partitioning element a. Also
            double temp = arr[mid];
            arr[mid] = arr[l + 1];
            arr[l + 1] = temp;
          }

          if (arr[l] > arr[ir]) {
            // if (arr[l].CompareTo(arr[ir]) > 0) {  // rearrange so that arr[l] arr[ir] <= arr[l+1],
            // SWAP(arr[l], arr[ir]); . arr[ir] >= arr[l+1]
            double temp = arr[l];
            arr[l] = arr[ir];
            arr[ir] = temp;
          }

          if (arr[l + 1] > arr[ir]) {
            // if (arr[l + 1].CompareTo(arr[ir]) > 0) {
            // SWAP(arr[l + 1], arr[ir]);
            double temp = arr[l + 1];
            arr[l + 1] = arr[ir];
            arr[ir] = temp;
          }
          if (arr[l] > arr[l + 1]) {
          //if (arr[l].CompareTo(arr[l + 1]) > 0) {
            // SWAP(arr[l], arr[l + 1]);
            double temp = arr[l];
            arr[l] = arr[l + 1];
            arr[l + 1] = temp;

          }
          i = l + 1; // Initialize pointers for partitioning.
          j = ir;
          a = arr[l + 1]; // Partitioning element.
          for (; ; ) { // Beginning of innermost loop.
            do i++; while (arr[i] < a /* arr[i].CompareTo(a) < 0 */); // Scan up to find element > a.
            do j--; while (arr[j] > a /* arr[j].CompareTo(a) > 0 */); // Scan down to find element < a.
            if (j < i) break; // Pointers crossed. Partitioning complete.
            {
              // SWAP(arr[i], arr[j]);
              double temp = arr[i];
              arr[i] = arr[j];
              arr[j] = temp;
            }
          } // End of innermost loop.
          arr[l + 1] = arr[j]; // Insert partitioning element.
          arr[j] = a;
          if (j >= k) ir = j - 1; // Keep active the partition that contains the
          if (j <= k) l = i; // kth element.
        }
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
