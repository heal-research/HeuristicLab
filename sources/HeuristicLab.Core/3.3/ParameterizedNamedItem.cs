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

using HeuristicLab.Collections;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// A base class for items which have a name and contain parameters.
  /// </summary>
  [Item("ParameterizedNamedItem", "A base class for items which have a name and contain parameters.")]
  [StorableClass]
  public abstract class ParameterizedNamedItem : NamedItem, IParameterizedNamedItem {
    private ParameterCollection parameters;
    [Storable]
    protected ParameterCollection Parameters {
      get { return parameters; }
      private set {
        parameters = value;
        readOnlyParameters = null;
      }
    }
    private ReadOnlyObservableKeyedCollection<string, IParameter> readOnlyParameters;
    IObservableKeyedCollection<string, IParameter> IParameterizedItem.Parameters {
      get {
        if (readOnlyParameters == null) readOnlyParameters = parameters.AsReadOnly();
        return readOnlyParameters;
      }
    }

    protected ParameterizedNamedItem()
      : base() {
      name = ItemName;
      description = ItemDescription;
      Parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name)
      : base(name) {
      description = ItemDescription;
      Parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, ParameterCollection parameters)
      : base(name) {
      description = ItemDescription;
      Parameters = parameters;
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, string description)
      : base(name, description) {
      Parameters = new ParameterCollection();
      readOnlyParameters = null;
    }
    protected ParameterizedNamedItem(string name, string description, ParameterCollection parameters)
      : base(name, description) {
      Parameters = parameters;
      readOnlyParameters = null;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ParameterizedNamedItem clone = (ParameterizedNamedItem)base.Clone(cloner);
      clone.Parameters = (ParameterCollection)cloner.Clone(parameters);
      return clone;
    }
  }
}
