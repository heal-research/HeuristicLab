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
using HeuristicLab.Analysis;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Line Chart View")]
  [Content(typeof(LineChartContent), true)]
  public partial class LineChartView : PreprocessingChartView {

    private const string LINE_CHART_TITLE = "Line Chart";

    public LineChartView() {
      InitializeComponent();
      chartType = DataRowVisualProperties.DataRowChartType.Line;
      chartTitle = LINE_CHART_TITLE;
    }

    public new LineChartContent Content {
      get { return (LineChartContent)base.Content; }
      set { base.Content = value; }
    }

    private void allInOneCheckBox_CheckedChanged(object sender, EventArgs e) {
      Content.AllInOneMode = allInOneCheckBox.Checked;

      GenerateChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        allInOneCheckBox.Checked = Content.AllInOneMode;
      }
    }
  }
}
