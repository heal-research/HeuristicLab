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

using System;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.PermutationEncoding.Crossovers {
  public class PermutationMultiCrossover : MultiCrossover<IPermutationCrossover>, IPermutationCrossover {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<ItemArray<Permutation>> ParentsParameter {
      get { return (ILookupParameter<ItemArray<Permutation>>)Parameters["Parents"]; }
    }

    public ILookupParameter<Permutation> ChildParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Child"]; }
    }

    [StorableConstructor]
    private PermutationMultiCrossover(bool deserializing) : base(deserializing) { }
    public PermutationMultiCrossover()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<Permutation>("Parents", "The parent permutations which should be crossed."));
      ParentsParameter.ActualName = "Permutation";
      Parameters.Add(new LookupParameter<Permutation>("Child", "The child permutation resulting from the crossover."));
      ChildParameter.ActualName = "Permutation";

      Initialize();
      ParameterizeCrossovers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      Operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IPermutationCrossover>>(Operators_ItemsAdded);
    }

    private void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IPermutationCrossover>> e) {
      ParameterizeCrossovers();
    }

    private void ParameterizeCrossovers() {
      foreach (IPermutationCrossover crossover in Operators) {
        crossover.ChildParameter.ActualName = ChildParameter.Name;
        crossover.ParentsParameter.ActualName = ParentsParameter.Name;
      }
      foreach (IStochasticOperator crossover in Operators) {
        crossover.RandomParameter.ActualName = RandomParameter.Name;
      }
    }
  }
}
