using System;
using System.Threading;
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Problems;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms {

  [Item("Linear/Mixed Integer Programming (LP/MIP)", "")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms)]
  [StorableClass]
  public class LinearProgrammingAlgorithm : BasicAlgorithm {

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> dualToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<EnumValue<LpAlgorithmValues>> lpAlgorithmParam;

    [Storable]
    private readonly IFixedValueParameter<BoolValue> presolveParam;

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> primalToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> relativeGapToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<BoolValue> scalingParam;

    [Storable]
    private readonly IConstrainedValueParameter<ISolver> solverParam;

    [Storable]
    private readonly IFixedValueParameter<TimeSpanValue> timeLimitParam;

    public LinearProgrammingAlgorithm() {
      Parameters.Add(solverParam =
        new ConstrainedValueParameter<ISolver>(nameof(Solver), "The solver used to solve the model."));
      Parameters.Add(relativeGapToleranceParam = new FixedValueParameter<DoubleValue>(nameof(RelativeGapTolerance),
        "Limit for relative MIP gap.", new DoubleValue(MPSolverParameters.kDefaultRelativeMipGap)));
      Parameters.Add(timeLimitParam = new FixedValueParameter<TimeSpanValue>(nameof(TimeLimit),
        "Limit for runtime. Set to zero for unlimited runtime.", new TimeSpanValue()));
      Parameters.Add(presolveParam =
        new FixedValueParameter<BoolValue>(nameof(Presolve), "Advanced usage: presolve mode.", new BoolValue()));
      Parameters.Add(lpAlgorithmParam = new FixedValueParameter<EnumValue<LpAlgorithmValues>>(nameof(LpAlgorithm),
        "Algorithm to solve linear programs.", new EnumValue<LpAlgorithmValues>(LpAlgorithmValues.DualSimplex)));
      Parameters.Add(dualToleranceParam = new FixedValueParameter<DoubleValue>(nameof(DualTolerance),
        "Advanced usage: tolerance for dual feasibility of basic solutions.",
        new DoubleValue(MPSolverParameters.kDefaultDualTolerance)));
      Parameters.Add(primalToleranceParam = new FixedValueParameter<DoubleValue>(nameof(PrimalTolerance),
        "Advanced usage: tolerance for primal feasibility of basic solutions. " +
        "This does not control the integer feasibility tolerance of integer " +
        "solutions for MIP or the tolerance used during presolve.",
        new DoubleValue(MPSolverParameters.kDefaultPrimalTolerance)));
      Parameters.Add(scalingParam = new FixedValueParameter<BoolValue>(nameof(Scaling),
        "Advanced usage: enable or disable matrix scaling.", new BoolValue()));

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
      : base(deserializing) {
    }

    private LinearProgrammingAlgorithm(LinearProgrammingAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      solverParam = cloner.Clone(original.solverParam);
      relativeGapToleranceParam = cloner.Clone(original.relativeGapToleranceParam);
      timeLimitParam = cloner.Clone(original.timeLimitParam);
      presolveParam = cloner.Clone(original.presolveParam);
      lpAlgorithmParam = cloner.Clone(original.lpAlgorithmParam);
      dualToleranceParam = cloner.Clone(original.dualToleranceParam);
      primalToleranceParam = cloner.Clone(original.primalToleranceParam);
      scalingParam = cloner.Clone(original.scalingParam);
    }

    public double DualTolerance {
      get => dualToleranceParam.Value.Value;
      set => dualToleranceParam.Value.Value = value;
    }

    public LpAlgorithmValues LpAlgorithm {
      get => lpAlgorithmParam.Value.Value;
      set => lpAlgorithmParam.Value.Value = value;
    }

    public bool Presolve {
      get => presolveParam.Value.Value;
      set => presolveParam.Value.Value = value;
    }

    public double PrimalTolerance {
      get => primalToleranceParam.Value.Value;
      set => primalToleranceParam.Value.Value = value;
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

    public bool Scaling {
      get => scalingParam.Value.Value;
      set => scalingParam.Value.Value = value;
    }

    public ISolver Solver {
      get => solverParam.Value;
      set => solverParam.Value = value;
    }

    public override bool SupportsPause => Solver.SupportsPause;

    public override bool SupportsStop => Solver.SupportsStop;

    public TimeSpan TimeLimit {
      get => timeLimitParam.Value.Value;
      set => timeLimitParam.Value.Value = value;
    }

    public override IDeepCloneable Clone(Cloner cloner) => new LinearProgrammingAlgorithm(this, cloner);

    public override void Pause() {
      base.Pause();
      Solver.Interrupt();
    }

    public override void Prepare() {
      base.Prepare();
      Results.Clear();

      foreach (var solver in solverParam.ValidValues) {
        solver.Reset();
      }
    }

    public override void Stop() {
      base.Stop();
      Solver.Interrupt();
    }
    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
    }
    protected override void Run(CancellationToken cancellationToken) => Solver.Solve(this, cancellationToken);
  }
}