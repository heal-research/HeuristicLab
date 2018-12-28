#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  [Item("RankBasedParetoFrontAnalyzer", "Uses the rank value that is computed by e.g. the NSGA2's fast non dominated sort operator to collect all solutions and their qualities of front 0 (the current Pareto front).")]
  [StorableType("9F314FB6-6BA2-4CC8-843B-B68CF01F7BE7")]
  public class RankBasedParetoFrontAnalyzer : ParetoFrontAnalyzer {
    public IScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (IScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }

    [StorableConstructor]
    protected RankBasedParetoFrontAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected RankBasedParetoFrontAnalyzer(RankBasedParetoFrontAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public RankBasedParetoFrontAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The rank of solution [0..N] describes to which front it belongs."));
    }

    protected override void Analyze(ItemArray<DoubleArray> qualities, ResultCollection results) {
      ItemArray<IntValue> ranks = RankParameter.ActualValue;

      bool populationLevel = RankParameter.Depth == 1;

      int objectives = qualities[0].Length;
      int frontSize = ranks.Count(x => x.Value == 0);
      ItemArray<IScope> paretoArchive = null;
      if (populationLevel) paretoArchive = new ItemArray<IScope>(frontSize);

      DoubleMatrix front = new DoubleMatrix(frontSize, objectives);
      int counter = 0;
      for (int i = 0; i < ranks.Length; i++) {
        if (ranks[i].Value == 0) {
          for (int k = 0; k < objectives; k++)
            front[counter, k] = qualities[i][k];
          if (populationLevel) {
            paretoArchive[counter] = (IScope)ExecutionContext.Scope.SubScopes[i].Clone();
          }
          counter++;
        }
      }

      front.RowNames = GetRowNames(front);
      front.ColumnNames = GetColumnNames(front);

      if (results.ContainsKey("Pareto Front"))
        results["Pareto Front"].Value = front;
      else results.Add(new Result("Pareto Front", front));

      if (populationLevel) {
        if (results.ContainsKey("Pareto Archive"))
          results["Pareto Archive"].Value = paretoArchive;
        else results.Add(new Result("Pareto Archive", paretoArchive));
      }
    }

    private IEnumerable<string> GetRowNames(DoubleMatrix front) {
      for (int i = 1; i <= front.Rows; i++)
        yield return "Solution " + i.ToString();
    }

    private IEnumerable<string> GetColumnNames(DoubleMatrix front) {
      for (int i = 1; i <= front.Columns; i++)
        yield return "Objective " + i.ToString();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RankBasedParetoFrontAnalyzer(this, cloner);
    }
  }
}
