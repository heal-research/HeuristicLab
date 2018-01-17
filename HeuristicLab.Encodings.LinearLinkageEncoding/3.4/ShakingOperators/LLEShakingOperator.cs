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
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("LLE Shaking Operator", "A shaking operator for VNS which uses LLE manipulators to perform the shaking.")]
  [StorableClass]
  public class LLEShakingOperator : ShakingOperator<ILinearLinkageManipulator>, IStochasticOperator, ILinearLinkageShakingOperator {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    public ICheckedItemList<ILinearLinkageManipulator> Shakers {
      get { return Operators; }
    }

    [StorableConstructor]
    protected LLEShakingOperator(bool deserializing) : base(deserializing) { }
    protected LLEShakingOperator(LLEShakingOperator original, Cloner cloner) : base(original, cloner) { }
    public LLEShakingOperator()
      : base() {
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The encoding to shake."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator that will be used for stochastic shaking operators."));
      for (var i = 1; i < 4; i++) {
        Operators.Add(new MoveItemManipulator(i * 2));
        Operators.Add(new SwapItemManipulator(i * 2));
        Operators.Add(new SplitGroupManipulator(i * 2));
        Operators.Add(new MergeGroupManipulator(i * 2));
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LLEShakingOperator(this, cloner);
    }

    #region Wiring of some parameters
    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeOperators(e.Items);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeOperators(e.Items);
    }

    private void ParameterizeOperators(IEnumerable<IndexedItem<ILinearLinkageManipulator>> items) {
      if (!items.Any()) return;
      foreach (var op in items.Select(x => x.Value).OfType<IStochasticOperator>())
        op.RandomParameter.ActualName = RandomParameter.Name;
      foreach (var op in items.Select(x => x.Value))
        op.LLEParameter.ActualName = LLEParameter.Name;
    }
    #endregion
  }
}
