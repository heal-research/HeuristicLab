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
  [StorableType("A1A271EC-8B81-452A-A9FB-7BDF223C18E4")]
  public class TimeWindowedDynamicSymbolicRegressionProblemState
    : SingleObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, IDynamicProblemState<TimeWindowedDynamicSymbolicRegressionProblemState>
  {
    public const string ProblemParameterName = "InnerProblem";
    public const string WindowStartParameterName = "WindowStart";
    public const string WindowSizeParameterName = "WindowSize";
    public const string MoveSizeParameterName = "MoveSize";

    public IFixedValueParameter<IntValue> WindowStartParameter => (IFixedValueParameter<IntValue>)Parameters[WindowStartParameterName];
    public IFixedValueParameter<IntValue> WindowSizeParameter => (IFixedValueParameter<IntValue>)Parameters[WindowSizeParameterName];
    public IFixedValueParameter<IntValue> MoveSizeParameter => (IFixedValueParameter<IntValue>)Parameters[MoveSizeParameterName];
    public IValueParameter<SymbolicRegressionSingleObjectiveProblem> ProblemParameter => (IValueParameter<SymbolicRegressionSingleObjectiveProblem>)Parameters[ProblemParameterName];

    public int WindowStart {
      get {
        return WindowStartParameter.Value.Value;
      }
      set {
        WindowStartParameter.Value.Value = value;
      }
    }
    public int WindowSize {
      get {
        return WindowSizeParameter.Value.Value;
      }
      set {
        WindowSizeParameter.Value.Value = value;
      }
    }
    public int MoveSize {
      get {
        return MoveSizeParameter.Value.Value;
      }
      set {
        MoveSizeParameter.Value.Value = value;
      }
    }
    public SymbolicRegressionSingleObjectiveProblem Problem => ProblemParameter.Value;
    public IRegressionProblemData ProblemData {
      get {
        return Problem.ProblemData;
      }
      set {
        Problem.ProblemData = value;
      }
    }

    [StorableConstructor]
    protected TimeWindowedDynamicSymbolicRegressionProblemState(StorableConstructorFlag _) : base(_) { }
    protected TimeWindowedDynamicSymbolicRegressionProblemState(TimeWindowedDynamicSymbolicRegressionProblemState original, Cloner cloner) : base(original, cloner) { }

    public TimeWindowedDynamicSymbolicRegressionProblemState() {
      Parameters.Add(new ValueParameter<SymbolicRegressionSingleObjectiveProblem>(ProblemParameterName, new SymbolicRegressionSingleObjectiveProblem()));
      Parameters.Add(new FixedValueParameter<IntValue>(WindowStartParameterName, new IntValue(0)));
      Parameters.Add(new FixedValueParameter<IntValue>(WindowSizeParameterName, new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>(MoveSizeParameterName, new IntValue(1)));

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
      return new TimeWindowedDynamicSymbolicRegressionProblemState(this, cloner);
    }

    public void Initialize(IRandom random) {
      ConfigureEncoding();
    }

    public void Update(IRandom random, long version) {
      WindowStart += MoveSize;
    }

    public void MergeFrom(TimeWindowedDynamicSymbolicRegressionProblemState original) {
      //clone?
    }

    public override bool Maximization => Parameters.ContainsKey(ProblemParameterName) ? Problem.Maximization.Value : true;
    public override double Evaluate(Individual individual, IRandom random) {
      return Problem.Evaluator.Evaluate(
        individual.SymbolicExpressionTree(Encoding.Name),
        ProblemData,
        GetWindow().ToArray(),
        Problem.SymbolicExpressionTreeInterpreter,
        Problem.ApplyLinearScaling.Value,
        Problem.EstimationLimits?.Lower ?? 0, Problem.EstimationLimits?.Upper ?? 1);
    }

    private IEnumerable<int> GetWindow() {
      var min = ProblemData.TrainingPartition.Start;
      var size = ProblemData.TrainingPartition.Size;
      var windowEnd = WindowStart + WindowSize;
      for (var i = WindowStart; i < windowEnd; i++) {
        yield return i % size + min;
      }
    }
  }
}
