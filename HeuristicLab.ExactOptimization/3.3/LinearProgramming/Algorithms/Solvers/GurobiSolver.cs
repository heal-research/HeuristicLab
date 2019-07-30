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
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("Gurobi", "Gurobi (http://www.gurobi.com/) must be installed and licensed.")]
  [StorableType("2C567159-8C09-4B5F-BFF8-D72561686C6B")]
  public class GurobiSolver : ExternalIncrementalLinearSolver {

    public GurobiSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.GurobiLibraryName }));
      SolverSpecificParameters =
        "# for file format, see http://www.gurobi.com/documentation/8.1/refman/prm_format.html" + Environment.NewLine +
        "# for parameters, see http://www.gurobi.com/documentation/8.1/refman/parameters.html" + Environment.NewLine +
        "# examples:" + Environment.NewLine +
        "# Seed 10" + Environment.NewLine +
        "# Method 2 # Barrier (LP, root node of MIP)" + Environment.NewLine +
        "# NodeMethod 2 # Barrier (MIP)" + Environment.NewLine;
    }

    protected GurobiSolver(GurobiSolver original, Cloner cloner)
          : base(original, cloner) {
      problemTypeParam = cloner.Clone(original.problemTypeParam);
    }

    [StorableConstructor]
    protected GurobiSolver(StorableConstructorFlag _) : base(_) { }

    protected override Solver.OptimizationProblemType OptimizationProblemType =>
      ProblemType == ProblemType.LinearProgramming
        ? Solver.OptimizationProblemType.GurobiLinearProgramming
        : Solver.OptimizationProblemType.GurobiMixedIntegerProgramming;

    public override IDeepCloneable Clone(Cloner cloner) => new GurobiSolver(this, cloner);
  }
}
