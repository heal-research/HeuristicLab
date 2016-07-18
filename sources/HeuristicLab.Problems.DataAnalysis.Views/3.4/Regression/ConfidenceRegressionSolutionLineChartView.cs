#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Line Chart (95% confidence interval)")]
  [Content(typeof(IConfidenceRegressionSolution))]
  public partial class ConfidenceRegressionSolutionLineChartView : DataAnalysisSolutionEvaluationView {
    private const string TARGETVARIABLE_SERIES_NAME = "Target Variable";
    private const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Values (training)";
    private const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Values (test)";
    private const string ESTIMATEDVALUES_ALL_SERIES_NAME = "Estimated Values (all samples)";

    public new IConfidenceRegressionSolution Content {
      get { return (IConfidenceRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public ConfidenceRegressionSolutionLineChartView()
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

    private void RedrawChart() {
      this.chart.Series.Clear();
      if (Content != null) {
        this.chart.ChartAreas[0].AxisX.Minimum = 0;
        this.chart.ChartAreas[0].AxisX.Maximum = Content.ProblemData.Dataset.Rows - 1;

        // training series
        this.chart.Series.Add(ESTIMATEDVALUES_TRAINING_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].LegendText = ESTIMATEDVALUES_TRAINING_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].ChartType = SeriesChartType.Range;
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].EmptyPointStyle.Color = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Color;
        var mean = Content.EstimatedTrainingValues.ToArray();
        var s2 = Content.EstimatedTrainingVariances.ToArray();
        var lower = mean.Zip(s2, GetLowerConfBound).ToArray();
        var upper = mean.Zip(s2, GetUpperConfBound).ToArray();
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.DataBindXY(Content.ProblemData.TrainingIndices.ToArray(), lower, upper);
        this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME]);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Tag = Content;

        // test series
        this.chart.Series.Add(ESTIMATEDVALUES_TEST_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].LegendText = ESTIMATEDVALUES_TEST_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].ChartType = SeriesChartType.Range;

        mean = Content.EstimatedTestValues.ToArray();
        s2 = Content.EstimatedTestVariances.ToArray();
        lower = mean.Zip(s2, GetLowerConfBound).ToArray();
        upper = mean.Zip(s2, GetUpperConfBound).ToArray();
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.DataBindXY(Content.ProblemData.TestIndices.ToArray(), lower, upper);
        this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME]);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Tag = Content;

        // series of remaining points
        int[] allIndices = Enumerable.Range(0, Content.ProblemData.Dataset.Rows).Except(Content.ProblemData.TrainingIndices).Except(Content.ProblemData.TestIndices).ToArray();
        mean = Content.EstimatedValues.ToArray();
        s2 = Content.EstimatedVariances.ToArray();
        lower = mean.Zip(s2, GetLowerConfBound).ToArray();
        upper = mean.Zip(s2, GetUpperConfBound).ToArray();
        List<double> allLower = allIndices.Select(index => lower[index]).ToList();
        List<double> allUpper = allIndices.Select(index => upper[index]).ToList();
        this.chart.Series.Add(ESTIMATEDVALUES_ALL_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].LegendText = ESTIMATEDVALUES_ALL_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].ChartType = SeriesChartType.Range;
        if (allIndices.Count() > 0) {
          this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Points.DataBindXY(allIndices, allLower, allUpper);
          this.InsertEmptyPoints(this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME]);
        }
        this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Tag = Content;

        // target
        this.chart.Series.Add(TARGETVARIABLE_SERIES_NAME);
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].LegendText = Content.ProblemData.TargetVariable;
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.DataBindXY(Enumerable.Range(0, Content.ProblemData.Dataset.Rows).ToArray(),
          Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToArray());

        this.ToggleSeriesData(this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME]);


        // the series have been added in different order than in the normal line chart 
        // --> adapt coloring;
        chart.ApplyPaletteColors();
        this.chart.Palette = ChartColorPalette.None;
        var s0Color = chart.Series[0].Color;
        var s1Color = chart.Series[1].Color;
        var s2Color = chart.Series[2].Color;
        var s3Color = chart.Series[3].Color;
        this.chart.PaletteCustomColors = new Color[] { s1Color, s2Color, s3Color, s0Color };

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
          var p = new DataPoint((int)((p1.XValue + p2.XValue) / 2), new double[] { 0.0, 0.0 }) { IsEmpty = true };
          // insert 
          series.Points.Insert(i + 1, p);
        }
        ++i;
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      double estimatedValuesRange = estimatedValues.Max() - estimatedValues.Min();
      double targetValuesRange = targetValues.Max() - targetValues.Min();
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

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (this.chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          ClearPointsQuick(series.Points);
        }
      } else if (Content != null) {

        IEnumerable<int> indices = null;
        IEnumerable<double> mean = null;
        IEnumerable<double> s2 = null;
        double[] lower = null;
        double[] upper = null;
        switch (series.Name) {
          case ESTIMATEDVALUES_ALL_SERIES_NAME:
            indices = Enumerable.Range(0, Content.ProblemData.Dataset.Rows).Except(Content.ProblemData.TrainingIndices).Except(Content.ProblemData.TestIndices).ToArray();
            mean = Content.EstimatedValues.ToArray();
            s2 = Content.EstimatedVariances.ToArray();
            lower = mean.Zip(s2, GetLowerConfBound).ToArray();
            upper = mean.Zip(s2, GetUpperConfBound).ToArray();
            lower = indices.Select(index => lower[index]).ToArray();
            upper = indices.Select(index => upper[index]).ToArray();
            break;
          case ESTIMATEDVALUES_TRAINING_SERIES_NAME:
            indices = Content.ProblemData.TrainingIndices.ToArray();
            mean = Content.EstimatedTrainingValues.ToArray();
            s2 = Content.EstimatedTrainingVariances.ToArray();
            lower = mean.Zip(s2, GetLowerConfBound).ToArray();
            upper = mean.Zip(s2, GetUpperConfBound).ToArray();
            break;
          case ESTIMATEDVALUES_TEST_SERIES_NAME:
            indices = Content.ProblemData.TestIndices.ToArray();
            mean = Content.EstimatedTestValues.ToArray();
            s2 = Content.EstimatedTestVariances.ToArray();
            lower = mean.Zip(s2, GetLowerConfBound).ToArray();
            upper = mean.Zip(s2, GetUpperConfBound).ToArray();
            break;
        }
        if (indices.Count() > 0) {
          series.Points.DataBindXY(indices, lower, upper);
          this.InsertEmptyPoints(series);
          chart.Legends[series.Legend].ForeColor = Color.Black;
          UpdateCursorInterval();
          chart.Refresh();
        }
      }
    }

    private double GetLowerConfBound(double m, double s) {
      return m - 1.96 * Math.Sqrt(s);
    }


    private double GetUpperConfBound(double m, double s) {
      return m + 1.96 * Math.Sqrt(s);
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
      e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[2].Cells[1].ForeColor = this.chart.Series[ESTIMATEDVALUES_ALL_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[3].Cells[1].ForeColor = this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.Count == 0 ? Color.Gray : Color.Black;
    }
  }
}
