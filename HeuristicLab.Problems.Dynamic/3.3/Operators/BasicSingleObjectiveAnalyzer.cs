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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;
using System.Runtime.InteropServices;

namespace HeuristicLab.Optimization {
  [Item("Single-objective basic Analyzer", "Calls the script's Analyze method to be able to write into the results collection.")]
  [StorableType("9AF5F2F5-EE28-4581-92C5-1AFB0B46D996")]
  public abstract class BasicSingleObjectiveAnalyzer : SingleSuccessorOperator/*, ISingleObjectiveAnalysisOperator*/, IAnalyzer, IStochasticOperator {
    public bool EnabledByDefault => true;

    public ILookupParameter<IEncoding> EncodingParameter => (ILookupParameter<IEncoding>)Parameters["Encoding"];

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter => (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"];

    public ILookupParameter<ResultCollection> ResultsParameter => (ILookupParameter<ResultCollection>)Parameters["Results"];

    public ILookupParameter<IRandom> RandomParameter => (ILookupParameter<IRandom>)Parameters["Random"];

    public abstract void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random);

    [Storable]
    public ISingleObjectiveProblemDefinition Problem { get; set; } 


    [StorableConstructor]
    protected BasicSingleObjectiveAnalyzer(StorableConstructorFlag _) : base(_) { }

    protected BasicSingleObjectiveAnalyzer(BasicSingleObjectiveAnalyzer original, Cloner cloner) : base(original, cloner) {
      Problem = original.Problem;
    }
    public BasicSingleObjectiveAnalyzer() {
      Parameters.Add(new LookupParameter<IEncoding>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The quality of the parameter vector."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to write to."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IOperation Apply() {
      var encoding = EncodingParameter.ActualValue;
      var results = ResultsParameter.ActualValue;
      var random = RandomParameter.ActualValue;

      IEnumerable<IScope> scopes = new[] { ExecutionContext.Scope };
      for (var i = 0; i < QualityParameter.Depth; i++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));

      var individuals = scopes.Select(encoding.GetIndividual).ToArray();
      Analyze(individuals, QualityParameter.ActualValue.Select(x => x.Value).ToArray(), results, random);
      return base.Apply();
    }
  }
}
