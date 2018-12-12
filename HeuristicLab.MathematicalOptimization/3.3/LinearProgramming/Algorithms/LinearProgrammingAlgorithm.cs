﻿#region License Information
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

  [Item("Linear/Mixed Integer Programming (LP/MIP)", "Linear/mixed integer programming implemented in several solvers. " +
    "See also https://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/Howto/LinearMixedIntegerProgramming")] // TODO: update link
  [Creatable(CreatableAttribute.Categories.ExactAlgorithms)]
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
    private readonly IFixedValueParameter<PercentValue> relativeGapToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<BoolValue> scalingParam;

    [Storable]
    private IConstrainedValueParameter<ISolver> solverParam;

    [Storable]
    private readonly IFixedValueParameter<TimeSpanValue> timeLimitParam;

    public IConstrainedValueParameter<ISolver> SolverParameter {
      get => solverParam;
      set => solverParam = value;
    }

    public LinearProgrammingAlgorithm() {
      Parameters.Add(solverParam =
        new ConstrainedValueParameter<ISolver>(nameof(Solver), "The solver used to solve the model."));

      ISolver defaultSolver;
      solverParam.ValidValues.Add(new BopSolver());
      solverParam.ValidValues.Add(defaultSolver = new CoinOrSolver());
      solverParam.ValidValues.Add(new CplexSolver());
      solverParam.ValidValues.Add(new GlopSolver());
      solverParam.ValidValues.Add(new GlpkSolver());
      solverParam.ValidValues.Add(new GurobiSolver());
      solverParam.ValidValues.Add(new ScipSolver());
      solverParam.Value = defaultSolver;

      Parameters.Add(relativeGapToleranceParam = new FixedValueParameter<PercentValue>(nameof(RelativeGapTolerance),
        "Limit for relative MIP gap.", new PercentValue(MPSolverParameters.kDefaultRelativeMipGap)));
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
    }

    [StorableConstructor]
    protected LinearProgrammingAlgorithm(bool deserializing)
      : base(deserializing) {
    }

    protected LinearProgrammingAlgorithm(LinearProgrammingAlgorithm original, Cloner cloner)
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
      Solver.InterruptSolve();
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
      Solver.InterruptSolve();
    }

    protected override void Run(CancellationToken cancellationToken) => Solver.Solve(this, cancellationToken);
  }
}
