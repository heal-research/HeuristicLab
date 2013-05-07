#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// Represents history values of data tables.
  /// </summary>
  [Item("DataTableHistory", "Represents history values of data tables.")]
  [StorableClass]
  public class DataTableHistory : ItemCollection<DataTable> {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Cab; }
    }

    [StorableConstructor]
    protected DataTableHistory(bool deserializing) : base(deserializing) { }
    protected DataTableHistory(DataTableHistory original, Cloner cloner) : base(original, cloner) { }
    public DataTableHistory() : base() { }
    public DataTableHistory(IEnumerable<DataTable> collection) : base(new ItemCollection<DataTable>(collection)) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTableHistory(this, cloner);
    }
  }
}
