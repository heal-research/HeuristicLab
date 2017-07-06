#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  public partial class FactorPartialDependencePlot : UserControl, IPartialDependencePlot {
    private ModifiableDataset sharedFixedVariables; // used for synchronising variable values between charts
    private ModifiableDataset internalDataset; // holds the x values for each point drawn

    private CancellationTokenSource cancelCurrentRecalculateSource;

    private readonly List<IRegressionSolution> solutions;
    private readonly Dictionary<IRegressionSolution, Series> seriesCache;
    private readonly Dictionary<IRegressionSolution, Series> ciSeriesCache;

    #region Properties
    public string XAxisTitle {
      get { return chart.ChartAreas[0].AxisX.Title; }
      set { chart.ChartAreas[0].AxisX.Title = value; }
    }

    public string YAxisTitle {
      get { return chart.ChartAreas[0].AxisY.Title; }
      set { chart.ChartAreas[0].AxisY.Title = value; }
    }

    public bool ShowLegend {
      get { return chart.Legends[0].Enabled; }
      set { chart.Legends[0].Enabled = value; }
    }
    public bool ShowCursor {
      get { return chart.Annotations[0].Visible; }
      set {
        chart.Annotations[0].Visible = value;
        if (!value) chart.Titles[0].Text = string.Empty;
      }
    }

    private int yAxisTicks = 5;
    public int YAxisTicks {
      get { return yAxisTicks; }
      set {
        if (value != yAxisTicks) {
          yAxisTicks = value;
          SetupAxis(chart.ChartAreas[0].AxisY, yMin, yMax, YAxisTicks, FixedYAxisMin, FixedYAxisMax);
          RecalculateInternalDataset();
        }
      }
    }
    private double? fixedYAxisMin;
    public double? FixedYAxisMin {
      get { return fixedYAxisMin; }
      set {
        if ((value.HasValue && fixedYAxisMin.HasValue && !value.Value.IsAlmost(fixedYAxisMin.Value)) || (value.HasValue != fixedYAxisMin.HasValue)) {
          fixedYAxisMin = value;
          SetupAxis(chart.ChartAreas[0].AxisY, yMin, yMax, YAxisTicks, FixedYAxisMin, FixedYAxisMax);
        }
      }
    }
    private double? fixedYAxisMax;
    public double? FixedYAxisMax {
      get { return fixedYAxisMax; }
      set {
        if ((value.HasValue && fixedYAxisMax.HasValue && !value.Value.IsAlmost(fixedYAxisMax.Value)) || (value.HasValue != fixedYAxisMax.HasValue)) {
          fixedYAxisMax = value;
          SetupAxis(chart.ChartAreas[0].AxisY, yMin, yMax, YAxisTicks, FixedYAxisMin, FixedYAxisMax);
        }
      }
    }

    private string freeVariable;
    public string FreeVariable {
      get { return freeVariable; }
      set {
        if (value == freeVariable) return;
        if (solutions.Any(s => !s.ProblemData.Dataset.StringVariables.Contains(value))) {
          throw new ArgumentException("Variable does not exist in the ProblemData of the Solutions.");
        }
        freeVariable = value;
        RecalculateInternalDataset();
      }
    }

    private double yMin;
    public double YMin {
      get { return yMin; }
    }
    private double yMax;
    public double YMax {
      get { return yMax; }
    }

    public bool IsZoomed {
      get { return chart.ChartAreas[0].AxisX.ScaleView.IsZoomed; }
    }

    internal ElementPosition InnerPlotPosition {
      get { return chart.ChartAreas[0].InnerPlotPosition; }
    }
    #endregion

    private List<string> variableValues;

    public event EventHandler ChartPostPaint;

    public FactorPartialDependencePlot() {
      InitializeComponent();

      solutions = new List<IRegressionSolution>();
      seriesCache = new Dictionary<IRegressionSolution, Series>();
      ciSeriesCache = new Dictionary<IRegressionSolution, Series>();

      // Configure axis
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      chart.ChartAreas[0].CursorX.Interval = 0;

      chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      chart.ChartAreas[0].CursorY.Interval = 0;

      Disposed += Control_Disposed;
    }

    private void Control_Disposed(object sender, EventArgs e) {
      if (cancelCurrentRecalculateSource != null)
        cancelCurrentRecalculateSource.Cancel();
    }

    public void Configure(IEnumerable<IRegressionSolution> solutions, ModifiableDataset sharedFixedVariables, string freeVariable, IList<string> variableValues, bool initializeAxisRanges = true) {
      if (!SolutionsCompatible(solutions))
        throw new ArgumentException("Solutions are not compatible with the problem data.");
      this.freeVariable = freeVariable;
      this.variableValues = new List<string>(variableValues);

      this.solutions.Clear();
      this.solutions.AddRange(solutions);

      // add an event such that whenever a value is changed in the shared dataset, 
      // this change is reflected in the internal dataset (where the value becomes a whole column)
      if (this.sharedFixedVariables != null)
        this.sharedFixedVariables.ItemChanged -= sharedFixedVariables_ItemChanged;
      this.sharedFixedVariables = sharedFixedVariables;
      this.sharedFixedVariables.ItemChanged += sharedFixedVariables_ItemChanged;

      RecalculateInternalDataset();

      chart.Series.Clear();
      seriesCache.Clear();
      ciSeriesCache.Clear();
      foreach (var solution in this.solutions) {
        var series = CreateSeries(solution);
        seriesCache.Add(solution, series.Item1);
        if (series.Item2 != null)
          ciSeriesCache.Add(solution, series.Item2);
      }

      InitSeriesData();
      OrderAndColorSeries();

    }

    public async Task RecalculateAsync(bool updateOnFinish = true, bool resetYAxis = true) {
      if (IsDisposed
        || sharedFixedVariables == null || !solutions.Any() || string.IsNullOrEmpty(freeVariable)
        || !variableValues.Any())
        return;

      calculationPendingTimer.Start();

      // cancel previous recalculate call
      if (cancelCurrentRecalculateSource != null)
        cancelCurrentRecalculateSource.Cancel();
      cancelCurrentRecalculateSource = new CancellationTokenSource();
      var cancellationToken = cancelCurrentRecalculateSource.Token;

      // Update series
      try {
        var limits = await UpdateAllSeriesDataAsync(cancellationToken);

        yMin = limits.Lower;
        yMax = limits.Upper;
        // Set y-axis
        if (resetYAxis)
          SetupAxis(chart.ChartAreas[0].AxisY, yMin, yMax, YAxisTicks, FixedYAxisMin, FixedYAxisMax);

        calculationPendingTimer.Stop();
        calculationPendingLabel.Visible = false;
        if (updateOnFinish)
          Update();
      } catch (OperationCanceledException) { } catch (AggregateException ae) {
        if (!ae.InnerExceptions.Any(e => e is OperationCanceledException))
          throw;
      }
    }

    public void UpdateTitlePosition() {
      var title = chart.Titles[0];
      var plotArea = InnerPlotPosition;

      title.Visible = plotArea.Width != 0;

      title.Position.X = plotArea.X + (plotArea.Width / 2);
    }

    private void SetupAxis(Axis axis, double minValue, double maxValue, int ticks, double? fixedAxisMin, double? fixedAxisMax) {
      if (minValue < maxValue) {
        double axisMin, axisMax, axisInterval;
        ChartUtil.CalculateAxisInterval(minValue, maxValue, ticks, out axisMin, out axisMax, out axisInterval);
        axis.Minimum = fixedAxisMin ?? axisMin;
        axis.Maximum = fixedAxisMax ?? axisMax;
        axis.Interval = (axis.Maximum - axis.Minimum) / ticks;
      }

      try {
        chart.ChartAreas[0].RecalculateAxesScale();
      } catch (InvalidOperationException) {
        // Can occur if eg. axis min == axis max
      }
    }


    private void RecalculateInternalDataset() {
      if (sharedFixedVariables == null)
        return;

      var factorValues = new List<string>(variableValues);

      var variables = sharedFixedVariables.VariableNames.ToList();
      var values = new List<IList>();
      foreach (var varName in variables) {
        if (varName == FreeVariable) {
          values.Add(factorValues);
        } else if (sharedFixedVariables.VariableHasType<double>(varName)) {
          values.Add(Enumerable.Repeat(sharedFixedVariables.GetDoubleValue(varName, 0), factorValues.Count).ToList());
        } else if (sharedFixedVariables.VariableHasType<string>(varName)) {
          values.Add(Enumerable.Repeat(sharedFixedVariables.GetStringValue(varName, 0), factorValues.Count).ToList());
        }
      }

      internalDataset = new ModifiableDataset(variables, values);
    }

    private Tuple<Series, Series> CreateSeries(IRegressionSolution solution) {
      var series = new Series {
        ChartType = SeriesChartType.Column,
        Name = solution.ProblemData.TargetVariable + " " + solutions.IndexOf(solution),
        XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String
      };
      series.LegendText = series.Name;

      Series confidenceIntervalSeries = null;
      confidenceIntervalSeries = new Series {
        ChartType = SeriesChartType.BoxPlot,
        XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String,
        Color = Color.Black,
        YValuesPerPoint = 5,
        Name = "95% Conf. Interval " + series.Name,
        IsVisibleInLegend = false
      };
      return Tuple.Create(series, confidenceIntervalSeries);
    }

    private void OrderAndColorSeries() {
      chart.SuspendRepaint();

      chart.Series.Clear();
      // Add mean series for applying palette colors
      foreach (var solution in solutions) {
        chart.Series.Add(seriesCache[solution]);
      }

      chart.Palette = ChartColorPalette.BrightPastel;
      chart.ApplyPaletteColors();
      chart.Palette = ChartColorPalette.None;

      // Add confidence interval series after its coresponding series for correct z index
      foreach (var solution in solutions) {
        Series ciSeries;
        if (ciSeriesCache.TryGetValue(solution, out ciSeries)) {
          int idx = chart.Series.IndexOf(seriesCache[solution]);
          chart.Series.Insert(idx + 1, ciSeries);
        }
      }

      chart.ResumeRepaint(true);
    }

    private async Task<DoubleLimit> UpdateAllSeriesDataAsync(CancellationToken cancellationToken) {
      var updateTasks = solutions.Select(solution => UpdateSeriesDataAsync(solution, cancellationToken));

      double min = double.MaxValue, max = double.MinValue;
      foreach (var update in updateTasks) {
        var limit = await update;
        if (limit.Lower < min) min = limit.Lower;
        if (limit.Upper > max) max = limit.Upper;
      }

      return new DoubleLimit(min, max);
    }

    private Task<DoubleLimit> UpdateSeriesDataAsync(IRegressionSolution solution, CancellationToken cancellationToken) {
      return Task.Run(() => {
        var yvalues = solution.Model.GetEstimatedValues(internalDataset, Enumerable.Range(0, internalDataset.Rows)).ToList();

        double min = double.MaxValue, max = double.MinValue;

        var series = seriesCache[solution];
        for (int i = 0; i < variableValues.Count; i++) {
          series.Points[i].SetValueXY(variableValues[i], yvalues[i]);
          if (yvalues[i] < min) min = yvalues[i];
          if (yvalues[i] > max) max = yvalues[i];
        }
        chart.Invalidate();

        cancellationToken.ThrowIfCancellationRequested();

        var confidenceBoundSolution = solution as IConfidenceRegressionSolution;
        if (confidenceBoundSolution != null) {
          var confidenceIntervalSeries = ciSeriesCache[solution];
          var variances = confidenceBoundSolution.Model.GetEstimatedVariances(internalDataset, Enumerable.Range(0, internalDataset.Rows)).ToList();
          for (int i = 0; i < variableValues.Count; i++) {
            var lower = yvalues[i] - 1.96 * Math.Sqrt(variances[i]);
            var upper = yvalues[i] + 1.96 * Math.Sqrt(variances[i]);
            confidenceIntervalSeries.Points[i].SetValueXY(variableValues[i], lower, upper, yvalues[i], yvalues[i], yvalues[i]);
            if (lower < min) min = lower;
            if (upper > max) max = upper;
          }
          chart.Invalidate();
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new DoubleLimit(min, max);
      }, cancellationToken);
    }

    private void InitSeriesData() {
      if (internalDataset == null)
        return;

      foreach (var solution in solutions)
        InitSeriesData(solution, variableValues);
    }

    private void InitSeriesData(IRegressionSolution solution, IList<string> values) {

      var series = seriesCache[solution];
      series.Points.SuspendUpdates();
      series.Points.Clear();
      for (int i = 0; i < values.Count; i++) {
        series.Points.AddXY(values[i], 0.0);
        series.Points.Last().ToolTip = values[i];
      }

      UpdateAllSeriesStyles(variableValues.IndexOf(sharedFixedVariables.GetStringValue(FreeVariable, 0)));
      series.Points.ResumeUpdates();

      Series confidenceIntervalSeries;
      if (ciSeriesCache.TryGetValue(solution, out confidenceIntervalSeries)) {
        confidenceIntervalSeries.Points.SuspendUpdates();
        confidenceIntervalSeries.Points.Clear();
        for (int i = 0; i < values.Count; i++)
          confidenceIntervalSeries.Points.AddXY(values[i], 0.0, 0.0, 0.0, 0.0, 0.0);
        confidenceIntervalSeries.Points.ResumeUpdates();
      }
    }

    public async Task AddSolutionAsync(IRegressionSolution solution) {
      if (!SolutionsCompatible(solutions.Concat(new[] { solution })))
        throw new ArgumentException("The solution is not compatible with the problem data.");
      if (solutions.Contains(solution))
        return;

      solutions.Add(solution);

      var series = CreateSeries(solution);
      seriesCache.Add(solution, series.Item1);
      if (series.Item2 != null)
        ciSeriesCache.Add(solution, series.Item2);

      InitSeriesData(solution, variableValues);
      OrderAndColorSeries();

      await RecalculateAsync();
      var args = new EventArgs<IRegressionSolution>(solution);
      OnSolutionAdded(this, args);
    }

    public async Task RemoveSolutionAsync(IRegressionSolution solution) {
      throw new NotSupportedException();
    }

    private static bool SolutionsCompatible(IEnumerable<IRegressionSolution> solutions) {
      var refSolution = solutions.First();
      var refSolVars = refSolution.ProblemData.Dataset.VariableNames;
      foreach (var solution in solutions.Skip(1)) {
        var variables1 = solution.ProblemData.Dataset.VariableNames;
        if (!variables1.All(refSolVars.Contains))
          return false;

        foreach (var factorVar in variables1.Where(solution.ProblemData.Dataset.VariableHasType<string>)) {
          var distinctVals = refSolution.ProblemData.Dataset.GetStringValues(factorVar).Distinct();
          if (solution.ProblemData.Dataset.GetStringValues(factorVar).Any(val => !distinctVals.Contains(val))) return false;
        }
      }
      return true;
    }

    #region Events
    public event EventHandler<EventArgs<IRegressionSolution>> SolutionAdded;
    public void OnSolutionAdded(object sender, EventArgs<IRegressionSolution> args) {
      var added = SolutionAdded;
      if (added == null) return;
      added(sender, args);
    }

    public event EventHandler<EventArgs<IRegressionSolution>> SolutionRemoved;
    public void OnSolutionRemoved(object sender, EventArgs<IRegressionSolution> args) {
      var removed = SolutionRemoved;
      if (removed == null) return;
      removed(sender, args);
    }

    public event EventHandler VariableValueChanged;
    public void OnVariableValueChanged(object sender, EventArgs args) {
      var changed = VariableValueChanged;
      if (changed == null) return;
      changed(sender, args);
    }

    public event EventHandler ZoomChanged;
    public void OnZoomChanged(object sender, EventArgs args) {
      var changed = ZoomChanged;
      if (changed == null) return;
      changed(sender, args);
    }

    private void sharedFixedVariables_ItemChanged(object o, EventArgs<int, int> e) {
      if (o != sharedFixedVariables) return;
      var variables = sharedFixedVariables.VariableNames.ToList();
      var rowIndex = e.Value;
      var columnIndex = e.Value2;

      var variableName = variables[columnIndex];
      if (variableName == FreeVariable) return;
      if (internalDataset.VariableHasType<double>(variableName)) {
        var v = sharedFixedVariables.GetDoubleValue(variableName, rowIndex);
        var values = new List<double>(Enumerable.Repeat(v, internalDataset.Rows));
        internalDataset.ReplaceVariable(variableName, values);
      } else if (internalDataset.VariableHasType<string>(variableName)) {
        var v = sharedFixedVariables.GetStringValue(variableName, rowIndex);
        var values = new List<String>(Enumerable.Repeat(v, internalDataset.Rows));
        internalDataset.ReplaceVariable(variableName, values);
      } else {
        // unsupported type 
        throw new NotSupportedException();
      }
    }

    private async void chart_DragDrop(object sender, DragEventArgs e) {
      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (data != null) {
        var solution = data as IRegressionSolution;
        if (!solutions.Contains(solution))
          await AddSolutionAsync(solution);
      }
    }
    private void chart_DragEnter(object sender, DragEventArgs e) {
      if (!e.Data.GetDataPresent(HeuristicLab.Common.Constants.DragDropDataFormat)) return;
      e.Effect = DragDropEffects.None;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      var regressionSolution = data as IRegressionSolution;
      if (regressionSolution != null) {
        e.Effect = DragDropEffects.Copy;
      }
    }

    private void calculationPendingTimer_Tick(object sender, EventArgs e) {
      calculationPendingLabel.Visible = true;
      Update();
    }

    private void chart_SelectionRangeChanged(object sender, CursorEventArgs e) {
      OnZoomChanged(this, EventArgs.Empty);
    }

    private void chart_Resize(object sender, EventArgs e) {
      UpdateTitlePosition();
    }

    private void chart_PostPaint(object sender, ChartPaintEventArgs e) {
      if (ChartPostPaint != null)
        ChartPostPaint(this, EventArgs.Empty);
    }
    #endregion

    private void chart_MouseClick(object sender, MouseEventArgs e) {
      var hitTestResult = chart.HitTest(e.X, e.Y, ChartElementType.DataPoint);
      if (hitTestResult != null && hitTestResult.ChartElementType == ChartElementType.DataPoint) {
        var series = hitTestResult.Series;
        var dataPoint = (DataPoint)hitTestResult.Object;
        var idx = series.Points.IndexOf(dataPoint);
        UpdateSelectedValue(variableValues[idx]);

        UpdateAllSeriesStyles(idx);
      }
    }

    private void UpdateAllSeriesStyles(int selectedValueIndex) {
      if (ShowCursor) {
        chart.Titles[0].Text = FreeVariable + " : " + variableValues[selectedValueIndex];
        chart.Update();
      }
      foreach (var s in seriesCache.Values) {
        if (s.ChartType == SeriesChartType.Column)
          for (int i = 0; i < s.Points.Count; i++) {
            if (i != selectedValueIndex) {
              s.Points[i].BorderDashStyle = ChartDashStyle.NotSet;
            } else {
              s.Points[i].BorderDashStyle = ChartDashStyle.Dash;
              s.Points[i].BorderColor = Color.Red;
            }
          }
      }
    }

    private void UpdateSelectedValue(string variableValue) {
      sharedFixedVariables.SetVariableValue(variableValue, FreeVariable, 0);
      OnVariableValueChanged(this, EventArgs.Empty);
    }
  }
}

