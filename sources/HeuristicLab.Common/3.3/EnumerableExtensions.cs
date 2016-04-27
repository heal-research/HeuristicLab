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
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Common {
  public static class EnumerableExtensions {
    /// <summary>
    /// Selects all elements in the sequence that are maximal with respect to the given value.
    /// </summary>
    /// <remarks>
    /// Runtime complexity of the operation is O(N).
    /// </remarks>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="source">The enumeration in which the items with a maximal value should be found.</param>
    /// <param name="valueSelector">The function that selects the value to compare.</param>
    /// <returns>All elements in the enumeration where the selected value is the maximum.</returns>
    public static IEnumerable<T> MaxItems<T>(this IEnumerable<T> source, Func<T, IComparable> valueSelector) {
      IEnumerator<T> enumerator = source.GetEnumerator();
      if (!enumerator.MoveNext()) return Enumerable.Empty<T>();
      IComparable max = valueSelector(enumerator.Current);
      var result = new List<T>();
      result.Add(enumerator.Current);

      while (enumerator.MoveNext()) {
        T item = enumerator.Current;
        IComparable comparison = valueSelector(item);
        if (comparison.CompareTo(max) > 0) {
          result.Clear();
          result.Add(item);
          max = comparison;
        } else if (comparison.CompareTo(max) == 0) {
          result.Add(item);
        }
      }
      return result;
    }

    /// <summary>
    /// Selects all elements in the sequence that are minimal with respect to the given value.
    /// </summary>
    /// <remarks>
    /// Runtime complexity of the operation is O(N).
    /// </remarks>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="source">The enumeration in which items with a minimal value should be found.</param>
    /// <param name="valueSelector">The function that selects the value.</param>
    /// <returns>All elements in the enumeration where the selected value is the minimum.</returns>
    public static IEnumerable<T> MinItems<T>(this IEnumerable<T> source, Func<T, IComparable> valueSelector) {
      IEnumerator<T> enumerator = source.GetEnumerator();
      if (!enumerator.MoveNext()) return Enumerable.Empty<T>();
      IComparable min = valueSelector(enumerator.Current);
      var result = new List<T>();
      result.Add(enumerator.Current);

      while (enumerator.MoveNext()) {
        T item = enumerator.Current;
        IComparable comparison = valueSelector(item);
        if (comparison.CompareTo(min) < 0) {
          result.Clear();
          result.Add(item);
          min = comparison;
        } else if (comparison.CompareTo(min) == 0) {
          result.Add(item);
        }
      }
      return result;
    }

    /// <summary>
    /// Compute the n-ary cartesian product of arbitrarily many sequences: http://blogs.msdn.com/b/ericlippert/archive/2010/06/28/computing-a-cartesian-product-with-linq.aspx
    /// </summary>
    /// <typeparam name="T">The type of the elements inside each sequence</typeparam>
    /// <param name="sequences">The collection of sequences</param>
    /// <returns>An enumerable sequence of all the possible combinations of elements</returns>
    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences) {
      IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
      return sequences.Where(s => s.Any()).Aggregate(result, (current, s) => (from seq in current from item in s select seq.Concat(new[] { item })));
    }

    /// <summary>
    /// Compute all k-combinations of elements from the provided collection.
    /// <param name="elements">The collection of elements</param>
    /// <param name="k">The combination group size</param>
    /// <returns>An enumerable sequence of all the possible k-combinations of elements</returns>
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> elements, int k) {
      if (k > elements.Count)
        throw new ArgumentException();
      if (k == 1) {
        foreach(var element in elements) yield return new[] { element };
      } else {
        int n = elements.Count;
        var range = new int[k];
        for (int i = 1; i < k; ++i)
          range[i] = i;

        var length = BinomialCoefficient(n, k);

        for (int i = 0; i < length; ++i) {
          yield return range.Select(x => elements[x]);
          if (i == length-1) break;
          var m = k - 1;
          var max = n - 1;
          while (range[m] == max) { --m; --max; }
          range[m]++;
          for (int j = m + 1; j < k; ++j) {
            range[j] = range[j-1] + 1;
          }
        }
      }
    }

    /// <summary>
    /// This function gets the total number of unique combinations based upon N and K,
    /// where N is the total number of items and K is the size of the group.
    /// It calculates the total number of unique combinations C(N, K) = N! / ( K! (N - K)! )
    /// using the  recursion C(N+1, K+1) = (N+1 / K+1) * C(N, K).
    /// <remarks>http://blog.plover.com/math/choose.html</remarks>
    /// <param name="n">The number of elements</param>
    /// <param name="k">The size of the group</param>
    /// <returns>The binomial coefficient C(N, K)</returns>
    /// </summary>
    public static long BinomialCoefficient(long n, long k) 	{
      if (k > n) return 0;
      if (k == n) return 1;
      if (k > n - k)
        k = n - k;
      long r = 1;
      for (long d = 1; d <= k; d++) 	{
        r *= n--;
        r /= d;
      }
      return r;
    }
  }
}
