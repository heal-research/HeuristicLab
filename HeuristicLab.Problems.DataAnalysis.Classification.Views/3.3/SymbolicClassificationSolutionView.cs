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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Classification.Views {
  [View("Symbolic Classification View")]
  [Content(typeof(SymbolicClassificationSolution), true)]
  public sealed partial class SymbolicClassificationSolutionView : AsynchronousContentView {
    private const double TrainingAxisValue = 0.0;
    private const double TestAxisValue = 10.0;
    private const double TrainingTestBorder = (TestAxisValue - TrainingAxisValue) / 2;
    private const string TrainingLabelText = "Training Samples";
    private const string TestLabelText = "Test Samples";

    public new SymbolicClassificationSolution Content {
      get { return (SymbolicClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    private Dictionary<double, Series> classValueSeriesMapping;
    private Random random;
    private bool updateInProgress;

    public SymbolicClassificationSolutionView()
      : base() {
      InitializeComponent();

      classValueSeriesMapping = new Dictionary<double, Series>();
      random = new Random();
      updateInProgress = false;

      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisX.Minimum = TrainingAxisValue - TrainingTestBorder;
      this.chart.ChartAreas[0].AxisX.Maximum = TestAxisValue + TrainingTestBorder;
      AddCustomLabelToAxis(this.chart.ChartAreas[0].AxisX);

      this.chart.ChartAreas[0].AxisY.Title = "Estimated Values";
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
    }

    private void AddCustomLabelToAxis(Axis axis) {
      CustomLabel trainingLabel = new CustomLabel();
      trainingLabel.Text = TrainingLabelText;
      trainingLabel.FromPosition = TrainingAxisValue - TrainingTestBorder;
      trainingLabel.ToPosition = TrainingAxisValue + TrainingTestBorder;
      axis.CustomLabels.Add(trainingLabel);

      CustomLabel testLabel = new CustomLabel();
      testLabel.Text = TestLabelText;
      testLabel.FromPosition = TestAxisValue - TrainingTestBorder;
      testLabel.ToPosition = TestAxisValue + TrainingTestBorder;
      axis.CustomLabels.Add(testLabel);
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.EstimatedValuesChanged += new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
      Content.ThresholdsChanged += new EventHandler(Content_ThresholdsChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.EstimatedValuesChanged -= new EventHandler(Content_EstimatedValuesChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
      Content.ThresholdsChanged -= new EventHandler(Content_ThresholdsChanged);
    }

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_ThresholdsChanged(object sender, EventArgs e) {
      AddThresholds();
    }
    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else if (!updateInProgress) {
        updateInProgress = true;
        chart.Series.Clear();
        classValueSeriesMapping.Clear();
        if (Content != null) {
          IEnumerator<string> classNameEnumerator = Content.ProblemData.ClassNames.GetEnumerator();
          IEnumerator<double> classValueEnumerator = Content.ProblemData.SortedClassValues.GetEnumerator();
          while (classNameEnumerator.MoveNext() && classValueEnumerator.MoveNext()) {
            Series series = new Series(classNameEnumerator.Current);
            series.ChartType = SeriesChartType.FastPoint;
            series.Tag = classValueEnumerator.Current;
            chart.Series.Add(series);
            classValueSeriesMapping.Add(classValueEnumerator.Current, series);
            FillSeriesWithDataPoints(series);
          }
          AddThresholds();
        }
        chart.ChartAreas[0].RecalculateAxesScale();
        updateInProgress = false;
      }
    }

    private void FillSeriesWithDataPoints(Series series) {
      List<double> estimatedValues = Content.EstimatedValues.ToList();
      foreach (int row in Content.ProblemData.TrainingIndizes) {
        double estimatedValue = estimatedValues[row];
        double targetValue = Content.ProblemData.Dataset[Content.ProblemData.TargetVariable.Value, row];
        if (targetValue.IsAlmost((double)series.Tag)) {
          double jitterValue = random.NextDouble() * 2.0 - 1.0;
          DataPoint point = new DataPoint();
          point.XValue = TrainingAxisValue + 0.01 * jitterValue * JitterTrackBar.Value * (TrainingTestBorder * 0.9);
          point.YValues[0] = estimatedValue;
          point.Tag = new KeyValuePair<double, double>(TrainingAxisValue, jitterValue);
          series.Points.Add(point);
        }
      }

      foreach (int row in Content.ProblemData.TestIndizes) {
        double estimatedValue = estimatedValues[row];
        double targetValue = Content.ProblemData.Dataset[Content.ProblemData.TargetVariable.Value, row];
        if (targetValue == (double)series.Tag) {
          double jitterValue = random.NextDouble() * 2.0 - 1.0;
          DataPoint point = new DataPoint();
          point.XValue = TestAxisValue + 0.01 * jitterValue * JitterTrackBar.Value * (TrainingTestBorder * 0.9);
          point.YValues[0] = estimatedValue;
          point.Tag = new KeyValuePair<double, double>(TestAxisValue, jitterValue);
          series.Points.Add(point);
        }
      }

      UpdateCursorInterval();
    }

    private void AddThresholds() {
      chart.Annotations.Clear();
      int classIndex = 1;
      foreach (double threshold in Content.Thresholds) {
        if (!double.IsInfinity(threshold)) {
          HorizontalLineAnnotation annotation = new HorizontalLineAnnotation();
          annotation.AllowMoving = true;
          annotation.AllowResizing = false;
          annotation.LineWidth = 2;
          annotation.LineColor = Color.Red;

          annotation.IsInfinitive = true;
          annotation.ClipToChartArea = chart.ChartAreas[0].Name;
          annotation.Tag = classIndex;  //save classIndex as Tag to avoid moving the threshold accross class bounderies

          annotation.AxisX = chart.ChartAreas[0].AxisX;
          annotation.AxisY = chart.ChartAreas[0].AxisY;
          annotation.Y = threshold;

          chart.Annotations.Add(annotation);
          classIndex++;
        }
      }
    }

    private void JitterTrackBar_ValueChanged(object sender, EventArgs e) {
      foreach (Series series in chart.Series) {
        foreach (DataPoint point in series.Points) {
          double value = ((KeyValuePair<double, double>)point.Tag).Key;
          double jitterValue = ((KeyValuePair<double, double>)point.Tag).Value; ;
          point.XValue = value + 0.01 * jitterValue * JitterTrackBar.Value * (TrainingTestBorder * 0.9);
        }
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
    }

    private void ToggleSeries(Series series) {
      if (series.Points.Count == 0)
        FillSeriesWithDataPoints(series);
      else
        series.Points.Clear();
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        if (result.Series != null) ToggleSeries(result.Series);
      }
    }

    private void chart_AnnotationPositionChanging(object sender, AnnotationPositionChangingEventArgs e) {
      int classIndex = (int)e.Annotation.Tag;

      double classValue = Content.ProblemData.SortedClassValues.ElementAt(classIndex);
      if (e.NewLocationY >= classValue)
        e.NewLocationY = classValue;

      classValue = Content.ProblemData.SortedClassValues.ElementAt(classIndex - 1);
      if (e.NewLocationY <= classValue)
        e.NewLocationY = classValue;

      double[] thresholds = Content.Thresholds.ToArray();
      thresholds[classIndex] = e.NewLocationY;
      Content.Thresholds = thresholds;
    }

    private void UpdateCursorInterval() {
      Series series = chart.Series[0];
      double[] xValues = (from point in series.Points
                          where !point.IsEmpty
                          select point.XValue)
                    .DefaultIfEmpty(1.0)
                    .ToArray();
      double[] yValues = (from point in series.Points
                          where !point.IsEmpty
                          select point.YValues[0])
                    .DefaultIfEmpty(1.0)
                    .ToArray();

      double xRange = xValues.Max() - xValues.Min();
      double yRange = yValues.Max() - yValues.Min();
      if (xRange.IsAlmost(0.0)) xRange = 1.0;
      if (yRange.IsAlmost(0.0)) yRange = 1.0;
      double xDigits = (int)Math.Log10(xRange) - 3;
      double yDigits = (int)Math.Log10(yRange) - 3;
      double xZoomInterval = Math.Pow(10, xDigits);
      double yZoomInterval = Math.Pow(10, yDigits);
      this.chart.ChartAreas[0].CursorX.Interval = xZoomInterval;
      this.chart.ChartAreas[0].CursorY.Interval = yZoomInterval;
    }
  }
}
