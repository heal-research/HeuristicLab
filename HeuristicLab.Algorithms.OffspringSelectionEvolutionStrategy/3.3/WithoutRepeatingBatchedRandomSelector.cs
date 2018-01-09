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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.OffspringSelectionEvolutionStrategy {
  [Item("WithoutRepeatingBatchedRandomSelector", "Selects m batches of n parents where in each batch the n parents are drawn without repeating.")]
  [StorableClass]
  public class WithoutRepeatingBatchedRandomSelector : StochasticSelector {
    public override bool CanChangeName {
      get { return true; }
    }
    public IValueLookupParameter<IntValue> ChildrenParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Children"]; }
    }
    public IValueLookupParameter<IntValue> ParentsPerChildParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["ParentsPerChild"]; }
    }

    [StorableConstructor]
    protected WithoutRepeatingBatchedRandomSelector(bool deserializing) : base(deserializing) { }
    protected WithoutRepeatingBatchedRandomSelector(WithoutRepeatingBatchedRandomSelector original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new WithoutRepeatingBatchedRandomSelector(this, cloner);
    }

    public WithoutRepeatingBatchedRandomSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("Children", "The number of children to select (number of batches)."));
      Parameters.Add(new ValueLookupParameter<IntValue>("ParentsPerChild", "The number of parents per child (size of each batch)."));
    }

    protected override IScope[] Select(List<IScope> scopes) {
      IRandom random = RandomParameter.ActualValue;
      int children = ChildrenParameter.ActualValue.Value;
      int parents = ParentsPerChildParameter.ActualValue.Value;
      int parentsAvailable = scopes.Count;

      if (parents > parentsAvailable)
        throw new InvalidOperationException("WithoutRepeatingBatchedRandomSelector: Cannot select more parents per child than there are parents available");

      IScope[] result = new IScope[children * parents];
      int count = 0;
      HashSet<int> selectedParents = new HashSet<int>();
      for (int i = 0; i < children; i++) {
        selectedParents.Clear();
        for (int j = 0; j < parents; j++) {
          int nextParent = j; // will be used in case parents == parentsAvailable
          if (parents < parentsAvailable) {
            do {
              nextParent = random.Next(parentsAvailable);
            } while (selectedParents.Contains(nextParent));
          }

          result[count++] = (IScope)scopes[nextParent].Clone();
          selectedParents.Add(nextParent);
        }
      }

      return result;
    }
  }
}
