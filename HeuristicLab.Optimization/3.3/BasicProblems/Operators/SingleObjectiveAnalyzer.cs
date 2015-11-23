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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Single-objective Analyzer", "Calls the script's Analyze method to be able to write into the results collection.")]
  [StorableClass]
  public sealed class SingleObjectiveAnalyzer<TSolution> : SingleSuccessorOperator, ISingleObjectiveAnalysisOperator<TSolution>, IAnalyzer, IStochasticOperator
  where TSolution : class, ISolution {
    public bool EnabledByDefault { get { return true; } }

    public ILookupParameter<IEncoding<TSolution>> EncodingParameter {
      get { return (ILookupParameter<IEncoding<TSolution>>)Parameters["Encoding"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public Action<TSolution[], double[], ResultCollection, IRandom> AnalyzeAction { get; set; }

    [StorableConstructor]
    private SingleObjectiveAnalyzer(bool deserializing) : base(deserializing) { }
    private SingleObjectiveAnalyzer(SingleObjectiveAnalyzer<TSolution> original, Cloner cloner) : base(original, cloner) { }
    public SingleObjectiveAnalyzer() {
      Parameters.Add(new LookupParameter<IEncoding<TSolution>>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to write to."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveAnalyzer<TSolution>(this, cloner);
    }

    public override IOperation Apply() {
      var encoding = EncodingParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var random = RandomParameter.ActualValue;

      IEnumerable<IScope> scopes = new[] { ExecutionContext.Scope };
      for (var i = 0; i < QualityParameter.Depth; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));

      var individuals = scopes.Select(s => ScopeUtil.GetSolution(s, encoding)).ToArray();
      AnalyzeAction(individuals, QualityParameter.ActualValue.Select(x => x.Value).ToArray(), results, random);
      return base.Apply();
    }
  }
}
