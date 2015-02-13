#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// An operator that analyzes the validation best symbolic classification solution for multi objective symbolic classification problems.
  /// </summary>
  [Item("SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best symbolic classification solution for multi objective symbolic classification problems.")]
  [StorableClass]
  public sealed class SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer : SymbolicDataAnalysisMultiObjectiveValidationBestSolutionAnalyzer<ISymbolicClassificationSolution, ISymbolicClassificationMultiObjectiveEvaluator, IClassificationProblemData>,
  ISymbolicDataAnalysisBoundedOperator, ISymbolicClassificationModelCreatorOperator {
    private const string ModelCreatorParameterName = "ModelCreator";
    private const string EstimationLimitsParameterName = "EstimationLimits";

    #region parameter properties
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public IValueLookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (IValueLookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    ILookupParameter<ISymbolicClassificationModelCreator> ISymbolicClassificationModelCreatorOperator.ModelCreatorParameter {
      get { return ModelCreatorParameter; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The loewr and upper limit for the estimated values produced by the symbolic classification model."));
      Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectiveValidationBestSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (!Parameters.ContainsKey(ModelCreatorParameterName))
        Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
      #endregion
    }

    protected override ISymbolicClassificationSolution CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQualities) {
      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel((ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);

      model.RecalculateModelParameters(ProblemDataParameter.ActualValue, ProblemDataParameter.ActualValue.TrainingIndices);
      return model.CreateClassificationSolution((IClassificationProblemData)ProblemDataParameter.ActualValue.Clone());
    }
  }
}
