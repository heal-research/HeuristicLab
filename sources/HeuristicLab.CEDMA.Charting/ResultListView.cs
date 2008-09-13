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
    private const string CONSTANT_SIZE = "<constant>";
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private double maxXJitterPercent = .1;
    private double maxYJitterPercent = .1;

    public ResultListView(ResultList results) {
      this.results = results;
      results.Changed += new EventHandler(results_Changed);
      InitializeComponent();
      xAxisComboBox.Items.AddRange(results.VariableNames);
      yAxisComboBox.Items.Add(FREQUENCY);
      yAxisComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.Items.Add(CONSTANT_SIZE);
      sizeComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.SelectedItem = sizeComboBox.Items[0];
      InitChart();
    }

    private DateTime lastUpdate = DateTime.Now;
    void results_Changed(object sender, EventArgs e) {
      if(DateTime.Now.Subtract(lastUpdate).TotalSeconds < 3) return;
      lastUpdate = DateTime.Now;
      InitChart();
    }

    private void InitChart() {
      dataChart.Chart = new BubbleChart(0, 0, 100, 100);
      foreach(string dim in results.VariableNames) {
        dataChart.Chart.AddDimension(dim);
        IList<double> xs = results.GetValues(dim);
        for(int i = 0; i < xs.Count; i++) {
          double x = xs[i];
          if(double.IsInfinity(x) || x == double.MaxValue || x == double.MinValue) x = double.NaN;
          if(!double.IsNaN(x)) {
            dataChart.Chart.AddDataPoint(dim, x);
          }
        }
      }
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
        dataChart.Chart.ShowXvsY((string)xAxisComboBox.SelectedItem, (string)yAxisComboBox.SelectedItem);
      }
    }

    private void CreateHistogramChart() {
      // TASK
    }
    
    private void jitterTrackBar_ValueChanged(object sender, EventArgs e) {
      if(dataChart.Chart != null) {
        double xJitterFactor = xTrackBar.Value / 100.0 ;
        double yJitterFactor = yTrackBar.Value / 100.0 ;
        dataChart.Chart.SetJitter(xJitterFactor, yJitterFactor);
      }
      UpdateChart();
    }

    private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if(dataChart.Chart != null) {
        dataChart.Chart.SetBubbleSizeDimension((string)sizeComboBox.SelectedItem, invertCheckbox.Checked);
        UpdateChart();
      }
    }
  }
}
