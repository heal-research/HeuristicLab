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
  public class ConstrainedDoubleData : ConstrainedObjectData {
    public new double Data {
      get { return ((DoubleData)base.Data).Data; }
      set { ((DoubleData)base.Data).Data = value; }
    }

    public ConstrainedDoubleData() : this (0.0) {
    }
    public ConstrainedDoubleData(double data) : base() {
      base.Data = new DoubleData(data);
    }

    public virtual bool TrySetData(double data) {
      return base.TrySetData(new DoubleData(data));
    }
    public virtual bool TrySetData(double data, out ICollection<IConstraint> violatedConstraints) {
      return base.TrySetData(new DoubleData(data), out violatedConstraints);
    }

    public override IView CreateView() {
      return new ConstrainedDoubleDataView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedDoubleData clone = (ConstrainedDoubleData)base.Clone(clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Value", (DoubleData) base.Data, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      base.Data = (DoubleData)PersistenceManager.Restore(node.SelectSingleNode("Value"), restoredObjects);
    }

    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
