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
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("8efcf623-6ea3-4010-a7b2-d3788b8648d1")]
  public interface ISubScopesQualityComparator {
    ILookupParameter<DoubleValue> LeftSideParameter { get; }
    ILookupParameter<ItemArray<DoubleValue>> RightSideParameter { get; }
    ILookupParameter<BoolValue> ResultParameter { get; }
    IValueLookupParameter<BoolValue> MaximizationParameter { get; }
  }
}
