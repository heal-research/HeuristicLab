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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  [Item("HeatMap", "Represents a heat map of double values.")]
  [StorableClass]
  public class HeatMap : DoubleMatrix {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Gradient; }
    }

    [StorableConstructor]
    protected HeatMap(bool deserializing) : base(deserializing) { }
    protected HeatMap(HeatMap original, Cloner cloner) : base(original, cloner) { }
    public HeatMap() : base() { }
    public HeatMap(int rows, int columns) : base(rows, columns) { }
    public HeatMap(int rows, int columns, IEnumerable<string> columnNames) : base(rows, columns, columnNames) { }
    public HeatMap(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(rows, columns, columnNames, rowNames) { }
    public HeatMap(double[,] elements) : base(elements) { }
    public HeatMap(double[,] elements, IEnumerable<string> columnNames) : base(elements, columnNames) { }
    public HeatMap(double[,] elements, IEnumerable<string> columnNames, IEnumerable<string> rowNames) : base(elements, columnNames, rowNames) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HeatMap(this, cloner);
    }

    public override string ToString() {
      return ItemName;
    }
  }
}
