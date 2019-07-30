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
using HeuristicLab.Encodings.RealVectorEncoding;
using System;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [StorableType("0BFE0CF3-24A1-4495-A1F3-6D0ED6634BD7")]
  public interface ICMAUpdater : IOperator {
    Type CMAType { get; }

    ILookupParameter<RealVector> MeanParameter { get; }
    ILookupParameter<RealVector> OldMeanParameter { get; }
    IScopeTreeLookupParameter<RealVector> OffspringParameter { get; }
    IScopeTreeLookupParameter<DoubleValue> QualityParameter { get; }
    IValueLookupParameter<IntValue> MaximumEvaluatedSolutionsParameter { get; }
    ILookupParameter<BoolValue> DegenerateStateParameter { get; }
  }
}
