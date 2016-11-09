#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Preprocessing Chart View")]
  [Content(typeof(PreprocessingChartContent), false)]
  public partial class PreprocessingChartView : PreprocessingCheckedVariablesView {

    protected PreprocessingDataTable dataTable;
    protected List<PreprocessingDataTable> dataTablePerVariable;
    protected List<DataRow> dataRows;
    protected List<DataRow> selectedDataRows;

    protected DataRowVisualProperties.DataRowChartType chartType;
    protected string chartTitle;

    private const string DEFAULT_CHART_TITLE = "Chart";
    private const int FIXED_CHART_SIZE = 300;
    private const int MAX_TABLE_AUTO_SIZE_ROWS = 3;


    public IEnumerable<double> Classification { get; set; }
    public bool IsDetailedChartViewEnabled { get; set; }

    public PreprocessingChartView() {
      InitializeComponent();
      chartType = DataRowVisualProperties.DataRowChartType.Line;
      chartTitle = DEFAULT_CHART_TITLE;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
        GenerateChart();
      }
    }

    private void InitData() {
      //Create data tables and data rows
      dataRows = Content.CreateAllDataRows(chartType);
      dataTable = new PreprocessingDataTable(chartTitle);
      dataTablePerVariable = new List<PreprocessingDataTable>();

      //add data rows to data tables according to checked item list
      foreach (var checkedItem in Content.VariableItemList.CheckedItems) {
        string variableName = Content.VariableItemList[checkedItem.Index].Value;
        PreprocessingDataTable d = new PreprocessingDataTable(variableName);
        DataRow row = GetDataRow(variableName);

        if (row != null) {
          //add row to data table
          dataTable.Rows.Add(row);

          //add row to data table per variable
          d.Rows.Add(row);
          dataTablePerVariable.Add(d);
        }
      }

      UpdateSelection();
    }

    protected override void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      base.CheckedItemsChanged(sender, checkedItems);

      foreach (IndexedItem<StringValue> item in checkedItems.Items) {
        string variableName = item.Value.Value;

        // not checked -> remove
        if (!VariableIsChecked(variableName)) {
          dataTableView.SetRowEnabled(variableName, false);
          dataTable.SelectedRows.Remove(variableName);
          dataTablePerVariable.Remove(dataTablePerVariable.Find(x => (x.Name == variableName)));
        } else {
          DataRow row = GetDataRow(variableName);
          DataRow selectedRow = GetSelectedDataRow(variableName);
          dataTableView.SetRowEnabled(variableName, true);

          PreprocessingDataTable pdt = new PreprocessingDataTable(variableName);
          pdt.Rows.Add(row);
          // dataTablePerVariable does not contain unchecked variables => reduce insert position by number of uncheckt variables to correct the index
          int uncheckedUntilVariable = checkedItemList.Content.TakeWhile(x => x.Value != variableName).Count(x => !checkedItemList.Content.ItemChecked(x));
          dataTablePerVariable.Insert(item.Index - uncheckedUntilVariable, pdt);

          //update selection
          if (selectedRow != null) {
            dataTable.SelectedRows.Add(selectedRow);
            pdt.SelectedRows.Add(selectedRow);
          }
        }
      }

      // update chart if not in all in one mode
      if (Content != null && !Content.AllInOneMode)
        GenerateChart();
    }

    private DataRow GetSelectedDataRow(string variableName) {
      foreach (DataRow row in selectedDataRows) {
        if (row.Name == variableName)
          return row;
      }
      return null;
    }
    private DataRow GetDataRow(string variableName) {
      foreach (DataRow row in dataRows) {
        if (row.Name == variableName)
          return row;
      }
      return null;
    }

    #region Add/Remove/Update Variable, Reset
    protected override void AddVariable(string name) {
      base.AddVariable(name);
      DataRow row = Content.CreateDataRow(name, chartType);
      dataTable.Rows.Add(row);
      PreprocessingDataTable d = new PreprocessingDataTable(name);
      d.Rows.Add(row);
      dataTablePerVariable.Add(d);

      if (!Content.AllInOneMode)
        GenerateChart();
    }

    // remove variable from data table and item list
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);
      dataTable.Rows.Remove(name);
      dataTablePerVariable.Remove(dataTablePerVariable.Find(x => (x.Name == name)));

      if (!Content.AllInOneMode)
        GenerateChart();
    }

    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);
      DataRow newRow = Content.CreateDataRow(name, chartType);
      dataTable.Rows.Remove(name);
      dataTable.Rows.Add(newRow);
      DataTable dt = dataTablePerVariable.Find(x => x.Rows.Find(y => y.Name == name) != null);
      if (dt != null) {
        dt.Rows.Remove(name);
        dt.Rows.Add(newRow);
      }
    }
    protected override void ResetAllVariables() {
      InitData();
    }
    #endregion

    #region Generate Charts
    protected void GenerateChart() {
      ClearTableLayout();
      if (Content.AllInOneMode) {
        GenerateSingleChartLayout();
      } else
        GenerateMultiChartLayout();
    }

    private void GenerateSingleChartLayout() {
      tableLayoutPanel.ColumnCount = 1;
      tableLayoutPanel.RowCount = 1;
      tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
      tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
      tableLayoutPanel.Controls.Add(dataTableView, 0, 0);
      dataTableView.Content = dataTable;
    }

    private void GenerateMultiChartLayout() {
      int checkedItemsCnt = 0;
      foreach (var item in Content.VariableItemList.CheckedItems)
        checkedItemsCnt++;

      // set columns and rows based on number of items
      int columns = GetNrOfMultiChartColumns(checkedItemsCnt);
      int rows = GetNrOfMultiChartRows(checkedItemsCnt, columns);

      tableLayoutPanel.ColumnCount = columns;
      tableLayoutPanel.RowCount = rows;

      List<PreprocessingDataTable>.Enumerator enumerator = dataTablePerVariable.GetEnumerator();
      for (int x = 0; x < columns; x++) {

        if (rows <= MAX_TABLE_AUTO_SIZE_ROWS)
          tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / columns));
        else
          //scrollbar is shown if there are more than 3 rows -> remove scroll bar width from total width
          tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, (tableLayoutPanel.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth) / columns));
        for (int y = 0; y < rows; y++) {
          //Add a row only when creating the first column
          if (x == 0) {
            // fixed chart size when there are more than 3 tables
            if (rows > MAX_TABLE_AUTO_SIZE_ROWS)
              tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, FIXED_CHART_SIZE));
            else
              tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / rows));
          }

          enumerator.MoveNext();
          PreprocessingDataTable d = enumerator.Current;
          AddDataTableToTableLayout(d, x, y);

        }
      }
    }
    private int GetNrOfMultiChartColumns(int itemCount) {
      int columns = 0;
      if (itemCount <= 2)
        columns = 1;
      else if (itemCount <= 6)
        columns = 2;
      else
        columns = 3;
      return columns;
    }
    private int GetNrOfMultiChartRows(int itemCount, int columns) {
      int rows = 0;
      if (columns == 3)
        rows = (itemCount + 2) / columns;
      else if (columns == 2)
        rows = (itemCount + 1) / columns;
      else
        rows = itemCount / columns;
      return rows;
    }

    private void AddDataTableToTableLayout(PreprocessingDataTable dataTable, int x, int y) {
      PreprocessingDataTableView dataView = new PreprocessingDataTableView();
      dataView.Classification = Classification;
      dataView.IsDetailedChartViewEnabled = IsDetailedChartViewEnabled;

      if (dataTable == null) {
        // dummy panel for empty field 
        Panel p = new Panel();
        p.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(p, y, x);
      } else {
        dataView.Content = dataTable;
        dataView.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(dataView, y, x);
      }
    }

    protected void ClearTableLayout() {
      //Clear out the existing controls
      tableLayoutPanel.Controls.Clear();

      //Clear out the existing row and column styles
      tableLayoutPanel.ColumnStyles.Clear();
      tableLayoutPanel.RowStyles.Clear();
      tableLayoutPanel.AutoScroll = false;
      tableLayoutPanel.AutoScroll = true;
    }
    //Remove horizontal scroll bar if visible
    private void tableLayoutPanel_Layout(object sender, LayoutEventArgs e) {
      if (tableLayoutPanel.HorizontalScroll.Visible) {
        // Add padding on the right in order to accomodate the vertical scrollbar
        int vWidth = SystemInformation.VerticalScrollBarWidth;
        tableLayoutPanel.Padding = new Padding(0, 0, vWidth, 0);
      } else {
        // Reset padding
        tableLayoutPanel.Padding = new Padding(0);
      }
    }
    #endregion

    #region Update Selection
    protected override void PreprocessingData_SelctionChanged(object sender, EventArgs e) {
      base.PreprocessingData_SelctionChanged(sender, e);
      UpdateSelection();
    }

    private void UpdateSelection() {
      //update data table selection
      selectedDataRows = Content.CreateAllSelectedDataRows(chartType);
      dataTable.SelectedRows.Clear();
      foreach (var selectedRow in selectedDataRows) {
        if (VariableIsChecked(selectedRow.Name))
          dataTable.SelectedRows.Add(selectedRow);
      }

      //update data table per variable selection
      foreach (PreprocessingDataTable d in dataTablePerVariable) {
        d.SelectedRows.Clear();
        DataRow row = selectedDataRows.Find(x => x.Name == d.Name);
        if (row != null)
          d.SelectedRows.Add(row);
      }
    }
    #endregion
  }
}
