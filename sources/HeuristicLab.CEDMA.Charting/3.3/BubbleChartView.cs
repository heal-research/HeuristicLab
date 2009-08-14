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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.SparseMatrix;

namespace HeuristicLab.CEDMA.Charting {
  public partial class BubbleChartView : ViewBase {
    private const string CONSTANT_SIZE = "<constant>";
    private Label pleaseSelectAxisLabel = new Label();
    public BubbleChartView(VisualMatrix results) {
      InitializeComponent();
      bubbleChartControl.Chart = new ModelingBubbleChart(results, 0, 0, 100, 100);
      xAxisComboBox.Items.AddRange(results.OrdinalVariables);
      xAxisComboBox.Items.AddRange(results.CategoricalVariables);
      xAxisComboBox.Items.AddRange(results.MultiDimensionalCategoricalVariables);
      xAxisComboBox.Items.AddRange(results.MultiDimensionalOrdinalVariables);
      yAxisComboBox.Items.AddRange(results.OrdinalVariables);
      yAxisComboBox.Items.AddRange(results.CategoricalVariables);
      yAxisComboBox.Items.AddRange(results.MultiDimensionalCategoricalVariables);
      yAxisComboBox.Items.AddRange(results.MultiDimensionalOrdinalVariables);
      sizeComboBox.Items.Add(CONSTANT_SIZE);
      sizeComboBox.Items.AddRange(results.OrdinalVariables);
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
      bubbleChartControl.Chart.SetBubbleSizeDimension((string)sizeComboBox.SelectedItem, false);
      UpdateChart();
    }
  }

  public class BubbleChartViewFactory : IResultsViewFactory {
    #region IResultsViewFactory Members

    public string Name {
      get { return "Bubble chart"; }
    }

    public IControl CreateView(VisualMatrix results) {
      return new BubbleChartView(results);
    }

    #endregion
  }
}
