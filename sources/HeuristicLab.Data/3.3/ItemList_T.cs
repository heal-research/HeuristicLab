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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Data {
  /// <summary>
  /// Represents a list of items where the items are of the type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The type of the items in this list. <paramref name="T"/> must implement <see cref="IItem"/>.</typeparam>
  public class ItemList<T> : ItemBase, IList<T> where T : IItem {

    [Storable]
    private List<T> list;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ItemList&lt;T&gt;"/>.
    /// </summary>
    public ItemList() {
      list = new List<T>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ItemListView&lt;T&gt;"/>.
    /// </summary>
    /// <returns>The created instance as <see cref="ItemListView&lt;T&gt;"/>.</returns>
    public override IView CreateView() {
      return new ItemListView<T>(this);
    }

    /// <summary>
    /// Clones the current instance.
    /// </summary>
    /// <remarks>Saves the cloned instance in the dictionary <paramref name="clonedObjects"/>.</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="ItemList&lt;T&gt;"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemList<T> clone = new ItemList<T>();
      clonedObjects.Add(Guid, clone);
      CloneElements(clone, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Clones all elements in the current list.
    /// </summary>
    /// <remarks>Clones only elements that have not already been cloned 
    /// (and therefore exist in the dictionary <paramref name="clonedObjects"/>).</remarks>
    /// <param name="destination">The <see cref="ItemList&lt;T&gt;"/> where to save the cloned objects.</param>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    protected void CloneElements(ItemList<T> destination, IDictionary<Guid, object> clonedObjects) {
      for (int i = 0; i < list.Count; i++)
        destination.list.Add((T) Auxiliary.Clone(list[i], clonedObjects));
    }

    /// <summary>
    /// The string representation of the list.
    /// </summary>
    /// <returns>The elements of the list as string, each element separated by a semicolon. <br/>
    /// If the list is empty, "Empty List" is returned.</returns>
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

    #region IList<T> Members
    /// <inheritdoc cref="List&lt;T&gt;.IndexOf(T)"/>
    public int IndexOf(T item) {
      return list.IndexOf(item);
    }
    ///// <summary>
    ///// Inserts a specified <paramref name="item"/> at a specific position <paramref name="index"/>.
    ///// </summary>
    ///// <remarks>Calls <see cref="M:ItemList&lt;T&gt;.OnItemAdded"/>.</remarks>
    ///// <param name="index">The position where to insert the <paramref name="item"/>.</param>
    ///// <param name="item">The element to insert.</param>
    /// <inheritdoc cref="List&lt;T&gt;.Insert"/>
    /// <remarks>Calls <see cref="OnItemAdded"/>.</remarks>
    public void Insert(int index, T item) {
      list.Insert(index, item);
      OnItemAdded(item, index);
    }
    ///// <summary>
    ///// Removes the element at the specified <paramref name="index"/>.
    ///// </summary>
    ///// <remarks>Calls <see cref="M:ItemList&lt;T&gt;.OnItemRemoved"/>.</remarks>
    ///// <param name="index">The position where to remove the element.</param>
    /// <inheritdoc cref="List&lt;T&gt;.RemoveAt"/>
    /// <remarks>Calls <see cref="OnItemRemoved"/>.</remarks>
    public void RemoveAt(int index) {
      IItem item = list[index];
      list.RemoveAt(index);
      OnItemRemoved(item, index);
    }
    /// <summary>
    /// Gets or sets the element at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position where to get or set the element.</param>
    /// <returns>The element at the specified <paramref name="index"/>.</returns>
    public T this[int index] {
      get { return list[index]; }
      set { list[index] = value; }
    }
    #endregion

    #region ICollection<T> Members
    ///// <summary>
    ///// Adds an item to the current list.
    ///// </summary>
    ///// <remarks>Calls <see cref="M:ItemList&lt;T&gt;.OnItemAdded"/>.</remarks>
    ///// <param name="item">The element to add.</param>
    /// <inheritdoc cref="List&lt;T&gt;.Add"/>
    /// <remarks>Calls <see cref="OnItemAdded"/>.</remarks>
    public void Add(T item) {
      list.Add(item);
      OnItemAdded(item, list.Count - 1);
    }
    ///// <summary>
    ///// Empties the list.
    ///// </summary>
    ///// <remarks>Calls <see cref="M:ItemList&lt;T&gt;.OnCleared"/>.</remarks>
    /// <inheritdoc cref="List&lt;T&gt;.Clear"/>
    /// <remarks>Calls <see cref="OnCleared"/>.</remarks>
    public void Clear() {
      list.Clear();
      OnCleared();
    }
    /// <inheritdoc cref="List&lt;T&gt;.Contains"/>
    public bool Contains(T item) {
      return list.Contains(item);
    }
    /// <inheritdoc cref="List&lt;T&gt;.CopyTo(T[], int)"/>
    public void CopyTo(T[] array, int arrayIndex) {
      list.CopyTo(array, arrayIndex);
    }
    /// <inheritdoc cref="List&lt;T&gt;.Count"/>
    public int Count {
      get { return list.Count; }
    }
    /// <summary>
    /// Checks whether the current list is read-only.
    /// </summary>
    /// <remarks>Always returns <c>false</c>.</remarks>
    public bool IsReadOnly {
      get { return false; }
    }
    /// <summary>
    /// Removes the specified <paramref name="item"/>.
    /// </summary>
    /// <remarks>If the <paramref name="item"/> can be successfully removed, 
    /// <see cref="OnItemRemoved"/> is called.</remarks>
    /// <param name="item">The element to remove.</param>
    /// <returns><c>true</c>, if the element could be removed successfully, <c>false</c> otherwise.</returns>
    public bool Remove(T item) {
      int index = list.IndexOf(item);
      if (list.Remove(item)) {
        OnItemRemoved(item, index);
        return true;
      } else {
        return false;
      }
    }
    #endregion

    #region IEnumerable<T> Members
    /// <inheritdoc cref="List&lt;T&gt;.GetEnumerator"/>
    public IEnumerator<T> GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    /// <inheritdoc cref="List&lt;T&gt;.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() {
      return list.GetEnumerator();
    }
    #endregion

    #region List<T> Methods
    /// <inheritdoc cref="List&lt;T&gt;.LastIndexOf(T)"/>
    public int LastIndexOf(T item) {
      return list.LastIndexOf(item);
    }

    /// <inheritdoc cref="List&lt;T&gt;.LastIndexOf(T, int)"/>
    public int LastIndexOf(T item, int index) {
      return list.LastIndexOf(item, index);
    }

    /// <inheritdoc cref="List&lt;T&gt;.LastIndexOf(T, int, int)"/>
    public int LastIndexOf(T item, int index, int count) {
      return list.LastIndexOf(item, index, count);
    }

    /// <inheritdoc cref="List&lt;T&gt;.IndexOf(T, int)"/>
    public int IndexOf(T item, int index) {
      return list.IndexOf(item, index);
    }

    /// <inheritdoc cref="List&lt;T&gt;.IndexOf(T, int, int)"/>
    public int IndexOf(T item, int index, int count) {
      return list.IndexOf(item, index, count);
    }
    /// <summary>
    /// Adds all the elements in the specified <paramref name="collection"/> to the current list.
    /// </summary>
    /// <param name="collection">The elements to add to the current list.</param>
    public void AddRange(IEnumerable<T> collection) {
      foreach (T obj in collection) {
        this.Add(obj);
      }
    }

    /// <inheritdoc cref="List&lt;T&gt;.Exists"/>
    public bool Exists(Predicate<T> match) {
      return list.Exists(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.BinarySearch(T)"/>
    public int BinarySearch(T item) {
      return list.BinarySearch(item);
    }

    /// <inheritdoc cref="List&lt;T&gt;.BinarySearch(T, System.Collections.Generic.IComparer&lt;T&gt;)"/>
    public int BinarySearch(T item, IComparer<T> comparer) {
      return list.BinarySearch(item, comparer);
    }

    /// <inheritdoc cref="List&lt;T&gt;.BinarySearch(int, int, T, System.Collections.Generic.IComparer&lt;T&gt;)"/>
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer) {
      return list.BinarySearch(index, count, item, comparer);
    }
    /// <inheritdoc cref="List&lt;T&gt;.Find"/>
    public T Find(Predicate<T> match) {
      return list.Find(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.FindAll"/>
    public List<T> FindAll(Predicate<T> match) {
      return list.FindAll(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.FindIndex(System.Predicate&lt;T&gt;)"/>
    public int FindIndex(Predicate<T> match) {
      return list.FindIndex(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.FindLast"/>
    public T FindLast(Predicate<T> match) {
      return list.FindLast(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.FindLastIndex(System.Predicate&lt;T&gt;)"/>
    public int FindLastIndex(Predicate<T> match) {
      return list.FindLastIndex(match);
    }

    /// <inheritdoc cref="List&lt;T&gt;.Sort()"/>
    public void Sort() {
      list.Sort();
    }

    /// <inheritdoc cref="List&lt;T&gt;.Sort(System.Collections.Generic.IComparer&lt;T&gt;)"/>
    public void Sort(IComparer<T> comparer) {
      list.Sort(comparer);
    }

    /// <inheritdoc cref="List&lt;T&gt;.Sort(System.Comparison&lt;T&gt;)"/>
    public void Sort(Comparison<T> comparison) {
      list.Sort(comparison);
    }

    /// <inheritdoc cref="List&lt;T&gt;.Reverse()"/>
    public void Reverse() {
      list.Reverse();
    }

    /// <summary>
    /// Converts all elements in the current list to a specified type <typeparamref name="TOutput"/>.
    /// </summary>
    /// <typeparam name="TOutput">The type to convert the items to, which must implement <see cref="IItem"/>.</typeparam>
    /// <param name="converter">A delegate that converts elements from type <c>T</c> to type <typeparamref name="TOutput"/>.</param>
    /// <returns>A list containing the converted elements.</returns>
    public ItemList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) where TOutput : IItem {
      ItemList<TOutput> targetList = new ItemList<TOutput>();
      foreach (T item in list) {
        targetList.Add(converter.Invoke(item));
      }
      return targetList;
    }

    /// <inheritdoc cref="List&lt;T&gt;.TrueForAll"/>
    public bool TrueForAll(Predicate<T> match) {
      return list.TrueForAll(match);
    }

    #endregion

    /// <summary>
    /// Occurs where a new item is added to the list.
    /// </summary>
    public event EventHandler<EventArgs<IItem, int>> ItemAdded;
    /// <summary>
    /// Fires a new <c>ItemAdded</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="item">The added element.</param>
    /// <param name="index">The position where the new element was added.</param>
    protected virtual void OnItemAdded(IItem item, int index) {
      if (ItemAdded != null)
        ItemAdded(this, new EventArgs<IItem, int>(item, index));
      OnChanged();
    }
    /// <summary>
    /// Occurs when an element is deleted from the list.
    /// </summary>
    public event EventHandler<EventArgs<IItem, int>> ItemRemoved;
    /// <summary>
    /// Fires a new <c>ItemRemoved</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="item">The removed element.</param>
    /// <param name="index">The position from where the element was removed.</param>
    protected virtual void OnItemRemoved(IItem item, int index) {
      if (ItemRemoved != null)
        ItemRemoved(this, new EventArgs<IItem, int>(item, index));
      OnChanged();
    }
    /// <summary>
    /// Occurs when the list is emptied.
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
