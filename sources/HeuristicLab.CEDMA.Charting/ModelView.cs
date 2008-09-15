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
using HeuristicLab.Functions;
using HeuristicLab.Logging;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Charting.Data;

namespace HeuristicLab.CEDMA.Charting {
  public partial class ModelView : ViewBase {
    private Dataset dataset;
    private IFunctionTree model;
    private int targetVariable;
    private Record record;

    public ModelView(Record record, Dataset dataset, IFunctionTree model, int targetVariable) {
      InitializeComponent();
      this.dataset = dataset;
      this.model = model;
      this.targetVariable = targetVariable;
      this.record = record;

      splitContainer.Panel1.Controls.Add((Control)model.CreateView());
      splitContainer.Panel1.Controls[0].Dock = DockStyle.Fill;
      lowerSplitContainer.Panel1.Controls.Add(CreateLineChart());
      lowerSplitContainer.Panel1.Controls[0].Dock = DockStyle.Fill;
      lowerSplitContainer.Panel2.Controls.Add(CreateScatterPlot());
      lowerSplitContainer.Panel2.Controls[0].Dock = DockStyle.Fill;
    }

    private Control CreateScatterPlot() {
      double minX = double.PositiveInfinity;
      double maxX = double.NegativeInfinity;

      Datachart chart = new Datachart(0, 0, 1, 1);
      chart.Title = "Scatter plot";
      chart.UpdateEnabled = false;
      Pen pen = new Pen(Color.FromArgb(80, Color.Blue));
      chart.AddDataRow(DataRowType.Points, pen, pen.Brush);
      IEvaluator eval = model.CreateEvaluator();
      eval.ResetEvaluator(model, dataset);
      for(int i = 0; i < dataset.Rows; i += 10) {
        double predicted = eval.Evaluate(i);
        double original = dataset.GetValue(i, targetVariable);
        if(double.IsInfinity(predicted) || predicted == double.MaxValue || predicted == double.MinValue) predicted = double.NaN;
        if(double.IsInfinity(original) || original == double.MaxValue || original == double.MinValue) original = double.NaN;
        if(!double.IsNaN(predicted) && !double.IsNaN(original)) {
          chart.AddDataPoint(0, original, predicted);
          if(original < minX) minX = original;
          if(original > maxX) maxX = original;
          if(predicted < minX) minX = predicted;
          if(predicted > maxX) maxX = predicted;
        }
      }
      chart.UpdateEnabled = true;
      chart.Group.Add(new Axis(chart, (maxX - minX) / 2 + minX, (maxX - minX) / 2+minX, AxisType.Both));
      DataChartControl control = new DataChartControl();
      control.Chart = chart;
      if(minX < maxX) {
        double xExcess = (maxX - minX) * 0.1;
        chart.ZoomIn(minX - xExcess, minX-xExcess, maxX + xExcess, maxX + xExcess);
        control.ScaleOnResize = false;
      }
      return control;
    }

    private Control CreateLineChart() {
      double minY = double.PositiveInfinity;
      double maxY = double.NegativeInfinity;

      Datachart chart = new Datachart(0, 0, 1, 1);
      chart.Title = "Linechart";
      chart.UpdateEnabled = false;
      Pen redPen = new Pen(Color.FromArgb(180, Color.Red));
      Pen bluePen = new Pen(Color.FromArgb(180, Color.Blue));
      chart.AddDataRow(DataRowType.Lines, bluePen, bluePen.Brush);
      chart.AddDataRow(DataRowType.Lines, redPen, redPen.Brush);
      IEvaluator eval = model.CreateEvaluator();
      eval.ResetEvaluator(model, dataset);
      int n = 0;
      for(int i = 0; i < dataset.Rows; i += 10) {
        double predicted = eval.Evaluate(i);
        double original = dataset.GetValue(i, targetVariable);
        if(double.IsInfinity(predicted) || predicted == double.MaxValue || predicted == double.MinValue) predicted = double.NaN;
        if(double.IsInfinity(original) || original == double.MaxValue || original == double.MinValue) original = double.NaN;
        if(!double.IsNaN(predicted) && !double.IsNaN(original)) {
          chart.AddDataPoint(0, i, predicted);
          chart.AddDataPoint(1, i, original);
          n++;
          if(original < minY) minY = original;
          if(original > maxY) maxY = original;
          if(predicted < minY) minY = predicted;
          if(predicted > maxY) maxY = predicted;
        }
      }
      chart.UpdateEnabled = true;
      chart.Group.Add(new Axis(chart, 0, minY, AxisType.Both));
      DataChartControl control = new DataChartControl();
      control.Chart = chart;
      if(minY < maxY) {
        double xExcess = dataset.Rows * 0.05;
        double yExcess = (maxY - minY) * 0.1;
        chart.ZoomIn(0.0 - xExcess, minY - yExcess, (double)dataset.Rows + xExcess, maxY + yExcess);
        control.ScaleOnResize = false;
      }
      return control;
    }

    private void algoButton_Click(object sender, EventArgs e) {
      record.OpenGeneratingAlgorithm();
    }
  }
}
