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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("Run-length Distribution View")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionRLDView : ItemView {
    private List<Series> invisibleTargetSeries;

    private const string AllInstances = "All Instances";

    private static readonly Color[] colors = new[] {
      Color.FromArgb(0x40, 0x6A, 0xB7),
      Color.FromArgb(0xB1, 0x6D, 0x01),
      Color.FromArgb(0x4E, 0x8A, 0x06),
      Color.FromArgb(0x75, 0x50, 0x7B),
      Color.FromArgb(0x72, 0x9F, 0xCF),
      Color.FromArgb(0xA4, 0x00, 0x00),
      Color.FromArgb(0xAD, 0x7F, 0xA8),
      Color.FromArgb(0x29, 0x50, 0xCF),
      Color.FromArgb(0x90, 0xB0, 0x60),
      Color.FromArgb(0xF5, 0x89, 0x30),
      Color.FromArgb(0x55, 0x57, 0x53),
      Color.FromArgb(0xEF, 0x59, 0x59),
      Color.FromArgb(0xED, 0xD4, 0x30),
      Color.FromArgb(0x63, 0xC2, 0x16),
    };
    private static readonly ChartDashStyle[] lineStyles = new[] {
      ChartDashStyle.Solid,
      ChartDashStyle.Dash,
      ChartDashStyle.DashDot,
      ChartDashStyle.Dot
    };
    private static readonly DataRowVisualProperties.DataRowLineStyle[] hlLineStyles = new[] {
      DataRowVisualProperties.DataRowLineStyle.Solid,
      DataRowVisualProperties.DataRowLineStyle.Dash,
      DataRowVisualProperties.DataRowLineStyle.DashDot,
      DataRowVisualProperties.DataRowLineStyle.Dot
    };

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    private List<AlgorithmInstance> groups;

    private double[] targets;
    private double[] budgets;
    private bool targetsAreRelative = true;
    private bool showLabelsInTargetChart = true;
    private readonly BindingList<ProblemInstance> problems;

    private bool updateInProgress;
    private bool suppressContentEvents;
    private readonly IndexedDataTable<double> byCostDataTable;
    public IndexedDataTable<double> ByCostDataTable {
      get { return byCostDataTable; }
    }

    public RunCollectionRLDView() {
      InitializeComponent();
      invisibleTargetSeries = new List<Series>();

      try {
        updateInProgress = true;
        targetChart.CustomizeAllChartAreas();
        targetChart.ChartAreas[0].CursorX.Interval = 1;
        targetChart.SuppressExceptions = true;
        byCostDataTable = new IndexedDataTable<double>("ECDF by Cost", "A data table containing the ECDF of function values (relative to best-known).") {
          VisualProperties = {
            YAxisTitle = "Proportion of runs",
            YAxisMinimumFixedValue = 0,
            YAxisMinimumAuto = false,
            YAxisMaximumFixedValue = 1,
            YAxisMaximumAuto = false
          }
        };
        byCostViewHost.Content = byCostDataTable;

        relativeOrAbsoluteComboBox.SelectedItem = targetsAreRelative ? "relative" : "absolute";
        problems = new BindingList<ProblemInstance>();
        problemComboBox.DataSource = new BindingSource() { DataSource = problems };
        problemComboBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
      } finally { updateInProgress = false; }
    }

    #region Content events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsRemoved += Content_ItemsRemoved;
      Content.CollectionReset += Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
      Content.OptimizerNameChanged += Content_AlgorithmNameChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= Content_ItemsAdded;
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.CollectionReset -= Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
      Content.OptimizerNameChanged -= Content_AlgorithmNameChanged;
      base.DeregisterContentEvents();
    }

    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (var run in e.Items) RegisterRunEvents(run);
      if (suppressContentEvents) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
        return;
      }
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        UpdateComboBoxes();
        GroupRuns();
      } finally { updateInProgress = false; }
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (var run in e.Items) DeregisterRunEvents(run);
      if (suppressContentEvents) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
        return;
      }
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        UpdateComboBoxes();
        GroupRuns();
      } finally { updateInProgress = false; }
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      foreach (var run in e.OldItems) DeregisterRunEvents(run);
      foreach (var run in e.Items) RegisterRunEvents(run);
      if (suppressContentEvents) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
        return;
      }
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        UpdateComboBoxes();
        GroupRuns();
      } finally { updateInProgress = false; }
    }
    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
        return;
      }
      suppressContentEvents = Content.UpdateOfRunsInProgress;
      if (!suppressContentEvents) {
        if (updateInProgress) return;
        try {
          updateInProgress = true;
          UpdateComboBoxes();
          GroupRuns();
        } finally { updateInProgress = false; }
      }
    }

    private void RegisterRunEvents(IRun run) {
      run.PropertyChanged += run_PropertyChanged;
    }
    private void DeregisterRunEvents(IRun run) {
      run.PropertyChanged -= run_PropertyChanged;
    }
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressContentEvents) return;
      if (InvokeRequired) {
        Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      } else {
        if (e.PropertyName == "Visible") {
          if (updateInProgress) return;
          try {
            updateInProgress = true;
            UpdateRuns();
          } finally { updateInProgress = false; }
        }
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      dataTableComboBox.Items.Clear();
      groupComboBox.Items.Clear();
      targetChart.ChartAreas[0].AxisX.IsLogarithmic = false;
      targetChart.Series.Clear();
      invisibleTargetSeries.Clear();
      byCostDataTable.VisualProperties.XAxisLogScale = false;
      byCostDataTable.Rows.Clear();

      UpdateCaption();
      if (Content != null) {
        try {
          updateInProgress = true;
          UpdateComboBoxes();
          UpdateRuns();
        } finally { updateInProgress = false; }
      }
    }


    private void UpdateComboBoxes() {
      var selectedGroupItem = (string)groupComboBox.SelectedItem;

      var groupings = Content.ParameterNames.OrderBy(x => x).ToArray();
      groupComboBox.Items.Clear();
      groupComboBox.Items.Add(AllInstances);
      groupComboBox.Items.AddRange(groupings);
      if (selectedGroupItem != null && groupComboBox.Items.Contains(selectedGroupItem)) {
        groupComboBox.SelectedItem = selectedGroupItem;
      } else if (groupComboBox.Items.Count > 0) {
        groupComboBox.SelectedItem = groupComboBox.Items[0];
      }

      string selectedDataTable = (string)dataTableComboBox.SelectedItem;

      dataTableComboBox.Items.Clear();
      var dataTables = (from run in Content
                        from result in run.Results
                        where result.Value is IndexedDataTable<double>
                        select result.Key).Distinct().ToArray();

      dataTableComboBox.Items.AddRange(dataTables);
      if (selectedDataTable != null && dataTableComboBox.Items.Contains(selectedDataTable)) {
        dataTableComboBox.SelectedItem = selectedDataTable;
      } else if (dataTableComboBox.Items.Count > 0) {
        dataTableComboBox.SelectedItem = dataTableComboBox.Items[0];
      }

      var selectedProblemItem = (ProblemInstance)problemComboBox.SelectedItem;
      
      UpdateProblemInstances();
      
      foreach (var p in problems) {
        if (p.Equals(selectedProblemItem))
          problemComboBox.SelectedItem = p;
      }

      if (selectedProblemItem == null && problems.Count > 1) problemComboBox.SelectedItem = problems[1];

      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      groupComboBox.Enabled = Content != null;
      problemComboBox.Enabled = Content != null && problemComboBox.Items.Count > 1;
      dataTableComboBox.Enabled = Content != null && dataTableComboBox.Items.Count > 1;
      addTargetsAsResultButton.Enabled = Content != null && targets != null && dataTableComboBox.SelectedIndex >= 0;
      addBudgetsAsResultButton.Enabled = Content != null && budgets != null && dataTableComboBox.SelectedIndex >= 0;
      generateTargetsButton.Enabled = targets != null;
    }

    private void GroupRuns() {
      groups = new List<AlgorithmInstance>();

      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;

      var selectedGroup = (string)groupComboBox.SelectedItem;
      if (string.IsNullOrEmpty(selectedGroup)) return;

      var selectedProblem = (ProblemInstance)problemComboBox.SelectedItem;
      if (selectedProblem == null) return;

      foreach (var alg in from r in Content
                          where (selectedGroup == AllInstances || r.Parameters.ContainsKey(selectedGroup))
                            && r.Visible
                            && selectedProblem.Match(r)
                          let key = selectedGroup == AllInstances
                            ? AllInstances : r.Parameters[selectedGroup].ToString()
                          group r by key into g
                          select g) {
        var trials = (from run in alg
                     from pd in problems.Skip(1) // exclude artificial match all
                     where pd.Match(run) && run.Results.ContainsKey(table)
                     let cgraph = run.Results[table] as IndexedDataTable<double>
                     where cgraph != null && cgraph.Rows.Count > 0
                        && cgraph.Rows.First().Values.Count > 0
                     group cgraph by pd into g
                     select g).ToList();

        if (trials.Count == 0) continue;
        groups.Add(new AlgorithmInstance(alg.Key, trials));
      }
    }

    private void UpdateProblemInstances() {
      try {
        problems.Clear();
        var table = (string)dataTableComboBox.SelectedItem;
        
        var problemDict = CalculateBestTargetPerProblemInstance(table);

        var problemTypesDifferent = problemDict.Keys.Select(x => x.ProblemType).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
        var problemNamesDifferent = problemDict.Keys.Select(x => x.ProblemName).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
        var evaluatorDifferent = problemDict.Keys.Select(x => x.Evaluator).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
        var maximizationDifferent = problemDict.Keys.Select(x => x.Maximization).Distinct().Count() > 1;
        var allEqual = !problemTypesDifferent && !problemNamesDifferent && !evaluatorDifferent && !maximizationDifferent;

        problems.Add(ProblemInstance.MatchAll);
        foreach (var p in problemDict.OrderBy(x => x.Key.ProblemName, new NaturalStringComparer()).ToList()) {
          p.Key.BestKnownQuality = p.Value;
          p.Key.DisplayProblemType = problemTypesDifferent;
          p.Key.DisplayProblemName = problemNamesDifferent || allEqual;
          p.Key.DisplayEvaluator = evaluatorDifferent;
          p.Key.DisplayMaximization = maximizationDifferent;
          problems.Add(p.Key);
        }
      } finally { ((BindingSource)problemComboBox.DataSource).ResetBindings(false); }
    }

    #region Performance analysis by (multiple) target(s)
    private void UpdateResultsByTarget() {
      // necessary to reset log scale -> empty chart cannot use log scaling
      targetChart.ChartAreas[0].AxisX.IsLogarithmic = false;
      targetChart.Series.Clear();
      invisibleTargetSeries.Clear();
      
      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;

      if (targets == null) GenerateDefaultTargets();
      
      if (groups.Count == 0) return;

      var xAxisTitles = new HashSet<string>();

      // hits describes the number of target hits at a certain time for a certain group
      var hits = new Dictionary<string, SortedList<double, int>>();
      // misses describes the number of target misses after a certain time for a certain group
      // for instance when a run ends, but has not achieved all targets, misses describes
      // how many targets have been left open at the point when the run ended
      var misses = new Dictionary<string, SortedList<double, int>>();
      var totalRuns = new Dictionary<string, int>();

      var aggregate = aggregateTargetsCheckBox.Checked;
      double minEff = double.MaxValue, maxEff = double.MinValue;
      foreach (var alg in groups) {
        var noRuns = 0;
        SortedList<double, int> epdfHits = null, epdfMisses = null;
        if (aggregate) {
          hits[alg.Name] = epdfHits = new SortedList<double, int>();
          misses[alg.Name] = epdfMisses = new SortedList<double, int>();
        }
        foreach (var problem in alg.GetCases().Where(x => x.Maximization.HasValue)) {
          var max = problem.Maximization.Value;
          var absTargets = GetAbsoluteTargets(problem).ToArray();
          foreach (var run in alg.GetTrials(problem)) {
            noRuns++;
            xAxisTitles.Add(run.XAxisName);

            var efforts = absTargets.Select(t => GetEffortToHitTarget(run, t, max)).ToArray();
            minEff = Math.Min(minEff, efforts.Min(x => x.Item2));
            maxEff = Math.Max(maxEff, efforts.Max(x => x.Item2));
            for (var idx = 0; idx < efforts.Length; idx++) {
              var e = efforts[idx];
              if (!aggregate) {
                var key = alg.Name + "@" + (targetsAreRelative
                            ? (targets[idx] * 100).ToString(CultureInfo.CurrentCulture.NumberFormat) + "%"
                            : targets[idx].ToString(CultureInfo.CurrentCulture.NumberFormat));
                if (!hits.TryGetValue(key, out epdfHits))
                  hits[key] = epdfHits = new SortedList<double, int>();
                if (!misses.TryGetValue(key, out epdfMisses))
                  misses[key] = epdfMisses = new SortedList<double, int>();
                totalRuns[key] = noRuns;
              };
              var list = e.Item1 ? epdfHits : epdfMisses;
              int v;
              if (list.TryGetValue(e.Item2, out v))
                list[e.Item2] = v + 1;
              else list[e.Item2] = 1;
            }
          }
        }
        if (aggregate) totalRuns[alg.Name] = noRuns;
      }

      UpdateTargetChartAxisXBounds(minEff, maxEff);

      DrawTargetsEcdf(hits, misses, totalRuns);

      if (targets.Length == 1) {
        if (targetsAreRelative)
          targetChart.ChartAreas[0].AxisY.Title = "Probability to be " + (targets[0] * 100) + "% worse than best";
        else targetChart.ChartAreas[0].AxisY.Title = "Probability to reach at least a fitness of " + targets[0];
      } else targetChart.ChartAreas[0].AxisY.Title = "Proportion of reached targets";
      targetChart.ChartAreas[0].AxisX.Title = string.Join(" / ", xAxisTitles);
      targetChart.ChartAreas[0].AxisX.IsLogarithmic = CanDisplayLogarithmic();
      targetChart.ChartAreas[0].CursorY.Interval = 0.05;

      UpdateErtTables();
    }

    private void DrawTargetsEcdf(Dictionary<string, SortedList<double, int>> hits, Dictionary<string, SortedList<double, int>> misses, Dictionary<string, int> noRuns) {
      var colorCount = 0;
      var lineStyleCount = 0;
      
      var showMarkers = markerCheckBox.Checked;
      foreach (var list in hits) {
        var row = new Series(list.Key) {
          ChartType = list.Value.Count > 1000 ? SeriesChartType.FastLine : SeriesChartType.StepLine,
          BorderWidth = 3,
          Color = colors[colorCount],
          BorderDashStyle = lineStyles[lineStyleCount],
        };
        var rowShade = new Series(list.Key + "-range") {
          IsVisibleInLegend = false,
          ChartType = SeriesChartType.Range,
          Color = Color.FromArgb(32, colors[colorCount]),
          YValuesPerPoint = 2
        };

        var ecdf = 0.0;
        var missedecdf = 0.0;
        var iter = misses[list.Key].GetEnumerator();
        var moreMisses = iter.MoveNext();
        var totalTargets = noRuns[list.Key];
        if (aggregateTargetsCheckBox.Checked) totalTargets *= targets.Length;
        var movingTargets = totalTargets;
        var labelPrinted = false;
        foreach (var h in list.Value) {
          var prevmissedecdf = missedecdf;
          while (moreMisses && iter.Current.Key < h.Key) {
            if (!labelPrinted && row.Points.Count > 0) {
              var point = row.Points.Last();
              if (showLabelsInTargetChart)
                point.Label = row.Name;
              point.MarkerStyle = MarkerStyle.Cross;
              point.MarkerBorderWidth = 1;
              point.MarkerSize = 10;
              labelPrinted = true;
              rowShade.Points.Add(new DataPoint(point.XValue, new[] {ecdf / totalTargets, (ecdf + missedecdf) / totalTargets}));
            }
            missedecdf += iter.Current.Value;
            movingTargets -= iter.Current.Value;
            if (row.Points.Count > 0 && row.Points.Last().XValue == iter.Current.Key) {
              row.Points.Last().SetValueY(ecdf / movingTargets);
            } else {
              var dp = new DataPoint(iter.Current.Key, ecdf / movingTargets);
              if (showMarkers) {
                dp.MarkerStyle = MarkerStyle.Circle;
                dp.MarkerBorderWidth = 1;
                dp.MarkerSize = 5;
              }
              row.Points.Add(dp);
              prevmissedecdf = missedecdf;
            }
            if (boundShadingCheckBox.Checked) {
              if (rowShade.Points.Count > 0 && rowShade.Points.Last().XValue == iter.Current.Key)
                rowShade.Points.Last().SetValueY(ecdf / totalTargets, (ecdf + missedecdf) / totalTargets);
              else rowShade.Points.Add(new DataPoint(iter.Current.Key, new[] {ecdf / totalTargets, (ecdf + missedecdf) / totalTargets}));
            }
            moreMisses = iter.MoveNext();
            if (!labelPrinted) {
              var point = row.Points.Last();
              if (showLabelsInTargetChart)
                point.Label = row.Name;
              point.MarkerStyle = MarkerStyle.Cross;
              point.MarkerBorderWidth = 1;
              point.MarkerSize = 10;
              labelPrinted = true;
            }
          }
          ecdf += h.Value;
          if (row.Points.Count > 0 && row.Points.Last().XValue == h.Key) {
            row.Points.Last().SetValueY(ecdf / movingTargets);
          } else {
            var dp = new DataPoint(h.Key, ecdf / movingTargets);
            if (showMarkers) {
              dp.MarkerStyle = MarkerStyle.Circle;
              dp.MarkerBorderWidth = 1;
              dp.MarkerSize = 5;
            }
            row.Points.Add(dp);
          }
          if (missedecdf > 0 && boundShadingCheckBox.Checked) {
            if (rowShade.Points.Count > 0 && rowShade.Points.Last().XValue == h.Key)
              rowShade.Points.Last().SetValueY(ecdf / totalTargets, (ecdf + missedecdf) / totalTargets);
            else rowShade.Points.Add(new DataPoint(h.Key, new[] {ecdf / totalTargets, (ecdf + missedecdf) / totalTargets}));
          }
        }

        while (moreMisses) {
          // if there are misses beyond the last hit we extend the shaded area
          missedecdf += iter.Current.Value;
          if (row.Points.Count == 0 || row.Points.Last().XValue < iter.Current.Key) {
            var dp = new DataPoint(iter.Current.Key, ecdf / movingTargets);
            if (showMarkers) {
              dp.MarkerStyle = MarkerStyle.Circle;
              dp.MarkerBorderWidth = 1;
              dp.MarkerSize = 5;
            }
            row.Points.Add(dp);
            if (boundShadingCheckBox.Checked) {
              rowShade.Points.Add(new DataPoint(iter.Current.Key, new[] { ecdf / totalTargets, (ecdf + missedecdf) / totalTargets }));
            }
          }
          moreMisses = iter.MoveNext();

          if (!labelPrinted && row.Points.Count > 0) {
            var point = row.Points.Last();
            if (showLabelsInTargetChart)
              point.Label = row.Name;
            point.MarkerStyle = MarkerStyle.Cross;
            point.MarkerBorderWidth = 1;
            point.MarkerSize = 10;
            labelPrinted = true;
          }
        }

        if (!labelPrinted && row.Points.Count > 0) {
          var point = row.Points.Last();
          if (showLabelsInTargetChart)
            point.Label = row.Name;
          point.MarkerStyle = MarkerStyle.Cross;
          point.MarkerBorderWidth = 1;
          point.MarkerSize = 10;
          rowShade.Points.Add(new DataPoint(point.XValue, new[] {ecdf / totalTargets, (ecdf + missedecdf) / totalTargets}));
          labelPrinted = true;
        }

        ConfigureSeries(row);
        targetChart.Series.Add(rowShade);
        targetChart.Series.Add(row);

        colorCount = (colorCount + 1) % colors.Length;
        if (colorCount == 0) lineStyleCount = (lineStyleCount + 1) % lineStyles.Length;
      }
    }

    private void UpdateTargetChartAxisXBounds(double minEff, double maxEff) {
      var minZeros = (int)Math.Floor(Math.Log10(minEff));
      var maxZeros = (int)Math.Floor(Math.Log10(maxEff));
      var axisMin = (decimal)Math.Pow(10, minZeros);
      var axisMax = (decimal)Math.Pow(10, maxZeros);
      if (!targetLogScalingCheckBox.Checked) {
        var minAdd = (decimal)Math.Pow(10, minZeros - 1) * 2;
        var maxAdd = (decimal)Math.Pow(10, maxZeros - 1) * 2;
        while (axisMin + minAdd < (decimal)minEff) axisMin += minAdd;
        while (axisMax <= (decimal)maxEff) axisMax += maxAdd;
      } else axisMax = (decimal)Math.Pow(10, (int)Math.Ceiling(Math.Log10(maxEff)));
      targetChart.ChartAreas[0].AxisX.Minimum = (double)axisMin;
      targetChart.ChartAreas[0].AxisX.Maximum = (double)axisMax;
    }

    private IEnumerable<double> GetAbsoluteTargets(ProblemInstance pInstance) {
      if (!targetsAreRelative) return targets;
      if (!pInstance.Maximization.HasValue) throw new ArgumentException("Problem doesn't specify if it is to be maximized or minimized.");

      var maximization = pInstance.Maximization.Value;
      var bestKnown = pInstance.BestKnownQuality;
      if (double.IsNaN(bestKnown)) throw new ArgumentException("Problem instance does not have a defined best - known quality.");
      IEnumerable<double> tmp = null;
      if (bestKnown > 0) {
        tmp = targets.Select(x => (maximization ? (1 - x) : (1 + x)) * bestKnown);
      } else if (bestKnown < 0) {
        tmp = targets.Select(x => (!maximization ? (1 - x) : (1 + x)) * bestKnown);
      } else {
        // relative to 0 is impossible
        tmp = targets;
      }
      return tmp;
    }

    private double[] GetAbsoluteTargetsWorstToBest(ProblemInstance pInstance) {
      if (targetsAreRelative && double.IsNaN(pInstance.BestKnownQuality)) throw new ArgumentException("Problem instance does not have a defined best-known quality.");
      if (!pInstance.Maximization.HasValue) throw new ArgumentException("Problem doesn't specify if it is to be maximized or minimized.");
      var absTargets = GetAbsoluteTargets(pInstance);
      return (pInstance.Maximization.Value
        ? absTargets.OrderBy(x => x) : absTargets.OrderByDescending(x => x)).ToArray();
    }

    private void GenerateDefaultTargets() {
      targets = new[] { 0.1, 0.095, 0.09, 0.085, 0.08, 0.075, 0.07, 0.065, 0.06, 0.055, 0.05, 0.045, 0.04, 0.035, 0.03, 0.025, 0.02, 0.015, 0.01, 0.005, 0 };
      SynchronizeTargetTextBox();
    }

    private Tuple<bool, double> GetEffortToHitTarget(
        ConvergenceGraph cgraph,
        double absTarget, bool maximization) {
      if (cgraph.Points == 0)
        throw new ArgumentException("Convergence graph is empty.", "convergenceGraph");
      
      var index = cgraph.BinarySearch(new ConvergenceGraphPoint(0.0, absTarget), new TargetComparer(maximization));
      if (index >= 0) {
        return Tuple.Create(true, cgraph[index].Runlength);
      } else {
        index = ~index;
        if (index >= cgraph.Points)
          return Tuple.Create(false, cgraph.Last().Runlength);
        return Tuple.Create(true, cgraph[index].Runlength);
      }
    }

    private void UpdateErtTables() {
      ertTableView.Content = null;
      var columns = targets.Length + 1;
      var totalRows = groups.Count * groups.Max(x => x.GetNumberOfCases()) + groups.Max(x => x.GetNumberOfCases());
      var matrix = new StringMatrix(totalRows, columns);
      var rowNames = new List<string>();
      matrix.ColumnNames = targets.Select(x => targetsAreRelative ? (100 * x).ToString() + "%" : x.ToString())
        .Concat(new[] { "#succ" }).ToList();
      var rowCount = 0;

      var tableName = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(tableName)) return;
      
      var problems = groups.SelectMany(x => x.GetCases()).Distinct().ToList();

      foreach (var problem in problems.OrderBy(x => x.ProblemName, new NaturalStringComparer())) {
        if (double.IsNaN(problem.BestKnownQuality) || !problem.Maximization.HasValue) continue;
        rowNames.Add(problem.ToString());
        var max = problem.Maximization.Value;
        var absTargets = GetAbsoluteTargetsWorstToBest(problem);
        if (targetsAreRelative) {
          // print out the absolute target values
          for (var i = 0; i < absTargets.Length; i++) {
            matrix[rowCount, i] = absTargets[i].ToString("##,0.0", CultureInfo.CurrentCulture.NumberFormat);
          }
        }
        rowCount++;

        foreach (var alg in groups) {
          rowNames.Add(alg.Name);
          var runs = alg.GetTrials(problem).ToList();
          if (runs.Count == 0) {
            matrix[rowCount, columns - 1] = "N/A";
            rowCount++;
            continue;
          }
          var result = default(ErtCalculationResult);
          for (var i = 0; i < absTargets.Length; i++) {
            result = ExpectedRuntimeHelper.CalculateErt(runs.Select(x => x.ToTuples()), absTargets[i], max);
            matrix[rowCount, i] = result.ToString();
          }
          matrix[rowCount, columns - 1] = targets.Length > 0 ? result.SuccessfulRuns + "/" + result.TotalRuns : "-";
          rowCount++;
        }
      }
      matrix.RowNames = rowNames;
      ertTableView.Content = matrix;
      ertTableView.DataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
    }
    #endregion

    #region Performance analysis by (multiple) budget(s)
    private void UpdateResultsByCost() {
      // necessary to reset log scale -> empty chart cannot use log scaling
      byCostDataTable.VisualProperties.XAxisLogScale = false;
      byCostDataTable.Rows.Clear();

      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;

      if (budgets == null) GenerateDefaultBudgets(table);
      
      if (groups.Count == 0) return;

      var colorCount = 0;
      var lineStyleCount = 0;
      
      foreach (var alg in groups) {
        var hits = new Dictionary<string, SortedList<double, double>>();

        foreach (var problem in alg.GetCases()) {
          foreach (var run in alg.GetTrials(problem)) {
            CalculateHitsForEachBudget(hits, run, problem, alg.Name);
          }
        }

        foreach (var list in hits) {
          var row = new IndexedDataRow<double>(list.Key) {
            VisualProperties = {
              ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
              LineWidth = 2,
              Color = colors[colorCount],
              LineStyle = hlLineStyles[lineStyleCount],
              StartIndexZero = false
            }
          };

          var total = 0.0;
          var count = list.Value.Count;
          foreach (var h in list.Value) {
            total += h.Value;
            row.Values.Add(Tuple.Create(h.Key, total / (double)count));
          }

          byCostDataTable.Rows.Add(row);
        }
        colorCount = (colorCount + 1) % colors.Length;
        if (colorCount == 0) lineStyleCount = (lineStyleCount + 1) % lineStyles.Length;
      }

      byCostDataTable.VisualProperties.XAxisTitle = "Targets to Best-Known Ratio";
      byCostDataTable.VisualProperties.XAxisLogScale = byCostDataTable.Rows.Count > 0 && budgetLogScalingCheckBox.Checked;
    }

    private void GenerateDefaultBudgets(string table) {
      var runs = Content.Where(x => x.Results.ContainsKey(table) && x.Results[table] is IndexedDataTable<double>)
                        .Select(x => (IndexedDataTable<double>)x.Results[table])
                        .Where(x => x.Rows.Count > 0 && x.Rows.First().Values.Count > 0)
                        .Select(x => x.Rows.First())
                        .ToList();
      if (runs.Count == 0) {
        budgets = new double[0];
        suppressBudgetsEvents = true;
        budgetsTextBox.Text = string.Empty;
        suppressBudgetsEvents = false;
        return;
      }

      var min = runs.Select(x => x.Values.Select(y => y.Item1).Min()).Min();
      var max = runs.Select(x => x.Values.Select(y => y.Item1).Max()).Max();
      var points = 3;
      budgets = Enumerable.Range(1, points).Select(x => min + (x / (double)points) * (max - min)).ToArray();
      suppressBudgetsEvents = true;
      budgetsTextBox.Text = string.Join(" ; ", budgets);
      suppressBudgetsEvents = false;
    }

    private void CalculateHitsForEachBudget(Dictionary<string, SortedList<double, double>> hits, ConvergenceGraph cgraph, ProblemInstance problem, string groupName) {
      var max = problem.Maximization.Value;
      var prevIndex = 0;
      foreach (var b in budgets) {
        var key = groupName + "-" + b;
        var index = cgraph.BinarySearch(prevIndex, cgraph.Points - prevIndex, new ConvergenceGraphPoint(b, 0.0), new CostComparer());
        if (index < 0) {
          index = ~index;
          if (index >= cgraph.Points) break; // the run wasn't long enough to use up budget b (or any subsequent larger one)
        }
        if (!hits.ContainsKey(key)) hits.Add(key, new SortedList<double, double>());
        var v = cgraph[index];
        var relTgt = CalculateRelativeDifference(max, problem.BestKnownQuality, v.Quality) + 1;
        if (hits[key].ContainsKey(relTgt))
          hits[key][relTgt]++;
        else hits[key][relTgt] = 1.0;
        prevIndex = index;
      }
    }
    #endregion

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " RLD View" : ViewAttribute.GetViewName(GetType());
    }

    private void SynchronizeTargetTextBox() {
      if (InvokeRequired) Invoke((Action)SynchronizeTargetTextBox);
      else {
        if (targetsAreRelative)
          targetsTextBox.Text = string.Join("% ; ", targets.Select(x => x * 100)) + "%";
        else targetsTextBox.Text = string.Join(" ; ", targets);
      }
    }

    private void groupComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        UpdateRuns();
        SetEnabledStateOfControls();
      } finally { updateInProgress = false; }
    }
    private void problemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        UpdateRuns();
        SetEnabledStateOfControls();
      } finally { updateInProgress = false; }
    }
    private void dataTableComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        if (dataTableComboBox.SelectedIndex >= 0)
          GenerateDefaultBudgets((string)dataTableComboBox.SelectedItem);
        UpdateBestKnownQualities();
        UpdateRuns();
        SetEnabledStateOfControls();
      } finally { updateInProgress = false; }
    }

    private void logScalingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateResultsByTarget();
      byCostDataTable.VisualProperties.XAxisLogScale = byCostDataTable.Rows.Count > 0 && budgetLogScalingCheckBox.Checked;
    }

    private void boundShadingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateResultsByTarget();
    }

    #region Event handlers for target analysis
    private void targetsTextBox_Validating(object sender, CancelEventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        var targetStrings = targetsTextBox.Text.Split(new[] { '%', ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var targetList = new List<decimal>();
        foreach (var ts in targetStrings) {
          decimal t;
          if (!decimal.TryParse(ts, out t)) {
            errorProvider.SetError(targetsTextBox, "Not all targets can be parsed: " + ts);
            e.Cancel = true;
            return;
          }
          if (targetsAreRelative)
            targetList.Add(t / 100);
          else targetList.Add(t);
        }
        if (targetList.Count == 0) {
          errorProvider.SetError(targetsTextBox, "Give at least one target value!");
          e.Cancel = true;
          return;
        }
        e.Cancel = false;
        errorProvider.SetError(targetsTextBox, null);
        targets = targetsAreRelative ? targetList.Select(x => (double)x).OrderByDescending(x => x).ToArray() : targetList.Select(x => (double)x).ToArray();

        SynchronizeTargetTextBox();
        UpdateResultsByTarget();
        SetEnabledStateOfControls();

      } finally { updateInProgress = false; }
    }

    private void aggregateTargetsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        SuspendRepaint();
        UpdateResultsByTarget();
      } finally {
        updateInProgress = false;
        ResumeRepaint(true);
      }
    }

    private void relativeOrAbsoluteComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (updateInProgress) return;
      try {
        updateInProgress = true;
        var pd = (ProblemInstance)problemComboBox.SelectedItem;
        if (!double.IsNaN(pd.BestKnownQuality) && pd.Maximization.HasValue) {
          var max = pd.Maximization.Value;
          if (targetsAreRelative) targets = GetAbsoluteTargets(pd).ToArray();
          else {
            // Rounding to 5 digits since it's certainly appropriate for this application
            if (pd.BestKnownQuality > 0) {
              targets = targets.Select(x => Math.Round(max ? 1.0 - (x / pd.BestKnownQuality) : (x / pd.BestKnownQuality) - 1.0, 5)).ToArray();
            } else if (pd.BestKnownQuality < 0) {
              targets = targets.Select(x => Math.Round(!max ? 1.0 - (x / pd.BestKnownQuality) : (x / pd.BestKnownQuality) - 1.0, 5)).ToArray();
            }
          }
        }
        targetsAreRelative = (string)relativeOrAbsoluteComboBox.SelectedItem == "relative";
        SynchronizeTargetTextBox();
        
        SuspendRepaint();
        UpdateResultsByTarget();
      } finally {
        updateInProgress = false;
        ResumeRepaint(true);
      }
    }

    private void generateTargetsButton_Click(object sender, EventArgs e) {
      if (targets == null) return;
      decimal max = 10, min = 0, count = 10;
      max = (decimal)targets.Max();
      min = (decimal)targets.Min();
      count = targets.Length - 1;
      if (targetsAreRelative) {
        max *= 100;
        min *= 100;
      }
      using (var dialog = new DefineArithmeticProgressionDialog(false, min, max, (max - min) / count)) {
        if (dialog.ShowDialog() == DialogResult.OK) {
          if (dialog.Values.Any()) {
            targets = targetsAreRelative
              ? dialog.Values.OrderByDescending(x => x).Select(x => (double)x / 100.0).ToArray()
              : dialog.Values.Select(x => (double)x).ToArray();

            try {
              updateInProgress = true;
              SynchronizeTargetTextBox();
              UpdateResultsByTarget();
              SetEnabledStateOfControls();
            } finally { updateInProgress = false; }
          }
        }
      }
    }

    private void addTargetsAsResultButton_Click(object sender, EventArgs e) {
      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;
      
      foreach (var run in Content) {
        if (!run.Results.ContainsKey(table) || !(run.Results[table] is IndexedDataTable<double>)) continue;
        var resultsTable = (IndexedDataTable<double>)run.Results[table];
        var values = resultsTable.Rows.First().Values;
        var pd = new ProblemInstance(run);
        if (!pd.Maximization.HasValue) continue;
        pd = problems.Single(x => x.Equals(pd));
        if (targetsAreRelative && double.IsNaN(pd.BestKnownQuality)) continue;

        var max = pd.Maximization.Value;
        var absTargets = GetAbsoluteTargetsWorstToBest(pd);
        var cgraph = new ConvergenceGraph(resultsTable, max);

        var prevIndex = 0;
        for (var i = 0; i < absTargets.Length; i++) {
          var absTarget = absTargets[i];
          var index = cgraph.BinarySearch(prevIndex, values.Count - prevIndex, new ConvergenceGraphPoint(0.0, absTarget), new TargetComparer(max));
          if (index < 0) {
            index = ~index;
            if (index >= values.Count) break; // the target (and subsequent ones) wasn't achieved
          }
          var target = targetsAreRelative ? (targets[i] * 100) : absTarget;
          run.Results[table + (targetsAreRelative ? ".RelTarget " : ".AbsTarget ") + target + (targetsAreRelative ? "%" : string.Empty)] = new DoubleValue(values[index].Item1);
          prevIndex = index;
        }
      }
    }

    private void markerCheckBox_CheckedChanged(object sender, EventArgs e) {
      try {
        updateInProgress = true;
        SuspendRepaint();
        UpdateResultsByTarget();
      } finally {
        updateInProgress = false;
        ResumeRepaint(true);
      }
    }

    private void showLabelsCheckBox_CheckedChanged(object sender, EventArgs e) {
      showLabelsInTargetChart = showLabelsCheckBox.Checked;
      try {
        updateInProgress = true;
        SuspendRepaint();
        UpdateResultsByTarget();
      } finally {
        updateInProgress = false;
        ResumeRepaint(true);
      }
    }
    #endregion

    #region Event handlers for cost analysis
    private bool suppressBudgetsEvents;
    private void budgetsTextBox_Validating(object sender, CancelEventArgs e) {
      if (suppressBudgetsEvents) return;
      var budgetStrings = budgetsTextBox.Text.Split(new[] { ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      var budgetList = new List<double>();
      foreach (var ts in budgetStrings) {
        double b;
        if (!double.TryParse(ts, out b)) {
          errorProvider.SetError(budgetsTextBox, "Not all budgets can be parsed: " + ts);
          e.Cancel = true;
          return;
        }
        budgetList.Add(b);
      }
      if (budgetList.Count == 0) {
        errorProvider.SetError(budgetsTextBox, "Give at least one budget value!");
        e.Cancel = true;
        return;
      }
      e.Cancel = false;
      errorProvider.SetError(budgetsTextBox, null);
      budgets = budgetList.OrderBy(x => x).ToArray();
      try {
        suppressBudgetsEvents = true;
        budgetsTextBox.Text = string.Join(" ; ", budgets);
      } finally { suppressBudgetsEvents = false; }
      UpdateResultsByCost();
      SetEnabledStateOfControls();
    }

    private void generateBudgetsButton_Click(object sender, EventArgs e) {
      decimal max = 1, min = 0, count = 10;
      if (budgets != null) {
        max = (decimal)budgets.Max();
        min = (decimal)budgets.Min();
        count = budgets.Length;
      } else if (Content.Count > 0 && dataTableComboBox.SelectedIndex >= 0) {
        var table = (string)dataTableComboBox.SelectedItem;
        min = (decimal)Content.Where(x => x.Results.ContainsKey(table)).Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Min(y => y.Item1)).Min();
        max = (decimal)Content.Where(x => x.Results.ContainsKey(table)).Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Max(y => y.Item1)).Max();
        count = 3;
      }
      using (var dialog = new DefineArithmeticProgressionDialog(false, min, max, (max - min) / count)) {
        if (dialog.ShowDialog() == DialogResult.OK) {
          if (dialog.Values.Any()) {
            budgets = dialog.Values.OrderBy(x => x).Select(x => (double)x).ToArray();

            try {
              suppressBudgetsEvents = true;
              budgetsTextBox.Text = string.Join(" ; ", budgets);
            } finally { suppressBudgetsEvents = false; }

            UpdateResultsByCost();
            SetEnabledStateOfControls();
          }
        }
      }
    }

    private void addBudgetsAsResultButton_Click(object sender, EventArgs e) {
      var table = (string)dataTableComboBox.SelectedItem;

      foreach (var run in Content) {
        if (!run.Results.ContainsKey(table)) continue;
        var resultsTable = (IndexedDataTable<double>)run.Results[table];
        var values = resultsTable.Rows.First().Values;
        var pd = new ProblemInstance(run);
        if (!pd.Maximization.HasValue) continue;
        pd = problems.Single(x => x.Equals(pd));
        var max = pd.Maximization.Value;
        var cgraph = new ConvergenceGraph(resultsTable, max);
        var prevIndex = 0;
        foreach (var b in budgets) {
          var index = cgraph.BinarySearch(prevIndex, values.Count - prevIndex, new ConvergenceGraphPoint(b, 0.0), new CostComparer());
          if (index < 0) {
            index = ~index;
            if (index >= values.Count) break; // the run wasn't long enough to use up budget b (or any subsequent larger one)
          }
          var v = values[index];
          var tgt = targetsAreRelative ? CalculateRelativeDifference(max, pd.BestKnownQuality, v.Item2) : v.Item2;
          run.Results[table + (targetsAreRelative ? ".CostForRelTarget " : ".CostForAbsTarget ") + b] = new DoubleValue(tgt);
          prevIndex = index;
        }
      }
    }
    #endregion

    #region Helpers
    private double CalculateRelativeDifference(bool maximization, double bestKnown, double fit) {
      if (bestKnown == 0) {
        // no relative difference with respect to bestKnown possible
        return maximization ? -fit : fit;
      }
      var absDiff = (fit - bestKnown);
      var relDiff = absDiff / bestKnown;
      if (maximization) {
        return bestKnown > 0 ? -relDiff : relDiff;
      } else {
        return bestKnown > 0 ? relDiff : -relDiff;
      }
    }

    private Dictionary<ProblemInstance, double> CalculateBestTargetPerProblemInstance(string table) {
      if (table == null) table = string.Empty;
      var dict = new Dictionary<ProblemInstance, double>();
      foreach (var k in from r in Content where r.Visible let pd = new ProblemInstance(r) group r by pd into g select new { Problem = g.Key, Runs = g.ToList() }) {
        var pd = k.Problem;
        if (!pd.Maximization.HasValue) continue;
        var values = GetQualityValues(k.Runs, table).ToList();
        var target = double.NaN;
        if (values.Count > 0) {
          target = pd.Maximization.Value ? values.Max() : values.Min();
        }
        dict[pd] = target;
      }
      return dict;
    }

    private IEnumerable<double> GetQualityValues(List<IRun> runs, string table) {
      foreach (var r in runs) {
        IItem item;
        if (r.Parameters.TryGetValue("BestKnownQuality", out item)) {
          var dval = item as DoubleValue;
          if (dval != null && !double.IsNaN(dval.Value))
            yield return dval.Value;
        }
        if (r.Results.TryGetValue(table, out item)) {
          var dt = item as IndexedDataTable<double>;
          if (dt != null && dt.Rows.Count > 0 && dt.Rows.First().Values.Count > 0) {
            var last = dt.Rows.First().Values.Last().Item2;
            if (!double.IsNaN(last))
              yield return Math.Round(last, 10);
          }
        }
      }
    }

    private void UpdateRuns() {
      if (InvokeRequired) {
        Invoke((Action)UpdateRuns);
        return;
      }
      SuspendRepaint();
      try {
        GroupRuns();
        UpdateResultsByTarget();
        UpdateResultsByCost();
      } finally { ResumeRepaint(true); }
    }

    private void UpdateBestKnownQualities() {
      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;

      var targetsPerProblem = CalculateBestTargetPerProblemInstance(table);
      foreach (var pd in problems) {
        double bkq;
        if (targetsPerProblem.TryGetValue(pd, out bkq))
          pd.BestKnownQuality = bkq;
        else pd.BestKnownQuality = double.NaN;
      }
    }
    #endregion

    private void ConfigureSeries(Series series) {
      series.SmartLabelStyle.Enabled = showLabelsInTargetChart;
      series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
      series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
      series.SmartLabelStyle.CalloutLineColor = series.Color;
      series.SmartLabelStyle.CalloutLineWidth = 2;
      series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Underlined;
      series.SmartLabelStyle.IsOverlappedHidden = false;
      series.SmartLabelStyle.MaxMovingDistance = 200;
      series.ToolTip = series.LegendText + " X = #VALX, Y = #VALY";
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = targetChart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        ToggleTargetChartSeriesVisible(result.Series);
      }
    }
    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = targetChart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }
    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      foreach (LegendItem legendItem in e.LegendItems) {
        var series = targetChart.Series[legendItem.SeriesName];
        if (series != null) {
          bool seriesIsInvisible = invisibleTargetSeries.Any(x => x.Name == series.Name);
          foreach (LegendCell cell in legendItem.Cells) {
            cell.ForeColor = seriesIsInvisible ? Color.Gray : Color.Black;
          }
        }
      }
    }

    private void ToggleTargetChartSeriesVisible(Series series) {
      var indexList = invisibleTargetSeries.FindIndex(x => x.Name == series.Name);
      var indexChart = targetChart.Series.IndexOf(series);
      if (targetChart.Series.Count == 1) targetChart.ChartAreas[0].AxisX.IsLogarithmic = false;
      targetChart.Series.RemoveAt(indexChart);
      var s = indexList >= 0 ? invisibleTargetSeries[indexList] : new Series(series.Name) {
        Color = series.Color,
        ChartType = series.ChartType,
        BorderWidth = series.BorderWidth,
        BorderDashStyle = series.BorderDashStyle
      };
      if (indexList < 0) {
        // hide
        invisibleTargetSeries.Add(series);
        var shadeSeries = targetChart.Series.FirstOrDefault(x => x.Name == series.Name + "-range");
        if (shadeSeries != null) {
          if (targetChart.Series.Count == 1) targetChart.ChartAreas[0].AxisX.IsLogarithmic = false;
          targetChart.Series.Remove(shadeSeries);
          invisibleTargetSeries.Add(shadeSeries);
          indexChart--;
        }
      } else {
        // show
        invisibleTargetSeries.RemoveAt(indexList);
        var shadeSeries = invisibleTargetSeries.FirstOrDefault(x => x.Name == series.Name + "-range");
        if (shadeSeries != null) {
          invisibleTargetSeries.Remove(shadeSeries);
          InsertOrAddSeries(indexChart, shadeSeries);
          indexChart++;
        }
      }
      InsertOrAddSeries(indexChart, s);
      targetChart.ChartAreas[0].AxisX.IsLogarithmic = CanDisplayLogarithmic();
    }

    private bool CanDisplayLogarithmic() {
      return targetLogScalingCheckBox.Checked
        && targetChart.Series.Count > 0 // must have a series
        && targetChart.Series.Any(x => x.Points.Count > 0) // at least one series must have points
        && targetChart.Series.All(s => s.Points.All(p => p.XValue > 0)); // all points must be positive
    }

    private void InsertOrAddSeries(int index, Series s) {
      if (targetChart.Series.Count <= index)
        targetChart.Series.Add(s);
      else targetChart.Series.Insert(index, s);
    }

    #region Helper classes
    private class AlgorithmInstance : INotifyPropertyChanged {
      private string name;
      public string Name {
        get { return name; }
        set {
          if (name == value) return;
          name = value;
          OnPropertyChanged("Name");
        }
      }

      private Dictionary<ProblemInstance, List<ConvergenceGraph>> performanceData;

      public int GetNumberOfCases() {
        return performanceData.Count;
      }

      public IEnumerable<ProblemInstance> GetCases() {
        return performanceData.Keys;
      }

      public int GetNumberOfTrials(ProblemInstance p) {
        if (p == ProblemInstance.MatchAll) return performanceData.Select(x => x.Value.Count).Sum();
        List<ConvergenceGraph> trials;
        if (performanceData.TryGetValue(p, out trials))
          return trials.Count;

        return 0;
      }

      public IEnumerable<ConvergenceGraph> GetTrials(ProblemInstance p) {
        if (p == ProblemInstance.MatchAll) return performanceData.SelectMany(x => x.Value);
        List<ConvergenceGraph> trials;
        if (performanceData.TryGetValue(p, out trials))
          return trials;

        return Enumerable.Empty<ConvergenceGraph>();
      }

      public AlgorithmInstance(string name, IEnumerable<IGrouping<ProblemInstance, IndexedDataTable<double>>> trials) {
        this.name = name;

        performanceData = new Dictionary<ProblemInstance, List<ConvergenceGraph>>();
        foreach (var t in trials) {
          if (double.IsNaN(t.Key.BestKnownQuality) || !t.Key.Maximization.HasValue) continue;
          performanceData[t.Key] = t.Select(c => new ConvergenceGraph(c, t.Key.Maximization.Value)).ToList();
        }
      }

      public override bool Equals(object obj) {
        var other = obj as AlgorithmInstance;
        if (other == null) return false;
        return name == other.name;
      }

      public override int GetHashCode() {
        return name.GetHashCode();
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected virtual void OnPropertyChanged(string propertyName = null) {
        var handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private class ProblemInstance : INotifyPropertyChanged {
      private readonly bool matchAll;
      public static readonly ProblemInstance MatchAll = new ProblemInstance() {
        ProblemName = "All with Best-Known"
      };

      private ProblemInstance() {
        ProblemType = string.Empty;
        ProblemName = string.Empty;
        Evaluator = string.Empty;
        Maximization = null;
        DisplayProblemType = false;
        DisplayProblemName = false;
        DisplayEvaluator = false;
        DisplayMaximization = false;
        matchAll = true;
        BestKnownQuality = double.NaN;
      }

      public ProblemInstance(IRun run) {
        ProblemType = GetStringValueOrEmpty(run, "Problem Type");
        ProblemName = GetStringValueOrEmpty(run, "Problem Name");
        Evaluator = GetStringValueOrEmpty(run, "Evaluator");
        Maximization = GetBoolValueOrEmpty(run, "Maximization");
        DisplayProblemType = !string.IsNullOrEmpty(ProblemType);
        DisplayProblemName = !string.IsNullOrEmpty(ProblemName);
        DisplayEvaluator = !string.IsNullOrEmpty(Evaluator);
        DisplayMaximization = Maximization.HasValue;
        matchAll = false;
        BestKnownQuality = GetDoubleValueOrNaN(run, "BestKnownQuality");
      }

      private bool displayProblemType;
      public bool DisplayProblemType {
        get { return displayProblemType; }
        set {
          if (displayProblemType == value) return;
          displayProblemType = value;
          OnPropertyChanged("DisplayProblemType");
        }
      }
      private string problemType;
      public string ProblemType {
        get { return problemType; }
        set {
          if (problemType == value) return;
          problemType = value;
          OnPropertyChanged("ProblemType");
        }
      }
      private bool displayProblemName;
      public bool DisplayProblemName {
        get { return displayProblemName; }
        set {
          if (displayProblemName == value) return;
          displayProblemName = value;
          OnPropertyChanged("DisplayProblemName");
        }
      }
      private string problemName;
      public string ProblemName {
        get { return problemName; }
        set {
          if (problemName == value) return;
          problemName = value;
          OnPropertyChanged("ProblemName");
        }
      }
      private bool displayEvaluator;
      public bool DisplayEvaluator {
        get { return displayEvaluator; }
        set {
          if (displayEvaluator == value) return;
          displayEvaluator = value;
          OnPropertyChanged("DisplayEvaluator");
        }
      }
      private string evaluator;
      public string Evaluator {
        get { return evaluator; }
        set {
          if (evaluator == value) return;
          evaluator = value;
          OnPropertyChanged("Evaluator");
        }
      }
      private bool displayMaximization;
      public bool DisplayMaximization {
        get { return displayMaximization; }
        set {
          if (displayMaximization == value) return;
          displayMaximization = value;
          OnPropertyChanged("DisplayMaximization");
        }
      }
      private bool? maximization;
      public bool? Maximization {
        get { return maximization; }
        set {
          if (maximization == value) return;
          maximization = value;
          OnPropertyChanged("Maximization");
        }
      }
      private double bestKnownQuality;
      public double BestKnownQuality {
        get { return bestKnownQuality; }
        set {
          if (bestKnownQuality == value) return;
          bestKnownQuality = value;
          OnPropertyChanged("BestKnownQuality");
        }
      }

      public bool Match(IRun run) {
        return matchAll ||
               (GetStringValueOrEmpty(run, "Problem Type") == ProblemType
               && GetStringValueOrEmpty(run, "Problem Name") == ProblemName
               && GetStringValueOrEmpty(run, "Evaluator") == Evaluator
               && GetBoolValueOrEmpty(run, "Maximization") == Maximization);
      }

      private double GetDoubleValueOrNaN(IRun run, string key) {
        IItem param;
        if (run.Parameters.TryGetValue(key, out param)) {
          var dv = param as DoubleValue;
          return dv != null ? dv.Value : double.NaN;
        }
        return double.NaN;
      }

      private string GetStringValueOrEmpty(IRun run, string key) {
        IItem param;
        if (run.Parameters.TryGetValue(key, out param)) {
          var sv = param as StringValue;
          return sv != null ? sv.Value : string.Empty;
        }
        return string.Empty;
      }

      private bool? GetBoolValueOrEmpty(IRun run, string key) {
        IItem param;
        if (run.Parameters.TryGetValue(key, out param)) {
          var bv = param as BoolValue;
          if (bv != null) return bv.Value;
        }
        return null;
      }

      public override bool Equals(object obj) {
        var other = obj as ProblemInstance;
        if (other == null) return false;
        return ProblemType == other.ProblemType
               && ProblemName == other.ProblemName
               && Evaluator == other.Evaluator
               && Maximization == other.Maximization;
      }

      public override int GetHashCode() {
        return ProblemType.GetHashCode() ^ ProblemName.GetHashCode() ^ Evaluator.GetHashCode() ^ Maximization.GetHashCode();
      }

      public override string ToString() {
        return string.Join("  --  ", new[] {
          (DisplayProblemType ? ProblemType : string.Empty),
          (DisplayProblemName ? ProblemName : string.Empty),
          (DisplayEvaluator ? Evaluator : string.Empty),
          (DisplayMaximization && Maximization.HasValue ? (Maximization.Value ? "MAX" : "MIN") : string.Empty),
          !double.IsNaN(BestKnownQuality) ? BestKnownQuality.ToString(CultureInfo.CurrentCulture.NumberFormat) : string.Empty }.Where(x => !string.IsNullOrEmpty(x)));
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected virtual void OnPropertyChanged(string propertyName = null) {
        var handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private class CostComparer : Comparer<ConvergenceGraphPoint> {
      public override int Compare(ConvergenceGraphPoint x, ConvergenceGraphPoint y) {
        return x.Runlength.CompareTo(y.Runlength);
      }
    }

    private class TargetComparer : Comparer<ConvergenceGraphPoint> {
      public bool Maximization { get; private set; }
      public TargetComparer(bool maximization) {
        Maximization = maximization;
      }

      public override int Compare(ConvergenceGraphPoint x, ConvergenceGraphPoint y) {
        return Maximization ? x.Quality.CompareTo(y.Quality) : y.Quality.CompareTo(x.Quality);
      }
    }

    private class ConvergenceGraph : IEnumerable<ConvergenceGraphPoint> {
      private List<ConvergenceGraphPoint> data;
      private bool maximization;
      private string xAxisLabel;

      public string XAxisName { get { return xAxisLabel; } }

      public int Points {
        get { return data.Count; }
      }

      public double TotalRunlength {
        get { return data.Last().Runlength; }
      }

      public double BestQuality {
        get { return data.Last().Quality; }
      }

      public double QualityAt(double runlength) {
        var point = data.SkipWhile(x => x.Runlength < runlength).FirstOrDefault();
        if (point == null) return double.NaN;
        return point.Quality;
      }

      public double RunlengthFor(double quality) {
        var point = (maximization ? data.SkipWhile(x => x.Quality < quality) : data.SkipWhile(x => x.Quality > quality)).FirstOrDefault();
        if (point == null) return double.NaN;
        return point.Runlength;
      }

      public ConvergenceGraphPoint this[int point] {
        get { return data[point]; }
      }

      public ConvergenceGraph(IndexedDataTable<double> table, bool maximization) {
        data = table.Rows.First().Values.Select(x => new ConvergenceGraphPoint() { Runlength = x.Item1, Quality = x.Item2 }).ToList();
        xAxisLabel = table.VisualProperties.XAxisTitle;
        this.maximization = maximization;
      }

      public IEnumerator<ConvergenceGraphPoint> GetEnumerator() {
        return data.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator() {
        return data.GetEnumerator();
      }

      public int BinarySearch(ConvergenceGraphPoint item, IComparer<ConvergenceGraphPoint> comparer) {
        return data.BinarySearch(item, comparer);
      }

      public int BinarySearch(int index, int count, ConvergenceGraphPoint point, IComparer<ConvergenceGraphPoint> comparer) {
        return data.BinarySearch(index, count, point, comparer);
      }

      public IEnumerable<Tuple<double, double>> ToTuples() {
        return data.Select(x => Tuple.Create(x.Runlength, x.Quality));
      }
    }

    private class ConvergenceGraphPoint {
      public double Runlength { get; set; }
      public double Quality { get; set; }

      public ConvergenceGraphPoint() {

      }
      public ConvergenceGraphPoint(double runlength, double quality) {
        Runlength = runlength;
        Quality = quality;
      }
    }
    #endregion
  }
}
