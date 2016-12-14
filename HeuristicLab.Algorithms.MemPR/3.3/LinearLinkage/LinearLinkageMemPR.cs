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
using HeuristicLab.Algorithms.MemPR.Util;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.LinearLinkage {
  [Item("MemPR (linear linkage)", "MemPR implementation for linear linkage vectors.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  public class LinearLinkageMemPR : MemPRAlgorithm<SingleObjectiveBasicProblem<LinearLinkageEncoding>, Encodings.LinearLinkageEncoding.LinearLinkage, LinearLinkageMemPRPopulationContext, LinearLinkageMemPRSolutionContext> {
    private const double UncommonBitSubsetMutationProbabilityMagicConst = 0.05;
    
    [StorableConstructor]
    protected LinearLinkageMemPR(bool deserializing) : base(deserializing) { }
    protected LinearLinkageMemPR(LinearLinkageMemPR original, Cloner cloner) : base(original, cloner) { }
    public LinearLinkageMemPR() {
      foreach (var trainer in ApplicationManager.Manager.GetInstances<ISolutionModelTrainer<LinearLinkageMemPRPopulationContext>>())
        SolutionModelTrainerParameter.ValidValues.Add(trainer);
      
      foreach (var localSearch in ApplicationManager.Manager.GetInstances<ILocalSearch<LinearLinkageMemPRSolutionContext>>()) {
        LocalSearchParameter.ValidValues.Add(localSearch);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLinkageMemPR(this, cloner);
    }

    protected override bool Eq(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> a, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> b) {
      var s1 = a.Solution;
      var s2 = b.Solution;
      if (s1.Length != s2.Length) return false;
      for (var i = 0; i < s1.Length; i++)
        if (s1[i] != s2[i]) return false;
      return true;
    }

    protected override double Dist(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> a, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> b) {
      return HammingSimilarityCalculator.CalculateSimilarity(a.Solution, b.Solution);
    }

    protected override ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> ToScope(Encodings.LinearLinkageEncoding.LinearLinkage code, double fitness = double.NaN) {
      var creator = Problem.SolutionCreator as ILinearLinkageCreator;
      if (creator == null) throw new InvalidOperationException("Can only solve linear linkage encoded problems with MemPR (linear linkage)");
      return new SingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage>(code, creator.LLEParameter.ActualName, fitness, Problem.Evaluator.QualityParameter.ActualName) {
        Parent = Context.Scope
      };
    }

    protected override ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> CalculateSubspace(IEnumerable<Encodings.LinearLinkageEncoding.LinearLinkage> solutions, bool inverse = false) {
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
      return new LinearLinkageSolutionSubspace(subspace);
    }

    protected override int TabuWalk(
        ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> scope,
        int maxEvals, CancellationToken token,
        ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> sub_space = null) {
      var maximization = Context.Problem.Maximization;
      var subspace = sub_space is LinearLinkageSolutionSubspace ? ((LinearLinkageSolutionSubspace)sub_space).Subspace : null;
      var evaluations = 0;
      var quality = scope.Fitness;
      if (double.IsNaN(quality)) {
        Evaluate(scope, token);
        quality = scope.Fitness;
        evaluations++;
        if (evaluations >= maxEvals) return evaluations;
      }
      var bestQuality = quality;
      var currentScope = (ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage>)scope.Clone();
      var current = currentScope.Solution;
      Encodings.LinearLinkageEncoding.LinearLinkage bestOfTheWalk = null;
      var bestOfTheWalkF = double.NaN;

      var tabu = new double[current.Length, current.Length];
      for (var i = 0; i < current.Length; i++) {
        for (var j = i; j < current.Length; j++) {
          tabu[i, j] = tabu[j, i] = maximization ? double.MinValue : double.MaxValue;
        }
        tabu[i, current[i]] = quality;
      }

      // this dictionary holds the last relevant links
      var groupItems = new List<int>();
      var lleb = current.ToBackLinks();
      Move bestOfTheRest = null;
      var bestOfTheRestF = double.NaN;
      var lastAppliedMove = -1;
      for (var iter = 0; iter < int.MaxValue; iter++) {
        // clear the dictionary before a new pass through the array is made
        groupItems.Clear();
        for (var i = 0; i < current.Length; i++) {
          if (subspace != null && !subspace[i]) {
            if (lleb[i] != i)
              groupItems.Remove(lleb[i]);
            groupItems.Add(i);
            continue;
          }

          var next = current[i];

          if (lastAppliedMove == i) {
            if (bestOfTheRest != null) {
              bestOfTheRest.Apply(current);
              bestOfTheRest.ApplyToLLEb(lleb);
              currentScope.Fitness = bestOfTheRestF;
              quality = bestOfTheRestF;
              if (maximization) {
                tabu[i, next] = Math.Max(tabu[i, next], bestOfTheRestF);
                tabu[i, current[i]] = Math.Max(tabu[i, current[i]], bestOfTheRestF);
              } else {
                tabu[i, next] = Math.Min(tabu[i, next], bestOfTheRestF);
                tabu[i, current[i]] = Math.Min(tabu[i, current[i]], bestOfTheRestF);
              }
              if (FitnessComparer.IsBetter(maximization, bestOfTheRestF, bestOfTheWalkF)) {
                bestOfTheWalk = (Encodings.LinearLinkageEncoding.LinearLinkage)current.Clone();
                bestOfTheWalkF = bestOfTheRestF;
              }
              bestOfTheRest = null;
              bestOfTheRestF = double.NaN;
              lastAppliedMove = i;
            } else {
              lastAppliedMove = -1;
            }
            break;
          } else {
            foreach (var move in MoveGenerator.GenerateForItem(i, groupItems, current, lleb)) {
              // we intend to break link i -> next
              var qualityToBreak = tabu[i, next];
              move.Apply(current);
              var qualityToRestore = tabu[i, current[i]]; // current[i] is new next
              Evaluate(currentScope, token);
              evaluations++;
              var moveF = currentScope.Fitness;
              var isNotTabu = FitnessComparer.IsBetter(maximization, moveF, qualityToBreak)
                              && FitnessComparer.IsBetter(maximization, moveF, qualityToRestore);
              var isImprovement = FitnessComparer.IsBetter(maximization, moveF, quality);
              var isAspired = FitnessComparer.IsBetter(maximization, moveF, bestQuality);
              if ((isNotTabu && isImprovement) || isAspired) {
                if (maximization) {
                  tabu[i, next] = Math.Max(tabu[i, next], moveF);
                  tabu[i, current[i]] = Math.Max(tabu[i, current[i]], moveF);
                } else {
                  tabu[i, next] = Math.Min(tabu[i, next], moveF);
                  tabu[i, current[i]] = Math.Min(tabu[i, current[i]], moveF);
                }
                quality = moveF;
                if (isAspired) bestQuality = quality;

                move.ApplyToLLEb(lleb);

                if (FitnessComparer.IsBetter(maximization, moveF, bestOfTheWalkF)) {
                  bestOfTheWalk = (Encodings.LinearLinkageEncoding.LinearLinkage)current.Clone();
                  bestOfTheWalkF = moveF;
                }

                bestOfTheRest = null;
                bestOfTheRestF = double.NaN;
                lastAppliedMove = i;
                break;
              } else {
                if (isNotTabu) {
                  if (FitnessComparer.IsBetter(maximization, moveF, bestOfTheRestF)) {
                    bestOfTheRest = move;
                    bestOfTheRestF = moveF;
                  }
                }
                move.Undo(current);
                currentScope.Fitness = quality;
              }
              if (evaluations >= maxEvals) break;
            }
          }
          if (lleb[i] != i)
            groupItems.Remove(lleb[i]);
          groupItems.Add(i);
          if (evaluations >= maxEvals) break;
          if (token.IsCancellationRequested) break;
        }
        if (lastAppliedMove == -1) { // no move has been applied
          if (bestOfTheRest != null) {
            var i = bestOfTheRest.Item;
            var next = current[i];
            bestOfTheRest.Apply(current);
            currentScope.Fitness = bestOfTheRestF;
            quality = bestOfTheRestF;
            if (maximization) {
              tabu[i, next] = Math.Max(tabu[i, next], bestOfTheRestF);
              tabu[i, current[i]] = Math.Max(tabu[i, current[i]], bestOfTheRestF);
            } else {
              tabu[i, next] = Math.Min(tabu[i, next], bestOfTheRestF);
              tabu[i, current[i]] = Math.Min(tabu[i, current[i]], bestOfTheRestF);
            }
            if (FitnessComparer.IsBetter(maximization, bestOfTheRestF, bestOfTheWalkF)) {
              bestOfTheWalk = (Encodings.LinearLinkageEncoding.LinearLinkage)current.Clone();
              bestOfTheWalkF = bestOfTheRestF;
            }

            bestOfTheRest = null;
            bestOfTheRestF = double.NaN;
          } else break;
        }
        if (evaluations >= maxEvals) break;
        if (token.IsCancellationRequested) break;
      }
      if (bestOfTheWalk != null) {
        scope.Solution = bestOfTheWalk;
        scope.Fitness = bestOfTheWalkF;
      }
      return evaluations;
    }

    protected override ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> Cross(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> p1Scope, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> p2Scope, CancellationToken token) {
      var p1 = p1Scope.Solution;
      var p2 = p2Scope.Solution;
      return ToScope(GroupCrossover.Apply(Context.Random, p1, p2));
    }

    protected override void Mutate(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> offspring, CancellationToken token, ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> subspace = null) {
      var lle = offspring.Solution;
      var subset = subspace is LinearLinkageSolutionSubspace ? ((LinearLinkageSolutionSubspace)subspace).Subspace : null;
      for (var i = 0; i < lle.Length - 1; i++) {
        if (subset == null || subset[i]) continue; // mutation works against crossover so aims to mutate noTouch points
        if (Context.Random.NextDouble() < UncommonBitSubsetMutationProbabilityMagicConst) {
          subset[i] = true;
          var index = Context.Random.Next(i, lle.Length);
          for (var j = index - 1; j >= i; j--) {
            if (lle[j] == index) index = j;
          }
          lle[i] = index;
          index = i;
          var idx2 = i;
          for (var j = i - 1; j >= 0; j--) {
            if (lle[j] == lle[index]) {
              lle[j] = idx2;
              index = idx2 = j;
            } else if (lle[j] == idx2) idx2 = j;
          }
        }
      }
    }

    protected override ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> Relink(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> a, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> b, CancellationToken token) {
      var maximization = Context.Problem.Maximization;
      if (double.IsNaN(a.Fitness)) {
        Evaluate(a, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      if (double.IsNaN(b.Fitness)) {
        Evaluate(b, token);
        Context.IncrementEvaluatedSolutions(1);
      }
      var child = (ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage>)a.Clone();
      var cgroups = child.Solution.GetGroups().Select(x => new HashSet<int>(x)).ToList();
      var g2 = b.Solution.GetGroups().ToList();
      var order = Enumerable.Range(0, g2.Count).Shuffle(Context.Random).ToList();
      ISingleObjectiveSolutionScope <Encodings.LinearLinkageEncoding.LinearLinkage> bestChild = null;
      for (var j = 0; j < g2.Count; j++) {
        var g = g2[order[j]];
        var changed = false;
        for (var k = j; k < cgroups.Count; k++) {
          foreach (var f in g) if (cgroups[k].Remove(f)) changed = true;
          if (cgroups[k].Count == 0) {
            cgroups.RemoveAt(k);
            k--;
          }
        }
        cgroups.Insert(0, new HashSet<int>(g));
        child.Solution.SetGroups(cgroups);
        if (changed) {
          Evaluate(child, token);
          if (bestChild == null || FitnessComparer.IsBetter(maximization, child.Fitness, bestChild.Fitness)) {
            bestChild = (ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage>)child.Clone();
          }
        }
      };
      return bestChild;
    }
  }
}
