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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Line Chart")]
  [Content(typeof(ITradingSolution))]
  public partial class TradingSolutionLineChartView : ItemView, ITradingSolutionEvaluationView {
    private const string PRICEVARIABLE_SERIES_NAME = "PriceVariable";
    private const string SIGNALS_SERIES_NAME = "Signals";

    public new ITradingSolution Content {
      get { return (ITradingSolution)base.Content; }
      set { base.Content = value; }
    }

    public TradingSolutionLineChartView()
      : base() {
      InitializeComponent();
      //configure axis
      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 1;

      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorY.Interval = 0;
    }

    private void RedrawChart() {
      this.chart.Series.Clear();
      if (Content != null) {
        this.chart.Series.Add(PRICEVARIABLE_SERIES_NAME);
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].LegendText = Content.ProblemData.PriceVariable;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].Points.DataBindY(Content.ProblemData.Dataset.GetVariableValues(Content.ProblemData.PriceVariable));
        this.UpdateStripLines();

        this.chart.Series.Add(SIGNALS_SERIES_NAME);
        this.chart.Series[SIGNALS_SERIES_NAME].LegendText = Content.ItemName;
        this.chart.Series[SIGNALS_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[SIGNALS_SERIES_NAME].Points.DataBindY(Content.Signals.ToArray());
        this.chart.Series[SIGNALS_SERIES_NAME].Tag = Content;
        UpdateCursorInterval();
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[SIGNALS_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[PRICEVARIABLE_SERIES_NAME].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
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

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      RedrawChart();
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      UpdateEstimatedValuesLineChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      RedrawChart();
    }

    private void UpdateEstimatedValuesLineChart() {
      if (InvokeRequired) Invoke((Action)UpdateEstimatedValuesLineChart);
      else {
        if (this.chart.Series.Count > 0) {
          Series s = this.chart.Series.SingleOrDefault(x => x.Tag == Content);
          if (s != null) {
            s.Points.DataBindY(Content.Signals.ToArray());
            s.LegendText = Content.ItemName;
            this.UpdateStripLines();
            UpdateCursorInterval();
          }
        }
      }
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
      this.CreateAndAddStripLine("Training", Color.FromArgb(20, Color.Green),
        Content.ProblemData.TrainingPartition.Start,
        Content.ProblemData.TrainingPartition.End);
      this.CreateAndAddStripLine("Test", Color.FromArgb(20, Color.Red),
        Content.ProblemData.TestPartition.Start,
        Content.ProblemData.TestPartition.End);
    }

    private void CreateAndAddStripLine(string title, Color c, int start, int end) {
      StripLine stripLine = new StripLine();
      stripLine.BackColor = c;
      stripLine.Text = title;
      stripLine.Font = new Font("Times New Roman", 12, FontStyle.Bold);
      stripLine.StripWidth = end - start;
      stripLine.IntervalOffset = start;
      this.chart.ChartAreas[0].AxisX.StripLines.Add(stripLine);
    }
  }
}
