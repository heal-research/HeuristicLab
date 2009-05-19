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
  /// The representation of an int value having some constraints (e.g. smaller than 2).
  /// </summary>
  [EmptyStorableClass]
  public class ConstrainedIntData : ConstrainedObjectData {
    /// <summary>
    /// Gets or sets the int value of the current instance as <see cref="IntData"/>.
    /// </summary>
    /// <remarks>Uses property <see cref="ConstrainedObjectData.Data"/> of base class 
    /// <see cref="ConstrainedObjectData"/>. No own data storage present.</remarks>
    public new int Data {
      get { return ((IntData)base.Data).Data; }
      set { ((IntData)base.Data).Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedIntData"/> with default value <c>0</c>.
    /// </summary>
    public ConstrainedIntData()
      : this(0) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedIntData"/> with the specified 
    /// int value <paramref name="data"/> as <see cref="IntData"/>.
    /// </summary>
    /// <param name="data">The integer value to represent.</param>
    public ConstrainedIntData(int data)
      : base() {
      base.Data = new IntData(data);
    }

    /// <inheritdoc cref="ConstrainedObjectData.TrySetData(object)"/>
    public virtual bool TrySetData(int data) {
      return base.TrySetData(new IntData(data));
    }
    /// <inheritdoc cref="ConstrainedObjectData.TrySetData(object, out System.Collections.Generic.ICollection&lt;HeuristicLab.Core.IConstraint&gt;)"/>
    public virtual bool TrySetData(int data, out ICollection<IConstraint> violatedConstraints) {
      return base.TrySetData(new IntData(data), out violatedConstraints);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConstrainedIntDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="ConstrainedIntDataView"/>.</returns>
    public override IView CreateView() {
      return new ConstrainedIntDataView(this);
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>Uses the <see cref="ConstrainedObjectData.Clone"/> implementation of base class <see cref="ConstrainedObjectData"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The clone instance as <see cref="ConstrainedIntData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedIntData clone = (ConstrainedIntData)base.Clone(clonedObjects);
      return clone;
    }
  }
}
