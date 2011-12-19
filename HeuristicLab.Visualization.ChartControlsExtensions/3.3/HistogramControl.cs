#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public partial class HistogramControl : UserControl {
    private static readonly string SeriesName = "Histogram";

    private List<double> points;
    public int NumberOfBins {
      get { return (int)binsNumericUpDown.Value; }
      set { binsNumericUpDown.Value = value; }
    }

    public int MinimumNumberOfBins {
      get { return (int)binsNumericUpDown.Minimum; }
      set { binsNumericUpDown.Minimum = value; }
    }

    public int MaximumNumberOfBins {
      get { return (int)binsNumericUpDown.Maximum; }
      set { binsNumericUpDown.Maximum = value; }
    }

    public int IncrementNumberOfBins {
      get { return (int)binsNumericUpDown.Increment; }
      set { binsNumericUpDown.Increment = value; }
    }

    public bool CalculateExactBins {
      get { return exactCheckBox.Checked; }
      set { exactCheckBox.Checked = value; }
    }

    public bool ShowExactCheckbox {
      get { return exactCheckBox.Visible; }
      set { exactCheckBox.Visible = value; }
    }

    public HistogramControl() {
      InitializeComponent();
      points = new List<double>();
      InitializeChart();
    }

    public void AddPoint(double point) {
      points.Add(point);
      UpdateHistogram();
    }

    public void AddPoints(IEnumerable<double> points) {
      this.points.AddRange(points);
      UpdateHistogram();
    }

    public void ClearPoints() {
      points.Clear();
      UpdateHistogram();
    }

    private void InitializeChart() {
      chart.Series.Clear();
      Series s = chart.Series.Add(SeriesName);
      s.ChartType = SeriesChartType.Column;
      s.BorderColor = Color.Black;
      s.BorderWidth = 1;
      s.BorderDashStyle = ChartDashStyle.Solid;
    }

    private void UpdateHistogram() {
      if (InvokeRequired) {
        Invoke((Action)UpdateHistogram, null);
        return;
      }
      Series histogramSeries = chart.Series[SeriesName];
      histogramSeries.Points.Clear();

      if (!points.Any()) return;
      int bins = (int)binsNumericUpDown.Value;

      double minValue = points.Min();
      double maxValue = points.Max();
      double intervalWidth = (maxValue - minValue) / bins;
      if (intervalWidth <= 0) return;

      if (!exactCheckBox.Checked) {
        intervalWidth = HumanRoundRange(intervalWidth);
        minValue = Math.Floor(minValue / intervalWidth) * intervalWidth;
        maxValue = Math.Ceiling(maxValue / intervalWidth) * intervalWidth;
      }

      double current = minValue, intervalCenter = intervalWidth / 2.0;
      int count = 0;
      foreach (double v in points.OrderBy(x => x)) {
        while (v > current + intervalWidth) {
          histogramSeries.Points.AddXY(current + intervalCenter, count);
          current += intervalWidth;
          count = 0;
        }
        count++;
      }
      histogramSeries.Points.AddXY(current + intervalCenter, count);

      histogramSeries["PointWidth"] = "1";

      ChartArea chartArea = chart.ChartAreas[histogramSeries.ChartArea];
      chartArea.AxisY.Title = "Frequency";
      chartArea.AxisX.Minimum = minValue;
      chartArea.AxisX.Maximum = maxValue;

      double axisInterval = intervalWidth;
      while ((maxValue - minValue) / axisInterval > 10.0) {
        axisInterval *= 2.0;
      }
      chartArea.AxisX.Interval = axisInterval;
    }

    private double HumanRoundRange(double range) {
      double base10 = Math.Pow(10.0, Math.Floor(Math.Log10(range)));
      double rounding = range / base10;
      if (rounding <= 1.5) rounding = 1;
      else if (rounding <= 2.25) rounding = 2;
      else if (rounding <= 3.75) rounding = 2.5;
      else if (rounding <= 7.5) rounding = 5;
      else rounding = 10;
      return rounding * base10;
    }

    private void binsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      UpdateHistogram();
    }

    private void exactCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateHistogram();
    }
  }
}
