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
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// The base class for all storable objects.
  /// </summary>
  public abstract class StorableBase : IStorable {

    [Storable]
    private Guid myGuid;
    /// <summary>
    /// Gets the Guid of the item.
    /// </summary>
    public Guid Guid {
      get { return myGuid; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="StorableBase"/> with a new <see cref="Guid"/>. 
    /// </summary>
    protected StorableBase() {
      myGuid = Guid.NewGuid();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Uses the <see cref="Auxiliary.Clone"/> method of the class <see cref="Auxiliary"/>.</remarks>
    /// <returns>The clone.</returns>
    public object Clone() {
      return Auxiliary.Clone(this, new Dictionary<Guid, object>());
    }
    /// <summary>
    /// Clones the current instance with the <see cref="M:Activator.CreateInstance"/> 
    /// method of <see cref="Activator"/>.
    /// </summary>
    /// <param name="clonedObjects">All already cloned objects.</param>
    /// <returns>The clone.</returns>
    public virtual object Clone(IDictionary<Guid, object> clonedObjects) {
      object clone = Activator.CreateInstance(this.GetType());
      clonedObjects.Add(Guid, clone);
      return clone;
    }
  }
}
