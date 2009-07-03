using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.Charting;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CEDMA.Charting {
  public class BubbleChartViewFactory : IResultsViewFactory {
    #region IResultsViewFactory Members

    public string Name {
      get { return "Bubble chart"; }
    }

    public IControl CreateView(Results results) {
      return new BubbleChartView(results);
    }

    #endregion
  }

  public partial class BubbleChartView : ViewBase {
    private Results Results {
      get { return (Results)Item; }
      set { Item = value; }
    }
    private const string CONSTANT_SIZE = "<constant>";
    private Label pleaseSelectAxisLabel = new Label();
    public BubbleChartView(Results results) {
      InitializeComponent();
      Results = results;
      bubbleChartControl.Chart = new BubbleChart(Results, 0, 0, 100, 100);
      xAxisComboBox.Items.AddRange(Results.OrdinalVariables);
      xAxisComboBox.Items.AddRange(Results.CategoricalVariables);
      xAxisComboBox.Items.AddRange(Results.MultiDimensionalCategoricalVariables);
      xAxisComboBox.Items.AddRange(Results.MultiDimensionalOrdinalVariables);
      yAxisComboBox.Items.AddRange(Results.OrdinalVariables);
      yAxisComboBox.Items.AddRange(Results.CategoricalVariables);
      yAxisComboBox.Items.AddRange(Results.MultiDimensionalCategoricalVariables);
      yAxisComboBox.Items.AddRange(Results.MultiDimensionalOrdinalVariables);
      sizeComboBox.Items.Add(CONSTANT_SIZE);
      sizeComboBox.Items.AddRange(Results.OrdinalVariables);
      sizeComboBox.SelectedItem = sizeComboBox.Items[0];
      yAxisComboBox.SelectedItem = yAxisComboBox.Items[0];
      xAxisComboBox.SelectedItem = xAxisComboBox.Items[0];
    }

    private void axisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    private void UpdateChart() {
      if (xAxisComboBox.SelectedItem == null || yAxisComboBox.SelectedItem == null) return;
      bubbleChartControl.Chart.ShowXvsY((string)xAxisComboBox.SelectedItem, (string)yAxisComboBox.SelectedItem);
    }

    private void jitterTrackBar_ValueChanged(object sender, EventArgs e) {
      double xJitterFactor = xTrackBar.Value / 100.0;
      double yJitterFactor = yTrackBar.Value / 100.0;
      bubbleChartControl.Chart.SetJitter(xJitterFactor, yJitterFactor);
      UpdateChart();
    }

    private void sizeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      bubbleChartControl.Chart.SetBubbleSizeDimension((string)sizeComboBox.SelectedItem, invertCheckbox.Checked);
      UpdateChart();
    }
  }
}
