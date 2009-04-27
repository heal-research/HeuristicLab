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
  /// The representation of a string.
  /// </summary>
  public class StringData : ObjectData {
    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/> of base class <see cref="ObjectData"/>. 
    /// No own data storage present.</remarks>
    public new string Data {
      get { return (string)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StringData"/> 
    /// with the name of the type of the current instance as default value.
    /// </summary>
    public StringData() {
      Data = this.GetType().Name;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="StringData"/> with the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The string value the current instance should represent.</param>
    public StringData(string data) {
      Data = data;
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="StringDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="StringDataView"/>.</returns>
    public override IView CreateView() {
      return new StringDataView(this);
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>The current instance is added to the dictionary <paramref name="clonedObjects"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The coned instance as <see cref="StringData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      StringData clone = new StringData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }

    /// <summary>
    /// The string representation of the current instance.
    /// </summary>
    /// <returns>The string value.</returns>
    public override string ToString() {
      return Data;
    }
  }
}
