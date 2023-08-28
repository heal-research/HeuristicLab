using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("7CAE47C8-96B2-44D4-83A9-0E5E4A408494")]
  public class DynamicSymbolicRegressionProblemState 
    : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, IDynamicProblemState<DynamicSymbolicRegressionProblemState>
  {
    public const string ProblemParameterName = "InnerProblem";

    public IValueParameter<IntMatrix> TrainingPartitionsParameter => (IValueParameter<IntMatrix>)Parameters["TrainingPartitions"];
    public IValueParameter<IntMatrix> TestPartitionsParameter => (IValueParameter<IntMatrix>)Parameters["TestPartitions"];
    
    public IValueParameter<IntArray> CurrentTrainingPartitionParameter => (IValueParameter<IntArray>)Parameters["CurrentTrainingPartition"];
    public IValueParameter<IntArray> CurrentTestPartitionParameter => (IValueParameter<IntArray>)Parameters["CurrentTestPartition"];
    
    public IValueParameter<SymbolicRegressionSingleObjectiveProblem> ProblemParameter => (IValueParameter<SymbolicRegressionSingleObjectiveProblem>)Parameters[ProblemParameterName];
    
    

    public IntMatrix TrainingPartitions {
      get { return TrainingPartitionsParameter.Value; }
      set { TrainingPartitionsParameter.Value = value; }
    }
    public IntMatrix TestPartitions {
      get { return TestPartitionsParameter.Value; }
      set { TestPartitionsParameter.Value = value; }
    }
    
    public IntArray CurrentTrainingPartition {
      get { return CurrentTrainingPartitionParameter.Value; }
      set { CurrentTrainingPartitionParameter.Value = value; }
    }
    public IntArray CurrentTestPartition {
      get { return CurrentTestPartitionParameter.Value; }
      set { CurrentTestPartitionParameter.Value = value; }
    }
    
    public SymbolicRegressionSingleObjectiveProblem Problem => ProblemParameter.Value;
    public IRegressionProblemData ProblemData {
      get { return Problem.ProblemData; }
      set { Problem.ProblemData = value; }
    }

    [StorableConstructor]
    protected DynamicSymbolicRegressionProblemState(StorableConstructorFlag _) : base(_) { }
    protected DynamicSymbolicRegressionProblemState(DynamicSymbolicRegressionProblemState original, Cloner cloner) : base(original, cloner) { }

    public DynamicSymbolicRegressionProblemState() {
      Parameters.Add(new ValueParameter<SymbolicRegressionSingleObjectiveProblem>(ProblemParameterName, new SymbolicRegressionSingleObjectiveProblem()));
      Parameters.Add(new ValueParameter<IntMatrix>("TrainingPartitions", new IntMatrix(new int[4, 2] { { 0,  5}, { 5, 10}, {10, 15}, {15, 20} })));
      Parameters.Add(new ValueParameter<IntMatrix>("TestPartitions",     new IntMatrix(new int[4, 2] { { 5, 10}, {10, 15}, {15, 20}, {20, 25} })));
      Parameters.Add(new ValueParameter<IntArray>("CurrentTrainingPartition", new IntArray(2)));
      Parameters.Add(new ValueParameter<IntArray>("CurrentTestPartition",     new IntArray(2)));
      
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
    }

    public void Update(IRandom random, long version) {
      int index = (int)(version % TrainingPartitions.Rows);

      CurrentTrainingPartition[0] = TrainingPartitions[index, 0];
      CurrentTrainingPartition[1] = TrainingPartitions[index, 1];
      
      CurrentTestPartition[0] = TestPartitions[index, 0];
      CurrentTestPartition[1] = TestPartitions[index, 1];
    }

    public void MergeFrom(DynamicSymbolicRegressionProblemState original) {
      throw new NotImplementedException();
    }

    public override bool Maximization => !Parameters.ContainsKey(ProblemParameterName) || (Problem?.Maximization.Value??true);
    public override double Evaluate(Individual individual, IRandom random) {
      return Problem.Evaluator.Evaluate(
        individual.SymbolicExpressionTree(Encoding.Name), 
        ProblemData, 
        GetTrainingIndices(), 
        Problem.SymbolicExpressionTreeInterpreter, 
        Problem.ApplyLinearScaling.Value, 
        Problem.EstimationLimits?.Lower ?? 0, Problem.EstimationLimits?.Upper ?? 1);
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(individuals, qualities, results, random);
    }

    private IEnumerable<int> GetTrainingIndices() {
      int begin = CurrentTrainingPartition[0];
      int end = CurrentTrainingPartition[1];
      return Enumerable.Range(begin, end - begin);
    }
    private IEnumerable<int> GetTestIndices() {
      int begin = CurrentTestPartition[0];
      int end = CurrentTestPartition[1];
      return Enumerable.Range(begin, end - begin);
    }
  }
}
