#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

  [Item("SCIP", "SCIP (http://scip.zib.de/) must be installed and licensed.")]
  [StorableType("A4185E0D-25AE-48C4-8136-A764780683D5")]
  public class ScipSolver : ExternalIncrementalLinearSolver {
    private TimeSpan timeLimit = TimeSpan.Zero;

    public ScipSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.ScipLibraryName }));
      problemTypeParam.Value =
        (EnumValue<ProblemType>)new EnumValue<ProblemType>(ProblemType.MixedIntegerProgramming).AsReadOnly();
      SolverSpecificParameters =
        "# for file format and parameters, see https://scip.zib.de/doc/html/PARAMETERS.php" + Environment.NewLine +
        "# examples:" + Environment.NewLine +
        "# branching/random/seed = 10" + Environment.NewLine +
        "# lp/initalgorithm = b # Barrier (root node of MIP)" + Environment.NewLine +
        "# lp/resolvealgorithm = b # Barrier (MIP)" + Environment.NewLine;
    }

    [StorableConstructor]
    protected ScipSolver(StorableConstructorFlag _) : base(_) { }

    protected ScipSolver(ScipSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override Solver.OptimizationProblemType OptimizationProblemType =>
      Solver.OptimizationProblemType.ScipMixedIntegerProgramming;

    protected override TimeSpan IntermediateTimeLimit => timeLimit += QualityUpdateInterval;

    public override IDeepCloneable Clone(Cloner cloner) => new ScipSolver(this, cloner);

    public override void Solve(ILinearProblemDefinition problemDefintion, ResultCollection results, CancellationToken cancellationToken) {
      timeLimit = TimeSpan.Zero;
      base.Solve(problemDefintion, results, cancellationToken);
    }
  }
}
