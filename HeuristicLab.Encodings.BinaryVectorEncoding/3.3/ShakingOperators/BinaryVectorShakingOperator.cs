#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  /// <summary>
  /// A shaking operator for VNS.
  /// </summary>
  [Item("BinaryVectorShakingOperator", "A shaking operator for VNS which uses available manipulation operators to perform the shaking.")]
  [StorableClass]
  public class BinaryVectorShakingOperator : ShakingOperator<IBinaryVectorManipulator>, IBinaryVectorMultiNeighborhoodShakingOperator, IStochasticOperator {

    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected BinaryVectorShakingOperator(bool deserializing) : base(deserializing) { }
    protected BinaryVectorShakingOperator(BinaryVectorShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryVectorShakingOperator(this, cloner);
    }
    public BinaryVectorShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The binary vector to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      foreach (IBinaryVectorManipulator shaker in ApplicationManager.Manager.GetInstances<IBinaryVectorManipulator>().OrderBy(x => x.Name))
        Operators.Add(shaker);
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IBinaryVectorManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IBinaryVectorManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<IBinaryVectorManipulator>> items) {
      if (items.Any()) {
        foreach (IStochasticOperator op in items.Select(x => x.Value).OfType<IStochasticOperator>())
          op.RandomParameter.ActualName = RandomParameter.Name;
        foreach (IBinaryVectorManipulator op in items.Select(x => x.Value).OfType<IBinaryVectorManipulator>())
          op.BinaryVectorParameter.ActualName = BinaryVectorParameter.Name;
      }
    }
    #endregion
  }
}
