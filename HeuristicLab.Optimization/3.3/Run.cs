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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents the parameters and results of an algorithm run.
  /// </summary>
  [Item("Run", "The parameters and results of an algorithm run.")]
  [StorableClass]
  public sealed class Run : NamedItem, IRun, IStorableContent {
    public string Filename { get; set; }

    [StorableConstructor]
    private Run(bool deserializing) : base(deserializing) { }
    private Run(Run original, Cloner cloner)
      : base(original, cloner) {
      color = original.color;
      algorithm = cloner.Clone(original.algorithm);

      parameters = new Dictionary<string, IItem>();
      foreach (string key in original.parameters.Keys)
        parameters.Add(key, cloner.Clone(original.parameters[key]));

      results = new Dictionary<string, IItem>();
      foreach (string key in original.results.Keys)
        results.Add(key, cloner.Clone(original.results[key]));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Run(this, cloner);
    }

    public Run()
      : base() {
      name = ItemName;
      description = ItemDescription;
      color = Color.Black;
      algorithm = null;
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
    }
    public Run(IAlgorithm algorithm)
      : base() {
      if (algorithm == null) throw new ArgumentNullException();
      name = algorithm.Name + " Run (" + algorithm.ExecutionTime.ToString() + ")";
      description = ItemDescription;
      color = Color.Black;
      Initialize(algorithm);
    }
    public Run(string name, IAlgorithm algorithm)
      : base(name) {
      if (algorithm == null) throw new ArgumentNullException();
      color = Color.Black;
      description = ItemDescription;
      Initialize(algorithm);
    }
    public Run(string name, string description, IAlgorithm algorithm)
      : base(name, description) {
      if (algorithm == null) throw new ArgumentNullException();
      color = Color.Black;
      Initialize(algorithm);
    }

    private void Initialize(IAlgorithm algorithm) {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();

      if (algorithm.StoreAlgorithmInEachRun) {
        IAlgorithm clone = (IAlgorithm)algorithm.Clone();
        clone.CollectParameterValues(parameters);
        clone.CollectResultValues(results);
        clone.Runs.Clear();
        this.algorithm = clone;
      } else {
        algorithm.CollectParameterValues(parameters);
        algorithm.CollectResultValues(results);
        Cloner cloner = new Cloner();
        parameters = parameters.Select(x => new KeyValuePair<string, IItem>(x.Key, cloner.Clone(x.Value))).ToDictionary(x => x.Key, x => x.Value);
        results = results.Select(x => new KeyValuePair<string, IItem>(x.Key, cloner.Clone(x.Value))).ToDictionary(x => x.Key, x => x.Value);
      }
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (color == Color.Empty) color = Color.Black;
    }

    [Storable]
    private IAlgorithm algorithm;
    public IAlgorithm Algorithm {
      get { return algorithm; }
    }
    [Storable]
    private Dictionary<string, IItem> parameters;
    public IDictionary<string, IItem> Parameters {
      get { return parameters; }
    }
    [Storable]
    private Dictionary<string, IItem> results;
    public IDictionary<string, IItem> Results {
      get { return results; }
    }

    [Storable]
    private Color color;
    public Color Color {
      get { return this.color; }
      set {
        if (color != value) {
          this.color = value;
          this.OnChanged();
        }
      }
    }
    private bool visible = true;
    public bool Visible {
      get { return this.visible; }
      set {
        if (visible != value) {
          this.visible = value;
          this.OnChanged();
        }
      }
    }
    public event EventHandler Changed;
    private void OnChanged() {
      EventHandler handler = Changed;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
  }
}
