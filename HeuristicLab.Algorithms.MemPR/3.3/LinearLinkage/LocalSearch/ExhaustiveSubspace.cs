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
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.LinearLinkage.LocalSearch {
  [Item("Exhaustive Local (Subspace) Search (linear linkage)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class ExhaustiveSubspace<TContext> : NamedItem, ILocalSearch<TContext>
      where TContext : ISingleSolutionHeuristicAlgorithmContext<SingleObjectiveBasicProblem<LinearLinkageEncoding>, Encodings.LinearLinkageEncoding.LinearLinkage>, ILinearLinkageSubspaceContext {

    [StorableConstructor]
    protected ExhaustiveSubspace(bool deserializing) : base(deserializing) { }
    protected ExhaustiveSubspace(ExhaustiveSubspace<TContext> original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveSubspace() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveSubspace<TContext>(this, cloner);
    }

    public void Optimize(TContext context) {
      var evalWrapper = new EvaluationWrapper<Encodings.LinearLinkageEncoding.LinearLinkage>(context.Problem, context.Solution);
      var quality = context.Solution.Fitness;
      try {
        var result = ExhaustiveLocalSearch.Optimize(context.Random, context.Solution.Solution, ref quality,
          context.Problem.Maximization, evalWrapper.Evaluate, CancellationToken.None, context.Subspace.Subspace);
        context.IncrementEvaluatedSolutions(result.Item1);
        context.Iterations = result.Item2;
      } finally {
        context.Solution.Fitness = quality;
      }
    }
  }
}
