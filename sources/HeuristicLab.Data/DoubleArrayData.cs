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
  public class DoubleArrayData : ArrayDataBase {
    public new double[] Data {
      get { return (double[])base.Data; }
      set { base.Data = value; }
    }

    public DoubleArrayData() {
      Data = new double[0];
    }
    public DoubleArrayData(double[] data) {
      Data = data;
    }

    public override IView CreateView() {
      return new DoubleArrayDataView(this);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = ToString(CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
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

    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
