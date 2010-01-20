#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace HeuristicLab.Common {
  [Obsolete("Use collections of the HeuristicLab.Collections plugin instead", false)]
  public class NotificationDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
    
    public NotificationDictionary() {
      this.myDictionary = new Dictionary<TKey, TValue>(); 
    }

    #region Event Handler

    public event EventHandler BeforeInsertEvent;
    public event EventHandler AfterInsertEvent;
    public event EventHandler BeforeRemoveEvent;
    public event EventHandler AfterRemoveEvent;
    public event EventHandler ChangeEvent;
    public event EventHandler ClearEvent;

    private Dictionary<TKey, TValue> myDictionary; 

    protected virtual void OnBeforeInsert() {
      if (BeforeInsertEvent != null)
        BeforeInsertEvent(this, new EventArgs());
    }

    protected virtual void OnAfterInsert() {
      if (AfterInsertEvent != null)
        AfterInsertEvent(this, new EventArgs());
    }

    protected virtual void OnBeforeRemove() {
      if (BeforeRemoveEvent != null)
        BeforeRemoveEvent(this, new EventArgs());
    }

    protected virtual void OnAfterRemove() {
      if (AfterRemoveEvent != null)
        AfterRemoveEvent(this, new EventArgs());
    }

    protected virtual void OnChange() {
      if (ChangeEvent != null)
        ChangeEvent(this, new EventArgs());
    }

    protected virtual void OnClear() {
      if (ClearEvent != null)
        ClearEvent(this, new EventArgs());
    }

    #endregion

    #region IDictionary<TKey,TValue> Members

    public void Add(TKey key, TValue value) {
      OnBeforeInsert(); 
      myDictionary.Add(key, value);
      OnAfterInsert();
    }

    public bool ContainsKey(TKey key) {
      return myDictionary.ContainsKey(key);
    }

    public ICollection<TKey> Keys {
      get { return myDictionary.Keys; }
    }

    public bool Remove(TKey key) {
      OnBeforeRemove(); 
      bool returnVal = myDictionary.Remove(key);
      OnAfterRemove();
      return returnVal;
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return TryGetValue(key, out value); 
    }

    public ICollection<TValue> Values {
      get { return myDictionary.Values; }
    }

    public TValue this[TKey key] {
      get {
        return myDictionary[key];
      }
      set {
        if (!value.Equals(myDictionary[key])) {
          myDictionary[key] = value;
          OnChange(); 
        }
      }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, TValue> item) {
      OnBeforeInsert();
      myDictionary.Add(item.Key, item.Value);
      OnAfterInsert();
    }

    public void Clear() {
      myDictionary.Clear();
      OnClear(); 
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return myDictionary.Contains(item); 
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      ((IDictionary<TKey, TValue>)myDictionary).CopyTo(array, arrayIndex); 
    }

    public int Count {
      get { return myDictionary.Count; }
    }

    public bool IsReadOnly {
      get { return false; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
      OnBeforeRemove(); 
      bool returnVal = myDictionary.Remove(item.Key);
      OnAfterRemove();
      return returnVal;
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      return myDictionary.GetEnumerator(); 
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return myDictionary.GetEnumerator(); 
    }

    #endregion
  }
}
