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
  /// Class to represent boolean values.
  /// </summary>
  public class BoolData : ObjectData {
    /// <summary>
    /// Gets or sets the boolean value.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/>
    /// of base class <see cref="ObjectData"/>. No own data storage present.</remarks>
    public new bool Data {
      get { return (bool)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BoolData"/> with default value <c>false</c>.
    /// </summary>
    public BoolData() {
      Data = false;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="BoolData"/> with the boolean value <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The boolean value to assign.</param>
    public BoolData(bool data) {
      Data = data;
    }
    /// <summary>
    /// Creates a new instance of the <see cref="BoolDataView"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="BoolDataView"/>.</returns>
    public override IView CreateView() {
      return new BoolDataView(this);
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>The cloned instance is added to the <paramref name="dictionary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="BoolData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      BoolData clone = new BoolData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }
  }
}
