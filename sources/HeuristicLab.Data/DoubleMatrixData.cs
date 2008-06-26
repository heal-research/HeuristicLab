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
using System.Globalization;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public class DoubleMatrixData : ArrayDataBase {
    public new double[,] Data {
      get { return (double[,])base.Data; }
      set { base.Data = value; }
    }

    public DoubleMatrixData() {
      Data = new double[1, 1];
    }
    public DoubleMatrixData(double[,] data) {
      Data = data;
    }

    public override IView CreateView() {
      return new DoubleMatrixDataView(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute dim1 = document.CreateAttribute("Dimension1");
      dim1.Value = Data.GetLength(0).ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim1);
      XmlAttribute dim2 = document.CreateAttribute("Dimension2");
      dim2.Value = Data.GetLength(1).ToString(CultureInfo.InvariantCulture.NumberFormat);
      node.Attributes.Append(dim2);
      node.InnerText = ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      int dim1 = int.Parse(node.Attributes["Dimension1"].Value, CultureInfo.InvariantCulture.NumberFormat);
      int dim2 = int.Parse(node.Attributes["Dimension2"].Value, CultureInfo.InvariantCulture.NumberFormat);
      string[] tokens = node.InnerText.Split(';');
      double[,] data = new double[dim1, dim2];
      for (int i = 0; i < dim1; i++) {
        for (int j = 0; j < dim2; j++) {
          if(double.TryParse(tokens[i * dim2 + j], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data[i, j])==false) {
            throw new FormatException("Can't parse " + tokens[i * dim2 + j] + " as double value.");
          }
        }
      }
      Data = data;
    }

    public override string ToString() {
      return ToString(CultureInfo.CurrentCulture.NumberFormat);
    }

    private string ToString(NumberFormatInfo format) {
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.GetLength(0); i++) {
        for (int j = 0; j < Data.GetLength(1); j++) {
          builder.Append(";");
          builder.Append(Data[i, j].ToString("r", format));
        }
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
