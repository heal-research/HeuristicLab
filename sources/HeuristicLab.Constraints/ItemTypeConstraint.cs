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
  public class ItemTypeConstraint : ConstraintBase {
    private Type type;
    public Type Type {
      get { return type; }
      set {
        type = value;
        OnChanged();
      }
    }

    public override string Description {
      get {
        return @"The ItemTypeConstraint limits the type of a given item.
If the item is a ConstrainedItemList, any containing elements are limited to the type and not the ConstrainedItemList itself.";
      }
    }

    public ItemTypeConstraint() {
      type = typeof(ItemBase);
    }

    public ItemTypeConstraint(Type type) {
      this.type = type;
    }

    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list != null) {
        for (int i = 0; i < list.Count; i++)
          if (!list[i].GetType().Equals(type)) return false;
        return true;
      }
      return data.GetType().Equals(type);
    }

    public override IView CreateView() {
      return new ItemTypeConstraintView(this);
    }

    public override void Accept(IConstraintVisitor visitor) {
      visitor.Visit(this);
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemTypeConstraint clone = new ItemTypeConstraint(type);
      clonedObjects.Add(Guid, clone);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute itemTypeAttribute = document.CreateAttribute("ItemType");
      string typeString = Type.AssemblyQualifiedName;
      string[] tokens = typeString.Split(new string[] { ", " }, StringSplitOptions.None);
      typeString = tokens[0] + ", " + tokens[1];
      itemTypeAttribute.Value = typeString;
      node.Attributes.Append(itemTypeAttribute);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlAttribute itemTypeAttribute = node.Attributes["ItemType"];
      type = Type.GetType(itemTypeAttribute.Value);
    }
    #endregion
  }
}
