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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents the base class for all basic item types.
  /// </summary>
  [EmptyStorableClass]
  public abstract class ItemBase : IItem {
    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <remarks>
    /// This method is the entry point for creating a deep clone of a whole object graph.
    /// </remarks>
    /// <returns>A clone of this instance.</returns>
    public object Clone() {
      return Clone(new Cloner());
    }

    /// <summary>
    /// Creates a deep clone of this instance.
    /// </summary>
    /// <remarks>This method should not be called directly. It is used for creating clones of
    /// objects which are contained in the object that is currently cloned.</remarks>
    /// <param name="cloner">The cloner which is responsible for keeping track of all already
    /// cloned objects.</param>
    /// <returns>A clone of this instance.</returns>
    public virtual IItem Clone(ICloner cloner) {
      ItemBase clone = (ItemBase)Activator.CreateInstance(this.GetType());
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }

    /// <summary>
    /// Gets the string representation of the current instance.
    /// </summary>
    /// <returns>The type name of the current instance.</returns>
    public override string ToString() {
      return GetType().Name;
    }

    /// <summary>
    /// Fires a new <c>Changed</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="OnChanged"/>.</remarks>
    public void FireChanged() {
      OnChanged();
    }

    /// <summary>
    /// Occurs when the current item was changed.
    /// </summary>
    public event EventHandler Changed;
    /// <summary>
    /// Fires a new <c>Changed</c> event.
    /// </summary>
    protected virtual void OnChanged() {
      if (Changed != null)
        Changed(this, new EventArgs());
    }
  }
}
