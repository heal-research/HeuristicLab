#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Histogram View")]
  [Content(typeof(HistogramContent), true)]
  public partial class HistogramView : PreprocessingChartView {
    private const string HISTOGRAM_CHART_TITLE = "Histogram";

    public HistogramView() {
      InitializeComponent();
      chartType = DataRowVisualProperties.DataRowChartType.Histogram;
      chartTitle = HISTOGRAM_CHART_TITLE;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        classifierComboBox.Items.Clear();
        classifierComboBox.Items.Add("None");

        foreach (string var in Content.GetVariableNamesForHistogramClassification()) {
          classifierComboBox.Items.Add(var);
        }

        if (classifierComboBox.SelectedItem == null && Content.ClassifierVariableIndex < classifierComboBox.Items.Count) {
          classifierComboBox.SelectedIndex = Content.ClassifierVariableIndex;
        }
      }
    }

    public new HistogramContent Content {
      get { return (HistogramContent)base.Content; }
      set { base.Content = value; }
    }

    private void classifierComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (classifierComboBox.SelectedItem == null)
        return;

      if (classifierComboBox.SelectedIndex != 0) {
        int columndIndex = Content.PreprocessingData.GetColumnIndex(classifierComboBox.SelectedItem.ToString());
        Classification = Content.PreprocessingData.GetValues<double>(columndIndex);
      } else {
        Classification = null;
      }

      Content.ClassifierVariableIndex = classifierComboBox.SelectedIndex;
      if (Content.IsDetailedChartViewEnabled != IsDetailedChartViewEnabled) {
        displayDetailsCheckBox.Checked = Content.IsDetailedChartViewEnabled;
      } else {
        GenerateChart();
      }
    }
    private void displayDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      bool isChecked = displayDetailsCheckBox.Checked;
      if (IsDetailedChartViewEnabled != isChecked) {
        IsDetailedChartViewEnabled = isChecked;
        Content.IsDetailedChartViewEnabled = isChecked;
        GenerateChart();
      }
    }
  }
}
