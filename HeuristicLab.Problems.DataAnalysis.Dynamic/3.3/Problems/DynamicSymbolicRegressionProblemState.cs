using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Dynamic;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic; 

[StorableType("7CAE47C8-96B2-44D4-83A9-0E5E4A408494")]
public class DynamicSymbolicRegressionProblemState
  : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, IDynamicProblemState<DynamicSymbolicRegressionProblemState> 
{
  public const string ProblemParameterName = "InnerProblem";
  private const string OptimizeParametersParameterName = "Optimize Parameters";
  private const string ImpactFactorRepetitionsBestSolutionParameterName = "ImpactFactorRepetionsBestSolution";
  private const string ImpactFactorRepetitionsPopulationParameterName = "ImpactFactorRepetionsPopulation";
  
  public IValueParameter<SymbolicRegressionSingleObjectiveProblem> ProblemParameter => (IValueParameter<SymbolicRegressionSingleObjectiveProblem>)Parameters[ProblemParameterName];
  public IFixedValueParameter<BoolValue> OptimizeParametersParameter => (IFixedValueParameter<BoolValue>)Parameters[OptimizeParametersParameterName];
  public IFixedValueParameter<IntValue> ImpactFactorRepetitionsBestSolutionParameter => (IFixedValueParameter<IntValue>)Parameters[ImpactFactorRepetitionsBestSolutionParameterName];
  public IFixedValueParameter<IntValue> ImpactFactorRepetitionsPopulationParameter => (IFixedValueParameter<IntValue>)Parameters[ImpactFactorRepetitionsPopulationParameterName];

  
  public SymbolicRegressionSingleObjectiveProblem Problem => ProblemParameter.Value;
  public DynamicRegressionProblemData ProblemData { get { return (DynamicRegressionProblemData)Problem.ProblemData; } set { Problem.ProblemData = value; } }
  public bool OptimizeParameters { get { return OptimizeParametersParameter.Value.Value; } set { OptimizeParametersParameter.Value.Value = value; } }
  
  private const string BestTrainingSolutionParameterName = "Current Best Training Solution";
  public IResultParameter<ISymbolicRegressionSolution> BestTrainingSolutionParameter => (IResultParameter<ISymbolicRegressionSolution>)Parameters[BestTrainingSolutionParameterName];
  
  
  [StorableConstructor]
  protected DynamicSymbolicRegressionProblemState(StorableConstructorFlag _) : base(_) { }
  protected DynamicSymbolicRegressionProblemState(DynamicSymbolicRegressionProblemState original, Cloner cloner) : base(original, cloner) { }

  public DynamicSymbolicRegressionProblemState(DynamicSymbolicRegressionProblem dynamicProblem) {
    var problem = new SymbolicRegressionSingleObjectiveProblem {
      ProblemData = dynamicProblem.ProblemData,
      EvaluatorParameter = { Value = dynamicProblem.ModelEvaluator },
      SymbolicExpressionTreeInterpreter = dynamicProblem.Interpreter,
      SymbolicExpressionTreeGrammar = dynamicProblem.Grammar,
      ApplyLinearScaling = { Value = dynamicProblem.ApplyLinearScaling },
      MaximumSymbolicExpressionTreeDepthParameter = { Value = { Value = dynamicProblem.MaximumTreeDepth } },
      MaximumSymbolicExpressionTreeLengthParameter = { Value = { Value = dynamicProblem.MaximumTreeLength } },
      EstimationLimits = { Lower = dynamicProblem.EstimationLimits.Lower, Upper = dynamicProblem.EstimationLimits.Upper }
    };

    Parameters.Add(new ValueParameter<SymbolicRegressionSingleObjectiveProblem>(ProblemParameterName, problem));
    Parameters.Add(new ResultParameter<ISymbolicRegressionSolution>(BestTrainingSolutionParameterName, "") { Hidden = true });
    Parameters.Add(dynamicProblem.OptimizeParametersParameter);
    
    Parameters.Add(dynamicProblem.ImpactFactorRepetitionsBestSolutionParameter);
    Parameters.Add(dynamicProblem.ImpactFactorRepetitionsPopulationParameter);
   
    ConfigureEncoding();
  }

  private void ConfigureEncoding() {
    Encoding.TreeLength = Problem.MaximumSymbolicExpressionTreeLength.Value;
    Encoding.TreeDepth = Problem.MaximumSymbolicExpressionTreeDepth.Value;
    Encoding.Grammar = Problem.SymbolicExpressionTreeGrammar;
    Encoding.SolutionCreator = Problem.SolutionCreator;
    Encoding.ConfigureOperators(Problem.OperatorsParameter.Value.OfType<IOperator>());
    Operators.AddRange(Problem.OperatorsParameter.Value.OfType<IOperator>());
  }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new DynamicSymbolicRegressionProblemState(this, cloner);
  }

  public void Initialize(IRandom random) {
    ConfigureEncoding();
    Update(random, 0);
  }

  public void Update(IRandom random, long version) {
    int index = ProblemData.PartitionsUpdate switch {
      PartitionsUpdateMode.Wrapping => (int)(version % ProblemData.TrainingPartitions.Rows),
      PartitionsUpdateMode.KeepLast => (int)Math.Min(version, ProblemData.TrainingPartitions.Rows - 1),
      _ => 0
    };
    
    ProblemData.TrainingPartition.Start = ProblemData.TrainingPartitions[index, 0];
    ProblemData.TrainingPartition.End = ProblemData.TrainingPartitions[index, 1];
      
    ProblemData.TestPartition.Start = ProblemData.TestPartitions[index, 0];
    ProblemData.TestPartition.End = ProblemData.TestPartitions[index, 1];
  }

  public void MergeFrom(DynamicSymbolicRegressionProblemState original) {
    throw new NotImplementedException();
  }

  public override bool Maximization => !Parameters.ContainsKey(ProblemParameterName) || (Problem?.Maximization.Value ?? true);
  public override double Evaluate(Individual individual, IRandom random) {
    var tree = individual.SymbolicExpressionTree(Encoding.Name);

    if (OptimizeParameters) {
      ParameterOptimization.OptimizeTreeParameters(
        ProblemData, 
        tree,
        rows: ProblemData.TrainingIndices,
        interpreter: Problem.SymbolicExpressionTreeInterpreter,
        lowerEstimationLimit: Problem.EstimationLimits.Lower, upperEstimationLimit: Problem.EstimationLimits.Upper);
    }
    
    return Problem.Evaluator.Evaluate(
      tree,
      ProblemData,
      ProblemData.TrainingIndices,
      Problem.SymbolicExpressionTreeInterpreter,
      Problem.ApplyLinearScaling.Value,
      Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper);
  }

  public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
    base.Analyze(individuals, qualities, results, random);
    
    var best = GetBestIndividual(individuals, qualities).Item1;

    if (!results.ContainsKey(BestTrainingSolutionParameter.ActualName)) {
      results.Add(new Result(BestTrainingSolutionParameter.ActualName, typeof(SymbolicRegressionSolution)));
    }

    var tree = (ISymbolicExpressionTree)best.SymbolicExpressionTree(Encoding.Name).Clone();
    var model = new SymbolicRegressionModel(ProblemData.TargetVariable, tree, 
      Problem.SymbolicExpressionTreeInterpreter,
      Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper);
    if (Problem.ApplyLinearScaling.Value) model.Scale(ProblemData);
    var solution = model.CreateRegressionSolution(ProblemData);
    
    results[BestTrainingSolutionParameter.ActualName].Value = solution;

    if (ImpactFactorRepetitionsBestSolutionParameter.Value.Value > 0) {
      AnalyzeVariableImpacts(results, "BestSolutionVariableImpacts", ImpactFactorRepetitionsBestSolutionParameter.Value.Value, best);
    }
    if (ImpactFactorRepetitionsPopulationParameter.Value.Value > 0) {
      AnalyzeVariableImpacts(results, "PopulationVariableImpacts", ImpactFactorRepetitionsPopulationParameter.Value.Value, individuals);
    }
  }

  private void AnalyzeVariableImpacts(ResultCollection results, string resultName, int repetitions, params Individual[] individuals) {
    if (!results.ContainsKey(resultName)) {
      var newTable = new DataTable(resultName) {
        VisualProperties = {
          XAxisTitle = "Generation", YAxisTitle = "Impact",
          YAxisMinimumFixedValue = 0.0, YAxisMaximumFixedValue = 1.0, YAxisMinimumAuto = false, YAxisMaximumAuto = false
        }
      };
      var variableRows = Problem.ProblemData.InputVariables.CheckedItems
        .Select(v => v.Value.Value)
        .Select(v => new DataRow(v, $"Impact of {v}"));
      newTable.Rows.AddRange(variableRows);
      results.Add(new Result(resultName, "", newTable));
    }
    
    var aggregatedDataTable = (DataTable)results[resultName].Value;
    var allSolutionImpacts = individuals.Select(i => i.SymbolicExpressionTree()).Select(t => CalculateImpact(t, repetitions)).ToList();
    var solutionsImpact = new Dictionary<string, double>();
    foreach (var solutionImpacts in allSolutionImpacts) {
      foreach (var solutionImpact in solutionImpacts) {
        if (!solutionsImpact.ContainsKey(solutionImpact.Key)) solutionsImpact.Add(solutionImpact.Key, 0.0);
        solutionsImpact[solutionImpact.Key] += solutionImpact.Value;
      }
    }
    foreach (var bestSolutionImpact in solutionsImpact) {
      var row = aggregatedDataTable.Rows[bestSolutionImpact.Key];
      row.Values.Add(bestSolutionImpact.Value / individuals.Length);
    }
  }
  
  private Dictionary<string, double> CalculateImpact(ISymbolicExpressionTree tree, int repetitions) {
    var problemData = this.Problem.ProblemData;
    var interpreter = this.Problem.SymbolicExpressionTreeInterpreter;
    var estimationLimits = this.Problem.EstimationLimits;
      
    var model = new SymbolicRegressionModel(problemData.TargetVariable, tree, interpreter, estimationLimits.Lower, estimationLimits.Upper);
    var trainingIndices = problemData.TrainingIndices.ToList();
    
    return RegressionSolutionVariableImpactsCalculator.CalculateImpacts(
      model,
      problemData,
      model.GetEstimatedValues(problemData.Dataset, trainingIndices).ToList(),
      trainingIndices,
      repetitions: repetitions
    ).ToDictionary(x => x.Item1, x => x.Item2);
  }
}
