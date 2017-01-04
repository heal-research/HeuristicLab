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

    protected override bool Eq(ISingleObjectiveSolutionScope<BinaryVector> a, ISingleObjectiveSolutionScope<BinaryVector> b) {
      var len = a.Solution.Length;
      var acode = a.Solution;
      var bcode = b.Solution;
      for (var i = 0; i < len; i++)
        if (acode[i] != bcode[i]) return false;
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

      var max = Context.Population.Max(x => x.Fitness);
      var min = Context.Population.Min(x => x.Fitness);
      var range = (max - min);
      if (range.IsAlmost(0)) range = Math.Abs(max * 0.05);
      //else range += range * 0.05;
      if (range.IsAlmost(0)) { // because min = max = 0
        Context.IncrementEvaluatedSolutions(evaluations);
        return;
      }

      //var upperBound = Problem.Maximization ? min - range : max + range;
      //var lowerBound = Problem.Maximization ? max : min;
      var temp = 1.0;
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
          }
          var diff = Problem.Maximization ? after - before : before - after;
          if (diff > 0) moved = true;
          else {
            var prob = Math.Exp(temp * diff / range);
            if (Context.Random.NextDouble() >= prob) {
              // the move is not good enough -> undo the move
              current[idx] = !current[idx];
              currentScope.Fitness = before;
            }
          }
          temp += 100.0 / maxEvals;
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

      var offspring = ToScope(null);
      var probe = ToScope(new BinaryVector(N));
      var order = Enumerable.Range(0, N - 1).Shuffle(Context.Random).ToList();
      for (var x = 0; x < N - 1; x++) {
        var b = order[x];
        if (p1.Solution[b] == p2.Solution[b]) continue;
        for (var i = 0; i <= b; i++)
          probe.Solution[i] = p1.Solution[i];
        for (var i = b + 1; i < probe.Solution.Length; i++)
          probe.Solution[i] = p2.Solution[i];

        Evaluate(probe, token);
        evaluations++;
        if (Context.IsBetter(probe, offspring)) {
          if (Context.IsBetter(probe.Fitness, better)) {
            Context.IncrementEvaluatedSolutions(evaluations);
            return probe;
          }
          offspring = (ISingleObjectiveSolutionScope<BinaryVector>)probe.Clone();
        }
      }

      while (evaluations < Context.LocalSearchEvaluations) {
        probe.Solution = UniformCrossover.Apply(Context.Random, p1.Solution, p2.Solution);
        Evaluate(probe, token);
        evaluations++;
        if (Context.IsBetter(probe, offspring)) {
          if (Context.IsBetter(probe.Fitness, better)) {
            Context.IncrementEvaluatedSolutions(evaluations);
            return probe;
          }
          offspring = (ISingleObjectiveSolutionScope<BinaryVector>)probe.Clone();
        }
      }
      Context.IncrementEvaluatedSolutions(evaluations);
      return offspring;
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
