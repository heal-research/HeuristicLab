#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Collections {
  [StorableClass]
  [Serializable]
  public class BidirectionalLookup<TFirst, TSecond> {
    [Storable]
    private readonly Dictionary<TFirst, HashSet<TSecond>> firstToSecond;
    [Storable]
    private readonly Dictionary<TSecond, HashSet<TFirst>> secondToFirst;

    [StorableConstructor]
    protected BidirectionalLookup(bool deserializing) : base() { }
    public BidirectionalLookup() {
      firstToSecond = new Dictionary<TFirst, HashSet<TSecond>>();
      secondToFirst = new Dictionary<TSecond, HashSet<TFirst>>();
    }
    public BidirectionalLookup(IEqualityComparer<TFirst> firstComparer) {
      firstToSecond = new Dictionary<TFirst, HashSet<TSecond>>(firstComparer);
      secondToFirst = new Dictionary<TSecond, HashSet<TFirst>>();
    }
    public BidirectionalLookup(IEqualityComparer<TSecond> secondComparer) {
      firstToSecond = new Dictionary<TFirst, HashSet<TSecond>>();
      secondToFirst = new Dictionary<TSecond, HashSet<TFirst>>(secondComparer);
    }
    public BidirectionalLookup(IEqualityComparer<TFirst> firstComparer, IEqualityComparer<TSecond> secondComparer) {
      firstToSecond = new Dictionary<TFirst, HashSet<TSecond>>(firstComparer);
      secondToFirst = new Dictionary<TSecond, HashSet<TFirst>>(secondComparer);
    }

    #region Properties
    public int CountFirst {
      get { return firstToSecond.Count; }
    }

    public int CountSecond {
      get { return secondToFirst.Count; }
    }

    public IEnumerable<IEnumerable<TSecond>> FirstValues {
      get { return firstToSecond.Values.AsEnumerable(); }
    }

    public IEnumerable<IEnumerable<TFirst>> SecondValues {
      get { return secondToFirst.Values.AsEnumerable(); }
    }

    public IEnumerable<IGrouping<TFirst, TSecond>> FirstEnumerable {
      get { return firstToSecond.Select(x => new StorableGrouping<TFirst, TSecond>(x.Key, x.Value, secondToFirst.Comparer)); }
    }

    public IEnumerable<IGrouping<TSecond, TFirst>> SecondEnumerable {
      get { return secondToFirst.Select(x => new StorableGrouping<TSecond, TFirst>(x.Key, x.Value, firstToSecond.Comparer)); }
    }
    #endregion

    #region Methods
    public void Add(TFirst firstValue, TSecond secondValue) {
      HashSet<TSecond> firstSet;
      if (!firstToSecond.TryGetValue(firstValue, out firstSet)) {
        firstSet = new HashSet<TSecond>(secondToFirst.Comparer);
        firstToSecond[firstValue] = firstSet;
      }
      HashSet<TFirst> secondSet;
      if (!secondToFirst.TryGetValue(secondValue, out secondSet)) {
        secondSet = new HashSet<TFirst>(firstToSecond.Comparer);
        secondToFirst[secondValue] = secondSet;
      }
      firstSet.Add(secondValue);
      secondSet.Add(firstValue);
    }

    public void AddRangeFirst(TFirst firstValue, IEnumerable<TSecond> secondValues) {
      HashSet<TSecond> firstSet;
      if (!firstToSecond.TryGetValue(firstValue, out firstSet)) {
        firstSet = new HashSet<TSecond>(secondToFirst.Comparer);
        firstToSecond[firstValue] = firstSet;
      }
      foreach (var s in secondValues) {
        HashSet<TFirst> secondSet;
        if (!secondToFirst.TryGetValue(s, out secondSet)) {
          secondSet = new HashSet<TFirst>(firstToSecond.Comparer);
          secondToFirst[s] = secondSet;
        }
        firstSet.Add(s);
        secondSet.Add(firstValue);
      }
    }

    public void AddRangeSecond(TSecond secondValue, IEnumerable<TFirst> firstValues) {
      HashSet<TFirst> secondSet;
      if (!secondToFirst.TryGetValue(secondValue, out secondSet)) {
        secondSet = new HashSet<TFirst>(firstToSecond.Comparer);
        secondToFirst[secondValue] = secondSet;
      }
      foreach (var f in firstValues) {
        HashSet<TSecond> firstSet;
        if (!firstToSecond.TryGetValue(f, out firstSet)) {
          firstSet = new HashSet<TSecond>(secondToFirst.Comparer);
          firstToSecond[f] = firstSet;
        }
        firstSet.Add(secondValue);
        secondSet.Add(f);
      }
    }

    public bool ContainsFirst(TFirst firstValue) {
      return firstToSecond.ContainsKey(firstValue);
    }

    public bool ContainsSecond(TSecond secondValue) {
      return secondToFirst.ContainsKey(secondValue);
    }

    public IEnumerable<TSecond> GetByFirst(TFirst firstValue) {
      return firstToSecond[firstValue];
    }

    public IEnumerable<TFirst> GetBySecond(TSecond secondValue) {
      return secondToFirst[secondValue];
    }

    public void SetByFirst(TFirst firstValue, IEnumerable<TSecond> secondValues) {
      RemoveByFirst(firstValue);
      AddRangeFirst(firstValue, secondValues);
    }

    public void SetBySecond(TSecond secondValue, IEnumerable<TFirst> firstValues) {
      RemoveBySecond(secondValue);
      AddRangeSecond(secondValue, firstValues);
    }

    public void RemovePair(TFirst first, TSecond second) {
      if (!ContainsFirst(first) || !ContainsSecond(second)) return;
      firstToSecond[first].Remove(second);
      if (!firstToSecond[first].Any()) firstToSecond.Remove(first);
      secondToFirst[second].Remove(first);
      if (!secondToFirst[second].Any()) secondToFirst.Remove(second);
    }

    public void RemoveByFirst(TFirst firstValue) {
      if (!ContainsFirst(firstValue)) return;
      var secondValues = firstToSecond[firstValue].ToArray();
      firstToSecond.Remove(firstValue);
      foreach (var s in secondValues) {
        secondToFirst[s].Remove(firstValue);
        if (!secondToFirst[s].Any()) secondToFirst.Remove(s);
      }
    }

    public void RemoveBySecond(TSecond secondValue) {
      if (!ContainsSecond(secondValue)) return;
      var firstValues = secondToFirst[secondValue].ToArray();
      secondToFirst.Remove(secondValue);
      foreach (var f in firstValues) {
        firstToSecond[f].Remove(secondValue);
        if (!firstToSecond[f].Any()) firstToSecond.Remove(f);
      }
    }

    public void Clear() {
      firstToSecond.Clear();
      secondToFirst.Clear();
    }
    #endregion

    [StorableClass]
    private class StorableGrouping<TKey, TValue> : IGrouping<TKey, TValue> {

      [Storable]
      private readonly TKey key;
      [Storable]
      private readonly HashSet<TValue> values;

      public StorableGrouping(TKey key, IEnumerable<TValue> values, IEqualityComparer<TValue> comparer) {
        this.key = key;
        this.values = new HashSet<TValue>(values, comparer);
      }

      public TKey Key {
        get { return key; }
      }

      public IEnumerator<TValue> GetEnumerator() {
        return values.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }
    }
  }
}
