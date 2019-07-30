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
  [StorableType("6d0a0999-97c2-4716-a2d5-6a28f305cbdd")]
  /// <summary>
  /// An interface which represents a replacement operator for replacing solutions of single-objective optimization problems.
  /// </summary>
  public interface ISingleObjectiveReplacer : IReplacer, ISingleObjectiveOperator {
    IValueLookupParameter<BoolValue> MaximizationParameter { get; }
    ILookupParameter<ItemArray<DoubleValue>> QualityParameter { get; }
  }
}
