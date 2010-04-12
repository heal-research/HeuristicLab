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

using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// A table of data values.
  /// </summary>
  [Item("DataTable", "A table of data values.")]
  [StorableClass]
  public sealed class DataTable : NamedItem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Table; }
    }

    [Storable]
    private NamedItemCollection<DataRow> rows;
    public NamedItemCollection<DataRow> Rows {
      get { return rows; }
    }

    public DataTable()
      : base() {
      rows = new NamedItemCollection<DataRow>();
    }
    public DataTable(string name)
      : base(name) {
      rows = new NamedItemCollection<DataRow>();
    }
    public DataTable(string name, string description)
      : base(name, description) {
      rows = new NamedItemCollection<DataRow>();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      DataTable clone = new DataTable(Name, Description);
      cloner.RegisterClonedObject(this, clone);
      clone.rows = (NamedItemCollection<DataRow>)cloner.Clone(rows);
      return clone;
    }
  }
}
