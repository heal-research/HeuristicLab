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
  /// Represents a double value having some constraints (e.g. smaller than 5.3).
  /// </summary>
  [EmptyStorableClass]
  public class ConstrainedDoubleData : ConstrainedObjectData {
    /// <summary>
    /// Gets or sets the double value of the current instance as <see cref="DoubleData"/>.
    /// </summary>
    /// <remarks>Uses property <see cref="ConstrainedObjectData.Data"/> of base 
    /// class <see cref="ConstrainedObjectData"/>. No own data storage present.</remarks>
    public new double Data {
      get { return ((DoubleData)base.Data).Data; }
      set { ((DoubleData)base.Data).Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedDoubleData"/> with default value <c>0.0</c>.
    /// </summary>
    public ConstrainedDoubleData()
      : this(0.0) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedDoubleData"/> with the specified 
    /// double value <paramref name="data"/> as <see cref="DoubleData"/>.
    /// </summary>
    /// <param name="data">The double value to represent.</param>
    public ConstrainedDoubleData(double data)
      : base() {
      base.Data = new DoubleData(data);
    }

    /// <inheritdoc cref="ConstrainedObjectData.TrySetData(object)"/>
    public virtual bool TrySetData(double data) {
      return base.TrySetData(new DoubleData(data));
    }
    /// <inheritdoc cref="ConstrainedObjectData.TrySetData(object, out System.Collections.Generic.ICollection&lt;HeuristicLab.Core.IConstraint&gt;)"/>
    public virtual bool TrySetData(double data, out ICollection<IConstraint> violatedConstraints) {
      return base.TrySetData(new DoubleData(data), out violatedConstraints);
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>Uses the <see cref="ConstrainedObjectData.Clone"/> implementation of base class <see cref="ConstrainedObjectData"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="ConstrainedDoubleData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedDoubleData clone = (ConstrainedDoubleData)base.Clone(clonedObjects);
      return clone;
    }

  }
}
