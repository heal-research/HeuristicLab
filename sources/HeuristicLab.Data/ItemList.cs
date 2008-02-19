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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public class ItemList : ItemBase, IList<IItem> {
    private List<IItem> list;

    private Type myItemType;
    public Type ItemType {
      get { return myItemType; }
      set {
        if (value == null) value = typeof(IItem);
        if (value != myItemType) {
          if (!typeof(IItem).IsAssignableFrom(value))
            throw new InvalidOperationException("Cannot set item type. Only types compatible to IItem are allowed.");
          foreach (IItem item in list) {
            if (!value.IsAssignableFrom(item.GetType()))
              throw new InvalidOperationException("Cannot set item type. Items are incompatible.");
          }
          myItemType = value;
          OnItemTypeChanged();
        }
      }
    }

    public ItemList() {
      list = new List<IItem>();
      myItemType = typeof(IItem);
    }

    public override IView CreateView() {
      return new ItemListView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemList clone = new ItemList();
      clonedObjects.Add(Guid, clone);
      clone.myItemType = ItemType;
      for (int i = 0; i < list.Count; i++)
        clone.list.Add((IItem)Auxiliary.Clone(list[i], clonedObjects));
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      if (ItemType != null) {
        XmlAttribute itemTypeAttribute = document.CreateAttribute("ItemType");
        string typeString = ItemType.AssemblyQualifiedName;
        string[] tokens = typeString.Split(new string[] { ", " }, StringSplitOptions.None);
        typeString = tokens[0] + ", " + tokens[1];
        itemTypeAttribute.Value = typeString;
        node.Attributes.Append(itemTypeAttribute);
      }
      for (int i = 0; i < list.Count; i++)
        node.AppendChild(PersistenceManager.Persist(list[i], document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlAttribute itemTypeAttribute = node.Attributes["ItemType"];
      if (itemTypeAttribute != null)
        myItemType = Type.GetType(itemTypeAttribute.Value);
      for (int i = 0; i < node.ChildNodes.Count; i++)
        list.Add((IItem)PersistenceManager.Restore(node.ChildNodes[i], restoredObjects));
    }

    public override string ToString() {
      if (list.Count > 0) {
        StringBuilder builder = new StringBuilder();
        builder.Append(list[0].ToString());
        for (int i = 1; i < list.Count; i++) {
          builder.Append(";");
          builder.Append(list[i].ToString());
        }
        return builder.ToString();
      } else {
        return "Empty List";
      }
    }

    #region IList<IItem> Members
    public int IndexOf(IItem item) {
      return list.IndexOf(item);
    }
    public void Insert(int index, IItem item) {
      if ((ItemType != null) && (!ItemType.IsAssignableFrom(item.GetType())))
        throw new InvalidOperationException("item type is incompatible to required item type");
      list.Insert(index, item);
      OnItemAdded(item, index);
    }
    public void RemoveAt(int index) {
      IItem item = list[index];
      list.RemoveAt(index);
      OnItemRemoved(item, index);
    }
    public IItem this[int index] {
      get { return list[index]; }
      set {
        if ((ItemType != null) && (!ItemType.IsAssignableFrom(value.GetType())))
          throw new InvalidOperationException("item type is incompatible to required item type");
        list[index] = value;
      }
    }
    #endregion

    #region ICollection<IItem> Members
    public void Add(IItem item) {
      if ((ItemType != null) && (!ItemType.IsAssignableFrom(item.GetType())))
        throw new InvalidOperationException("item type is incompatible to required item type");
      list.Add(item);
      OnItemAdded(item, list.Count - 1);
    }
    public void Clear() {
      list.Clear();
      OnCleared();
    }
    public bool Contains(IItem item) {
      return list.Contains(item);
    }
    public void CopyTo(IItem[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    public int Count {
      get { return list.Count; }
    }
    public bool IsReadOnly {
      get { return false; }
    }
    public bool Remove(IItem item) {
      int index = list.IndexOf(item);
      if (list.Remove(item)) {
        OnItemRemoved(item, index);
        return true;
      } else {
        return false;
      }
    }
    #endregion

    #region IEnumerable<IItem> Members
    public IEnumerator<IItem> GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    public event EventHandler ItemTypeChanged;
    protected virtual void OnItemTypeChanged() {
      if (ItemTypeChanged != null)
        ItemTypeChanged(this, new EventArgs());
      OnChanged();
    }
    public event EventHandler<ItemIndexEventArgs> ItemAdded;
    protected virtual void OnItemAdded(IItem item, int index) {
      if (ItemAdded != null)
        ItemAdded(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    public event EventHandler<ItemIndexEventArgs> ItemRemoved;
    protected virtual void OnItemRemoved(IItem item, int index) {
      if (ItemRemoved != null)
        ItemRemoved(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    public event EventHandler Cleared;
    protected virtual void OnCleared() {
      if (Cleared != null)
        Cleared(this, new EventArgs());
      OnChanged();
    }
  }
}
