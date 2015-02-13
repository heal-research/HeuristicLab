#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Common;

namespace HeuristicLab.Algorithms.NSGA2 {
  [Item("BasicMultiObjectiveSolutionAnalyzer", "Basic analyzer for multiobjective problems that collects and presents the current pareto front as double matrix.")]
  [StorableClass]
  public class BasicMultiObjectiveQualityAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public IScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (IScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }
    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    [StorableConstructor]
    protected BasicMultiObjectiveQualityAnalyzer(bool deserializing) : base(deserializing) { }
    protected BasicMultiObjectiveQualityAnalyzer(BasicMultiObjectiveQualityAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BasicMultiObjectiveQualityAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The rank of solution describes to which front it belongs."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of qualities of each solution."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The result collection to store the front to."));
    }

    public override IOperation Apply() {
      ItemArray<IntValue> ranks = RankParameter.ActualValue;
      ItemArray<DoubleArray> qualities = QualitiesParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      
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

      if (results.ContainsKey("Pareto Front"))
        results["Pareto Front"].Value = front;
      else results.Add(new Result("Pareto Front", front));

      if (populationLevel) {
        if (results.ContainsKey("Pareto Archive"))
          results["Pareto Archive"].Value = paretoArchive;
        else results.Add(new Result("Pareto Archive", paretoArchive));
      }
      return base.Apply();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BasicMultiObjectiveQualityAnalyzer(this, cloner);
    }
  }
}
