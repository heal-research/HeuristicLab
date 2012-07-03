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
  /// An operator that analyzes the validation best symbolic classification solution for multi objective symbolic classification problems.
  /// </summary>
  [Item("SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best symbolic classification solution for multi objective symbolic classification problems.")]
  [StorableClass]
  public sealed class SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer : SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer<ISymbolicClassificationSolution, ISymbolicClassificationMultiObjectiveEvaluator, IClassificationProblemData>,
  ISymbolicDataAnalysisBoundedOperator {
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";

    #region parameter properties
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
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
    private SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The loewr and upper limit for the estimated values produced by the symbolic classification model."));
      Parameters.Add(new ValueParameter<BoolValue>(ApplyLinearScalingParameterName, "Flag that indicates if the produced symbolic classification solution should be linearly scaled.", new BoolValue(false)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicClassificationSolution CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQualities) {
      var model = new SymbolicDiscriminantFunctionClassificationModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScaling.Value) {
        SymbolicDiscriminantFunctionClassificationModel.Scale(model, ProblemDataParameter.ActualValue);
      }
      return new SymbolicDiscriminantFunctionClassificationSolution(model, (IClassificationProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
