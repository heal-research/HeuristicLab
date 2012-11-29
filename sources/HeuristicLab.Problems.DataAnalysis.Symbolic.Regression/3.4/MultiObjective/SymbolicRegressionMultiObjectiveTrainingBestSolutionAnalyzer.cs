#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// An operator that analyzes the training best symbolic regression solution for multi objective symbolic regression problems.
  /// </summary>
  [Item("SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic regression solution for multi objective symbolic regression problems.")]
  [StorableClass]
  public sealed class SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer : SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer<ISymbolicRegressionSolution>,
    ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    #region parameter properties
    public ILookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The problem data for the symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic regression model."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicRegressionSolution CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality) {
      var model = new SymbolicRegressionModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);
      return new SymbolicRegressionSolution(model, (IRegressionProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
