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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Permutation.LocalSearch {
  [Item("Exhaustive Hillclimber (permutation)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class ExhaustiveHillClimb<TContext> : NamedItem, ILocalSearch<TContext>
      where TContext : ISingleSolutionHeuristicAlgorithmContext<SingleObjectiveBasicProblem<PermutationEncoding>, Encodings.PermutationEncoding.Permutation> {

    [StorableConstructor]
    protected ExhaustiveHillClimb(bool deserializing) : base(deserializing) { }
    protected ExhaustiveHillClimb(ExhaustiveHillClimb<TContext> original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveHillClimb() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveHillClimb<TContext>(this, cloner);
    }

    public void Optimize(TContext context) {
      var evalWrapper = new EvaluationWrapper<Encodings.PermutationEncoding.Permutation>(context.Problem, context.Solution);
      var quality = context.Solution.Fitness;
      try {
        var result = Exhaustive.HillClimb(context.Random, context.Solution.Solution, ref quality,
          context.Problem.Maximization, evalWrapper.Evaluate, CancellationToken.None);
        context.IncrementEvaluatedSolutions(result.Item1);
        context.Iterations = result.Item2;
      } finally {
        context.Solution.Fitness = quality;
      }
    }
  }
}
