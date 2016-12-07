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
      return a.Solution.SequenceEqual(b.Solution);
    }

    protected override double Dist(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> a, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> b) {
      if (a.Solution.Length != b.Solution.Length) throw new ArgumentException("Comparing encodings of unequal length");
      var dist = 0;
      for (var i = 0; i < a.Solution.Length; i++) {
        if (a.Solution[i] != b.Solution[i]) dist++;
      }
      return dist / (double)a.Solution.Length;
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

    protected override int TabuWalk(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> scope, int maxEvals, CancellationToken token, ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> subspace = null) {
      return 0;
    }

    protected override ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> Cross(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> p1Scope, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> p2Scope, CancellationToken token) {
      var p1 = p1Scope.Solution;
      var p2 = p2Scope.Solution;
      var transfered = new bool[p1.Length];
      var subspace = new bool[p1.Length];
      var lleeChild = new int[p1.Length];
      var lleep1 = p1.ToLLEe();
      var lleep2 = p2.ToLLEe();
      for (var i = p1.Length - 1; i >= 0; i--) {
        // Step 1
        subspace[i] = p1[i] != p2[i];
        var p1IsEnd = p1[i] == i;
        var p2IsEnd = p2[i] == i;
        if (p1IsEnd & p2IsEnd) {
          transfered[i] = true;
        } else if (p1IsEnd | p2IsEnd) {
          transfered[i] = Context.Random.NextDouble() < 0.5;
        }
        lleeChild[i] = i;

        // Step 2
        if (transfered[i]) continue;
        var end1 = lleep1[i];
        var end2 = lleep2[i];
        var containsEnd1 = transfered[end1];
        var containsEnd2 = transfered[end2];
        if (containsEnd1 & containsEnd2) {
          if (Context.Random.NextDouble() < 0.5) {
            lleeChild[i] = end1;
          } else {
            lleeChild[i] = end2;
          }
        } else if (containsEnd1) {
          lleeChild[i] = end1;
        } else if (containsEnd2) {
          lleeChild[i] = end2;
        } else {
          if (Context.Random.NextDouble() < 0.5) {
            lleeChild[i] = lleeChild[p1[i]];
          } else {
            lleeChild[i] = lleeChild[p2[i]];
          }
        }
      }
      var child = new Encodings.LinearLinkageEncoding.LinearLinkage(lleeChild.Length);
      child.FromLLEe(lleeChild);
      
      return ToScope(child);
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
