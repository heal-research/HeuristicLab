#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  public static class DatasetUtil {
    /// <summary>
    /// Shuffle all the lists with the same shuffling.
    /// </summary>
    /// <param name="values">The value lists to be shuffled.</param>
    /// <param name="random">The random number generator</param>
    /// <returns>A new list containing shuffled copies of the original value lists.</returns>
    public static List<IList> ShuffleLists(this List<IList> values, IRandom random) {
      int count = values.First().Count;
      int[] indices = Enumerable.Range(0, count).Shuffle(random).ToArray();
      List<IList> shuffled = new List<IList>(values.Count);
      for (int col = 0; col < values.Count; col++) {

        if (values[col] is IList<double>)
          shuffled.Add(new List<double>());
        else if (values[col] is IList<DateTime>)
          shuffled.Add(new List<DateTime>());
        else if (values[col] is IList<string>)
          shuffled.Add(new List<string>());
        else
          throw new InvalidOperationException();

        for (int i = 0; i < count; i++) {
          shuffled[col].Add(values[col][indices[i]]);
        }
      }
      return shuffled;

    }
  }
}
