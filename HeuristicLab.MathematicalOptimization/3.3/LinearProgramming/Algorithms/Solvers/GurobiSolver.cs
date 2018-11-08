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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers.Base;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Algorithms.Solvers {

  [Item("Gurobi", "Gurobi (http://www.gurobi.com/) must be installed and licenced.")]
  [StorableClass]
  public class GurobiSolver : ExternalIncrementalSolver {

    public GurobiSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.GurobiLibraryName }));
    }

    protected GurobiSolver(GurobiSolver original, Cloner cloner)
          : base(original, cloner) {
      programmingTypeParam = cloner.Clone(original.programmingTypeParam);
    }

    [StorableConstructor]
    protected GurobiSolver(bool deserializing)
      : base(deserializing) {
    }

    public override bool SupportsPause => true;

    public override bool SupportsStop => true;

    protected override OptimizationProblemType OptimizationProblemType =>
              LinearProgrammingType == LinearProgrammingType.LinearProgramming
        ? OptimizationProblemType.GUROBI_LINEAR_PROGRAMMING
        : OptimizationProblemType.GUROBI_MIXED_INTEGER_PROGRAMMING;
  }
}
