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
    private BubbleChartControl bubbleChartControl;
    private HistogramControl histogramControl;
    private Label pleaseSelectAxisLabel = new Label();

    public ResultListView(ResultList results) {
      this.results = results;
      InitializeComponent();
      InitCharts();
      xAxisComboBox.Items.AddRange(results.VariableNames);
      yAxisComboBox.Items.Add(FREQUENCY);
      yAxisComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.Items.Add(CONSTANT_SIZE);
      sizeComboBox.Items.AddRange(results.VariableNames);
      sizeComboBox.SelectedItem = sizeComboBox.Items[0];
      sizeComboBox.Enabled = false;
      invertCheckbox.Enabled = false;
      sizeLabel.Enabled = false;
      yAxisComboBox.SelectedItem = yAxisComboBox.Items[0];
      xAxisComboBox.SelectedItem = xAxisComboBox.Items[0];
    }

    private void InitCharts() {
      bubbleChartControl = new BubbleChartControl();
      bubbleChartControl.Chart = new BubbleChart(results, 0, 0, 100, 100);
      histogramControl = new HistogramControl();
      histogramControl.Chart = new Histogram(results, 0, 0, 100, 100);
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
        xJitterlabel.Enabled = false;
        xTrackBar.Enabled = false;
        yJitterLabel.Enabled = false;
        yTrackBar.Enabled = false;
        sizeComboBox.Enabled = false;
        invertCheckbox.Enabled = false;
        sizeLabel.Enabled = false;
        chartPanel.Controls.Clear();
        chartPanel.Controls.Add(histogramControl);
        histogramControl.Chart.ShowFrequency((string)xAxisComboBox.SelectedItem);
        histogramControl.Dock = DockStyle.Fill;
      } else {
        xJitterlabel.Enabled = true;
        xTrackBar.Enabled = true;
        yJitterLabel.Enabled = true;
        yTrackBar.Enabled = true;
        sizeComboBox.Enabled = true;
        invertCheckbox.Enabled = true;
        sizeLabel.Enabled = true;
        chartPanel.Controls.Clear();
        chartPanel.Controls.Add(bubbleChartControl);
        bubbleChartControl.Chart.ShowXvsY((string)xAxisComboBox.SelectedItem, (string)yAxisComboBox.SelectedItem);
        bubbleChartControl.Dock = DockStyle.Fill;
      }
    }

    private void jitterTrackBar_ValueChanged(object sender, EventArgs e) {
      if(bubbleChartControl.Chart != null) {
        double xJitterFactor = xTrackBar.Value / 100.0;
        double yJitterFactor = yTrackBar.Value / 100.0;
        bubbleChartControl.Chart.SetJitter(xJitterFactor, yJitterFactor);
      }
      UpdateChart();
    }

    private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if(bubbleChartControl.Chart != null) {
        bubbleChartControl.Chart.SetBubbleSizeDimension((string)sizeComboBox.SelectedItem, invertCheckbox.Checked);
        UpdateChart();
      }
    }
  }
}
