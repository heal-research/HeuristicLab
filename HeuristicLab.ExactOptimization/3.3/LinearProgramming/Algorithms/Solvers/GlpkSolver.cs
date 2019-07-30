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

using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("GLPK", "GLPK (https://www.gnu.org/software/glpk/) can be used out of the box.")]
  [StorableType("CD1F0577-D68A-483B-8811-17E04573B3F2")]
  public class GlpkSolver : ExternalLinearSolver {

    public GlpkSolver() {
      Parameters.Remove(solverSpecificParametersParam);
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.GlpkLibraryName }));
    }

    protected GlpkSolver(GlpkSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected GlpkSolver(StorableConstructorFlag _) : base(_) { }

    public override bool SupportsPause => false;

    protected override Solver.OptimizationProblemType OptimizationProblemType =>
      ProblemType == ProblemType.LinearProgramming
        ? Solver.OptimizationProblemType.GlpkLinearProgramming
        : Solver.OptimizationProblemType.GlpkMixedIntegerProgramming;

    public override IDeepCloneable Clone(Cloner cloner) => new GlpkSolver(this, cloner);
  }
}
