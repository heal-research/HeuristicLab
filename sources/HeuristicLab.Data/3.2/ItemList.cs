#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  /// <summary>
  /// Generic list of elements that implement the interface <see cref="IItem"/>.
  /// </summary>
  public class ItemList : ItemList<IItem> {
    /// <summary>
    /// Clones the current list and all its elements.
    /// </summary>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="ItemList"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemList clone = new ItemList();
      clonedObjects.Add(Guid, clone);
      base.CloneElements(clone, clonedObjects);
      return clone;
    }
  }
}
