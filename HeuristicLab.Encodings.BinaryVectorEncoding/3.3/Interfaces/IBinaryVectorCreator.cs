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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [StorableType("60af1d5b-398c-4167-aa3f-08fce3d7d0a4")]
  /// <summary>
  /// An interface which represents an operator for creating vectors of bool-valued data.
  /// </summary>
  public interface IBinaryVectorCreator : IBinaryVectorOperator, ISolutionCreator {
    IValueLookupParameter<IntValue> LengthParameter { get; }
    ILookupParameter<BinaryVector> BinaryVectorParameter { get; }
  }
}