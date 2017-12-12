#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.TestFunctions {
  /// <summary>
  /// An interface which represents operators for analyzing the best solution of single objective TestFunction Problems given in real vector representation.
  /// </summary>
  public interface IBestSingleObjectiveTestFunctionSolutionAnalyzer : IAnalyzer, ISingleObjectiveOperator {
    ILookupParameter RealVectorParameter { get; }
    ILookupParameter QualityParameter { get; }
    ILookupParameter<SingleObjectiveTestFunctionSolution> BestSolutionParameter { get; }
    IValueLookupParameter<ResultCollection> ResultsParameter { get; }
  }
}
