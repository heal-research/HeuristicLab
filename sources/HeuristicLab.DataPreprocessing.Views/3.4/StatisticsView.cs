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
    private bool horizontal = true;
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
      var logic = Content.StatisticsLogic;
      rowsTextBox.Text = logic.GetRowCount().ToString();
      columnsTextBox.Text = logic.GetColumnCount().ToString();
      numericColumnsTextBox.Text = logic.GetNumericColumnCount().ToString();
      nominalColumnsTextBox5.Text = logic.GetNominalColumnCount().ToString();
      missingValuesTextBox.Text = logic.GetMissingValueCount().ToString();
      totalValuesTextBox.Text = (logic.GetColumnCount() * logic.GetRowCount() - logic.GetMissingValueCount()).ToString();

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

      for (int i = 0; i < logic.GetColumnCount(); i++) {
        var data = GetStatistics(i);
        for (int j = 0; j < data.Count; j++) {
          if (horizontal)
            statisticsMatrix[j, i] = data[j];
          else
            statisticsMatrix[i, j] = data[j];
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

    private List<string> GetStatistics(int varIdx) {
      List<string> list;
      var logic = Content.StatisticsLogic;
      if (logic.VariableHasType<double>(varIdx)) {
        list = GetDoubleColumns(varIdx);
      } else if (logic.VariableHasType<string>(varIdx)) {
        list = GetStringColumns(varIdx);
      } else if (logic.VariableHasType<DateTime>(varIdx)) {
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
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(statIdx),
        logic.GetMissingValueCount(statIdx).ToString(),
        logic.GetMin<double>(statIdx, double.NaN).ToString(),
        logic.GetMax<double>(statIdx, double.NaN).ToString(),
        logic.GetMedian(statIdx).ToString(),
        logic.GetAverage(statIdx).ToString(),
        logic.GetStandardDeviation(statIdx).ToString(),
        logic.GetVariance(statIdx).ToString(),
        logic.GetOneQuarterPercentile(statIdx).ToString(),
        logic.GetThreeQuarterPercentile(statIdx).ToString(),
        logic.GetMostCommonValue<double>(statIdx, double.NaN).ToString(),
        logic.GetDifferentValuesCount<double>(statIdx).ToString()
      };
    }

    private List<string> GetStringColumns(int statIdx) {
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(statIdx),
        logic.GetMissingValueCount(statIdx).ToString(),
        "", //min
        "", //max
        "", //median
        "", //average
        "", //standard deviation
        "", //variance
        "", //quarter percentile
        "", //three quarter percentile
        logic.GetMostCommonValue<string>(statIdx,string.Empty) ?? "",
        logic.GetDifferentValuesCount<string>(statIdx).ToString()
      };
    }

    private List<string> GetDateTimeColumns(int statIdx) {
      var logic = Content.StatisticsLogic;
      return new List<string> {
        logic.GetColumnTypeAsString(statIdx),
        logic.GetMissingValueCount(statIdx).ToString(),
        logic.GetMin<DateTime>(statIdx, DateTime.MinValue).ToString(),
        logic.GetMax<DateTime>(statIdx, DateTime.MinValue).ToString(),
        logic.GetMedianDateTime(statIdx).ToString(),
        logic.GetAverageDateTime(statIdx).ToString(),
        logic.GetStandardDeviation(statIdx).ToString(),
        logic.GetVariance(statIdx).ToString(),
        logic.GetOneQuarterPercentile(statIdx).ToString(),
        logic.GetThreeQuarterPercentile(statIdx).ToString(),
        logic.GetMostCommonValue<DateTime>(statIdx, DateTime.MinValue).ToString(),
        logic.GetDifferentValuesCount<DateTime>(statIdx).ToString()
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
