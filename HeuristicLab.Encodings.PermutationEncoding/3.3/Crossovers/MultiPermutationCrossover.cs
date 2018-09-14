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

using System;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("MultiPermutationCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiPermutationCrossover : StochasticMultiBranch<IPermutationCrossover>, IPermutationCrossover, IStochasticOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }

    public ILookupParameter<ItemArray<Permutation>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<Permutation>>)Parameters["Parents"]; }
    }

    public ILookupParameter<Permutation> ChildParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Child"]; }
    }

    [StorableConstructor]
    protected MultiPermutationCrossover(bool deserializing) : base(deserializing) { }
    protected MultiPermutationCrossover(MultiPermutationCrossover original, Cloner cloner) : base(original, cloner) { }
    public MultiPermutationCrossover()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<Permutation>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "Permutation";
      Parameters.Add(new LookupParameter<Permutation>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "Permutation";

      foreach (Type type in ApplicationManager.Manager.GetTypes(typeof(IPermutationCrossover))) {
        if (!typeof(MultiOperator<IPermutationCrossover>).IsAssignableFrom(type))
          Operators.Add((IPermutationCrossover)Activator.CreateInstance(type), true);
      }

      SelectedOperatorParameter.ActualName = "SelectedCrossoverOperator";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiPermutationCrossover(this, cloner);
    }

    protected override void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationCrossover>> e) {
      base.Operators_ItemsReplaced(sender, e);
      ParameterizeCrossovers();
    }

    protected override void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationCrossover>> e) {
      base.Operators_ItemsAdded(sender, e);
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IPermutationCrossover crossover in Operators.OfType<IPermutationCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }

    public override IOperation InstrumentedApply() {
      if (Operators.Count == 0) throw new InvalidOperationException(Name + ": Please add at least one permutation crossover to choose from.");
      return base.InstrumentedApply();
    }
  }
}
