#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using System.Collections.Specialized;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Scatter Plot View")]
  [Content(typeof(DataAnalysisSolution), true)]
  public partial class ScatterPlotView : AsynchronousContentView {
    private const string DEFAULT_CAPTION = "Scatter Plot";
    private const string ALL_SERIES = "All Samples";
    private const string TRAINING_SERIES = "Training Samples";
    private const string TEST_SERIES = "Test Samples";

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotView()
      : base() {
      InitializeComponent();
      this.Caption = DEFAULT_CAPTION;

      this.chart.Series.Add(ALL_SERIES);
      this.chart.Series[ALL_SERIES].LegendText = ALL_SERIES;
      this.chart.Series[ALL_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.Series.Add(TRAINING_SERIES);
      this.chart.Series[TRAINING_SERIES].LegendText = TRAINING_SERIES;
      this.chart.Series[TRAINING_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.Series.Add(TEST_SERIES);
      this.chart.Series[TEST_SERIES].LegendText = TEST_SERIES;
      this.chart.Series[TEST_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
      this.chart.AxisViewChanged += new EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(chart_AxisViewChanged);

      //configure axis                  
      this.chart.ChartAreas[0].AxisX.Title = "Estimated Values";
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 1;
      this.chart.ChartAreas[0].CursorY.Interval = 1;

      this.chart.ChartAreas[0].AxisY.Title = "Target Values";
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
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


    void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    void Content_EstimatedValuesChanged(object sender, EventArgs e) {
      UpdateSeries();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else {
        if (Content != null) {
          this.Caption = Content.ItemName + " " + DEFAULT_CAPTION;
          this.UpdateSeries();
          if (!this.chart.Series.Any(s => s.Points.Count > 0))
            this.ToggleSeriesData(this.chart.Series[TRAINING_SERIES]);
        } else {
          this.Caption = DEFAULT_CAPTION;
          this.ClearChart();
        }
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[ALL_SERIES].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[ALL_SERIES].Points.Select(x => x.YValues[1]).DefaultIfEmpty(1.0);
      double estimatedValuesRange = estimatedValues.Max() - estimatedValues.Min();
      double targetValuesRange = targetValues.Max() - targetValues.Min();
      double interestingValuesRange = Math.Min(Math.Max(targetValuesRange, 1.0), Math.Max(estimatedValuesRange, 1.0));
      double digits = (int)Math.Log10(interestingValuesRange) - 3;
      double zoomInterval = Math.Max(Math.Pow(10, digits), 10E-5);
      this.chart.ChartAreas[0].CursorX.Interval = zoomInterval;
      this.chart.ChartAreas[0].CursorY.Interval = zoomInterval;
    }


    private void UpdateSeries() {
      if (InvokeRequired) Invoke((Action)UpdateSeries);
      else {
        string targetVariableName = Content.ProblemData.TargetVariable.Value;
        Dataset dataset = Content.ProblemData.Dataset;
        int trainingStart = Content.ProblemData.TrainingSamplesStart.Value;
        int trainingEnd = Content.ProblemData.TrainingSamplesEnd.Value;
        int testStart = Content.ProblemData.TestSamplesStart.Value;
        int testEnd = Content.ProblemData.TestSamplesEnd.Value;
        if (this.chart.Series[ALL_SERIES].Points.Count > 0)
          this.chart.Series[ALL_SERIES].Points.DataBindXY(Content.EstimatedValues.ToArray(), "",
            dataset[targetVariableName], "");
        if (this.chart.Series[TRAINING_SERIES].Points.Count > 0)
          this.chart.Series[TRAINING_SERIES].Points.DataBindXY(Content.EstimatedTrainingValues.ToArray(), "",
            dataset.GetVariableValues(targetVariableName, trainingStart, trainingEnd), "");
        if (this.chart.Series[TEST_SERIES].Points.Count > 0)
          this.chart.Series[TEST_SERIES].Points.DataBindXY(Content.EstimatedTestValues.ToArray(), "",
            dataset.GetVariableValues(targetVariableName, testStart, testEnd), "");

        double max = Math.Max(Content.EstimatedValues.Max(), dataset.GetMax(targetVariableName));
        double min = Math.Min(Content.EstimatedValues.Min(), dataset.GetMin(targetVariableName));

        max = Math.Ceiling(max) * 1.2;
        min = Math.Floor(min) * 0.8;

        this.chart.ChartAreas[0].AxisX.Maximum = max;
        this.chart.ChartAreas[0].AxisX.Minimum = min;
        this.chart.ChartAreas[0].AxisY.Maximum = max;
        this.chart.ChartAreas[0].AxisY.Minimum = min;
        UpdateCursorInterval();
      }
    }

    private void ClearChart() {
      this.chart.Series[ALL_SERIES].Points.Clear();
      this.chart.Series[TRAINING_SERIES].Points.Clear();
      this.chart.Series[TEST_SERIES].Points.Clear();      
    }

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (this.chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          series.Points.Clear();
        }
      } else if (Content != null) {
        string targetVariableName = Content.ProblemData.TargetVariable.Value;
        Dataset dataset = Content.ProblemData.Dataset;
        int trainingStart = Content.ProblemData.TrainingSamplesStart.Value;
        int trainingEnd = Content.ProblemData.TrainingSamplesEnd.Value;
        int testStart = Content.ProblemData.TestSamplesStart.Value;
        int testEnd = Content.ProblemData.TestSamplesEnd.Value;

        IEnumerable<double> predictedValues = null;
        IEnumerable<double> targetValues = null;
        switch (series.Name) {
          case ALL_SERIES:
            predictedValues = Content.EstimatedValues;
            targetValues = dataset[targetVariableName];
            break;
          case TRAINING_SERIES:
            predictedValues = Content.EstimatedTrainingValues;
            targetValues = dataset.GetVariableValues(targetVariableName, trainingStart, trainingEnd);
            break;
          case TEST_SERIES:
            predictedValues = Content.EstimatedTestValues;
            targetValues = dataset.GetVariableValues(targetVariableName, testStart, testEnd);
            break;
        }
        series.Points.DataBindXY(predictedValues, "", targetValues, "");
        this.chart.Legends[series.Legend].ForeColor = Color.Black;
        UpdateCursorInterval();
      }
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        this.ToggleSeriesData(result.Series);
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }

    private void chart_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e) {
      this.chart.ChartAreas[0].AxisX.ScaleView.Size = e.NewSize;
      this.chart.ChartAreas[0].AxisY.ScaleView.Size = e.NewSize;
    }

    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[ALL_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[TRAINING_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[2].Cells[1].ForeColor = this.chart.Series[TEST_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
    }
  }
}
