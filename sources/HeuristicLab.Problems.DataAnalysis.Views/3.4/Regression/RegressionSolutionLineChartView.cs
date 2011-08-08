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
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Line Chart")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionLineChartView : DataAnalysisSolutionEvaluationView {
    private const string TARGETVARIABLE_SERIES_NAME = "Target Variable";
    private const string ESTIMATEDVALUES_TRAINING_SERIES_NAME = "Estimated Values (training)";
    private const string ESTIMATEDVALUES_TEST_SERIES_NAME = "Estimated Values (test)";

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public RegressionSolutionLineChartView()
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

        this.chart.Series.Add(TARGETVARIABLE_SERIES_NAME);
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].LegendText = Content.ProblemData.TargetVariable;
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[TARGETVARIABLE_SERIES_NAME].Points.DataBindXY(Enumerable.Range(0, Content.ProblemData.Dataset.Rows).ToArray(),
          Content.ProblemData.Dataset.GetVariableValues(Content.ProblemData.TargetVariable));

        this.chart.Series.Add(ESTIMATEDVALUES_TRAINING_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].LegendText = ESTIMATEDVALUES_TRAINING_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Points.DataBindXY(Content.ProblemData.TrainingIndizes.ToArray(),
          Content.EstimatedTrainingValues.ToArray());
        this.chart.DataManipulator.InsertEmptyPoints(Content.ProblemData.Dataset.Rows, IntervalType.Number, ESTIMATEDVALUES_TRAINING_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME].Tag = Content;

        this.chart.Series.Add(ESTIMATEDVALUES_TEST_SERIES_NAME);
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].LegendText = ESTIMATEDVALUES_TEST_SERIES_NAME;
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].ChartType = SeriesChartType.FastLine;
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Points.DataBindXY(Content.ProblemData.TestIndizes.ToArray(),
          Content.EstimatedTestValues.ToArray());
        this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME].Tag = Content;
        UpdateCursorInterval();
        this.UpdateStripLines();
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
          Series s = this.chart.Series[ESTIMATEDVALUES_TRAINING_SERIES_NAME];
          if (s != null) {
            s.Points.DataBindXY(Content.ProblemData.TrainingIndizes.ToArray(), Content.EstimatedTrainingValues.ToArray());
            s.LegendText = ESTIMATEDVALUES_TRAINING_SERIES_NAME;
          }
          s = this.chart.Series[ESTIMATEDVALUES_TEST_SERIES_NAME];
          if (s != null) {
            s.Points.DataBindXY(Content.ProblemData.TestIndizes.ToArray(), Content.EstimatedTestValues.ToArray());
            s.LegendText = ESTIMATEDVALUES_TEST_SERIES_NAME;
          }
          this.UpdateStripLines();
          UpdateCursorInterval();
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

      int[] attr = new int[Content.ProblemData.Dataset.Rows + 1]; // add a virtual last row that is again empty to simplify loop further down
      foreach (var row in Content.ProblemData.TrainingIndizes) {
        attr[row] += 1;
      }
      foreach (var row in Content.ProblemData.TestIndizes) {
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
  }
}
