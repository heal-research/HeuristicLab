#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * 
 * The LRU cache is based on an idea by Robert Rossney see 
 * <http://csharp-lru-cache.googlecode.com>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {

  [Item("EvaluationCache", "Cache for external evaluation values")]
  [StorableClass]
  public class EvaluationCache : ParameterizedNamedItem {

    #region Types
    private sealed class CacheEntry {

      public readonly string Key;
      public double Value;

      public CacheEntry(string key, double value) {
        Key = key;
        Value = value;
      }

      public CacheEntry(string key) {
        Key = key;
      }

      public override bool Equals(object obj) {
        CacheEntry other = obj as CacheEntry;
        if (other == null)
          return false;
        return Key.Equals(other.Key);
      }

      public override int GetHashCode() {
        return Key.GetHashCode();
      }

      public override string ToString() {
        return string.Format("{{{0} : {1}}}", Key, Value);
      }
    }

    public delegate double Evaluator(SolutionMessage message);
    #endregion

    #region Fields
    private LinkedList<CacheEntry> list;
    private Dictionary<CacheEntry, LinkedListNode<CacheEntry>> index;

    private HashSet<string> activeEvaluations = new HashSet<string>();
    private object cacheLock = new object();
    #endregion

    #region Properties
    public static new System.Drawing.Image StaticItemImage {
      get { return VSImageLibrary.Database; }
    }
    public int Size { get { lock (cacheLock) return index.Count; } }
    public int ActiveEvaluations { get { lock (cacheLock) return activeEvaluations.Count; } }

    [Storable]
    public int Hits { get; private set; }
    #endregion

    #region events
    public event EventHandler Changed;

    protected virtual void OnChanged() {
      EventHandler handler = Changed;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion

    #region Parameters
    public FixedValueParameter<IntValue> CapacityParameter {
      get { return (FixedValueParameter<IntValue>)Parameters["Capacity"]; }
    }
    public FixedValueParameter<BoolValue> PersistentCacheParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["PersistentCache"]; }
    }
    #endregion

    #region Parameter Values
    public int Capacity {
      get { return CapacityParameter.Value.Value; }
      set { CapacityParameter.Value.Value = value; }
    }
    public bool IsPersistent {
      get { return PersistentCacheParameter.Value.Value; }
    }
    #endregion

    #region Persistence
    [Storable(Name = "Cache")]
    private IEnumerable<KeyValuePair<string, double>> Cache_Persistence {
      get {
        if (IsPersistent) {
          return GetCacheValues();
        } else {
          return Enumerable.Empty<KeyValuePair<string, double>>();
        }
      }
      set {
        SetCacheValues(value);
      }
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationCache(bool deserializing) : base(deserializing) { }
    protected EvaluationCache(EvaluationCache original, Cloner cloner)
      : base(original, cloner) {
      SetCacheValues(original.GetCacheValues());
      RegisterEvents();
    }
    public EvaluationCache() {
      list = new LinkedList<CacheEntry>();
      index = new Dictionary<CacheEntry, LinkedListNode<CacheEntry>>();
      Parameters.Add(new FixedValueParameter<IntValue>("Capacity", "Maximum number of cache entries.", new IntValue(10000)));
      Parameters.Add(new FixedValueParameter<BoolValue>("PersistentCache", "Save cache when serializing object graph?", new BoolValue(false)));
      RegisterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationCache(this, cloner);
    }
    #endregion

    #region Event Handling
    private void RegisterEvents() {
      CapacityParameter.Value.ValueChanged += new EventHandler(Value_ValueChanged);
    }

    void Value_ValueChanged(object sender, EventArgs e) {
      if (Capacity < 0)
        throw new ArgumentOutOfRangeException("Cache capacity cannot be less than zero");
      lock (cacheLock)
        Trim();
      OnChanged();
    }
    #endregion

    #region Methods
    public void Reset() {
      lock (cacheLock) {
        list = new LinkedList<CacheEntry>();
        index = new Dictionary<CacheEntry, LinkedListNode<CacheEntry>>();
        Hits = 0;
      }
      OnChanged();
    }

    public double GetValue(SolutionMessage message, Evaluator evaluate) {
      CacheEntry entry = new CacheEntry(message.ToString());
      LinkedListNode<CacheEntry> node;
      bool lockTaken = false;
      bool waited = false;
      try {
        Monitor.Enter(cacheLock, ref lockTaken);
        while (true) {
          if (index.TryGetValue(entry, out node)) {
            list.Remove(node);
            list.AddLast(node);
            Hits++;
            lockTaken = false;
            Monitor.Exit(cacheLock);
            OnChanged();
            return node.Value.Value;
          } else {
            if (!waited && activeEvaluations.Contains(entry.Key)) {
              while (activeEvaluations.Contains(entry.Key))
                Monitor.Wait(cacheLock);
              waited = true;
            } else {
              activeEvaluations.Add(entry.Key);
              lockTaken = false;
              Monitor.Exit(cacheLock);
              OnChanged();
              try {
                entry.Value = evaluate(message);
                Monitor.Enter(cacheLock, ref lockTaken);
                index[entry] = list.AddLast(entry);
                Trim();
              }
              finally {
                if (!lockTaken)
                  Monitor.Enter(cacheLock, ref lockTaken);
                activeEvaluations.Remove(entry.Key);
                Monitor.PulseAll(cacheLock);
                lockTaken = false;
                Monitor.Exit(cacheLock);
              }
              OnChanged();
              return entry.Value;
            }
          }
        }
      }
      finally {
        if (lockTaken)
          Monitor.Exit(cacheLock);
      }
    }

    private void Trim() {
      while (list.Count > Capacity) {
        LinkedListNode<CacheEntry> item = list.First;
        list.Remove(item);
        index.Remove(item.Value);
      }
    }

    private IEnumerable<KeyValuePair<string, double>> GetCacheValues() {
      lock (cacheLock) {
        return index.ToDictionary(kvp => kvp.Key.Key, kvp => kvp.Key.Value);
      }
    }

    private void SetCacheValues(IEnumerable<KeyValuePair<string, double>> value) {
      lock (cacheLock) {
        list = new LinkedList<CacheEntry>();
        index = new Dictionary<CacheEntry, LinkedListNode<CacheEntry>>();
        foreach (var kvp in value) {
          var entry = new CacheEntry(kvp.Key);
          entry.Value = kvp.Value;
          index[entry] = list.AddLast(entry);
        }
      }
    }

    public void Save(string filename) {
      using (var writer = new StreamWriter(filename)) {
        lock (cacheLock) {
          foreach (var entry in list) {
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
              "\"{0}\", {1}",
              Regex.Replace(entry.Key, "\\s", "").Replace("\"", "\"\""),
              entry.Value));
          }
        }
        writer.Close();
      }
    }
    #endregion
  }
}
