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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Analysis;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text;
using HeuristicLab.Data;
using System.Drawing;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization.Views {
  [View("Parameter Analysis")]
  [Content(typeof(RunCollection), false)]
  public sealed partial class RunCollectionParameterAnalysisView : ItemView {
    #region Colors
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
    #endregion

    private bool suppressUpdates = false;
    private int stepSize = 1000;
    private Dictionary<IRun, List<Tuple<int, double>>> runData;
    private Dictionary<string, ParameterInfo> paramInfos;

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }

    public RunCollectionParameterAnalysisView() {
      InitializeComponent();
      chart.CustomizeAllChartAreas();
      stepSizeTextBox.Text = stepSize.ToString();
      errorProvider.SetIconAlignment(stepSizeTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(stepSizeTextBox, 2);
    }

    #region Content Events
    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= Content_ItemsAdded;
      Content.ItemsRemoved -= Content_ItemsRemoved;
      Content.CollectionReset -= Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged -= Content_UpdateOfRunsInProgressChanged;
      DeregisterRunEvents(Content);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += Content_ItemsAdded;
      Content.ItemsRemoved += Content_ItemsRemoved;
      Content.CollectionReset += Content_CollectionReset;
      Content.UpdateOfRunsInProgressChanged += Content_UpdateOfRunsInProgressChanged;
      RegisterRunEvents(Content);
    }
    private void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (var run in runs)
        run.PropertyChanged -= Run_PropertyChanged;
    }
    private void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (var run in runs)
        run.PropertyChanged += Run_PropertyChanged;
    }

    private void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      RegisterRunEvents(e.Items);
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.Items);
    }
    private void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.OldItems);
      RegisterRunEvents(e.Items);
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs>)Content_UpdateOfRunsInProgressChanged, sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (suppressUpdates) return;
        CollectRunData();
        AnalyzeParameters();
        UpdateChart();
      }
    }
    private void Run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        Invoke((Action<object, PropertyChangedEventArgs>)Run_PropertyChanged, sender, e);
      else {
        CollectRunData();
        AnalyzeParameters();
        UpdateChart();
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      CollectRunData();
      AnalyzeParameters();
      UpdateChart();
    }
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      parametersGroupBox.Enabled = Content != null;
      groupsGroupBox.Enabled = Content != null;
      stepSizeTextBox.Enabled = Content != null;
      logScalingCheckBox.Enabled = Content != null;
      dataRowsGroupBox.Enabled = Content != null;
    }

    private void AnalyzeParameters() {
      paramInfos = new Dictionary<string, ParameterInfo>();
      if (Content == null) return;

      foreach (var run in runData.Keys) {
        foreach (var param in run.Parameters) {
          ParameterInfo info;
          if (!paramInfos.TryGetValue(param.Key, out info))
            paramInfos.Add(param.Key, new ParameterInfo(param.Key, param.Value.ToString(), run));
          else
            info.AddValue(param.Value.ToString(), run);
        }
      }

      // remove irrelevant parameters
      if (paramInfos.ContainsKey("Seed"))
        paramInfos.Remove("Seed");

      // remove all parameters which only have a single value
      var singles = new List<string>();
      foreach (var paramInfo in paramInfos.Values) {
        if (paramInfo.Values.Count == 1)
          singles.Add(paramInfo.Name);
      }
      foreach (var single in singles)
        paramInfos.Remove(single);

      // set color of parameter values
      int i = 0;
      foreach (var valueInfo in paramInfos.Values.SelectMany(x => x.Values.Values)) {
        valueInfo.Color = colors[i];
        i = (i + 1) % colors.Length;
      }

      // populate parametersTreeView
      parametersTreeView.Nodes.Clear();
      var paramsRoot = new TreeNode("All Runs (" + runData.Keys.Count + ")");
      paramsRoot.Tag = runData.Keys.ToList();
      foreach (var paramInfo in paramInfos.Values.OrderBy(x => x.Name)) {
        var node = new TreeNode(paramInfo.Name + " (" + paramInfo.RunCount + ")");
        foreach (var value in paramInfo.Values) {
          var child = new TreeNode(value.Key + " (" + value.Value.RunCount + ")");
          child.Tag = value.Value;
          node.Nodes.Add(child);
        }
        node.Tag = paramInfo;
        paramsRoot.Nodes.Add(node);
      }
      parametersTreeView.Nodes.Add(paramsRoot);
      paramsRoot.Expand();
      parametersTreeView.SelectedNode = paramsRoot;

      // populate groupsTreeView
      groupsTreeView.Nodes.Clear();
      var groupsRoot = new TreeNode("All Runs (" + runData.Keys.Count + ")");
      groupsRoot.Tag = runData.Keys.ToList();
      groupsTreeView.Nodes.Add(groupsRoot);
      groupsRoot.Expand();
      groupsTreeView.SelectedNode = groupsRoot;
    }
    private void CollectRunData() {
      runData = new Dictionary<IRun, List<Tuple<int, double>>>();
      if (Content == null) return;

      foreach (var run in Content) {
        try {
          IList<Tuple<double, double>> values = (run.Results["QualityPerEvaluations"] as IndexedDataTable<double>).Rows["First-hit Graph"].Values;
          var bestKnown = (run.Results["BestKnownQuality"] as DoubleValue).Value;

          var data = new List<Tuple<int, double>>();
          int i = 0;
          double qual = double.NaN;
          foreach (var val in values) {
            while (i * stepSize < val.Item1) {
              if (!double.IsNaN(qual)) data.Add(Tuple.Create(i * stepSize, qual));
              i++;
            }
            var diff = Math.Abs(bestKnown - val.Item2);
            qual = bestKnown == 0 ? diff : diff / bestKnown;
          }
          runData.Add(run, data);
        }
        catch {
        }
      }
    }

    private void UpdateChart() {
      if (suppressUpdates) return;

      var selectedNode = groupsTreeView.Focused ? groupsTreeView.SelectedNode : parametersTreeView.SelectedNode;
      chart.Series.Clear();
      chart.Legends[0].CustomItems.Clear();
      if (selectedNode.Parent == null) {
        var series = BuildSeries(selectedNode.Tag as IEnumerable<IRun>, colors[0]);
        chart.Titles[0].Text = selectedNode.Text;
        foreach (var s in series)
          chart.Series.Add(s);
        var legendItem = new LegendItem();
        var legendItemInfo = new LegendItemInfo(colors[0], series);
        legendItem.Color = legendItemInfo.Color;
        legendItem.BorderColor = Color.Transparent;
        legendItem.Name = selectedNode.Text;
        legendItem.Tag = legendItemInfo;
        chart.Legends[0].CustomItems.Add(legendItem);
      } else if (selectedNode.Tag is ParameterInfo) {
        var paramInfo = selectedNode.Tag as ParameterInfo;
        chart.Titles[0].Text = paramInfo.Name + " (" + paramInfo.RunCount + ")";
        foreach (var value in paramInfo.Values) {
          var series = BuildSeries(value.Value.Runs, value.Value.Color);
          foreach (var s in series)
            chart.Series.Add(s);
          var legendItem = new LegendItem();
          var legendItemInfo = new LegendItemInfo(value.Value.Color, series);
          legendItem.Color = legendItemInfo.Color;
          legendItem.BorderColor = Color.Transparent;
          legendItem.Name = value.Key + " (" + value.Value.RunCount + ")";
          legendItem.Tag = legendItemInfo;
          chart.Legends[0].CustomItems.Add(legendItem);
        }
      } else if (selectedNode.Tag is ParameterValueInfo) {
        var valueInfo = selectedNode.Tag as ParameterValueInfo;
        var series = BuildSeries(valueInfo.Runs, valueInfo.Color);
        chart.Titles[0].Text = selectedNode.Parent.Text;
        foreach (var s in series)
          chart.Series.Add(s);
        var legendItem = new LegendItem();
        var legendItemInfo = new LegendItemInfo(valueInfo.Color, series);
        legendItem.Color = legendItemInfo.Color;
        legendItem.BorderColor = Color.Transparent;
        legendItem.Name = selectedNode.Text;
        legendItem.Tag = legendItemInfo;
        chart.Legends[0].CustomItems.Add(legendItem);
      } else if (selectedNode.Tag is GroupInfo) {
        var groupInfo = selectedNode.Tag as GroupInfo;
        if (groupInfo.IsParameter) {
          chart.Titles[0].Text = groupInfo.Text + " (" + groupInfo.Runs.Count + ")";
          foreach (TreeNode node in selectedNode.Nodes) {
            var childInfo = node.Tag as GroupInfo;
            var series = BuildSeries(childInfo.Runs, childInfo.Color);
            foreach (var s in series)
              chart.Series.Add(s);
            var legendItem = new LegendItem();
            var legendItemInfo = new LegendItemInfo(childInfo.Color, series);
            legendItem.Color = legendItemInfo.Color;
            legendItem.BorderColor = Color.Transparent;
            legendItem.Name = childInfo.Text + " (" + childInfo.Runs.Count + ")";
            legendItem.Tag = legendItemInfo;
            chart.Legends[0].CustomItems.Add(legendItem);
          }
        } else {
          var parentInfo = selectedNode.Parent.Tag as GroupInfo;
          chart.Titles[0].Text = parentInfo.Text + " (" + parentInfo.Runs.Count + ")";
          var series = BuildSeries(groupInfo.Runs, groupInfo.Color);
          foreach (var s in series)
            chart.Series.Add(s);
          var legendItem = new LegendItem();
          var legendItemInfo = new LegendItemInfo(groupInfo.Color, series);
          legendItem.Color = legendItemInfo.Color;
          legendItem.BorderColor = Color.Transparent;
          legendItem.Name = groupInfo.Text + " (" + groupInfo.Runs.Count + ")";
          legendItem.Tag = legendItemInfo;
          chart.Legends[0].CustomItems.Add(legendItem);
        }
      }
    }
    private void UpdateSeriesVisibility() {
      foreach (var legendItem in chart.Legends[0].CustomItems) {
        var legendItemInfo = legendItem.Tag as LegendItemInfo;
        foreach (var s in legendItemInfo.Series) {
          if (legendItemInfo.SeriesVisible) {
            var seriesInfo = s.Tag as SeriesInfo;
            switch (seriesInfo.Type) {
              case SeriesTypes.MinMax:
                s.Color = minMaxCheckBox.Checked ? seriesInfo.Color : Color.Transparent;
                break;
              case SeriesTypes.Quartiles:
                s.Color = quartilesCheckBox.Checked ? seriesInfo.Color : Color.Transparent;
                break;
              case SeriesTypes.Median:
                s.Color = medianCheckBox.Checked ? seriesInfo.Color : Color.Transparent;
                break;
              case SeriesTypes.Average:
                s.Color = averageCheckBox.Checked ? seriesInfo.Color : Color.Transparent;
                break;
            }
          } else {
            s.Color = Color.Transparent;
          }
        }
      }
    }

    private IEnumerable<Series> BuildSeries(IEnumerable<IRun> runs, Color color) {
      var values = new Dictionary<int, List<double>>();
      foreach (var run in runs) {
        foreach (var step in runData[run]) {
          List<double> vals;
          if (!values.TryGetValue(step.Item1, out vals)) {
            vals = new List<double>();
            values.Add(step.Item1, vals);
          }
          vals.Add(step.Item2);
        }
      }

      List<Series> series = new List<Series>();
      var minMaxSeries = new Series();
      var minMaxSeriesInfo = new SeriesInfo(Color.FromArgb(25, color), SeriesTypes.MinMax);
      minMaxSeries.ChartType = SeriesChartType.Range;
      minMaxSeries.Color = minMaxCheckBox.Checked ? minMaxSeriesInfo.Color : Color.Transparent;
      minMaxSeries.IsVisibleInLegend = false;
      minMaxSeries.Tag = minMaxSeriesInfo;
      series.Add(minMaxSeries);

      var quartilesSeries = new Series();
      var quartilesSeriesInfo = new SeriesInfo(Color.FromArgb(50, color), SeriesTypes.Quartiles);
      quartilesSeries.ChartType = SeriesChartType.Range;
      quartilesSeries.Color = quartilesCheckBox.Checked ? quartilesSeriesInfo.Color : Color.Transparent;
      quartilesSeries.IsVisibleInLegend = false;
      quartilesSeries.Tag = quartilesSeriesInfo;
      series.Add(quartilesSeries);

      var medianSeries = new Series();
      var medianSeriesInfo = new SeriesInfo(color, SeriesTypes.Median);
      medianSeries.ChartType = SeriesChartType.FastLine;
      medianSeries.Color = medianCheckBox.Checked ? medianSeriesInfo.Color : Color.Transparent;
      medianSeries.BorderWidth = 3;
      medianSeries.BorderDashStyle = ChartDashStyle.Solid;
      medianSeries.IsVisibleInLegend = false;
      medianSeries.Tag = medianSeriesInfo;
      series.Add(medianSeries);

      var averageSeries = new Series();
      var averageSeriesInfo = new SeriesInfo(color, SeriesTypes.Average);
      averageSeries.ChartType = SeriesChartType.FastLine;
      averageSeries.Color = averageCheckBox.Checked ? averageSeriesInfo.Color : Color.Transparent;
      averageSeries.BorderWidth = 3;
      averageSeries.BorderDashStyle = ChartDashStyle.Dash;
      averageSeries.IsVisibleInLegend = false;
      averageSeries.Tag = averageSeriesInfo;
      series.Add(averageSeries);

      foreach (var point in values.OrderBy(x => x.Key)) {
        if (point.Value.Count > 0) {
          minMaxSeries.Points.Add(new DataPoint(point.Key, new double[] { point.Value.Min(), point.Value.Max() }));
          quartilesSeries.Points.Add(new DataPoint(point.Key, new double[] { point.Value.Quantile(0.25), point.Value.Quantile(0.75) }));
          medianSeries.Points.Add(new DataPoint(point.Key, point.Value.Median()));
          averageSeries.Points.Add(new DataPoint(point.Key, point.Value.Average()));
        }
      }
      return series;
    }

    #region Control Events
    #region stepSizeTextBox
    private void stepSizeTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        stepSizeLabel.Select();  // select label to validate data

      if (e.KeyCode == Keys.Escape) {
        stepSizeTextBox.Text = stepSize.ToString();
        stepSizeLabel.Select();  // select label to validate data
      }
    }
    private void stepSizeTextBox_Validating(object sender, CancelEventArgs e) {
      int val;
      if (!int.TryParse(stepSizeTextBox.Text, out val) || val <= 0) {
        e.Cancel = true;
        errorProvider.SetError(stepSizeTextBox, "Invalid Value (Valid Value Format: \"[+]digits\")");
        stepSizeTextBox.SelectAll();
      }
    }
    private void stepSizeTextBox_Validated(object sender, EventArgs e) {
      int val = int.Parse(stepSizeTextBox.Text);
      errorProvider.SetError(stepSizeTextBox, string.Empty);
      stepSizeTextBox.Text = val.ToString();
      if (stepSize != val) {
        stepSize = val;
        CollectRunData();
        UpdateChart();
      }
    }
    #endregion
    #region logScalingCheckBox
    private void logScalingCheckBox_CheckedChanged(object sender, EventArgs e) {
      chart.ChartAreas[0].AxisX.IsLogarithmic = logScalingCheckBox.Checked;
    }
    #endregion
    #region chart
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        var legendItemInfo = (result.Object as LegendItem).Tag as LegendItemInfo;
        legendItemInfo.SeriesVisible = !legendItemInfo.SeriesVisible;
        UpdateSeriesVisibility();
      }
    }
    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      switch (result.ChartElementType) {
        case ChartElementType.LegendItem:
          Cursor = Cursors.Hand;
          break;
        default:
          Cursor = Cursors.Default;
          break;
      }
    }
    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      foreach (LegendItem legendItem in e.LegendItems) {
        var legendItemInfo = legendItem.Tag as LegendItemInfo;
        legendItem.Color = legendItemInfo.SeriesVisible ? legendItemInfo.Color : Color.Transparent;
        foreach (LegendCell cell in legendItem.Cells) {
          cell.ForeColor = legendItemInfo.SeriesVisible ? Color.Black : Color.Gray;
        }
      }
    }
    #endregion
    #region parametersTreeView
    private void parametersTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      UpdateChart();
      addGroupButton.Enabled = (parametersTreeView.SelectedNode != null) && (parametersTreeView.SelectedNode.Parent != null);
    }
    #endregion
    #region groupsTreeView
    private void groupsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      UpdateChart();
      removeGroupButton.Enabled = (groupsTreeView.SelectedNode != null) && (groupsTreeView.SelectedNode.Parent != null);
    }
    #endregion
    #region addGroupButton, removeGroupButton
    private void addGroupButton_Click(object sender, EventArgs e) {
      var group = groupsTreeView.SelectedNode;
      var param = parametersTreeView.SelectedNode;
      var groupRuns = group.Parent == null ? group.Tag as IEnumerable<IRun> : (group.Tag as GroupInfo).Runs;

      if (param.Tag is ParameterInfo) {
        var paramInfo = param.Tag as ParameterInfo;
        var paramNode = new TreeNode();
        int count = 0;
        foreach (var valueInfo in paramInfo.Values.Values) {
          var runs = groupRuns.Intersect(valueInfo.Runs);
          var valueNode = new TreeNode(valueInfo.Value + " (" + runs.Count() + ")");
          count += runs.Count();
          valueNode.Tag = new GroupInfo(valueInfo.Value, valueInfo.Color, runs, false);
          paramNode.Nodes.Add(valueNode);
        }
        paramNode.Text = paramInfo.Name + " (" + count + ")";
        paramNode.Tag = new GroupInfo(paramInfo.Name, Color.Empty, paramInfo.Values.Values.SelectMany(x => x.Runs), true);
        group.Nodes.Add(paramNode);
      } else if (param.Tag is ParameterValueInfo) {
        var paramInfo = param.Parent.Tag as ParameterInfo;
        var valueInfo = param.Tag as ParameterValueInfo;
        var runs = groupRuns.Intersect(valueInfo.Runs);
        var paramNode = new TreeNode(paramInfo.Name + " (" + runs.Count() + ")");
        var valueNode = new TreeNode(valueInfo.Value + " (" + runs.Count() + ")");
        valueNode.Tag = new GroupInfo(valueInfo.Value, valueInfo.Color, runs, false);
        paramNode.Tag = new GroupInfo(paramInfo.Name, Color.Empty, runs, true);
        paramNode.Nodes.Add(valueNode);
        group.Nodes.Add(paramNode);
      }
      groupsTreeView.Nodes[0].Expand();
    }
    private void removeGroupButton_Click(object sender, EventArgs e) {
      if (groupsTreeView.SelectedNode != null)
        groupsTreeView.SelectedNode.Remove();
      UpdateChart();
    }
    #endregion
    #region minMaxCheckBox, quartilesCheckBox, averageCheckBox, medianCheckBox
    private void dataRowCheckBox_CheckedChanged(object sender, EventArgs e) {
      UpdateSeriesVisibility();
    }
    #endregion
    #endregion

    #region Inner Types
    class ParameterInfo {
      public string Name { get; private set; }
      public Dictionary<string, ParameterValueInfo> Values { get; private set; }
      public IEnumerable<IRun> Runs {
        get { return Values.Values.SelectMany(x => x.Runs); }
      }
      public int RunCount {
        get { return Values.Values.Select(x => x.RunCount).Sum(); }
      }

      public ParameterInfo(string name, string value, IRun run) {
        Name = name;
        Values = new Dictionary<string, ParameterValueInfo>();
        AddValue(value, run);
      }
      public void AddValue(string value, IRun run) {
        ParameterValueInfo valueInfo;
        if (!Values.TryGetValue(value, out valueInfo)) {
          Values.Add(value, new ParameterValueInfo(value, run));
        } else {
          valueInfo.Runs.Add(run);
        }
      }
    }
    class ParameterValueInfo {
      public string Value { get; private set; }
      public Color Color { get; set; }
      public IList<IRun> Runs { get; private set; }
      public int RunCount {
        get { return Runs.Count; }
      }

      public ParameterValueInfo(string value, IRun run) {
        Value = value;
        Runs = new List<IRun>();
        Runs.Add(run);
      }
      public ParameterValueInfo(string value, Color color, IRun run) : this(value, run) {
        Color = color;
      }
    }
    class GroupInfo {
      public string Text { get; private set; }
      public Color Color { get; private set; }
      public IList<IRun> Runs { get; private set; }
      public bool IsParameter { get; private set; }
      public GroupInfo(string text, Color color, IEnumerable<IRun> runs, bool isParamter) {
        Text = text;
        Color = color;
        Runs = runs.ToList();
        IsParameter = isParamter;
      }
    }
    enum SeriesTypes {
      MinMax,
      Quartiles,
      Average,
      Median
    }
    class SeriesInfo {
      public Color Color { get; private set; }
      public SeriesTypes Type { get; private set; }
      public SeriesInfo(Color color, SeriesTypes type) {
        Color = color;
        Type = type;
      }
    }
    class LegendItemInfo {
      public Color Color { get; private set; }
      public bool SeriesVisible { get; set; }
      public IEnumerable<Series> Series { get; private set; }
      public LegendItemInfo(Color color, IEnumerable<Series> series) {
        Color = color;
        SeriesVisible = true;
        Series = series;
      }
    }
    #endregion
  }
}
