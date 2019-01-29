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
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("CPLEX", "CPLEX (https://www.ibm.com/analytics/cplex-optimizer) must be installed and licenced.")]
  [StorableClass]
  public class CplexSolver : ExternalIncrementalLinearSolver {

    public CplexSolver() {
      Parameters.Add(libraryNameParam = new FixedValueParameter<FileValue>(nameof(LibraryName),
        new FileValue { FileDialogFilter = FileDialogFilter, Value = Properties.Settings.Default.CplexLibraryName }));
      SolverSpecificParameters =
        "CPLEX Parameter File Version 12.8.0" + Environment.NewLine +
        "# for file format, see https://www.ibm.com/support/knowledgecenter/SSSA5P_12.8.0/ilog.odms.cplex.help/CPLEX/FileFormats/topics/PRM.html" + Environment.NewLine +
        "# for parameters, see https://www.ibm.com/support/knowledgecenter/SSSA5P_12.8.0/ilog.odms.cplex.help/CPLEX/Parameters/topics/introListTopical.html" + Environment.NewLine +
        "# examples:" + Environment.NewLine +
        "# CPXPARAM_RandomSeed 10" + Environment.NewLine +
        "# CPXPARAM_LPMethod 4 # Barrier (LP)" + Environment.NewLine +
        "# CPXPARAM_MIP_Strategy_SubAlgorithm 4 # Barrier (MIP)" + Environment.NewLine +
        "# CPXPARAM_MIP_Strategy_Search 2 # Apply dynamic search (MIP)" + Environment.NewLine;
    }

    [StorableConstructor]
    protected CplexSolver(bool deserializing)
      : base(deserializing) {
    }

    protected CplexSolver(CplexSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override Solver.OptimizationProblemType OptimizationProblemType =>
      ProblemType == ProblemType.LinearProgramming
        ? Solver.OptimizationProblemType.CplexLinearProgramming
        : Solver.OptimizationProblemType.CplexMixedIntegerProgramming;

    public override IDeepCloneable Clone(Cloner cloner) => new CplexSolver(this, cloner);
  }
}
