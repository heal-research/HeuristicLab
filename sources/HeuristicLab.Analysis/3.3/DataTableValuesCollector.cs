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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a table of data values.
  /// </summary>
  [Item("DataTableValuesCollector", "An operator which collects the actual values of parameters and adds them to a table of data values.")]
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
        if (param.ActualValue is DoubleValue) {
          AddValue(table, (param.ActualValue as DoubleValue).Value, param.Name, param.Description);
        } else if (param.ActualValue is IEnumerable<DoubleValue>) {
          int counter = 0;
          foreach (DoubleValue data in (param.ActualValue as IEnumerable<DoubleValue>)) {
            AddValue(table, data.Value, param.Name + counter.ToString(), param.Description);
            counter++;
          }
        } else {
          AddValue(table, double.NaN, param.Name, param.Description);
        }
      }
      return base.Apply();
    }

    private void AddValue(DataTable table, double data, string name, string description) {
      DataRow row;
      table.Rows.TryGetValue(name, out row);
      if (row == null) {
        row = new DataRow(name, description);
        row.Values.Add(data);
        table.Rows.Add(row);
      } else {
        row.Values.Add(data);
      }
    }
  }
}
