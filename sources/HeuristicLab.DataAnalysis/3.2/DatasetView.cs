#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.DataAnalysis {
  public partial class DatasetView : EditorBase {
    public Dataset Dataset {
      get { return (Dataset)Item; }
      set {
        Item = value;
        Refresh();
      }
    }
    public DatasetView()
      : base() {
      InitializeComponent();
      // format all cells with the round-trip formatter to make sure that values that are exported and imported to
      // another C# app (HL2) have the same numeric value
      dataGridView.DefaultCellStyle.Format = "r";

      //events for virtual mode of datagrid
      this.dataGridView.VirtualMode = true;
      this.dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(dataGridView_CellValueNeeded);
    }

    public DatasetView(Dataset dataset)
      : this() {
      this.Dataset = dataset;
      contextMenuStrip.Items.Add(new ToolStripSeparator());
      foreach (IDatasetManipulator manipulator in ApplicationManager.Manager.GetInstances<IDatasetManipulator>()) {
        contextMenuStrip.Items.Add(new ToolStripButton(manipulator.Action, null, delegate(object source, EventArgs args) {
          manipulator.Execute(Dataset);
          Refresh();
        }));
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (Dataset != null) {
        // DataGridView is bitching around. When it's columnCount (maybe also rowCount) is changed it creates
        // new column objects and they have SortMode set to 'automatic'. However this is not allowed if the
        // selectionmode is set to 'ColumnHeaderSelect' at the same time, resulting in an exception.
        // A solution is to set the SelectionMode to CellSelect before any changes. After the columns
        // have been updated (and their SortMode set to 'NotSortable') we switch back to SelectionMode=ColumnHeaderSelect.
        dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
        //dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        int rows = Dataset.Rows;
        int columns = Dataset.Columns;
        nameTextBox.Text = Dataset.Name;
        rowsTextBox.Text = rows + "";
        columnsTextBox.Text = columns + "";
        dataGridView.ColumnCount = columns;
        dataGridView.RowCount = rows;
        //for (int i = 0; i < rows; i++) {
        //  for (int j = 0; j < columns; j++) {
        //    dataGridView.Rows[i].Cells[j].Value = Dataset.GetValue(i, j);
        //    dataGridView.Rows[i].HeaderCell.Value = i.ToString();
        //  }
        //}
        for (int i = 0; i < columns; i++) {
          dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; // SortMode has to be NotSortable to allow ColumnHeaderSelect
          dataGridView.Columns[i].Name = GetColumnName(i);
          dataGridView.Columns[i].HeaderText = GetColumnName(i) + System.Environment.NewLine + "(" + i + ")";
          dataGridView.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter;
        }
        dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect; // switch back to column selection
        //dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
      } else {
        rowsTextBox.Text = "1";
        columnsTextBox.Text = "1";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 1;
      }
      UpdateRowHeaders();
      this.dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
    }

    private void UpdateRowHeaders() {
      for (int i = dataGridView.FirstDisplayedScrollingRowIndex; i < dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(true); i++)
        dataGridView.Rows[i].HeaderCell.Value = i.ToString();
      this.dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      dataGridView.Invalidate();
    }

    private void dataGridView_Scroll(object sender, ScrollEventArgs e) {
      UpdateRowHeaders();
    }

    private void dataGridView_Resize(object sender, EventArgs e) {
      UpdateRowHeaders();
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, e.ColumnIndex, (string)e.FormattedValue);
        e.Cancel = false;
      } else {
        e.Cancel = true;
      }
    }

    private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
      if (this.Dataset == null)
        e.Value = null;
      else
        e.Value = Dataset.GetValue(e.RowIndex, e.ColumnIndex);
    }

    private void SetArrayElement(int row, int column, string element) {
      double result;
      double.TryParse(element, out result);
      if (result != Dataset.GetValue(row, column)) {
        Dataset.SetValue(row, column, result);
      }
    }

    private bool ValidateData(string element) {
      double result;
      return element != null && double.TryParse(element, out result);
    }

    private void scaleValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (DataGridViewColumn column in dataGridView.SelectedColumns) {
        Dataset.ScaleVariable(column.Index);
        column.Name = GetColumnName(column.Index) + " [scaled]";
      }
      Refresh();
    }

    private void originalValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (DataGridViewColumn column in dataGridView.SelectedColumns) {
        Dataset.UnscaleVariable(column.Index);
        column.Name = GetColumnName(column.Index);
      }
      Refresh();
    }

    private string GetColumnName(int index) {
      if (Dataset.Columns == dataGridView.Columns.Count) {
        return Dataset.GetVariableName(index);
      } else {
        return "Var " + index;
      }
    }

    private void showScalingToolStripMenuItem_Click(object sender, EventArgs e) {
      ManualScalingControl scalingControl = new ManualScalingControl(false);
      double[,] scalingParameters = new double[2, Dataset.Columns];
      for (int i = 0; i < Dataset.Columns; i++) {
        scalingParameters[0, i] = Dataset.ScalingFactor[i];
        scalingParameters[1, i] = Dataset.ScalingOffset[i];
      }
      scalingControl.Data = scalingParameters;
      scalingControl.ShowDialog();
    }

    private void scaleValuesmanuallyToolStripMenuItem_Click(object sender, EventArgs e) {
      ManualScalingControl scalingControl = new ManualScalingControl(true);
      double[,] scalingParameters = new double[2, Dataset.Columns];
      for (int i = 0; i < Dataset.Columns; i++) {
        scalingParameters[0, i] = Dataset.ScalingFactor[i];
        scalingParameters[1, i] = Dataset.ScalingOffset[i];
      }
      scalingControl.Data = scalingParameters;
      if (scalingControl.ShowDialog() == DialogResult.OK) {
        for (int i = 0; i < Dataset.Columns; i++) {
          Dataset.ScaleVariable(i, scalingControl.Data[0, i], scalingControl.Data[1, i]);
        }
      }
      Refresh();
    }


  }
}