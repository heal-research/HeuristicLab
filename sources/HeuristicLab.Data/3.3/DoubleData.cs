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
  /// Class to represent double values.
  /// </summary>
  [EmptyStorableClass]
  public class DoubleData : ObjectData {
    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/> of base class <see cref="ObjectData"></see>. 
    /// No own data storage present.</remarks>
    public new double Data {
      get { return (double)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleData"/> with default value <c>0.0</c>.
    /// </summary>
    public DoubleData() {
      Data = 0.0;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="DoubleData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The double value the instance should represent.</param>
    public DoubleData(double data) {
      Data = data;
    }

    /// <summary>
    /// Clones the current instance and adds it to the dictionary <paramref name="clonedObjects"/>.
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="DoubleData"/>.</returns>
    public override IItem Clone(ICloner cloner) {
      DoubleData clone = new DoubleData();
      cloner.RegisterClonedObject(this, clone);
      clone.Data = Data;
      return clone;
    }
  }
}
