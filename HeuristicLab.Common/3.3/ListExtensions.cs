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

namespace HeuristicLab.Common {
  public static class ListExtensions {

    public static void StableSort<T>(this List<T> values, Comparison<T> comparison) {
      values.StableSort(new StableSortComparer<T>(comparison));
    }

    public static void StableSort<T>(this List<T> values, IComparer<T> comparer = null) {
      int i = 0;
      foreach (var e in values.OrderBy(x => x, comparer ?? Comparer<T>.Default))
        values[i++] = e;
    }

    public static void StableSort<T>(this List<T> values, int index, int count, Comparison<T> comparison) {
      values.StableSort(index, count, new StableSortComparer<T>(comparison));
    }

    public static void StableSort<T>(this List<T> values, int index, int count, IComparer<T> comparer = null) {
      if (index < 0) throw new ArgumentOutOfRangeException("index is less than zero.");
      if (count < 0) throw new ArgumentOutOfRangeException("count is less than zero.");
      if (index + count > values.Count) throw new ArgumentException("index and count do not specify a valid range in the List<T>.");
      var orderedList = values.Skip(index).Take(count).OrderBy(x => x, comparer ?? Comparer<T>.Default);
      int i = index;
      foreach (var e in orderedList)
        values[i++] = e;
    }

    private class StableSortComparer<T> : IComparer<T> {
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
