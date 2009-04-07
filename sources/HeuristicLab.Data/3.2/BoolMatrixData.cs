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
  /// A two-dimensional matrix consisting of boolean values.
  /// </summary>
  public class BoolMatrixData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the boolean elements of the matrix.
    /// </summary>
    /// <remarks>Uses the property <see cref="ArrayDataBase.Data"/> of base 
    /// class <see cref="ArrayDataBase"/>. No own data storage present.</remarks>
    public new bool[,] Data {
      get { return (bool[,])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolMatrixData"/> class.
    /// </summary>
    public BoolMatrixData() {
      Data = new bool[1, 1];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolMatrixData"/> class.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The boolean matrix the instance should represent.</param>
    public BoolMatrixData(bool[,] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BoolMatrixDataView"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="BoolMatrixDataView"/>.</returns>
    public override IView CreateView() {
      return new BoolMatrixDataView(this);
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The dimensions of the matrix are saved as attributes (<c>Dimension1</c>,<c>Dimension2</c>).<br/>
    /// The elements of the matrix are saved as string in the node's inner text, 
    /// each element separated by a semicolon, all line by line.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute dim1 = document.CreateAttribute("Dimension1");
      dim1.Value = Data.GetLength(0).ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim1);
      XmlAttribute dim2 = document.CreateAttribute("Dimension2");
      dim2.Value = Data.GetLength(1).ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim2);
      node.InnerText = ToString();
      return node;
    }
    /// <summary>
    /// Loads the persisted boolean matrix from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The dimensions of the matrix must be saved as Attributes (<c>Dimension1</c>, <c>Dimension2</c>). <br/>
    /// The elements of the matrix must be saved in the node's inner text as string, 
    /// each element separated by a semicolon, line by line (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      int dim1 = int.Parse(node.Attributes["Dimension1"].Value, CultureInfo.InvariantCulture.NumberFormat);
      int dim2 = int.Parse(node.Attributes["Dimension2"].Value, CultureInfo.InvariantCulture.NumberFormat);
      string[] tokens = node.InnerText.Split(';');
      bool[,] data = new bool[dim1, dim2];
      for (int i = 0; i < dim1; i++) {
        for (int j = 0; j < dim2; j++) {
          data[i, j] = bool.Parse(tokens[i * dim2 + j]);
        }
      }
      Data = data;
    }
    /// <summary>
    /// The string representation of the matrix.
    /// </summary>
    /// <returns>The elements of the matrix as a string seperated by semicolons, line by line.</returns>
    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.GetLength(0); i++) {
        for (int j = 0; j < Data.GetLength(1); j++) {
          builder.Append(";");
          builder.Append(Data[i, j].ToString());
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
