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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// A row of data values.
  /// </summary>
  [Item("DataRow", "A row of data values.")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class DataRow : NamedItem {
    [Storable]
    private ObservableList<double> values;
    public IObservableList<double> Values {
      get { return values; }
    }

    public DataRow()
      : base() {
      values = new ObservableList<double>();
    }
    public DataRow(string name)
      : base(name) {
      values = new ObservableList<double>();
    }
    public DataRow(string name, string description)
      : base(name, description) {
      values = new ObservableList<double>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataRow clone = new DataRow(Name, Description);
      cloner.RegisterClonedObject(this, clone);
      clone.values.AddRange(values);
      return clone;
    }
  }
}
