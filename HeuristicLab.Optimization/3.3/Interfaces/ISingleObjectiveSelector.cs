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
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("242d4dc6-e31b-4058-afea-f3d95b44a94a")]
  /// <summary>
  /// An interface which represents a selection operator for selecting solutions of single-objective optimization problems.
  /// </summary>
  public interface ISingleObjectiveSelector : ISelector, ISingleObjectiveOperator {
    IValueLookupParameter<BoolValue> MaximizationParameter { get; }
    ILookupParameter<ItemArray<DoubleValue>> QualityParameter { get; }
  }
}
