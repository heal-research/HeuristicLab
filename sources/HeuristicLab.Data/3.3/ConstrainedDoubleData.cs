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

namespace HeuristicLab.Data {
  /// <summary>
  /// Represents a double value having some constraints (e.g. smaller than 5.3).
  /// </summary>
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
    public ConstrainedDoubleData() : this (0.0) {
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedDoubleData"/> with the specified 
    /// double value <paramref name="data"/> as <see cref="DoubleData"/>.
    /// </summary>
    /// <param name="data">The double value to represent.</param>
    public ConstrainedDoubleData(double data) : base() {
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
    /// Creates a new instance of <see cref="ConstrainedDoubleDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="ConstrainedDoubleDataView"/>.</returns>
    public override IView CreateView() {
      return new ConstrainedDoubleDataView(this);
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

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Uses the <see cref="ConstrainedItemBase.GetXmlNode"/> implementation of base class 
    /// <see cref="ConstrainedObjectData"/>. The double value is saved as a <see cref="DoubleData"/> 
    /// in a child node having the tag name <c>Value</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Value", (DoubleData) base.Data, document, persistedObjects));
      return node;
    }
    /// <summary>
    /// Loads the persisted double value from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The double value must be saved in the child node as a persisted <see cref="DoubleData"/>
    /// having the tag name <c>Value</c> (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the double is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      base.Data = (DoubleData)PersistenceManager.Restore(node.SelectSingleNode("Value"), restoredObjects);
    }
  }
}
