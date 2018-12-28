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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Penalty Score Evaluator", "Calculates the penalty score of a symbolic classification solution.")]
  [StorableType("59D79396-2CBD-4033-9752-52A44243D554")]
  public class SymbolicClassificationSingleObjectivePenaltyScoreEvaluator : SymbolicClassificationSingleObjectiveEvaluator, ISymbolicClassificationModelCreatorOperator {
    private const string ModelCreatorParameterName = "ModelCreator";
    public override bool Maximization { get { return false; } }

    public IValueLookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (IValueLookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    ILookupParameter<ISymbolicClassificationModelCreator> ISymbolicClassificationModelCreatorOperator.ModelCreatorParameter {
      get { return ModelCreatorParameter; }
    }

    [StorableConstructor]
    protected SymbolicClassificationSingleObjectivePenaltyScoreEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicClassificationSingleObjectivePenaltyScoreEvaluator(SymbolicClassificationSingleObjectivePenaltyScoreEvaluator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationSingleObjectivePenaltyScoreEvaluator()
      : base() {
      Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSingleObjectivePenaltyScoreEvaluator(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (!Parameters.ContainsKey(ModelCreatorParameterName))
        Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
      #endregion
    }


    public override IOperation InstrumentedApply() {
      double quality = Evaluate(ExecutionContext, SymbolicExpressionTreeParameter.ActualValue, ProblemDataParameter.ActualValue, GenerateRowsToEvaluate());
      QualityParameter.ActualValue = new DoubleValue(quality);
      return base.InstrumentedApply();
    }

    public static double Calculate(IClassificationModel model, IClassificationProblemData problemData, IEnumerable<int> rows) {
      var estimations = model.GetEstimatedClassValues(problemData.Dataset, rows).GetEnumerator();
      if (!estimations.MoveNext()) return double.NaN;

      var penalty = 0.0;
      var count = 0;
      foreach (var r in rows) {
        var actualClass = problemData.Dataset.GetDoubleValue(problemData.TargetVariable, r);
        penalty += problemData.GetClassificationPenalty(actualClass, estimations.Current);
        estimations.MoveNext();
        count++;
      }
      return penalty / count;
    }

    public override double Evaluate(IExecutionContext context, ISymbolicExpressionTree tree, IClassificationProblemData problemData, IEnumerable<int> rows) {
      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = context;
      EstimationLimitsParameter.ExecutionContext = context;
      ModelCreatorParameter.ExecutionContext = context;
      ApplyLinearScalingParameter.ExecutionContext = context;

      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel(problemData.TargetVariable, tree, SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(problemData);
      model.RecalculateModelParameters(problemData, rows);
      double penalty = Calculate(model, problemData, rows);

      SymbolicDataAnalysisTreeInterpreterParameter.ExecutionContext = null;
      EstimationLimitsParameter.ExecutionContext = null;
      ModelCreatorParameter.ExecutionContext = null;
      ApplyLinearScalingParameter.ExecutionContext = null;

      return penalty;
    }
  }
}
