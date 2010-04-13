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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  [Item("VariableCollection", "Represents a collection of variables.")]
  public sealed class VariableCollection : NamedItemCollection<IVariable> {
    public VariableCollection() : base() { }
    public VariableCollection(int capacity) : base(capacity) { }
    public VariableCollection(IEnumerable<IVariable> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      VariableCollection clone = new VariableCollection();
      cloner.RegisterClonedObject(this, clone);
      clone.ReadOnlyView = ReadOnlyView;
      foreach (string key in dict.Keys)
        clone.dict.Add(key, (IVariable)cloner.Clone(dict[key]));
      clone.Initialize();
      return clone;
    }
  }
}
