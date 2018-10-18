using System;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base {

  [StorableClass]
  public class Solver : ParameterizedNamedItem, ISolver, IDisposable {

    [Storable]
    protected IValueParameter<EnumValue<LinearProgrammingType>> programmingTypeParam;

    protected LinearSolver solver;

    public Solver() {
      Parameters.Add(programmingTypeParam =
        new ValueParameter<EnumValue<LinearProgrammingType>>(nameof(LinearProgrammingType),
          new EnumValue<LinearProgrammingType>()));
    }

    [StorableConstructor]
    protected Solver(bool deserializing)
      : base(deserializing) {
    }

    protected Solver(Solver original, Cloner cloner)
      : base(original, cloner) {
      programmingTypeParam = cloner.Clone(original.programmingTypeParam);
    }

    public LinearProgrammingType LinearProgrammingType {
      get => programmingTypeParam.Value.Value;
      set => programmingTypeParam.Value.Value = value;
    }

    protected virtual OptimizationProblemType OptimizationProblemType { get; }
    public virtual bool SupportsPause => false;
    public virtual bool SupportsStop => false;

    public override IDeepCloneable Clone(Cloner cloner) => new Solver(this, cloner);

    public void Dispose() => solver?.Dispose();

    public void Interrupt() => solver.Stop();

    public virtual void Reset() {
      solver = null;
    }

    public virtual void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken) =>
      Solve(algorithm);

    public virtual void Solve(LinearProgrammingAlgorithm algorithm) =>
      Solve(algorithm, algorithm.TimeLimit, false);

    public virtual void Solve(LinearProgrammingAlgorithm algorithm, TimeSpan timeLimit, bool incrementality) {
      string libraryName = null;
      if (this is IExternalSolver externalSolver)
        libraryName = externalSolver.LibraryName;

      if (solver == null) {
        solver = LinearSolver.CreateSolver(OptimizationProblemType, Name,
          libraryName, s => algorithm.Problem.ProblemDefinition.BuildModel(s));
      }

      solver.TimeLimit = timeLimit;
      solver.RelativeGapTolerance = algorithm.RelativeGapTolerance;
      solver.PrimalTolerance = algorithm.PrimalTolerance;
      solver.DualTolerance = algorithm.DualTolerance;
      solver.Presolve = algorithm.Presolve;
      solver.Scaling = algorithm.Scaling;
      solver.LpAlgorithm = algorithm.LpAlgorithm;
      solver.Incrementality = incrementality;

      solver.Solve();

      algorithm.Problem.ProblemDefinition.Analyze(solver.Solver, algorithm.Results);
      algorithm.Results.AddOrUpdateResult("Result Status", new EnumValue<ResultStatus>(solver.ResultStatus));
      algorithm.Results.AddOrUpdateResult("Best Objective Value",
        new DoubleValue(solver.ObjectiveValue ?? double.NaN));
      algorithm.Results.AddOrUpdateResult("Best Objective Bound",
        new DoubleValue(solver.ObjectiveBound ?? double.NaN));
      algorithm.Results.AddOrUpdateResult("Absolute Gap", new DoubleValue(solver.AbsoluteGap ?? double.NaN));
      algorithm.Results.AddOrUpdateResult("Relative Gap", new DoubleValue(solver.RelativeGap ?? double.NaN));
      algorithm.Results.AddOrUpdateResult("Number of Constraints", new IntValue(solver.NumberOfConstraints));
      algorithm.Results.AddOrUpdateResult("Number of Variables", new IntValue(solver.NumberOfVariables));
      algorithm.Results.AddOrUpdateResult("Number of Nodes", new DoubleValue(solver.NumberOfNodes));
      algorithm.Results.AddOrUpdateResult("Iterations", new DoubleValue(solver.Iterations));
      algorithm.Results.AddOrUpdateResult("Solver Version", new StringValue(solver.SolverVersion));
      algorithm.Results.AddOrUpdateResult("Wall Time", new TimeSpanValue(solver.WallTime ?? TimeSpan.Zero));
    }
  }
}