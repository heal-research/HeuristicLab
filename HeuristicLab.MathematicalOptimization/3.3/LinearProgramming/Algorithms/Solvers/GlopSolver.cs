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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("Glop", "Glop (https://developers.google.com/optimization/lp/glop) can be used out of the box.")]
  [StorableClass]
  public class GlopSolver : IncrementalLinearSolver {

    public GlopSolver() {
      problemTypeParam.Value = (EnumValue<ProblemType>)problemTypeParam.Value.AsReadOnly();
      SolverSpecificParameters =
        "# for file format, see Protocol Buffers text format (https://developers.google.com/protocol-buffers/docs/overview#whynotxml)" + Environment.NewLine +
        "# for parameters, see https://github.com/google/or-tools/blob/v6.10/ortools/glop/parameters.proto" + Environment.NewLine +
        "# examples:" + Environment.NewLine +
        "# random_seed: 10" + Environment.NewLine +
        "# use_dual_simplex: true (LP)" + Environment.NewLine;
    }

    [StorableConstructor]
    protected GlopSolver(bool deserializing)
      : base(deserializing) {
    }

    protected GlopSolver(GlopSolver original, Cloner cloner)
      : base(original, cloner) {
    }

    protected override Solver.OptimizationProblemType OptimizationProblemType =>
      Solver.OptimizationProblemType.GlopLinearProgramming;

    public override IDeepCloneable Clone(Cloner cloner) => new GlopSolver(this, cloner);
  }
}
