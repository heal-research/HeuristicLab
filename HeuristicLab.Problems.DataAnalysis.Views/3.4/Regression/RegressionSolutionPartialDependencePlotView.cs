#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Partial Dependence Plots")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionPartialDependencePlotView : DataAnalysisSolutionEvaluationView {
    private readonly Dictionary<string, IPartialDependencePlot> partialDependencePlots;
    private readonly Dictionary<string, DensityChart> densityCharts;
    private readonly Dictionary<string, Panel> groupingPanels;
    private ModifiableDataset sharedFixedVariables;

    private const int Points = 200;
    private int MaxColumns = 4;

    private IEnumerable<string> VisibleVariables {
      get {
        foreach (ListViewItem item in variableListView.CheckedItems)
          yield return item.Text;
      }
    }
    private IEnumerable<IPartialDependencePlot> VisiblePartialDependencePlots {
      get { return VisibleVariables.Select(v => partialDependencePlots[v]); }
    }
    private IEnumerable<DensityChart> VisibleDensityCharts {
      get { return VisibleVariables.Select(v => densityCharts[v]); }
    }
    private IEnumerable<Panel> VisibleChartsPanels {
      get { return VisibleVariables.Select(v => groupingPanels[v]); }
    }

    public RegressionSolutionPartialDependencePlotView() {
      InitializeComponent();
      partialDependencePlots = new Dictionary<string, IPartialDependencePlot>();
      densityCharts = new Dictionary<string, DensityChart>();
      groupingPanels = new Dictionary<string, Panel>();

      limitView.Content = new DoubleLimit(0, 1);
      limitView.Content.ValueChanged += limit_ValueChanged;

      densityComboBox.SelectedIndex = 1; // select Training

      // Avoid additional horizontal scrollbar
      var vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
      scrollPanel.Padding = new Padding(0, 0, vertScrollWidth, 0);
      scrollPanel.AutoScroll = true;
    }

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += solution_ModelChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.ModelChanged -= solution_ModelChanged;
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;
      var problemData = Content.ProblemData;

      // Init Y-axis range
      double min = double.MaxValue, max = double.MinValue;
      var trainingTarget = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, problemData.TrainingIndices);
      foreach (var t in trainingTarget) {
        if (t < min) min = t;
        if (t > max) max = t;
      }
      double range = max - min;
      const double scale = 1.0 / 3.0;
      double axisMin, axisMax, axisInterval;
      ChartUtil.CalculateAxisInterval(min - scale * range, max + scale * range, 5, out axisMin, out axisMax, out axisInterval);
      automaticYAxisCheckBox.Checked = false;
      limitView.ReadOnly = false;
      limitView.Content.Lower = axisMin;
      limitView.Content.Upper = axisMax;

      // create dataset of problemData input variables and model input variables
      // necessary workaround to have the variables in the occuring order
      var inputvariables =
        new HashSet<string>(Content.ProblemData.AllowedInputVariables.Union(Content.Model.VariablesUsedForPrediction));
      var allowedInputVariables =
        Content.ProblemData.Dataset.VariableNames.Where(v => inputvariables.Contains(v)).ToList();


      var doubleVariables = allowedInputVariables.Where(problemData.Dataset.VariableHasType<double>);
      var doubleVariableValues = (IEnumerable<IList>)doubleVariables.Select(x => new List<double> { problemData.Dataset.GetDoubleValues(x, problemData.TrainingIndices).Median() });

      var factorVariables = allowedInputVariables.Where(problemData.Dataset.VariableHasType<string>);
      var factorVariableValues = (IEnumerable<IList>)factorVariables.Select(x => new List<string> {
        problemData.Dataset.GetStringValues(x, problemData.TrainingIndices)
        .GroupBy(val => val).OrderByDescending(g => g.Count()).First().Key // most frequent value
      });

      if (sharedFixedVariables != null)
        sharedFixedVariables.ItemChanged -= SharedFixedVariables_ItemChanged;

      sharedFixedVariables = new ModifiableDataset(doubleVariables.Concat(factorVariables), doubleVariableValues.Concat(factorVariableValues));


      // create controls
      partialDependencePlots.Clear();
      densityCharts.Clear();
      groupingPanels.Clear();
      foreach (var variableName in doubleVariables) {
        var plot = CreatePartialDependencePlot(variableName, sharedFixedVariables);
        partialDependencePlots.Add(variableName, plot);

        var densityChart = new DensityChart() {
          Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
          Margin = Padding.Empty,
          Height = 12,
          Visible = false,
          Top = (int)(plot.Height * 0.1),
        };
        densityCharts.Add(variableName, densityChart);

        plot.ZoomChanged += (o, e) => {
          var pdp = (PartialDependencePlot)o;
          var density = densityCharts[pdp.FreeVariable];
          density.Visible = densityComboBox.SelectedIndex != 0 && !pdp.IsZoomed;
          if (density.Visible)
            UpdateDensityChart(density, pdp.FreeVariable);
        };
        plot.SizeChanged += (o, e) => {
          var pdp = (PartialDependencePlot)o;
          var density = densityCharts[pdp.FreeVariable];
          density.Top = (int)(pdp.Height * 0.1);
        };

        // Initially, the inner plot areas are not initialized for hidden charts (scollpanel, ...)
        // This event handler listens for the paint event once (where everything is already initialized) to do some manual layouting.
        plot.ChartPostPaint += OnPartialDependencePlotPostPaint;

        var panel = new Panel() {
          Dock = DockStyle.Fill,
          Margin = Padding.Empty,
          BackColor = Color.White
        };

        panel.Controls.Add(densityChart);
        panel.Controls.Add(plot);
        groupingPanels.Add(variableName, panel);
      }
      foreach (var variableName in factorVariables) {
        var plot = CreateFactorPartialDependencePlot(variableName, sharedFixedVariables);
        partialDependencePlots.Add(variableName, plot);

        var densityChart = new DensityChart() {
          Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
          Margin = Padding.Empty,
          Height = 12,
          Visible = false,
          Top = (int)(plot.Height * 0.1),
        };
        densityCharts.Add(variableName, densityChart);
        plot.ZoomChanged += (o, e) => {
          var pdp = (FactorPartialDependencePlot)o;
          var density = densityCharts[pdp.FreeVariable];
          density.Visible = densityComboBox.SelectedIndex != 0 && !pdp.IsZoomed;
          if (density.Visible)
            UpdateDensityChart(density, pdp.FreeVariable);
        };
        plot.SizeChanged += (o, e) => {
          var pdp = (FactorPartialDependencePlot)o;
          var density = densityCharts[pdp.FreeVariable];
          density.Top = (int)(pdp.Height * 0.1);
        };

        // Initially, the inner plot areas are not initialized for hidden charts (scollpanel, ...)
        // This event handler listens for the paint event once (where everything is already initialized) to do some manual layouting.
        plot.ChartPostPaint += OnFactorPartialDependencePlotPostPaint;

        var panel = new Panel() {
          Dock = DockStyle.Fill,
          Margin = Padding.Empty,
          BackColor = Color.White
        };

        panel.Controls.Add(densityChart);
        panel.Controls.Add(plot);
        groupingPanels.Add(variableName, panel);
      }
      // update variable list
      variableListView.ItemChecked -= variableListView_ItemChecked;
      variableListView.Items.Clear();
      foreach (var variable in allowedInputVariables)
        variableListView.Items.Add(key: variable, text: variable, imageIndex: 0);

      foreach (var variable in Content.Model.VariablesUsedForPrediction)
        variableListView.Items[variable].Checked = true;
      variableListView.ItemChecked += variableListView_ItemChecked;

      sharedFixedVariables.ItemChanged += SharedFixedVariables_ItemChanged;

      rowNrNumericUpDown.Maximum = Content.ProblemData.Dataset.Rows - 1;

      RecalculateAndRelayoutCharts();
    }

    private void SharedFixedVariables_ItemChanged(object sender, EventArgs<int, int> e) {
      double yValue = Content.Model.GetEstimatedValues(sharedFixedVariables, new[] { 0 }).Single();
      string title = Content.ProblemData.TargetVariable + ": " + yValue.ToString("G5", CultureInfo.CurrentCulture);
      foreach (var chart in partialDependencePlots.Values) {
        if (!string.IsNullOrEmpty(chart.YAxisTitle)) { // only show title for first column in grid
          chart.YAxisTitle = title;
        }
      }
    }


    private void OnPartialDependencePlotPostPaint(object o, EventArgs e) {
      var plot = (PartialDependencePlot)o;
      var density = densityCharts[plot.FreeVariable];

      density.Width = plot.Width;

      var gcPlotPosition = plot.InnerPlotPosition;
      density.Left = (int)(gcPlotPosition.X / 100.0 * plot.Width);
      density.Width = (int)(gcPlotPosition.Width / 100.0 * plot.Width);
      plot.UpdateTitlePosition();

      // removed after succesful layouting due to performance reasons
      if (gcPlotPosition.Width != 0)
        plot.ChartPostPaint -= OnPartialDependencePlotPostPaint;
    }

    private void OnFactorPartialDependencePlotPostPaint(object o, EventArgs e) {
      var plot = (FactorPartialDependencePlot)o;
      var density = densityCharts[plot.FreeVariable];

      density.Width = plot.Width;

      var gcPlotPosition = plot.InnerPlotPosition;
      density.Left = (int)(gcPlotPosition.X / 100.0 * plot.Width);
      density.Width = (int)(gcPlotPosition.Width / 100.0 * plot.Width);
      plot.UpdateTitlePosition();

      // removed after succesful layouting due to performance reasons
      if (gcPlotPosition.Width != 0)
        plot.ChartPostPaint -= OnFactorPartialDependencePlotPostPaint;
    }

    private async void RecalculateAndRelayoutCharts() {
      foreach (var variable in VisibleVariables) {
        var plot = partialDependencePlots[variable];
        await plot.RecalculateAsync(false, false);
      }
      partialDependencePlotTableLayout.SuspendLayout();
      SetupYAxis();
      ReOrderControls();
      SetStyles();
      partialDependencePlotTableLayout.ResumeLayout();
      partialDependencePlotTableLayout.Refresh();
      foreach (var variable in VisibleVariables) {
        DensityChart densityChart;
        if (densityCharts.TryGetValue(variable, out densityChart)) {
          UpdateDensityChart(densityChart, variable);
        }
      }
    }
    private PartialDependencePlot CreatePartialDependencePlot(string variableName, ModifiableDataset sharedFixedVariables) {
      var plot = new PartialDependencePlot {
        Dock = DockStyle.Fill,
        Margin = Padding.Empty,
        ShowLegend = false,
        ShowCursor = true,
        ShowConfigButton = false,
        YAxisTicks = 5,
      };
      plot.VariableValueChanged += async (o, e) => {
        var recalculations = VisiblePartialDependencePlots
          .Except(new[] { (IPartialDependencePlot)o })
          .Select(async chart => {
            await chart.RecalculateAsync(updateOnFinish: false, resetYAxis: false);
          }).ToList();
        await Task.WhenAll(recalculations);

        if (recalculations.All(t => t.IsCompleted))
          SetupYAxis();
      };
      plot.Configure(new[] { Content }, sharedFixedVariables, variableName, Points);
      plot.SolutionAdded += partialDependencePlot_SolutionAdded;
      plot.SolutionRemoved += partialDependencePlot_SolutionRemoved;
      return plot;
    }
    private FactorPartialDependencePlot CreateFactorPartialDependencePlot(string variableName, ModifiableDataset sharedFixedVariables) {
      var plot = new FactorPartialDependencePlot {
        Dock = DockStyle.Fill,
        Margin = Padding.Empty,
        ShowLegend = false,
        ShowCursor = true,
        YAxisTicks = 5,
      };
      plot.VariableValueChanged += async (o, e) => {
        var recalculations = VisiblePartialDependencePlots
          .Except(new[] { (FactorPartialDependencePlot)o })
          .Select(async chart => {
            await chart.RecalculateAsync(updateOnFinish: false, resetYAxis: false);
          }).ToList();
        await Task.WhenAll(recalculations);

        if (recalculations.All(t => t.IsCompleted))
          SetupYAxis();
      };
      var variableValues = Content.ProblemData.Dataset.GetStringValues(variableName).Distinct().OrderBy(n => n).ToList();
      plot.Configure(new[] { Content }, sharedFixedVariables, variableName, variableValues);
      plot.SolutionAdded += partialDependencePlot_SolutionAdded;
      plot.SolutionRemoved += partialDependencePlot_SolutionRemoved;
      return plot;
    }
    private void SetupYAxis() {
      double axisMin, axisMax;
      if (automaticYAxisCheckBox.Checked) {
        double min = double.MaxValue, max = double.MinValue;
        foreach (var chart in VisiblePartialDependencePlots) {
          if (chart.YMin < min) min = chart.YMin;
          if (chart.YMax > max) max = chart.YMax;
        }

        double axisInterval;
        ChartUtil.CalculateAxisInterval(min, max, 5, out axisMin, out axisMax, out axisInterval);
      } else {
        axisMin = limitView.Content.Lower;
        axisMax = limitView.Content.Upper;
      }

      foreach (var chart in VisiblePartialDependencePlots) {
        chart.FixedYAxisMin = axisMin;
        chart.FixedYAxisMax = axisMax;
      }
    }

    // reorder chart controls so that they always appear in the same order as in the list view
    // the table layout containing the controls should be suspended before calling this method
    private void ReOrderControls() {
      var tl = partialDependencePlotTableLayout;
      tl.Controls.Clear();
      int row = 0, column = 0;
      double yValue = Content.Model.GetEstimatedValues(sharedFixedVariables, new[] { 0 }).Single();
      string title = Content.ProblemData.TargetVariable + ": " + yValue.ToString("G5", CultureInfo.CurrentCulture);

      foreach (var v in VisibleVariables) {
        var chartsPanel = groupingPanels[v];
        tl.Controls.Add(chartsPanel, column, row);

        var chart = partialDependencePlots[v];
        chart.YAxisTitle = column == 0 ? title : string.Empty;
        column++;

        if (column == MaxColumns) {
          row++;
          column = 0;
        }
      }
    }

    private void SetStyles() {
      var tl = partialDependencePlotTableLayout;
      tl.RowStyles.Clear();
      tl.ColumnStyles.Clear();
      int numVariables = VisibleVariables.Count();
      if (numVariables == 0)
        return;

      // set column styles
      tl.ColumnCount = Math.Min(numVariables, MaxColumns);
      for (int c = 0; c < tl.ColumnCount; c++)
        tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f / tl.ColumnCount));

      // set row styles
      tl.RowCount = (int)Math.Ceiling((double)numVariables / tl.ColumnCount);
      var columnWidth = tl.Width / tl.ColumnCount; // assume all columns have the same width
      var rowHeight = (int)(0.8 * columnWidth);
      for (int r = 0; r < tl.RowCount; r++)
        tl.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
    }

    private async void partialDependencePlot_SolutionAdded(object sender, EventArgs<IRegressionSolution> e) {
      var solution = e.Value;
      foreach (var chart in partialDependencePlots.Values) {
        if (sender == chart) continue;
        await chart.AddSolutionAsync(solution);
      }
    }

    private async void partialDependencePlot_SolutionRemoved(object sender, EventArgs<IRegressionSolution> e) {
      var solution = e.Value;
      foreach (var chart in partialDependencePlots.Values) {
        if (sender == chart) continue;
        await chart.RemoveSolutionAsync(solution);
      }
    }

    private async void variableListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var item = e.Item;
      var variable = item.Text;
      var plot = partialDependencePlots[variable];
      var chartsPanel = groupingPanels[variable];
      var tl = partialDependencePlotTableLayout;

      tl.SuspendLayout();
      if (item.Checked) {
        tl.Controls.Add(chartsPanel);
        await plot.RecalculateAsync(false, false);
      } else {
        tl.Controls.Remove(chartsPanel);
      }

      if (tl.Controls.Count > 0) {
        SetupYAxis();
        ReOrderControls();
        SetStyles();
      }
      tl.ResumeLayout();
      tl.Refresh();
      densityComboBox_SelectedIndexChanged(this, EventArgs.Empty);
    }

    private void automaticYAxisCheckBox_CheckedChanged(object sender, EventArgs e) {
      limitView.ReadOnly = automaticYAxisCheckBox.Checked;
      SetupYAxis();
      partialDependencePlotTableLayout.Refresh();
      densityComboBox_SelectedIndexChanged(this, EventArgs.Empty); // necessary to realign the density plots
    }

    private void limit_ValueChanged(object sender, EventArgs e) {
      if (automaticYAxisCheckBox.Checked)
        return;
      SetupYAxis();
      partialDependencePlotTableLayout.Refresh();
      densityComboBox_SelectedIndexChanged(this, EventArgs.Empty); // necessary to realign the density plots
    }

    private void densityComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content == null)
        return;

      int si = densityComboBox.SelectedIndex;
      if (si == 0) {
        foreach (var densityChart in densityCharts.Values)
          densityChart.Visible = false;
      } else {
        var indices = GetDensityIndices(si).ToList();

        foreach (var entry in densityCharts) {
          var variableName = entry.Key;
          var densityChart = entry.Value;
          if (!VisibleVariables.Contains(variableName) || partialDependencePlots[variableName].IsZoomed)
            continue;

          UpdateDensityChart(densityChart, variableName, indices);
        }
      }
    }
    private IEnumerable<int> GetDensityIndices(int selectedIndex) {
      var problemData = Content.ProblemData;
      return
        selectedIndex == 1 ? problemData.TrainingIndices :
        selectedIndex == 2 ? problemData.TestIndices :
        problemData.AllIndices;
    }
    private void UpdateDensityChart(DensityChart densityChart, string variable, IList<int> indices = null) {
      if (densityComboBox.SelectedIndex == 0)
        return;
      if (indices == null) {
        indices = GetDensityIndices(densityComboBox.SelectedIndex).ToList();
      }
      if (Content.ProblemData.Dataset.VariableHasType<double>(variable)) {
        var data = Content.ProblemData.Dataset.GetDoubleValues(variable, indices).ToList();
        var plot = partialDependencePlots[variable] as PartialDependencePlot;
        if (plot != null) {
          var min = plot.FixedXAxisMin;
          var max = plot.FixedXAxisMax;
          var buckets = plot.DrawingSteps;
          if (min.HasValue && max.HasValue) {
            densityChart.UpdateChart(data, min.Value, max.Value, buckets);
            densityChart.Width = plot.Width;

            var gcPlotPosition = plot.InnerPlotPosition;
            densityChart.Left = (int)(gcPlotPosition.X / 100.0 * plot.Width);
            densityChart.Width = (int)(gcPlotPosition.Width / 100.0 * plot.Width);

            densityChart.Visible = true;
          }
          plot.UpdateTitlePosition();
        }
      } else if (Content.ProblemData.Dataset.VariableHasType<string>(variable)) {
        var data = Content.ProblemData.Dataset.GetStringValues(variable).ToList();
        var plot = partialDependencePlots[variable] as FactorPartialDependencePlot;
        if (plot != null) {
          densityChart.UpdateChart(data);
          densityChart.Width = plot.Width;

          var gcPlotPosition = plot.InnerPlotPosition;
          densityChart.Left = (int)(gcPlotPosition.X / 100.0 * plot.Width);
          densityChart.Width = (int)(gcPlotPosition.Width / 100.0 * plot.Width);

          densityChart.Visible = true;

          plot.UpdateTitlePosition();
        }
      }
    }

    private void columnsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      MaxColumns = (int)columnsNumericUpDown.Value;
      int columns = Math.Min(VisibleVariables.Count(), MaxColumns);
      if (columns > 0) {
        var tl = partialDependencePlotTableLayout;
        MaxColumns = columns;
        tl.SuspendLayout();
        ReOrderControls();
        SetStyles();
        tl.ResumeLayout();
        tl.Refresh();
        densityComboBox_SelectedIndexChanged(this, EventArgs.Empty);
      }
    }

    private async void solution_ModelChanged(object sender, EventArgs e) {
      foreach (var variable in VisibleVariables) {
        var pdp = partialDependencePlots[variable];
        var densityChart = densityCharts[variable];
        // recalculate and refresh
        await pdp.RecalculateAsync(false, false);
        pdp.Refresh();
        UpdateDensityChart(densityChart, variable);
      }
    }

    private void rowNrNumericUpDown_ValueChanged(object sender, EventArgs e) {
      int rowNumber = (int)rowNrNumericUpDown.Value;
      var dataset = Content.ProblemData.Dataset;

      object[] newRow = sharedFixedVariables.VariableNames
        .Select<string, object>(variableName => {
          if (dataset.DoubleVariables.Contains(variableName)) {
            return dataset.GetDoubleValue(variableName, rowNumber);
          } else if (dataset.StringVariables.Contains(variableName)) {
            return dataset.GetStringValue(variableName, rowNumber);
          } else {
            throw new NotSupportedException("Only double and string(factor) columns are currently supported.");
          }
        }).ToArray();

      sharedFixedVariables.ReplaceRow(0, newRow);
    }
  }
}
