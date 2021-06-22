using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("4B69A82A-265B-46DA-9055-B6E0EB6C3EC8")]
  public class SymbolicRegressionSingleObjectiveMetaModelAnalyzer 
    : SymbolicRegressionMetaModelAnalyzer<SymbolicRegressionSingleObjectiveProblem>, ISymbolicExpressionTreeAnalyzer {

    #region constants
    private const string BestMetaModelParameterName = "Best Meta Model";
    #endregion

    #region parameter properties
    public IResultParameter<ItemList<ISymbolicRegressionSolution>> BestMetaModelParameter =>
      (IResultParameter<ItemList<ISymbolicRegressionSolution>>)Parameters[BestMetaModelParameterName];
    #endregion

    #region constructors and cloning
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveMetaModelAnalyzer(StorableConstructorFlag _) : base(_) { }

    protected SymbolicRegressionSingleObjectiveMetaModelAnalyzer(SymbolicRegressionSingleObjectiveMetaModelAnalyzer original, Cloner cloner) : base(original, cloner) { }

    public SymbolicRegressionSingleObjectiveMetaModelAnalyzer() {
      Parameters.Add(new ResultParameter<ItemList<ISymbolicRegressionSolution>>(BestMetaModelParameterName,
        "A list with the meta model for all problems."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Parameters.Remove(BestMetaModelParameterName);

      if (!Parameters.ContainsKey(BestMetaModelParameterName))
        Parameters.Add(new ResultParameter<ItemList<ISymbolicRegressionSolution>>(BestMetaModelParameterName,
          "A list with all meta models for the problems."));
    }

    public override IDeepCloneable Clone(Cloner cloner) => new SymbolicRegressionSingleObjectiveMetaModelAnalyzer(this, cloner);
    #endregion

    protected override void PerformApply(
      SymbolicRegressionSingleObjectiveProblem baseProblem, 
      IEnumerable<SymbolicRegressionSingleObjectiveProblem> problems, 
      string targetVariable) {
      // init
      var solutions = this.SymbolicExpressionTree.ToArray();
      var bestQualityWithConstantOpt = baseProblem.Maximization.Value ? double.MinValue : double.MaxValue;
      var bestQualityWithoutConstantOpt = baseProblem.Maximization.Value ? double.MinValue : double.MaxValue;
      var evaluator = baseProblem.Evaluator;
      var interpreter = baseProblem.SymbolicExpressionTreeInterpreter;
      ISymbolicExpressionTree bestSolutionWithConstantOpt = null;
      ISymbolicExpressionTree bestSolutionWithoutConstantOpt = null;
      var metaModels = new ItemList<ISymbolicRegressionSolution>();
      // iterate solutions
      foreach (var solution in solutions) {
        // calculate with constant optimization
        var tmpSolution = (ISymbolicExpressionTree) solution.Clone();
        double qualityAvg = CalculateAverageQuality(tmpSolution, evaluator, problems, interpreter, true);
        // check if this solution is the best
        bool isBest = baseProblem.Maximization.Value ? (bestQualityWithConstantOpt < qualityAvg) : (bestQualityWithConstantOpt > qualityAvg);
        if (isBest) {
          bestQualityWithConstantOpt = qualityAvg;
          bestSolutionWithConstantOpt = tmpSolution;
        }

        // calculate it again without constant optimization to have a comparison
        tmpSolution = (ISymbolicExpressionTree) solution.Clone();
        qualityAvg = CalculateAverageQuality(tmpSolution, evaluator, problems, interpreter, false);
        // check if this solution is the best
        isBest = baseProblem.Maximization.Value ? (bestQualityWithoutConstantOpt < qualityAvg) : (bestQualityWithoutConstantOpt > qualityAvg);
        if (isBest) {
          bestQualityWithoutConstantOpt = qualityAvg;
          bestSolutionWithoutConstantOpt = tmpSolution;
        }
      }

      foreach(var problem in problems) {
        metaModels.Add(BuildSolution(bestSolutionWithConstantOpt, targetVariable, problem, "withConstantOpt"));
        metaModels.Add(BuildSolution(bestSolutionWithoutConstantOpt, targetVariable, problem, "withoutConstantOpt"));
      }
        
      BestMetaModelParameter.ActualValue = metaModels;
    }

    private SymbolicRegressionSolution BuildSolution(
      ISymbolicExpressionTree solution, 
      string targetVariable,
      SymbolicRegressionSingleObjectiveProblem problem,
      string suffix) {
      var model = new SymbolicRegressionModel(
            targetVariable,
            (ISymbolicExpressionTree)solution.Clone(),
            new SymbolicDataAnalysisExpressionTreeInterpreter());
      return new SymbolicRegressionSolution(model, problem.ProblemData) { Name = $"{problem.Name}_solution_{suffix}" };
    }

    private double CalculateAverageQuality(
      ISymbolicExpressionTree solution,
      ISymbolicDataAnalysisSingleObjectiveEvaluator<IRegressionProblemData> evaluator,
      IEnumerable<SymbolicRegressionSingleObjectiveProblem> problems,
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter,
      bool useConstantOptimization) {
      double qualitySum = 0.0;
      // iterate problems
      foreach (var problem in problems) {
        var problemData = problem.ProblemData;

        if (useConstantOptimization) {
          SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(
            interpreter, 
            solution, 
            problemData, 
            problemData.TrainingIndices,
            false, 10, true);
        }

        qualitySum += evaluator.Evaluate(
          ExecutionContext,
          solution,
          problemData,
          problemData.TrainingIndices);
      }
        
      // calculate the average quality
      return qualitySum / problems.Count();
    }
  }
}
