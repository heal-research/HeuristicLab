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
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// An operator that collects the validation Pareto-best symbolic classification solutions for single objective symbolic classification problems.
  /// </summary>
  [Item("SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer", "An operator that collects the validation Pareto-best symbolic classification solutions for single objective symbolic classification problems.")]
  [StorableClass]
  public sealed class SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer : SymbolicDataAnalysisSingleObjectiveValidationParetoBestSolutionAnalyzer<ISymbolicClassificationSolution, ISymbolicClassificationSingleObjectiveEvaluator, IClassificationProblemData> {
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    #region parameter properties
    public IValueParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion

    #region properties
    public BoolValue ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer(SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the produced symbolic classification solution should be linearly scaled.", new BoolValue(false)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicClassificationSolution CreateSolution(ISymbolicExpressionTree bestTree) {
      var model = new SymbolicDiscriminantFunctionClassificationModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScaling.Value) SymbolicDiscriminantFunctionClassificationModel.Scale(model, ProblemDataParameter.ActualValue);

      model.SetAccuracyMaximizingThresholds(ProblemDataParameter.ActualValue);
      return new SymbolicDiscriminantFunctionClassificationSolution(model, (IClassificationProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
