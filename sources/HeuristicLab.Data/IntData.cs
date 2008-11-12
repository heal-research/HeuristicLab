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
  /// The representation of an int value.
  /// </summary>
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
    /// Creates a new instance of the class <see cref="IntDataView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="IntDataView"/>.</returns>
    public override IView CreateView() {
      return new IntDataView(this);
    }

    /// <summary>
    /// Clones the current instance. 
    /// </summary>
    /// <remarks>Adds the cloned instance to the dictionary <paramref name="clonedObjects"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="IntData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      IntData clone = new IntData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The int value is saved as string in the node's inner text, 
    /// formatted according to the local culture info and its number format.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects.(Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = Data.ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
    /// <summary>
    ///  Loads the persisted int value from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The int value must be saved in the node's inner text as a string and 
    /// formatted according to the locale culture info and 
    /// its number format (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the int is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Data = int.Parse(node.InnerText, CultureInfo.InvariantCulture.NumberFormat);
    }

    /// <summary>
    /// The point of intersection where an <see cref="IObjectDataVisitor"/> 
    /// can change the int value.
    /// </summary>
    /// <param name="visitor">The visitor that changes the element.</param>
    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
