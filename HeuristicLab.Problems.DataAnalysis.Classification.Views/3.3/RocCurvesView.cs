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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
namespace HeuristicLab.Problems.DataAnalysis.Classification.Views {
  [View("ROC Curves View")]
  [Content(typeof(SymbolicClassificationSolution))]
  public partial class RocCurvesView : AsynchronousContentView {
    private const string xAxisTitle = "False Positive Rate";
    private const string yAxisTitle = "True Positive Rate";
    private const string TrainingSamples = "Training";
    private const string TestSamples = "Test";
    private Dictionary<string, List<ROCPoint>> cachedRocPoints;

    public RocCurvesView() {
      InitializeComponent();

      cachedRocPoints = new Dictionary<string, List<ROCPoint>>();

      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.SelectedIndex = 0;

      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].AxisX.Minimum = 0.0;
      chart.ChartAreas[0].AxisX.Maximum = 1.0;
      chart.ChartAreas[0].AxisX.MajorGrid.Interval = 0.2;
      chart.ChartAreas[0].AxisY.Minimum = 0.0;
      chart.ChartAreas[0].AxisY.Maximum = 1.0;
      chart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.2;

      chart.ChartAreas[0].AxisX.Title = xAxisTitle;
      chart.ChartAreas[0].AxisY.Title = yAxisTitle;
    }

    public new SymbolicClassificationSolution Content {
      get { return (SymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EstimatedValuesChanged += new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.EstimatedValuesChanged -= new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    private void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      chart.Series.Clear();
      if (Content != null) UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else {
        chart.Series.Clear();
        chart.Annotations.Clear();
        cachedRocPoints.Clear();

        int slices = 100;
        IEnumerable<int> rows;

        if (cmbSamples.SelectedItem.ToString() == TrainingSamples) {
          rows = Content.ProblemData.TrainingIndizes;
        } else if (cmbSamples.SelectedItem.ToString() == TestSamples) {
          rows = Content.ProblemData.TestIndizes;
        } else throw new InvalidOperationException();

        double[] estimatedValues = Content.GetEstimatedValues(rows).ToArray();
        double[] targetClassValues = Content.ProblemData.Dataset.GetEnumeratedVariableValues(Content.ProblemData.TargetVariable.Value, rows).ToArray();
        double minThreshold = estimatedValues.Min();
        double maxThreshold = estimatedValues.Max();
        double thresholdIncrement = (maxThreshold - minThreshold) / slices;
        minThreshold -= thresholdIncrement;
        maxThreshold += thresholdIncrement;

        List<double> classValues = Content.ProblemData.SortedClassValues.ToList();

        foreach (double classValue in classValues) {
          List<ROCPoint> rocPoints = new List<ROCPoint>();
          int positives = targetClassValues.Where(c => c.IsAlmost(classValue)).Count();
          int negatives = targetClassValues.Length - positives;

          for (double lowerThreshold = minThreshold; lowerThreshold < maxThreshold; lowerThreshold += thresholdIncrement) {
            for (double upperThreshold = lowerThreshold + thresholdIncrement; upperThreshold < maxThreshold; upperThreshold += thresholdIncrement) {
              //only adapt lower threshold for binary classification problems and upper class prediction              
              if (classValues.Count == 2 && classValue == classValues[1]) upperThreshold = double.PositiveInfinity;

              int truePositives = 0;
              int falsePositives = 0;

              for (int row = 0; row < estimatedValues.Length; row++) {
                if (lowerThreshold < estimatedValues[row] && estimatedValues[row] < upperThreshold) {
                  if (targetClassValues[row].IsAlmost(classValue)) truePositives++;
                  else falsePositives++;
                }
              }

              double truePositiveRate = ((double)truePositives) / positives;
              double falsePositiveRate = ((double)falsePositives) / negatives;

              ROCPoint rocPoint = new ROCPoint(truePositiveRate, falsePositiveRate, lowerThreshold, upperThreshold);
              if (!rocPoints.Any(x => x.truePositiveRate >= rocPoint.truePositiveRate && x.falsePositiveRate <= rocPoint.falsePositiveRate)) {
                rocPoints.RemoveAll(x => x.falsePositiveRate >= rocPoint.falsePositiveRate && x.truePositiveRate <= rocPoint.truePositiveRate);
                rocPoints.Add(rocPoint);
              }
            }
            //only adapt upper threshold for binary classification problems and upper class prediction              
            if (classValues.Count == 2 && classValue == classValues[0]) lowerThreshold = double.PositiveInfinity;
          }

          string className = Content.ProblemData.ClassNames.ElementAt(classValues.IndexOf(classValue));
          cachedRocPoints[className] = rocPoints.OrderBy(x => x.falsePositiveRate).ToList(); ;

          Series series = new Series(className);
          series.ChartType = SeriesChartType.Line;
          series.MarkerStyle = MarkerStyle.Diamond;
          series.MarkerSize = 5;
          chart.Series.Add(series);
          FillSeriesWithDataPoints(series, cachedRocPoints[className]);

          double auc = CalculateAreaUnderCurve(series);
          series.LegendToolTip = "AUC: " + auc;
        }
      }
    }

    private void FillSeriesWithDataPoints(Series series, IEnumerable<ROCPoint> rocPoints) {
      series.Points.Add(new DataPoint(0, 0));
      foreach (ROCPoint rocPoint in rocPoints) {
        DataPoint point = new DataPoint();
        point.XValue = rocPoint.falsePositiveRate;
        point.YValues[0] = rocPoint.truePositiveRate;
        point.Tag = rocPoint;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("True Positive Rate: " + rocPoint.truePositiveRate);
        sb.AppendLine("False Positive Rate: " + rocPoint.falsePositiveRate);
        sb.AppendLine("Upper Threshold: " + rocPoint.upperThreshold);
        sb.AppendLine("Lower Threshold: " + rocPoint.lowerThreshold);
        point.ToolTip = sb.ToString();

        series.Points.Add(point);
      }
      series.Points.Add(new DataPoint(1, 1));
    }

    private double CalculateAreaUnderCurve(Series series) {
      if (series.Points.Count < 1) throw new ArgumentException("Could not calculate area under curve if less than 1 data points were given.");

      double auc = 0.0;
      for (int i = 1; i < series.Points.Count; i++) {
        double width = series.Points[i].XValue - series.Points[i - 1].XValue;
        double y1 = series.Points[i - 1].YValues[0];
        double y2 = series.Points[i].YValues[0];

        auc += (y1 + y2) * width / 2;
      }

      return auc;
    }

    private void cmbSamples_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null)
        UpdateChart();
    }


