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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.LocalSearch;
using HeuristicLab.Optimization.SolutionModel;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Binary {
  [Item("MemPR (binary)", "MemPR implementation for binary vectors.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  public class BinaryMemPR : MemPRAlgorithm<BinaryVector, BinaryMemPRContext, BinarySingleSolutionMemPRContext> {
    private const double UncommonBitSubsetMutationProbabilityMagicConst = 0.05;
    
    [StorableConstructor]
    protected BinaryMemPR(bool deserializing) : base(deserializing) { }
    protected BinaryMemPR(BinaryMemPR original, Cloner cloner) : base(original, cloner) { }
    public BinaryMemPR() {
      foreach (var trainer in ApplicationManager.Manager.GetInstances<IBinarySolutionModelTrainer<BinaryMemPRContext>>())
        SolutionModelTrainerParameter.ValidValues.Add(trainer);
      
      foreach (var localSearch in ApplicationManager.Manager.GetInstances<IBinaryLocalSearch<BinarySingleSolutionMemPRContext>>()) {
        // only use local search operators that can deal with a restricted solution space
        var lsType = localSearch.GetType();
        var genTypeDef = lsType.GetGenericTypeDefinition();
        // TODO: By convention, context type must be put last
        // TODO: Fails with non-generic types
        if (genTypeDef.GetGenericArguments().Last().GetGenericParameterConstraints().Any(x => typeof(IBinarySolutionSubspaceContext).IsAssignableFrom(x))) {
          localSearch.EvaluateFunc = EvaluateFunc;
          LocalSearchParameter.ValidValues.Add(localSearch);
        }
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryMemPR(this, cloner);
    }

    protected double EvaluateFunc(BinaryVector solution) {
      var scope = ToScope(solution);
      Evaluate(scope, CancellationToken.None);
      return scope.Fitness;
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
      var len = a.Solution.Length;
      var acode = a.Solution;
      var bcode = b.Solution;
      var hamming = 0;
      for (var i = 0; i < len; i++)
        if (acode[i] != bcode[i]) hamming++;
      return hamming / (double)len;
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> ToScope(BinaryVector code, double fitness = double.NaN) {
      var creator = Problem.SolutionCreator as IBinaryVectorCreator;
      if (creator == null) throw new InvalidOperationException("Can only solve binary encoded problems with MemPR (binary)");
      return new SingleObjectiveSolutionScope<BinaryVector>(code, creator.BinaryVectorParameter.ActualName, fitness, Problem.Evaluator.QualityParameter.ActualName) {
        Parent = Context.Scope
      };
    }

    protected override ISolutionSubspace CalculateSubspace(IEnumerable<BinaryVector> solutions, bool inverse = false) {
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

    protected override ISingleObjectiveSolutionScope<BinaryVector> Create(CancellationToken token) {
      var child = ToScope(null);
      RunOperator(Problem.SolutionCreator, child, token);
      return child;
    }

    protected override void TabuWalk(ISingleObjectiveSolutionScope<BinaryVector> scope, int steps, CancellationToken token, ISolutionSubspace subspace = null) {
      var subset = subspace != null ? ((BinarySolutionSubspace)subspace).Subspace : null;
      if (double.IsNaN(scope.Fitness)) Evaluate(scope, token);
      SingleObjectiveSolutionScope<BinaryVector> bestOfTheWalk = null;
      var currentScope = (SingleObjectiveSolutionScope<BinaryVector>)scope.Clone();
      var current = currentScope.Solution;
      var N = current.Length;
      var tabu = new Tuple<double, double>[N];
      for (var i = 0; i < N; i++) tabu[i] = Tuple.Create(current[i] ? double.NaN : currentScope.Fitness, !current[i] ? double.NaN : currentScope.Fitness);
      var subN = subset != null ? subset.Count(x => x) : N;
      if (subN == 0) return;
      var order = Enumerable.Range(0, N).Where(x => subset == null || subset[x]).Shuffle(Context.Random).ToArray();

      for (var iter = 0; iter < steps; iter++) {
        var allTabu = true;
        var bestOfTheRestF = double.NaN;
        int bestOfTheRest = -1;
        var improved = false;

        for (var i = 0; i < subN; i++) {
          var idx = order[i];
          var before = currentScope.Fitness;
          current[idx] = !current[idx];
          Evaluate(currentScope, token);
          var after = currentScope.Fitness;

          if (IsBetter(after, before) && (bestOfTheWalk == null || IsBetter(after, bestOfTheWalk.Fitness))) {
            bestOfTheWalk = (SingleObjectiveSolutionScope<BinaryVector>)currentScope.Clone();
          }

          var qualityToBeat = current[idx] ? tabu[idx].Item2 : tabu[idx].Item1;
          var isTabu = !IsBetter(after, qualityToBeat);
          if (!isTabu) allTabu = false;

          if (IsBetter(after, before) && !isTabu) {
            improved = true;
            tabu[idx] = current[idx] ? Tuple.Create(after, tabu[idx].Item2) : Tuple.Create(tabu[idx].Item1, after);
          } else { // undo the move
            if (!isTabu && IsBetter(after, bestOfTheRestF)) {
              bestOfTheRest = idx;
              bestOfTheRestF = after;
            }
            current[idx] = !current[idx];
            currentScope.Fitness = before;
          }
        }
        if (!allTabu && !improved) {
          var better = currentScope.Fitness;
          current[bestOfTheRest] = !current[bestOfTheRest];
          tabu[bestOfTheRest] = current[bestOfTheRest] ? Tuple.Create(better, tabu[bestOfTheRest].Item2) : Tuple.Create(tabu[bestOfTheRest].Item1, better);
          currentScope.Fitness = bestOfTheRestF;
        } else if (allTabu) break;
      }

      scope.Adopt(bestOfTheWalk ?? currentScope);
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> Cross(ISingleObjectiveSolutionScope<BinaryVector> p1, ISingleObjectiveSolutionScope<BinaryVector> p2, CancellationToken token) {
      var offspring = (ISingleObjectiveSolutionScope<BinaryVector>)p1.Clone();
      offspring.Fitness = double.NaN;
      var code = offspring.Solution;
      var p2Code = p2.Solution;
      var bp = 0;
      var lastbp = 0;
      for (var i = 0; i < code.Length; i++) {
        if (code[i] == p2Code[i]) continue; // common bit
        if (bp % 2 == 1) {
          code[i] = p2Code[i];
        }
        if (Context.Random.Next(code.Length) < i - lastbp) {
          bp = (bp + 1) % 2;
          lastbp = i;
        }
      }
      return offspring;
    }

    protected override void Mutate(ISingleObjectiveSolutionScope<BinaryVector> offspring, CancellationToken token, ISolutionSubspace subspace = null) {
      var subset = subspace != null ? ((BinarySolutionSubspace)subspace).Subspace : null;
      offspring.Fitness = double.NaN;
      var code = offspring.Solution;
      for (var i = 0; i < code.Length; i++) {
        if (subset != null && subset[i]) continue;
        if (Context.Random.NextDouble() < UncommonBitSubsetMutationProbabilityMagicConst) {
          code[i] = !code[i];
          if (subset != null) subset[i] = true;
        }
      }
    }

    protected override ISingleObjectiveSolutionScope<BinaryVector> Relink(ISingleObjectiveSolutionScope<BinaryVector> a, ISingleObjectiveSolutionScope<BinaryVector> b, CancellationToken token) {
      if (double.IsNaN(a.Fitness)) Evaluate(a, token);
      if (double.IsNaN(b.Fitness)) Evaluate(b, token);
      if (Context.Random.NextDouble() < 0.5)
        return IsBetter(a, b) ? Relink(a, b, token, false) : Relink(b, a, token, true);
      else return IsBetter(a, b) ? Relink(b, a, token, true) : Relink(a, b, token, false);
    }

    protected virtual ISingleObjectiveSolutionScope<BinaryVector> Relink(ISingleObjectiveSolutionScope<BinaryVector> betterScope, ISingleObjectiveSolutionScope<BinaryVector> worseScope, CancellationToken token, bool fromWorseToBetter) {
      var childScope = (ISingleObjectiveSolutionScope<BinaryVector>)(fromWorseToBetter ? worseScope : betterScope).Clone();
      var child = childScope.Solution;
      var better = betterScope.Solution;
      var worse = worseScope.Solution;
      ISingleObjectiveSolutionScope<BinaryVector> best = null;
      var cF = fromWorseToBetter ? worseScope.Fitness : betterScope.Fitness;
      var bF = double.NaN;
      var order = Enumerable.Range(0, better.Length).Shuffle(Context.Random).ToArray();
      while (true) {
        var bestS = double.NaN;
        var bestIdx = -1;
        for (var i = 0; i < child.Length; i++) {
          var idx = order[i];
          // either move from worse to better or move from better away from worse
          if (fromWorseToBetter && child[idx] == better[idx] ||
            !fromWorseToBetter && child[idx] != worse[idx]) continue;
          child[idx] = !child[idx]; // move
          Evaluate(childScope, token);
          var s = childScope.Fitness;
          childScope.Fitness = cF;
          child[idx] = !child[idx]; // undo move
          if (IsBetter(s, cF)) {
            bestS = s;
            bestIdx = idx;
            break; // first-improvement
          }
          if (double.IsNaN(bestS) || IsBetter(s, bestS)) {
            // least-degrading
            bestS = s;
            bestIdx = idx;
          }
        }
        if (bestIdx < 0) break;
        child[bestIdx] = !child[bestIdx];
        cF = bestS;
        childScope.Fitness = cF;
        if (IsBetter(cF, bF)) {
          bF = cF;
          best = (ISingleObjectiveSolutionScope<BinaryVector>)childScope.Clone();
        }
      }
      return best ?? childScope;
    }
  }
}
