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

using System;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Encodings.BinaryVectorEncoding.LocalSearch {
  [Item("Exhaustive Bitflip Local Search (binary)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class ExhaustiveBitflip<TProblem, TContext> : NamedItem, IBinaryLocalSearch<TContext>
      where TProblem : class, ISingleObjectiveProblemDefinition<BinaryVectorEncoding, BinaryVector>, ISingleObjectiveProblem<BinaryVectorEncoding, BinaryVector> // need both, because one has Maximization, the other only the IParameter
      where TContext : IProblemContext<TProblem, BinaryVectorEncoding, BinaryVector>,
                       ISingleObjectiveSolutionContext<BinaryVector>, IStochasticContext,
                       IEvaluatedSolutionsContext, ILongRunningOperationContext {

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    protected ExhaustiveBitflip(bool deserializing) : base(deserializing) { }
    protected ExhaustiveBitflip(ExhaustiveBitflip<TProblem, TContext> original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveBitflip() : base() {
      name = ItemName;
      description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveBitflip<TProblem, TContext>(this, cloner);
    }

    public void Optimize(TContext context) {
      var quality = context.Solution.Fitness;
      try {
        var result = Search(context.Random, context.Solution.Solution, ref quality,
          context.Problem.Maximization, context.Problem.Evaluate, context.CancellationToken);
        context.IncEvaluatedSolutions(result.Item1);
        var stepsContext = context as IImprovementStepsContext;
        if (stepsContext != null)
          stepsContext.ImprovementSteps = result.Item2;
      } finally {
        context.Solution.Fitness = quality;
      }
    }

    private static bool IsBetter(bool maximization, double a, double b) {
      return maximization && a > b
        || !maximization && a < b;
    }

    public static Tuple<int, int> Search(IRandom random, BinaryVector solution, ref double quality, bool maximization, Func<BinaryVector, IRandom, double> evalFunc, CancellationToken token, bool[] subspace = null) {
      if (double.IsNaN(quality)) quality = evalFunc(solution, null);
      var improved = false;
      var order = Enumerable.Range(0, solution.Length).Shuffle(random).ToArray();
      var lastImp = -1;
      var steps = 0;
      var evaluations = 0;
      do {
        improved = false;
        for (var i = 0; i < solution.Length; i++) {
          // in case we didn't make an improvement this round and arrived at the index of the last improvement
          // break means we don't need to try the remaining moves again as they have brought no improvement
          if (!improved && lastImp == i) break;
          var idx = order[i];
          if (subspace != null && !subspace[idx]) continue;
          // bitflip the solution
          solution[idx] = !solution[idx];
          var after = evalFunc(solution, null);
          evaluations++;
          if (IsBetter(maximization, after, quality)) {
            steps++;
            quality = after;
            lastImp = i;
            improved = true;
          } else {
            // undo the bitflip in case no improvement was made
            solution[idx] = !solution[idx];
          }
          token.ThrowIfCancellationRequested();
        }
      } while (improved);

      return Tuple.Create(evaluations, steps);
    }
  }
}
