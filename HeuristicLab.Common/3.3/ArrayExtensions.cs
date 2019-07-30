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
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.Common {
  public static class ArrayExtensions {

    public static void StableSort<T>(this T[] values, Comparison<T> comparison) {
      values.StableSort(new StableSortComparer<T>(comparison));
    }

    public static void StableSort<T>(this T[] values, IComparer<T> comparer = null) {
      var sorted = values.OrderBy(x => x, comparer ?? Comparer<T>.Default).ToArray();
      Array.ConstrainedCopy(sorted, 0, values, 0, values.Length);
    }

    public static void StableSort<T>(this T[] values, int index, int length, Comparison<T> comparison) {
      values.StableSort(index, length, new StableSortComparer<T>(comparison));
    }

    public static void StableSort<T>(this T[] values, int index, int length, IComparer<T> comparer = null) {
      if (index < 0) throw new ArgumentOutOfRangeException("index is less than zero.");
      if (length < 0) throw new ArgumentOutOfRangeException("length is less than zero.");
      if (index + length > values.Length) throw new ArgumentException("index and length do not specify a valid range in the array.");
      var sortedArray = values.Skip(index).Take(length).OrderBy(x => x, comparer ?? Comparer<T>.Default).ToArray();
      Array.ConstrainedCopy(sortedArray, 0, values, index, length);
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
