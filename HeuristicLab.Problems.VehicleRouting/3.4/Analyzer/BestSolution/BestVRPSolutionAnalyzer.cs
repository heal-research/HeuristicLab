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
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  //TODO: remove this operator -> part of VRP problem analyze
  [Item("BestVRPSolutionAnalyzer", "An operator for analyzing the best solution of Vehicle Routing Problems.")]
  [StorableType("3e1bb409-0b8f-4324-826c-2190aa5fb2b6")]
  public sealed class BestVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ISingleObjectiveOperator {

    [Storable] private ILookupParameter<IVRPProblemInstance> problemInstanceParameter;
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter => problemInstanceParameter;
    [Storable] private IScopeTreeLookupParameter<IVRPEncodedSolution> vrpToursParameter;
    public IScopeTreeLookupParameter<IVRPEncodedSolution> VRPToursParameter => vrpToursParameter;

    [Storable] private IScopeTreeLookupParameter<VRPEvaluation> evaluationParameter;
    public IScopeTreeLookupParameter<VRPEvaluation> EvaluationParameter => evaluationParameter;

    [Storable] private ILookupParameter<VRPSolution> bestSolutionParameter;
    public ILookupParameter<VRPSolution> BestSolutionParameter => bestSolutionParameter;

    [Storable] private ILookupParameter<VRPSolution> bestFeasibleSolutionParameter;
    public ILookupParameter<VRPSolution> BestFeasibleSolutionParameter => bestFeasibleSolutionParameter;

    [Storable] private IResultParameter<VRPEvaluation> bestEvaluationResult;
    public IResultParameter<VRPEvaluation> BestSolutionEvaluationResult => bestEvaluationResult;
    [Storable] private IResultParameter<VRPEvaluation> bestFeasibleEvaluationResult;
    public IResultParameter<VRPEvaluation> BestFeasibleSolutionEvaluationResult => bestFeasibleEvaluationResult;
    [Storable] private IResultParameter<VRPSolution> bestSolutionResult;
    public IResultParameter<VRPSolution> BestSolutionResult => bestSolutionResult;
    [Storable] private IResultParameter<VRPSolution> bestFeasibleSolutionResult;
    public IResultParameter<VRPSolution> BestFeasibleSolutionResult => bestFeasibleSolutionResult;

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private BestVRPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private BestVRPSolutionAnalyzer(BestVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      problemInstanceParameter = cloner.Clone(original.problemInstanceParameter);
      vrpToursParameter = cloner.Clone(original.vrpToursParameter);
      evaluationParameter = cloner.Clone(original.evaluationParameter);
      bestSolutionParameter = cloner.Clone(original.bestSolutionParameter);
      bestFeasibleSolutionParameter = cloner.Clone(original.bestFeasibleSolutionParameter);
      bestEvaluationResult = cloner.Clone(original.bestEvaluationResult);
      bestFeasibleEvaluationResult = cloner.Clone(original.bestFeasibleEvaluationResult);
      bestSolutionResult = cloner.Clone(original.bestSolutionResult);
      bestFeasibleSolutionResult = cloner.Clone(original.bestFeasibleSolutionResult);
    }
    public BestVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(problemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The VRP problem instance."));
      Parameters.Add(vrpToursParameter = new ScopeTreeLookupParameter<IVRPEncodedSolution>("VRPTours", "The VRP encoded solution."));
      Parameters.Add(evaluationParameter = new ScopeTreeLookupParameter<VRPEvaluation>("EvaluationResult", "The qualities of the VRP solutions which should be analyzed."));
      Parameters.Add(bestSolutionParameter = new LookupParameter<VRPSolution>("BestSolution", "The best-so-far solution."));
      Parameters.Add(bestFeasibleSolutionParameter = new LookupParameter<VRPSolution>("BestFeasibleSolution", "The best-so-far feasible solution."));
      Parameters.Add(bestEvaluationResult = new ResultParameter<VRPEvaluation>("Best VRP Solution Evaluation", "The best VRP evaluation.", "Results"));
      Parameters.Add(bestFeasibleEvaluationResult = new ResultParameter<VRPEvaluation>("Best Feasible VRP Solution Evaluation", "The best feasible VRP evaluation.", "Results"));
      Parameters.Add(bestSolutionResult = new ResultParameter<VRPSolution>("Best VRP Solution", "The best-so-far VRP solution."));
      Parameters.Add(bestFeasibleSolutionResult = new ResultParameter<VRPSolution>("Best feasible VRP Solution", "The best-so-far feasible VRP solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestVRPSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var evaluations = EvaluationParameter.ActualValue;

      var bestInPop = evaluations.Select((x, index) => new { index, Eval = x }).OrderBy(x => x.Eval.Quality).First();
      var bestFeasibleInPop = evaluations.Select((x, index) => new { index, Eval = x }).Where(x => x.Eval.IsFeasible).OrderBy(x => x.Eval.Quality).FirstOrDefault();

      var bestEvaluation = BestSolutionEvaluationResult.ActualValue;
      
      var bestSolution = BestSolutionParameter.ActualValue;
      if (bestSolution == null || bestInPop.Eval.Quality < bestSolution.Evaluation.Quality) {
        var best = new VRPSolution(ProblemInstanceParameter.ActualValue,
          VRPToursParameter.ActualValue[bestInPop.index], (VRPEvaluation)bestInPop.Eval.Clone());
        BestSolutionParameter.ActualValue = best;
        BestSolutionResult.ActualValue = best;
        BestSolutionEvaluationResult.ActualValue = (VRPEvaluation)bestInPop.Eval.Clone();
      };

      if (bestFeasibleInPop != null) {
        var bestFeasibleSolution = BestFeasibleSolutionParameter.ActualValue;
        if (bestFeasibleSolution == null || bestFeasibleInPop.Eval.Quality < bestFeasibleSolution.Evaluation.Quality) {
          var bestFeasible = new VRPSolution(ProblemInstanceParameter.ActualValue,
          VRPToursParameter.ActualValue[bestFeasibleInPop.index], (VRPEvaluation)bestFeasibleInPop.Eval.Clone());
          BestFeasibleSolutionParameter.ActualValue = bestFeasible;
          bestFeasibleSolutionResult.ActualValue = bestFeasible;
          BestFeasibleSolutionEvaluationResult.ActualValue = (VRPEvaluation)bestFeasibleInPop.Eval.Clone();
        }
      }

      return base.Apply();
    }
  }
}
