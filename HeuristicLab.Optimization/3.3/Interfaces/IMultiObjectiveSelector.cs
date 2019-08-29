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
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("df0370eb-6483-4bf9-a18f-dae937c32f4f")]
  /// <summary>
  /// An interface which represents a selection operator for selecting solutions of multi-objective optimization problems.
  /// </summary>
  public interface IMultiObjectiveSelector : ISelector, IMultiObjectiveOperator {
    ILookupParameter<BoolArray> MaximizationParameter { get; }
    ILookupParameter<ItemArray<DoubleArray>> QualitiesParameter { get; }
  }
}
