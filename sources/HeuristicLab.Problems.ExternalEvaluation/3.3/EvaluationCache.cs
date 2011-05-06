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
 */
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Problems.ExternalEvaluation {

  [Item("EvaluationCache", "Cache for external evaluation values")]
  [StorableClass]
  public class EvaluationCache : NamedItem {

    public delegate double Evaluator(SolutionMessage message);

    public override System.Drawing.Image ItemImage {
      get { return VSImageLibrary.Database; }
    }

    #region Fields & Properties
    [Storable]
    private Dictionary<string, double> cache;

    [Storable]
    private int cacheHits;

    public int CacheSize { get { return cache.Count; } }

    public int CacheHits { get { return cacheHits; } }
    #endregion

    #region Events
    public event EventHandler CacheSizeChanged;
    public event EventHandler CacheHitsChanged;

    protected virtual void OnCacheSizeChanged() {
      EventHandler handler = CacheSizeChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    protected virtual void OnCacheHitsChanged() {
      EventHandler handler = CacheHitsChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion



    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationCache(bool deserializing) : base(deserializing) { }
    protected EvaluationCache(EvaluationCache original, Cloner cloner)
      : base(original, cloner) {
      cache = original.cache.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    public EvaluationCache() {
      cache = new Dictionary<string, double>();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationCache(this, cloner);
    }
    #endregion

    public double GetValue(SolutionMessage message, Evaluator evaluate) {
      string s = message.ToString();
      double value;
      if (cache.TryGetValue(s, out value)) {
        cacheHits++;
        OnCacheHitsChanged();
      } else {
        value = evaluate(message);
        cache[s] = value;
        OnCacheSizeChanged();
      }
      return value;
    }

    public void Reset() {
      cache = new Dictionary<string, double>();
      OnCacheSizeChanged();
      cacheHits = 0;
      OnCacheHitsChanged();
    }
  }
}
