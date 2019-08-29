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

using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [StorableType("4d5f6fe2-924e-4052-b1c3-32d12b232353")]
  /// <summary>
  /// An interface which represents an operator for manipulating vectors of real-valued data.
  /// </summary>
  public interface IRealVectorManipulator : IRealVectorSolutionOperator, IRealVectorBoundedOperator, IManipulator {
  }
}