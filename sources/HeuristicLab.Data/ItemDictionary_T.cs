using System;
using System.Collections; 
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public class ItemDictionary<K,V> : ItemBase, IDictionary<K,V> 
    where K : IItem
    where V : IItem{
    private Dictionary<K, V> dict;

    public Dictionary<K, V> Dictionary {
      get { return dict; }
    }

    public ItemDictionary() {
      dict = new Dictionary<K, V>(new IItemKeyComparer<K>());
    }

    #region ItemBase Members
    public override IView CreateView() {
      return new ItemDictionaryView<K,V>(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ItemDictionary<K,V> clone = new ItemDictionary<K,V>();
      clonedObjects.Add(Guid, clone);
      foreach (KeyValuePair<K, V> item in dict) {
        clone.dict.Add((K) Auxiliary.Clone(item.Key, clonedObjects), (V) Auxiliary.Clone(item.Value, clonedObjects));
      }
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      foreach (KeyValuePair<K, V> item in dict) {
        XmlNode keyNode = PersistenceManager.Persist("Key", item.Key, document, persistedObjects);
        XmlNode valueNode = PersistenceManager.Persist("Val", item.Value, document, persistedObjects); 
        XmlNode pairNode = document.CreateNode(XmlNodeType.Element, "KeyValuePair", null); 
        pairNode.AppendChild(keyNode); 
        pairNode.AppendChild(valueNode); 
        node.AppendChild(pairNode); 
      }
      return node; 
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      for (int i = 0; i < node.ChildNodes.Count; i++) {
        K key = (K) PersistenceManager.Restore(node.ChildNodes[i].SelectSingleNode("Key"), restoredObjects);
        V val = (V) PersistenceManager.Restore(node.ChildNodes[i].SelectSingleNode("Val"), restoredObjects);
        dict[key] = val; 
      }
    }

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

    public void Add(K key, V value) {
      dict.Add(key, value);
      OnItemAdded(key, value);
    }

    public bool ContainsKey(K key) {
      return dict.ContainsKey(key);
    }

    public ICollection<K> Keys {
      get { return dict.Keys; }
    }

    public bool Remove(K key) {
      V value = dict[key]; 
      bool removed = dict.Remove(key);
      OnItemRemoved(key, value); 
      return removed; 
    }

    public bool TryGetValue(K key, out V value) {
      return dict.TryGetValue(key, out value);
    }

    public ICollection<V> Values {
      get { return dict.Values; }
    }

    public V this[K key] {
      get { return dict[key]; }
      set { dict[key] = value; }
    }

    #endregion

    #region ICollection<KeyValuePair<K,V>> Members

    public void Add(KeyValuePair<K, V> item) {
      dict.Add(item.Key, item.Value);
      OnItemAdded(item.Key, item.Value); 
    }

    public void Clear() {
      dict.Clear();
      OnCleared(); 
    }

    public bool Contains(KeyValuePair<K, V> item) {
      return (dict.ContainsKey(item.Key) && dict[item.Key].Equals(item.Value));
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
      throw new NotImplementedException(); 
    }

    public int Count {
      get { return dict.Count; }
    }

    public bool IsReadOnly {
      get { return false; }
    }

    public bool Remove(KeyValuePair<K, V> item) {
      bool removed = dict.Remove(item.Key);
      if (removed) {
        OnItemRemoved(item.Key, item.Value);
      } 
      return removed; 
    }
    #endregion

    #region IEnumerable<KeyValuePair<K,V>> Members
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
      return dict.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() {
      return dict.GetEnumerator();
    }
    #endregion

    #region Event Handler
    public event EventHandler<KeyValueEventArgs> ItemAdded;
    protected virtual void OnItemAdded(K key, V value) {
      if (ItemAdded != null)
        ItemAdded(this, new KeyValueEventArgs(key, value));
      OnChanged();
    }

    public event EventHandler<KeyValueEventArgs> ItemRemoved;
    protected virtual void OnItemRemoved(K key, V value) {
      if (ItemRemoved != null)
        ItemRemoved(this, new KeyValueEventArgs(key, value));
      OnChanged();
    }

    public event EventHandler Cleared;
    protected virtual void OnCleared() {
      if (Cleared != null)
        Cleared(this, new EventArgs());
      OnChanged();
    }
    #endregion

    #region IEqualityComparer
    internal sealed class IItemKeyComparer<T> : IEqualityComparer<T>
        where T : IItem {
      public bool Equals(T x, T y) {
        if (x is IComparable) {
          return (((IComparable) x).CompareTo(y) == 0); 
        }
        if (y is IComparable) {
          return (((IComparable) y).CompareTo(x) == 0); 
        }
        return x.Equals(y); 
      }

      public int GetHashCode(T obj) {
        if (obj is IObjectData) {
          return ((IObjectData) obj).Data.GetHashCode(); 
        }
        return obj.Guid.GetHashCode(); 
      }
    }
    #endregion
  }
}
