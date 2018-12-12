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
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("SCIP", "SCIP (http://scip.zib.de/) must be installed and licenced.")]
  [StorableClass]
  public class ScipSolver : ExternalIncrementalSolver {

    private TimeSpan timeLimit = TimeSpan.Zero;

    public ScipSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.ScipLibraryName }));
      problemTypeParam.Value =
        (EnumValue<ProblemType>)new EnumValue<ProblemType>(ProblemType.MixedIntegerProgramming).AsReadOnly();
      SolverSpecificParameters.Value =
        "# for file format and parameters, see https://scip.zib.de/doc/html/PARAMETERS.php" + Environment.NewLine +
        "# example:" + Environment.NewLine +
        "# branching/random/seed = 10" + Environment.NewLine;
    }

    [StorableConstructor]
    protected ScipSolver(bool deserializing)
      : base(deserializing) {
    }

    protected ScipSolver(ScipSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override OptimizationProblemType OptimizationProblemType =>
      OptimizationProblemType.ScipMixedIntegerProgramming;
    protected override TimeSpan TimeLimit => timeLimit += QualityUpdateInterval;

    public override void Solve(LinearProgrammingAlgorithm algorithm, CancellationToken cancellationToken) {
      timeLimit = TimeSpan.Zero;
      base.Solve(algorithm, cancellationToken);
    }
  }
}
