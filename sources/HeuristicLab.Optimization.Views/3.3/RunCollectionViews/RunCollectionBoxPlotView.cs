#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection BoxPlots")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionBoxPlotView : AsynchronousContentView {
    private enum AxisDimension { Color = 0 }
    private const string BoxPlotSeriesName = "BoxPlotSeries";
    private const string BoxPlotChartAreaName = "BoxPlotChartArea";

    private bool suppressUpdates = false;
    private string xAxisValue;
    private string yAxisValue;
    private Dictionary<int, Dictionary<object, double>> categoricalMapping;
    private SortedDictionary<double, Series> seriesCache;

    public RunCollectionBoxPlotView() {
      InitializeComponent();
      categoricalMapping = new Dictionary<int, Dictionary<object, double>>();
      seriesCache = new SortedDictionary<double, Series>();
      chart.ChartAreas[0].Visible = false;
      chart.Series.Clear();
      chart.ChartAreas.Add(BoxPlotChartAreaName);
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[BoxPlotChartAreaName].Axes.ToList().ForEach(x => { x.ScaleView.Zoomable = true; x.ScaleView.MinSize = 0; });
      chart.ChartAreas[BoxPlotChartAreaName].CursorX.Interval = 0.5;
      chart.ChartAreas[BoxPlotChartAreaName].CursorY.Interval = 1e-5;
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }
    public IStringConvertibleMatrix Matrix {
      get { return this.Content; }
    }

    #region RunCollection and Run events
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Reset += new EventHandler(Content_Reset);
      Content.ColumnNamesChanged += new EventHandler(Content_ColumnNamesChanged);
      Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged += new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.AlgorithmNameChanged += new EventHandler(Content_AlgorithmNameChanged);
      RegisterRunEvents(Content);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Reset -= new EventHandler(Content_Reset);
      Content.ColumnNamesChanged -= new EventHandler(Content_ColumnNamesChanged);
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.UpdateOfRunsInProgressChanged -= new EventHandler(Content_UpdateOfRunsInProgressChanged);
      Content.AlgorithmNameChanged -= new EventHandler(Content_AlgorithmNameChanged);
      DeregisterRunEvents(Content);
    }

    protected virtual void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed += new EventHandler(run_Changed);
    }
    protected virtual void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed -= new EventHandler(run_Changed);
    }

    private void Content_CollectionReset(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.OldItems);
      RegisterRunEvents(e.Items);
    }
    private void Content_ItemsRemoved(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      DeregisterRunEvents(e.Items);
    }
    private void Content_ItemsAdded(object sender, HeuristicLab.Collections.CollectionItemsChangedEventArgs<IRun> e) {
      RegisterRunEvents(e.Items);
    }
    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (!suppressUpdates) UpdateDataPoints();
      }
    }

    private void Content_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Reset), sender, e);
      else {
        this.categoricalMapping.Clear();
        UpdateDataPoints();
      }
    }
    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else {
        UpdateComboBoxes();
      }
    }
    private void run_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(run_Changed), sender, e);
      else if (!suppressUpdates) {
        IRun run = (IRun)sender;
        UpdateDataPoints();
      }
    }

    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }
    #endregion

    #region update comboboxes, datapoints, runs
    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.categoricalMapping.Clear();
      UpdateComboBoxes();
      UpdateDataPoints();
      UpdateCaption();
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.AlgorithmName + "Box Plots" : ViewAttribute.GetViewName(GetType());
    }

    private void UpdateComboBoxes() {
      string selectedXAxis = (string)this.xAxisComboBox.SelectedItem;
      string selectedYAxis = (string)this.yAxisComboBox.SelectedItem;
      this.xAxisComboBox.Items.Clear();
      this.yAxisComboBox.Items.Clear();
      if (Content != null) {
        string[] additionalAxisDimension = Enum.GetNames(typeof(AxisDimension));
        this.xAxisComboBox.Items.AddRange(additionalAxisDimension);
        this.xAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
        this.yAxisComboBox.Items.AddRange(additionalAxisDimension);
        this.yAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());

        bool changed = false;
        if (selectedXAxis != null && xAxisComboBox.Items.Contains(selectedXAxis)) {
          xAxisComboBox.SelectedItem = selectedXAxis;
          changed = true;
        }
        if (selectedYAxis != null && yAxisComboBox.Items.Contains(selectedYAxis)) {
          yAxisComboBox.SelectedItem = selectedYAxis;
          changed = true;
        }
        if (changed)
          UpdateDataPoints();
      }
    }

    private void UpdateDataPoints() {
      this.chart.Series.Clear();
      this.seriesCache.Clear();
      if (Content != null) {
        foreach (IRun run in this.Content.Where(r => r.Visible))
          this.AddDataPoint(run);
        foreach (Series s in this.seriesCache.Values)
          this.chart.Series.Add(s);

        UpdateStatistics();
        if (seriesCache.Count > 0) {
          Series boxPlotSeries = CreateBoxPlotSeries();
          this.chart.Series.Add(boxPlotSeries);
        }

        UpdateAxisLabels();
      }
      UpdateNoRunsVisibleLabel();
    }

    private void UpdateStatistics() {
      DoubleMatrix matrix = new DoubleMatrix(9, seriesCache.Count);
      matrix.SortableView = false;
      List<string> columnNames = new List<string>();
      foreach (Series series in seriesCache.Values) {
        DataPoint datapoint = series.Points.FirstOrDefault();
        if (datapoint != null) {
          IRun run = (IRun)datapoint.Tag;
          string selectedAxis = (string)xAxisComboBox.SelectedItem;
          IItem value = null;

          if (Enum.IsDefined(typeof(AxisDimension), selectedAxis)) {
            AxisDimension axisDimension = (AxisDimension)Enum.Parse(typeof(AxisDimension), selectedAxis);
            switch (axisDimension) {
              case AxisDimension.Color: value = new StringValue(run.Color.ToString());
                break;
            }
          } else value = Content.GetValue(run, selectedAxis);
          string columnName = string.Empty;
          if (value is DoubleValue || value is IntValue)
            columnName = selectedAxis + ": ";
          columnName += value.ToString();
          columnNames.Add(columnName);
        }
      }
      matrix.ColumnNames = columnNames;
      matrix.RowNames = new string[] { "Count", "Minimum", "Maximum", "Average", "Median", "Standard Deviation", "Variance", "25th Percentile", "75th Percentile" };

      for (int i = 0; i < seriesCache.Count; i++) {
        Series series = seriesCache.ElementAt(i).Value;
        double[] seriesValues = series.Points.Select(p => p.YValues[0]).OrderBy(d => d).ToArray();
        matrix[0, i] = seriesValues.Length;
        matrix[1, i] = seriesValues.Min();
        matrix[2, i] = seriesValues.Max();
        matrix[3, i] = seriesValues.Average();
        matrix[4, i] = seriesValues.Median();
        matrix[5, i] = seriesValues.StandardDeviation();
        matrix[6, i] = seriesValues.Variance();
        matrix[7, i] = seriesValues.Percentile(0.25);
        matrix[8, i] = seriesValues.Percentile(0.75);
      }
      statisticsMatrixView.Content = matrix;
    }

    private Series CreateBoxPlotSeries() {
      Series boxPlotSeries = new Series(BoxPlotSeriesName);
      string seriesNames = string.Concat(seriesCache.Keys.Select(x => x.ToString() + ";").ToArray());
      seriesNames = seriesNames.Remove(seriesNames.Length - 1); //delete last ; from string

      boxPlotSeries.ChartArea = BoxPlotChartAreaName;
      boxPlotSeries.ChartType = SeriesChartType.BoxPlot;
      boxPlotSeries["BoxPlotSeries"] = seriesNames;
      boxPlotSeries["BoxPlotShowUnusualValues"] = "true";
      boxPlotSeries["PointWidth"] = "0.4";
      boxPlotSeries.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.VerticalCenter;
      boxPlotSeries.BackSecondaryColor = System.Drawing.Color.FromArgb(130, 224, 64, 10);
      boxPlotSeries.BorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
      boxPlotSeries.Color = System.Drawing.Color.FromArgb(224, 64, 10);

      return boxPlotSeries;
    }

    private void AddDataPoint(IRun run) {
      double? xValue;
      double? yValue;

      if (!xAxisComboBox.DroppedDown)
        this.xAxisValue = (string)xAxisComboBox.SelectedItem;
      if (!yAxisComboBox.DroppedDown)
        this.yAxisValue = (string)yAxisComboBox.SelectedItem;

      xValue = GetValue(run, this.xAxisValue);
      yValue = GetValue(run, this.yAxisValue);

      if (xValue.HasValue && yValue.HasValue) {
        if (!this.seriesCache.ContainsKey(xValue.Value))
          seriesCache[xValue.Value] = new Series(xValue.Value.ToString());

        Series series = seriesCache[xValue.Value];
        DataPoint point = new DataPoint(xValue.Value, yValue.Value);
        point.Tag = run;
        series.Points.Add(point);
      }
    }
    #endregion

    #region get values from run
    private double? GetValue(IRun run, string columnName) {
      if (run == null || string.IsNullOrEmpty(columnName))
        return null;

      if (Enum.IsDefined(typeof(AxisDimension), columnName)) {
        AxisDimension axisDimension = (AxisDimension)Enum.Parse(typeof(AxisDimension), columnName);
        return GetValue(run, axisDimension);
      } else {
        int columnIndex = Matrix.ColumnNames.ToList().IndexOf(columnName);
        IItem value = Content.GetValue(run, columnIndex);
        if (value == null)
          return null;

        DoubleValue doubleValue = value as DoubleValue;
        IntValue intValue = value as IntValue;
        TimeSpanValue timeSpanValue = value as TimeSpanValue;
        double? ret = null;
        if (doubleValue != null) {
          if (!double.IsNaN(doubleValue.Value) && !double.IsInfinity(doubleValue.Value))
            ret = doubleValue.Value;
        } else if (intValue != null)
          ret = intValue.Value;
        else if (timeSpanValue != null) {
          ret = timeSpanValue.Value.TotalSeconds;
        } else
          ret = GetCategoricalValue(columnIndex, value.ToString());

        return ret;
      }
    }
    private double GetCategoricalValue(int dimension, string value) {
      if (!this.categoricalMapping.ContainsKey(dimension))
        this.categoricalMapping[dimension] = new Dictionary<object, double>();
      if (!this.categoricalMapping[dimension].ContainsKey(value)) {
        if (this.categoricalMapping[dimension].Values.Count == 0)
          this.categoricalMapping[dimension][value] = 1.0;
        else
          this.categoricalMapping[dimension][value] = this.categoricalMapping[dimension].Values.Max() + 1.0;
      }
      return this.categoricalMapping[dimension][value];
    }
    private double GetValue(IRun run, AxisDimension axisDimension) {
      double value = double.NaN;
      switch (axisDimension) {
        case AxisDimension.Color: {
            value = GetCategoricalValue(-1, run.Color.ToString());
            break;
          }
        default: {
            throw new ArgumentException("No handling strategy for " + axisDimension.ToString() + " is defined.");
          }
      }
      return value;
    }
    #endregion

    #region GUI events
    private void UpdateNoRunsVisibleLabel() {
      if (this.chart.Series.Count > 0) {
        noRunsLabel.Visible = false;
        showStatisticsCheckBox.Enabled = true;
        splitContainer.Panel2Collapsed = !showStatisticsCheckBox.Checked;
      } else {
        noRunsLabel.Visible = true;
        showStatisticsCheckBox.Enabled = false;
        splitContainer.Panel2Collapsed = true;
      }
    }

    private void AxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateDataPoints();
    }
    private void UpdateAxisLabels() {
      Axis xAxis = this.chart.ChartAreas[BoxPlotChartAreaName].AxisX;
      Axis yAxis = this.chart.ChartAreas[BoxPlotChartAreaName].AxisY;
      int axisDimensionCount = Enum.GetNames(typeof(AxisDimension)).Count();
      SetCustomAxisLabels(xAxis, xAxisComboBox.SelectedIndex - axisDimensionCount);
      SetCustomAxisLabels(yAxis, yAxisComboBox.SelectedIndex - axisDimensionCount);
      if (xAxisComboBox.SelectedItem != null)
        xAxis.Title = xAxisComboBox.SelectedItem.ToString();
      if (yAxisComboBox.SelectedItem != null)
        yAxis.Title = yAxisComboBox.SelectedItem.ToString();
    }

    private void chart_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e) {
      this.UpdateAxisLabels();
    }

    private void SetCustomAxisLabels(Axis axis, int dimension) {
      axis.CustomLabels.Clear();
      if (categoricalMapping.ContainsKey(dimension)) {
        foreach (var pair in categoricalMapping[dimension]) {
          string labelText = pair.Key.ToString();
          CustomLabel label = new CustomLabel();
          label.ToolTip = labelText;
          if (labelText.Length > 25)
            labelText = labelText.Substring(0, 25) + " ... ";
          label.Text = labelText;
          label.GridTicks = GridTickTypes.TickMark;
          label.FromPosition = pair.Value - 0.5;
          label.ToPosition = pair.Value + 0.5;
          axis.CustomLabels.Add(label);
        }
      } else if (dimension > 0 && Content.GetValue(0, dimension) is TimeSpanValue) {
        this.chart.ChartAreas[0].RecalculateAxesScale();
        Axis correspondingAxis = this.chart.ChartAreas[0].Axes.Where(x => x.Name == axis.Name).SingleOrDefault();
        if (correspondingAxis == null)
          correspondingAxis = axis;
        for (double i = correspondingAxis.Minimum; i <= correspondingAxis.Maximum; i += correspondingAxis.LabelStyle.Interval) {
          TimeSpan time = TimeSpan.FromSeconds(i);
          string x = string.Format("{0:00}:{1:00}:{2:00}", (int)time.Hours, time.Minutes, time.Seconds);
          axis.CustomLabels.Add(i - correspondingAxis.LabelStyle.Interval / 2, i + correspondingAxis.LabelStyle.Interval / 2, x);
        }
      } else if (chart.ChartAreas[BoxPlotChartAreaName].AxisX == axis) {
        double position = 1.0;
        foreach (Series series in chart.Series) {
          if (series.Name != BoxPlotSeriesName) {
            string labelText = series.Points[0].XValue.ToString();
            CustomLabel label = new CustomLabel();
            label.FromPosition = position - 0.5;
            label.ToPosition = position + 0.5;
            label.GridTicks = GridTickTypes.TickMark;
            label.Text = labelText;
            axis.CustomLabels.Add(label);
            position++;
          }
        }
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      string newTooltipText = string.Empty;
      string oldTooltipText;
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      if (h.ChartElementType == ChartElementType.AxisLabels) {
        newTooltipText = ((CustomLabel)h.Object).ToolTip;
      }

      oldTooltipText = this.tooltip.GetToolTip(chart);
      if (newTooltipText != oldTooltipText)
        this.tooltip.SetToolTip(chart, newTooltipText);
    }
    #endregion

    private void showStatisticsCheckBox_CheckedChanged(object sender, EventArgs e) {
      splitContainer.Panel2Collapsed = !showStatisticsCheckBox.Checked;
    }

  }
}
