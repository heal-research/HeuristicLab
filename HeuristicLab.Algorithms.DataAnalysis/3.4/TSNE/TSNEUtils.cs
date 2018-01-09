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

namespace HeuristicLab.Algorithms.DataAnalysis {
  internal static class TSNEUtils {
    internal static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action) {
      if (sequence == null) return;
      if (action == null) throw new ArgumentException("Null Action can not be performed");
      var i = 0;
      foreach (var item in sequence) {
        action(i, item);
        i++;
      }
    }

    internal static void Swap<T>(this IList<T> list, int indexA, int indexB) {
      var tmp = list[indexA];
      list[indexA] = list[indexB];
      list[indexB] = tmp;
    }

    private static int Partition<T>(this IList<T> list, int left, int right, int pivotindex, IComparer<T> comparer) {
      var pivotValue = list[pivotindex];
      list.Swap(pivotindex, right);
      var storeIndex = left;
      for (var i = left; i < right; i++)
        if (comparer.Compare(list[i], pivotValue) < 0)
          list.Swap(storeIndex++, i);
      list.Swap(right, storeIndex);
      return storeIndex;
    }

    /// <summary>
    /// Quick-Sort based partial ascending sorting
    /// [0,left-1] not changed;
    /// [left, n] elements are smaller than or equal to list[n]
    /// [n, right] elements are grater than or equal to list[n]
    /// [right, list.Count[ not changed;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">list that shall be partially sorted</param>
    /// <param name="left">left index of sorted list part</param>
    /// <param name="right">right index of sorted list part</param>
    /// <param name="n">index around which the partial sorting occures</param>
    /// <param name="comparer">comparer for list elemnts </param>
    /// <returns></returns>
    internal static void PartialSort<T>(this IList<T> list, int left, int right, int n, IComparer<T> comparer) {
      while (true) {
        if (left == right) return;
        var pivotindex = left + (int) Math.Floor(new System.Random().Next() % (right - (double) left + 1));
        pivotindex = list.Partition(left, right, pivotindex, comparer);
        if (n == pivotindex) return;
        if (n < pivotindex) right = pivotindex - 1;
        else left = pivotindex + 1;
      }
    }
  }
}