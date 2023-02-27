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
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableType("54D82779-7A37-43E4-AFD6-0C3E8D24F6EE")]
  [Item("SymbolicClassificationSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for classification problems.")]
  public class SymbolicClassificationSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public IValueParameter<SymbolicClassificationSingleObjectiveEvaluator> EvaluatorParameter {
      get { return (IValueParameter<SymbolicClassificationSingleObjectiveEvaluator>)Parameters["Evaluator"]; }
    }

    public SymbolicClassificationSolutionImpactValuesCalculator(SymbolicClassificationSingleObjectiveEvaluator evaluator = null) {
      if (evaluator == null) evaluator = new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator();
      Parameters.Add(new ValueParameter<SymbolicClassificationSingleObjectiveEvaluator>("Evaluator", evaluator));
    }

    protected SymbolicClassificationSolutionImpactValuesCalculator(SymbolicClassificationSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSolutionImpactValuesCalculator(this, cloner);
    }
    [StorableConstructor]
    protected SymbolicClassificationSolutionImpactValuesCalculator(StorableConstructorFlag _) : base(_) { }
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Evaluator")) {
        Parameters.Add(new ValueParameter<SymbolicClassificationSingleObjectiveEvaluator>("Evaluator", new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator()));
      }
    }

    protected override double CalculateQualityForImpacts(ISymbolicDataAnalysisModel model, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;
      classificationModel.RecalculateModelParameters(classificationProblemData, rows);
      double qualityForImpactsCalculation = EvaluatorParameter.Value.Evaluate(classificationModel.SymbolicExpressionTree, classificationProblemData, rows, model.Interpreter);
      return EvaluatorParameter.Value.Maximization ? qualityForImpactsCalculation : -qualityForImpactsCalculation;
    }
  }
}
