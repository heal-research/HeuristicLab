#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Algorithms.SingleObjective;
using HeuristicLab.Optimization.LocalSearch;
using HeuristicLab.Optimization.Manipulation;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.LocalSearch {
  [Item("Iterated Local Search (ILS)", "Performs a repeated local search by applying a kick to each previous local optimum.")]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 999)]
  [StorableClass]
  public class IteratedLocalSearch<TProblem, TEncoding, TSolution> : HeuristicAlgorithm<LocalSearchContext<TProblem, TEncoding, TSolution>, TProblem, TEncoding, TSolution>
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {

    public IConstrainedValueParameter<ILocalSearch<LocalSearchContext<TProblem, TEncoding, TSolution>>> LocalSearchParameter {
      get { return (IConstrainedValueParameter<ILocalSearch<LocalSearchContext<TProblem, TEncoding, TSolution>>>)Parameters["LocalSearch"]; }
    }
    public IConstrainedValueParameter<IManipulator<LocalSearchContext<TProblem, TEncoding, TSolution>>> KickerParameter {
      get { return (IConstrainedValueParameter<IManipulator<LocalSearchContext<TProblem, TEncoding, TSolution>>>)Parameters["Kicker"]; }
    }

    [Storable]
    private BestAverageWorstQualityAnalyzer qualityAnalyzer;

    [StorableConstructor]
    protected IteratedLocalSearch(bool deserializing) : base(deserializing) { }
    protected IteratedLocalSearch(IteratedLocalSearch<TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      qualityAnalyzer = cloner.Clone(original.qualityAnalyzer);
    }
    public IteratedLocalSearch() {
      ProblemAnalyzer = new MultiAnalyzer();
      AlgorithmAnalyzer = new MultiAnalyzer();
      qualityAnalyzer = new BestAverageWorstQualityAnalyzer();
      AlgorithmAnalyzer.Operators.Add(qualityAnalyzer, true);

      Parameters.Add(new ConstrainedValueParameter<ILocalSearch<LocalSearchContext<TProblem, TEncoding, TSolution>>>("LocalSearch", "The local search operator to use."));
      Parameters.Add(new ConstrainedValueParameter<IManipulator<LocalSearchContext<TProblem, TEncoding, TSolution>>>("Kicker", "The manipulator operator that performs the kick."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IteratedLocalSearch<TProblem, TEncoding, TSolution>(this, cloner);
    }

    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      qualityAnalyzer.QualityParameter.ActualName = Problem.Evaluator.QualityParameter.ActualName;
      qualityAnalyzer.QualityParameter.Depth = 1;
      qualityAnalyzer.QualityParameter.Hidden = true;
    }

    protected override void PerformInitialize(CancellationToken token) {
      Context.Solution = CreateEmptySolutionScope();
      RunOperator(Problem.Encoding.SolutionCreator, Context.Solution, token);
      Evaluate(Context.Solution, token);
      DoLocalSearch();
    }

    protected override void PerformIterate(CancellationToken token) {
      Context.Iterations++;
      KickerParameter.Value.Manipulate(Context);
      DoLocalSearch();
    }

    private void DoLocalSearch() {
      LocalSearchParameter.Value.Optimize(Context);
    }

    protected override void PerformAnalyze(CancellationToken token) {
      base.PerformAnalyze(token);
      IResult res;
      if (!Results.TryGetValue("Iterations", out res)) {
        res = new Result("Iterations", new IntValue(Context.Iterations));
        Results.Add(res);
      }
      ((IntValue)res.Value).Value = Context.Iterations;
    }
  }
}
