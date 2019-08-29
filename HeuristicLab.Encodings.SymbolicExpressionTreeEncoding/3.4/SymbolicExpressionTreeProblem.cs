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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("A1B9F4C8-5E29-493C-A483-2AC68453BC63")]
  public abstract class SymbolicExpressionTreeProblem : SingleObjectiveProblem<SymbolicExpressionTreeEncoding, ISymbolicExpressionTree> {

    // persistence
    [StorableConstructor]
    protected SymbolicExpressionTreeProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }


    // cloning
    protected SymbolicExpressionTreeProblem(SymbolicExpressionTreeProblem original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicExpressionTreeProblem(SymbolicExpressionTreeEncoding encoding)
      : base(encoding) {
      EncodingParameter.ReadOnly = true;
    }

    public override void Analyze(ISymbolicExpressionTree[] trees, double[] qualities, ResultCollection results,
      IRandom random) {
      if (!results.ContainsKey("Best Solution Quality")) {
        results.Add(new Result("Best Solution Quality", typeof(DoubleValue)));
      }
      if (!results.ContainsKey("Best Solution")) {
        results.Add(new Result("Best Solution", typeof(ISymbolicExpressionTree)));
      }

      var bestQuality = Maximization ? qualities.Max() : qualities.Min();

      if (results["Best Solution Quality"].Value == null ||
          IsBetter(bestQuality, ((DoubleValue)results["Best Solution Quality"].Value).Value)) {
        var bestIdx = Array.IndexOf(qualities, bestQuality);
        var bestClone = (IItem)trees[bestIdx].Clone();

        results["Best Solution"].Value = bestClone;
        results["Best Solution Quality"].Value = new DoubleValue(bestQuality);
      }
    }
  }
}
