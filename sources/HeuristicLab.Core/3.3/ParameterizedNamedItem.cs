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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// A base class for items which have a name and contain parameters.
  /// </summary>
  [Item("ParameterizedNamedItem", "A base class for items which have a name and contain parameters.")]
  [StorableClass]
  public abstract class ParameterizedNamedItem : NamedItem, IParameterizedNamedItem {
    [Storable]
    private ParameterCollection parameters;
    protected ParameterCollection Parameters {
      get { return parameters; }
    }
    private ReadOnlyKeyedItemCollection<string, IParameter> readOnlyParameters;
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get {
        if (readOnlyParameters == null) readOnlyParameters = parameters.AsReadOnly();
        return readOnlyParameters;
      }
    }

    [StorableConstructor]
    protected ParameterizedNamedItem(bool deserializing) : base(deserializing) { }
    protected ParameterizedNamedItem(ParameterizedNamedItem original, Cloner cloner)
      : base(original, cloner) {
      parameters = cloner.Clone(original.parameters);
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem()
      : base() {
      name = ItemName;
      description = ItemDescription;
      parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name)
      : base(name) {
      description = ItemDescription;
      parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, ParameterCollection parameters)
      : base(name) {
      description = ItemDescription;
      this.parameters = parameters;
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, string description)
      : base(name, description) {
      parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, string description, ParameterCollection parameters)
      : base(name, description) {
      this.parameters = parameters;
      readOnlyParameters = null;
    }

    public virtual void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (IValueParameter param in parameters.OfType<IValueParameter>()) {
        if (param.GetsCollected && param.Value != null) values.Add(param.Name, param.Value);
        if (param.Value is IParameterizedItem) {
          Dictionary<string, IItem> children = new Dictionary<string, IItem>();
          ((IParameterizedItem)param.Value).CollectParameterValues(children);
          foreach (string key in children.Keys)
            values.Add(param.Name + "." + key, children[key]);
        }
      }
    }
  }
}
