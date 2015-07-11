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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Analysis.Statistics;

namespace HeuristicLab.Analysis.Views {
  public partial class HistogramControl : UserControl {
    protected static readonly string SeriesName = "Histogram";
    protected static readonly decimal bandwidthMin = 0.0000000000001m;
    protected Dictionary<string, List<double>> points;
    protected bool suppressUpdate;

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
      points = new Dictionary<string, List<double>>();
    }

    protected void InitNewRow(string name) {
      if (!points.ContainsKey(name)) {
        points.Add(name, new List<double>());
      }
    }

    protected void InitSeries(string name) {
      if (!chart.Series.Any(x => x.Name == name)) {
        Series s = chart.Series.Add(name);
        s.ChartType = SeriesChartType.Column;
        s.BorderColor = Color.Black;
        s.BorderWidth = 1;
        s.BorderDashStyle = ChartDashStyle.Solid;
      }
    }

    public void AddPoint(double point) {
      InitNewRow(SeriesName);
      InitSeries(SeriesName);
      points[SeriesName].Add(point);
      UpdateHistogram();
    }

    public void AddPoints(IEnumerable<double> points) {
      InitNewRow(SeriesName);
      InitSeries(SeriesName);
      this.points[SeriesName].AddRange(points);
      UpdateHistogram();
    }

    public void AddPoint(string name, double point, bool replace = false) {
      InitNewRow(name);
      InitSeries(name);
      if (replace) {
        points[name].Clear();
      }
      points[name].Add(point);
      UpdateHistogram();
    }

    public void AddPoints(string name, IEnumerable<double> points, bool replace = false) {
      InitNewRow(name);
      InitSeries(name);
      if (replace) {
        this.points[name].Clear();
      }
      this.points[name].AddRange(points);
      UpdateHistogram();
    }

    public void ClearPoints() {
      points.Clear();
      UpdateHistogram();
    }

    protected void UpdateHistogram(double bandwith = double.NaN) {
      if (InvokeRequired) {
        Invoke((Action<double>)UpdateHistogram, bandwith);
        return;
      }

      double overallMin = double.MaxValue;
      double overallMax = double.MinValue;
      int bins = (int)binsNumericUpDown.Value;

      chart.Series.Clear();
      foreach (var point in points) {
        if (!point.Value.Any()) continue;

        double minValue = point.Value.Min();
        double maxValue = point.Value.Max();
        double intervalWidth = (maxValue - minValue) / bins;
        if (intervalWidth <= 0) continue;

        Series histogramSeries = new Series(point.Key);
        chart.Series.Add(histogramSeries);

        if (!exactCheckBox.Checked) {
          intervalWidth = HumanRoundRange(intervalWidth);
          minValue = Math.Floor(minValue / intervalWidth) * intervalWidth;
          maxValue = Math.Ceiling(maxValue / intervalWidth) * intervalWidth;
        }

        double current = minValue, intervalCenter = intervalWidth / 2.0;
        int count = 0;
        foreach (double v in point.Value.OrderBy(x => x)) {
          while (v > current + intervalWidth) {
            histogramSeries.Points.AddXY(current + intervalCenter, count);
            current += intervalWidth;
            count = 0;
          }
          count++;
        }
        histogramSeries.Points.AddXY(current + intervalCenter, count);
        histogramSeries["PointWidth"] = "1";

        overallMax = Math.Max(overallMax, maxValue);
        overallMin = Math.Min(overallMin, minValue);

        chart.ApplyPaletteColors();
        CalculateDensity(histogramSeries, point.Value, bandwith);
      }

      if (chart.Series.Any()) {
        noDataLabel.Visible = false;
      } else {
        noDataLabel.Visible = true;
      }

      ChartArea chartArea = chart.ChartAreas[0];
      // don't show grid lines for second y-axis
      chartArea.AxisY2.MajorGrid.Enabled = false;
      chartArea.AxisY2.MinorGrid.Enabled = false;
      chartArea.AxisY.Title = "Frequency";

      double overallIntervalWidth = (overallMax - overallMin) / bins;
      double axisInterval = overallIntervalWidth;
      while ((overallMax - overallMin) / axisInterval > 10.0) {
        axisInterval *= 2.0;
      }
      chartArea.AxisX.Interval = axisInterval;
    }

    protected void CalculateDensity(Series series, List<double> row, double bandwidth = double.NaN) {
      string densitySeriesName = "Density " + series.Name;
      double stepWidth = series.Points[1].XValue - series.Points[0].XValue;
      var rowArray = row.ToArray();

      if (chart.Series.Any(x => x.Name == densitySeriesName)) {
        var ds = chart.Series.Single(x => x.Name == densitySeriesName);
        chart.Series.Remove(ds);
      }

      if (double.IsNaN(bandwidth)) {
        bandwidth = KernelDensityEstimator.EstimateBandwidth(rowArray);
        decimal bwDecimal = (decimal)bandwidth;
        if (bwDecimal < bandwidthMin) {
          bwDecimal = bandwidthMin;
          bandwidth = decimal.ToDouble(bwDecimal);
        }
        suppressUpdate = true;
        bandwidthNumericUpDown.Value = bwDecimal;
      }
      var density = KernelDensityEstimator.Density(rowArray, rowArray.Length, stepWidth, bandwidth);

      Series newSeries = new Series(densitySeriesName);
      newSeries.Color = series.Color;
      newSeries.ChartType = SeriesChartType.FastLine;
      newSeries.BorderWidth = 2;
      foreach (var d in density) {
        newSeries.Points.Add(new DataPoint(d.Item1, d.Item2));
      }

      // densities should be plotted on the second axis (different scale)
      newSeries.YAxisType = AxisType.Secondary;
      chart.Series.Add(newSeries);
    }

    protected double HumanRoundRange(double range) {
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

    private void bandwidthNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (!suppressUpdate) {
        UpdateHistogram(decimal.ToDouble(bandwidthNumericUpDown.Value));
      }
      suppressUpdate = false;
    }
  }
}
