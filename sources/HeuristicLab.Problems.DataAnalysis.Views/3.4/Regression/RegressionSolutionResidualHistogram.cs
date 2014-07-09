#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Residual Histogram")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionResidualHistogram : DataAnalysisSolutionEvaluationView {

    #region variables
    protected const string ALL_SAMPLES = "All samples";
    protected const string TRAINING_SAMPLES = "Training samples";
    protected const string TEST_SAMPLES = "Test samples";
    /// <summary>
    /// approximate amount of bins 
    /// </summary>
    protected const double bins = 25;
    #endregion

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public RegressionSolutionResidualHistogram()
      : base() {
      InitializeComponent();
      foreach (string series in new List<String>() { ALL_SAMPLES, TRAINING_SAMPLES, TEST_SAMPLES }) {
        chart.Series.Add(series);
        chart.Series[series].LegendText = series;
        chart.Series[series].ChartType = SeriesChartType.Column;
        chart.Series[series]["PointWidth"] = "0.9";
        chart.Series[series].BorderWidth = 1;
        chart.Series[series].BorderDashStyle = ChartDashStyle.Solid;
        chart.Series[series].BorderColor = Color.Black;
        chart.Series[series].ToolTip = series + " Y = #VALY from #CUSTOMPROPERTY(from) to #CUSTOMPROPERTY(to)";
      }
      //configure axis 
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].AxisX.Title = "Residuals";
      chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      chart.ChartAreas[0].CursorX.Interval = 1;
      chart.ChartAreas[0].CursorY.Interval = 1;
      chart.ChartAreas[0].AxisY.Title = "Relative Frequency";
      chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
    }

    private void RedrawChart() {
      foreach (Series series in chart.Series) {
        series.Points.Clear();
      }
      if (Content != null) {
        List<double> residuals = CalculateResiduals(Content);

        double max = 0.0;
        foreach (Series series in chart.Series) {
          CalculateFrequencies(residuals, series);
          double seriesMax = series.Points.Select(p => p.YValues.First()).Max();
          max = max < seriesMax ? seriesMax : max;
        }

        // ALL_SAMPLES has to be calculated to know its highest frequency, but it is not shown in the beginning
        chart.Series.First(s => s.Name.Equals(ALL_SAMPLES)).Points.Clear();

        double roundedMax, intervalWidth;
        CalculateResidualParameters(residuals, out roundedMax, out intervalWidth);

        ChartArea chartArea = chart.ChartAreas[0];
        chartArea.AxisX.Minimum = -roundedMax - intervalWidth;
        chartArea.AxisX.Maximum = roundedMax + intervalWidth;
        // get the highest frequency of a residual of any series
        chartArea.AxisY.Maximum = max;
        if (chartArea.AxisY.Maximum < 0.1) {
          chartArea.AxisY.Interval = 0.01;
          chartArea.AxisY.Maximum = Math.Ceiling(chartArea.AxisY.Maximum * 100) / 100;
        } else {
          chartArea.AxisY.Interval = 0.1;
          chartArea.AxisY.Maximum = Math.Ceiling(chartArea.AxisY.Maximum * 10) / 10;
        }
        chartArea.AxisX.Interval = intervalWidth;
        int curBins = (int)Math.Round((roundedMax * 2) / intervalWidth);
        //shifts the x axis label so that zero is in the middle
        if (curBins % 2 == 0)
          chartArea.AxisX.IntervalOffset = intervalWidth;
        else
          chartArea.AxisX.IntervalOffset = intervalWidth / 2;
      }
    }

    private List<double> CalculateResiduals(IRegressionSolution solution) {
      List<double> residuals = new List<double>();

      IRegressionProblemData problemdata = solution.ProblemData;
      List<double> targetValues = problemdata.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable).ToList();
      List<double> estimatedValues = solution.EstimatedValues.ToList();

      for (int i = 0; i < solution.ProblemData.Dataset.Rows; i++) {
        double residual = estimatedValues[i] - targetValues[i];
        residuals.Add(residual);
      }
      return residuals;
    }

    private void CalculateFrequencies(List<double> residualValues, Series series) {
      double roundedMax, intervalWidth;
      CalculateResidualParameters(residualValues, out roundedMax, out intervalWidth);

      IEnumerable<double> relevantResiduals = residualValues;
      IRegressionProblemData problemdata = Content.ProblemData;
      if (series.Name.Equals(TRAINING_SAMPLES)) {
        relevantResiduals = residualValues.Skip(problemdata.TrainingPartition.Start).Take(problemdata.TrainingPartition.Size);
      } else if (series.Name.Equals(TEST_SAMPLES)) {
        relevantResiduals = residualValues.Skip(problemdata.TestPartition.Start).Take(problemdata.TestPartition.Size);
      }

      double intervalCenter = intervalWidth / 2.0;
      double sampleCount = relevantResiduals.Count();
      double current = -roundedMax;
      DataPointCollection seriesPoints = series.Points;

      for (int i = 0; i <= bins; i++) {
        IEnumerable<double> help = relevantResiduals.Where(x => x >= (current - intervalCenter) && x < (current + intervalCenter));
        seriesPoints.AddXY(current, help.Count() / sampleCount);
        seriesPoints[seriesPoints.Count - 1]["from"] = (current - intervalCenter).ToString();
        seriesPoints[seriesPoints.Count - 1]["to"] = (current + intervalCenter).ToString();
        current += intervalWidth;
      }
    }

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          series.Points.Clear();
        }
      } else if (Content != null) {
        List<double> residuals = CalculateResiduals(Content);
        CalculateFrequencies(residuals, series);
        chart.Legends[series.Legend].ForeColor = Color.Black;
        chart.Refresh();
      }
    }

    private static void CalculateResidualParameters(List<double> residuals, out double roundedMax, out double intervalWidth) {
      double realMax = Math.Max(Math.Abs(residuals.Min()), Math.Abs(residuals.Max()));
      roundedMax = HumanRoundMax(realMax);
      intervalWidth = (roundedMax * 2.0) / bins;
      intervalWidth = HumanRoundMax(intervalWidth);
      // sets roundedMax to a value, so that zero will be in the middle of the x axis
      double help = realMax / intervalWidth;
      help = help % 1 < 0.5 ? (int)help : (int)help + 1;
      roundedMax = help * intervalWidth;
    }

    private static double HumanRoundMax(double max) {
      double base10;
      if (max > 0) base10 = Math.Pow(10.0, Math.Floor(Math.Log10(max)));
      else base10 = Math.Pow(10.0, Math.Ceiling(Math.Log10(-max)));
      double rounding = (max > 0) ? base10 : -base10;
      while (rounding < max) rounding += base10;
      return rounding;
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
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        ToggleSeriesData(result.Series);
      }
    }
    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        Cursor = Cursors.Hand;
      else
        Cursor = Cursors.Default;
    }
    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      if (chart.Series.Count != 3) return;
      e.LegendItems[0].Cells[1].ForeColor = chart.Series[ALL_SAMPLES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[1].Cells[1].ForeColor = chart.Series[TRAINING_SAMPLES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[2].Cells[1].ForeColor = chart.Series[TEST_SAMPLES].Points.Count == 0 ? Color.Gray : Color.Black;
    }
    #endregion
  }
}
