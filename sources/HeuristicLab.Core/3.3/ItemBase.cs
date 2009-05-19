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
  public abstract class ItemBase : StorableBase, IItem {
    /// <summary>
    /// Creates a new instance of <see cref="ItemBaseView"/> for 
    /// visual representation of the current instance.
    /// </summary>
    /// <returns>The created instance as <see cref="ItemBaseView"/>.</returns>
    public virtual IView CreateView() {
      return new ItemBaseView(this);
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
