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

namespace HeuristicLab.Problems.DataAnalyis.Views {
  [View("Scatter Plot View")]
  [Content(typeof(DataAnalysisSolution))]
  public partial class ScatterPlotView : ContentView {
    private const string DEFAULT_CAPTION = "Scatter Plot";
    private const string ALL_SERIES = "All Samples";
    private const string TRAINING_SERIES = "Training Samples";
    private const string validationSeries = "Validation Samples";
    private const string TEST_SERIES = "Test Samples";

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

      this.chart.Series.Add(validationSeries);
      this.chart.Series[validationSeries].LegendText = validationSeries;
      this.chart.Series[validationSeries].ChartType = SeriesChartType.FastPoint;

      this.chart.Series.Add(TEST_SERIES);
      this.chart.Series[TEST_SERIES].LegendText = TEST_SERIES;
      this.chart.Series[TEST_SERIES].ChartType = SeriesChartType.FastPoint;

      this.chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
      this.chart.AxisViewChanged += new EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(chart_AxisViewChanged);

      //configure axis                  
      this.chart.ChartAreas[0].AxisX.Title = "Estimated Values";
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 0;
      this.chart.ChartAreas[0].CursorY.Interval = 0;

      this.chart.ChartAreas[0].AxisY.Title = "Target Values";
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
    }

    public ScatterPlotView(IVisualModel visualModel)
      : this() {
      this.VisualModel = visualModel;
    }

    private IVisualModel visualModel;
    public IVisualModel VisualModel {
      get { return this.visualModel; }
      private set {
        if (this.visualModel != null) {
          this.visualModel.Changed -= new EventHandler(model_Changed);                 
        }
        this.visualModel = value;
        if (this.visualModel != null) {
          this.Caption = this.visualModel.ModelName + " " + DEFAULT_CAPTION;
          this.visualModel.Changed += new EventHandler(model_Changed);
          this.UpdateSeries();
          if (!this.chart.Series.Any(s => s.Points.Count > 0))
            this.ToggleSeriesData(this.chart.Series[TRAINING_SERIES]);
        } else {
          this.Caption = DEFAULT_CAPTION;
          this.ClearChart();
        }
      }
    }

    private void UpdateSeries() {
      if (this.chart.Series[ALL_SERIES].Points.Count > 0)
        this.chart.Series[ALL_SERIES].Points.DataBindXY(this.visualModel.PredictedValues.ToArray(), "",
          this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName), "");
      if (this.chart.Series[TRAINING_SERIES].Points.Count > 0)
        this.chart.Series[TRAINING_SERIES].Points.DataBindXY(this.visualModel.PredictedTrainingValues.ToArray(), "",
          this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.TrainingSamplesStart, this.visualModel.TrainingSamplesEnd), "");
      if (this.chart.Series[validationSeries].Points.Count > 0)
        this.chart.Series[validationSeries].Points.DataBindXY(this.visualModel.PredictedValidationValues.ToArray(), "",
          this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.ValidationSamplesStart, this.visualModel.ValidationSamplesEnd), "");
      if (this.chart.Series[TEST_SERIES].Points.Count > 0)
        this.chart.Series[TEST_SERIES].Points.DataBindXY(this.visualModel.PredictedTestValues.ToArray(), "",
          this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.TestSamplesStart, this.visualModel.TestSamplesEnd), "");

      double x = this.visualModel.PredictedValues.Max();
      double y = this.visualModel.Dataset.GetMaximum(this.visualModel.TargetVariableName);
      double max = x > y ? x : y;
      x = this.visualModel.PredictedValues.Min();
      y = this.visualModel.Dataset.GetMinimum(this.visualModel.TargetVariableName);
      double min = x < y ? x : y;

      max = Math.Ceiling(max) * 1.2;
      min = Math.Floor(min) * 0.8;

      this.chart.ChartAreas[0].AxisX.Maximum = max;
      this.chart.ChartAreas[0].AxisX.Minimum = min;
      this.chart.ChartAreas[0].AxisY.Maximum = max;
      this.chart.ChartAreas[0].AxisY.Minimum = min;
    }

    private void ClearChart() {
      this.chart.Series[ALL_SERIES].Points.Clear();
      this.chart.Series[TRAINING_SERIES].Points.Clear();
      this.chart.Series[validationSeries].Points.Clear();
      this.chart.Series[TEST_SERIES].Points.Clear();
    }


    private void model_Changed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Action<object, EventArgs> action = new Action<object, EventArgs>(model_Changed);
        this.Invoke(action, sender, e);
      } else {
        this.Caption = this.visualModel.ModelName + " " + DEFAULT_CAPTION;
        this.UpdateSeries();
      }
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      base.OnClosed( e);
      this.VisualModel = null;
    }

    protected override void ModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      if (InvokeRequired) {
        Action<object, NotifyCollectionChangedEventArgs> action = new Action<object, NotifyCollectionChangedEventArgs>(ModelsCollectionChanged);
        this.Invoke(action, sender, e);
      } else {
        if (e.Action == NotifyCollectionChangedAction.Remove) {
          if (e.OldItems.Contains(this.visualModel))
            this.VisualModel = null;
        }
        if(e.Action == NotifyCollectionChangedAction.Reset)
          this.VisualModel = null;
      }
    }  

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        if (this.chart.Series.Any(s => s != series && s.Points.Count > 0)) {
          series.Points.Clear();    
        }
      } else if (this.visualModel != null) {
        IEnumerable<double> predictedValues = null;
        IEnumerable<double> targetValues = null;
        switch (series.Name) {
          case ALL_SERIES:
            predictedValues = this.visualModel.PredictedValues;
            targetValues = this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName);
            break;
          case TRAINING_SERIES:
            predictedValues = this.visualModel.PredictedTrainingValues;
            targetValues = this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.TrainingSamplesStart, this.visualModel.TrainingSamplesEnd);
            break;
          case validationSeries:
            predictedValues = this.visualModel.PredictedValidationValues;
            targetValues = this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.ValidationSamplesStart, this.visualModel.ValidationSamplesEnd);
            break;
          case TEST_SERIES:
            predictedValues = this.visualModel.PredictedTestValues;
            targetValues = this.visualModel.Dataset.GetVariableValues(this.visualModel.TargetVariableName, this.visualModel.TestSamplesStart, this.visualModel.TestSamplesEnd);
            break;
        }
        series.Points.DataBindXY(predictedValues.ToArray(), "", targetValues, "");
        this.chart.Legends[series.Legend].ForeColor = Color.Black;
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
      e.LegendItems[2].Cells[1].ForeColor = this.chart.Series[validationSeries].Points.Count == 0 ? Color.Gray : Color.Black;
      e.LegendItems[3].Cells[1].ForeColor = this.chart.Series[TEST_SERIES].Points.Count == 0 ? Color.Gray : Color.Black;
    }


    private void ScatterPlotView_DragDrop(object sender, DragEventArgs e) {
      IVisualModel model = this.ExtractModel(e.Data);
      if (model != null) {
        this.VisualModel = model;
      }
    }

    private void ScatterPlotView_DragEnter(object sender, DragEventArgs e) {
      IVisualModel model = this.ExtractModel(e.Data);
      if (model != null)
        e.Effect = DragDropEffects.Link;
      else
        e.Effect = DragDropEffects.None;
    }
  }
}
