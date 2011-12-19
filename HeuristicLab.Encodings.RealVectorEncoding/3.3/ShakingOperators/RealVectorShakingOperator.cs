#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("RealVectorShakingOperator", "A shaking operator for VNS which uses available manipulation operators to perform the shaking.")]
  [StorableClass]
  public class RealVectorShakingOperator : ShakingOperator<IRealVectorManipulator>, IRealVectorMultiNeighborhoodShakingOperator, IStochasticOperator {

    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected RealVectorShakingOperator(bool deserializing) : base(deserializing) { }
    protected RealVectorShakingOperator(RealVectorShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorShakingOperator(this, cloner);
    }
    public RealVectorShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The real vector to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      foreach (IRealVectorManipulator shaker in ApplicationManager.Manager.GetInstances<IRealVectorManipulator>().OrderBy(x => x.Name))
        if (!(shaker is MultiRealVectorManipulator)
          && !(shaker is ISelfAdaptiveManipulator)) Operators.Add(shaker);
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IRealVectorManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IRealVectorManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IRealVectorManipulator op in items.Select(x => x.Value).OfType<IRealVectorManipulator>())
          op.RealVectorParameter.ActualName = RealVectorParameter.Name;
      }
    }
    #endregion
  }
}
