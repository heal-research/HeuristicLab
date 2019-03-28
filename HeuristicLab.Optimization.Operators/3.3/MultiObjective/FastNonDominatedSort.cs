#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// FastNonDominatedSort as described in: Deb, Pratap, Agrawal and Meyarivan, "A Fast and Elitist Multiobjective
  /// Genetic Algorithm: NSGA-II", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002
  /// </summary>
  [Item("FastNonDominatedSort", @"FastNonDominatedSort as described in: Deb, Pratap, Agrawal and Meyarivan, ""A Fast and Elitist Multiobjective
Genetic Algorithm: NSGA-II"", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002")]
  [StorableType("0D1919E6-FCDE-4216-831A-E6427C3D986E")]
  public class FastNonDominatedSort : SingleSuccessorOperator, IMultiObjectiveOperator {

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
    protected FastNonDominatedSort(StorableConstructorFlag _) : base(_) { }
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

      int[] rank;
      var fronts = DominationCalculator<IScope>.CalculateAllParetoFronts(scope.SubScopes.ToArray(), qualities, maximization, out rank, dominateOnEqualQualities);

      RankParameter.ActualValue = new ItemArray<IntValue>(rank.Select(x => new IntValue(x)));

      scope.SubScopes.Clear();

      for (var i = 0; i < fronts.Count; i++) {
        Scope frontScope = new Scope("Front " + i);
        foreach (var p in fronts[i])
          frontScope.SubScopes.Add(p.Item1);
        if (frontScope.SubScopes.Count > 0)
          scope.SubScopes.Add(frontScope);
      }
      return base.Apply();
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
