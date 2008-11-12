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
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The string value is saved in the node's inner text.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects.(Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = Data;
      return node;
    }
    /// <summary>
    ///  Loads the persisted string value from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The string value must be saved in the node's inner text 
    /// (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the string is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Data = node.InnerText;
    }

    /// <summary>
    /// The string representation of the current instance.
    /// </summary>
    /// <returns>The string value.</returns>
    public override string ToString() {
      return Data;
    }

    /// <summary>
    /// The point of intersection where an <see cref="IObjectDataVisitor"/> 
    /// can change the string.
    /// </summary>
    /// <param name="visitor">The visitor that changes the element.</param>
    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
