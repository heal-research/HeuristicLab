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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Grouping {
  [Item("MemPR (linear linkage)", "MemPR implementation for linear linkage vectors.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 999)]
  public class LinearLinkageMemPR : MemPRAlgorithm<SingleObjectiveBasicProblem<LinearLinkageEncoding>, LinearLinkage, LinearLinkageMemPRPopulationContext, LinearLinkageMemPRSolutionContext> {
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

    protected override bool Eq(ISingleObjectiveSolutionScope<LinearLinkage> a, ISingleObjectiveSolutionScope<LinearLinkage> b) {
      var s1 = a.Solution;
      var s2 = b.Solution;
      if (s1.Length != s2.Length) return false;
      for (var i = 0; i < s1.Length; i++)
        if (s1[i] != s2[i]) return false;
      return true;
    }

    protected override double Dist(ISingleObjectiveSolutionScope<LinearLinkage> a, ISingleObjectiveSolutionScope<LinearLinkage> b) {
      return Dist(a.Solution, b.Solution);
    }

    private double Dist(LinearLinkage a, LinearLinkage b) {
      return 1.0 - HammingSimilarityCalculator.CalculateSimilarity(a, b);
    }

    protected override ISingleObjectiveSolutionScope<LinearLinkage> ToScope(LinearLinkage code, double fitness = double.NaN) {
      var creator = Problem.SolutionCreator as ILinearLinkageCreator;
      if (creator == null) throw new InvalidOperationException("Can only solve linear linkage encoded problems with MemPR (linear linkage)");
      return new SingleObjectiveSolutionScope<LinearLinkage>(code, creator.LLEParameter.ActualName, fitness, Problem.Evaluator.QualityParameter.ActualName) {
        Parent = Context.Scope
      };
    }

    protected override ISolutionSubspace<LinearLinkage> CalculateSubspace(IEnumerable<LinearLinkage> solutions, bool inverse = false) {
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

    protected override void AdaptiveWalk(
        ISingleObjectiveSolutionScope<LinearLinkage> scope,
        int maxEvals, CancellationToken token,
        ISolutionSubspace<LinearLinkage> sub_space = null) {
      var maximization = Context.Problem.Maximization;
      var subspace = sub_space is LinearLinkageSolutionSubspace ? ((LinearLinkageSolutionSubspace)sub_space).Subspace : null;
      var evaluations = 0;
      var quality = scope.Fitness;
      if (double.IsNaN(quality)) {
        Evaluate(scope, token);
        quality = scope.Fitness;
        evaluations++;
        if (evaluations >= maxEvals) return;
      }
      var bestQuality = quality;
      var currentScope = (ISingleObjectiveSolutionScope<LinearLinkage>)scope.Clone();
      var current = currentScope.Solution;
      LinearLinkage bestOfTheWalk = null;
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
                bestOfTheWalk = (LinearLinkage)current.Clone();
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
                  bestOfTheWalk = (LinearLinkage)current.Clone();
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
              bestOfTheWalk = (LinearLinkage)current.Clone();
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
    }

    protected override ISingleObjectiveSolutionScope<LinearLinkage> Breed(ISingleObjectiveSolutionScope<LinearLinkage> p1Scope, ISingleObjectiveSolutionScope<LinearLinkage> p2Scope, CancellationToken token) {
      var cache = new HashSet<LinearLinkage>(new LinearLinkageEqualityComparer());
      var cachehits = 0;
      var evaluations = 1;
      ISingleObjectiveSolutionScope<LinearLinkage> offspring = null;
      for (; evaluations < Context.LocalSearchEvaluations; evaluations++) {
        var code = GroupCrossover.Apply(Context.Random, p1Scope.Solution, p2Scope.Solution);
        if (cache.Contains(code)) {
          cachehits++;
          if (cachehits > 10) break;
          continue;
        }
        var probe = ToScope(code);
        Evaluate(probe, token);
        cache.Add(code);
        if (offspring == null || Context.IsBetter(probe, offspring)) {
          offspring = probe;
          if (Context.IsBetter(offspring, p1Scope) && Context.IsBetter(offspring, p2Scope))
            break;
        }
      }
      Context.IncrementEvaluatedSolutions(evaluations-1);
      return offspring;
    }

    protected override ISingleObjectiveSolutionScope<LinearLinkage> Link(ISingleObjectiveSolutionScope<LinearLinkage> a, ISingleObjectiveSolutionScope<LinearLinkage> b, CancellationToken token, bool delink = false) {
      var evaluations = 0;
      if (double.IsNaN(a.Fitness)) {
        Evaluate(a, token);
        evaluations++;
      }
      if (double.IsNaN(b.Fitness)) {
        Evaluate(b, token);
        evaluations++;
      }

      var probe = (ISingleObjectiveSolutionScope<LinearLinkage>)a.Clone();
      ISingleObjectiveSolutionScope<LinearLinkage> best = null;
      while (true) {
        Move bestMove = null;
        var bestMoveQ = double.NaN;
        // this approach may not fully relink the two solutions
        foreach (var m in MoveGenerator.Generate(probe.Solution)) {
          var distBefore = Dist(probe, b);
          m.Apply(probe.Solution);
          var distAfter = Dist(probe, b);
          if (delink && distAfter > distBefore || !delink && distAfter < distBefore) {
            var beforeQ = probe.Fitness;
            Evaluate(probe, token);
            evaluations++;
            var q = probe.Fitness;
            m.Undo(probe.Solution);
            probe.Fitness = beforeQ;

            if (Context.IsBetter(q, bestMoveQ)) {
              bestMove = m;
              bestMoveQ = q;
            }
            if (Context.IsBetter(q, beforeQ)) break;
          } else m.Undo(probe.Solution);
        }
        if (bestMove == null) break;
        bestMove.Apply(probe.Solution);
        probe.Fitness = bestMoveQ;
        if (best == null || Context.IsBetter(probe, best))
          best = (ISingleObjectiveSolutionScope<LinearLinkage>)probe.Clone();
      }
      Context.IncrementEvaluatedSolutions(evaluations);

      return best ?? probe;
    }
  }
}
