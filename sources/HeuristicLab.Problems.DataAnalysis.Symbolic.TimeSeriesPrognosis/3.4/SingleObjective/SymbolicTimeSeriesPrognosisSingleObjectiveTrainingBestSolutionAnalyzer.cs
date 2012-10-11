#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.TimeSeriesPrognosis {
  /// <summary>
  /// An operator that analyzes the training best symbolic time-series prognosis solution for single objective symbolic time-series prognosis problems.
  /// </summary>
  [Item("SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic time-series prognosis solution for single objective symbolic time-series prognosis problems.")]
  [StorableClass]
  public sealed class SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveTrainingBestSolutionAnalyzer<ISymbolicTimeSeriesPrognosisSolution>,
  ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    #region parameter properties
    public ILookupParameter<ITimeSeriesPrognosisProblemData> ProblemDataParameter {
      get { return (ILookupParameter<ITimeSeriesPrognosisProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion


    [StorableConstructor]
    private SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer(SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<ITimeSeriesPrognosisProblemData>(ProblemDataParameterName, "The problem data for the symbolic regression solution."));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic time series prognosis interpreter for the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic regression model."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTimeSeriesPrognosisSingleObjectiveTrainingBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicTimeSeriesPrognosisSolution CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality) {
      var model = new SymbolicTimeSeriesPrognosisModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue as ISymbolicTimeSeriesPrognosisExpressionTreeInterpreter, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) SymbolicTimeSeriesPrognosisModel.Scale(model, ProblemDataParameter.ActualValue, ProblemDataParameter.ActualValue.TargetVariable);
      return new SymbolicTimeSeriesPrognosisSolution(model, (ITimeSeriesPrognosisProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
