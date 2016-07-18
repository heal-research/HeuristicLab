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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Target Response Gradients")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionTargetResponseGradientView : DataAnalysisSolutionEvaluationView {
    private readonly Dictionary<string, GradientChart> gradientCharts;
    private readonly Dictionary<string, DensityChart> densityCharts;
    private readonly Dictionary<string, Panel> groupingPanels;

    private const int Points = 200;
    private int MaxColumns = 4;

    private IEnumerable<string> VisibleVariables {
      get {
        foreach (ListViewItem item in variableListView.CheckedItems)
          yield return item.Text;
      }
    }
    private IEnumerable<GradientChart> VisibleGradientCharts {
      get { return VisibleVariables.Select(v => gradientCharts[v]); }
    }
    private IEnumerable<DensityChart> VisibleDensityCharts {
      get { return VisibleVariables.Select(v => densityCharts[v]); }
    }
    private IEnumerable<Panel> VisibleChartsPanels {
      get { return VisibleVariables.Select(v => groupingPanels[v]); }
    }

    public RegressionSolutionTargetResponseGradientView() {
      InitializeComponent();
      gradientCharts = new Dictionary<string, GradientChart>();
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

      // create dataset
      var allowedInputVariables = Content.ProblemData.AllowedInputVariables;
      var variableValues = allowedInputVariables.Select(x => new List<double> { problemData.Dataset.GetDoubleValues(x, problemData.TrainingIndices).Median() });
      var sharedFixedVariables = new ModifiableDataset(allowedInputVariables, variableValues);

      // create controls
      gradientCharts.Clear();
      densityCharts.Clear();
      groupingPanels.Clear();
      foreach (var variableName in allowedInputVariables) {
        var gradientChart = CreateGradientChart(variableName, sharedFixedVariables);
        gradientCharts.Add(variableName, gradientChart);

        var densityChart = new DensityChart() {
          Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
          Margin = Padding.Empty,
          Height = 12,
          Visible = false
        };
        densityCharts.Add(variableName, densityChart);

        gradientChart.ZoomChanged += (o, e) => {
          var gradient = (GradientChart)o;
          var density = densityCharts[gradient.FreeVariable];
          density.Visible = densityComboBox.SelectedIndex != 0 && !gradient.IsZoomed;
          if (density.Visible)
            UpdateDensityChart(density, gradient.FreeVariable);
        };

        var panel = new Panel() {
          Dock = DockStyle.Fill,
          Margin = Padding.Empty,
          BackColor = Color.White
        };

        panel.Controls.Add(densityChart);
        panel.Controls.Add(gradientChart);
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

      RecalculateAndRelayoutCharts();
    }

    private async void RecalculateAndRelayoutCharts() {
      foreach (var variable in VisibleVariables) {
        var gradientChart = gradientCharts[variable];
        await gradientChart.RecalculateAsync();
      }
      gradientChartTableLayout.SuspendLayout();
      SetupYAxis();
      ReOrderControls();
      SetStyles();
      gradientChartTableLayout.ResumeLayout();
      gradientChartTableLayout.Refresh();
      foreach (var variable in VisibleVariables) {
        var densityChart = densityCharts[variable];
        UpdateDensityChart(densityChart, variable);
      }
    }
    private GradientChart CreateGradientChart(string variableName, ModifiableDataset sharedFixedVariables) {
      var gradientChart = new GradientChart {
        Dock = DockStyle.Fill,
        Margin = Padding.Empty,
        ShowLegend = false,
        ShowCursor = true,
        ShowConfigButton = false,
        YAxisTicks = 5,
      };
      gradientChart.VariableValueChanged += async (o, e) => {
        var recalculations = VisibleGradientCharts.Except(new[] { (GradientChart)o }).Select(async chart => {
          await chart.RecalculateAsync(updateOnFinish: false, resetYAxis: false);
        }).ToList();
        await Task.WhenAll(recalculations);

        if (recalculations.All(t => t.IsCompleted))
          SetupYAxis();
      };
      gradientChart.Configure(new[] { Content }, sharedFixedVariables, variableName, Points);
      gradientChart.SolutionAdded += gradientChart_SolutionAdded;
      gradientChart.SolutionRemoved += gradientChart_SolutionRemoved;
      return gradientChart;
    }

    private void SetupYAxis() {
      double axisMin, axisMax;
      if (automaticYAxisCheckBox.Checked) {
        double min = double.MaxValue, max = double.MinValue;
        foreach (var chart in VisibleGradientCharts) {
          if (chart.YMin < min) min = chart.YMin;
          if (chart.YMax > max) max = chart.YMax;
        }

        double axisInterval;
        ChartUtil.CalculateAxisInterval(min, max, 5, out axisMin, out axisMax, out axisInterval);
      } else {
        axisMin = limitView.Content.Lower;
        axisMax = limitView.Content.Upper;
      }

      foreach (var chart in VisibleGradientCharts) {
        chart.FixedYAxisMin = axisMin;
        chart.FixedYAxisMax = axisMax;
      }
    }

    // reorder chart controls so that they always appear in the same order as in the list view
    // the table layout containing the controls should be suspended before calling this method
    private void ReOrderControls() {
      var tl = gradientChartTableLayout;
      tl.Controls.Clear();
      int row = 0, column = 0;
      foreach (var v in VisibleVariables) {
        var chartsPanel = groupingPanels[v];
        tl.Controls.Add(chartsPanel, column, row);

        var chart = gradientCharts[v];
        chart.YAxisTitle = column == 0 ? Content.ProblemData.TargetVariable : string.Empty;
        column++;

        if (column == MaxColumns) {
          row++;
          column = 0;
        }
      }
    }

    private void SetStyles() {
      var tl = gradientChartTableLayout;
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

    private async void gradientChart_SolutionAdded(object sender, EventArgs<IRegressionSolution> e) {
      var solution = e.Value;
      foreach (var chart in gradientCharts.Values) {
        if (sender == chart) continue;
        await chart.AddSolutionAsync(solution);
      }
    }

    private async void gradientChart_SolutionRemoved(object sender, EventArgs<IRegressionSolution> e) {
      var solution = e.Value;
      foreach (var chart in gradientCharts.Values) {
        if (sender == chart) continue;
        await chart.RemoveSolutionAsync(solution);
      }
    }

    private async void variableListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var item = e.Item;
      var variable = item.Text;
      var gradientChart = gradientCharts[variable];
      var chartsPanel = groupingPanels[variable];
      var tl = gradientChartTableLayout;

      tl.SuspendLayout();
      if (item.Checked) {
        tl.Controls.Add(chartsPanel);
        await gradientChart.RecalculateAsync();
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
      gradientChartTableLayout.Refresh();
      densityComboBox_SelectedIndexChanged(this, EventArgs.Empty); // necessary to realign the density plots
    }

    private void limit_ValueChanged(object sender, EventArgs e) {
      if (automaticYAxisCheckBox.Checked)
        return;
      SetupYAxis();
      gradientChartTableLayout.Refresh();
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
          if (!VisibleVariables.Contains(variableName) || gradientCharts[variableName].IsZoomed)
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
      var data = Content.ProblemData.Dataset.GetDoubleValues(variable, indices).ToList();
      var gradientChart = gradientCharts[variable];
      var min = gradientChart.FixedXAxisMin;
      var max = gradientChart.FixedXAxisMax;
      var buckets = gradientChart.DrawingSteps;
      if (min.HasValue && max.HasValue) {
        densityChart.UpdateChart(data, min.Value, max.Value, buckets);
        densityChart.Width = gradientChart.Width;

        var gcPlotPosition = gradientChart.InnerPlotPosition;
        densityChart.Left = (int)(gcPlotPosition.X / 100.0 * gradientChart.Width);
        densityChart.Width = (int)(gcPlotPosition.Width / 100.0 * gradientChart.Width);

        densityChart.Visible = true;
      }
    }

    private void columnsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      MaxColumns = (int)columnsNumericUpDown.Value;
      int columns = Math.Min(VisibleVariables.Count(), MaxColumns);
      if (columns > 0) {
        var tl = gradientChartTableLayout;
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
        var gradientChart = gradientCharts[variable];
        var densityChart = densityCharts[variable];
        // recalculate and refresh
        await gradientChart.RecalculateAsync();
        gradientChart.Refresh();
        UpdateDensityChart(densityChart, variable);
      }
    }
  }
}
