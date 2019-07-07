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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Analysis.Views;
using HeuristicLab.Collections;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Line Chart View")]
  [Content(typeof(LineChartContent), true)]
  public partial class LineChartView : PreprocessingChartView {
    protected Dictionary<string, DataRow> allInOneDataRows;
    protected DataTable allInOneDataTable;
    protected DataTableView allInOneDataTableView;

    public new LineChartContent Content {
      get { return (LineChartContent)base.Content; }
      set { base.Content = value; }
    }

    public LineChartView() {
      InitializeComponent();
      sizeGroupBox.Visible = false;

      allInOneDataRows = new Dictionary<string, DataRow>();
      allInOneDataTable = new DataTable();
    }

    protected override void InitData() {
      base.InitData();

      allInOneDataRows.Clear();
      foreach (var x in Content.VariableItemList.Select((v, i) => new { variable = v.Value, i })) {
        var row = Content.CreateDataRow(x.variable, DataRowVisualProperties.DataRowChartType.Line);
        row.VisualProperties.Color = Colors[x.i % Colors.Length];
        allInOneDataRows.Add(x.variable, row);
      }

      allInOneDataTable.Rows.Clear();
      var rows = Content.VariableItemList.CheckedItems.Select(v => allInOneDataRows[v.Value.Value]);
      allInOneDataTable.Rows.AddRange(rows);
    }

    protected override int GetNumberOfVisibleDataTables() {
      return Content.AllInOneMode ? 1 : base.GetNumberOfVisibleDataTables();
    }
    protected override IEnumerable<DataTableView> GetVisibleDataTables() {
      if (Content.AllInOneMode) {
        if (allInOneDataTableView == null)
          allInOneDataTableView = new DataTableView() { Content = allInOneDataTable, ShowChartOnly = true };
        return new[] { allInOneDataTableView };
      }
      return base.GetVisibleDataTables();
    }
    protected override DataTable CreateDataTable(string variableName) {
      var dt = new DataTable();
      var row = Content.CreateDataRow(variableName, DataRowVisualProperties.DataRowChartType.Line);
      dt.Rows.Add(row);

      var validValues = row.Values.Where(x => !double.IsNaN(x) && !double.IsInfinity(x)).ToList();
      if (validValues.Any()) {
        try {
          double axisMin, axisMax, axisInterval;
          ChartUtil.CalculateOptimalAxisInterval(validValues.Min(), validValues.Max(), out axisMin, out axisMax, out axisInterval);
          dt.VisualProperties.YAxisMinimumAuto = false;
          dt.VisualProperties.YAxisMaximumAuto = false;
          dt.VisualProperties.YAxisMinimumFixedValue = axisMin;
          dt.VisualProperties.YAxisMaximumFixedValue = axisMax;
        } catch (ArgumentOutOfRangeException) { }
      }
      return dt;
    }

    private void allInOneCheckBox_CheckedChanged(object sender, EventArgs e) {
      Content.AllInOneMode = allInOneCheckBox.Checked;

      sizeGroupBox.Visible = !allInOneCheckBox.Checked;

      GenerateLayout();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        allInOneCheckBox.Checked = Content.AllInOneMode;
      }
    }

    protected override void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      base.CheckedItemsChanged(sender, checkedItems);

      foreach (IndexedItem<StringValue> item in checkedItems.Items) {
        string variableName = item.Value.Value;

        if (IsVariableChecked(variableName)) {
          // ToDo: avoid clearing all rows, but how?
          allInOneDataTable.Rows.Clear();
          var rows = Content.VariableItemList.CheckedItems.Select(r => allInOneDataRows[r.Value.Value]);
          allInOneDataTable.Rows.AddRange(rows);
        } else {
          allInOneDataTable.Rows.Remove(variableName);
        }
      }
    }

    #region Add/Remove/Update Variable, Reset
    protected override void AddVariable(string name) {
      base.AddVariable(name);
      var row = Content.CreateDataRow(name, DataRowVisualProperties.DataRowChartType.Line);
      allInOneDataTable.Rows.Add(row);
    }

    // remove variable from data table and item list
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);
      allInOneDataTable.Rows.Remove(name);
    }

    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);
      allInOneDataTable.Rows.Remove(name);
      var newRow = Content.CreateDataRow(name, DataRowVisualProperties.DataRowChartType.Line);
      allInOneDataTable.Rows.Add(newRow);
    }
    #endregion
  }
}