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
using System.Drawing;
using HeuristicLab.Analysis;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Histogram View")]
  [Content(typeof(HistogramContent), true)]
  public partial class HistogramView : PreprocessingChartView {
    public new HistogramContent Content {
      get { return (HistogramContent)base.Content; }
      set { base.Content = value; }
    }

    public HistogramView() {
      InitializeComponent();
      aggregationComboBox.DataSource = Enum.GetValues(typeof(DataTableVisualProperties.DataTableHistogramAggregation));
      aggregationComboBox.SelectedItem = DataTableVisualProperties.DataTableHistogramAggregation.Overlapping;
      orderComboBox.DataSource = Enum.GetValues(typeof(PreprocessingChartContent.LegendOrder));
      orderComboBox.SelectedItem = PreprocessingChartContent.LegendOrder.Alphabetically;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      groupingComboBox.Items.Clear();
      groupingComboBox.Items.Add(string.Empty);

      if (Content != null) {
        foreach (string var in PreprocessingChartContent.GetVariableNamesForGrouping(Content.PreprocessingData)) {
          groupingComboBox.Items.Add(var);
        }

        groupingComboBox.SelectedItem = Content.GroupingVariableName ?? string.Empty;
      }
    }

    protected override DataTable CreateDataTable(string variableName) {
      var aggregation = (DataTableVisualProperties.DataTableHistogramAggregation)aggregationComboBox.SelectedItem;
      var hist = HistogramContent.CreateHistogram(Content.PreprocessingData, variableName, Content.GroupingVariableName, aggregation, Content.Order);
      hist.VisualProperties.TitleFont = new Font(DefaultFont.FontFamily, 10, FontStyle.Bold);
      return hist;
    }

    private void classifierComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      Content.GroupingVariableName = groupingComboBox.SelectedItem.ToString();

      // rebuild datatables
      InitData();
      GenerateLayout();
    }

    private void aggregationComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      foreach (var dt in dataTables.Values) {
        dt.VisualProperties.HistogramAggregation = (DataTableVisualProperties.DataTableHistogramAggregation)aggregationComboBox.SelectedItem;
      }
    }

    private void orderComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content == null) return;

      Content.Order = (PreprocessingChartContent.LegendOrder)orderComboBox.SelectedItem;

      // rebuild datatables
      InitData();
      GenerateLayout();
    }
  }
}
