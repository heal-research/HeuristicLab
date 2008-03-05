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

namespace HeuristicLab.Core {
  public abstract class StorableBase : IStorable {
    private Guid myGuid;
    public Guid Guid {
      get { return myGuid; }
    }

    protected StorableBase() {
      myGuid = Guid.NewGuid();
    }

    public object Clone() {
      return Auxiliary.Clone(this, new Dictionary<Guid, object>());
    }
    public virtual object Clone(IDictionary<Guid, object> clonedObjects) {
      object clone = Activator.CreateInstance(this.GetType());
      clonedObjects.Add(Guid, clone);
      return clone;
    }

    public virtual XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = document.CreateNode(XmlNodeType.Element, name, null);
      XmlAttribute typeAttribute = document.CreateAttribute("Type");
      typeAttribute.Value = PersistenceManager.BuildTypeString(this.GetType());
      node.Attributes.Append(typeAttribute);
      XmlAttribute guidAttribute = document.CreateAttribute("GUID");
      guidAttribute.Value = Guid.ToString();
      node.Attributes.Append(guidAttribute);
      return node;
    }
    public virtual void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      myGuid = new Guid(node.Attributes["GUID"].Value);
    }
  }
}
