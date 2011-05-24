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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Analysis;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
namespace HeuristicLab.Problems.ExternalEvaluation {

  [Item("EvaluationCache", "Cache for external evaluation values")]
  [StorableClass]
  public class EvaluationCache : ParameterizedNamedItem {

    #region Types
    private class CacheEntry {

      public string Key { get; private set; }
      public double Value { get; set; }

      public CacheEntry(string key, double value) {
        Key = key;
        Value = value;
      }

      public CacheEntry(string key) : this(key, 0) { }

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
    private object evaluationLock = new object();
    private AutoResetEvent evaluationDone = new AutoResetEvent(false);
    #endregion

    #region Properties
    public override System.Drawing.Image ItemImage {
      get { return VSImageLibrary.Database; }
    }
    public int Size {
      get {
        lock (cacheLock) {
          return index.Count;
        }
      }
    }
    public int ActiveEvaluations {
      get {
        lock (evaluationLock) {
          return activeEvaluations.Count;
        }
      }
    }
    [Storable]
    public int Hits { get; private set; }
    #endregion

    #region events
    public event EventHandler SizeChanged;
    public event EventHandler HitsChanged;
    public event EventHandler ActiveEvalutionsChanged;

    protected virtual void OnSizeChanged() {
      EventHandler handler = SizeChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    protected virtual void OnHitsChanged() {
      EventHandler handler = HitsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    protected virtual void OnActiveEvalutionsChanged() {
      EventHandler handler = ActiveEvalutionsChanged;
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
    [Storable(Name="Cache")]
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
      Trim();
    }
    #endregion

    #region Methods
    public void Reset() {
      lock (cacheLock) {
        list = new LinkedList<CacheEntry>();
        index = new Dictionary<CacheEntry, LinkedListNode<CacheEntry>>();
        Hits = 0;
      }
      OnSizeChanged();
      OnHitsChanged();
    }

    public double GetValue(SolutionMessage message, Evaluator evaluate) {
      CacheEntry entry = new CacheEntry(message.ToString());
      LinkedListNode<CacheEntry> node;
      bool lockTaken = false;
      try {
        Monitor.Enter(cacheLock, ref lockTaken);
        if (index.TryGetValue(entry, out node)) {
          list.Remove(node);
          list.AddLast(node);
          Hits++;
          lockTaken = false;
          Monitor.Exit(cacheLock);
          OnHitsChanged();
          return node.Value.Value;
        } else {
          lockTaken = false;
          Monitor.Exit(cacheLock);
          return Evaluate(message, evaluate, entry);
        }
      } finally {
        if (lockTaken)
          Monitor.Exit(cacheLock);
      }
    }

    private double Evaluate(SolutionMessage message, Evaluator evaluate, CacheEntry entry) {
      bool lockTaken = false;
      try {
        Monitor.Enter(evaluationLock, ref lockTaken);
        if (activeEvaluations.Contains(entry.Key)) {
          while (activeEvaluations.Contains(entry.Key)) {
            lockTaken = false;
            Monitor.Exit(evaluationLock);
            evaluationDone.WaitOne();
            Monitor.Enter(evaluationLock, ref lockTaken);
          }
          lock (cacheLock) {
            return index[entry].Value.Value;
          }
        } else {
          activeEvaluations.Add(entry.Key);
          lockTaken = false;
          Monitor.Exit(evaluationLock);
          OnActiveEvalutionsChanged();
          try {
            entry.Value = evaluate(message);
            lock (cacheLock) {
              index[entry] = list.AddLast(entry);
            }
            Trim();
          } finally {
            lock (evaluationLock) {
              activeEvaluations.Remove(entry.Key);
              evaluationDone.Set();
            }
            OnActiveEvalutionsChanged();
          }
          return entry.Value;
        }
      } finally {
        if (lockTaken)
          Monitor.Exit(evaluationLock);
      }
    }

    private void Trim() {
      lock (cacheLock) {
        while (list.Count > Capacity) {
          LinkedListNode<CacheEntry> item = list.First;
          list.Remove(item);
          index.Remove(item.Value);
        }
      }
      OnSizeChanged();
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
