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
  /// Class to represent double values.
  /// </summary>
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
    /// Creates a new instance of the <see cref="DoubleDataView"/> class.
    /// </summary>
    /// <returns>The created instance of <see cref="DoubleDataView"/>.</returns>
    public override IView CreateView() {
      return new DoubleDataView(this);
    }

    /// <summary>
    /// Clones the current instance and adds it to the dictionary <paramref name="clonedObjects"/>.
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="DoubleData"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      DoubleData clone = new DoubleData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The double value is saved in the node's inner text as string, 
    /// its format depending on the local culture info and its number format.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = Data.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
    /// <summary>
    /// Loads the persisted double value from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The double value must be saved in the node's inner text as a string and 
    /// formatted according to the locale culture info and 
    /// its number format (see <see cref="GetXmlNode"/>).</remarks>
    /// <exception cref="System.FormatException">Thrown when the saved value cannot be parsed as a double value.</exception>
    /// <param name="node">The <see cref="XmlNode"/> where the double is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      double data;
      if(double.TryParse(node.InnerText, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data) == true) {
        Data = data;
      } else {
        throw new FormatException("Can't parse " + node.InnerText + " as double value.");       
      }
    }

    /// <summary>
    /// The point of intersection where an <see cref="IObjectDataVisitor"/> 
    /// can change the elements of the matrix.
    /// </summary>
    /// <param name="visitor">The visitor that changes the elements.</param>
    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
