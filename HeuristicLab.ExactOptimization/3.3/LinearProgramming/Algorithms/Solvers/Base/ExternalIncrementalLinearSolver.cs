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
using HEAL.Attic;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [StorableType("CF3FC8B1-37DF-4A08-9DE5-AB578E1132DD")]
  public class ExternalIncrementalLinearSolver : IncrementalLinearSolver, IExternalLinearSolver {
    protected const string FileDialogFilter = "Dynamic-Link Library (*.dll)|*.dll|All Files (*.*)|*.*";

    [Storable]
    protected IFixedValueParameter<FileValue> libraryNameParam;

    public ExternalIncrementalLinearSolver() {
    }

    [StorableConstructor]
    protected ExternalIncrementalLinearSolver(StorableConstructorFlag _) : base(_) { }

    protected ExternalIncrementalLinearSolver(ExternalIncrementalLinearSolver original, Cloner cloner)
      : base(original, cloner) {
      libraryNameParam = cloner.Clone(original.libraryNameParam);
    }

    public string LibraryName {
      get => libraryNameParam?.Value.Value;
      set => libraryNameParam.Value.Value = value;
    }

    public IFixedValueParameter<FileValue> LibraryNameParameter => libraryNameParam;

    protected override Solver CreateSolver(Solver.OptimizationProblemType optimizationProblemType,
      string libraryName = null) => base.CreateSolver(optimizationProblemType, LibraryName);
  }
}
