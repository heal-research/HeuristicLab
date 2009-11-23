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
using System.Globalization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  /// <summary>
  /// The representation of an int value.
  /// </summary>
  [EmptyStorableClass]
  public class IntData : ObjectData {
    /// <summary>
    /// Gets or sets the int value.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/> of base class <see cref="ObjectData"/>. 
    /// No own data storage present.</remarks>
    public new int Data {
      get { return (int)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntData"/> with default value <c>0</c>.
    /// </summary>
    public IntData() {
      Data = 0;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="IntData"/>.
    /// </summary>
    /// <param name="data">The int value the current instance should represent.</param>
    public IntData(int data) {
      Data = data;
    }

    /// <summary>
    /// Clones the current instance. 
    /// </summary>
    /// <remarks>Adds the cloned instance to the dictionary <paramref name="clonedObjects"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="IntData"/>.</returns>
    public override IItem Clone(ICloner cloner) {
      IntData clone = new IntData();
      cloner.RegisterClonedObject(this, clone);
      clone.Data = Data;
      return clone;
    }
  }
}
