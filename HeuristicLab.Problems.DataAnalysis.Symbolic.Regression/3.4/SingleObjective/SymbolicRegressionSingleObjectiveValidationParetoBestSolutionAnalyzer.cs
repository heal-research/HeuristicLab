#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// An operator that collects the validation Pareto-best symbolic regression solutions for single objective symbolic regression problems.
  /// </summary>
  [Item("SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer", "An operator that collects the validation Pareto-best symbolic regression solutions for single objective symbolic regression problems.")]
  [StorableClass]
  public sealed class SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer<ISymbolicRegressionSolution, ISymbolicRegressionSingleObjectiveEvaluator, IRegressionProblemData> {
    [StorableConstructor]
    private SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer(SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicRegressionSolution CreateSolution(ISymbolicExpressionTree bestTree) {
      var model = new SymbolicRegressionModel(ProblemDataParameter.ActualValue.TargetVariable, (ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);
      return new SymbolicRegressionSolution(model, (IRegressionProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
