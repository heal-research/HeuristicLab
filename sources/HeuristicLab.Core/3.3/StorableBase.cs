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
  /// <summary>
  /// The base class for all storable objects.
  /// </summary>
  public abstract class StorableBase : IStorable {
    private Guid myGuid;
    /// <summary>
    /// Gets the Guid of the item.
    /// </summary>
    public Guid Guid {
      get { return myGuid; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="StorableBase"/> with a new <see cref="Guid"/>. 
    /// </summary>
    protected StorableBase() {
      myGuid = Guid.NewGuid();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Uses the <see cref="Auxiliary.Clone"/> method of the class <see cref="Auxiliary"/>.</remarks>
    /// <returns>The clone.</returns>
    public object Clone() {
      return Auxiliary.Clone(this, new Dictionary<Guid, object>());
    }
    /// <summary>
    /// Clones the current instance with the <see cref="M:Activator.CreateInstance"/> 
    /// method of <see cref="Activator"/>.
    /// </summary>
    /// <param name="clonedObjects">All already cloned objects.</param>
    /// <returns>The clone.</returns>
    public virtual object Clone(IDictionary<Guid, object> clonedObjects) {
      object clone = Activator.CreateInstance(this.GetType());
      clonedObjects.Add(Guid, clone);
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The type of the current instance is saved as <see cref="XmlAttribute"/> with tag name
    /// <c>Type</c>, the guid is also saved as an attribute with the tag name <c>GUID</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
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
    /// <summary>
    /// Loads the persisted object from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>Loads only guid; type,... already loaded by the <see cref="PersistenceManager"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the object is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public virtual void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      myGuid = new Guid(node.Attributes["GUID"].Value);
    }
  }
}
