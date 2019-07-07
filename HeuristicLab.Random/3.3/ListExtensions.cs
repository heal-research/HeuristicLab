#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Random {
  public static class ListExtensions {
    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB) {
      T tmp = list[indexA];
      list[indexA] = list[indexB];
      list[indexB] = tmp;
      return list;
    }

    public static IList<T> ShuffleInPlace<T>(this IList<T> list, IRandom random) {
      return list.ShuffleInPlace(random, 0, list.Count - 1);
    }
    public static IList<T> ShuffleInPlace<T>(this IList<T> list, IRandom random, int maxIndex) {
      return list.ShuffleInPlace(random, 0, maxIndex);
    }
    public static IList<T> ShuffleInPlace<T>(this IList<T> list, IRandom random, int minIndex, int maxIndex) {
      for (int i = maxIndex; i > minIndex; i--) {
        int swapIndex = random.Next(minIndex, i + 1);
        list.Swap(i, swapIndex);
      }
      return list;
    }

    public static IEnumerable<T> ShuffleList<T>(this IList<T> source, IRandom random) {
      for (int i = source.Count - 1; i > 0; i--) {
        // Swap element "i" with a random earlier element (including itself)
        int swapIndex = random.Next(i + 1);
        source.Swap(i, swapIndex);
        yield return source[i];
      }
      yield return source[0];
    }
  }
}
