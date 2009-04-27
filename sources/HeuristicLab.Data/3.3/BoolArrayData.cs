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
  /// An array consisting of boolean values.
  /// </summary>
  public class BoolArrayData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the boolean elements of the array.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase.Data"/> of base class <see cref="ArrayDataBase"/>. 
    /// No own data storage present.</remarks>
    public new bool[] Data {
      get { return (bool[])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolArrayData"/> class.
    /// </summary>
    public BoolArrayData() {
      Data = new bool[0];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolArrayData"/> class. 
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The boolean array the instance should represent.</param>
    public BoolArrayData(bool[] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BoolArrayDataView"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="BoolArrayDataView"/>.</returns>
    public override IView CreateView() {
      return new BoolArrayDataView(this);
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The boolean elements of the array are saved as string in the node's inner text, each element separated by a semicolon.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = ToString();
      return node;
    }
    /// <summary>
    /// Loads the persisted boolean array from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks> The elements of the boolean array must be saved in the node's inner 
    /// text as a string, each element separated by a semicolon 
    /// (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">A Dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      string[] tokens = node.InnerText.Split(';');
      bool[] data = new bool[tokens.Length];
      for (int i = 0; i < data.Length; i++)
        data[i] = bool.Parse(tokens[i]);
      Data = data;
    }
  }
}
