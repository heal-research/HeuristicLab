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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Statistics View")]
  [Content(typeof(StatisticsContent), true)]
  public partial class StatisticsView : ItemView {
    private bool horizontal = false;
    private StringMatrix statisticsMatrix;
    private static readonly string[] StatisticsNames = new[] {
      "Type",
      "Missing Values",
      "Min",
      "Max",
      "Median",
      "Average",
      "Std. Deviation",
      "Variance",
      "25th Percentile",
      "75th Percentile",
      "Most Common Value",
      "Num. diff. Values"
    };

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
        rowsTextBox.Text = string.Empty;
        columnsTextBox.Text = string.Empty;
        numericColumnsTextBox.Text = string.Empty;
        nominalColumnsTextBox5.Text = string.Empty;
        missingValuesTextBox.Text = string.Empty;
        totalValuesTextBox.Text = string.Empty;
        stringMatrixView.Content = null;
        statisticsMatrix = null;
      } else {
        UpdateData();
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += Content_Changed;
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= Content_Changed;
      base.DeregisterContentEvents();
    }

    private void UpdateData(Dictionary<string, bool> oldVisibility = null) {
      var data = Content.PreprocessingData;
      rowsTextBox.Text = data.Rows.ToString();
      columnsTextBox.Text = data.Columns.ToString();
      numericColumnsTextBox.Text = GetColumnCount<double>().ToString();
      nominalColumnsTextBox5.Text = GetColumnCount<string>().ToString();
      missingValuesTextBox.Text = data.GetMissingValueCount().ToString();
      totalValuesTextBox.Text = (data.Rows * data.Rows - data.GetMissingValueCount()).ToString();

      var variableNames = Content.PreprocessingData.VariableNames.ToList();
      if (horizontal)
        statisticsMatrix = new StringMatrix(StatisticsNames.Length, Content.PreprocessingData.Columns) {
          RowNames = StatisticsView.StatisticsNames,
          ColumnNames = variableNames
        };
      else
        statisticsMatrix = new StringMatrix(Content.PreprocessingData.Columns, StatisticsNames.Length) {
          RowNames = variableNames,
          ColumnNames = StatisticsView.StatisticsNames
        };

      for (int i = 0; i < data.Columns; i++) {
        var statistics = GetStatistics(i);
        for (int j = 0; j < statistics.Count; j++) {
          if (horizontal)
            statisticsMatrix[j, i] = statistics[j];
          else
            statisticsMatrix[i, j] = statistics[j];
        }
      }

      stringMatrixView.Parent.SuspendRepaint();
      stringMatrixView.Content = statisticsMatrix;

      var grid = stringMatrixView.DataGridView;
      int idx = 0;
      var list = horizontal ? grid.Columns : grid.Rows as IList;
      foreach (DataGridViewBand band in list) {
        var variable = variableNames[idx++];
        if (oldVisibility != null) {
          band.Visible = !oldVisibility.ContainsKey(variable) || oldVisibility[variable];
        }
      }
      if (horizontal)
        stringMatrixView.UpdateColumnHeaders();
      else
        stringMatrixView.UpdateRowHeaders();

      stringMatrixView.DataGridView.AutoResizeColumns();
      stringMatrixView.Parent.ResumeRepaint(true);
    }

    public int GetColumnCount<T>() {
      int count = 0;
      for (int i = 0; i < Content.PreprocessingData.Columns; ++i) {
        if (Content.PreprocessingData.VariableHasType<T>(i)) {
          ++count;
        }
      }
      return count;
    }

    private List<string> GetStatistics(int varIdx) {
      List<string> list;
      var data = Content.PreprocessingData;
      if (data.VariableHasType<double>(varIdx)) {
        list = GetDoubleColumns(varIdx);
      } else if (data.VariableHasType<string>(varIdx)) {
        list = GetStringColumns(varIdx);
      } else if (data.VariableHasType<DateTime>(varIdx)) {
        list = GetDateTimeColumns(varIdx);
      } else {
        list = new List<string>();
        for (int j = 0; j < StatisticsNames.Length; ++j) {
          list.Add("unknown column type");
        }
      }
      return list;
    }

    private List<string> GetDoubleColumns(int statIdx) {
      var data = Content.PreprocessingData;
      return new List<string> {
        data.GetVariableType(statIdx).Name,
        data.GetMissingValueCount(statIdx).ToString(),
        data.GetMin<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetMax<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetMedian<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetMean<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetStandardDeviation<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetVariance<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetQuantile<double>(0.25, statIdx, emptyValue: double.NaN).ToString(),
        data.GetQuantile<double>(0.75, statIdx, emptyValue: double.NaN).ToString(),
        data.GetMode<double>(statIdx, emptyValue: double.NaN).ToString(),
        data.GetDistinctValues<double>(statIdx).ToString()
      };
    }

    private List<string> GetStringColumns(int statIdx) {
      var data = Content.PreprocessingData;
      return new List<string> {
        data.GetVariableType(statIdx).Name,
        data.GetMissingValueCount(statIdx).ToString(),
        "", // data.GetMin<string>(statIdx, emptyValue: string.Empty), //min
        "", // data.GetMax<string>(statIdx, emptyValue: string.Empty), //max
        "", // data.GetMedian<string>(statIdx, emptyValue: string.Empty), //median
        "", //average
        "", //standard deviation
        "", //variance
        "", // data.GetQuantile<string>(0.25, statIdx, emptyValue: string.Empty), //quarter percentile
        "", // data.GetQuantile<string>(0.75, statIdx, emptyValue: string.Empty), //three quarter percentile
        data.GetMode<string>(statIdx, emptyValue: string.Empty),
        data.GetDistinctValues<string>(statIdx).ToString()
      };
    }

    private List<string> GetDateTimeColumns(int statIdx) {
      var data = Content.PreprocessingData;
      return new List<string> {
        data.GetVariableType(statIdx).Name,
        data.GetMissingValueCount(statIdx).ToString(),
        data.GetMin<DateTime>(statIdx).ToString(),
        data.GetMax<DateTime>(statIdx).ToString(),
        data.GetMedian<DateTime>(statIdx).ToString(),
        data.GetMean<DateTime>(statIdx).ToString(),
        "", // should be of type TimeSpan //data.GetStandardDeviation<DateTime>(statIdx).ToString(),
        "", // should be of type TimeSpan //data.GetVariance<DateTime>(statIdx).ToString(),
        data.GetQuantile<DateTime>(0.25, statIdx).ToString(),
        data.GetQuantile<DateTime>(0.75, statIdx).ToString(),
        data.GetMode<DateTime>(statIdx).ToString(),
        data.GetDistinctValues<DateTime>(statIdx).ToString()
      };
    }

    private void Content_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      UpdateData();
    }

    #region Show/Hide Variables
    private void checkInputsTargetButton_Click(object sender, EventArgs e) {
      var grid = stringMatrixView.DataGridView;
      var list = horizontal ? grid.Columns : grid.Rows as IList;
      var variableNames = Content.PreprocessingData.VariableNames.ToList();
      int idx = 0;
      foreach (DataGridViewBand band in list) {
        var variable = variableNames[idx++];
        bool isInputTarget = Content.PreprocessingData.InputVariables.Contains(variable)
                             || Content.PreprocessingData.TargetVariable == variable;
        band.Visible = isInputTarget;
        if (horizontal)
          stringMatrixView.UpdateColumnHeaders();
        else
          stringMatrixView.UpdateRowHeaders();
      }

    }
    private void checkAllButton_Click(object sender, EventArgs e) {
      var grid = stringMatrixView.DataGridView;
      var list = horizontal ? grid.Columns : grid.Rows as IList;
      foreach (DataGridViewBand band in list) {
        band.Visible = true;
      }
      if (horizontal)
        stringMatrixView.UpdateColumnHeaders();
      else
        stringMatrixView.UpdateRowHeaders();
    }
    private void uncheckAllButton_Click(object sender, EventArgs e) {
      var grid = stringMatrixView.DataGridView;
      var list = horizontal ? grid.Columns : grid.Rows as IList;
      foreach (DataGridViewBand band in list) {
        band.Visible = false;
      }
    }
    #endregion

    #region Orientation
    private void horizontalRadioButton_CheckedChanged(object sender, EventArgs e) {
      var grid = stringMatrixView.DataGridView;
      var oldVisibility = new Dictionary<string, bool>();
      var variableNames = Content.PreprocessingData.VariableNames.ToList();
      if (stringMatrixView.Content != null) {
        var list = horizontal ? grid.Columns : grid.Rows as IList;
        int idx = 0;
        foreach (DataGridViewBand band in list) {
          var variable = variableNames[idx++];
          oldVisibility.Add(variable, band.Visible);
        }
      }
      horizontal = horizontalRadioButton.Checked;
      UpdateData(oldVisibility);
    }
    private void verticalRadioButton_CheckedChanged(object sender, EventArgs e) {
      // everything is handled in horizontalRadioButton_CheckedChanged 
    }
    #endregion
  }
}
