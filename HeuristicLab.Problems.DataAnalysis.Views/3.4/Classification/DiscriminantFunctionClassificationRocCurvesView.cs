#region License Information
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("ROC Curves")]
  [Content(typeof(IDiscriminantFunctionClassificationSolution))]
  public partial class DiscriminantFunctionClassificationRocCurvesView : DataAnalysisSolutionEvaluationView {
    private const string xAxisTitle = "False Positive Rate";
    private const string yAxisTitle = "True Positive Rate";
    private const string TrainingSamples = "Training";
    private const string TestSamples = "Test";
    private Dictionary<string, IReadOnlyList<RocAucCalculator.RocPoint>> cachedRocPoints;

    public DiscriminantFunctionClassificationRocCurvesView() {
      InitializeComponent();

      cachedRocPoints = new Dictionary<string, IReadOnlyList<RocAucCalculator.RocPoint>>();

      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.SelectedIndex = 0;

      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].AxisX.Minimum = 0.0;
      chart.ChartAreas[0].AxisX.Maximum = 1.0;
      chart.ChartAreas[0].AxisX.MajorGrid.Interval = 0.2;
      chart.ChartAreas[0].AxisY.Minimum = 0.0;
      chart.ChartAreas[0].AxisY.Maximum = 1.0;
      chart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.2;

      chart.ChartAreas[0].AxisX.Title = xAxisTitle;
      chart.ChartAreas[0].AxisY.Title = yAxisTitle;
    }

    public new IDiscriminantFunctionClassificationSolution Content {
      get { return (IDiscriminantFunctionClassificationSolution)base.Content; }
      set { base.Content = value; }
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

    private void Content_ModelChanged(object sender, EventArgs e) {
      UpdateChart();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateChart();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      chart.Series.Clear();
      if (Content != null) UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else {
        chart.Series.Clear();
        chart.Annotations.Clear();
        cachedRocPoints.Clear();

        IEnumerable<int> rows;

        if (cmbSamples.SelectedItem.ToString() == TrainingSamples) {
          rows = Content.ProblemData.TrainingIndices;
        } else if (cmbSamples.SelectedItem.ToString() == TestSamples) {
          rows = Content.ProblemData.TestIndices;
        } else throw new InvalidOperationException();

        double[] estimatedValues = Content.GetEstimatedValues(rows).ToArray();
        double[] targetClassValues = Content.ProblemData.Dataset.GetDoubleValues(Content.ProblemData.TargetVariable, rows).ToArray();
        
        var rocCurves = RocAucCalculator.CalculateRocCurves(targetClassValues, estimatedValues, Content.ProblemData.ClassValues.ToList(), Content.ProblemData.ClassNames.ToList());
        
        foreach (var kvp in rocCurves) {
          var className = kvp.Key;
          var rocCurve = kvp.Value;
      
          cachedRocPoints[className] = rocCurve;
          double auc = RocAucCalculator.CalculateAreaUnderCurve(rocCurve);
          
          var series = new Series(className) {
            ChartType = SeriesChartType.Line,
            MarkerStyle = MarkerStyle.Diamond,
            MarkerSize = 5,
            LegendToolTip = "AUC: " + auc
          };
          FillSeriesWithDataPoints(series, rocCurve);
          chart.Series.Add(series);
        }
      }
    }

    private void FillSeriesWithDataPoints(Series series, IEnumerable<RocAucCalculator.RocPoint> rocPoints) {
      series.Points.Add(new DataPoint(0, 0));
      foreach (var rocPoint in rocPoints) {
        DataPoint point = new DataPoint();
        point.XValue = rocPoint.FalsePositiveRate;
        point.YValues[0] = rocPoint.TruePositiveRate;
        point.Tag = rocPoint;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("True Positive Rate: " + rocPoint.TruePositiveRate);
        sb.AppendLine("False Positive Rate: " + rocPoint.FalsePositiveRate);
        sb.AppendLine("Upper Threshold: " + rocPoint.UpperThreshold);
        sb.AppendLine("Lower Threshold: " + rocPoint.LowerThreshold);
        point.ToolTip = sb.ToString();

        series.Points.Add(point);
      }
      series.Points.Add(new DataPoint(1, 1));
    }

    private void cmbSamples_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (Content != null)
        UpdateChart();
    }


    #region show / hide series
    private void ToggleSeries(Series series) {
      if (series.Points.Count == 0)
        FillSeriesWithDataPoints(series, cachedRocPoints[series.Name]);
      else
        series.Points.Clear();
    }
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        if (result.Series != null) ToggleSeries(result.Series);
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

      string newTooltipText = string.Empty;
      if (result.ChartElementType == ChartElementType.DataPoint)
        newTooltipText = ((DataPoint)result.Object).ToolTip;

      string oldTooltipText = this.toolTip.GetToolTip(chart);
      if (newTooltipText != oldTooltipText)
        this.toolTip.SetToolTip(chart, newTooltipText);
    }
    #endregion
  }
}
