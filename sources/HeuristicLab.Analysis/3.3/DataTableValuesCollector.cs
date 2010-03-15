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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a table of data values.
  /// </summary>
  [Item("DataTableValuesCollector", "An operator which collects the actual values of parameters and adds them to a table of data values.")]
  [Creatable("Test")]
  [StorableClass]
  public class DataTableValuesCollector : ValuesCollector {
    public ValueLookupParameter<DataTable> DataTableParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["DataTable"]; }
    }

    public DataTableValuesCollector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DataTable>("DataTable", "The table of data values where the collected values should be stored."));
    }

    public override IOperation Apply() {
      DataTable table = DataTableParameter.ActualValue;
      if (table == null) {
        table = new DataTable(DataTableParameter.ActualName);
        DataTableParameter.ActualValue = table;
      }

      foreach (IParameter param in CollectedValues) {
        DoubleValue data = param.ActualValue as DoubleValue;
        if (data == null) throw new InvalidOperationException("Only double data values can be collected by a DataTableValuesCollector.");

        DataRow row;
        table.Rows.TryGetValue(param.Name, out row);
        if (row == null) {
          row = new DataRow(param.Name);
          row.Values.Add(data.Value);
          table.Rows.Add(row);
        } else {
          row.Values.Add(data.Value);
        }
      }
      return base.Apply();
    }
  }
}
