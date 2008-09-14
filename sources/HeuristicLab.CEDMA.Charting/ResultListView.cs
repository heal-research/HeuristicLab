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

    public ResultListView(ResultList results) {
      this.results = results;
      InitializeComponent();
      xAxisComboBox.Items.AddRange(results.VariableNames);
      yAxisComboBox.Items.Add(FREQUENCY);
      yAxisComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.Items.Add(CONSTANT_SIZE);
      sizeComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.SelectedItem = sizeComboBox.Items[0];
      InitChart();
    }

    private void InitChart() {
      dataChart.Chart = new BubbleChart(results, 0, 0, 100, 100);
    }

    private void yAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    private void xAxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
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
        double xJitterFactor = xTrackBar.Value / 100.0;
        double yJitterFactor = yTrackBar.Value / 100.0;
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
