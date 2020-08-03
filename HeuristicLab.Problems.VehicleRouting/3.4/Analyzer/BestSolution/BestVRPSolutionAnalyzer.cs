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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestVRPSolutionAnalyzer", "An operator for analyzing the best solution of Vehicle Routing Problems.")]
  [StorableType("3e1bb409-0b8f-4324-826c-2190aa5fb2b6")]
  public sealed class BestVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {

    [Storable] private IScopeTreeLookupParameter<VRPEvaluation> evaluationParameter;
    public IScopeTreeLookupParameter<VRPEvaluation> EvaluationParameter => evaluationParameter;

    [Storable] private ILookupParameter<VRPSolution> bestSolutionParameter;
    public ILookupParameter<VRPSolution> BestSolutionParameter => bestSolutionParameter;

    [Storable] private IResultParameter<VRPEvaluation> bestEvaluationParameter;
    public IResultParameter<VRPEvaluation> BestSolutionEvaluationParameter => bestEvaluationParameter;

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private BestVRPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private BestVRPSolutionAnalyzer(BestVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      evaluationParameter = cloner.Clone(original.evaluationParameter);
      bestSolutionParameter = cloner.Clone(original.bestSolutionParameter);
      bestEvaluationParameter = cloner.Clone(original.bestEvaluationParameter);
    }
    public BestVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(evaluationParameter = new ScopeTreeLookupParameter<VRPEvaluation>("EvaluationResult", "The qualities of the VRP solutions which should be analyzed."));
      Parameters.Add(bestSolutionParameter = new LookupParameter<VRPSolution>("BestSolution", "The best-so-far solution."));
      Parameters.Add(bestEvaluationParameter = new ResultParameter<VRPEvaluation>("Best VRP Evaluation", "The best VRP evaluation.", "Results"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestVRPSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var evaluations = EvaluationParameter.ActualValue;

      int i = evaluations.Select((x, index) => new { index, Eval = x }).OrderBy(x => x.Eval.Quality).First().index;

      var bestEvaluation = BestSolutionEvaluationParameter.ActualValue;
      
      var bestSolution = BestSolutionParameter.ActualValue;
      if (bestSolution == null || evaluations[i].Quality <= bestSolution.Quality.Value) {
        BestSolutionEvaluationParameter.ActualValue = (VRPEvaluation)evaluations[i].Clone();
      }

      return base.Apply();
    }
  }
}
