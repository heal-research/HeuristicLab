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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Selection;

namespace HeuristicLab.Optimization.Operators {
  [StorableType("CC2A5B04-361C-451B-87A4-524895748E79")]
  public class RankAndCrowdingSorter : AlgorithmOperator, IMultiObjectiveOperator {
    #region Parameter properties
    public ValueLookupParameter<BoolArray> MaximizationParameter {
      get { return (ValueLookupParameter<BoolArray>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleArray> QualitiesParameter {
      get { return (ScopeTreeLookupParameter<DoubleArray>)Parameters["Qualities"]; }
    }
    public ScopeTreeLookupParameter<IntValue> RankParameter {
      get { return (ScopeTreeLookupParameter<IntValue>)Parameters["Rank"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> CrowdingDistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["CrowdingDistance"]; }
    }
    public IValueLookupParameter<BoolValue> DominateOnEqualQualitiesParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["DominateOnEqualQualities"]; }
    }
    #endregion

    [StorableConstructor]
    protected RankAndCrowdingSorter(StorableConstructorFlag _) : base(_) { }
    protected RankAndCrowdingSorter(RankAndCrowdingSorter original, Cloner cloner) : base(original, cloner) { }
    public RankAndCrowdingSorter()
      : base() {
      Parameters.Add(new ValueLookupParameter<BoolArray>("Maximization", "For each objective a value that is true if that objective should be maximized, or false if it should be minimized."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("DominateOnEqualQualities", "Flag which determines wether solutions with equal quality values should be treated as dominated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleArray>("Qualities", "The vector of quality values."));
      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("Rank", "The rank of a solution (to which front it belongs)."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("CrowdingDistance", "The crowding distance of a solution in a population."));

      FastNonDominatedSort fastNonDominatedSort = new FastNonDominatedSort();
      UniformSubScopesProcessor subScopesProcessor = new UniformSubScopesProcessor();
      CrowdingDistanceAssignment crowdingDistanceAssignment = new CrowdingDistanceAssignment();
      CrowdedComparisonSorter crowdedComparisonSorter = new CrowdedComparisonSorter();
      MergingReducer mergingReducer = new MergingReducer();

      fastNonDominatedSort.MaximizationParameter.ActualName = MaximizationParameter.Name;
      fastNonDominatedSort.DominateOnEqualQualitiesParameter.ActualName = DominateOnEqualQualitiesParameter.Name;
      fastNonDominatedSort.QualitiesParameter.ActualName = QualitiesParameter.Name;
      fastNonDominatedSort.RankParameter.ActualName = RankParameter.Name;

      crowdingDistanceAssignment.CrowdingDistanceParameter.ActualName = CrowdingDistanceParameter.Name;
      crowdingDistanceAssignment.QualitiesParameter.ActualName = QualitiesParameter.Name;

      crowdedComparisonSorter.CrowdingDistanceParameter.ActualName = CrowdingDistanceParameter.Name;
      crowdedComparisonSorter.RankParameter.ActualName = RankParameter.Name;

      OperatorGraph.InitialOperator = fastNonDominatedSort;
      fastNonDominatedSort.Successor = subScopesProcessor;
      subScopesProcessor.Operator = crowdingDistanceAssignment;
      crowdingDistanceAssignment.Successor = crowdedComparisonSorter;
      crowdedComparisonSorter.Successor = null;
      subScopesProcessor.Successor = mergingReducer;
      mergingReducer.Successor = null;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RankAndCrowdingSorter(this, cloner);
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
