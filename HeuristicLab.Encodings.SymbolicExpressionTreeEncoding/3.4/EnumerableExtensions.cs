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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {

  //This class should not be used anymore. Use HeuristicLab.Random.RandomEnumberable instead
  //This could not be fixed right now, because the algorithm behavior would be modified => version increment
  [Obsolete("This class will be removed in the future, because the functionality is provided in HeuristicLab.Random.RandomEnumerable.")]
  public static class EnumerableExtensions {
    [Obsolete("This method should not be used anymore. Use the extensions provided by HeuristicLab.Random.RandomEnumberable instead.")]
    public static T SelectRandom<T>(this IEnumerable<T> xs, IRandom random) {
      var list = xs as IList<T>;
      if (list != null) {
        return list[random.Next(list.Count)];
      } else {
        list = xs.ToList();
        return list[random.Next(list.Count)];
      }
    }
    [Obsolete("This method should not be used anymore. Use the extensions provided by HeuristicLab.Random.RandomEnumberable instead.")]
    public static T SelectRandom<T>(this IEnumerable<T> xs, IEnumerable<double> weights, IRandom random) {
      var list = xs as IList<T>;
      var weightsList = weights as IList<double>;
      if (list == null) {
        list = xs.ToList();
      }
      if (weightsList == null) {
        weightsList = weights.ToList();
      }
      if (list.Count != weightsList.Count) throw new ArgumentException("Number of elements in enumerations doesn't match.");
      if (list.Count == 0) throw new ArgumentException("Enumeration is empty", "xs");

      double sum = weightsList.Sum();
      double r = random.NextDouble() * sum;
      double agg = 0;
      for (int i = 0; i < list.Count; i++) {
        agg += weightsList[i];
        if (agg > r) return list[i];
      }
      // should never happen
      throw new InvalidOperationException();
    }
  }
}
