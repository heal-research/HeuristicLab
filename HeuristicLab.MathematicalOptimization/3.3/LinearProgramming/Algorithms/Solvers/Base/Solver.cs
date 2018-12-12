#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    protected IValueParameter<EnumValue<ProblemType>> problemTypeParam;

    protected LinearSolver solver;

    [Storable]
    protected IFixedValueParameter<TextValue> solverSpecificParametersParam;

    public Solver() {
      Parameters.Add(problemTypeParam =
        new ValueParameter<EnumValue<ProblemType>>(nameof(ProblemType), new EnumValue<ProblemType>()));
      Parameters.Add(solverSpecificParametersParam =
        new FixedValueParameter<TextValue>(nameof(SolverSpecificParameters), new TextValue()));
    }

    [StorableConstructor]
    protected Solver(bool deserializing)
      : base(deserializing) {
    }

    protected Solver(Solver original, Cloner cloner)
      : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
      solverSpecificParametersParam = cloner.Clone(original.solverSpecificParametersParam);
    }

    public ProblemType ProblemType {
      get => problemTypeParam.Value.Value;
      set => problemTypeParam.Value.Value = value;
    }

    public TextValue SolverSpecificParameters => solverSpecificParametersParam.Value;

    public virtual bool SupportsPause => true;
    public virtual bool SupportsStop => true;
    protected virtual OptimizationProblemType OptimizationProblemType { get; }

    public override IDeepCloneable Clone(Cloner cloner) => new Solver(this, cloner);

    public void Dispose() => solver?.Dispose();

    public bool InterruptSolve() => solver?.InterruptSolve() ?? false;

    public virtual void Reset() {
      solver?.Dispose();
      solver = null;
    }

    public virtual void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken) =>
      Solve(algorithm);

    public virtual void Solve(LinearProgrammingAlgorithm algorithm) =>
      Solve(algorithm, algorithm.TimeLimit);

    public virtual void Solve(LinearProgrammingAlgorithm algorithm, TimeSpan timeLimit) {
      string libraryName = null;
      if (this is IExternalSolver externalSolver)
        libraryName = externalSolver.LibraryName;

      if (solver == null) {
        solver = new LinearSolver(OptimizationProblemType, s => algorithm.Problem.ProblemDefinition.BuildModel(s), Name,
          libraryName);
      }

      solver.TimeLimit = timeLimit;
      solver.RelativeGapTolerance = algorithm.RelativeGapTolerance;
      solver.PrimalTolerance = algorithm.PrimalTolerance;
      solver.DualTolerance = algorithm.DualTolerance;
      solver.Presolve = algorithm.Presolve;
      solver.Scaling = algorithm.Scaling;
      solver.LpAlgorithm = algorithm.LpAlgorithm;
      solver.Incrementality = true;

      if (!solver.SetSolverSpecificParameters(SolverSpecificParameters.Value))
        throw new ArgumentException("Solver specific parameters could not be set.");

      solver.Solve();

      algorithm.Problem.ProblemDefinition.Analyze(solver.Solver, algorithm.Results);
      algorithm.Results.AddOrUpdateResult(nameof(solver.ResultStatus),
        new EnumValue<ResultStatus>(solver.ResultStatus));
      algorithm.Results.AddOrUpdateResult($"Best{nameof(solver.ObjectiveValue)}",
        new DoubleValue(solver.ObjectiveValue ?? double.NaN));

      if (solver.IsMip) {
        algorithm.Results.AddOrUpdateResult($"Best{nameof(solver.ObjectiveBound)}",
          new DoubleValue(solver.ObjectiveBound ?? double.NaN));
        algorithm.Results.AddOrUpdateResult(nameof(solver.AbsoluteGap),
          new DoubleValue(solver.AbsoluteGap ?? double.NaN));
        algorithm.Results.AddOrUpdateResult(nameof(solver.RelativeGap),
          new PercentValue(solver.RelativeGap ?? double.NaN));
      }

      algorithm.Results.AddOrUpdateResult(nameof(solver.NumberOfConstraints), new IntValue(solver.NumberOfConstraints));
      algorithm.Results.AddOrUpdateResult(nameof(solver.NumberOfVariables), new IntValue(solver.NumberOfVariables));

      if (solver.IsMip) {
        algorithm.Results.AddOrUpdateResult(nameof(solver.NumberOfNodes), new DoubleValue(solver.NumberOfNodes));
      }

      algorithm.Results.AddOrUpdateResult(nameof(solver.Iterations), new DoubleValue(solver.Iterations));
      algorithm.Results.AddOrUpdateResult(nameof(solver.SolverVersion), new StringValue(solver.SolverVersion));
    }
  }
}
