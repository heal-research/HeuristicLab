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
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// Constraint that limits the type of a given item.
  /// </summary>
  /// <remarks>If the item is a <see cref="ConstrainedItemList"/>, any containing elements are limited to 
  /// the type and not the <see cref="ConstrainedItemList"/> itself.</remarks>
  public class ItemTypeConstraint : ConstraintBase {
    private Type type;
    /// <summary>
    /// Gets or sets the type to which the items should be limited.
    /// </summary>
    /// <remarks>Calls <see cref="ItemBase.OnChanged"/> of base class <see cref="ConstraintBase"/>
    /// in the setter.</remarks>
    public Type Type {
      get { return type; }
      set {
        type = value;
        OnChanged();
      }
    }

    /// <inheritdoc select="summary"/>
    public override string Description {
      get {
        return @"The ItemTypeConstraint limits the type of a given item.
If the item is a ConstrainedItemList, any containing elements are limited to the type and not the ConstrainedItemList itself.";
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraint"/> with the <c>Type</c> property
    /// set to <see cref="ItemBase"/> as default.
    /// </summary>
    public ItemTypeConstraint() {
      type = typeof(ItemBase);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraint"/> with the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type the items should be limited to.</param>
    public ItemTypeConstraint(Type type) {
      this.type = type;
    }

    /// <summary>
    /// Checks whether the given element fulfills the current constraint.
    /// </summary>
    /// <param name="data">The item to check.</param>
    /// <returns><c>true</c> if the constraint could be fulfilled, <c>false</c> otherwise.</returns>
    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list != null) {
        for (int i = 0; i < list.Count; i++)
          if (!list[i].GetType().Equals(type)) return false;
        return true;
      }
      return data.GetType().Equals(type);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ItemTypeConstraintView"/> to represent the current 
    /// instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="ItemTypeConstraintView"/>.</returns>
    public override IView CreateView() {
      return new ItemTypeConstraintView(this);
    }

    #region clone & persistence
    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="ItemTypeConstraint"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemTypeConstraint clone = new ItemTypeConstraint(type);
      clonedObjects.Add(Guid, clone);
      return clone;
    }

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The type of the current instance is saved as attribute with tag name <c>ItemType</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. 
    /// (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute itemTypeAttribute = document.CreateAttribute("ItemType");
      itemTypeAttribute.Value = PersistenceManager.BuildTypeString(Type);
      node.Attributes.Append(itemTypeAttribute);
      return node;
    }

    /// <summary>
    /// Loads the persisted constraint from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The constraint must be saved in a specific way, see <see cref="GetXmlNode"/> for 
    /// more information.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the instance is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlAttribute itemTypeAttribute = node.Attributes["ItemType"];
      type = Type.GetType(itemTypeAttribute.Value);
    }
    #endregion
  }
}
