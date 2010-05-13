#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Drawing;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents the parameters and results of an algorithm run.
  /// </summary>
  [Item("Run", "The parameters and results of an algorithm run.")]
  [StorableClass]
  public sealed class Run : NamedItem, IRun {
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

    private Color color = Color.Black;
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

    public Run()
      : base() {
      name = ItemName;
      description = ItemDescription;
      algorithm = null;
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
    }
    public Run(IAlgorithm algorithm)
      : base() {
      if (algorithm == null) throw new ArgumentNullException();
      name = algorithm.Name + " Run (" + algorithm.ExecutionTime.ToString() + ")";
      description = ItemDescription;
      Initialize((IAlgorithm)algorithm.Clone());
    }
    public Run(string name, IAlgorithm algorithm)
      : base(name) {
      if (algorithm == null) throw new ArgumentNullException();
      description = ItemDescription;
      Initialize((IAlgorithm)algorithm.Clone());
    }
    public Run(string name, string description, IAlgorithm algorithm)
      : base(name, description) {
      if (algorithm == null) throw new ArgumentNullException();
      Initialize((IAlgorithm)algorithm.Clone());
    }

    private void Initialize(IAlgorithm algorithm) {
      this.algorithm = algorithm;
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
      this.algorithm.CollectParameterValues(parameters);
      this.algorithm.CollectResultValues(results);
      this.algorithm.Prepare(true);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Run clone = (Run)base.Clone(cloner);
      clone.algorithm = (IAlgorithm)cloner.Clone(algorithm);
      foreach (string key in parameters.Keys)
        clone.parameters.Add(key, (IItem)cloner.Clone(parameters[key]));
      foreach (string key in results.Keys)
        clone.results.Add(key, (IItem)cloner.Clone(results[key]));
      return clone;
    }
  }
}
