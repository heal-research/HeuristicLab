#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public static class ListExtensions {
    public static void StableSort<T>(this List<T> values) {
      values.StableSort(0, values.Count);
    }

    public static void StableSort<T>(this List<T> values, int index, int count) {
      var orderedList = values.Skip(index).Take(count).OrderBy(x => x);
      int i = index;
      foreach (var e in orderedList)
        values[i++] = e;
    }

    public static void StableSort<T>(this List<T> values, IComparer<T> comparer) {
      values.StableSort(0, values.Count, comparer);
    }

    public static void StableSort<T>(this List<T> values, int index, int count, IComparer<T> comparer) {
      var orderedList = values.OrderBy(x => x, comparer);
      int i = 0;
      foreach (var e in orderedList)
        values[i++] = e;
    }

    public static void StableSort<T>(this List<T> values, Comparison<T> comparison) {
      values.StableSort(0, values.Count, comparison);
    }

    public static void StableSort<T>(this List<T> values, int index, int count, Comparison<T> comparison) {
      var orderedList = values.OrderBy(x => x, new StableSortComparer<T>(comparison));
      int i = 0;
      foreach (var e in orderedList)
        values[i++] = e;
    }

    public class StableSortComparer<T> : IComparer<T> {
      public StableSortComparer(Comparison<T> comparison) {
        this.comparison = comparison;
      }
      public int Compare(T x, T y) {
        return comparison(x, y);
      }
      private readonly Comparison<T> comparison;
    }
  }
}
