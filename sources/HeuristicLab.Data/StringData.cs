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
  public class StringData : ObjectData {
    public new string Data {
      get { return (string)base.Data; }
      set { base.Data = value; }
    }

    public StringData() {
      Data = this.GetType().Name;
    }
    public StringData(string data) {
      Data = data;
    }

    public override IView CreateView() {
      return new StringDataView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      StringData clone = new StringData();
      clonedObjects.Add(Guid, clone);
      clone.Data = Data;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.InnerText = Data;
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Data = node.InnerText;
    }

    public override string ToString() {
      return Data;
    }

    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
