using System;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Problems;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {
  [Item("Linear Programming Algorithm", "")]
  [Creatable(CreatableAttribute.Categories.Algorithms, Priority = 162)]
  [StorableClass]
  public class LinearProgrammingAlgorithm : BasicAlgorithm {

    [Storable]
    private readonly IConstrainedValueParameter<ISolver> solverParam;

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> relativeGapToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<TimeSpanValue> timeLimitParam;

    public LinearProgrammingAlgorithm() {
      Parameters.Add(solverParam = new ConstrainedValueParameter<ISolver>(nameof(Solver)));
      Parameters.Add(relativeGapToleranceParam = new FixedValueParameter<DoubleValue>(nameof(RelativeGapTolerance), new DoubleValue()));
      Parameters.Add(timeLimitParam = new FixedValueParameter<TimeSpanValue>(nameof(TimeLimit), new TimeSpanValue()));

      Problem = new LinearProgrammingProblem();

      solverParam.ValidValues.Add(new CoinOrSolver());
      solverParam.ValidValues.Add(new CplexSolver());
      solverParam.ValidValues.Add(new GlpkSolver());
      solverParam.ValidValues.Add(new GurobiSolver());
      solverParam.ValidValues.Add(new ScipSolver());
      solverParam.ValidValues.Add(new BopSolver());
      solverParam.ValidValues.Add(new GlopSolver());
    }

    [StorableConstructor]
    private LinearProgrammingAlgorithm(bool deserializing)
      : base(deserializing) { }

    private LinearProgrammingAlgorithm(LinearProgrammingAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      solverParam = cloner.Clone(original.solverParam);
      relativeGapToleranceParam = cloner.Clone(original.relativeGapToleranceParam);
      timeLimitParam = cloner.Clone(original.timeLimitParam);
    }

    public ISolver Solver {
      get => solverParam.Value;
      set => solverParam.Value = value;
    }

    public new LinearProgrammingProblem Problem {
      get => (LinearProgrammingProblem)base.Problem;
      set => base.Problem = value;
    }

    public override Type ProblemType { get; } = typeof(LinearProgrammingProblem);
    public double RelativeGapTolerance {
      get => relativeGapToleranceParam.Value.Value;
      set => relativeGapToleranceParam.Value.Value = value;
    }

    public override ResultCollection Results { get; } = new ResultCollection();

    public override bool SupportsPause { get; } = false;

    public TimeSpan TimeLimit {
      get => timeLimitParam.Value.Value;
      set => timeLimitParam.Value.Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) => new LinearProgrammingAlgorithm(this, cloner);
    protected override void Run(CancellationToken cancellationToken) {
      using (var solver = LinearSolver.CreateSolver(Solver.OptimizationProblemType, Name,
        Solver.LibraryName ?? string.Empty, s => Problem.ProblemDefinition.BuildModel(s))) {
        solver.RelativeGapTolerance = RelativeGapTolerance;
        solver.TimeLimit = TimeLimit;

        solver.Solve();

        Problem.ProblemDefinition.Analyze(solver.Solver, Results);
        Results.AddOrUpdateResult("Result Status", new EnumValue<ResultStatus>(solver.ResultStatus));
        Results.AddOrUpdateResult("Best Objective Value", new DoubleValue(solver.ObjectiveValue ?? double.NaN));
        Results.AddOrUpdateResult("Absolute Gap", new DoubleValue(solver.AbsoluteGap ?? double.NaN));
        Results.AddOrUpdateResult("Relative Gap", new DoubleValue(solver.RelativeGap ?? double.NaN));
        Results.AddOrUpdateResult("Number of Constraints", new IntValue(solver.NumberOfConstraints));
        Results.AddOrUpdateResult("Number of Variables", new IntValue(solver.NumberOfVariables));
        Results.AddOrUpdateResult("Number of Nodes", new DoubleValue(solver.NumberOfNodes));
        Results.AddOrUpdateResult("Iterations", new DoubleValue(solver.Iterations));
      }
    }
  }
}