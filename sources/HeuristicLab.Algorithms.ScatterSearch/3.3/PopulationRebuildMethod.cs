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
  /// An operator that updates the reference set and rebuilds the population.
  /// </summary>
  [Item("PopulationRebuildMethod", "An operator that updates the reference set and rebuilds the population.")]
  [StorableClass]
  public sealed class PopulationRebuildMethod : SingleSuccessorOperator, ISingleObjectiveOperator {
    #region Parameter properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public IValueLookupParameter<IntValue> NumberOfHighQualitySolutionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NumberOfHighQualitySolutions"]; }
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
    private IntValue NumberOfHighQualitySolutions {
      get { return NumberOfHighQualitySolutionsParameter.ActualValue; }
      set { NumberOfHighQualitySolutionsParameter.ActualValue = value; }
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
    private PopulationRebuildMethod(bool deserializing) : base(deserializing) { }
    private PopulationRebuildMethod(PopulationRebuildMethod original, Cloner cloner) : base(original, cloner) { }
    public PopulationRebuildMethod()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the population and the reference set."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfHighQualitySolutions", "The number of high quality solutions in the reference set."));
      Parameters.Add(new ValueLookupParameter<IItem>("Quality", "This parameter is used for name translation only."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ReferenceSetSize", "The size of the reference set."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PopulationRebuildMethod(this, cloner);
    }

    public override IOperation Apply() {
      IScope populationScope = CurrentScope.SubScopes[0];
      IScope referenceSetScope = CurrentScope.SubScopes[1];
      var orderedReferenceSet = Maximization.Value ? referenceSetScope.SubScopes.OrderByDescending(r => r.Variables[QualityParameter.ActualName].Value) :
                                                     referenceSetScope.SubScopes.OrderBy(r => r.Variables[QualityParameter.ActualName].Value);
      referenceSetScope.SubScopes.Replace(orderedReferenceSet.Take(NumberOfHighQualitySolutions.Value).ToList());
      populationScope.SubScopes.Clear();
      return base.Apply();
    }
  }
}
