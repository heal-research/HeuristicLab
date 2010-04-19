#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [Item("MultiIntegerVectorCrossover", "Randomly selects and applies one of its crossovers every time it is called.")]
  [StorableClass]
  public class MultiIntegerVectorCrossover : StochasticMultiOperator<IIntegerVectorCrossover>, IIntegerVectorCrossover {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }
    public override bool AutomaticTypeDiscovery {
      get { return true; }
    }

    public ILookupParameter<ItemArray<IntegerVector>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<IntegerVector>>)Parameters["Parents"]; }
    }

    public ILookupParameter<IntegerVector> ChildParameter {
      get { return (ILookupParameter<IntegerVector>)Parameters["Child"]; }
    }

    [StorableConstructor]
    private MultiIntegerVectorCrossover(bool deserializing) : base(deserializing) { }
    public MultiIntegerVectorCrossover()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<IntegerVector>("Parents", "The parent integer vector which should be crossed."));
      ParentsParameter.ActualName = "IntegerVector";
      Parameters.Add(new LookupParameter<IntegerVector>("Child", "The child integer vector resulting from the crossover."));
      ChildParameter.ActualName = "IntegerVector";

      Initialize();
      ParameterizeCrossovers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IIntegerVectorCrossover>>(Operators_ItemsAdded);
      Operators.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IIntegerVectorCrossover>>(Operators_ItemsReplaced);
    }

    private void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IIntegerVectorCrossover>> e) {
      ParameterizeCrossovers();
    }

    private void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IIntegerVectorCrossover>> e) {
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IIntegerVectorCrossover crossover in Operators.OfType<IIntegerVectorCrossover>()) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }
  }
}
