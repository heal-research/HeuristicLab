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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  public partial class VariableTrackbar : UserControl {
    private readonly string variableName;
    private const double FACTOR = 1000;

    public double Value {
      get { return trackBar.Value / FACTOR; }
    }

    public string VariableName {
      get { return variableName; }
    }

    public VariableTrackbar(string variableName, IEnumerable<double> values) {
      InitializeComponent();
      this.variableName = variableName;
      groupBox.Text = variableName;
      var valuesArr = values.ToArray();
      this.valueTextBox.Text = string.Format("{0:0.000e+00}", valuesArr.Median());
      boxPlotChart.Series["DataSeries"].Points.Clear();
      boxPlotChart.Series["DataSeries"].Points.DataBindY(valuesArr);
      boxPlotChart.Series["BoxPlot"].ChartType = SeriesChartType.BoxPlot;
      boxPlotChart.Series["BoxPlot"]["BoxPlotSeries"] = "DataSeries";
      boxPlotChart.Series["BoxPlot"]["BoxPlotWhiskerPercentile"] = "5";
      boxPlotChart.Series["BoxPlot"]["BoxPlotPercentile"] = "30";
      boxPlotChart.Series["BoxPlot"]["BoxPlotShowAverage"] = "true";
      boxPlotChart.Series["BoxPlot"]["BoxPlotShowMedian"] = "true";
      boxPlotChart.Series["BoxPlot"]["BoxPlotShowUnusualValues"] = "true";
      boxPlotChart.ChartAreas[0].AxisY.Minimum = valuesArr.Min() ;
      boxPlotChart.ChartAreas[0].AxisY.Maximum = valuesArr.Max() ;
      

      trackBar.Minimum = (int)Math.Round(valuesArr.Min() * FACTOR);
      trackBar.Maximum = (int)Math.Round(valuesArr.Max() * FACTOR);
      trackBar.Value = (int)Math.Round(valuesArr.Median() * FACTOR);
      trackBar.TickStyle = TickStyle.None;
      trackBar.Tag = variableName;
    }

    private void TrackBarValueChanged(object sender, EventArgs e) {
      valueTextBox.Text = string.Format("{0:0.000e+00}", trackBar.Value / FACTOR);
      RaiseValueChanged(EventArgs.Empty);
    }

    public event EventHandler ValueChanged;
    private void RaiseValueChanged(EventArgs e) {
      var handler = ValueChanged;
      if (handler != null) handler(this, e);
    }
  }
}
