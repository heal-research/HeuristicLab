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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Represents a log object where to store a specific value that should be logged.
  /// </summary>
  public class Log : ItemBase, IVisualizationItem {
    [Storable]
    private ItemList myItems;
    /// <summary>
    /// Gets or sets the items of the current instance.
    /// </summary>
    public ItemList Items {
      get { return myItems; }
      set { myItems = value; }
    }


    /// <summary>
    /// Initializes a new instance of <see cref="Log"/>.
    /// </summary>
    public Log() { }
    /// <summary>
    /// Initializes a new instance of <see cref="Log"/> with the given items.
    /// </summary>
    /// <param name="items">The list of items with which to initialize the current instance.</param>
    public Log(ItemList items) {
      myItems = items;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Log"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Log clone = (Log)base.Clone(clonedObjects);
      clone.myItems = (ItemList)Auxiliary.Clone(Items, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Creates an instance of <see cref="LogView"/> to represent the current instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="LogView"/>.</returns>
    public override IView CreateView() {
      return new LogView(this);
    }

    /// <summary>
    /// Occurs when the items have been changed.
    /// </summary>
    public event EventHandler ItemsChanged;
    /// <summary>
    /// Fires a new <c>ItemsChanged</c> event.
    /// </summary>
    protected virtual void OnItemsChanged() {
      if (ItemsChanged != null)
        ItemsChanged(this, new EventArgs());
    }
  }
}
