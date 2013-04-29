#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ScatterSearch {
  /// <summary>
  /// An operator that updates the solution pool.
  /// </summary>
  [Item("SolutionPoolUpdateMethod", "An operator that updates the solution pool.")]
  [StorableClass]
  public sealed class SolutionPoolUpdateMethod : SingleSuccessorOperator, ISimilarityBasedOperator {
    #region ISimilarityBasedOperator Members
    [Storable]
    public ISolutionSimilarityCalculator SimilarityCalculator { get; set; }
    #endregion

    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public IValueLookupParameter<BoolValue> NewSolutionsParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["NewSolutions"]; }
    }
    public IValueLookupParameter<IItem> QualityParameter {
      get { return (IValueLookupParameter<IItem>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<IntValue> ReferenceSetSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ReferenceSetSize"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private BoolValue Maximization {
      get { return MaximizationParameter.ActualValue; }
      set { MaximizationParameter.ActualValue = value; }
    }
    private BoolValue NewSolutions {
      get { return NewSolutionsParameter.ActualValue; }
    }
    private IItem Quality {
      get { return QualityParameter.ActualValue; }
    }
    private IntValue ReferenceSetSize {
      get { return ReferenceSetSizeParameter.ActualValue; }
      set { ReferenceSetSizeParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private SolutionPoolUpdateMethod(bool deserializing) : base(deserializing) { }
    private SolutionPoolUpdateMethod(SolutionPoolUpdateMethod original, Cloner cloner)
      : base(original, cloner) {
      this.SimilarityCalculator = cloner.Clone(original.SimilarityCalculator);
    }
    public SolutionPoolUpdateMethod() : base() { Initialize(); }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SolutionPoolUpdateMethod(this, cloner);
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that is the reference set."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("NewSolutions", "True if new solutions have been found, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IItem>("Quality", "This parameter is used for name translation only."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ReferenceSetSize", "The size of the reference set."));
      #endregion
    }

    public override IOperation Apply() {
      ScopeList parents = new ScopeList();
      ScopeList offspring = new ScopeList();

      // split parents and offspring
      foreach (var scope in CurrentScope.SubScopes) {
        parents.AddRange(scope.SubScopes.Take(scope.SubScopes.Count - 1));
        offspring.AddRange(scope.SubScopes.Last().SubScopes);
      }

      CurrentScope.SubScopes.Clear();

      // attention: assumes that parents are distinct
      // distinction might cause a too small reference set (e.g. reference set = {1, 2, 2, 2,..., 2} -> union = {1, 2}

      var orderedParents = Maximization.Value ? parents.OrderByDescending(x => x.Variables[QualityParameter.ActualName].Value) :
                                                parents.OrderBy(x => x.Variables[QualityParameter.ActualName].Value);
      var orderedOffspring = Maximization.Value ? offspring.OrderByDescending(x => x.Variables[QualityParameter.ActualName].Value) :
                                                  offspring.OrderBy(x => x.Variables[QualityParameter.ActualName].Value);

      CurrentScope.SubScopes.AddRange(orderedParents);

      double worstParentQuality = (orderedParents.Last().Variables[QualityParameter.ActualName].Value as DoubleValue).Value;

      var hasBetterQuality = Maximization.Value ? (Func<IScope, bool>)(x => { return (x.Variables[QualityParameter.ActualName].Value as DoubleValue).Value > worstParentQuality; }) :
                                                  (Func<IScope, bool>)(x => { return (x.Variables[QualityParameter.ActualName].Value as DoubleValue).Value < worstParentQuality; });

      // is there any offspring better than the worst parent?
      if (orderedOffspring.Any(hasBetterQuality)) {
        // produce the set union
        var union = orderedParents.Union(orderedOffspring.Where(hasBetterQuality), SimilarityCalculator);
        if (union.Count() > orderedParents.Count()) {
          var orderedUnion = Maximization.Value ? union.OrderByDescending(x => x.Variables[QualityParameter.ActualName].Value) :
                                                  union.OrderBy(x => x.Variables[QualityParameter.ActualName].Value);
          CurrentScope.SubScopes.Replace(orderedUnion.Take(ReferenceSetSize.Value));
          NewSolutions.Value = true;
        }
      }

      return base.Apply();
    }
  }
}
