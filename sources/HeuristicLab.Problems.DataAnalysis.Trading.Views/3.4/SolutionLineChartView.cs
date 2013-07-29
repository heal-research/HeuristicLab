#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Views {
  [View("Line Chart")]
  [Content(typeof(ISolution))]
  public partial class SolutionLineChartView : DataAnalysisSolutionEvaluationView, ISolutionEvaluationView {
    private const string PRICEVARIABLE_SERIES_NAME = "Price";
    private const string SIGNALS_SERIES_NAME = "Signals";
    private const string ASSET_SERIES_NAME = "Asset";


    public new ISolution Content {
      get { return (ISolution)base.Content; }
      set { base.Content = value; }
    }

    public SolutionLineChartView()
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
        this.chart.Series.Add(SIGNALS_SERIES_NAME);
        this.chart.Series[SIGNALS_SERIES_NAME].LegendText = SIGNALS_SERIES_NAME;
        this.chart.Series[SIGNALS_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[SIGNALS_SERIES_NAME].Points.DataBindY(Content.Signals.ToArray());
        this.chart.Series[SIGNALS_SERIES_NAME].Tag = Content;

        IEnumerable<double> accumulatedPrice = GetAccumulatedPrices(Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.PriceVariable));
        this.chart.Series.Add(PRICEVARIABLE_SERIES_NAME);
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].LegendText = PRICEVARIABLE_SERIES_NAME;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].Points.DataBindY(accumulatedPrice.ToArray());
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].Tag = Content;

        IEnumerable<double> profit = OnlineProfitCalculator.GetProfits(Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.PriceVariable), Content.Signals, Content.ProblemData.TransactionCosts);
        IEnumerable<double> accumulatedProfits = GetAccumulatedPrices(profit);
        this.chart.Series.Add(ASSET_SERIES_NAME);
        this.chart.Series[ASSET_SERIES_NAME].LegendText = ASSET_SERIES_NAME;
        this.chart.Series[ASSET_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[ASSET_SERIES_NAME].Points.DataBindY(accumulatedProfits.ToArray());
        this.chart.Series[ASSET_SERIES_NAME].Tag = Content;

        this.UpdateStripLines();
      }
    }

    private IEnumerable<double> GetAccumulatedPrices(IEnumerable<double> xs) {
      double sum = 0;
      foreach (var x in xs) {
        sum += x;
        yield return sum;
      }
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
      RedrawChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
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
