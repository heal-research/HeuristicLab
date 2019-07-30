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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("927C21BD-8913-406F-ADEA-DA4FED3FE4A2")]
  [Item("SymbolicRegressionSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for regression problems.")]
  public class SymbolicRegressionSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public SymbolicRegressionSolutionImpactValuesCalculator() { }
    protected SymbolicRegressionSolutionImpactValuesCalculator(SymbolicRegressionSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolutionImpactValuesCalculator(this, cloner);
    }
    [StorableConstructor]
    protected SymbolicRegressionSolutionImpactValuesCalculator(StorableConstructorFlag _) : base(_) { }

    protected override double CalculateQualityForImpacts(ISymbolicDataAnalysisModel model, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;
      var estimatedValues = regressionModel.GetEstimatedValues(problemData.Dataset, rows); // also bounds the values
      var targetValues = problemData.Dataset.GetDoubleValues(regressionProblemData.TargetVariable, rows);
      OnlineCalculatorError errorState;
      var r = OnlinePearsonsRCalculator.Calculate(targetValues, estimatedValues, out errorState);
      var quality = r * r;
      if (errorState != OnlineCalculatorError.None) return double.NaN;
      return quality;
    }
  }
}
