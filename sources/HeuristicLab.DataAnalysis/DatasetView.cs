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

namespace HeuristicLab.DataAnalysis {
  public partial class DatasetView : EditorBase {
    public Dataset Dataset {
      get { return (Dataset)Item; }
      set {
        Item = value;
        Refresh();
      }
    }

    private double[] scalingFactor;
    private double[] scalingOffset;
    public DatasetView()
      : base() {
      InitializeComponent();
    }

    public DatasetView(Dataset dataset)
      : this() {
      this.Dataset = dataset;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if(this.scalingFactor == null) {
        this.scalingFactor = new double[Dataset.Columns];
        this.scalingOffset = new double[Dataset.Columns];
        for(int i = 0; i < scalingFactor.Length; i++) {
          scalingFactor[i] = 1.0;
          scalingOffset[i] = 0.0;
        }
      }
      if (Dataset != null) {
        int rows = Dataset.Rows;
        int columns = Dataset.Columns;
        nameTextBox.Text = Dataset.Name;
        rowsTextBox.Text = rows + "";
        columnsTextBox.Text = columns + "";
        dataGridView.ColumnCount = columns;
        dataGridView.RowCount = rows;
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < columns; j++) {
            dataGridView.Rows[i].Cells[j].Value = Dataset.GetValue(i, j);
          }
        }
        for (int i = 0; i < columns; i++) {
          dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
          dataGridView.Columns[i].Name = GetColumnName(i);
          dataGridView.Columns[i].ContextMenuStrip = contextMenuStrip;
        }
        dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
      } else {
        rowsTextBox.Text = "1";
        columnsTextBox.Text = "1";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 1;
      }
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, e.ColumnIndex, (string)e.FormattedValue);
        e.Cancel = false;
      } else {
        e.Cancel = true;
      }
    }


    private void SetArrayElement(int row, int column, string element) {
      double result;
      double.TryParse(element, out result);
      if (result != Dataset.GetValue(row, column)) {
        Dataset.SetValue(row, column, result);
        Dataset.FireChanged();
      }
    }

    private bool ValidateData(string element) {
      double result;
      return element != null && double.TryParse(element, out result);
    }

    private void exportButton_Click(object sender, EventArgs e) {
      throw new NotImplementedException();
    }

    private void scaleValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach(DataGridViewColumn column in dataGridView.SelectedColumns) {
        if(scalingFactor[column.Index] == 1.0) {
          double min = Dataset.GetMinimum(column.Index);
          double max = Dataset.GetMaximum(column.Index);
          double range = max - min;
          scalingFactor[column.Index] = range;
          scalingOffset[column.Index] = min;
          column.Name = GetColumnName(column.Index) + " [scaled]";
          for(int i = 0; i < Dataset.Rows; i++) {
            Dataset.SetValue(i, column.Index, (Dataset.GetValue(i, column.Index)-min) / range);
          }
        }
      }
      Refresh();
    }

    private void originalValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach(DataGridViewColumn column in dataGridView.SelectedColumns) {
        if(scalingFactor[column.Index] != 1.0) {
          column.Name = GetColumnName(column.Index);
          for(int i = 0; i < Dataset.Rows; i++) {
            Dataset.SetValue(i, column.Index, Dataset.GetValue(i, column.Index) * scalingFactor[column.Index] + scalingOffset[column.Index]);
          }
          scalingFactor[column.Index] = 1.0;
          scalingOffset[column.Index] = 0.0;
        }
      }
      Refresh();      
    }

    private string GetColumnName(int index) {
      if(Dataset.VariableNames.Length == dataGridView.Columns.Count) {
        return Dataset.VariableNames[index];
      } else {
        return "Var " + index;
      }
    }
  }
}