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
      DiscoveryService discovery = new DiscoveryService();
      IDatasetManipulator[] manipuators = discovery.GetInstances<IDatasetManipulator>();
      contextMenuStrip.Items.Add(new ToolStripSeparator());
      foreach(IDatasetManipulator manipulator in manipuators) {
        contextMenuStrip.Items.Add(new ToolStripButton(manipulator.Action,null , delegate(object source, EventArgs args) 
          { manipulator.Execute(Dataset); }));
      }
    }

    public DatasetView(Dataset dataset)
      : this() {
      this.Dataset = dataset;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
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
      }
    }

    private bool ValidateData(string element) {
      double result;
      return element != null && double.TryParse(element, out result);
    }

    private void scaleValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach(DataGridViewColumn column in dataGridView.SelectedColumns) {
        Dataset.ScaleVariable(column.Index);
        column.Name = GetColumnName(column.Index) + " [scaled]";
      }
      Refresh();
    }

    private void originalValuesToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach(DataGridViewColumn column in dataGridView.SelectedColumns) {
        Dataset.UnscaleVariable(column.Index);
        column.Name = GetColumnName(column.Index);
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