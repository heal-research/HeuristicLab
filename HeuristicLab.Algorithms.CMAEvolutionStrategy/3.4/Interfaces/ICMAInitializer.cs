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
using System;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [StorableType("EA9351D0-94F1-49D5-BE86-000CA0D4996E")]
  public interface ICMAInitializer : IOperator {
    Type CMAType { get; }

    ILookupParameter<IntValue> PopulationSizeParameter { get; }
    ILookupParameter<IntValue> MuParameter { get; }
    IValueLookupParameter<IntValue> InitialIterationsParameter { get; }
    IValueLookupParameter<IntValue> DimensionParameter { get; }
    IValueLookupParameter<DoubleArray> InitialSigmaParameter { get; }
  }
}
