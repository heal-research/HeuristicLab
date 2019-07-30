#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Analysis.Views;
using HeuristicLab.Collections;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Preprocessing Chart View")]
  [Content(typeof(PreprocessingChartContent), false)]
  public partial class PreprocessingChartView : PreprocessingCheckedVariablesView {
    protected Dictionary<string, DataTable> dataTables;
    protected Dictionary<string, DataTableView> dataTableViews;

    public static readonly Color[] Colors = {
      Color.FromArgb(59, 136, 239), Color.FromArgb(252, 177, 59), Color.FromArgb(226, 64, 10),
      Color.FromArgb(5, 100, 146), Color.FromArgb(191, 191, 191), Color.FromArgb(26, 59, 105),
      Color.FromArgb(255, 226, 126), Color.FromArgb(18, 156, 221), Color.FromArgb(202, 107, 75),
      Color.FromArgb(0, 92, 219), Color.FromArgb(243, 210, 136), Color.FromArgb(80, 99, 129),
      Color.FromArgb(241, 185, 168), Color.FromArgb(224, 131, 10), Color.FromArgb(120, 147, 190)
    };

    public PreprocessingChartView() {
      InitializeComponent();
      dataTables = new Dictionary<string, DataTable>();
      dataTableViews = new Dictionary<string, DataTableView>();
      scrollPanel.HorizontalScroll.Visible = false;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
        GenerateLayout();
      }
    }

    protected virtual int GetNumberOfVisibleDataTables() {
      return Content.VariableItemList.CheckedItems.Count();
    }

    protected virtual IEnumerable<DataTableView> GetVisibleDataTables() {
      foreach (var name in Content.VariableItemList.CheckedItems) {
        if (!dataTableViews.ContainsKey(name.Value.Value))
          dataTableViews.Add(name.Value.Value, new DataTableView() { Content = dataTables[name.Value.Value], ShowChartOnly = true });
        yield return dataTableViews[name.Value.Value];
      }
    }

    protected virtual DataTable CreateDataTable(string variableName) {
      return null;
    }

    protected virtual void InitData() {
      dataTables.Clear();
      dataTableViews.Clear();
      foreach (var variable in Content.VariableItemList.Select(v => v.Value)) {
        dataTables.Add(variable, CreateDataTable(variable));
      }
    }

    protected override void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      base.CheckedItemsChanged(sender, checkedItems);

      GenerateLayout();
    }


    #region Add/Remove/Update Variable, Reset
    protected override void AddVariable(string name) {
      base.AddVariable(name);
      dataTables.Add(name, CreateDataTable(name));

      GenerateLayout();
    }

    // remove variable from data table and item list
    protected override void RemoveVariable(string name) {
      base.RemoveVariable(name);
      dataTables.Remove(name);
      dataTableViews.Remove(name);

      GenerateLayout();
    }

    protected override void UpdateVariable(string name) {
      base.UpdateVariable(name);
      dataTables.Remove(name);
      var newDataTable = CreateDataTable(name);
      dataTables.Add(name, newDataTable);
      dataTableViews[name].Content = newDataTable;
      GenerateLayout();
    }
    protected override void ResetAllVariables() {
      InitData();
    }
    #endregion

    protected override void CheckedChangedUpdate() {
      GenerateLayout();
    }

    #region Generate Layout
    protected void GenerateLayout() {
      if (SuppressCheckedChangedUpdate)
        return;

      scrollPanel.SuspendRepaint();

      ClearTableLayout();

      int nrCharts = GetNumberOfVisibleDataTables();

      // Set columns and rows based on number of items
      int columns = Math.Min(nrCharts, (int)columnsNumericUpDown.Value);
      int rows = (int)Math.Ceiling((float)nrCharts / columns);

      tableLayoutPanel.ColumnCount = Math.Max(columns, 0);
      tableLayoutPanel.RowCount = Math.Max(rows, 0);

      if (columns > 0 && rows > 0) {
        var width = (splitContainer.Panel2.Width - SystemInformation.VerticalScrollBarWidth) / columns;
        var height = width * 0.75f;

        using (var enumerator = GetVisibleDataTables().GetEnumerator()) {
          for (int row = 0; row < rows; row++) {
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            for (int col = 0; col < columns; col++) {
              if (row == 0) {
                // Add a column-style only when creating the first row
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width));
              }

              if (enumerator.MoveNext())
                AddDataTableToTableLayout(enumerator.Current, row, col);

            }
          }
        }
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
      }

      scrollPanel.ResumeRepaint(true);
    }

    private void AddDataTableToTableLayout(DataTableView dataTable, int row, int col) {
      if (dataTable == null) {
        // dummy panel for empty field 
        Panel p = new Panel { Dock = DockStyle.Fill };
        tableLayoutPanel.Controls.Add(p, col, row);
      } else {
        dataTable.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(dataTable, col, row);
      }
    }

    protected void ClearTableLayout() {
      //Clear out the existing controls
      tableLayoutPanel.Controls.Clear();

      //Clear out the existing row and column styles
      tableLayoutPanel.ColumnStyles.Clear();
      tableLayoutPanel.RowStyles.Clear();
    }
    //Remove horizontal scroll bar if visible
    private void tableLayoutPanel_Layout(object sender, LayoutEventArgs e) {
      if (tableLayoutPanel.HorizontalScroll.Visible) {
        // Add padding on the right in order to accomodate the vertical scrollbar
        tableLayoutPanel.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
      } else {
        // Reset padding
        tableLayoutPanel.Padding = new Padding(0);
      }
    }
    #endregion

    private void columnsNumericUpDown_ValueChanged(object sender, System.EventArgs e) {
      GenerateLayout();
    }

    private void splitContainer_Panel2_Resize(object sender, EventArgs e) {
      if (SuppressCheckedChangedUpdate)
        return;

      scrollPanel.SuspendRepaint();

      if (tableLayoutPanel.ColumnCount > 0 && tableLayoutPanel.RowCount > 0) {
        var width = (splitContainer.Panel2.Width - SystemInformation.VerticalScrollBarWidth) / tableLayoutPanel.ColumnCount;
        var height = width * 0.75f;

        for (int i = 0; i < tableLayoutPanel.RowStyles.Count - 1; i++) {
          tableLayoutPanel.RowStyles[i].Height = height;
        }
        for (int i = 0; i < tableLayoutPanel.ColumnStyles.Count; i++) {
          tableLayoutPanel.ColumnStyles[i].Width = width;
        }
      }

      scrollPanel.ResumeRepaint(true);
    }
  }
}
