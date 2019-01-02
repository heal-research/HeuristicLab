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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("f2c0ca54-a2a9-49b8-9f5f-734368000f68")]
  public interface ISymbolicDataAnalysisSingleObjectiveAnalyzer : ISymbolicDataAnalysisAnalyzer {
    IScopeTreeLookupParameter<DoubleValue> QualityParameter { get; }
    ILookupParameter<BoolValue> MaximizationParameter { get; }
    ILookupParameter<BoolValue> ApplyLinearScalingParameter { get; }
  }
}
