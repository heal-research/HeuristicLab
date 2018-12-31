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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [StorableType("2b3e467b-32a9-46fa-bfd2-41017bb5075c")]
  /// <summary>
  /// An interface which represents an operator for manipulating solutions.
  /// </summary>
  public interface IPermutationManipulator : IPermutationOperator, IManipulator {
    ILookupParameter<Permutation> PermutationParameter { get; }
  }
}
