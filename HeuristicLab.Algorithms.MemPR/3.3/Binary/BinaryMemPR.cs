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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Binary {
  [Item("MemPR (binary)", "MemPR implementation for binary vectors.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  public class BinaryMemPR : MemPRAlgorithm<SingleObjectiveBasicProblem<BinaryVectorEncoding>, BinaryVector, BinaryMemPRPopulationContext, BinaryMemPRSolutionContext> {
    [StorableConstructor]
    protected BinaryMemPR(bool deserializing) : base(deserializing) { }
    protected BinaryMemPR(BinaryMemPR original, Cloner cloner) : base(original, cloner) { }
    public BinaryMemPR() {
      foreach (var trainer in ApplicationManager.Manager.GetInstances<ISolutionModelTrainer<BinaryMemPRPopulationContext>>())
        SolutionModelTrainerParameter.ValidValues.Add(trainer);
      
      foreach (var localSearch in ApplicationManager.Manager.GetInstances<ILocalSearch<BinaryMemPRSolutionContext>>()) {
        LocalSearchParameter.ValidValues.Add(localSearch);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryMemPR(this, cloner);
    }

    protected override bool Eq(BinaryVector a, BinaryVector b) {
      var len = a.Length;
      for (var i = 0; i < len; i++)
        if (a[i] != b[i]) return false;
      return true;
    }

    protected override double Dist(ISingleObjectiveSolutionScope<BinaryVector> a, ISingleObjectiveSolutionScope<BinaryVector> b) {
      return 1.0 - HammingSimilarityCalculator.CalculateSimilarity(a.Solution, b.Solution);
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> ToScope(BinaryVector code, double fitness = double.NaN) {
      var creator = Problem.SolutionCreator as IBinaryVectorCreator;
      if (creator == null) throw new InvalidOperationException("Can only solve binary encoded problems with MemPR (binary)");
      return new SingleObjectiveSolutionScope<BinaryVector>(code, creator.BinaryVectorParameter.ActualName, fitness, Problem.Evaluator.QualityParameter.ActualName) {
        Parent = Context.Scope
      };
    }

    protected override ISolutionSubspace<BinaryVector> CalculateSubspace(IEnumerable<BinaryVector> solutions, bool inverse = false) {
      var pop = solutions.ToList();
      var N = pop[0].Length;
      var subspace = new bool[N];
      for (var i = 0; i < N; i++) {
        var val = pop[0][i];
        if (inverse) subspace[i] = true;
        for (var p = 1; p < pop.Count; p++) {
          if (pop[p][i] != val) subspace[i] = !inverse;
        }
      }
      return new BinarySolutionSubspace(subspace);
    }

    protected override void AdaptiveWalk(ISingleObjectiveSolutionScope<BinaryVector> scope, int maxEvals, CancellationToken token, ISolutionSubspace<BinaryVector> subspace = null) {
      var evaluations = 0;
      var subset = subspace != null ? ((BinarySolutionSubspace)subspace).Subspace : null;
      if (double.IsNaN(scope.Fitness)) {
        Evaluate(scope, token);
        evaluations++;
      }
      SingleObjectiveSolutionScope<BinaryVector> bestOfTheWalk = null;
      var currentScope = (SingleObjectiveSolutionScope<BinaryVector>)scope.Clone();
      var current = currentScope.Solution;
      var N = current.Length;

      var subN = subset != null ? subset.Count(x => x) : N;
      if (subN == 0) return;
      var order = Enumerable.Range(0, N).Where(x => subset == null || subset[x]).Shuffle(Context.Random).ToArray();

      var bound = Problem.Maximization ? Context.Population.Max(x => x.Fitness) : Context.Population.Min(x => x.Fitness);
      var range = Math.Abs(bound - Context.LocalOptimaLevel);
      if (range.IsAlmost(0)) range = Math.Abs(bound * 0.05);
      if (range.IsAlmost(0)) { // because bound = localoptimalevel = 0
        Context.IncrementEvaluatedSolutions(evaluations);
        return;
      }
      
      var temp = -range / Math.Log(1.0 / maxEvals);
      var endtemp = -range / Math.Log(0.1 / maxEvals);
      var annealFactor = Math.Pow(endtemp / temp, 1.0 / maxEvals);
      for (var iter = 0; iter < int.MaxValue; iter++) {
        var moved = false;

        for (var i = 0; i < subN; i++) {
          var idx = order[i];
          var before = currentScope.Fitness;
          current[idx] = !current[idx];
          Evaluate(currentScope, token);
          evaluations++;
          var after = currentScope.Fitness;

          if (Context.IsBetter(after, before) && (bestOfTheWalk == null || Context.IsBetter(after, bestOfTheWalk.Fitness))) {
            bestOfTheWalk = (SingleObjectiveSolutionScope<BinaryVector>)currentScope.Clone();
            if (Context.IsBetter(bestOfTheWalk, scope)) {
              moved = false;
              break;
            }
          }
          var diff = Problem.Maximization ? after - before : before - after;
          if (diff > 0) moved = true;
          else {
            var prob = Math.Exp(diff / temp);
            if (Context.Random.NextDouble() >= prob) {
              // the move is not good enough -> undo the move
              current[idx] = !current[idx];
              currentScope.Fitness = before;
            }
          }
          temp *= annealFactor;
          if (evaluations >= maxEvals) break;
        }
        if (!moved) break;
        if (evaluations >= maxEvals) break;
      }

      Context.IncrementEvaluatedSolutions(evaluations);
      scope.Adopt(bestOfTheWalk ?? currentScope);
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> Breed(ISingleObjectiveSolutionScope<BinaryVector> p1, ISingleObjectiveSolutionScope<BinaryVector> p2, CancellationToken token) {
      var evaluations = 0;
      var N = p1.Solution.Length;
      if (double.IsNaN(p1.Fitness)) {
        Evaluate(p1, token);
        evaluations++;
      }
      if (double.IsNaN(p2.Fitness)) {
        Evaluate(p2, token);
        evaluations++;
      }
      var better = Problem.Maximization ? Math.Max(p1.Fitness, p2.Fitness)
                                        : Math.Min(p1.Fitness, p2.Fitness);


      var cache = new HashSet<BinaryVector>(new BinaryVectorEqualityComparer());
      cache.Add(p1.Solution);
      cache.Add(p2.Solution);

      ISingleObjectiveSolutionScope<BinaryVector> offspring = null;
      var probe = ToScope(new BinaryVector(N));
      // first try all possible combinations of 1-point crossover
      /*var order = Enumerable.Range(1, N - 2).Where(x => p1.Solution[x] != p2.Solution[x]).Shuffle(Context.Random).ToList();
      foreach (var b in order) {
        for (var i = 0; i <= b; i++)
          probe.Solution[i] = p1.Solution[i];
        for (var i = b + 1; i < probe.Solution.Length; i++)
          probe.Solution[i] = p2.Solution[i];

        // only add to cache, because all solutions must be unique
        if (cache.Contains(probe.Solution)) continue;
        cache.Add(probe.Solution);
        Evaluate(probe, token);
        evaluations++;
        if (offspring == null || Context.IsBetter(probe, offspring)) {
          // immediately return upon finding a better offspring than better parent
          if (Context.IsBetter(probe.Fitness, better)) {
            Context.IncrementEvaluatedSolutions(evaluations);
            return probe;
          }
          offspring = (ISingleObjectiveSolutionScope<BinaryVector>)probe.Clone();
        }
      }*/

      var cacheHits = 0;
      // if we got some evaluations left, try uniform crossover
      while (evaluations < Math.Min(Context.LocalSearchEvaluations, N)) {
        probe.Solution = UniformCrossover.Apply(Context.Random, p1.Solution, p2.Solution);
        if (cache.Contains(probe.Solution)) {
          cacheHits++;
          if (cacheHits > 10) break; // variability of uniform crossover is too low -> parents are too similar
          continue;
        } else cache.Add(probe.Solution);
        Evaluate(probe, token);
        evaluations++;
        if (offspring == null || Context.IsBetter(probe, offspring)) {
          // immediately return upon finding a better offspring than better parent
          if (Context.IsBetter(probe.Fitness, better)) {
            Context.IncrementEvaluatedSolutions(evaluations);
            return probe;
          }
          offspring = (ISingleObjectiveSolutionScope<BinaryVector>)probe.Clone();
        }
      }
      Context.IncrementEvaluatedSolutions(evaluations);
      // return best offspring found
      return offspring ?? probe;
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> Link(ISingleObjectiveSolutionScope<BinaryVector> a, ISingleObjectiveSolutionScope<BinaryVector> b, CancellationToken token, bool delink = false) {
      var evaluations = 0;
      if (double.IsNaN(a.Fitness)) {
        Evaluate(a, token);
        evaluations++;
      }
      if (double.IsNaN(b.Fitness)) {
        Evaluate(b, token);
        evaluations++;
      }

      var childScope = (ISingleObjectiveSolutionScope<BinaryVector>)a.Clone();
      var child = childScope.Solution;
      ISingleObjectiveSolutionScope<BinaryVector> best = null;
      var cF = a.Fitness;
      var bF = double.NaN;
      var order = Enumerable.Range(0, child.Length)
        .Where(x => !delink && child[x] != b.Solution[x] || delink && child[x] == b.Solution[x])
        .Shuffle(Context.Random).ToList();
      if (order.Count == 0) return childScope;

      while (true) {
        var bestS = double.NaN;
        var bestI = -1;
        for (var i = 0; i < order.Count; i++) {
          var idx = order[i];
          child[idx] = !child[idx]; // move
          Evaluate(childScope, token);
          evaluations++;
          var s = childScope.Fitness;
          childScope.Fitness = cF;
          child[idx] = !child[idx]; // undo move
          if (Context.IsBetter(s, cF)) {
            bestS = s;
            bestI = i;
            break; // first-improvement
          }
          if (Context.IsBetter(s, bestS)) {
            // least-degrading
            bestS = s;
            bestI = i;
          }
        }
        child[order[bestI]] = !child[order[bestI]];
        order.RemoveAt(bestI);
        cF = bestS;
        childScope.Fitness = cF;
        if (Context.IsBetter(cF, bF)) {
          bF = cF;
          best = (ISingleObjectiveSolutionScope<BinaryVector>)childScope.Clone();
        }
        if (order.Count == 0) break;
      }
      Context.IncrementEvaluatedSolutions(evaluations);
      return best ?? childScope;
    }
  }
}
