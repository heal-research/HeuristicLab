﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.MainForm;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Line Chart")]
  [Content(typeof(IRegressionSolution))]
  public abstract partial class RegressionSolutionLineChartViewBase : DataAnalysisSolutionEvaluationView {
    protected const string TARGETVARIABLE_SERIES_NAME = "Target Variable";
    protected const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Values (training)";
    protected const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Values (test)";
    protected const string ESTIMATEDVALUES_ALL_SERIES_NAME = "Estimated Values (all samples)";

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    protected RegressionSolutionLineChartViewBase()
      : base() {
      InitializeComponent();
      //configure axis
      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
      this.chart.ChartAreas[0].CursorX.Interval = 1;

      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorY.Interval = 0;
    }

    protected abstract void GetTrainingSeries(out int[] idx, out double[] y);

    protected abstract void GetTestSeries(out int[] x, out double[] y);

    protected abstract void GetAllValuesSeries(out int[] x, out double[] y);

    protected virtual void RedrawChart() {
      this.chart.Series.Clear();
      if (Content != null) {
        this.chart.ChartAreas[0].AxisX.Minimum = 0;
        this.chart.ChartAreas[0].AxisX.Maximum = Content.ProblemData.Dataset.Rows - 1;

        this.chart.Series.Add(TARGETVARIABLE_SERIES_NAME);
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].LegendText = TARGETVARIABLE_SERIES_NAME;
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;

        var rows = Enumerable.Range(0, Content.ProblemData.Dataset.Rows).ToArray();
        var targetValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable);


        this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.DataBindXY(rows.ToArray(), targetValues.Select(v => double.IsInfinity(v) ? double.NaN : v).ToArray());
        // training series
        this.chart.Series.Add(ESTIMATEDVALUES_TRAINING_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].LegendText = ESTIMATEDVALUES_TRAINING_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        int[] trainingIdx;
        double[] trainingY;
        GetTrainingSeries(out trainingIdx, out trainingY);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.DataBindXY(trainingIdx, trainingY);
        this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME]);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Tag = Content;

        // test series
        this.chart.Series.Add(ESTIMATEDVALUES_TEST_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].LegendText = ESTIMATEDVALUES_TEST_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        int[] testIdx;
        double[] testY;
        GetTestSeries(out testIdx, out testY);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.DataBindXY(testIdx, testY);
        this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME]);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Tag = Content;

        // series of remaining points
        int[] allIdx;
        double[] allEstimatedValues;
        GetAllValuesSeries(out allIdx, out allEstimatedValues);

        this.chart.Series.Add(ESTIMATEDVALUES_ALL_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].LegendText = ESTIMATEDVALUES_ALL_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        if (allEstimatedValues.Length > 0) {
          this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Points.DataBindXY(allIdx, allEstimatedValues);
          this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME]);
        }
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Tag = Content;
        this.ToggleSeriesData(this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME]);

        // set the y-axis
        var axisY = this.chart.ChartAreas[0].AxisY;
        axisY.Title = Content.ProblemData.TargetVariable;
        double min = double.MaxValue, max = double.MinValue;
        foreach (var point in chart.Series.SelectMany(x => x.Points)) {
          if (point.IsEmpty || !point.YValues.Any() || double.IsInfinity(point.YValues[0]) || double.IsNaN(point.YValues[0]))
            continue;
          var y = point.YValues[0];
          if (y < min)
            min = y;
          if (y > max)
            max = y;
        }

        double axisMin, axisMax, axisInterval;
        ChartUtil.CalculateOptimalAxisInterval(min, max, out axisMin, out axisMax, out axisInterval);
        axisY.Minimum = axisMin;
        axisY.Maximum = axisMax;
        axisY.Interval = axisInterval;

        UpdateCursorInterval();
        this.UpdateStripLines();
      }
    }

    private void InsertEmptyPoints(Series series) {
      int i = 0;
      while (i < series.Points.Count - 1) {
        if (series.Points[i].IsEmpty) {
          ++i;
          continue;
        }

        var p1 = series.Points[i];
        var p2 = series.Points[i + 1];
        // check for consecutive indices
        if ((int)p2.XValue - (int)p1.XValue != 1) {
          // insert an empty point between p1 and p2 so that the line will be invisible (transparent)
          var p = new DataPoint((int)((p1.XValue + p2.XValue) / 2), 0.0) { IsEmpty = true };
          series.Points.Insert(i + 1, p);
        }
        ++i;
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      double estimatedValuesRange = estimatedValues.Max() - estimatedValues.Min();
      double targetValuesRange = targetValues.Where(v => !double.IsInfinity(v) && !double.IsNaN(v)).Max() - 
                                 targetValues.Where(v => !double.IsInfinity(v) && !double.IsNaN(v)).Min();
      double interestingValuesRange = Math.Min(Math.Max(targetValuesRange, 1.0), Math.Max(estimatedValuesRange, 1.0));
      double digits = (int)Math.Log10(interestingValuesRange) - 3;
      double yZoomInterval = Math.Max(Math.Pow(10, digits), 10E-5);
      this.chart.ChartAreas[0].CursorY.Interval = yZoomInterval;
    }

    #region events
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

    protected override void OnContentChanged() {
      base.OnContentChanged();
      RedrawChart();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      RedrawChart();
    }
    private void Content_ModelChanged(object sender, EventArgs e) {
      RedrawChart();
    }



    private void Chart_MouseDoubleClick(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartArea != null && (result.ChartElementType == ChartElementType.PlottingArea ||
                                       result.ChartElementType == ChartElementType.Gridlines) ||
                                       result.ChartElementType == ChartElementType.StripLines) {
        foreach (var axis in result.ChartArea.Axes)
          axis.ScaleView.ZoomReset(int.MaxValue);
      }
    }
    #endregion

    private void UpdateStripLines() {
      this.chart.ChartAreas[0].AxisX.StripLines.Clear();

      int[] attr = new int[Content.ProblemData.Dataset.Rows + 1]; // add a virtual last row that is again empty to simplify loop further down
      foreach (var row in Content.ProblemData.TrainingIndices) {
        attr[row] += 1;
      }
      foreach (var row in Content.ProblemData.TestIndices) {
        attr[row] += 2;
      }
      int start = 0;
      int curAttr = attr[start];
      for (int row = 0; row < attr.Length; row++) {
        if (attr[row] != curAttr) {
          switch (curAttr) {
            case 0: break;
            case 1:
              this.CreateAndAddStripLine("Training", start, row, Color.FromArgb(40, Color.Green), Color.Transparent);
              break;
            case 2:
              this.CreateAndAddStripLine("Test", start, row, Color.FromArgb(40, Color.Red), Color.Transparent);
              break;
            case 3:
              this.CreateAndAddStripLine("Training and Test", start, row, Color.FromArgb(40, Color.Green), Color.FromArgb(40, Color.Red), ChartHatchStyle.WideUpwardDiagonal);
              break;
            default:
              // should not happen
              break;
          }
          curAttr = attr[row];
          start = row;
        }
      }
    }

    private void CreateAndAddStripLine(string title, int start, int end, Color color, Color secondColor, ChartHatchStyle hatchStyle = ChartHatchStyle.None) {
      StripLine stripLine = new StripLine();
      stripLine.BackColor = color;
      stripLine.BackSecondaryColor = secondColor;
      stripLine.BackHatchStyle = hatchStyle;
      stripLine.Text = title;
      stripLine.Font = new Font("Times New Roman", 12, FontStyle.Bold);
      // strip range is [start .. end] inclusive, but we evaluate [start..end[ (end is exclusive)
      // the strip should be by one longer (starting at start - 0.5 and ending at end + 0.5)
      stripLine.StripWidth = end - start;
      stripLine.IntervalOffset = start - 0.5; // start slightly to the left of the first point to clearly indicate the first point in the partition
      this.chart.ChartAreas[0].AxisX.StripLines.Add(stripLine);
    }

    public void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (this.chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          ClearPointsQuick(series.Points);
        }
      } else if (Content != null) {

        int[] indices = null;
        double[] predictedValues = null;
        switch (series.Name) {
          case ESTIMATEDVALUES_ALL_SERIES_NAME:
            GetAllValuesSeries(out indices, out predictedValues);
            break;
          case ESTIMATEDVALUES_TRAINING_SERIES_NAME:
            GetTrainingSeries(out indices, out predictedValues);
            break;
          case ESTIMATEDVALUES_TEST_SERIES_NAME:
            GetTestSeries(out indices, out predictedValues);
            break;
        }
        if (predictedValues.Length > 0) {
          series.Points.DataBindXY(indices, predictedValues);
          this.InsertEmptyPoints(series);
        }
        chart.Legends[series.Legend].ForeColor = Color.Black;
        UpdateCursorInterval();
        chart.Refresh();
      }
    }

    // workaround as per http://stackoverflow.com/questions/5744930/datapointcollection-clear-performance
    private static void ClearPointsQuick(DataPointCollection points) {
      points.SuspendUpdates();
      while (points.Count > 0)
        points.RemoveAt(points.Count - 1);
      points.ResumeUpdates();
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem && result.Series.Name != TARGETVARIABLE_SERIES_NAME)
        Cursor = Cursors.Hand;
      else
        Cursor = Cursors.Default;
    }
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem && result.Series.Name != TARGETVARIABLE_SERIES_NAME) {
        ToggleSeriesData(result.Series);
      }
    }

    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      if (chart.Series.Count != 4) return;
      e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[2].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[3].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
    }
  }
}
