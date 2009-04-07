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
  /// <summary>
  /// A class representing a list of elements 
  /// (which are implementing the interface <see cref="IItem"/>) having some constraints.
  /// </summary>
  public class ConstrainedItemList : ConstrainedItemBase, IEnumerable, IEnumerable<IItem> {
    private List<IItem> list;
    private bool suspendConstraintCheck;

    /// <summary>
    /// Checks whether the test for the constraints is suspended. 
    /// </summary>
    public bool ConstraintCheckSuspended {
      get { return suspendConstraintCheck; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConstrainedItemList"/> with the constraint check enabled.
    /// </summary>
    public ConstrainedItemList()
      : base() {
      list = new List<IItem>();
      suspendConstraintCheck = false;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConstrainedItemListView"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="ConstrainedItemListView"/>.</returns>
    public override IView CreateView() {
      return new ConstrainedItemListView(this);
    }

    #region Clone & Persistence
    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>The elements of the current instance are cloned with the 
    /// <see cref="HeuristicLab.Core.Auxiliary.Clone"/> method of the class <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="ConstrainedItemList"/>.</returns>
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

    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>The basic instance is saved through the call of 
    /// <see cref="HeuristicLab.Core.ConstrainedItemBase.GetXmlNode"/> of base class 
    /// <see cref="ConstrainedItemBase"/>. <br/>
    /// The list itself is saved as child node having the tag name <c>ListItems</c> 
    /// and each element is saved as a child node of the <c>ListItems</c> node. 
    /// The <c>suspendConstraintCheck</c> attribute is saved as an attribute of the list node 
    /// having the tag name <c>SuspendConstraintCheck</c>.</remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where the data is saved.</param>
    /// <param name="persistedObjects">A dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
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

    /// <summary>
    /// Loads the persisted int value from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>The elements of the list must be saved as child nodes of 
    /// the node with the tag name <c>ListItems</c>, which itself is a child node of the current instance.<br/>
    /// The <c>suspendConstraintCheck</c> must be saved as attribute of the list node 
    /// with the tag name <c>SuspendConstraintCheck</c> (see <see cref="GetXmlNode"/>).</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the int is saved.</param>
    /// <param name="restoredObjects">A dictionary of all already restored objects. (Needed to avoid cycles.)</param>
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode listNode = node.SelectSingleNode("ListItems");
      list = new List<IItem>();
      for (int i = 0; i < listNode.ChildNodes.Count; i++)
        list.Add((IItem)PersistenceManager.Restore(listNode.ChildNodes[i], restoredObjects));
      suspendConstraintCheck = bool.Parse(listNode.Attributes["SuspendConstraintCheck"].Value);
    }
    #endregion

    /// <summary>
    /// The string representation of the current list instance.
    /// </summary>
    /// <returns>The current list as string, each element separated by a semicolon. 
    /// "Empty List" if the list has no elements.</returns>
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

    /// <inheritdoc cref="List&lt;T&gt;.IndexOf(T)"/>
    public int IndexOf(IItem item) {
      return list.IndexOf(item);
    }

    /// <summary>
    /// Sets <c>suspendConstraintCheck</c> to <c>true</c>.
    /// </summary>
    public void BeginCombinedOperation() {
      suspendConstraintCheck = true;
    }

    /// <summary>
    /// Checks whether the current instance fulfills all constraints.
    /// </summary>
    /// <param name="violatedConstraints">Output parameter, 
    /// contains all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if all constraints could be fulfilled, <c>false</c> otherwise.</returns>
    public bool EndCombinedOperation(out ICollection<IConstraint> violatedConstraints) {
      if (IsValid(out violatedConstraints))
        suspendConstraintCheck = false;
      else
        suspendConstraintCheck = true;

      return !suspendConstraintCheck;
    }

    /// <summary>
    /// Adds a new <paramref name="item"/> at a specified <paramref name="index"/> to the current instance if all constraints are fulfilled.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemAdded"/> if the insertion was successful.</remarks>
    /// <param name="index">The position where to insert the new element.</param>
    /// <param name="item">The new element to insert.</param>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the insertion was successful, <c>false</c> otherwise.</returns>
    public bool TryInsert(int index, IItem item, out ICollection<IConstraint> violatedConstraints) {
      list.Insert(index, item);
      violatedConstraints = new List<IConstraint>();
      if (suspendConstraintCheck || IsValid(out violatedConstraints)) {
        OnItemAdded(item, index);
        return true;
      } else {
        list.RemoveAt(index);
        return false;
      }
    }

    /// <summary>
    /// Removes an element at the specified <paramref name="index"/> 
    /// from the current instance if all constraints are fulfilled.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemRemoved"/> if the deletion was successful.</remarks>
    /// <param name="index">The position where to remove the element.</param>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the element could be removed successfully, <c>false</c> otherwise.</returns>
    public bool TryRemoveAt(int index, out ICollection<IConstraint> violatedConstraints) {
      IItem item = list[index];
      list.RemoveAt(index);
      violatedConstraints = new List<IConstraint>();
      if (suspendConstraintCheck || IsValid(out violatedConstraints)) {
        OnItemRemoved(item, index);
        return true;
      } else {
        list.Insert(index, item);
        return false;
      }
    }

    /// <summary>
    /// Gets the element of the current instance at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position of the searched element.</param>
    /// <returns>The searched element as <see cref="IItem"/>.</returns>
    public IItem this[int index] {
      get { return list[index]; }
    }

    /// <summary>
    /// Changes the element at a specified position if all constraints are fulfilled.
    /// </summary>
    /// <param name="index">The position where to change the element.</param>
    /// <param name="item">The element that replaces the current one.</param>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the substitution was successful, <c>false</c> otherwise.</returns>
    public bool TrySetAt(int index, IItem item, out ICollection<IConstraint> violatedConstraints) {
      IItem backup = this[index];
      list[index] = item;
      violatedConstraints = new List<IConstraint>();
      if (suspendConstraintCheck || IsValid(out violatedConstraints)) {
        return true;
      } else {
        list[index] = backup;
        return false;
      }
    }

    /// <summary>
    /// Adds a new <paramref name="item"/> to the current list if all constraints are fulfilled.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemAdded"/> if the add was successful.</remarks>
    /// <param name="item">The element to add.</param>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the element could be successfully added, <c>false</c> otherwise.</returns>
    public bool TryAdd(IItem item, out ICollection<IConstraint> violatedConstraints) {
      list.Add(item);
      violatedConstraints = new List<IConstraint>();
      if (suspendConstraintCheck || IsValid(out violatedConstraints)) {
        OnItemAdded(item, list.Count - 1);
        return true;
      } else {
        list.RemoveAt(list.Count - 1);
        return false;
      }
    }
    /// <summary>
    /// Empties the current list.
    /// </summary>
    /// <remarks>Calls <see cref="OnCleared"/>.</remarks>
    public void Clear() {
      list.Clear();
      OnCleared();
    }
    /// <inheritdoc cref="List&lt;T&gt;.Contains"/>
    public bool Contains(IItem item) {
      return list.Contains(item);
    }
    /// <inheritdoc cref="List&lt;T&gt;.CopyTo(T[],int)"/>
    public void CopyTo(IItem[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    /// <inheritdoc cref="List&lt;T&gt;.Count"/>
    public int Count {
      get { return list.Count; }
    }
    /// <summary>
    /// Checks whether the current instance is read-only.
    /// </summary>
    /// <remarks>Always returns <c>false</c>.</remarks>
    public bool IsReadOnly {
      get { return false; }
    }
    /// <summary>
    /// Removes a specified <paramref name="item"/> from the 
    /// current instance if all constraints are fulfilled.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    /// <param name="violatedConstraints">Output parameter, all constraints that could not be fulfilled.</param>
    /// <returns><c>true</c> if the deletion was successful, <c>false</c> otherwise.</returns>
    public bool TryRemove(IItem item, out ICollection<IConstraint> violatedConstraints) {
      int index = list.IndexOf(item);
      if (index >= 0) {
        return TryRemoveAt(index, out violatedConstraints);
      } else {
        violatedConstraints = new List<IConstraint>();
        return false;
      }
    }

    /// <inheritdoc cref="List&lt;T&gt;.GetEnumerator"/>
    public IEnumerator<IItem> GetEnumerator() {
      return list.GetEnumerator();
    }

    /// <inheritdoc cref="List&lt;T&gt;.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
    }

    /// <summary>
    /// Occurs when a new item is added.
    /// </summary>
    public event EventHandler<ItemIndexEventArgs> ItemAdded;
    /// <summary>
    /// Fires a new <c>ItemAdded</c> event.
    /// </summary>         
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="item">The element that was added.</param>
    /// <param name="index">The position where the element was added.</param>
    protected virtual void OnItemAdded(IItem item, int index) {
      if (ItemAdded != null)
        ItemAdded(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    /// <summary>
    /// Occurs when an element is removed from the current instance.
    /// </summary>
    public event EventHandler<ItemIndexEventArgs> ItemRemoved;
    /// <summary>
    /// Fires a new <c>ItemRemoved</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="item">The element that has been removed.</param>
    /// <param name="index">The position from where it has been removed.</param>
    protected virtual void OnItemRemoved(IItem item, int index) {
      if (ItemRemoved != null)
        ItemRemoved(this, new ItemIndexEventArgs(item, index));
      OnChanged();
    }
    /// <summary>
    /// Occurs when the current list is emptied.
    /// </summary>
    public event EventHandler Cleared;
    /// <summary>
    /// Fires a new <c>Cleared</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    protected virtual void OnCleared() {
      if (Cleared != null)
        Cleared(this, new EventArgs());
      OnChanged();
    }
  }
}
