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

using System.Collections;
using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public interface IDistance<in T> : IDistance {
    /// <summary>
    /// Calculates a distance measure between two objects.
    /// 1.) non-negative d(x,y) >= 0
    /// 2.) symmetric d(x,y) = d(y,x)
    /// 3.) zero-reflexive d(x,x) = 0;
    /// </summary>
    /// <returns>d(x,y)</returns>
    double Get(T x, T y);

    /// <summary>
    /// Returns a comparator wich compares the distances to item. (allows for sorting nearest/farthest neighbours)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IComparer<T> GetDistanceComparer(T item);
  }


  public interface IDistance : IItem {
    double Get(object x, object y);
    IComparer GetDistanceComparer(object item);
  }
}
