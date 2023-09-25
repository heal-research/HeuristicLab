﻿using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Dynamic;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic; 

[StorableType("7CAE47C8-96B2-44D4-83A9-0E5E4A408494")]
public class DynamicSymbolicRegressionProblemState
  : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, IDynamicProblemState<DynamicSymbolicRegressionProblemState> 
{
  public const string ProblemParameterName = "InnerProblem";
  public IValueParameter<SymbolicRegressionSingleObjectiveProblem> ProblemParameter => (IValueParameter<SymbolicRegressionSingleObjectiveProblem>)Parameters[ProblemParameterName];
  
  public SymbolicRegressionSingleObjectiveProblem Problem => ProblemParameter.Value;
  public DynamicRegressionProblemData ProblemData {
    get { return (DynamicRegressionProblemData)Problem.ProblemData; }
    set { Problem.ProblemData = value; }
  }

  [StorableConstructor]
  protected DynamicSymbolicRegressionProblemState(StorableConstructorFlag _) : base(_) { }
  protected DynamicSymbolicRegressionProblemState(DynamicSymbolicRegressionProblemState original, Cloner cloner) : base(original, cloner) { }

  public DynamicSymbolicRegressionProblemState(DynamicRegressionProblemData dynamicRegressionProblemData) {
    var problem = new SymbolicRegressionSingleObjectiveProblem() {
      ProblemData = dynamicRegressionProblemData
    };
    Parameters.Add(new ValueParameter<SymbolicRegressionSingleObjectiveProblem>(ProblemParameterName, problem));
   
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
    return Problem.Evaluator.Evaluate(
      individual.SymbolicExpressionTree(Encoding.Name),
      ProblemData,
      ProblemData.TrainingIndices,
      Problem.SymbolicExpressionTreeInterpreter,
      Problem.ApplyLinearScaling.Value,
      Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper);
  }

  public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
    base.Analyze(individuals, qualities, results, random);
  }
}
