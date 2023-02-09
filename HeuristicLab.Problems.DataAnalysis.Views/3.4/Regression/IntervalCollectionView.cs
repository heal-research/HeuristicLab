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


using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {

  [View("IntervalCollection View")]
  [Content(typeof(IntervalCollection), true)]
  public sealed partial class IntervalCollectionView : AsynchronousContentView {

    public new IntervalCollection Content {
      get => (IntervalCollection)base.Content;
      set => base.Content = value;
    }

    private DataGridView DataGridView {
      get => dataGridView;
    }

    public IntervalCollectionView() {
      InitializeComponent();
      dataGridView.AutoGenerateColumns = false;
      dataGridView.AllowUserToAddRows = false;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        DataGridView.Rows.Clear();
        DataGridView.Columns.Clear();
      } else {
        UpdateData();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      dataGridView.Enabled = Content != null;
      dataGridView.ReadOnly = ReadOnly;
    }

    private void UpdateData() {
      DataGridView.Rows.Clear();
      DataGridView.Columns.Clear();
      SetColumnNames();


      var variablesCount = Content.Count;

      DataGridViewRow[] rows = new DataGridViewRow[variablesCount];
      for (var i = 0; i < variablesCount; ++i) {
        var row = new DataGridViewRow();
        rows[i] = row;
      }
      dataGridView.Rows.AddRange(rows);

      var j = 0;
      foreach (var variableInterval in Content.GetVariableIntervals()) {
        dataGridView.Rows[j].HeaderCell.Value = variableInterval.Item1;
        dataGridView.Rows[j].Cells[0].Value = variableInterval.Item2.LowerBound;
        dataGridView.Rows[j].Cells[1].Value = variableInterval.Item2.UpperBound;
        j++;
      }

      dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
      dataGridView.Enabled = true;
      //Disable Sorting
      foreach (DataGridViewColumn column in dataGridView.Columns) {
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }
    }

    private void SetColumnNames() {
      dataGridView.Columns.Add("lowerBound", "Lower bound");
      dataGridView.Columns.Add("upperBound", "Upper bound");
    }

    private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
      if (Content == null) return;

      var key = dataGridView.Rows[e.RowIndex].HeaderCell.Value.ToString();
      var gridData = dataGridView[e.ColumnIndex, e.RowIndex].Value;
      //Cells maybe null during initialization
      var lowerBound = Content.GetInterval(key).LowerBound;
      var upperBound = Content.GetInterval(key).UpperBound;

      //Check if the input data is a string (user-defined input)
      //if so parse the toString() of the value, otherwise do a hard-cast
      //to not loose the double precision
      double parsedValue;
      if (gridData is string) {
        parsedValue = double.Parse(gridData.ToString());
      } else {
        parsedValue = (double)gridData;
      }

      if (e.ColumnIndex == 0) lowerBound = parsedValue;
      if (e.ColumnIndex == 1) upperBound = parsedValue;

      var newInterval = new Interval(lowerBound, upperBound);

      // update if there was a change
      if (!Content.GetInterval(key).Equals(newInterval))
        Content.SetInterval(key, newInterval);
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (!double.TryParse(e.FormattedValue.ToString(), out var value)) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = "Value must be a double value.";
        return;
      }

      var lowerBound = double.Parse(dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
      var upperBound = double.Parse(dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString());

      if (e.ColumnIndex == 0 && value > upperBound || e.ColumnIndex == 1 && value < lowerBound) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = "Lower bound of interval must be smaller than upper bound.";
        return;
      }

      dataGridView[e.ColumnIndex, e.RowIndex].Value = value;
    }

    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
  }
}
