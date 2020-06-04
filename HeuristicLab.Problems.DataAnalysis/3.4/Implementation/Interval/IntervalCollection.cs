#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.DataAnalysis {
  [Item("IntervalCollection", "Represents variables with their interval ranges.")]
  [StorableType("230B4E4B-41E5-4D33-9BC3-E2DAADDCA5AE")]
  public class IntervalCollection : Item {
    public static new System.Drawing.Image StaticItemImage {
      get => HeuristicLab.Common.Resources.VSImageLibrary.Object;
    }
    private IDictionary<string, Interval> intervals { get; } = new Dictionary<string, Interval>();

    [Storable(Name = "StorableIntervalInformation")]
    private KeyValuePair<string, double[]>[] StorableIntervalInformation {
      get {
        var l = new List<KeyValuePair<string, double[]>>();
        foreach (var varInt in intervals)

          l.Add(new KeyValuePair<string, double[]>(varInt.Key,
            new double[] { varInt.Value.LowerBound, varInt.Value.UpperBound }));
        return l.ToArray();
      }

      set {
        foreach (var varInt in value)
          intervals.Add(varInt.Key, new Interval(varInt.Value[0], varInt.Value[1]));
      }
    }

    public int Count => intervals.Count;

    public IntervalCollection() : base() { }
    [StorableConstructor]
    protected IntervalCollection(StorableConstructorFlag _) : base(_) { }

    protected IntervalCollection(IntervalCollection original, Cloner cloner) : base(original, cloner) {
      foreach (var keyValuePair in original.intervals) {
        intervals.Add(keyValuePair.Key, new Interval(keyValuePair.Value.LowerBound, keyValuePair.Value.UpperBound));
      }
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IntervalCollection(this, cloner);
    }

    public IntervalCollection(IDictionary<string, Interval> intervals) {
      this.intervals = intervals;
    }

    public Interval GetInterval(string identifier) {
      if (!intervals.ContainsKey(identifier)) throw new ArgumentException($"The given identifier:{ identifier } is not present!");

      return intervals[identifier];
    }

    public void SetInterval(string identifier, Interval interval) {
      intervals[identifier] = interval;
    }

    public void AddInterval(string identifier, Interval interval) {
      intervals.Add(identifier, interval);
    }

    public void DeleteInterval(string identifier) {
      intervals.Remove(identifier);
    }

    public IReadOnlyDictionary<string, Interval> GetReadonlyDictionary() {
      return intervals.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IEnumerable<Tuple<string, Interval>> GetVariableIntervals() {
      foreach (var variableInterval in intervals)
        yield return Tuple.Create(variableInterval.Key, variableInterval.Value);
    }
  }
}

