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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Algorithms.DataAnalysis;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Error Characteristics Curve")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionErrorCharacteristicsCurveView : DataAnalysisSolutionEvaluationView {
    protected const string TrainingSamples = "Training";
    protected const string TestSamples = "Test";
    protected const string AllSamples = "All Samples";

    public RegressionSolutionErrorCharacteristicsCurveView()
      : base() {
      InitializeComponent();

      cmbSamples.Items.Add(TrainingSamples);
      cmbSamples.Items.Add(TestSamples);
      cmbSamples.Items.Add(AllSamples);

      cmbSamples.SelectedIndex = 0;

      residualComboBox.SelectedIndex = 0;

      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].AxisX.Title = residualComboBox.SelectedItem.ToString();
      chart.ChartAreas[0].AxisX.Minimum = 0.0;
      chart.ChartAreas[0].AxisX.Maximum = 0.0;
      chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
      chart.ChartAreas[0].CursorX.Interval = 0.01;

      chart.ChartAreas[0].AxisY.Title = "Ratio of Residuals";
      chart.ChartAreas[0].AxisY.Minimum = 0.0;
      chart.ChartAreas[0].AxisY.Maximum = 1.0;
      chart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.2;
      chart.ChartAreas[0].CursorY.Interval = 0.01;
    }

    // the view holds one regression solution as content but also contains several other regression solutions for comparison
    // the following invariants must hold
    // (Solutions.IsEmpty && Content == null) ||
    // (Solutions[0] == Content && Solutions.All(s => s.ProblemData.TargetVariable == Content.TargetVariable))

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    private readonly IList<IRegressionSolution> solutions = new List<IRegressionSolution>();
    public IEnumerable<IRegressionSolution> Solutions {
      get { return solutions.AsEnumerable(); }
    }

    public IRegressionProblemData ProblemData {
      get {
        if (Content == null) return null;
        return Content.ProblemData;
      }
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

    protected virtual void Content_ModelChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ModelChanged, sender, e);
      else {
        // recalculate baseline solutions (for symbolic regression models the features used in the model might have changed)
        solutions.Clear(); // remove all
        solutions.Add(Content); // re-add the first solution
        // and recalculate all other solutions
        foreach (var sol in CreateBaselineSolutions()) {
          solutions.Add(sol);
        }
        UpdateChart();
      }
    }
    protected virtual void Content_ProblemDataChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)Content_ProblemDataChanged, sender, e);
      else {
        // recalculate baseline solutions
        solutions.Clear(); // remove all
        solutions.Add(Content); // re-add the first solution
        // and recalculate all other solutions
        foreach (var sol in CreateBaselineSolutions()) {
          solutions.Add(sol);
        }
        UpdateChart();
      }
    }
    protected override void OnContentChanged() {
      base.OnContentChanged();
      // the content object is always stored as the first element in solutions
      solutions.Clear();
      ReadOnly = Content == null;
      if (Content != null) {
        // recalculate all solutions
        solutions.Add(Content);
        if (ProblemData.TrainingIndices.Any()) {
          foreach (var sol in CreateBaselineSolutions())
            solutions.Add(sol);
          // more solutions can be added by drag&drop
        }
      }
      UpdateChart();
    }

    protected virtual void UpdateChart() {
      chart.Series.Clear();
      chart.Annotations.Clear();
      chart.ChartAreas[0].AxisX.Maximum = 0.0;
      chart.ChartAreas[0].CursorX.Interval = 0.01;

      if (Content == null) return;
      if (cmbSamples.SelectedItem.ToString() == TrainingSamples && !ProblemData.TrainingIndices.Any()) return;
      if (cmbSamples.SelectedItem.ToString() == TestSamples && !ProblemData.TestIndices.Any()) return;

      foreach (var sol in Solutions) {
        AddSeries(sol);
      }

      chart.ChartAreas[0].AxisX.Title = residualComboBox.SelectedItem.ToString();
    }

    protected void AddSeries(IRegressionSolution solution) {
      if (chart.Series.Any(s => s.Name == solution.Name)) return;

      Series solutionSeries = new Series(solution.Name);
      solutionSeries.Tag = solution;
      solutionSeries.ChartType = SeriesChartType.FastLine;
      var residuals = GetResiduals(GetOriginalValues(), GetEstimatedValues(solution));

      var maxValue = residuals.Max();
      if (maxValue >= chart.ChartAreas[0].AxisX.Maximum) {
        double scale = Math.Pow(10, Math.Floor(Math.Log10(maxValue)));
        var maximum = scale * (1 + (int)(maxValue / scale));
        chart.ChartAreas[0].AxisX.Maximum = maximum;
        chart.ChartAreas[0].CursorX.Interval = residuals.Min() / 100;
      }

      UpdateSeries(residuals, solutionSeries);

      solutionSeries.ToolTip = "Area over Curve: " + CalculateAreaOverCurve(solutionSeries);
      solutionSeries.LegendToolTip = "Double-click to open model";
      chart.Series.Add(solutionSeries);
    }

    protected void UpdateSeries(List<double> residuals, Series series) {
      series.Points.Clear();
      residuals.Sort();
      if (!residuals.Any() || residuals.All(double.IsNaN)) return;

      series.Points.AddXY(0, 0);
      for (int i = 0; i < residuals.Count; i++) {
        var point = new DataPoint();
        if (residuals[i] > chart.ChartAreas[0].AxisX.Maximum) {
          point.XValue = chart.ChartAreas[0].AxisX.Maximum;
          point.YValues[0] = ((double)i) / residuals.Count;
          point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
          series.Points.Add(point);
          break;
        }

        point.XValue = residuals[i];
        point.YValues[0] = ((double)i + 1) / residuals.Count;
        point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
        series.Points.Add(point);
      }

      if (series.Points.Last().XValue < chart.ChartAreas[0].AxisX.Maximum) {
        var point = new DataPoint();
        point.XValue = chart.ChartAreas[0].AxisX.Maximum;
        point.YValues[0] = 1;
        point.ToolTip = "Error: " + point.XValue + "\n" + "Samples: " + point.YValues[0];
        series.Points.Add(point);
      }
    }

    protected IEnumerable<double> GetOriginalValues() {
      IEnumerable<double> originalValues;
      switch (cmbSamples.SelectedItem.ToString()) {
        case TrainingSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices);
          break;
        case TestSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndices);
          break;
        case AllSamples:
          originalValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable);
          break;
        default:
          throw new NotSupportedException();
      }
      return originalValues;
    }

    protected IEnumerable<double> GetEstimatedValues(IRegressionSolution solution) {
      IEnumerable<double> estimatedValues;
      switch (cmbSamples.SelectedItem.ToString()) {
        case TrainingSamples:
          estimatedValues = solution.EstimatedTrainingValues;
          break;
        case TestSamples:
          estimatedValues = solution.EstimatedTestValues;
          break;
        case AllSamples:
          estimatedValues = solution.EstimatedValues;
          break;
        default:
          throw new NotSupportedException();
      }
      return estimatedValues;
    }

    protected virtual List<double> GetResiduals(IEnumerable<double> originalValues, IEnumerable<double> estimatedValues) {
      switch (residualComboBox.SelectedItem.ToString()) {
        case "Absolute error": return originalValues.Zip(estimatedValues, (x, y) => Math.Abs(x - y)).ToList();
        case "Squared error": return originalValues.Zip(estimatedValues, (x, y) => (x - y) * (x - y)).ToList();
        case "Relative error": return originalValues.Zip(estimatedValues, (x, y) => x.IsAlmost(0.0) ? -1 : Math.Abs((x - y) / x))
          .Where(x => x > 0) // remove entries where the original value is 0
          .ToList();
        default: throw new NotSupportedException();
      }
    }

    private double CalculateAreaOverCurve(Series series) {
      if (series.Points.Count < 1) return 0;

      double auc = 0.0;
      for (int i = 1; i < series.Points.Count; i++) {
        double width = series.Points[i].XValue - series.Points[i - 1].XValue;
        double y1 = 1 - series.Points[i - 1].YValues[0];
        double y2 = 1 - series.Points[i].YValues[0];

        auc += (y1 + y2) * width / 2;
      }

      return auc;
    }

    protected void cmbSamples_SelectedIndexChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)cmbSamples_SelectedIndexChanged, sender, e);
      else UpdateChart();
    }

    private void Chart_MouseDoubleClick(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType != ChartElementType.LegendItem) return;

      MainFormManager.MainForm.ShowContent((IRegressionSolution)result.Series.Tag);
    }

    protected virtual IEnumerable<IRegressionSolution> CreateBaselineSolutions() {
      yield return CreateConstantSolution();
      yield return CreateLinearSolution();
    }

    private IRegressionSolution CreateConstantSolution() {
      double averageTrainingTarget = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).Average();
      var model = new ConstantRegressionModel(averageTrainingTarget);
      var solution = new ConstantRegressionSolution(model, (IRegressionProblemData)ProblemData.Clone());
      solution.Name = "Baseline (constant)";
      return solution;
    }
    private IRegressionSolution CreateLinearSolution() {
      double rmsError, cvRmsError;
      var solution = LinearRegression.CreateLinearRegressionSolution((IRegressionProblemData)ProblemData.Clone(), out rmsError, out cvRmsError);
      solution.Name = "Baseline (linear)";
      return solution;
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        Cursor = Cursors.Hand;
      } else {
        Cursor = Cursors.Default;
      }
    }

    private void chart_DragDrop(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(HeuristicLab.Common.Constants.DragDropDataFormat)) {

        var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        var dataAsRegressionSolution = data as IRegressionSolution;
        var dataAsResult = data as IResult;

        if (dataAsRegressionSolution != null) {
          solutions.Add((IRegressionSolution)dataAsRegressionSolution.Clone());
        } else if (dataAsResult != null && dataAsResult.Value is IRegressionSolution) {
          solutions.Add((IRegressionSolution)dataAsResult.Value.Clone());
        }

        UpdateChart();
      }
    }

    private void chart_DragEnter(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!e.Data.GetDataPresent(HeuristicLab.Common.Constants.DragDropDataFormat)) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      var dataAsRegressionSolution = data as IRegressionSolution;
      var dataAsResult = data as IResult;

      if (!ReadOnly &&
        (dataAsRegressionSolution != null || (dataAsResult != null && dataAsResult.Value is IRegressionSolution))) {
        e.Effect = DragDropEffects.Copy;
      }
    }

    private void residualComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateChart();
    }
  }
}
