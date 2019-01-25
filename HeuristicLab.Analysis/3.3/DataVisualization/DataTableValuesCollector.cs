#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator which collects the actual values of parameters and adds them to a table of data values.
  /// </summary>
  [Item("DataTableValuesCollector", "An operator which collects the actual values of parameters and adds them to a table of data values.")]
  [StorableType("B3409421-D7F0-4C79-B8F8-B7E77E7C554F")]
  public class DataTableValuesCollector : ValuesCollector {
    public ValueLookupParameter<DataTable> DataTableParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["DataTable"]; }
    }
    private IFixedValueParameter<BoolValue> StartIndexZeroParameter {
      get { return (FixedValueParameter<BoolValue>)Parameters["StartIndexZero"]; }
    }

    public bool StartIndexZero {
      get { return StartIndexZeroParameter.Value.Value; }
      set { StartIndexZeroParameter.Value.Value = value; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    protected DataTableValuesCollector(StorableConstructorFlag _) : base(_) { }
    protected DataTableValuesCollector(DataTableValuesCollector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTableValuesCollector(this, cloner);
    }
    #endregion
    public DataTableValuesCollector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DataTable>("DataTable", "The table of data values where the collected values should be stored."));
      Parameters.Add(new FixedValueParameter<BoolValue>("StartIndexZero", "True, if the collected data values should start with index 0, otherwise false.", new BoolValue(true), false));
      StartIndexZeroParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (!Parameters.ContainsKey("StartIndexZero")) {
        Parameters.Add(new FixedValueParameter<BoolValue>("StartIndexZero", "True, if the collected data values should start with index 0, otherwise false.", new BoolValue(true), false));
        StartIndexZeroParameter.Hidden = true;
      }
      #endregion
    }

    public override IOperation Apply() {
      DataTable table = DataTableParameter.ActualValue;
      if (table == null) {
        table = new DataTable(DataTableParameter.ActualName);
        DataTableParameter.ActualValue = table;
      }

      foreach (IParameter param in CollectedValues) {
        ILookupParameter lookupParam = param as ILookupParameter;
        string name = lookupParam != null ? lookupParam.TranslatedName : param.Name;

        if (param.ActualValue is DoubleValue) {
          AddValue(table, (param.ActualValue as DoubleValue).Value, name, param.Description);
        } else if (param.ActualValue is IntValue) {
          AddValue(table, (param.ActualValue as IntValue).Value, name, param.Description);
        } else if (param.ActualValue is IEnumerable<DoubleValue>) {
          IEnumerable<DoubleValue> values = (IEnumerable<DoubleValue>)param.ActualValue;
          if (values.Count() <= 1) {
            foreach (DoubleValue data in values)
              AddValue(table, data != null ? data.Value : double.NaN, name, param.Description);
          } else {
            int counter = 1;
            foreach (DoubleValue data in values) {
              AddValue(table, data != null ? data.Value : double.NaN, name + " " + counter.ToString(), param.Description);
              counter++;
            }
          }
        } else if (param.ActualValue is IEnumerable<IntValue>) {
          IEnumerable<IntValue> values = (IEnumerable<IntValue>)param.ActualValue;
          if (values.Count() <= 1) {
            foreach (IntValue data in values)
              AddValue(table, data != null ? data.Value : double.NaN, name, param.Description);
          } else {
            int counter = 1;
            foreach (IntValue data in values) {
              AddValue(table, data != null ? data.Value : double.NaN, name + " " + counter.ToString(), param.Description);
              counter++;
            }
          }
        } else {
          AddValue(table, double.NaN, name, param.Description);
        }
      }
      return base.Apply();
    }

    private void AddValue(DataTable table, double data, string name, string description) {
      DataRow row;
      table.Rows.TryGetValue(name, out row);
      if (row == null) {
        row = new DataRow(name, description);
        row.VisualProperties.StartIndexZero = StartIndexZero;
        row.Values.Add(data);
        table.Rows.Add(row);
      } else {
        row.Values.Add(data);
      }
    }
  }
}
