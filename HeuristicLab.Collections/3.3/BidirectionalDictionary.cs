#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public class BidirectionalDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>> {
    [Storable]
    private readonly Dictionary<TFirst, TSecond> firstToSecond;
    [Storable]
    private readonly Dictionary<TSecond, TFirst> secondToFirst;

    [StorableConstructor]
    protected BidirectionalDictionary(bool deserializing) : base() { }
    public BidirectionalDictionary() {
      firstToSecond = new Dictionary<TFirst, TSecond>();
      secondToFirst = new Dictionary<TSecond, TFirst>();
    }
    public BidirectionalDictionary(IEqualityComparer<TFirst> firstComparer)
      : base() {
      firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
      secondToFirst = new Dictionary<TSecond, TFirst>();
    }
    public BidirectionalDictionary(IEqualityComparer<TSecond> secondComparer)
      : base() {
      firstToSecond = new Dictionary<TFirst, TSecond>();
      secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
    }
    public BidirectionalDictionary(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer)
      : base() {
      firstToSecond = new Dictionary<TFirst, TSecond>(firstComparer);
      secondToFirst = new Dictionary<TSecond, TFirst>(secondComparer);
    }
    public BidirectionalDictionary(BidirectionalDictionary<TFirst, TSecond> other)
      : base() {
      firstToSecond = new Dictionary<TFirst, TSecond>(other.firstToSecond, other.firstToSecond.Comparer);
      secondToFirst = new Dictionary<TSecond, TFirst>(other.secondToFirst, other.secondToFirst.Comparer);
    }

    #region Properties
    public int Count {
      get { return firstToSecond.Count; }
    }

    public IEnumerable<TFirst> FirstKeys {
      get { return firstToSecond.Keys; }
    }

    public IEnumerable<TSecond> SecondKeys {
      get { return secondToFirst.Keys; }
    }
    #endregion

    #region Methods
    public void Add(TFirst firstValue, TSecond secondValue) {
      if (firstToSecond.ContainsKey(firstValue))
        throw new ArgumentException("Could not add first value " + firstValue.ToString() + " because it is already contained in the bidirectional dictionary.");
      if (secondToFirst.ContainsKey(secondValue))
        throw new ArgumentException("Could not add second value " + secondValue.ToString() + " because it is already contained in the bidirectional dictionary.");

      firstToSecond.Add(firstValue, secondValue);
      secondToFirst.Add(secondValue, firstValue);
    }

    public bool ContainsFirst(TFirst firstValue) {
      return firstToSecond.ContainsKey(firstValue);
    }

    public bool ContainsSecond(TSecond secondValue) {
      return secondToFirst.ContainsKey(secondValue);
    }

    public TSecond GetByFirst(TFirst firstValue) {
      return firstToSecond[firstValue];
    }

    public TFirst GetBySecond(TSecond secondValue) {
      return secondToFirst[secondValue];
    }

    public void SetByFirst(TFirst firstValue, TSecond secondValue) {
      if (secondToFirst.ContainsKey(secondValue))
        throw new ArgumentException("Could not set second value " + secondValue.ToString() + " because it is already contained in the bidirectional dictionary.");

      RemoveByFirst(firstValue);
      Add(firstValue, secondValue);
    }

    public void SetBySecond(TSecond secondValue, TFirst firstValue) {
      if (firstToSecond.ContainsKey(firstValue))
        throw new ArgumentException("Could not set first value " + firstValue.ToString() + " because it is already contained in the bidirectional dictionary.");

      RemoveBySecond(secondValue);
      Add(firstValue, secondValue);
    }

    public void RemoveByFirst(TFirst firstValue) {
      if (ContainsFirst(firstValue)) {
        TSecond secondValue = firstToSecond[firstValue];
        firstToSecond.Remove(firstValue);
        secondToFirst.Remove(secondValue);
      }
    }

    public void RemoveBySecond(TSecond secondValue) {
      if (ContainsSecond(secondValue)) {
        TFirst firstValue = secondToFirst[secondValue];
        secondToFirst.Remove(secondValue);
        firstToSecond.Remove(firstValue);
      }
    }

    public bool TryGetByFirst(TFirst key, out TSecond secondValue) {
      return firstToSecond.TryGetValue(key, out secondValue);
    }

    public bool TryGetBySecond(TSecond key, out TFirst firstValue) {
      return secondToFirst.TryGetValue(key, out firstValue);
    }

    public void Clear() {
      firstToSecond.Clear();
      secondToFirst.Clear();
    }

    public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() {
      return firstToSecond.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    #endregion
  }
}
