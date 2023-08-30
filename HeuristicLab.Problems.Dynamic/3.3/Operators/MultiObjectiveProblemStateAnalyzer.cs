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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Dynamic.Operators {
  [Item("Problem State Analyzer (multi objective)", "Calls the script's Analyze method to be able to write into the results collection.")]
  [StorableType("69EB3EFA-1E3B-4A7A-AB8B-9F59196211ED")]
  public sealed class MultiObjectiveProblemStateAnalyzer : SingleSuccessorOperator, IAnalyzer, IStochasticOperator /*, IMultiObjectiveAnalysisOperator*/ {
    public bool EnabledByDefault => true;
    public ILookupParameter<IEncoding> EncodingParameter => (ILookupParameter<IEncoding>)Parameters["Encoding"];

    public ILookupParameter<ResultCollection> ResultsParameter => (ILookupParameter<ResultCollection>)Parameters["Results"];

    public ILookupParameter<IRandom> RandomParameter => (ILookupParameter<IRandom>)Parameters["Random"];

    public Action<Individual[], double[][], ResultCollection, IRandom> AnalyzeAction { get; set; }

    [Storable] public IMultiObjectiveProblemDefinition Problem { get; set; }


    [StorableConstructor]
    private MultiObjectiveProblemStateAnalyzer(StorableConstructorFlag _) : base(_) { }

    private MultiObjectiveProblemStateAnalyzer(MultiObjectiveProblemStateAnalyzer original, Cloner cloner) : base(original, cloner) {
      Problem = original.Problem;
    }
    public MultiObjectiveProblemStateAnalyzer() {
      Parameters.Add(new LookupParameter<IEncoding>("Encoding", "An item that holds the problem's encoding."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to write to."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProblemStateAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var results = ResultsParameter.ActualValue;
      var random = RandomParameter.ActualValue;
      ((IDynamicProblemDefinition)Problem).AnalyzeProblem(results, random);
      return base.Apply();
    }
  }
}
