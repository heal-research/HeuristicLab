#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.DataAnalysis {
  public class Statistics {

    /// <summary>
    /// Minimum returns the smalles entry of values.
    /// Throws and exception if values is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static T Minimum<T>(IEnumerable<T> values) where T : struct, IComparable, IComparable<T> {
      IEnumerator<T> enumerator = values.GetEnumerator();

      // this will throw an exception if the values collection is empty
      enumerator.MoveNext();
      T minimum = enumerator.Current;

      while (enumerator.MoveNext()) {
        T current = enumerator.Current;
        if (current.CompareTo(minimum) < 0) {
          minimum = current;
        }
      }

      return minimum;
    }

    /// <summary>
    /// Maximum returns the largest entry of values.
    /// Throws an exception if values is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <returns></returns>
    public static T Maximum<T>(IEnumerable<T> values) where T : struct, IComparable, IComparable<T> {
      IEnumerator<T> enumerator = values.GetEnumerator();

      // this will throw an exception if the values collection is empty
      enumerator.MoveNext();
      T maximum = enumerator.Current;

      while (enumerator.MoveNext()) {
        T current = enumerator.Current;
        if (current.CompareTo(maximum) > 0) {
          maximum = current;
        }
      }

      return maximum;
    }

    /// <summary>
    /// Range calculates the difference between the larges and smallest entry of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Range(double[] values) {
      return Range(values, 0, values.Length);
    }

    /// <summary>
    /// Range calculates the difference between the larges and smallest entry of values.
    /// </summary>
    public static double Range(List<double> values) {
      return Range(values.ToArray(), 0, values.Count);
    }

    /// <summary>
    /// Range calculates the difference between the largest and smallest entry of values between start and end.
    /// </summary>
    /// <param name="values">collection of values</param>
    /// <param name="start">start index (inclusive)</param>
    /// <param name="end">end index (exclusive)</param>
    /// <returns></returns>
    public static double Range(double[] values, int start, int end) {
      if (start < 0 || start > values.Length || end < 0 || end > values.Length || start > end) {
        throw new InvalidOperationException();
      }

      double minimum = double.PositiveInfinity;
      double maximum = double.NegativeInfinity;
      for (int i = start; i < end; i++) {
        if (!double.IsNaN(values[i])) {
          if (values[i] > maximum) {
            maximum = values[i];
          }
          if (values[i] < minimum) {
            minimum = values[i];
          }
        }
      }
      return (maximum - minimum);
    }

    /// <summary>
    /// Calculates the mean of all values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Mean(List<double> values) {
      return Mean(values.ToArray(), 0, values.Count);
    }

    // Calculates the mean of all values.
    public static double Mean(double[] values) {
      return Mean(values, 0, values.Length);
    }

    /// <summary>
    /// Calculates the mean of the values between start and end.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="start">start index (inclusive)</param>
    /// <param name="end">end index(exclusive)</param>
    /// <returns></returns>
    public static double Mean(double[] values, int start, int end) {
      if (values.Length == 0) throw new ArgumentException("Values is empty.");
      if(end <=start) throw new ArgumentException("End is smaller or equal start");
      double sum = 0.0;
      int n = 0;
      for (int i = start; i < end; i++) {
        if (!double.IsNaN(values[i])) {
          sum += values[i];
          n++;
        }
      }
      if (n > 0)
        return sum / n;
      else throw new ArgumentException("Only NaN elements in values");
    }

    /// <summary>
    /// Calculates the median of the values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Median(double[] values) {
      if (values.Length == 0) throw new InvalidOperationException();
      int n = values.Length;
      double[] sortedValues = new double[n];

      Array.Copy(values, sortedValues, n);
      Array.Sort(sortedValues);

      // return the middle element (if n is uneven) or the average of the two middle elements if n is even.
      if (n % 2 == 1) {
        return sortedValues[n / 2];
      } else {
        return (sortedValues[(n / 2)-1] + sortedValues[n / 2 ]) / 2.0;
      }
    }


    /// <summary>
    /// Calculates the standard deviation of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double StandardDeviation(double[] values) {
      return Math.Sqrt(Variance(values));
    }

    /// <summary>
    /// Calculates the variance of values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static double Variance(double[] values) {
      return Variance(values, 0, values.Length);
    }


    /// <summary>
    /// Calculates the variance of the entries of values between start and end.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="start">start index (inclusive)</param>
    /// <param name="end">end index (exclusive)</param>
    /// <returns></returns>
    public static double Variance(double[] values, int start, int end) {
      if (values.Length == 0) throw new ArgumentException("Values is empty.");
      if (end <= start) throw new ArgumentException("End is smaller or equal start");
      if (end - start == 1)
        return 0.0;

      double mean = Mean(values, start, end);
      double squaredErrorsSum = 0.0;

      int n = 0;
      for (int i = start; i < end; i++) {
        if (!double.IsNaN(values[i])) {
          double d = values[i] - mean;
          squaredErrorsSum += d * d;
          n++;
        }
      }
      if (n < 2) {
        throw new ArgumentException("Only one non-NaN element in values");
      }
      return squaredErrorsSum / (n - 1);
    }
  }
}
