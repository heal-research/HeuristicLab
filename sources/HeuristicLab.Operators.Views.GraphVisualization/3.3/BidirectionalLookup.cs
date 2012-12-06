#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  [Obsolete("Use BidirectionalDictionary in HeuristicLab.Collections")]
  public class BidirectionalLookup<TFirst, TSecond> {
    [Storable]
    private Dictionary<TFirst, TSecond> firstToSecond;
    [Storable]
    private Dictionary<TSecond, TFirst> secondToFirst;

    [StorableConstructor]
    protected BidirectionalLookup(bool deserializing) : base() { }

    public BidirectionalLookup() {
      this.firstToSecond = new Dictionary<TFirst, TSecond>();
      this.secondToFirst = new Dictionary<TSecond, TFirst>();
    }

    #region properties
    public int Count {
      get { return this.firstToSecond.Count; }
    }

    public IEnumerable<TFirst> FirstValues {
      get { return this.firstToSecond.Keys; }
    }

    public IEnumerable<TSecond> SecondValues {
      get { return this.secondToFirst.Keys; }
    }

    public IEnumerable<KeyValuePair<TFirst, TSecond>> FirstEnumerable {
      get { return this.firstToSecond; }
    }

    public IEnumerable<KeyValuePair<TSecond, TFirst>> SecondEnumerable {
      get { return this.secondToFirst; }
    }
    #endregion


    #region methods
    public void Add(TFirst firstValue, TSecond secondValue) {
      if (this.firstToSecond.ContainsKey(firstValue))
        throw new ArgumentException("Could not add first value " + firstValue.ToString() + " because it is already contained in the bidirectional lookup.");
      if (this.secondToFirst.ContainsKey(secondValue))
        throw new ArgumentException("Could not add second value " + secondValue.ToString() + " because it is already contained in the bidirectional lookup.");

      firstToSecond.Add(firstValue, secondValue);
      secondToFirst.Add(secondValue, firstValue);
    }

    public bool ContainsFirst(TFirst firstValue) {
      return this.firstToSecond.ContainsKey(firstValue);
    }

    public bool ContainsSecond(TSecond secondValue) {
      return this.secondToFirst.ContainsKey(secondValue);
    }

    public TSecond GetByFirst(TFirst firstValue) {
      return this.firstToSecond[firstValue];
    }

    public TFirst GetBySecond(TSecond secondValue) {
      return this.secondToFirst[secondValue];
    }

    public void SetByFirst(TFirst firstValue, TSecond secondValue) {
      if (this.secondToFirst.ContainsKey(secondValue))
        throw new ArgumentException("Could not set second value " + secondValue.ToString() + " because it is already contained in the bidirectional lookup.");

      this.RemoveByFirst(firstValue);
      this.Add(firstValue, secondValue);
    }

    public void SetBySecond(TSecond secondValue, TFirst firstValue) {
      if (this.firstToSecond.ContainsKey(firstValue))
        throw new ArgumentException("Could not set first value " + firstValue.ToString() + " because it is already contained in the bidirectional lookup.");

      this.RemoveBySecond(secondValue);
      this.Add(firstValue, secondValue);
    }

    public void RemoveByFirst(TFirst firstValue) {
      if (this.ContainsFirst(firstValue)) {
        TSecond secondValue = this.firstToSecond[firstValue];
        this.firstToSecond.Remove(firstValue);
        this.secondToFirst.Remove(secondValue);
      }
    }

    public void RemoveBySecond(TSecond secondValue) {
      if (this.ContainsSecond(secondValue)) {
        TFirst firstValue = this.secondToFirst[secondValue];
        this.secondToFirst.Remove(secondValue);
        this.firstToSecond.Remove(firstValue);
      }
    }

    public void Clear() {
      this.firstToSecond.Clear();
      this.secondToFirst.Clear();
    }
    #endregion
  }
}
