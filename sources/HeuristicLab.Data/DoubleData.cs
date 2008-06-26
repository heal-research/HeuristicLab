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
  public class DoubleData : ObjectData {
    public new double Data {
      get { return (double)base.Data; }
      set { base.Data = value; }
    }

    public DoubleData() {
      Data = 0.0;
    }
    public DoubleData(double data) {
      Data = data;
    }

    public override IView CreateView() {
      return new DoubleDataView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      DoubleData clone = new DoubleData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = Data.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      double data;
      if(double.TryParse(node.InnerText, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out data) == true) {
        Data = data;
      } else {
        throw new FormatException("Can't parse " + node.InnerText + " as double value.");       
      }
    }

    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
