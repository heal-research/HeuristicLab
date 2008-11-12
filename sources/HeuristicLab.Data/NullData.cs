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

namespace HeuristicLab.Data {
  /// <summary>
  /// A class that represents <c>null</c>.
  /// </summary>
  public class NullData : ObjectData {
    /// <summary>
    /// Gets <c>null</c> and throws an exception in the setter.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the setter is called.</exception>
    public override object Data {
      get { return null; }
      set { throw new InvalidOperationException("Data property of NullData cannot be set"); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NullData"/> with <c>null</c>.
    /// </summary>
    public NullData() {
      base.Data = null;
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>Adds the cloned instance to the dictionary <paramref name="clonedObjects"/>.</remarks>
    /// <param name="clonedObjects">All already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="NullData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NullData clone = new NullData();
      clonedObjects.Add(Guid, clone);
      return clone;
    }

    /// <summary>
    /// The point of intersection where an <see cref="IObjectDataVisitor"/> 
    /// can change the value.
    /// </summary>
    /// <param name="visitor">The visitor that changes the element.</param>
    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
