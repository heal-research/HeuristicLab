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
  public class ConstrainedIntData : ConstrainedObjectData {
    public new int Data {
      get { return ((IntData)base.Data).Data; }
      set { ((IntData)base.Data).Data = value; }
    }

    public ConstrainedIntData() : this (0) {
    }
    public ConstrainedIntData(int data) : base() {
      base.Data = new IntData(data);
    }

    public virtual bool TrySetData(int data) {
      return base.TrySetData(new IntData(data));
    }
    public virtual bool TrySetData(int data, out ICollection<IConstraint> violatedConstraints) {
      return base.TrySetData(new IntData(data), out violatedConstraints);
    }

    public override IView CreateView() {
      return new ConstrainedIntDataView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedIntData clone = (ConstrainedIntData)base.Clone(clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Value", ((IntData)base.Data), document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      base.Data = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Value"), restoredObjects);
    }

    public override void Accept(IObjectDataVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
