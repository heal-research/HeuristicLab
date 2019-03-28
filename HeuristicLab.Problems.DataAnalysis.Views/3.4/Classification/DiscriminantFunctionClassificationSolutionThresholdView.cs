#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Classification Threshold")]
  [Content(typeof(IDiscriminantFunctionClassificationSolution), false)]
  public sealed partial class DiscriminantFunctionClassificationSolutionThresholdView : DataAnalysisSolutionEvaluationView {
    private const double TrainingAxisValue = 0.0;
    private const double TestAxisValue = 10.0;
    private const double TrainingTestBorder = (TestAxisValue - TrainingAxisValue) / 2;
    private const string TrainingLabelText = "Training Samples";
    private const string TestLabelText = "Test Samples";

    public new IDiscriminantFunctionClassificationSolution Content {
      get { return (IDiscriminantFunctionClassificationSolution)base.Content; }
      set { base.Content = value; }
    }

    private Dictionary<double, Series> classValueSeriesMapping;
    private System.Random random;
    private bool updateInProgress;

    public DiscriminantFunctionClassificationSolutionThresholdView()
      : base() {
      InitializeComponent();

      classValueSeriesMapping = new Dictionary<double, Series>();
      random = new System.Random();
      updateInProgress = false;

      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisX.Minimum = TrainingAxisValue - TrainingTestBorder;
      this.chart.ChartAreas[0].AxisX.Maximum = TestAxisValue + TrainingTestBorder;
      AddCustomLabelToAxis(this.chart.ChartAreas[0].AxisX);

      this.chart.ChartAreas[0].AxisY.Title = "Estimated Values";
      this.chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
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
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_ModelChanged(object sender, EventArgs e) {
      Content.Model.ThresholdsChanged += new EventHandler(Model_ThresholdsChanged);
      UpdateChart();
    }
    private void Model_ThresholdsChanged(object sender, EventArgs e) {
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
          IEnumerator<double> classValueEnumerator = Content.ProblemData.ClassValues.OrderBy(x => x).GetEnumerator();
          while (classNameEnumerator.MoveNext() && classValueEnumerator.MoveNext()) {
            Series series = new Series(Content.Model.TargetVariable + ": " + classNameEnumerator.Current);
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
      var targetValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToList();

      foreach (int row in Content.ProblemData.TrainingIndices) {
        double estimatedValue = estimatedValues[row];
        double targetValue = targetValues[row];
        if (targetValue.IsAlmost((double)series.Tag)) {
          double jitterValue = random.NextDouble() * 2.0 - 1.0;
          DataPoint point = new DataPoint();
          point.XValue = TrainingAxisValue + 0.01 * jitterValue * JitterTrackBar.Value * (TrainingTestBorder * 0.9);
          point.YValues[0] = estimatedValue;
          point.Tag = new KeyValuePair<double, double>(TrainingAxisValue, jitterValue);
          series.Points.Add(point);
        }
      }

      foreach (int row in Content.ProblemData.TestIndices) {
        double estimatedValue = estimatedValues[row];
        double targetValue = targetValues[row];
        if (targetValue.IsAlmost((double)series.Tag)) {
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
      IClassificationProblemData problemData = Content.ProblemData;
      var classValues = Content.Model.ClassValues.ToArray();
      Axis y = chart.ChartAreas[0].AxisY;
      Axis x = chart.ChartAreas[0].AxisX;
      string name;
      foreach (double threshold in Content.Model.Thresholds) {
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
          annotation.AxisY = y;
          annotation.Y = threshold;

          name = problemData.GetClassName(classValues[classIndex - 1]);
          TextAnnotation beneathLeft = CreateTextAnnotation(name, classIndex, x, y, x.Minimum, threshold, ContentAlignment.TopLeft);
          TextAnnotation beneathRight = CreateTextAnnotation(name, classIndex, x, y, x.Maximum, threshold, ContentAlignment.TopRight);

          name = problemData.GetClassName(classValues[classIndex]);
          TextAnnotation aboveLeft = CreateTextAnnotation(name, classIndex, x, y, x.Minimum, threshold, ContentAlignment.BottomLeft);
          TextAnnotation aboveRight = CreateTextAnnotation(name, classIndex, x, y, x.Maximum, threshold, ContentAlignment.BottomRight);

          chart.Annotations.Add(annotation);
          chart.Annotations.Add(beneathLeft);
          chart.Annotations.Add(aboveLeft);
          chart.Annotations.Add(beneathRight);
          chart.Annotations.Add(aboveRight);

          beneathLeft.ResizeToContent();
          beneathRight.ResizeToContent();
          aboveLeft.ResizeToContent();
          aboveRight.ResizeToContent();

          beneathRight.Width = -beneathRight.Width;
          aboveLeft.Height = -aboveLeft.Height;
          aboveRight.Height = -aboveRight.Height;
          aboveRight.Width = -aboveRight.Width;

          classIndex++;
        }
      }
    }

    private TextAnnotation CreateTextAnnotation(string name, int classIndex, Axis axisX, Axis axisY, double x, double y, ContentAlignment alignment) {
      TextAnnotation annotation = new TextAnnotation();
      annotation.Text = name;
      annotation.AllowMoving = true;
      annotation.AllowResizing = false;
      annotation.AllowSelecting = false;
      annotation.IsSizeAlwaysRelative = true;
      annotation.ClipToChartArea = chart.ChartAreas[0].Name;
      annotation.Tag = classIndex;
      annotation.AxisX = axisX;
      annotation.AxisY = axisY;
      annotation.Alignment = alignment;
      annotation.X = x;
      annotation.Y = y;
      return annotation;
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
      double[] thresholds = Content.Model.Thresholds.ToArray();
      thresholds[classIndex] = e.NewLocationY;
      Array.Sort(thresholds);
      Content.Model.SetThresholdsAndClassValues(thresholds, Content.Model.ClassValues);
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
