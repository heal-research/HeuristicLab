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
  public class ConstrainedItemList : ConstrainedItemBase, IEnumerable, IEnumerable<IItem> {
    private List<IItem> list;
    private bool suspendConstraintCheck;

    public ConstrainedItemList()
      : base() {
      list = new List<IItem>();
      suspendConstraintCheck = false;
    }

    public override IView CreateView() {
      return new ConstrainedItemListView(this);
    }

    #region Clone & Persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ConstrainedItemList clone = new ConstrainedItemList();
      clonedObjects.Add(Guid, clone);
      foreach (IConstraint constraint in Constraints)
        clone.AddConstraint((IConstraint)Auxiliary.Clone(constraint, clonedObjects));
      clone.suspendConstraintCheck = suspendConstraintCheck;
      foreach (IItem item in list) {
        clone.list.Add((IItem)Auxiliary.Clone(item, clonedObjects));
      }
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode listNode = document.CreateNode(XmlNodeType.Element, "ListItems", null);
      for (int i = 0; i < list.Count; i++)
        listNode.AppendChild(PersistenceManager.Persist(list[i], document, persistedObjects));
      XmlAttribute sccAttrib = document.CreateAttribute("SuspendConstraintCheck");
      sccAttrib.Value = suspendConstraintCheck.ToString();
      listNode.Attributes.Append(sccAttrib);
      node.AppendChild(listNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode listNode = node.SelectSingleNode("ListItems");
      list = new List<IItem>();
      for (int i = 0; i < listNode.ChildNodes.Count; i++)
        list.Add((IItem)PersistenceManager.Restore(listNode.ChildNodes[i], restoredObjects));
      suspendConstraintCheck = bool.Parse(listNode.Attributes["SuspendConstraintCheck"].Value);
    }
    #endregion

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

    public int IndexOf(IItem item) {
      return list.IndexOf(item);
    }

    public void BeginCombinedOperation() {
      suspendConstraintCheck = true;
    }

    public bool EndCombinedOperation(out ICollection<IConstraint> violatedConstraints) {
      suspendConstraintCheck = false;
      return IsValid(out violatedConstraints);
    }

    public bool TryInsert(int index, IItem item, out ICollection<IConstraint> violatedConstraints) {
      list.Insert(index, item);
      if (!suspendConstraintCheck && IsValid(out violatedConstraints)) {
        OnItemAdded(item, index);
        return true;
      } else {
        violatedConstraints = new List<IConstraint>();
        list.RemoveAt(index);
        return false;
      }
    }

    public bool TryRemoveAt(int index, out ICollection<IConstraint> violatedConstraints) {
      IItem item = list[index];
      list.RemoveAt(index);
      if (!suspendConstraintCheck && IsValid(out violatedConstraints)) {
        OnItemRemoved(item, index);
        return true;
      } else {
        violatedConstraints = new List<IConstraint>();
        list.Insert(index, item);
        return false;
      }
    }

    public IItem this[int index] {
      get { return list[index]; }
    }

    public bool TrySetAt(int index, IItem item, out ICollection<IConstraint> violatedConstraints) {
      IItem backup = this[index];
      list[index] = item;
      if (!suspendConstraintCheck && IsValid(out violatedConstraints)) {
        return true;
      } else {
        violatedConstraints = new List<IConstraint>();
        list[index] = backup;
        return false;
      }
    }

    public bool TryAdd(IItem item, out ICollection<IConstraint> violatedConstraints) {
      list.Add(item);
      if (!suspendConstraintCheck && IsValid(out violatedConstraints)) {
        OnItemAdded(item, list.Count - 1);
        return true;
      } else {
        violatedConstraints = new List<IConstraint>();
        list.RemoveAt(list.Count - 1);
        return false;
      }
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
    public bool TryRemove(IItem item, out ICollection<IConstraint> violatedConstraints) {
      int index = list.IndexOf(item);
      if (index >= 0) {
        return TryRemoveAt(index, out violatedConstraints);
      } else {
        violatedConstraints = new List<IConstraint>();
        return false;
      }
    }

    public IEnumerator<IItem> GetEnumerator() {
      return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
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
