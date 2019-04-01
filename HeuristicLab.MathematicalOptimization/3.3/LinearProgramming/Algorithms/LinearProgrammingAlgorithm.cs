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
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("Mixed-Integer Linear Programming (LP, MIP)", "Linear/mixed integer programming implemented in several solvers. " +
    "See also https://dev.heuristiclab.com/trac.fcgi/wiki/Documentation/Reference/ExactOptimization")]
  [Creatable(CreatableAttribute.Categories.ExactAlgorithms)]
  [StorableType("D6BAE020-6315-4C8A-928F-E47C67F3BE8F")]
  public sealed class LinearProgrammingAlgorithm : BasicAlgorithm {

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> dualToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<BoolValue> presolveParam;

    [Storable]
    private readonly IFixedValueParameter<DoubleValue> primalToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<PercentValue> relativeGapToleranceParam;

    [Storable]
    private readonly IFixedValueParameter<BoolValue> scalingParam;

    [Storable]
    private readonly IFixedValueParameter<TimeSpanValue> timeLimitParam;

    [Storable]
    private IConstrainedValueParameter<ILinearSolver> linearSolverParam;

    #region Problem Properties

    public new LinearProblem Problem {
      get => (LinearProblem)base.Problem;
      set => base.Problem = value;
    }

    public override Type ProblemType { get; } = typeof(LinearProblem);

    #endregion
    #region Parameter Properties

    public IFixedValueParameter<DoubleValue> DualToleranceParameter => dualToleranceParam;
    public IConstrainedValueParameter<ILinearSolver> LinearSolverParameter => linearSolverParam;
    public IFixedValueParameter<BoolValue> PresolveParameter => presolveParam;
    public IFixedValueParameter<DoubleValue> PrimalToleranceParameter => primalToleranceParam;
    public IFixedValueParameter<PercentValue> RelativeGapToleranceParameter => relativeGapToleranceParam;
    public IFixedValueParameter<BoolValue> ScalingParameter => scalingParam;
    public IFixedValueParameter<TimeSpanValue> TimeLimitParameter => timeLimitParam;

    #endregion
    #region Properties

    public double DualTolerance {
      get => dualToleranceParam.Value.Value;
      set => dualToleranceParam.Value.Value = value;
    }

    public ILinearSolver LinearSolver {
      get => linearSolverParam.Value;
      set => linearSolverParam.Value = value;
    }

    public bool Presolve {
      get => presolveParam.Value.Value;
      set => presolveParam.Value.Value = value;
    }

    public double PrimalTolerance {
      get => primalToleranceParam.Value.Value;
      set => primalToleranceParam.Value.Value = value;
    }

    public double RelativeGapTolerance {
      get => relativeGapToleranceParam.Value.Value;
      set => relativeGapToleranceParam.Value.Value = value;
    }

    public bool Scaling {
      get => scalingParam.Value.Value;
      set => scalingParam.Value.Value = value;
    }

    public override bool SupportsPause => LinearSolver.SupportsPause;
    public override bool SupportsStop => LinearSolver.SupportsStop;

    public TimeSpan TimeLimit {
      get => timeLimitParam.Value.Value;
      set => timeLimitParam.Value.Value = value;
    }

    #endregion

    public LinearProgrammingAlgorithm() {
      Parameters.Add(linearSolverParam =
        new ConstrainedValueParameter<ILinearSolver>(nameof(LinearSolver), "The solver used to solve the model."));

      ILinearSolver defaultSolver;
      linearSolverParam.ValidValues.Add(defaultSolver = new CoinOrSolver());
      linearSolverParam.ValidValues.Add(new CplexSolver());
      linearSolverParam.ValidValues.Add(new GlopSolver());
      linearSolverParam.ValidValues.Add(new GurobiSolver());
      linearSolverParam.ValidValues.Add(new ScipSolver());
      linearSolverParam.Value = defaultSolver;

      Parameters.Add(relativeGapToleranceParam = new FixedValueParameter<PercentValue>(nameof(RelativeGapTolerance),
        "Limit for relative MIP gap.", new PercentValue(SolverParameters.DefaultRelativeMipGap)));
      Parameters.Add(timeLimitParam = new FixedValueParameter<TimeSpanValue>(nameof(TimeLimit),
        "Limit for runtime. Set to zero for unlimited runtime.",
        new TimeSpanValue(new TimeSpan(0, 1, 0))));
      Parameters.Add(presolveParam =
        new FixedValueParameter<BoolValue>(nameof(Presolve), "Advanced usage: presolve mode.",
          new BoolValue(SolverParameters.DefaultPresolve == SolverParameters.PresolveValues.PresolveOn)) { Hidden = true });
      Parameters.Add(dualToleranceParam = new FixedValueParameter<DoubleValue>(nameof(DualTolerance),
        "Advanced usage: tolerance for dual feasibility of basic solutions.",
        new DoubleValue(SolverParameters.DefaultDualTolerance)) { Hidden = true });
      Parameters.Add(primalToleranceParam = new FixedValueParameter<DoubleValue>(nameof(PrimalTolerance),
        "Advanced usage: tolerance for primal feasibility of basic solutions. " +
        "This does not control the integer feasibility tolerance of integer " +
        "solutions for MIP or the tolerance used during presolve.",
        new DoubleValue(SolverParameters.DefaultPrimalTolerance)) { Hidden = true });
      Parameters.Add(scalingParam = new FixedValueParameter<BoolValue>(nameof(Scaling),
        "Advanced usage: enable or disable matrix scaling.", new BoolValue()) { Hidden = true });

      Problem = new LinearProblem();
    }

    [StorableConstructor]
    private LinearProgrammingAlgorithm(StorableConstructorFlag _) : base(_) { }

    private LinearProgrammingAlgorithm(LinearProgrammingAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      linearSolverParam = cloner.Clone(original.linearSolverParam);
      relativeGapToleranceParam = cloner.Clone(original.relativeGapToleranceParam);
      timeLimitParam = cloner.Clone(original.timeLimitParam);
      presolveParam = cloner.Clone(original.presolveParam);
      dualToleranceParam = cloner.Clone(original.dualToleranceParam);
      primalToleranceParam = cloner.Clone(original.primalToleranceParam);
      scalingParam = cloner.Clone(original.scalingParam);
    }

    public override IDeepCloneable Clone(Cloner cloner) => new LinearProgrammingAlgorithm(this, cloner);

    public override void Pause() {
      base.Pause();
      LinearSolver.InterruptSolve();
    }

    public override void Prepare() {
      base.Prepare();
      Results.Clear();

      foreach (var solver in linearSolverParam.ValidValues) {
        solver.Reset();
      }
    }

    public override void Stop() {
      base.Stop();
      LinearSolver.InterruptSolve();
    }

    protected override void Run(CancellationToken cancellationToken) {
      LinearSolver.PrimalTolerance = PrimalTolerance;
      LinearSolver.DualTolerance = DualTolerance;
      LinearSolver.Presolve = Presolve;
      LinearSolver.RelativeGapTolerance = RelativeGapTolerance;
      LinearSolver.Scaling = Scaling;
      LinearSolver.TimeLimit = TimeLimit;
      LinearSolver.Solve(Problem.ProblemDefinition, Results, cancellationToken);
    }
  }
}
