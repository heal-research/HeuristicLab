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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Statistics View")]
  [Content(typeof(StatisticsContent), true)]
  public partial class StatisticsView : ItemView {

    private List<List<string>> columnsRowsMatrix;
    private readonly int COLUMNS = 12;

    public new StatisticsContent Content {
      get { return (StatisticsContent)base.Content; }
      set { base.Content = value; }
    }

    public StatisticsView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        txtRows.Text = "";
        txtColumns.Text = "";
        txtNumericColumns.Text = "";
        txtNominalColumns.Text = "";
        txtMissingValuesTotal.Text = "";
        dataGridView.Columns.Clear();
      } else {
        UpdateData();
      }
    }

    /// <summary>
    /// Adds eventhandlers to the current instance.
    /// </summary>
    protected override void RegisterContentEvents() {
      Content.Changed += Content_Changed;
    }


    /// <summary>
    /// Removes the eventhandlers from the current instance.
    /// </summary>
    protected override void DeregisterContentEvents() {
      Content.Changed -= Content_Changed;
    }

    private void UpdateData() {
      var logic = Content.StatisticsLogic;
      var rowCount = logic.GetRowCount();
      txtRows.Text = rowCount.ToString();
      txtColumns.Text = logic.GetColumnCount().ToString();
      txtNumericColumns.Text = logic.GetNumericColumnCount().ToString();
      txtNominalColumns.Text = logic.GetNominalColumnCount().ToString();
      txtMissingValuesTotal.Text = logic.GetMissingValueCount().ToString();

      columnsRowsMatrix = new List<List<string>>();
      DataGridViewColumn[] columns = new DataGridViewColumn[COLUMNS];
      for (int i = 0; i < COLUMNS; ++i) {
        var column = new DataGridViewTextBoxColumn();
        column.SortMode = DataGridViewColumnSortMode.Automatic;
        column.FillWeight = 1;
        columns[i] = column;
      }

      columns[0].HeaderCell.Value = "Type";
      columns[1].HeaderCell.Value = "Missing Values";
      columns[2].HeaderCell.Value = "Min";
      columns[3].HeaderCell.Value = "Max";
      columns[4].HeaderCell.Value = "Median";
      columns[5].HeaderCell.Value = "Average";
      columns[6].HeaderCell.Value = "std. Deviation";
      columns[7].HeaderCell.Value = "Variance";
      columns[8].HeaderCell.Value = "25th Percentile";
      columns[9].HeaderCell.Value = "75th Percentile";
      columns[10].HeaderCell.Value = "Most Common Value";
      columns[11].HeaderCell.Value = "Num. diff. Values";

      if (rowCount > 0) {
        for (int i = 0; i < logic.GetColumnCount(); ++i) {
          columnsRowsMatrix.Add(GetList(i));
        }
      }

      dataGridView.Columns.Clear();
      dataGridView.Columns.AddRange(columns);
      dataGridView.RowCount = columnsRowsMatrix.Count;

      for (int i = 0; i < columnsRowsMatrix.Count; ++i) {
        dataGridView.Rows[i].HeaderCell.Value = logic.GetVariableName(i);
      }

      dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      dataGridView.AllowUserToResizeColumns = true;
    }

    private List<string> GetList(int i) {
      List<string> list;
      var logic = Content.StatisticsLogic;
      if (logic.VariableHasType<double>(i)) {
        list = GetDoubleColumns(i);
      } else if (logic.VariableHasType<string>(i)) {
        list = GetStringColumns(i);
      } else if (logic.VariableHasType<DateTime>(i)) {
        list = GetDateTimeColumns(i);
      } else {
        list = new List<string>();
        for (int j = 0; j < COLUMNS; ++j) {
          list.Add("unknown column type");
        }
      }
      return list;
    }

    private List<string> GetDoubleColumns(int columnIndex) {
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(columnIndex),
        logic.GetMissingValueCount(columnIndex).ToString(),
        logic.GetMin<double>(columnIndex,double.NaN).ToString(),
        logic.GetMax<double>(columnIndex,double.NaN).ToString(),
        logic.GetMedian(columnIndex).ToString(),
        logic.GetAverage(columnIndex).ToString(),
        logic.GetStandardDeviation(columnIndex).ToString(),
        logic.GetVariance(columnIndex).ToString(),
        logic.GetOneQuarterPercentile(columnIndex).ToString(),
        logic.GetThreeQuarterPercentile(columnIndex).ToString(),
        logic.GetMostCommonValue<double>(columnIndex,double.NaN).ToString(),
        logic.GetDifferentValuesCount<double>(columnIndex).ToString()
      };
    }

    private List<string> GetStringColumns(int columnIndex) {
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(columnIndex),
        logic.GetMissingValueCount(columnIndex).ToString(),
        "", //min
        "", //max
        "", //median
        "", //average
        "", //standard deviation
        "", //variance
        "", //quarter percentile
        "", //three quarter percentile
        logic.GetMostCommonValue<string>(columnIndex,string.Empty) ?? "",
        logic.GetDifferentValuesCount<string>(columnIndex).ToString()
      };
    }

    private List<string> GetDateTimeColumns(int columnIndex) {
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(columnIndex),
        logic.GetMissingValueCount(columnIndex).ToString(),
        logic.GetMin<DateTime>(columnIndex,DateTime.MinValue).ToString(),
        logic.GetMax<DateTime>(columnIndex,DateTime.MinValue).ToString(),
        logic.GetMedianDateTime(columnIndex).ToString(),
        logic.GetAverageDateTime(columnIndex).ToString(),
        logic.GetStandardDeviation(columnIndex).ToString(),
        logic.GetVariance(columnIndex).ToString(), //variance
        logic.GetOneQuarterPercentile(columnIndex).ToString(),
        logic.GetThreeQuarterPercentile(columnIndex).ToString(),
        logic.GetMostCommonValue<DateTime>(columnIndex,DateTime.MinValue).ToString(),
        logic.GetDifferentValuesCount<DateTime>(columnIndex).ToString()
      };
    }

    private void dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
      if (Content != null && e.RowIndex < columnsRowsMatrix.Count && e.ColumnIndex < columnsRowsMatrix[0].Count) {
        e.Value = columnsRowsMatrix[e.RowIndex][e.ColumnIndex];
      }
    }

    private void Content_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      switch (e.Type) {
        case DataPreprocessingChangedEventType.DeleteColumn:
          columnsRowsMatrix.RemoveAt(e.Column);
          break;
        case DataPreprocessingChangedEventType.AddColumn:
          columnsRowsMatrix.Insert(e.Row, GetList(e.Column));
          dataGridView.RowCount++;
          break;
        case DataPreprocessingChangedEventType.ChangeItem:
          columnsRowsMatrix[e.Column] = GetList(e.Column);
          break;
        case DataPreprocessingChangedEventType.DeleteRow:
        case DataPreprocessingChangedEventType.AddRow:
        default:
          for (int i = 0; i < columnsRowsMatrix.Count; ++i) {
            columnsRowsMatrix[i] = GetList(e.Column);
          }
          break;
      }
      dataGridView.Refresh();
    }
  }
}
