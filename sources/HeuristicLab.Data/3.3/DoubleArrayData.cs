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
  /// The representation of an array of double values.
  /// </summary>
  public class DoubleArrayData : ArrayDataBase {
    /// <summary>
    /// Gets or sets the double elements of the array.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase.Data"/> of base class 
    /// <see cref="ArrayDataBase"/>. No own data storage present.</remarks>
    public new double[] Data {
      get { return (double[])base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleArrayData"/> class.
    /// </summary>
    public DoubleArrayData() {
      Data = new double[0];
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleArrayData"/> class.
    /// <note type="caution"> No CopyConstructor! <paramref name="data"/> is not copied!</note>
    /// </summary>
    /// <param name="data">The double array the instance should represent.</param>
    public DoubleArrayData(double[] data) {
      Data = data;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DoubleArrayDataView"/> class.
    /// </summary>
    /// <returns>The created instance of the <see cref="DoubleArrayDataView"/>.</returns>
    public override IView CreateView() {
      return new DoubleArrayDataView(this);
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The double elements of the array are saved in the node's inner text as string, 
    /// each element, whose format depends on the locale culture info, separated by a semicolon.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"></see>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
    
    /// <summary>
    /// Loads the persisted double array from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The double elemets of the array must be saved in the inner text of the node as string, 
    /// each element separated by a semicolon and formatted according to the locale culture info and 
    /// its number format (see <see cref="GetXmlNode"/>).</remarks>
    /// <exception cref="System.FormatException">Thrown when a saved element cannot be parsed as a double value.</exception>
    /// <param name="node">The <see cref="XmlNode"></see> where the instance is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      string[] tokens = node.InnerText.Split(';');
      double[] data = new double[tokens.Length];
      for(int i = 0; i < data.Length; i++)
        if(double.TryParse(tokens[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data[i]) == false) {
          throw new FormatException("Can't parse " + tokens[i] + " as double value.");
        }
      Data = data;
    }

    /// <summary>
    /// The string representation of the array, formatted according to the given <paramref name="format"/>.
    /// </summary>
    /// <param name="format">The <see cref="NumberFormatInfo"></see> the single double 
    /// values should be formatted accordingly.</param>
    /// <returns>The elements of the array as string, each element separated by a semicolon and 
    /// formatted according to the parameter <paramref name="format"/>.</returns>
    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for(int i = 0; i < Data.Length; i++) {
        builder.Append(";");
        builder.Append(Data[i].ToString("r", format));
      }
      if(builder.Length > 0)
        builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
