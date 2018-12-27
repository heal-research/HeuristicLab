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
using System.ComponentModel;
using System.Drawing;
using HeuristicLab.Collections;
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

      parameters = new ObservableDictionary<string, IItem>();
      foreach (string key in original.parameters.Keys)
        parameters.Add(key, cloner.Clone(original.parameters[key]));

      results = new ObservableDictionary<string, IItem>();
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
      parameters = new ObservableDictionary<string, IItem>();
      results = new ObservableDictionary<string, IItem>();
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
      parameters = new ObservableDictionary<string, IItem>();
      results = new ObservableDictionary<string, IItem>();

      if (algorithm.StoreAlgorithmInEachRun) {
        var clone = (IAlgorithm)algorithm.Clone();
        clone.CollectParameterValues(parameters);
        clone.CollectResultValues(results);
        clone.Runs.Clear();
        this.algorithm = clone;
      } else {
        var par = new Dictionary<string, IItem>();
        var res = new Dictionary<string, IItem>();
        algorithm.CollectParameterValues(par);
        algorithm.CollectResultValues(res);
        var cloner = new Cloner();
        foreach (var k in par) parameters.Add(k.Key, cloner.Clone(k.Value));
        foreach (var k in res) results.Add(k.Key, cloner.Clone(k.Value));
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

    [Storable(Name = "parameters")]
    private IDictionary<string, IItem> StorableParameters {
      get { return parameters; }
      set {
        if (!(value is IObservableDictionary<string, IItem>))
          parameters = new ObservableDictionary<string, IItem>(value);
        else parameters = (IObservableDictionary<string, IItem>)value;
      }
    }
    private IObservableDictionary<string, IItem> parameters;
    public IObservableDictionary<string, IItem> Parameters {
      get { return parameters; }
      private set {
        if (parameters != value) {
          parameters = value;
          OnPropertyChanged("Parameters");
        }
      }
    }

    [Storable(Name = "results")]
    private IDictionary<string, IItem> StorableResults {
      get { return results; }
      set {
        if (!(value is IObservableDictionary<string, IItem>))
          results = new ObservableDictionary<string, IItem>(value);
        else results = (IObservableDictionary<string, IItem>)value;
      }
    }
    private IObservableDictionary<string, IItem> results;
    public IObservableDictionary<string, IItem> Results {
      get { return results; }
      private set {
        if (results != value) {
          results = value;
          OnPropertyChanged("Results");
        }
      }
    }

    [Storable]
    private Color color;
    public Color Color {
      get { return this.color; }
      set {
        if (color != value) {
          this.color = value;
          OnPropertyChanged("Color");
        }
      }
    }
    private bool visible = true;
    public bool Visible {
      get { return this.visible; }
      set {
        if (visible != value) {
          this.visible = value;
          OnPropertyChanged("Visible");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string property) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(property));
    }
  }
}
