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

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableType("868522CB-CA0A-4908-8108-86D2464F0FAE")]
  public interface IMultiObjectiveTestFunctionAnalyzer : IAnalyzer, IMultiObjectiveOperator {
    IScopeTreeLookupParameter<DoubleArray> QualitiesParameter { get; }
    ILookupParameter<ResultCollection> ResultsParameter { get; }
    ILookupParameter<IMultiObjectiveTestFunction> TestFunctionParameter { get; }
    ILookupParameter<DoubleMatrix> BestKnownFrontParameter { get; }
  }
}
