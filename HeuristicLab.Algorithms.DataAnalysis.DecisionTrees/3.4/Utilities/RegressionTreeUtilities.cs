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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  internal static class RegressionTreeUtilities {
    public static ResultCollection RunSubAlgorithm(IAlgorithm alg, int random, CancellationToken cancellationToken) {
      if (alg.Parameters.ContainsKey("SetSeedRandomly") && alg.Parameters.ContainsKey("Seed")) {
        var seed = alg.Parameters["Seed"].ActualValue as IntValue;
        var setSeed = alg.Parameters["SetSeedRandomly"].ActualValue as BoolValue;
        if (seed == null || setSeed == null)
          throw new ArgumentException("The parameters SetSeedRandomly and Seed do not have the expected type");
        setSeed.Value = false;
        seed.Value = random;
      }
      if (alg.ExecutionState != ExecutionState.Paused) alg.Prepare();
      alg.Start(cancellationToken);
      var res = alg.Results;
      alg.Runs.Clear();
      return res;
    }

    public static void SplitRows(IReadOnlyList<int> rows, IDataset data, string splitAttr, double splitValue, out IReadOnlyList<int> leftRows, out IReadOnlyList<int> rightRows) {
      //TODO check and revert?: points at borders are now used multipe times
      var assignment = data.GetDoubleValues(splitAttr, rows).Select(x => x.IsAlmost(splitValue) ? 2 : x < splitValue ? 0 : 1).ToArray();
      leftRows = rows.Zip(assignment, (i, b) => new {i, b}).Where(x => x.b == 0 || x.b == 2).Select(x => x.i).ToList();
      rightRows = rows.Zip(assignment, (i, b) => new {i, b}).Where(x => x.b > 0).Select(x => x.i).ToList();
    }

    public static IDataset ReduceDataset(IDataset data, IReadOnlyList<int> rows, IReadOnlyList<string> inputVariables, string target) {
      return new Dataset(inputVariables.Concat(new[] {target}), inputVariables.Concat(new[] {target}).Select(x => data.GetDoubleValues(x, rows).ToList()));
    }
  }
}