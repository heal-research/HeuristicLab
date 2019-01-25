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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  /// <summary>
  /// An operator that analyzes the validation best symbolic time-series prognosis solution for single objective symbolic time-series prognosis problems.
  /// </summary>
  [Item("SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best symbolic time-series prognosis solution for single objective symbolic time-series prognosis problems.")]
  [StorableType("642E4242-0FEF-45E6-BCE4-94D755256799")]
  public sealed class SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<ISymbolicTimeSeriesPrognosisSolution, ISymbolicTimeSeriesPrognosisSingleObjectiveEvaluator, ITimeSeriesPrognosisProblemData>, ISymbolicDataAnalysisBoundedOperator {
    private const string EstimationLimitsParameterName = "EstimationLimits";
    #region parameter properties
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer(SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic regression model."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisSingleObjectiveValidationBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicTimeSeriesPrognosisSolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality) {
      var model = new SymbolicTimeSeriesPrognosisModel(ProblemDataParameter.ActualValue.TargetVariable, (ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);

      return new SymbolicTimeSeriesPrognosisSolution(model, (ITimeSeriesPrognosisProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
