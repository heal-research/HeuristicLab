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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// An operator that collects the training Pareto-best symbolic regression solutions for single objective symbolic regression problems.
  /// </summary>
  [Item("SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer", "An operator that collects the training Pareto-best symbolic regression solutions for single objective symbolic regression problems.")]
  [StorableType("8FDF5528-8E95-44D6-AFFD-433B4AA55559")]
  public sealed class SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveTrainingParetoBestSolutionAnalyzer<IRegressionProblemData, ISymbolicRegressionSolution> {

    [StorableConstructor]
    private SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer(SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicRegressionSolution CreateSolution(ISymbolicExpressionTree bestTree) {
      var model = new SymbolicRegressionModel(ProblemDataParameter.ActualValue.TargetVariable, (ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);
      return new SymbolicRegressionSolution(model, (IRegressionProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
