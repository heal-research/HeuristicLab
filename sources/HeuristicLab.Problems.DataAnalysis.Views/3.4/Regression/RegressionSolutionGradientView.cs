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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Gradient View")]
  [Content(typeof(IRegressionSolution))]
  public partial class RegressionSolutionGradientView : ItemView {
    private const int DrawingSteps = 1000;

    private readonly List<string> variableNames;
    private readonly ObservableList<DensityTrackbar> trackbars;
    private ModifiableDataset sharedFixedVariables;

    private int ActiveDimension {
      get { return trackbars.FindIndex(tb => tb.Checked); }
    }

    public new IRegressionSolution Content {
      get { return (IRegressionSolution)base.Content; }
      set { base.Content = value; }
    }

    public RegressionSolutionGradientView()
      : base() {
      variableNames = new List<string>();

      trackbars = new ObservableList<DensityTrackbar>();
      trackbars.ItemsAdded += (sender, args) => {
        args.Items.Select(i => i.Value).ForEach(RegisterEvents);
      };
      trackbars.ItemsRemoved += (sender, args) => {
        args.Items.Select(i => i.Value).ForEach(DeregisterEvents);
      };
      trackbars.CollectionReset += (sender, args) => {
        args.OldItems.Select(i => i.Value).ForEach(DeregisterEvents);
        args.Items.Select(i => i.Value).ForEach(RegisterEvents);
      };

      InitializeComponent();

      // Avoid additional horizontal scrollbar
      var vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
      tableLayoutPanel.Padding = new Padding(0, 0, vertScrollWidth, 0);
    }

    private async void UpdateConfigurationControls() {
      variableNames.Clear();
      trackbars.Clear();

      tableLayoutPanel.SuspendRepaint();
      tableLayoutPanel.SuspendLayout();

      tableLayoutPanel.RowCount = 0;
      tableLayoutPanel.Controls.Clear();

      if (Content == null) {
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.ResumeRepaint(false);
        return;
      }

      variableNames.AddRange(Content.ProblemData.AllowedInputVariables);

      var newTrackbars = CreateConfiguration();

      sharedFixedVariables = new ModifiableDataset(variableNames, newTrackbars.Select(tb => new List<double>(1) { (double)tb.Value }));
      gradientChart.Configure(new[] { Content }, sharedFixedVariables, variableNames.First(), DrawingSteps);
      await gradientChart.RecalculateAsync();

      // Add to table and observable lists
      tableLayoutPanel.RowCount = variableNames.Count;
      while (tableLayoutPanel.RowStyles.Count < variableNames.Count)
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
      for (int i = 0; i < newTrackbars.Count; i++) {
        // events registered automatically
        trackbars.Add(newTrackbars[i]);
        tableLayoutPanel.Controls.Add(newTrackbars[i], 0, i);
      }

      tableLayoutPanel.ResumeLayout(true);
      tableLayoutPanel.ResumeRepaint(true);

      // Init Y-axis range
      var problemData = Content.ProblemData;
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
      gradientChart.FixedYAxisMin = axisMin;
      gradientChart.FixedYAxisMax = axisMax;

      trackbars.First().Checked = true;
    }

    private IList<DensityTrackbar> CreateConfiguration() {
      var ranges = new List<DoubleLimit>();
      foreach (string variableName in variableNames) {
        var values = Content.ProblemData.Dataset.GetDoubleValues(variableName, Content.ProblemData.AllIndices);
        double min, max, interval;
        ChartUtil.CalculateAxisInterval(values.Min(), values.Max(), 10, out min, out max, out interval);
        ranges.Add(new DoubleLimit(min, max));
      }

      var newTrackbars = new List<DensityTrackbar>();
      for (int i = 0; i < variableNames.Count; i++) {
        var name = variableNames[i];
        var trainingData = Content.ProblemData.Dataset.GetDoubleValues(name, Content.ProblemData.TrainingIndices).ToList();

        var dimensionTrackbar = new DensityTrackbar(name, ranges[i], trainingData) {
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        newTrackbars.Add(dimensionTrackbar);
      }

      return newTrackbars;
    }

    private void RegisterEvents(DensityTrackbar trackbar) {
      trackbar.CheckedChanged += trackbar_CheckedChanged;
      trackbar.ValueChanged += trackbar_ValueChanged;
      trackbar.LimitsChanged += trackbar_LimitsChanged;
    }
    private void DeregisterEvents(DensityTrackbar trackbar) {
      trackbar.CheckedChanged -= trackbar_CheckedChanged;
      trackbar.ValueChanged -= trackbar_ValueChanged;
      trackbar.LimitsChanged -= trackbar_LimitsChanged;
    }

    private async void trackbar_CheckedChanged(object sender, EventArgs e) {
      var trackBar = sender as DensityTrackbar;
      if (trackBar == null || !trackBar.Checked) return;
      // Uncheck all others
      foreach (var tb in trackbars.Except(new[] { trackBar }))
        tb.Checked = false;
      gradientChart.FreeVariable = variableNames[trackbars.IndexOf(trackBar)];
      await gradientChart.RecalculateAsync();
    }

    private async void trackbar_LimitsChanged(object sender, EventArgs e) {
      var trackBar = sender as DensityTrackbar;
      if (trackBar == null || !trackBar.Checked) return;
      gradientChart.FixedXAxisMin = trackBar.Limits.Lower;
      gradientChart.FixedXAxisMax = trackBar.Limits.Upper;
      await gradientChart.RecalculateAsync();
    }

    private async void trackbar_ValueChanged(object sender, EventArgs e) {
      var trackBar = sender as DensityTrackbar;
      if (trackBar == null) return;
      sharedFixedVariables.SetVariableValue((double)trackBar.Value, variableNames[trackbars.IndexOf(trackBar)], 0);
      await gradientChart.RecalculateAsync();
    }

    #region Events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += Content_ModelChanged;
      Content.ProblemDataChanged += Content_ProblemDataChanged;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= Content_ModelChanged;
      Content.ProblemDataChanged -= Content_ProblemDataChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateConfigurationControls();
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      UpdateConfigurationControls();
    }

    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      UpdateConfigurationControls();
    }
    #endregion
  }

  internal static class Extensions {
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
      foreach (T item in source)
        action(item);
    }
  }
}
