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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("MultiPermutationManipulator", "Randomly selects and applies one of its manipulators every time it is called.")]
  [StorableClass]
  public class MultiPermutationManipulator : StochasticMultiOperator<IPermutationManipulator>, IPermutationManipulator {
    public override bool CanChangeName {
      get { return false; }
    }
    protected override bool CreateChildOperation {
      get { return true; }
    }
    public override bool AutomaticTypeDiscovery {
      get { return true; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    [StorableConstructor]
    private MultiPermutationManipulator(bool deserializing) : base(deserializing) { }
    public MultiPermutationManipulator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation that is being manipulating."));

      Initialize();
      ParameterizeManipulators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IPermutationManipulator>>(Operators_ItemsAdded);
      Operators.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IPermutationManipulator>>(Operators_ItemsReplaced);
    }

    private void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      ParameterizeManipulators();
    }

    private void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationManipulator>> e) {
      ParameterizeManipulators();
    }

    private void ParameterizeManipulators() {
      foreach (IPermutationManipulator manipulator in Operators.OfType<IPermutationManipulator>()) {
        manipulator.PermutationParameter.ActualName = PermutationParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators.OfType<IStochasticOperator>()) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }
  }
}
