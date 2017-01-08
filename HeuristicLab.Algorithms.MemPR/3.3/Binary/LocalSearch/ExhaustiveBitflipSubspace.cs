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
using HeuristicLab.Encodings.Binary.LocalSearch;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Binary.LocalSearch {
  [Item("Exhaustive Bitflip Local (Subspace) Search (binary)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class ExhaustiveBitflipSubspace<TContext> : NamedItem, ILocalSearch<TContext>
      where TContext : ISingleSolutionHeuristicAlgorithmContext<SingleObjectiveBasicProblem<BinaryVectorEncoding>, BinaryVector>,
                       IBinaryVectorSubspaceContext, IEvaluationServiceContext<BinaryVector> {

    [StorableConstructor]
    protected ExhaustiveBitflipSubspace(bool deserializing) : base(deserializing) { }
    protected ExhaustiveBitflipSubspace(ExhaustiveBitflipSubspace<TContext> original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveBitflipSubspace() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveBitflipSubspace<TContext>(this, cloner);
    }

    public void Optimize(TContext context) {
      var quality = context.Solution.Fitness;
      try {
        var result = ExhaustiveBitflip.Optimize(context.Random, context.Solution.Solution, ref quality,
          context.Problem.Maximization, context.Evaluate, CancellationToken.None, context.Subspace.Subspace);
        context.IncrementEvaluatedSolutions(result.Item1);
        context.Iterations = result.Item2;
      } finally {
        context.Solution.Fitness = quality;
      }
    }
  }
}