    #region show / hide series
    private void ToggleSeries(Series series) {
      if (series.Points.Count == 0)
        FillSeriesWithDataPoints(series, cachedRocPoints[series.Name]);
      else
        series.Points.Clear();
    }
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        if (result.Series != null) ToggleSeries(result.Series);
      }
    }
    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      foreach (LegendItem legendItem in e.LegendItems) {
        var series = chart.Series[legendItem.SeriesName];
        if (series != null) {
          bool seriesIsInvisible = series.Points.Count == 0;
          foreach (LegendCell cell in legendItem.Cells)
            cell.ForeColor = seriesIsInvisible ? Color.Gray : Color.Black;
        }
      }
    }
    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;

      string newTooltipText = string.Empty;
      if (result.ChartElementType == ChartElementType.DataPoint)
        newTooltipText = ((DataPoint)result.Object).ToolTip;

      string oldTooltipText = this.toolTip.GetToolTip(chart);
      if (newTooltipText != oldTooltipText)
        this.toolTip.SetToolTip(chart, newTooltipText);
    }
    #endregion


    private class ROCPoint {
      public ROCPoint(double truePositiveRate, double falsePositiveRate, double lowerThreshold, double upperThreshold) {
        this.truePositiveRate = truePositiveRate;
        this.falsePositiveRate = falsePositiveRate;
        this.lowerThreshold = lowerThreshold;
        this.upperThreshold = upperThreshold;

      }
      public double truePositiveRate { get; private set; }
      public double falsePositiveRate { get; private set; }
      public double lowerThreshold { get; private set; }
      public double upperThreshold { get; private set; }
    }

  }
}
