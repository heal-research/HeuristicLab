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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [StorableType("250ec119-143a-4798-b074-32b44a0fc0d4")]
  /// <summary>
  /// An interface which represents an operator for manipulating vectors of bool-valued data.
  /// </summary>
  public interface IBinaryVectorManipulator : IBinaryVectorOperator, IManipulator {
    ILookupParameter<BinaryVector> BinaryVectorParameter { get; }
  }
}