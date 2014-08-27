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
      return sequences.Aggregate(result, (current, s) => (from seq in current from item in s select seq.Concat(new[] { item })));
    }
  }
}
