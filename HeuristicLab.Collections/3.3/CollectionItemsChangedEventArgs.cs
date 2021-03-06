#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Collections {
  public class CollectionItemsChangedEventArgs<T> : EventArgs {
    private IEnumerable<T> items;
    public IEnumerable<T> Items {
      get { return items; }
    }
    private IEnumerable<T> oldItems;
    public IEnumerable<T> OldItems {
      get { return oldItems; }
    }

    public CollectionItemsChangedEventArgs(IEnumerable<T> items) {
      this.items = items;
      this.oldItems = Enumerable.Empty<T>();
    }
    public CollectionItemsChangedEventArgs(IEnumerable<T> items, IEnumerable<T> oldItems) {
      this.items = items;
      this.oldItems = oldItems;
    }
  }
}
