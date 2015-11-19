#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ScatterSearch {
  /// <summary>
  /// An operator that updates the reference set.
  /// </summary>
  [Item("ReferenceSetUpdateMethod", "An operator that updates the reference set.")]
  [StorableClass]
  public sealed class ReferenceSetUpdateMethod : SingleSuccessorOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueLookupParameter<IntValue> ReferenceSetSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ReferenceSetSize"]; }
    }
    public IValueLookupParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IValueLookupParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private IntValue ReferenceSetSize {
      get { return ReferenceSetSizeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private ReferenceSetUpdateMethod(bool deserializing) : base(deserializing) { }
    private ReferenceSetUpdateMethod(ReferenceSetUpdateMethod original, Cloner cloner) : base(original, cloner) { }
    public ReferenceSetUpdateMethod()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the population and the reference set."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ReferenceSetSize", "The size of the reference set."));
      Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ReferenceSetUpdateMethod(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("SimilarityCalculator"))
        Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
      #endregion
    }

    public override IOperation Apply() {
      var populationSimilarity = new Dictionary<IScope, double>();
      var populationScope = CurrentScope.SubScopes[0];
      var refSetScope = CurrentScope.SubScopes[1];
      var similarityMatrix = SimilarityCalculatorParameter.ActualValue.CalculateSolutionCrowdSimilarity(populationScope, refSetScope);
      for (int i = 0; i < populationScope.SubScopes.Count; i++) {
        populationSimilarity[populationScope.SubScopes[i]] = similarityMatrix[i].Sum();
      }
      int numberOfHighQualitySolutions = CurrentScope.SubScopes[1].SubScopes.Count;
      foreach (var entry in populationSimilarity.OrderBy(x => x.Value).Take(ReferenceSetSize.Value - numberOfHighQualitySolutions)) {
        CurrentScope.SubScopes[1].SubScopes.Add(entry.Key);
        CurrentScope.SubScopes[0].SubScopes.Remove(entry.Key);
      }
      return base.Apply();
    }
  }
}
