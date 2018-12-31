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

using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("b29109e0-1930-429e-bc50-56790b97a7a0")]
  /// <summary>
  /// An interface which represents an operator for crossing vectors of int-valued data.
  /// </summary>
  public interface IIntegerVectorCrossover : IIntegerVectorOperator, ICrossover {
    ILookupParameter<ItemArray<IntegerVector>> ParentsParameter { get; }
    ILookupParameter<IntegerVector> ChildParameter { get; }
  }
}