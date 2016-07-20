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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  public partial class GradientChart : UserControl {
    private ModifiableDataset sharedFixedVariables; // used for syncronising variable values between charts
    private ModifiableDataset internalDataset; // holds the x values for each point drawn

    private CancellationTokenSource cancelCurrentRecalculateSource;

    private readonly List<IRegressionSolution> solutions;
    private readonly Dictionary<IRegressionSolution, Series> seriesCache;
    private readonly Dictionary<IRegressionSolution, Series> ciSeriesCache;

    private readonly ToolStripMenuItem configToolStripMenuItem;
    private readonly GradientChartConfigurationDialog configurationDialog;

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

    public bool ShowConfigButton {
      get { return configurationButton.Visible; }
      set { configurationButton.Visible = value; }
    }

    private int xAxisTicks = 5;
    public int XAxisTicks {
      get { return xAxisTicks; }
      set {
        if (value != xAxisTicks) {
          xAxisTicks = value;
          SetupAxis(chart.ChartAreas[0].AxisX, trainingMin, trainingMax, XAxisTicks, FixedXAxisMin, FixedXAxisMax);
          RecalculateInternalDataset();
        }
      }
    }
    private double? fixedXAxisMin;
    public double? FixedXAxisMin {
      get { return fixedXAxisMin; }
      set {
        if ((value.HasValue && fixedXAxisMin.HasValue && !value.Value.IsAlmost(fixedXAxisMin.Value)) || (value.HasValue != fixedXAxisMin.HasValue)) {
          fixedXAxisMin = value;
          if (trainingMin < trainingMax) {
            SetupAxis(chart.ChartAreas[0].AxisX, trainingMin, trainingMax, XAxisTicks, FixedXAxisMin, FixedXAxisMax);
            RecalculateInternalDataset();
            // set the vertical line position 
            if (VerticalLineAnnotation.X <= fixedXAxisMin) {
              var axisX = chart.ChartAreas[0].AxisX;
              var step = (axisX.Maximum - axisX.Minimum) / drawingSteps;
              VerticalLineAnnotation.X = axisX.Minimum + step;
            }
          }
        }
      }
    }
    private double? fixedXAxisMax;
    public double? FixedXAxisMax {
      get { return fixedXAxisMax; }
      set {
        if ((value.HasValue && fixedXAxisMax.HasValue && !value.Value.IsAlmost(fixedXAxisMax.Value)) || (value.HasValue != fixedXAxisMax.HasValue)) {
          fixedXAxisMax = value;
          if (trainingMin < trainingMax) {
            SetupAxis(chart.ChartAreas[0].AxisX, trainingMin, trainingMax, XAxisTicks, FixedXAxisMin, FixedXAxisMax);
            RecalculateInternalDataset();
            // set the vertical line position 
            if (VerticalLineAnnotation.X >= fixedXAxisMax) {
              var axisX = chart.ChartAreas[0].AxisX;
              var step = (axisX.Maximum - axisX.Minimum) / drawingSteps;
              VerticalLineAnnotation.X = axisX.Maximum - step;
            }
          }
        }
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

    private double trainingMin = 1;
    private double trainingMax = -1;

    private int drawingSteps = 1000;
    public int DrawingSteps {
      get { return drawingSteps; }
      set {
        if (value != drawingSteps) {
          drawingSteps = value;
          RecalculateInternalDataset();
          ResizeAllSeriesData();
        }
      }
    }

    private string freeVariable;
    public string FreeVariable {
      get { return freeVariable; }
      set {
        if (value == freeVariable) return;
        if (solutions.Any(s => !s.ProblemData.Dataset.DoubleVariables.Contains(value))) {
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

    private VerticalLineAnnotation VerticalLineAnnotation {
      get { return (VerticalLineAnnotation)chart.Annotations.SingleOrDefault(x => x is VerticalLineAnnotation); }
    }

    internal ElementPosition InnerPlotPosition {
      get { return chart.ChartAreas[0].InnerPlotPosition; }
    }
    #endregion

    public GradientChart() {
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

      configToolStripMenuItem = new ToolStripMenuItem("Configuration");
      configToolStripMenuItem.Click += config_Click;
      chart.ContextMenuStrip.Items.Add(new ToolStripSeparator());
      chart.ContextMenuStrip.Items.Add(configToolStripMenuItem);
      configurationDialog = new GradientChartConfigurationDialog(this);

      Disposed += GradientChart_Disposed;
    }

    private void GradientChart_Disposed(object sender, EventArgs e) {
      if (cancelCurrentRecalculateSource != null)
        cancelCurrentRecalculateSource.Cancel();
    }

    public void Configure(IEnumerable<IRegressionSolution> solutions, ModifiableDataset sharedFixedVariables, string freeVariable, int drawingSteps, bool initializeAxisRanges = true) {
      if (!SolutionsCompatible(solutions))
        throw new ArgumentException("Solutions are not compatible with the problem data.");
      this.freeVariable = freeVariable;
      this.drawingSteps = drawingSteps;

      this.solutions.Clear();
      this.solutions.AddRange(solutions);

      // add an event such that whenever a value is changed in the shared dataset, 
      // this change is reflected in the internal dataset (where the value becomes a whole column)
      if (this.sharedFixedVariables != null)
        this.sharedFixedVariables.ItemChanged -= sharedFixedVariables_ItemChanged;
      this.sharedFixedVariables = sharedFixedVariables;
      this.sharedFixedVariables.ItemChanged += sharedFixedVariables_ItemChanged;

      RecalculateTrainingLimits(initializeAxisRanges);
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

      // Set cursor and x-axis
      // Make sure to allow a small offset to be able to distinguish the vertical line annotation from the axis
      var defaultValue = sharedFixedVariables.GetDoubleValue(freeVariable, 0);
      var step = (trainingMax - trainingMin) / drawingSteps;
      var minimum = chart.ChartAreas[0].AxisX.Minimum;
      var maximum = chart.ChartAreas[0].AxisX.Maximum;
      if (defaultValue <= minimum)
        VerticalLineAnnotation.X = minimum + step;
      else if (defaultValue >= maximum)
        VerticalLineAnnotation.X = maximum - step;
      else
        VerticalLineAnnotation.X = defaultValue;

      if (ShowCursor)
        chart.Titles[0].Text = FreeVariable + " : " + defaultValue.ToString("N3", CultureInfo.CurrentCulture);

      ResizeAllSeriesData();
      OrderAndColorSeries();
    }

    public async Task RecalculateAsync(bool updateOnFinish = true, bool resetYAxis = true) {
      if (IsDisposed
        || sharedFixedVariables == null || !solutions.Any() || string.IsNullOrEmpty(freeVariable)
        || trainingMin.IsAlmost(trainingMax) || trainingMin > trainingMax || drawingSteps == 0)
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

        UpdateOutOfTrainingRangeStripLines();

        calculationPendingTimer.Stop();
        calculationPendingLabel.Visible = false;
        if (updateOnFinish)
          Update();
      }
      catch (OperationCanceledException) { }
      catch (AggregateException ae) {
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
      double axisMin, axisMax, axisInterval;
      ChartUtil.CalculateAxisInterval(minValue, maxValue, ticks, out axisMin, out axisMax, out axisInterval);
      axis.Minimum = fixedAxisMin ?? axisMin;
      axis.Maximum = fixedAxisMax ?? axisMax;
      axis.Interval = (axis.Maximum - axis.Minimum) / ticks;

      try {
        chart.ChartAreas[0].RecalculateAxesScale();
      }
      catch (InvalidOperationException) {
        // Can occur if eg. axis min == axis max
      }
    }

    private void RecalculateTrainingLimits(bool initializeAxisRanges) {
      trainingMin = solutions.Select(s => s.ProblemData.Dataset.GetDoubleValues(freeVariable, s.ProblemData.TrainingIndices).Min()).Max();
      trainingMax = solutions.Select(s => s.ProblemData.Dataset.GetDoubleValues(freeVariable, s.ProblemData.TrainingIndices).Max()).Min();

      if (initializeAxisRanges) {
        double xmin, xmax, xinterval;
        ChartUtil.CalculateAxisInterval(trainingMin, trainingMax, XAxisTicks, out xmin, out xmax, out xinterval);
        FixedXAxisMin = xmin;
        FixedXAxisMax = xmax;
      }
    }

    private void RecalculateInternalDataset() {
      if (sharedFixedVariables == null)
        return;

      // we expand the range in order to get nice tick intervals on the x axis
      double xmin, xmax, xinterval;
      ChartUtil.CalculateAxisInterval(trainingMin, trainingMax, XAxisTicks, out xmin, out xmax, out xinterval);

      if (FixedXAxisMin.HasValue) xmin = FixedXAxisMin.Value;
      if (FixedXAxisMax.HasValue) xmax = FixedXAxisMax.Value;
      double step = (xmax - xmin) / drawingSteps;

      var xvalues = new List<double>();
      for (int i = 0; i < drawingSteps; i++)
        xvalues.Add(xmin + i * step);

      var variables = sharedFixedVariables.DoubleVariables.ToList();
      internalDataset = new ModifiableDataset(variables,
        variables.Select(x => x == FreeVariable
          ? xvalues
          : Enumerable.Repeat(sharedFixedVariables.GetDoubleValue(x, 0), xvalues.Count).ToList()
        )
      );
    }

    private Tuple<Series, Series> CreateSeries(IRegressionSolution solution) {
      var series = new Series {
        ChartType = SeriesChartType.Line,
        Name = solution.ProblemData.TargetVariable + " " + solutions.IndexOf(solution)
      };
      series.LegendText = series.Name;

      var confidenceBoundSolution = solution as IConfidenceRegressionSolution;
      Series confidenceIntervalSeries = null;
      if (confidenceBoundSolution != null) {
        confidenceIntervalSeries = new Series {
          ChartType = SeriesChartType.Range,
          YValuesPerPoint = 2,
          Name = "95% Conf. Interval " + series.Name,
          IsVisibleInLegend = false
        };
      }
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

      // Add confidence interval series before its coresponding series for correct z index
      foreach (var solution in solutions) {
        Series ciSeries;
        if (ciSeriesCache.TryGetValue(solution, out ciSeries)) {
          var series = seriesCache[solution];
          ciSeries.Color = Color.FromArgb(40, series.Color);
          int idx = chart.Series.IndexOf(seriesCache[solution]);
          chart.Series.Insert(idx, ciSeries);
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
        var xvalues = internalDataset.GetDoubleValues(FreeVariable).ToList();
        var yvalues = solution.Model.GetEstimatedValues(internalDataset, Enumerable.Range(0, internalDataset.Rows)).ToList();

        double min = double.MaxValue, max = double.MinValue;

        var series = seriesCache[solution];
        for (int i = 0; i < xvalues.Count; i++) {
          series.Points[i].SetValueXY(xvalues[i], yvalues[i]);
          if (yvalues[i] < min) min = yvalues[i];
          if (yvalues[i] > max) max = yvalues[i];
        }
        chart.Invalidate();

        cancellationToken.ThrowIfCancellationRequested();

        var confidenceBoundSolution = solution as IConfidenceRegressionSolution;
        if (confidenceBoundSolution != null) {
          var confidenceIntervalSeries = ciSeriesCache[solution];
          var variances = confidenceBoundSolution.Model.GetEstimatedVariances(internalDataset, Enumerable.Range(0, internalDataset.Rows)).ToList();
          for (int i = 0; i < xvalues.Count; i++) {
            var lower = yvalues[i] - 1.96 * Math.Sqrt(variances[i]);
            var upper = yvalues[i] + 1.96 * Math.Sqrt(variances[i]);
            confidenceIntervalSeries.Points[i].SetValueXY(xvalues[i], lower, upper);
            if (lower < min) min = lower;
            if (upper > max) max = upper;
          }
          chart.Invalidate();
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new DoubleLimit(min, max);
      }, cancellationToken);
    }

    private void ResizeAllSeriesData() {
      if (internalDataset == null)
        return;

      var xvalues = internalDataset.GetDoubleValues(FreeVariable).ToList();
      foreach (var solution in solutions)
        ResizeSeriesData(solution, xvalues);
    }
    private void ResizeSeriesData(IRegressionSolution solution, IList<double> xvalues = null) {
      if (xvalues == null)
        xvalues = internalDataset.GetDoubleValues(FreeVariable).ToList();

      var series = seriesCache[solution];
      series.Points.SuspendUpdates();
      series.Points.Clear();
      for (int i = 0; i < xvalues.Count; i++)
        series.Points.Add(new DataPoint(xvalues[i], 0.0));
      series.Points.ResumeUpdates();

      Series confidenceIntervalSeries;
      if (ciSeriesCache.TryGetValue(solution, out confidenceIntervalSeries)) {
        confidenceIntervalSeries.Points.SuspendUpdates();
        confidenceIntervalSeries.Points.Clear();
        for (int i = 0; i < xvalues.Count; i++)
          confidenceIntervalSeries.Points.Add(new DataPoint(xvalues[i], new[] { -1.0, 1.0 }));
        confidenceIntervalSeries.Points.ResumeUpdates();
      }
    }

    public async Task AddSolutionAsync(IRegressionSolution solution) {
      if (!SolutionsCompatible(solutions.Concat(new[] { solution })))
        throw new ArgumentException("The solution is not compatible with the problem data.");
      if (solutions.Contains(solution))
        return;

      solutions.Add(solution);
      RecalculateTrainingLimits(true);

      var series = CreateSeries(solution);
      seriesCache.Add(solution, series.Item1);
      if (series.Item2 != null)
        ciSeriesCache.Add(solution, series.Item2);

      ResizeSeriesData(solution);
      OrderAndColorSeries();

      await RecalculateAsync();
      var args = new EventArgs<IRegressionSolution>(solution);
      OnSolutionAdded(this, args);
    }

    public async Task RemoveSolutionAsync(IRegressionSolution solution) {
      if (!solutions.Remove(solution))
        return;

      RecalculateTrainingLimits(true);

      seriesCache.Remove(solution);
      ciSeriesCache.Remove(solution);

      await RecalculateAsync();
      var args = new EventArgs<IRegressionSolution>(solution);
      OnSolutionRemoved(this, args);
    }

    private static bool SolutionsCompatible(IEnumerable<IRegressionSolution> solutions) {
      foreach (var solution1 in solutions) {
        var variables1 = solution1.ProblemData.Dataset.DoubleVariables;
        foreach (var solution2 in solutions) {
          if (solution1 == solution2)
            continue;
          var variables2 = solution2.ProblemData.Dataset.DoubleVariables;
          if (!variables1.All(variables2.Contains))
            return false;
        }
      }
      return true;
    }

    private void UpdateOutOfTrainingRangeStripLines() {
      var axisX = chart.ChartAreas[0].AxisX;
      var lowerStripLine = axisX.StripLines[0];
      var upperStripLine = axisX.StripLines[1];

      lowerStripLine.IntervalOffset = axisX.Minimum;
      lowerStripLine.StripWidth = Math.Abs(trainingMin - axisX.Minimum);

      upperStripLine.IntervalOffset = trainingMax;
      upperStripLine.StripWidth = Math.Abs(axisX.Maximum - trainingMax);
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
      var variables = sharedFixedVariables.DoubleVariables.ToList();
      var rowIndex = e.Value;
      var columnIndex = e.Value2;

      var variableName = variables[columnIndex];
      if (variableName == FreeVariable) return;
      var v = sharedFixedVariables.GetDoubleValue(variableName, rowIndex);
      var values = new List<double>(Enumerable.Repeat(v, DrawingSteps));
      internalDataset.ReplaceVariable(variableName, values);
    }

    private void chart_AnnotationPositionChanging(object sender, AnnotationPositionChangingEventArgs e) {
      var step = (trainingMax - trainingMin) / drawingSteps;
      double newLocation = step * (long)Math.Round(e.NewLocationX / step);
      var axisX = chart.ChartAreas[0].AxisX;
      if (newLocation >= axisX.Maximum)
        newLocation = axisX.Maximum - step;
      if (newLocation <= axisX.Minimum)
        newLocation = axisX.Minimum + step;

      e.NewLocationX = newLocation;

      UpdateCursor();
    }
    private void chart_AnnotationPositionChanged(object sender, EventArgs e) {
      UpdateCursor();
    }
    void UpdateCursor() {
      var x = VerticalLineAnnotation.X;
      sharedFixedVariables.SetVariableValue(x, FreeVariable, 0);

      if (ShowCursor) {
        chart.Titles[0].Text = FreeVariable + " : " + x.ToString("N3", CultureInfo.CurrentCulture);
        chart.Update();
      }

      OnVariableValueChanged(this, EventArgs.Empty);
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      bool hitCursor = chart.HitTest(e.X, e.Y).ChartElementType == ChartElementType.Annotation;
      chart.Cursor = hitCursor ? Cursors.VSplit : Cursors.Default;
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

    private void config_Click(object sender, EventArgs e) {
      configurationDialog.ShowDialog(this);
    }

    private void chart_SelectionRangeChanged(object sender, CursorEventArgs e) {
      OnZoomChanged(this, EventArgs.Empty);
    }

    private void chart_Resize(object sender, EventArgs e) {
      UpdateTitlePosition();
    }
    #endregion
  }
}

