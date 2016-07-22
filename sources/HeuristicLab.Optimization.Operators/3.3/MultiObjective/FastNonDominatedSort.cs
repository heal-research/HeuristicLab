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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// FastNonDominatedSort as described in: Deb, Pratap, Agrawal and Meyarivan, "A Fast and Elitist Multiobjective
  /// Genetic Algorithm: NSGA-II", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002
  /// </summary>
  [Item("FastNonDominatedSort", @"FastNonDominatedSort as described in: Deb, Pratap, Agrawal and Meyarivan, ""A Fast and Elitist Multiobjective
Genetic Algorithm: NSGA-II"", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002")]
  [StorableClass]
  public class FastNonDominatedSort : SingleSuccessorOperator, IMultiObjectiveOperator {
    private enum DominationResult { Dominates, IsDominated, IsNonDominated };

    #region Parameter properties
    public IValueLookupParameter<BoolArray> MaximizationParameter {
      get { return (IValueLookupParameter<BoolArray>)Parameters["Maximization"]; }
    }
    public IValueLookupParameter<BoolValue> DominateOnEqualQualitiesParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["DominateOnEqualQualities"]; }
    }
    public IScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (IScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }
    public IScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (IScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }
    #endregion

    [StorableConstructor]
    protected FastNonDominatedSort(bool deserializing) : base(deserializing) { }
    protected FastNonDominatedSort(FastNonDominatedSort original, Cloner cloner) : base(original, cloner) { }
    public FastNonDominatedSort() {
      Parameters.Add(new ValueLookupParameter<BoolArray>("Maximization", "Whether each objective is maximization or minimization."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("DominateOnEqualQualities", "Flag which determines wether solutions with equal quality values should be treated as dominated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The qualities of a solution.", 1));
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The rank of a solution.", 1));
    }

    public override IOperation Apply() {
      bool dominateOnEqualQualities = DominateOnEqualQualitiesParameter.ActualValue.Value;
      bool[] maximization = MaximizationParameter.ActualValue.ToArray();
      double[][] qualities = QualitiesParameter.ActualValue.Select(x => x.ToArray()).ToArray();
      if (qualities == null) throw new InvalidOperationException(Name + ": No qualities found.");

      IScope scope = ExecutionContext.Scope;
      int populationSize = scope.SubScopes.Count;

      List<ScopeList> fronts = new List<ScopeList>();
      Dictionary<IScope, List<int>> dominatedScopes = new Dictionary<IScope, List<int>>();
      int[] dominationCounter = new int[populationSize];
      ItemArray<IntValue> rank = new ItemArray<IntValue>(populationSize);

      for (int pI = 0; pI < populationSize - 1; pI++) {
        IScope p = scope.SubScopes[pI];
        List<int> dominatedScopesByp;
        if (!dominatedScopes.TryGetValue(p, out dominatedScopesByp))
          dominatedScopes[p] = dominatedScopesByp = new List<int>();
        for (int qI = pI + 1; qI < populationSize; qI++) {
          DominationResult test = Dominates(qualities[pI], qualities[qI], maximization, dominateOnEqualQualities);
          if (test == DominationResult.Dominates) {
            dominatedScopesByp.Add(qI);
            dominationCounter[qI] += 1;
          } else if (test == DominationResult.IsDominated) {
            dominationCounter[pI] += 1;
            if (!dominatedScopes.ContainsKey(scope.SubScopes[qI]))
              dominatedScopes.Add(scope.SubScopes[qI], new List<int>());
            dominatedScopes[scope.SubScopes[qI]].Add(pI);
          }
          if (pI == populationSize - 2
            && qI == populationSize - 1
            && dominationCounter[qI] == 0) {
            rank[qI] = new IntValue(0);
            AddToFront(scope.SubScopes[qI], fronts, 0);
          }
        }
        if (dominationCounter[pI] == 0) {
          rank[pI] = new IntValue(0);
          AddToFront(p, fronts, 0);
        }
      }
      int i = 0;
      while (i < fronts.Count && fronts[i].Count > 0) {
        ScopeList nextFront = new ScopeList();
        foreach (IScope p in fronts[i]) {
          List<int> dominatedScopesByp;
          if (dominatedScopes.TryGetValue(p, out dominatedScopesByp)) {
            for (int k = 0; k < dominatedScopesByp.Count; k++) {
              int dominatedScope = dominatedScopesByp[k];
              dominationCounter[dominatedScope] -= 1;
              if (dominationCounter[dominatedScope] == 0) {
                rank[dominatedScope] = new IntValue(i + 1);
                nextFront.Add(scope.SubScopes[dominatedScope]);
              }
            }
          }
        }
        i += 1;
        fronts.Add(nextFront);
      }

      RankParameter.ActualValue = rank;

      scope.SubScopes.Clear();

      for (i = 0; i < fronts.Count; i++) {
        Scope frontScope = new Scope("Front " + i);
        foreach (var p in fronts[i])
          frontScope.SubScopes.Add(p);
        if (frontScope.SubScopes.Count > 0)
          scope.SubScopes.Add(frontScope);
      }
      return base.Apply();
    }

    private static DominationResult Dominates(double[] left, double[] right, bool[] maximizations, bool dominateOnEqualQualities) {
      //mkommend Caution: do not use LINQ.SequenceEqual for comparing the two quality arrays (left and right) due to performance reasons
      if (dominateOnEqualQualities) {
        var equal = true;
        for (int i = 0; i < left.Length; i++) {
          if (left[i] != right[i]) {
            equal = false;
            break;
          }
        }
        if (equal) return DominationResult.Dominates;
      }

      bool leftIsBetter = false, rightIsBetter = false;
      for (int i = 0; i < left.Length; i++) {
        if (IsDominated(left[i], right[i], maximizations[i])) rightIsBetter = true;
        else if (IsDominated(right[i], left[i], maximizations[i])) leftIsBetter = true;
        if (leftIsBetter && rightIsBetter) break;
      }

      if (leftIsBetter && !rightIsBetter) return DominationResult.Dominates;
      if (!leftIsBetter && rightIsBetter) return DominationResult.IsDominated;
      return DominationResult.IsNonDominated;
    }

    private static bool IsDominated(double left, double right, bool maximization) {
      return maximization && left < right
        || !maximization && left > right;
    }

    private static void AddToFront(IScope p, List<ScopeList> fronts, int i) {
      if (i == fronts.Count) fronts.Add(new ScopeList());
      fronts[i].Add(p);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FastNonDominatedSort(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("DominateOnEqualQualities"))
        Parameters.Add(new ValueLookupParameter<BoolValue>("DominateOnEqualQualities", "Flag which determines wether solutions with equal quality values should be treated as dominated."));
      #endregion
    }
  }
}
