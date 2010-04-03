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

using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents the parameters and results of an algorithm run.
  /// </summary>
  [Item("Run", "The parameters and results of an algorithm run.")]
  [StorableClass]
  public sealed class Run : NamedItem {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
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

    public Run()
      : base("Anonymous") {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
    }
    public Run(string name)
      : base(name) {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
    }
    public Run(string name, IAlgorithm algorithm)
      : base(name) {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
      algorithm.CollectParameterValues(parameters);
      algorithm.CollectResultValues(results);
    }
    public Run(string name, string description)
      : base(name, description) {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
    }
    public Run(string name, string description, IAlgorithm algorithm)
      : base(name, description) {
      parameters = new Dictionary<string, IItem>();
      results = new Dictionary<string, IItem>();
      algorithm.CollectParameterValues(parameters);
      algorithm.CollectResultValues(results);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Run clone = new Run(Name, Description);
      cloner.RegisterClonedObject(this, clone);
      foreach (string key in parameters.Keys)
        clone.parameters.Add(key, (IItem)cloner.Clone(parameters[key]));
      foreach (string key in results.Keys)
        clone.results.Add(key, (IItem)cloner.Clone(results[key]));
      return clone;
    }
  }
}
