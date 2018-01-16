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

using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.BinPacking3D.Encoding {
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IDecoder<in T> : IItem {
    /// <summary>
    /// Generates a solution by using the input parameters.
    /// </summary>
    /// <param name="encodedSolution"></param>
    /// <param name="binShape">Bin for storing the items</param>
    /// <param name="items">A list of packing items which should be assigned to a bin</param>
    /// <param name="useStackingConstraints">If true, the items will be placed if the position is static stable.</param>
    /// <returns>Returns an decoded solution for displaying it on the HEAL gui.</returns>
    Solution Decode(T encodedSolution, PackingShape binShape, IList<PackingItem> items, bool useStackingConstraints);
  }
}
