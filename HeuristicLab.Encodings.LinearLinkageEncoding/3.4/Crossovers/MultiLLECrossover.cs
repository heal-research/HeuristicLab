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
  [Item("Multi LLE Crossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiLinearLinkageCrossover : StochasticMultiBranch<ILinearLinkageCrossover>, ILinearLinkageCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public IScopeTreeLookupParameter<LinearLinkage> ParentsParameter {
      get { return (IScopeTreeLookupParameter<LinearLinkage>)Parameters["Parents"]; }
    }

    public ILookupParameter<LinearLinkage> ChildParameter {
      get { return (ILookupParameter<LinearLinkage>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected MultiLinearLinkageCrossover(bool deserializing) : base(deserializing) { }
    protected MultiLinearLinkageCrossover(MultiLinearLinkageCrossover original, Cloner cloner) : base(original, cloner) { }
    public MultiLinearLinkageCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<LinearLinkage>("Parents", "The parent LLE which should be crossed."));
      ParentsParameter.ActualName = "LLE";
      Parameters.Add(new LookupParameter<LinearLinkage>("Child", "The child LLE resulting from the crossover."));
      ChildParameter.ActualName = "LLE";

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(ILinearLinkageCrossover), typeof(LinearLinkageEncoding).Assembly)) {
        if (!typeof(MultiOperator<ILinearLinkageCrossover>).IsAssignableFrom(type))
          Operators.Add((ILinearLinkageCrossover)Activator.CreateInstance(type), true);
      }

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiLinearLinkageCrossover(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers(e.Items);
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<ILinearLinkageCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers(e.Items);
    }

    private void ParameterizeCrossovers(IEnumerable<IndexedItem<ILinearLinkageCrossover>> crossovers) {
      foreach (var c in crossovers.Select(x => x.Value)) {
        c.ChildParameter.ActualName = ChildParameter.Name;
        c.ParentsParameter.ActualName = ParentsParameter.Name;
        var stOp = c as IStochasticOperator;
        if (stOp != null) stOp.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one LLE crossover to choose from.");
      return base.InstrumentedApply();
    }
  }
}
