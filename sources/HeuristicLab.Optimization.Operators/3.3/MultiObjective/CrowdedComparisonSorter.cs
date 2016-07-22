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
  /// CrowdedComparisonSorter as described in: Deb, Pratap, Agrawal and Meyarivan, "A Fast and Elitist Multiobjective
  /// Genetic Algorithm: NSGA-II", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002
  /// </summary>
  [Item("CrowdedComparisonSorter", @"CrowdedComparisonSorter as described in: Deb, Pratap, Agrawal and Meyarivan, ""A Fast and Elitist Multiobjective
Genetic Algorithm: NSGA-II"", IEEE Transactions On Evolutionary Computation, Vol. 6, No. 2, April 2002.")]
  [StorableClass]
  public class CrowdedComparisonSorter : SingleSuccessorOperator {

    public IScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (IScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> CrowdingDistanceParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["CrowdingDistance"]; }
    }

    [StorableConstructor]
    protected CrowdedComparisonSorter(bool deserializing) : base(deserializing) { }
    protected CrowdedComparisonSorter(CrowdedComparisonSorter original, Cloner cloner) : base(original, cloner) { }
    public CrowdedComparisonSorter() {
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The rank of the solution."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("CrowdingDistance", "The crowding distance of the solution."));
    }

    public override IOperation Apply() {
      ItemArray<IntValue> ranks = RankParameter.ActualValue;
      ItemArray<DoubleValue> distances = CrowdingDistanceParameter.ActualValue;
      int size = ranks.Length;
      int[] indices = Enumerable.Range(0, size).ToArray();

      IScope[] scopes = ExecutionContext.Scope.SubScopes.ToArray();
      Array.Sort(indices, scopes, new CustomComparer(ranks, distances));
      ExecutionContext.Scope.SubScopes.Clear();
      ExecutionContext.Scope.SubScopes.AddRange(scopes);
      return base.Apply();
    }

    private class CustomComparer : IComparer<int> {
      ItemArray<IntValue> ranks;
      ItemArray<DoubleValue> distances;

      public CustomComparer(ItemArray<IntValue> ranks, ItemArray<DoubleValue> distances) {
        this.ranks = ranks;
        this.distances = distances;
      }

      #region IComparer<int> Members

      public int Compare(int x, int y) {
        if (ranks[x].Value < ranks[y].Value) return -1;
        else if (ranks[x].Value > ranks[y].Value) return 1;
        else { // ranks are the same -> compare by distance
          if (distances[x].Value > distances[y].Value) return -1;
          else if (distances[x].Value < distances[y].Value) return 1;
          else return 0; // same distance
        }
      }

      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdedComparisonSorter(this, cloner);
    }
  }
}
