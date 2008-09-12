using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Charting;

namespace HeuristicLab.CEDMA.Charting {
  public partial class ResultListView : ViewBase {
    private ResultList results;
    private const string FREQUENCY = "<Frequency>";
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private double maxXJitterPercent = .1;
    private double maxYJitterPercent = .1;

    public ResultListView(ResultList results) {
      this.results = results;
      InitializeComponent();
      dataChart.Chart = new HeuristicLab.Charting.Data.Datachart(0, 0, 100, 1);
      xAxisComboBox.Items.AddRange(results.VariableNames);
      yAxisComboBox.Items.Add(FREQUENCY);
      yAxisComboBox.Items.AddRange(results.VariableNames);
    }

    private void yAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      yJitterFactor = 0.0;
      yTrackBar.Value = 0;
      UpdateChart();
    }

    private void xAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      xJitterFactor = 0.0;
      xTrackBar.Value = 0;
      UpdateChart();
    }

    private void UpdateChart() {
      if(xAxisComboBox.SelectedItem == null || yAxisComboBox.SelectedItem == null) return;
      if(yAxisComboBox.SelectedItem.Equals(FREQUENCY)) {
        CreateHistogramChart();
      } else {
        CreateScatterplot();
      }
    }

    private void CreateScatterplot() {
      double minX = double.PositiveInfinity;
      double minY = double.PositiveInfinity;
      double maxX = double.NegativeInfinity;
      double maxY = double.NegativeInfinity;

      xTrackBar.Enabled = true;
      yTrackBar.Enabled = true;
      Random random = new Random();
      Color curCol = Color.FromArgb(30, Color.Blue);
      Pen p = new Pen(curCol);
      SolidBrush b = new SolidBrush(curCol);
      IList<double> xs = results.GetValues((string)xAxisComboBox.SelectedItem);
      IList<double> ys = results.GetValues((string)yAxisComboBox.SelectedItem);
      dataChart.Chart = new HeuristicLab.Charting.Data.Datachart(0, 0, 100, 100);
      dataChart.Chart.UpdateEnabled = false;
      dataChart.Chart.Group.Add(new Axis(dataChart.Chart, 0, 0, AxisType.Both));
      dataChart.Chart.AddDataRow(HeuristicLab.Charting.Data.DataRowType.Points, p, b);
      for(int i = 0; i < xs.Count; i++) {
        double x = xs[i] + (random.NextDouble() * 2.0 - 1.0) * xJitterFactor;
        double y = ys[i] + (random.NextDouble() * 2.0 - 1.0) * yJitterFactor;
        if(double.IsInfinity(x) || x == double.MaxValue || x == double.MinValue) x = double.NaN;
        if(double.IsInfinity(y) || y == double.MaxValue || y == double.MinValue) y = double.NaN;
        if(!double.IsNaN(x) && !double.IsNaN(y)) {
          dataChart.Chart.AddDataPoint(0, x, y);
          if(x > maxX) maxX = x;
          if(y > maxY) maxY = y;
          if(x < minX) minX = x;
          if(y < minY) minY = y;
        }
      }
      dataChart.Chart.UpdateEnabled = true;
      if(minX<maxX && minY<maxY) dataChart.Chart.ZoomIn(minX, minY, maxX, maxY);
    }

    private void CreateHistogramChart() {
      double minX = double.PositiveInfinity;
      double minY = double.PositiveInfinity;
      double maxX = double.NegativeInfinity;
      double maxY = double.NegativeInfinity;

      xTrackBar.Enabled = false;
      yTrackBar.Enabled = false;
      Color curCol = Color.Blue;
      Pen p = new Pen(curCol);
      SolidBrush b = new SolidBrush(curCol);
      // frequency
      dataChart.Chart = new HeuristicLab.Charting.Data.Datachart(0, 0, 100, 100);
      dataChart.Chart.UpdateEnabled = false;
      dataChart.Chart.Group.Add(new Axis(dataChart.Chart, 0, 0, AxisType.Both));
      Histogram h = results.GetHistogram((string)xAxisComboBox.SelectedItem);
      for(int i = 0; i < h.Buckets; i++) {
        double lower = h.LowerValue(i);
        double upper = h.UpperValue(i);
        int freq = h.Frequency(i);
        if(lower < minX) minX = lower;
        if(upper > maxX) maxX = upper;
        if(freq > maxY) maxY = freq;
        dataChart.Chart.AddDataRow(HeuristicLab.Charting.Data.DataRowType.Bars, p, b);
        dataChart.Chart.AddDataPoint(0, lower, 0);
        dataChart.Chart.AddDataPoint(0, upper, freq);
      }
      minY = 0;
      dataChart.Chart.UpdateEnabled = true;
      if(minX < maxX && minY < maxY) dataChart.Chart.ZoomIn(minX, minY, maxX, maxY);
    }

    private void yTrackBar_ValueChanged(object sender, EventArgs e) {
      if(dataChart.Chart != null) {
        yJitterFactor = yTrackBar.Value / 100.0 * maxYJitterPercent * dataChart.Chart.Size.Height;
      }
      UpdateChart();
    }

    private void xTrackBar_ValueChanged(object sender, EventArgs e) {
      if(dataChart.Chart != null) {
        xJitterFactor = xTrackBar.Value / 100.0 * maxXJitterPercent * dataChart.Chart.Size.Width;
      }
      UpdateChart();
    }
  }
}
