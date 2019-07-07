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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Selection {
  [Item("CrowdedTournamentSelector", "Selects solutions using tournament selection by using the partial order defined in Deb et al. 2002. A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II. IEEE Transactions on Evolutionary Computation, 6(2), pp. 182-197.")]
  [StorableType("343E8160-873B-4F76-BCA0-9F031CB6D1D7")]
  public class CrowdedTournamentSelector : Selector, IMultiObjectiveSelector, IStochasticOperator {
    public ILookupParameter<BoolArray> MaximizationParameter {
      get { return (ILookupParameter<BoolArray>)Parameters["Maximization"]; }
    }
    public IValueLookupParameter<IntValue> NumberOfSelectedSubScopesParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NumberOfSelectedSubScopes"]; }
    }
    protected IValueParameter<BoolValue> CopySelectedParameter {
      get { return (IValueParameter<BoolValue>)Parameters["CopySelected"]; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<ItemArray<DoubleArray>> QualitiesParameter {
      get { return (ILookupParameter<ItemArray<DoubleArray>>)Parameters["Qualities"]; }
    }
    public IScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (IScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> CrowdingDistanceParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["CrowdingDistance"]; }
    }
    public IValueLookupParameter<IntValue> GroupSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["GroupSize"]; }
    }

    public BoolValue CopySelected {
      get { return CopySelectedParameter.Value; }
      set { CopySelectedParameter.Value = value; }
    }

    [StorableConstructor]
    protected CrowdedTournamentSelector(StorableConstructorFlag _) : base(_) { }
    protected CrowdedTournamentSelector(CrowdedTournamentSelector original, Cloner cloner) : base(original, cloner) { }
    public CrowdedTournamentSelector()
      : base() {
      Parameters.Add(new LookupParameter<BoolArray>("Maximization", "For each objective determines whether it should be maximized or minimized."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSelectedSubScopes", "The number of sub-scopes that should be selected."));
      Parameters.Add(new ValueParameter<BoolValue>("CopySelected", "True if the selected scopes are to be copied (cloned) otherwise they're moved."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The solutions' qualities vector."));
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The solutions' domination rank."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("CrowdingDistance", "The solutions' crowding distance values."));
      Parameters.Add(new ValueLookupParameter<IntValue>("GroupSize", "The size of the group from which the best will be chosen.", new IntValue(2)));
      CopySelectedParameter.Hidden = true;
    }

    protected override IScope[] Select(List<IScope> scopes) {
      IRandom random = RandomParameter.ActualValue;
      List<int> ranks = RankParameter.ActualValue.Select(x => x.Value).ToList();
      List<double> crowdingDistance = CrowdingDistanceParameter.ActualValue.Select(x => x.Value).ToList();
      int count = NumberOfSelectedSubScopesParameter.ActualValue.Value;
      int groupSize = GroupSizeParameter.ActualValue.Value;
      bool copy = CopySelected.Value;
      IScope[] selected = new IScope[count];

      for (int i = 0; i < count; i++) {
        int best = random.Next(scopes.Count);
        int index;
        for (int j = 1; j < groupSize; j++) {
          index = random.Next(scopes.Count);
          if (ranks[best] > ranks[index]
            || ranks[best] == ranks[index]
              && crowdingDistance[best] < crowdingDistance[index]) {
            best = index;
          }
        }

        if (copy)
          selected[i] = (IScope)scopes[best].Clone();
        else {
          selected[i] = scopes[best];
          scopes.RemoveAt(best);
          ranks.RemoveAt(best);
          crowdingDistance.RemoveAt(best);
        }
      }

      return selected;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdedTournamentSelector(this, cloner);
    }
  }
}
