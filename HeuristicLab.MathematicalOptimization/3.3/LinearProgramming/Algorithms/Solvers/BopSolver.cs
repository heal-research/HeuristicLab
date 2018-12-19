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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming {

  [Item("BOP", "BOP (https://developers.google.com/optimization/reference/bop/bop_solver/) can be used out of the box.")]
  [StorableClass]
  public class BopSolver : IncrementalLinearSolver {

    public BopSolver() {
      Parameters.Remove(problemTypeParam);
      Parameters.Add(new FixedValueParameter<StringValue>(nameof(ProblemType), new StringValue("ZeroOneProgramming").AsReadOnly()));
      SolverSpecificParameters =
        "# for file format, see Protocol Buffers text format (https://developers.google.com/protocol-buffers/docs/overview#whynotxml)" + Environment.NewLine +
        "# for parameters, see https://github.com/google/or-tools/blob/v6.10/ortools/bop/bop_parameters.proto" + Environment.NewLine +
        "# example:" + Environment.NewLine +
        "# random_seed: 10" + Environment.NewLine;
    }

    protected BopSolver(BopSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected BopSolver(bool deserializing)
      : base(deserializing) {
    }

    protected override OptimizationProblemType OptimizationProblemType =>
               OptimizationProblemType.BopIntegerProgramming;
  }
}
