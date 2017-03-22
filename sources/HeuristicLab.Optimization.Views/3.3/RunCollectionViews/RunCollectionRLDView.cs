#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
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

    private double[] targets;
    private double[] budgets;
    private bool targetsAreRelative = true;
    private readonly BindingList<ProblemInstance> problems;

    private bool suppressUpdates;
    private readonly IndexedDataTable<double> byCostDataTable;
    public IndexedDataTable<double> ByCostDataTable {
      get { return byCostDataTable; }
    }

    public RunCollectionRLDView() {
      InitializeComponent();
      invisibleTargetSeries = new List<Series>();

      targetChart.CustomizeAllChartAreas();
      targetChart.ChartAreas[0].CursorX.Interval = 1;
      targetChart.SuppressExceptions = true;
      byCostDataTable = new IndexedDataTable<double>("ECDF by Cost", "A data table containing the ECDF of each of a number of groups.") {
        VisualProperties = {
          YAxisTitle = "Proportion of unused budgets",
          YAxisMinimumFixedValue = 0,
          YAxisMinimumAuto = false,
          YAxisMaximumFixedValue = 1,
          YAxisMaximumAuto = false
        }
      };
      byCostViewHost.Content = byCostDataTable;
      suppressUpdates = true;
      relativeOrAbsoluteComboBox.SelectedItem = targetsAreRelative ? "relative" : "absolute";
      problems = new BindingList<ProblemInstance>();
      problemComboBox.DataSource = new BindingSource() { DataSource = problems };
      problemComboBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
      suppressUpdates = false;
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
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded), sender, e);
        return;
      }
      UpdateGroupAndProblemComboBox();
      UpdateDataTableComboBox();
      foreach (var run in e.Items)
        RegisterRunEvents(run);
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved), sender, e);
        return;
      }
      UpdateGroupAndProblemComboBox();
      UpdateDataTableComboBox();
      foreach (var run in e.Items)
        DeregisterRunEvents(run);
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset), sender, e);
        return;
      }
      UpdateGroupAndProblemComboBox();
      UpdateDataTableComboBox();
      foreach (var run in e.OldItems)
        DeregisterRunEvents(run);
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
      suppressUpdates = Content.UpdateOfRunsInProgress;
      if (!suppressUpdates) {
        UpdateDataTableComboBox();
        UpdateGroupAndProblemComboBox();
        UpdateRuns();
      }
    }

    private void RegisterRunEvents(IRun run) {
      run.PropertyChanged += run_PropertyChanged;
    }
    private void DeregisterRunEvents(IRun run) {
      run.PropertyChanged -= run_PropertyChanged;
    }
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired) {
        Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      } else {
        if (e.PropertyName == "Visible")
          UpdateRuns();
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
        UpdateGroupAndProblemComboBox();
        UpdateDataTableComboBox();
      }
    }


    private void UpdateGroupAndProblemComboBox() {
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

      var table = (string)dataTableComboBox.SelectedItem;
      var problemDict = CalculateBestTargetPerProblemInstance(table);

      var problemTypesDifferent = problemDict.Keys.Select(x => x.ProblemType).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
      var problemNamesDifferent = problemDict.Keys.Select(x => x.ProblemName).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
      var evaluatorDifferent = problemDict.Keys.Select(x => x.Evaluator).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count() > 1;
      var maximizationDifferent = problemDict.Keys.Select(x => x.Maximization).Distinct().Count() > 1;
      var allEqual = !problemTypesDifferent && !problemNamesDifferent && !evaluatorDifferent && !maximizationDifferent;

      var selectedProblemItem = (ProblemInstance)problemComboBox.SelectedItem;
      problemComboBox.DataSource = null;
      problemComboBox.Items.Clear();
      problems.Clear();
      problems.Add(ProblemInstance.MatchAll);
      problemComboBox.DataSource = new BindingSource() { DataSource = problems };
      problemComboBox.DataBindings.DefaultDataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
      if (problems[0].Equals(selectedProblemItem)) problemComboBox.SelectedItem = problems[0];
      foreach (var p in problemDict.ToList()) {
        p.Key.BestKnownQuality = p.Value;
        p.Key.DisplayProblemType = problemTypesDifferent;
        p.Key.DisplayProblemName = problemNamesDifferent || allEqual;
        p.Key.DisplayEvaluator = evaluatorDifferent;
        p.Key.DisplayMaximization = maximizationDifferent;
        problems.Add(p.Key);
        if (p.Key.Equals(selectedProblemItem)) problemComboBox.SelectedItem = p.Key;
      }
      SetEnabledStateOfControls();
    }

    private void UpdateDataTableComboBox() {
      string selectedItem = (string)dataTableComboBox.SelectedItem;

      dataTableComboBox.Items.Clear();
      var dataTables = (from run in Content
                        from result in run.Results
                        where result.Value is IndexedDataTable<double>
                        select result.Key).Distinct().ToArray();

      dataTableComboBox.Items.AddRange(dataTables);
      if (selectedItem != null && dataTableComboBox.Items.Contains(selectedItem)) {
        dataTableComboBox.SelectedItem = selectedItem;
      } else if (dataTableComboBox.Items.Count > 0) {
        dataTableComboBox.SelectedItem = dataTableComboBox.Items[0];
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      groupComboBox.Enabled = Content != null;
      problemComboBox.Enabled = Content != null && problemComboBox.Items.Count > 1;
      dataTableComboBox.Enabled = Content != null && dataTableComboBox.Items.Count > 1;
      addTargetsAsResultButton.Enabled = Content != null && targets != null && dataTableComboBox.SelectedIndex >= 0;
      addBudgetsAsResultButton.Enabled = Content != null && budgets != null && dataTableComboBox.SelectedIndex >= 0;
    }

    private IEnumerable<AlgorithmInstance> GroupRuns() {
      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) yield break;

      var selectedGroup = (string)groupComboBox.SelectedItem;
      if (string.IsNullOrEmpty(selectedGroup)) yield break;

      var selectedProblem = (ProblemInstance)problemComboBox.SelectedItem;
      if (selectedProblem == null) yield break;
      
      foreach (var x in (from r in Content
                         where (selectedGroup == AllInstances || r.Parameters.ContainsKey(selectedGroup))
                           && selectedProblem.Match(r)
                           && r.Results.ContainsKey(table)
                           && r.Visible
                         let key = selectedGroup == AllInstances ? AllInstances : r.Parameters[selectedGroup].ToString()
                         group r by key into g
                         select new AlgorithmInstance(g.Key, g, problems))) {
        yield return x;
      }
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

      var algInstances = GroupRuns().ToList();
      if (algInstances.Count == 0) return;

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
      foreach (var alg in algInstances) {
        var noRuns = 0;
        SortedList<double, int> epdfHits = null, epdfMisses = null;
        if (aggregate) {
          hits[alg.Name] = epdfHits = new SortedList<double, int>();
          misses[alg.Name] = epdfMisses = new SortedList<double, int>();
        }
        foreach (var problem in alg.GetProblemInstances()) {
          var max = problem.IsMaximization();
          var absTargets = GetAbsoluteTargets(problem).ToArray();
          foreach (var run in alg.GetRuns(problem)) {
            noRuns++;
            var resultsTable = (IndexedDataTable<double>)run.Results[table];
            xAxisTitles.Add(resultsTable.VisualProperties.XAxisTitle);

            var efforts = absTargets.Select(t => GetEffortToHitTarget(resultsTable.Rows.First().Values, t, max)).ToArray();
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

      UpdateErtTables(algInstances);
    }

    private void DrawTargetsEcdf(Dictionary<string, SortedList<double, int>> hits, Dictionary<string, SortedList<double, int>> misses, Dictionary<string, int> noRuns) {
      var colorCount = 0;
      var lineStyleCount = 0;
      
      var showMarkers = markerCheckBox.Checked;
      foreach (var list in hits) {
        var row = new Series(list.Key) {
          ChartType = SeriesChartType.StepLine,
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
          while (moreMisses && iter.Current.Key <= h.Key) {
            if (!labelPrinted && row.Points.Count > 0) {
              var point = row.Points.Last();
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
              row.Points.Last().Color = Color.FromArgb(255 - (int)Math.Floor(255 * (prevmissedecdf / totalTargets)), colors[colorCount]);
            } else {
              var dp = new DataPoint(iter.Current.Key, ecdf / movingTargets) {
                Color = Color.FromArgb(255 - (int)Math.Floor(255 * (prevmissedecdf / totalTargets)), colors[colorCount])
              };
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
            row.Points.Last().Color = Color.FromArgb(255 - (int)Math.Floor(255 * (missedecdf / totalTargets)), colors[colorCount]);
          } else {
            var dp = new DataPoint(h.Key, ecdf / movingTargets) {
              Color = Color.FromArgb(255 - (int)Math.Floor(255 * (missedecdf / totalTargets)), colors[colorCount])
            };
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
          var dp = new DataPoint(iter.Current.Key, ecdf / movingTargets) {
            Color = Color.FromArgb(255 - (int)Math.Floor(255 * (missedecdf / totalTargets)), colors[colorCount])
          };
          if (showMarkers) {
            dp.MarkerStyle = MarkerStyle.Circle;
            dp.MarkerBorderWidth = 1;
            dp.MarkerSize = 5;
          }
          row.Points.Add(dp);
          if (boundShadingCheckBox.Checked) {
            rowShade.Points.Add(new DataPoint(iter.Current.Key, new[] {ecdf / totalTargets, (ecdf + missedecdf) / totalTargets}));
          }
          moreMisses = iter.MoveNext();

          if (!labelPrinted && row.Points.Count > 0) {
            var point = row.Points.Last();
            point.Label = row.Name;
            point.MarkerStyle = MarkerStyle.Cross;
            point.MarkerBorderWidth = 1;
            point.MarkerSize = 10;
            labelPrinted = true;
          }
        }

        if (!labelPrinted) {
          var point = row.Points.Last();
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
      var maximization = pInstance.IsMaximization();
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
      if (double.IsNaN(pInstance.BestKnownQuality)) throw new ArgumentException("Problem instance does not have a defined best-known quality.");
      var absTargets = GetAbsoluteTargets(pInstance);
      return (pInstance.IsMaximization()
        ? absTargets.OrderBy(x => x) : absTargets.OrderByDescending(x => x)).ToArray();
    }

    private void GenerateDefaultTargets() {
      targets = new[] { 0.1, 0.095, 0.09, 0.085, 0.08, 0.075, 0.07, 0.065, 0.06, 0.055, 0.05, 0.045, 0.04, 0.035, 0.03, 0.025, 0.02, 0.015, 0.01, 0.005, 0 };
      suppressTargetsEvents = true;
      targetsTextBox.Text = string.Join("% ; ", targets.Select(x => x * 100)) + "%";
      suppressTargetsEvents = false;
    }

    private Tuple<bool, double> GetEffortToHitTarget(
        IEnumerable<Tuple<double, double>> convergenceGraph,
        double absTarget, bool maximization) {
      var hit = false;
      var effort = double.NaN;
      foreach (var dent in convergenceGraph) {
        effort = dent.Item1;
        hit = maximization && dent.Item2 >= absTarget || !maximization && dent.Item2 <= absTarget;
        if (hit) break;
      }
      if (double.IsNaN(effort)) throw new ArgumentException("Convergence graph is empty.", "convergenceGraph");
      return Tuple.Create(hit, effort);
    }

    private void UpdateErtTables(List<AlgorithmInstance> algorithmInstances) {
      ertTableView.Content = null;
      var columns = 1 + targets.Length + 1;
      var matrix = new string[algorithmInstances.Count * algorithmInstances.Max(x => x.GetNumberOfProblemInstances()) + algorithmInstances.Max(x => x.GetNumberOfProblemInstances()), columns];
      var rowCount = 0;

      var tableName = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(tableName)) return;
      
      var problems = algorithmInstances.SelectMany(x => x.GetProblemInstances()).Distinct().ToList();

      foreach (var problem in problems) {
        var max = problem.IsMaximization();
        matrix[rowCount, 0] = problem.ToString();
        var absTargets = GetAbsoluteTargetsWorstToBest(problem);
        for (var i = 0; i < absTargets.Length; i++) {
          matrix[rowCount, i + 1] = absTargets[i].ToString(CultureInfo.CurrentCulture.NumberFormat);
        }
        matrix[rowCount, columns - 1] = "#succ";
        rowCount++;

        foreach (var alg in algorithmInstances) {
          matrix[rowCount, 0] = alg.Name;
          var runs = alg.GetRuns(problem).ToList();
          if (runs.Count == 0) {
            matrix[rowCount, columns - 1] = "N/A";
            rowCount++;
            continue;
          }
          var result = default(ErtCalculationResult);
          for (var i = 0; i < absTargets.Length; i++) {
            result = ExpectedRuntimeHelper.CalculateErt(runs, tableName, absTargets[i], max);
            matrix[rowCount, i + 1] = result.ToString();
          }
          matrix[rowCount, columns - 1] = targets.Length > 0 ? result.SuccessfulRuns + "/" + result.TotalRuns : "-";
          rowCount++;
        }
      }
      ertTableView.Content = new StringMatrix(matrix);
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

      var algInstances = GroupRuns().ToList();
      if (algInstances.Count == 0) return;

      var colorCount = 0;
      var lineStyleCount = 0;
      
      foreach (var alg in algInstances) {
        var hits = new Dictionary<string, SortedList<double, double>>();

        foreach (var problem in alg.GetProblemInstances()) {
          foreach (var run in alg.GetRuns(problem)) {
            var resultsTable = (IndexedDataTable<double>)run.Results[table];

            if (aggregateBudgetsCheckBox.Checked) {
              CalculateHitsForAllBudgets(hits, resultsTable.Rows.First(), alg.GetNumberOfProblemInstances(), problem, alg.Name, alg.GetNumberOfRuns(problem));
            } else {
              CalculateHitsForEachBudget(hits, resultsTable.Rows.First(), alg.GetNumberOfProblemInstances(), problem, alg.Name, alg.GetNumberOfRuns(problem));
            }
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
          foreach (var h in list.Value) {
            total += h.Value;
            row.Values.Add(Tuple.Create(h.Key, total));
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
      var runs = Content;
      var min = runs.Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Select(y => y.Item1).Min()).Min();
      var max = runs.Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Select(y => y.Item1).Max()).Max();
      var points = 20;
      budgets = Enumerable.Range(1, points).Select(x => min + (x / (double)points) * (max - min)).ToArray();
      suppressBudgetsEvents = true;
      budgetsTextBox.Text = string.Join(" ; ", budgets);
      suppressBudgetsEvents = false;
    }

    private void CalculateHitsForEachBudget(Dictionary<string, SortedList<double, double>> hits, IndexedDataRow<double> row, int groupCount, ProblemInstance problem, string groupName, int problemCount) {
      var max = problem.IsMaximization();
      foreach (var b in budgets) {
        var key = groupName + "-" + b;
        if (!hits.ContainsKey(key)) hits.Add(key, new SortedList<double, double>());
        Tuple<double, double> prev = null;
        foreach (var v in row.Values) {
          if (v.Item1 >= b) {
            // the budget may be too low to achieve any target
            if (prev == null && v.Item1 != b) break;
            var tgt = ((prev == null || v.Item1 == b) ? v.Item2 : prev.Item2);
            var relTgt = CalculateRelativeDifference(max, problem.BestKnownQuality, tgt) + 1;
            if (hits[key].ContainsKey(relTgt))
              hits[key][relTgt] += 1.0 / (groupCount * problemCount);
            else hits[key][relTgt] = 1.0 / (groupCount * problemCount);
            break;
          }
          prev = v;
        }
        if (hits[key].Count == 0) hits.Remove(key);
      }
    }

    private void CalculateHitsForAllBudgets(Dictionary<string, SortedList<double, double>> hits, IndexedDataRow<double> row, int groupCount, ProblemInstance problem, string groupName, int problemCount) {
      var values = row.Values;
      if (!hits.ContainsKey(groupName)) hits.Add(groupName, new SortedList<double, double>());

      var i = 0;
      var j = 0;
      Tuple<double, double> prev = null;
      var max = problem.IsMaximization();
      while (i < budgets.Length && j < values.Count) {
        var current = values[j];
        if (current.Item1 >= budgets[i]) {
          if (prev != null || current.Item1 == budgets[i]) {
            var tgt = (prev == null || current.Item1 == budgets[i]) ? current.Item2 : prev.Item2;
            var relTgt = CalculateRelativeDifference(max, problem.BestKnownQuality, tgt) + 1;
            if (!hits[groupName].ContainsKey(relTgt)) hits[groupName][relTgt] = 0;
            hits[groupName][relTgt] += 1.0 / (groupCount * problemCount * budgets.Length);
          }
          i++;
        } else {
          j++;
          prev = current;
        }
      }
      var lastTgt = values.Last().Item2;
      var lastRelTgt = CalculateRelativeDifference(max, problem.BestKnownQuality, lastTgt) + 1;
      if (i < budgets.Length && !hits[groupName].ContainsKey(lastRelTgt)) hits[groupName][lastRelTgt] = 0;
      while (i < budgets.Length) {
        hits[groupName][lastRelTgt] += 1.0 / (groupCount * problemCount * budgets.Length);
        i++;
      }
    }
    #endregion

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " RLD View" : ViewAttribute.GetViewName(GetType());
    }

    private void groupComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateRuns();
      SetEnabledStateOfControls();
    }
    private void problemComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateRuns();
      SetEnabledStateOfControls();
    }
    private void dataTableComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (dataTableComboBox.SelectedIndex >= 0)
        GenerateDefaultBudgets((string)dataTableComboBox.SelectedItem);
      UpdateBestKnownQualities();
      UpdateRuns();
      SetEnabledStateOfControls();
    }

    private void logScalingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateResultsByTarget();
      byCostDataTable.VisualProperties.XAxisLogScale = byCostDataTable.Rows.Count > 0 && budgetLogScalingCheckBox.Checked;
    }

    private void boundShadingCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateResultsByTarget();
    }

    #region Event handlers for target analysis
    private bool suppressTargetsEvents;
    private void targetsTextBox_Validating(object sender, CancelEventArgs e) {
      if (suppressTargetsEvents) return;
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
      suppressTargetsEvents = true;
      try {
        if (targetsAreRelative)
          targetsTextBox.Text = string.Join("% ; ", targets.Select(x => x * 100)) + "%";
        else targetsTextBox.Text = string.Join(" ; ", targets);
      } finally { suppressTargetsEvents = false; }

      UpdateResultsByTarget();
      SetEnabledStateOfControls();
    }

    private void aggregateTargetsCheckBox_CheckedChanged(object sender, EventArgs e) {
      SuspendRepaint();
      try {
        UpdateResultsByTarget();
      } finally { ResumeRepaint(true); }
    }

    private void relativeOrAbsoluteComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (suppressUpdates) return;
      var pd = (ProblemInstance)problemComboBox.SelectedItem;
      if (!double.IsNaN(pd.BestKnownQuality)) {
        var max = pd.IsMaximization();
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
      SuspendRepaint();
      suppressTargetsEvents = true;
      try {
        if (targetsAreRelative)
          targetsTextBox.Text = string.Join("% ; ", targets.Select(x => x * 100)) + "%";
        else targetsTextBox.Text = string.Join(" ; ", targets);

        UpdateResultsByTarget();
      } finally { suppressTargetsEvents = false; ResumeRepaint(true); }
    }

    private void generateTargetsButton_Click(object sender, EventArgs e) {
      decimal max = 1, min = 0, count = 10;
      if (targets != null) {
        max = (decimal)targets.Max();
        min = (decimal)targets.Min();
        count = targets.Length;
      } else if (Content.Count > 0 && dataTableComboBox.SelectedIndex >= 0) {
        var table = (string)dataTableComboBox.SelectedItem;
        max = (decimal)Content.Where(x => x.Results.ContainsKey(table)).Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Max(y => y.Item2)).Max();
        min = (decimal)Content.Where(x => x.Results.ContainsKey(table)).Select(x => ((IndexedDataTable<double>)x.Results[table]).Rows.First().Values.Min(y => y.Item2)).Min();
        count = 6;
      }
      using (var dialog = new DefineArithmeticProgressionDialog(false, min, max, (max - min) / count)) {
        if (dialog.ShowDialog() == DialogResult.OK) {
          if (dialog.Values.Any()) {
            targets = dialog.Values.Select(x => (double)x).ToArray();
            suppressTargetsEvents = true;
            if (targetsAreRelative)
              targetsTextBox.Text = string.Join("% ; ", targets);
            else targetsTextBox.Text = string.Join(" ; ", targets);
            suppressTargetsEvents = false;

            UpdateResultsByTarget();
            SetEnabledStateOfControls();
          }
        }
      }
    }

    private void addTargetsAsResultButton_Click(object sender, EventArgs e) {
      var table = (string)dataTableComboBox.SelectedItem;
      if (string.IsNullOrEmpty(table)) return;
      
      foreach (var run in Content) {
        if (!run.Results.ContainsKey(table)) continue;
        var resultsTable = (IndexedDataTable<double>)run.Results[table];
        var values = resultsTable.Rows.First().Values;
        var i = 0;
        var j = 0;
        var pd = new ProblemInstance(run);
        var max = pd.IsMaximization();
        var absTargets = GetAbsoluteTargetsWorstToBest(problems.Single(x => x.Equals(pd)));
        while (i < absTargets.Length && j < values.Count) {
          var target = absTargets[i];
          var current = values[j];
          if (max && current.Item2 >= target || !max && current.Item2 <= target) {
            run.Results[table + ".Target" + target] = new DoubleValue(current.Item1);
            i++;
          } else {
            j++;
          }
        }
      }
    }

    private void markerCheckBox_CheckedChanged(object sender, EventArgs e) {
      SuspendRepaint();
      try {
        UpdateResultsByTarget();
      } finally { ResumeRepaint(true); }
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
          errorProvider.SetError(budgetsTextBox, "Not all targets can be parsed: " + ts);
          e.Cancel = true;
          return;
        }
        budgetList.Add(b);
      }
      if (budgetList.Count == 0) {
        errorProvider.SetError(budgetsTextBox, "Give at least one target value!");
        e.Cancel = true;
        return;
      }
      e.Cancel = false;
      errorProvider.SetError(budgetsTextBox, null);
      budgets = budgetList.ToArray();
      UpdateResultsByCost();
      SetEnabledStateOfControls();
    }

    private void aggregateBudgetsCheckBox_CheckedChanged(object sender, EventArgs e) {
      SuspendRepaint();
      try {
        UpdateResultsByCost();
      } finally { ResumeRepaint(true); }
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
        count = 6;
      }
      using (var dialog = new DefineArithmeticProgressionDialog(false, min, max, (max - min) / count)) {
        if (dialog.ShowDialog() == DialogResult.OK) {
          if (dialog.Values.Any()) {
            budgets = dialog.Values.OrderBy(x => x).Select(x => (double)x).ToArray();

            suppressBudgetsEvents = true;
            budgetsTextBox.Text = string.Join(" ; ", budgets);
            suppressBudgetsEvents = false;

            UpdateResultsByCost();
            SetEnabledStateOfControls();
          }
        }
      }
    }

    private void addBudgetsAsResultButton_Click(object sender, EventArgs e) {
      var table = (string)dataTableComboBox.SelectedItem;
      var budgetStrings = budgetsTextBox.Text.Split(new[] { ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      if (budgetStrings.Length == 0) {
        MessageBox.Show("Define a number of budgets.");
        return;
      }
      var budgetList = new List<double>();
      foreach (var bs in budgetStrings) {
        double v;
        if (!double.TryParse(bs, out v)) {
          MessageBox.Show("Budgets must be a valid number: " + bs);
          return;
        }
        budgetList.Add(v);
      }
      budgetList.Sort();

      foreach (var run in Content) {
        if (!run.Results.ContainsKey(table)) continue;
        var resultsTable = (IndexedDataTable<double>)run.Results[table];
        var values = resultsTable.Rows.First().Values;
        var i = 0;
        var j = 0;
        Tuple<double, double> prev = null;
        while (i < budgetList.Count && j < values.Count) {
          var current = values[j];
          if (current.Item1 >= budgetList[i]) {
            if (prev != null || current.Item1 == budgetList[i]) {
              var tgt = (prev == null || current.Item1 == budgetList[i]) ? current.Item2 : prev.Item2;
              run.Results[table + ".Cost" + budgetList[i]] = new DoubleValue(tgt);
            }
            i++;
          } else {
            j++;
            prev = current;
          }
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
      return (from r in Content
              where r.Visible
              let pd = new ProblemInstance(r)
              let target = r.Parameters.ContainsKey("BestKnownQuality")
                           && r.Parameters["BestKnownQuality"] is DoubleValue
                ? ((DoubleValue)r.Parameters["BestKnownQuality"]).Value
                : (r.Results.ContainsKey(table) && r.Results[table] is IndexedDataTable<double>
                  ? ((IndexedDataTable<double>)r.Results[table]).Rows.First().Values.Last().Item2
                  : double.NaN)
              group target by pd into g
              select new { Problem = g.Key, Target = g.Any(x => !double.IsNaN(x)) ? (g.Key.IsMaximization() ? g.Max() : g.Where(x => !double.IsNaN(x)).Min()) : double.NaN })
        .ToDictionary(x => x.Problem, x => x.Target);
    }

    private void UpdateRuns() {
      if (InvokeRequired) {
        Invoke((Action)UpdateRuns);
        return;
      }
      SuspendRepaint();
      try {
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
      //problemComboBox.ResetBindings();
    }
    #endregion

    private void ConfigureSeries(Series series) {
      series.SmartLabelStyle.Enabled = true;
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

      private Dictionary<ProblemInstance, List<IRun>> performanceData;

      public int GetNumberOfProblemInstances() {
        return performanceData.Count;
      }

      public IEnumerable<ProblemInstance> GetProblemInstances() {
        return performanceData.Keys;
      }

      public int GetNumberOfRuns(ProblemInstance p) {
        if (p == ProblemInstance.MatchAll) return performanceData.Select(x => x.Value.Count).Sum();
        List<IRun> runs;
        if (performanceData.TryGetValue(p, out runs))
          return runs.Count;

        return 0;
      }

      public IEnumerable<IRun> GetRuns(ProblemInstance p) {
        if (p == ProblemInstance.MatchAll) return performanceData.SelectMany(x => x.Value);
        List<IRun> runs;
        if (performanceData.TryGetValue(p, out runs))
          return runs;

        return Enumerable.Empty<IRun>();
      }

      public AlgorithmInstance(string name, IEnumerable<IRun> runs, IEnumerable<ProblemInstance> problems) {
        this.name = name;

        var pDict = problems.ToDictionary(r => r, _ => new List<IRun>());
        foreach (var y in runs) {
          var pd = new ProblemInstance(y);
          List<IRun> l;
          if (pDict.TryGetValue(pd, out l)) l.Add(y);
        }
        performanceData = pDict.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
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
        Maximization = string.Empty;
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
        Maximization = GetMaximizationValueOrEmpty(run, "Maximization");
        DisplayProblemType = !string.IsNullOrEmpty(ProblemType);
        DisplayProblemName = !string.IsNullOrEmpty(ProblemName);
        DisplayEvaluator = !string.IsNullOrEmpty(Evaluator);
        DisplayMaximization = !string.IsNullOrEmpty(Maximization);
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
      private string maximization;
      public string Maximization {
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

      public bool IsMaximization() {
        return Maximization == "MAX";
      }

      public bool Match(IRun run) {
        return matchAll ||
               GetStringValueOrEmpty(run, "Problem Type") == ProblemType
               && GetStringValueOrEmpty(run, "Problem Name") == ProblemName
               && GetStringValueOrEmpty(run, "Evaluator") == Evaluator
               && GetMaximizationValueOrEmpty(run, "Maximization") == Maximization;
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

      private string GetMaximizationValueOrEmpty(IRun run, string key) {
        IItem param;
        if (run.Parameters.TryGetValue(key, out param)) {
          var bv = param as BoolValue;
          return bv != null ? (bv.Value ? "MAX" : "MIN") : string.Empty;
        }
        return string.Empty;
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
          (DisplayMaximization ? Maximization : string.Empty),
          !double.IsNaN(BestKnownQuality) ? BestKnownQuality.ToString(CultureInfo.CurrentCulture.NumberFormat) : string.Empty }.Where(x => !string.IsNullOrEmpty(x)));
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected virtual void OnPropertyChanged(string propertyName = null) {
        var handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
