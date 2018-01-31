#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("IntegerVectorShakingOperator", "A shaking operator for VNS which uses available manipulation operators to perform the shaking.")]
  [StorableClass]
  public class IntegerVectorShakingOperator : ShakingOperator<IIntegerVectorManipulator>, IIntegerVectorMultiNeighborhoodShakingOperator, IStochasticOperator, IBoundedIntegerVectorOperator {

    public ILookupParameter<IntegerVector> IntegerVectorParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["IntegerVector"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public IValueLookupParameter<IntMatrix> BoundsParameter {
      get { return (IValueLookupParameter<IntMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected IntegerVectorShakingOperator(bool deserializing) : base(deserializing) { }
    protected IntegerVectorShakingOperator(IntegerVectorShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntegerVectorShakingOperator(this, cloner);
    }
    public IntegerVectorShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<IntegerVector>("IntegerVector", "The integer vector to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled."));

      foreach (IIntegerVectorManipulator shaker in ApplicationManager.Manager.GetInstances<IIntegerVectorManipulator>().OrderBy(x => x.Name))
        if (!(shaker is ISelfAdaptiveManipulator)) Operators.Add(shaker);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("Bounds"))
        Parameters.Add(new ValueLookupParameter<IntMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled."));
      #endregion
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IIntegerVectorManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IIntegerVectorManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IIntegerVectorManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IIntegerVectorManipulator op in items.Select(x => x.Value).OfType<IIntegerVectorManipulator>())
          op.IntegerVectorParameter.ActualName = IntegerVectorParameter.Name;
      }
    }
    #endregion
  }
}
