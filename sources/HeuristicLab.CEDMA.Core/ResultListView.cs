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

namespace HeuristicLab.CEDMA.Core {
  public partial class ResultListView : ViewBase {
    private ResultList results;
    private const string FREQUENCY = "<Frequency>";

    public ResultListView(ResultList results) {
      this.results = results;
      InitializeComponent();
      dataChart.Chart = new HeuristicLab.Charting.Data.Datachart(0, 0, 100, 1);
      xAxisComboBox.Items.AddRange(results.VariableNames);
      yAxisComboBox.Items.Add(FREQUENCY);
      yAxisComboBox.Items.AddRange(results.VariableNames);
    }

    private void yAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    private void xAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    private void UpdateChart() {
      double minX = double.PositiveInfinity;
      double minY = double.PositiveInfinity;
      double maxX = double.NegativeInfinity;
      double maxY = double.NegativeInfinity;
      if(xAxisComboBox.SelectedItem == null || yAxisComboBox.SelectedItem == null) return;
      if(yAxisComboBox.SelectedItem.Equals(FREQUENCY)) {
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
      } else {
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
          double x = xs[i];
          double y = ys[i];
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
      }

      dataChart.Chart.ZoomIn(minX, minY, maxX, maxY);
    }
  }
}
