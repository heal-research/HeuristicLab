﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using Google.Protobuf;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.ExternalEvaluation {

  [Item("EvaluationCache", "Cache for external evaluation values")]
  [StorableType("FDB10541-FA1C-4CF9-8175-83A55700C052")]
  public class EvaluationCache : ParameterizedNamedItem {

    #region Types
    private sealed class CacheEntry {
      public readonly string Key;
      private QualityMessage message;
      private byte[] rawMessage;

      private readonly object lockObject = new object();

      public byte[] RawMessage {
        get { return rawMessage; }
        set {
          lock (lockObject) {
            rawMessage = value;
            message = null;
          }
        }
      }

      public CacheEntry(string key) {
        Key = key;
      }

      public QualityMessage GetMessage(ExtensionRegistry extensions) {
        lock (lockObject) {
          if (message == null && rawMessage != null)
            message = QualityMessage.Parser.WithExtensionRegistry(extensions).ParseFrom(ByteString.CopyFrom(rawMessage));
        }
        return message;
      }
      public void SetMessage(QualityMessage value) {
        lock (lockObject) {
          message = value;
          rawMessage = value.ToByteArray();
        }
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

      public string QualityString(IFormatProvider formatProvider = null) {
        if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;
        if (RawMessage == null) return "-";
        var msg = message ?? CreateBasicQualityMessage();
        switch (msg.Type) {
          case QualityMessage.Types.Type.SingleObjectiveQualityMessage:
            return msg.GetExtension(SingleObjectiveQualityMessage.Extensions.QualityMessage_).Quality.ToString(formatProvider);
          case QualityMessage.Types.Type.MultiObjectiveQualityMessage:
            var qualities = msg.GetExtension(MultiObjectiveQualityMessage.Extensions.QualityMessage_).Qualities;
            return string.Format("[{0}]", string.Join(",", qualities.Select(q => q.ToString(formatProvider))));
          default:
            return "-";
        }
      }
      private QualityMessage CreateBasicQualityMessage() {
        var extensions = new ExtensionRegistry {
          SingleObjectiveQualityMessage.Extensions.QualityMessage_,
          MultiObjectiveQualityMessage.Extensions.QualityMessage_
        };
        return QualityMessage.Parser.WithExtensionRegistry(extensions).ParseFrom(ByteString.CopyFrom(rawMessage));
      }

      public override string ToString() {
        return string.Format("{{{0} : {1}}}", Key, QualityString());
      }
    }

    public delegate QualityMessage Evaluator(SolutionMessage message);
    #endregion

    #region Fields
    private LinkedList<CacheEntry> list;
    private Dictionary<CacheEntry, LinkedListNode<CacheEntry>> index;

    private HashSet<string> activeEvaluations = new HashSet<string>();
    private readonly object cacheLock = new object();
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
    [Storable(Name = "CacheNew")]
    private IEnumerable<KeyValuePair<string, byte[]>> Cache_Persistence {
      get { return IsPersistent ? GetCacheValues() : Enumerable.Empty<KeyValuePair<string, byte[]>>(); }
      set { SetCacheValues(value); }
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationCache(StorableConstructorFlag _) : base(_) { }
    protected EvaluationCache(EvaluationCache original, Cloner cloner)
        : base(original, cloner) {
      SetCacheValues(original.GetCacheValues());
      Hits = original.Hits;
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
      CapacityParameter.Value.ValueChanged += new EventHandler(CapacityChanged);
    }

    private void CapacityChanged(object sender, EventArgs e) {
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

    public QualityMessage GetValue(SolutionMessage message, Evaluator evaluate, ExtensionRegistry extensions) {
      var entry = new CacheEntry(message.ToString());
      bool lockTaken = false;
      bool waited = false;
      try {
        Monitor.Enter(cacheLock, ref lockTaken);
        while (true) {
          LinkedListNode<CacheEntry> node;
          if (index.TryGetValue(entry, out node)) {
            list.Remove(node);
            list.AddLast(node);
            Hits++;
            lockTaken = false;
            Monitor.Exit(cacheLock);
            OnChanged();
            return node.Value.GetMessage(extensions);
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
                entry.SetMessage(evaluate(message));
                Monitor.Enter(cacheLock, ref lockTaken);
                index[entry] = list.AddLast(entry);
                Trim();
              } finally {
                if (!lockTaken)
                  Monitor.Enter(cacheLock, ref lockTaken);
                activeEvaluations.Remove(entry.Key);
                Monitor.PulseAll(cacheLock);
                lockTaken = false;
                Monitor.Exit(cacheLock);
              }
              OnChanged();
              return entry.GetMessage(extensions);
            }
          }
        }
      } finally {
        if (lockTaken)
          Monitor.Exit(cacheLock);
      }
    }

    private void Trim() {
      while (list.Count > Capacity) {
        var item = list.First;
        list.Remove(item);
        index.Remove(item.Value);
      }
    }

    private IEnumerable<KeyValuePair<string, byte[]>> GetCacheValues() {
      lock (cacheLock) {
        return index.ToDictionary(kvp => kvp.Key.Key, kvp => kvp.Key.RawMessage);
      }
    }

    private void SetCacheValues(IEnumerable<KeyValuePair<string, byte[]>> value) {
      lock (cacheLock) {
        if (list == null) list = new LinkedList<CacheEntry>();
        if (index == null) index = new Dictionary<CacheEntry, LinkedListNode<CacheEntry>>();
        foreach (var kvp in value) {
          var entry = new CacheEntry(kvp.Key) { RawMessage = kvp.Value };
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
              entry.QualityString(CultureInfo.InvariantCulture)));
          }
        }
        writer.Close();
      }
    }
    #endregion
  }
}
