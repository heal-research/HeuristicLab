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
  /// A dictionary having key-value pairs of the type <see cref="IItem"/>.
  /// </summary>
  /// <typeparam name="K">The type of the keys, which must implement the interface <see cref="IItem"/>.</typeparam>
  /// <typeparam name="V">The type of the values, which must imlement the interface <see cref="IItem"/>.</typeparam>
  public class ItemDictionary<K,V> : ItemBase, IDictionary<K,V> 
    where K : IItem
    where V : IItem {

    [Storable]
    private Dictionary<K, V> dict;

    /// <summary>
    /// Gets the dictionary of key-value pairs.
    /// </summary>
    public Dictionary<K, V> Dictionary {
      get { return dict; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemDictionary&lt;TKey,TValue&gt;"/>.
    /// </summary>
    /// <remarks>Creates a new <see cref="Dictionary"/> 
    /// having <see cref="IItem"/> as type of keys and values 
    /// and a new <see cref="IItemKeyComparer&lt;T&gt;"/> as <see cref="IEqualityComparer"/>.</remarks>
    public ItemDictionary() {
      dict = new Dictionary<K, V>(new IItemKeyComparer<K>());
    }

    #region ItemBase Members
    /// <summary>
    /// Clones the current instance and adds it to the dictionary <paramref name="clonedObjects"/>.
    /// </summary>
    /// <remarks>Also the keys and values in the dictionary are cloned and saved to the dictionary <paramref name="clonedObjects"/>,
    /// when they are not already contained (deep copy).</remarks>
    /// <param name="clonedObjects">A dictionary of all already cloned objects.</param>
    /// <returns>The cloned instance as <see cref="ItemDictionary&lt;K,V&gt;"/>.</returns>
    public override IItem Clone(ICloner cloner) {
      ItemDictionary<K,V> clone = new ItemDictionary<K,V>();
      cloner.RegisterClonedObject(this, clone);
      foreach (KeyValuePair<K, V> item in dict) {
        clone.dict.Add((K) cloner.Clone(item.Key), (V) cloner.Clone(item.Value));
      }
      return clone;
    }

    /// <summary>
    /// The string representation of the dictionary
    /// </summary>
    /// <returns>The elements of the dictionary as string, each key-value pair in the format 
    /// <c>Key:Value</c>, separated by a semicolon. <br/>
    /// If the dictionary contains no entries, "Empty Dictionary" is returned.</returns>
    public override string ToString() {
      if (dict.Count > 0) {
        StringBuilder builder = new StringBuilder();
        foreach (KeyValuePair<K, V> item in dict) {
          builder.Append(item.Key.ToString());
          builder.Append(":");
          builder.Append(item.Value.ToString());
          builder.Append("; ");
        }
        return builder.ToString();
      } else {
        return "Empty Dictionary";
      }
    }
    #endregion

    #region IDictionary<K,V> Members

    ///// <summary>
    ///// Adds a new key value pair to the dictionary.
    ///// </summary>
    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.Add"/>
    /// <remarks>Calls <see cref="OnItemAdded"/>.</remarks>
    ///// <param name="key">The key to add.</param>
    ///// <param name="value">The value to add.</param>
    public void Add(K key, V value) {
      dict.Add(key, value);
      OnItemAdded(key, value);
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;.ContainsKey"/>
    public bool ContainsKey(K key) {
      return dict.ContainsKey(key);
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;.Keys"/>
    public ICollection<K> Keys {
      get { return dict.Keys; }
    }

    /// <summary>
    /// Removes a key-value pair having the specified <paramref name="key"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemRemoved"/>.</remarks>
    /// <param name="key">The key of the key-value pair, which should be removed.</param>
    /// <returns><c>true</c> if the key was found and successfully removed, 
    /// <c>false</c> if the key was not found.</returns>
    public bool Remove(K key) {
      V value = dict[key]; 
      bool removed = dict.Remove(key);
      OnItemRemoved(key, value); 
      return removed; 
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.TryGetValue"/>
    public bool TryGetValue(K key, out V value) {
      return dict.TryGetValue(key, out value);
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;.Values"/>
    public ICollection<V> Values {
      get { return dict.Values; }
    }

    /// <summary>
    /// Gets or sets the value of a specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the value which should be received or changed.</param>
    /// <returns>The value of the <paramref name="key"/>.</returns>
    public V this[K key] {
      get { return dict[key]; }
      set { dict[key] = value; }
    }

    #endregion

    #region ICollection<KeyValuePair<K,V>> Members

    /// <summary>
    /// Adds a key-value pair to the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemAdded"/>.</remarks>
    /// <param name="item">The key-value pair to add.</param>
    public void Add(KeyValuePair<K, V> item) {
      dict.Add(item.Key, item.Value);
      OnItemAdded(item.Key, item.Value); 
    }

    ///// <summary>
    ///// Empties the dictionary.
    ///// </summary>
    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.Clear"/>
    /// <remarks>Calls <see cref="OnCleared"/>.</remarks>
    public void Clear() {
      dict.Clear();
      OnCleared(); 
    }

    /// <summary>
    /// Checks whether the specified key-value pair exists in the current instance of the dictionary.
    /// </summary>
    /// <param name="item">The key-value pair to check.</param>
    /// <returns><c>true</c> if both, the key and the value exist in the dictionary, 
    /// <c>false</c> otherwise.</returns>
    public bool Contains(KeyValuePair<K, V> item) {
      return (dict.ContainsKey(item.Key) && dict[item.Key].Equals(item.Value));
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
      throw new NotImplementedException(); 
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.Count"/>
    public int Count {
      get { return dict.Count; }
    }

    /// <summary>
    /// Checks whether the dictionary is read-only.
    /// </summary>
    /// <remarks>Always returns <c>false</c>.</remarks>
    public bool IsReadOnly {
      get { return false; }
    }

    /// <summary>
    /// Removes the specified key-value pair.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemRemoved"/> when the removal was successful.</remarks>
    /// <param name="item">The key-value pair to remove.</param>
    /// <returns><c>true</c> if the removal was successful, <c>false</c> otherwise.</returns>
    public bool Remove(KeyValuePair<K, V> item) {
      bool removed = dict.Remove(item.Key);
      if (removed) {
        OnItemRemoved(item.Key, item.Value);
      } 
      return removed; 
    }
    #endregion

    #region IEnumerable<KeyValuePair<K,V>> Members
    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.GetEnumerator"/>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
      return dict.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    /// <inheritdoc cref="System.Collections.Generic.Dictionary&lt;K,V&gt;.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() {
      return dict.GetEnumerator();
    }
    #endregion

    #region Event Handler
    /// <summary>
    /// Occurs when a new item is added to the dictionary.
    /// </summary>
    public event EventHandler<EventArgs<IItem, IItem>> ItemAdded;
    /// <summary>
    /// Fires a new <c>ItemAdded</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="key">The key that was added.</param>
    /// <param name="value">The value that was added.</param>
    protected virtual void OnItemAdded(K key, V value) {
      if (ItemAdded != null)
        ItemAdded(this, new EventArgs<IItem, IItem>(key, value));
      OnChanged();
    }

    /// <summary>
    /// Occurs when an item is removed from the dictionary.
    /// </summary>
    public event EventHandler<EventArgs<IItem, IItem>> ItemRemoved;
    /// <summary>
    /// Fires a new <c>ItemRemoved</c> event.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ItemBase.OnChanged"/>.</remarks>
    /// <param name="key">The key that was removed.</param>
    /// <param name="value">The value that was removed</param>
    protected virtual void OnItemRemoved(K key, V value) {
      if (ItemRemoved != null)
        ItemRemoved(this, new EventArgs<IItem, IItem>(key, value));
      OnChanged();
    }

    /// <summary>
    /// Occurs when the dictionary is emptied.
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
    #endregion

    #region IEqualityComparer
    /// <summary>
    /// Compares two keys with each other.
    /// </summary>
    /// <typeparam name="T">The type of the keys.</typeparam>
    internal sealed class IItemKeyComparer<T> : IEqualityComparer<T>
        where T : IItem {
      /// <summary>
      /// Checks whether two keys are equal to each other.
      /// </summary>
      /// <param name="x">Key number one.</param>
      /// <param name="y">Key number two.</param>
      /// <returns><c>true</c> if the two keys are the same, <c>false</c> otherwise.</returns>
      public bool Equals(T x, T y) {
        if (x is IComparable) {
          return (((IComparable) x).CompareTo(y) == 0); 
        }
        if (y is IComparable) {
          return (((IComparable) y).CompareTo(x) == 0); 
        }
        return x.Equals(y); 
      }

      /// <summary>
      /// Serves as a hash function for a particular type.
      /// </summary>
      /// <param name="obj">The object where the hash code is searched for.</param>
      /// <returns>A hash code for the given <paramref name="obj"/>.</returns>
      public int GetHashCode(T obj) {
        if (obj is IObjectData) {
          return ((IObjectData) obj).Data.GetHashCode(); 
        }
        return obj.GetHashCode(); 
      }
    }
    #endregion
  }
}
