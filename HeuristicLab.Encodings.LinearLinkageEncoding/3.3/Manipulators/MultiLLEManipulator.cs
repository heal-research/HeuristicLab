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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Multi LLE Manipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableClass]
  public class MultiLinearLinkageManipulator : StochasticMultiBranch<ILinearLinkageManipulator>, ILinearLinkageManipulator, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<LinearLinkage> LLEParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["LLE"]; }
    }

    [StorableConstructor]
    protected MultiLinearLinkageManipulator(bool deserializing) : base(deserializing) { }
    protected MultiLinearLinkageManipulator(MultiLinearLinkageManipulator original, Cloner cloner) : base(original, cloner) { }
    public MultiLinearLinkageManipulator()
      : base() {
      Parameters.Add(new LookupParameter<LinearLinkage>("LLE", "The encoding vector that is to be manipulated."));
      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(ILinearLinkageManipulator), typeof(LinearLinkageEncoding).Assembly)) {
        if (!typeof(MultiOperator<ILinearLinkageManipulator>).IsAssignableFrom(type))
          Operators.Add((ILinearLinkageManipulator)Activator.CreateInstance(type), true);
      }
      Operators.SetItemCheckedState(Operators.OfType<SwapItemManipulator>().First(), false);
      Operators.SetItemCheckedState(Operators.OfType<GraftManipulator>().First(), false);
      SelectedOperatorParameter.ActualName = "SelectedManipulationOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiLinearLinkageManipulator(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageManipulator>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeManipulators(e.Items);
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageManipulator>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeManipulators(e.Items);
    }

    private void ParameterizeManipulators(IEnumerable<IndexedItem<ILinearLinkageManipulator>> manipulators) {
      foreach (var m in manipulators.Select(x => x.Value)) {
        m.LLEParameter.ActualName = LLEParameter.Name;
        var stOp = m as IStochasticOperator;
        if (stOp != null) stOp.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one LLE manipulator to choose from.");
      return base.InstrumentedApply();
    }
  }
}
