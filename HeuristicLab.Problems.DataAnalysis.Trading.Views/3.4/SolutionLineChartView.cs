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
      this.chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
      this.chart.ChartAreas[0].AxisY2.ScaleView.Zoomable = false;
      this.chart.ChartAreas[0].AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
      this.chart.ChartAreas[0].AxisY2.LabelStyle.Enabled = false;
      this.chart.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
      this.chart.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;
      this.chart.ChartAreas[0].AxisY2.MajorTickMark.Enabled = false;
      this.chart.ChartAreas[0].AxisY2.MinorTickMark.Enabled = false;
      this.chart.ChartAreas[0].CursorY.Interval = 0;
    }

    private void RedrawChart() {
      this.chart.Series.Clear();
      if (Content != null) {
        var trainingRows = Content.ProblemData.TrainingIndices;
        var testRows = Content.ProblemData.TestIndices;
        this.chart.Series.Add(SIGNALS_SERIES_NAME);
        this.chart.Series[SIGNALS_SERIES_NAME].YAxisType = AxisType.Secondary;
        this.chart.Series[SIGNALS_SERIES_NAME].LegendText = SIGNALS_SERIES_NAME;
        this.chart.Series[SIGNALS_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[SIGNALS_SERIES_NAME].Points.DataBindXY(
          trainingRows.Concat(testRows).ToArray(),
          Content.TrainingSignals.Concat(Content.TestSignals).ToArray());
        this.chart.Series[SIGNALS_SERIES_NAME].Tag = Content;

        var trainingPriceChanges = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.PriceChangeVariable,
                                                                               trainingRows);
        var testPriceChanges = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.PriceChangeVariable,
                                                                               testRows);
        IEnumerable<double> accumulatedTrainingPrice = GetAccumulatedProfits(trainingPriceChanges);
        IEnumerable<double> accumulatedTestPrice = GetAccumulatedProfits(testPriceChanges);
        this.chart.Series.Add(PRICEVARIABLE_SERIES_NAME);
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].YAxisType = AxisType.Primary;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].LegendText = PRICEVARIABLE_SERIES_NAME;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].Points.DataBindXY(
          trainingRows.Concat(testRows).ToArray(),
          accumulatedTrainingPrice.Concat(accumulatedTestPrice).ToArray());
        this.chart.Series[PRICEVARIABLE_SERIES_NAME].Tag = Content;


        IEnumerable<double> trainingProfit = OnlineProfitCalculator.GetProfits(trainingPriceChanges, Content.TrainingSignals, Content.ProblemData.TransactionCosts);
        IEnumerable<double> testProfit = OnlineProfitCalculator.GetProfits(testPriceChanges, Content.TestSignals, Content.ProblemData.TransactionCosts);
        IEnumerable<double> accTrainingProfit = GetAccumulatedProfits(trainingProfit);
        IEnumerable<double> accTestProfit = GetAccumulatedProfits(testProfit);
        this.chart.Series.Add(ASSET_SERIES_NAME);
        this.chart.Series[ASSET_SERIES_NAME].YAxisType = AxisType.Primary;
        this.chart.Series[ASSET_SERIES_NAME].LegendText = ASSET_SERIES_NAME;
        this.chart.Series[ASSET_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[ASSET_SERIES_NAME].Points.DataBindXY(
          trainingRows.Concat(testRows).ToArray(),
          accTrainingProfit.Concat(accTestProfit).ToArray());
        this.chart.Series[ASSET_SERIES_NAME].Tag = Content;

        this.UpdateStripLines();
      }
    }

    private IEnumerable<double> GetAccumulatedProfits(IEnumerable<double> xs) {
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
