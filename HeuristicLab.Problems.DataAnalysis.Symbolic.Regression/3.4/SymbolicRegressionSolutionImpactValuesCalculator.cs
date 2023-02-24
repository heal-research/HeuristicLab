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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("927C21BD-8913-406F-ADEA-DA4FED3FE4A2")]
  [Item("SymbolicRegressionSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for regression problems.")]
  public class SymbolicRegressionSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public ConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator> EvaluatorParameter {
      get { return (ConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator>)Parameters["Evaluator"]; }
    }

    public SymbolicRegressionSolutionImpactValuesCalculator() {
      var evaluators = new ItemSet<SymbolicRegressionSingleObjectiveEvaluator>(ApplicationManager.Manager.GetInstances<SymbolicRegressionSingleObjectiveEvaluator>());
      Parameters.Add(new ConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator>("Evaluator", evaluators, evaluators.OfType<SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator>().Single()));
    }

    protected SymbolicRegressionSolutionImpactValuesCalculator(SymbolicRegressionSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolutionImpactValuesCalculator(this, cloner);
    }
    [StorableConstructor]
    protected SymbolicRegressionSolutionImpactValuesCalculator(StorableConstructorFlag _) : base(_) { }
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Evaluator")) {
        var evaluators = new ItemSet<SymbolicRegressionSingleObjectiveEvaluator>(ApplicationManager.Manager.GetInstances<SymbolicRegressionSingleObjectiveEvaluator>());
        Parameters.Add(new ConstrainedValueParameter<SymbolicRegressionSingleObjectiveEvaluator>("Evaluator", evaluators, evaluators.OfType<SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator>().Single()));
      }
    }

    protected override double CalculateQualityForImpacts(ISymbolicDataAnalysisModel model, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var regressionModel = (ISymbolicRegressionModel)model;
      var regressionProblemData = (IRegressionProblemData)problemData;
      double quality = EvaluatorParameter.Value.Evaluate(regressionModel.SymbolicExpressionTree, regressionProblemData, rows, regressionModel.Interpreter);
      return quality;
    }
  }
}
