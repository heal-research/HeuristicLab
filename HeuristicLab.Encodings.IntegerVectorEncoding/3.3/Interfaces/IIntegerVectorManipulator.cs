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

using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("8c58d54f-5454-4b55-8be8-820ef0ee8687")]
  /// <summary>
  /// An interface which represents an operator for manipulating vectors of int-valued data.
  /// </summary>
  public interface IIntegerVectorManipulator : IIntegerVectorOperator, IManipulator {
    ILookupParameter<IntegerVector> IntegerVectorParameter { get; }
  }
}