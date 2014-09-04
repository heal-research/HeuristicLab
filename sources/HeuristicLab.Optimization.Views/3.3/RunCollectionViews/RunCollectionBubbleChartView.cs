#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimization.Views {
  [View("Bubble Chart")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionBubbleChartView : AsynchronousContentView {
    private enum SizeDimension { Constant = 0 }
    private enum AxisDimension { Index = 0 }

    private string xAxisValue;
    private string yAxisValue;
    private string sizeAxisValue;

    private readonly Dictionary<IRun, List<DataPoint>> runToDataPointMapping = new Dictionary<IRun, List<DataPoint>>();
    private readonly Dictionary<IRun, int> runToIndexMapping = new Dictionary<IRun, int>();
    private readonly Dictionary<int, Dictionary<object, double>> categoricalMapping = new Dictionary<int, Dictionary<object, double>>();
    private readonly Dictionary<IRun, double> xJitter = new Dictionary<IRun, double>();
    private readonly Dictionary<IRun, double> yJitter = new Dictionary<IRun, double>();

    private readonly HashSet<IRun> selectedRuns = new HashSet<IRun>();
    private readonly Random random = new Random();
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private bool isSelecting = false;
    private bool suppressUpdates = false;

    private readonly RunCollectionContentConstraint visibilityConstraint = new RunCollectionContentConstraint() { Active = true };

    public RunCollectionBubbleChartView() {
      InitializeComponent();

      chart.ContextMenuStrip.Items.Insert(0, openBoxPlotViewToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(1, getDataAsMatrixToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(2, new ToolStripSeparator());
      chart.ContextMenuStrip.Items.Insert(3, hideRunsToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(4, unhideAllRunToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(5, colorResetToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(6, new ToolStripSeparator());
      chart.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);

      colorDialog.Color = Color.Orange;
      colorRunsButton.Image = this.GenerateImage(16, 16, this.colorDialog.Color);
      isSelecting = false;

      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.Interval = 1;
      chart.ChartAreas[0].CursorY.Interval = 1;
      chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !this.isSelecting;
      chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !this.isSelecting;
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }
    public IStringConvertibleMatrix Matrix {
      get { return this.Content; }
    }
    public IEnumerable<IRun> SelectedRuns {
      get { return selectedRuns; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Reset += new EventHandler(Content_Reset);
      Content.ColumnNamesChanged += new EventHandler(Content_ColumnNamesChanged);
      Content.ItemsAdded += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset += new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.OptimizerNameChanged += new EventHandler(Content_AlgorithmNameChanged);
      Content.UpdateOfRunsInProgressChanged += new EventHandler(Content_UpdateOfRunsInProgressChanged);
      RegisterRunEvents(Content);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Reset -= new EventHandler(Content_Reset);
      Content.ColumnNamesChanged -= new EventHandler(Content_ColumnNamesChanged);
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
      Content.OptimizerNameChanged -= new EventHandler(Content_AlgorithmNameChanged);
      Content.UpdateOfRunsInProgressChanged -= new EventHandler(Content_UpdateOfRunsInProgressChanged);
      DeregisterRunEvents(Content);
    }
    protected virtual void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged += run_PropertyChanged;
    }
    protected virtual void DeregisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.PropertyChanged -= run_PropertyChanged;
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
    private void run_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (suppressUpdates) return;
      if (InvokeRequired)
        this.Invoke((Action<object, PropertyChangedEventArgs>)run_PropertyChanged, sender, e);
      else {
        if (e.PropertyName == "Color" || e.PropertyName == "Visible") {
          IRun run = (IRun)sender;
          UpdateRun(run);
          UpdateCursorInterval();
          chart.ChartAreas[0].RecalculateAxesScale();
          UpdateAxisLabels();
        }
      }
    }

    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (suppressUpdates) return;

        UpdateDataPoints();
        UpdateAxisLabels();
      }
    }

    private void UpdateRun(IRun run) {
      if (runToDataPointMapping.ContainsKey(run)) {
        foreach (DataPoint point in runToDataPointMapping[run]) {
          if (!run.Visible) {
            this.chart.Series[0].Points.Remove(point);
            continue;
          }
          if (selectedRuns.Contains(run)) {
            point.Color = Color.Red;
            point.MarkerStyle = MarkerStyle.Cross;
          } else {
            point.Color = Color.FromArgb(255 - LogTransform(transparencyTrackBar.Value), ((IRun)point.Tag).Color);
            point.MarkerStyle = MarkerStyle.Circle;
          }

        }
        if (!run.Visible) runToDataPointMapping.Remove(run);
      } else {
        AddDataPoint(run);
      }

      if (this.chart.Series[0].Points.Count == 0)
        noRunsLabel.Visible = true;
      else
        noRunsLabel.Visible = false;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateComboBoxes();
      UpdateDataPoints();
      UpdateCaption();
    }

    private void RebuildInverseIndex() {
      if (Content != null) {
        runToIndexMapping.Clear();
        int i = 0;
        foreach (var run in Content) {
          runToIndexMapping.Add(run, i);
          i++;
        }
      }
    }

    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else
        UpdateComboBoxes();
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.OptimizerName + " Bubble Chart" : ViewAttribute.GetViewName(GetType());
    }

    private void UpdateComboBoxes() {
      string selectedXAxis = (string)this.xAxisComboBox.SelectedItem;
      string selectedYAxis = (string)this.yAxisComboBox.SelectedItem;
      string selectedSizeAxis = (string)this.sizeComboBox.SelectedItem;
      this.xAxisComboBox.Items.Clear();
      this.yAxisComboBox.Items.Clear();
      this.sizeComboBox.Items.Clear();
      if (Content != null) {
        string[] additionalAxisDimension = Enum.GetNames(typeof(AxisDimension));
        this.xAxisComboBox.Items.AddRange(additionalAxisDimension);
        this.xAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
        this.yAxisComboBox.Items.AddRange(additionalAxisDimension);
        this.yAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
        string[] additionalSizeDimension = Enum.GetNames(typeof(SizeDimension));
        this.sizeComboBox.Items.AddRange(additionalSizeDimension);
        this.sizeComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
        this.sizeComboBox.SelectedItem = SizeDimension.Constant.ToString();

        bool changed = false;
        if (selectedXAxis != null && xAxisComboBox.Items.Contains(selectedXAxis)) {
          xAxisComboBox.SelectedItem = selectedXAxis;
          changed = true;
        }
        if (selectedYAxis != null && yAxisComboBox.Items.Contains(selectedYAxis)) {
          yAxisComboBox.SelectedItem = selectedYAxis;
          changed = true;
        }
        if (selectedSizeAxis != null && sizeComboBox.Items.Contains(selectedSizeAxis)) {
          sizeComboBox.SelectedItem = selectedSizeAxis;
          changed = true;
        }
        if (changed) {
          UpdateDataPoints();
          UpdateAxisLabels();
        }
      }
    }

    private void Content_AlgorithmNameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_AlgorithmNameChanged), sender, e);
      else UpdateCaption();
    }

    private void Content_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Reset), sender, e);
      else {
        UpdateDataPoints();
        UpdateAxisLabels();
      }
    }

    private void UpdateDataPoints() {
      Series series = this.chart.Series[0];
      series.Points.Clear();
      runToDataPointMapping.Clear();
      categoricalMapping.Clear();
      selectedRuns.Clear();
      RebuildInverseIndex();

      chart.ChartAreas[0].AxisX.IsMarginVisible = xAxisValue != AxisDimension.Index.ToString();
      chart.ChartAreas[0].AxisY.IsMarginVisible = yAxisValue != AxisDimension.Index.ToString();

      if (Content != null) {
        foreach (IRun run in this.Content)
          this.AddDataPoint(run);

        if (this.chart.Series[0].Points.Count == 0)
          noRunsLabel.Visible = true;
        else {
          noRunsLabel.Visible = false;
          UpdateMarkerSizes();
          UpdateCursorInterval();
        }
      }
      xTrackBar.Value = 0;
      yTrackBar.Value = 0;

      //needed to set axis back to automatic and refresh them, otherwise their values may remain NaN
      var xAxis = chart.ChartAreas[0].AxisX;
      var yAxis = chart.ChartAreas[0].AxisY;
      SetAutomaticUpdateOfAxis(xAxis, true);
      SetAutomaticUpdateOfAxis(yAxis, true);
      chart.Refresh();
    }

    private void UpdateMarkerSizes() {
      var series = chart.Series[0];
      if (series.Points.Count <= 0) return;

      var sizeValues = series.Points.Select(p => p.YValues[1]);
      double minSizeValue = sizeValues.Min();
      double maxSizeValue = sizeValues.Max();
      double sizeRange = maxSizeValue - minSizeValue;

      const int smallestBubbleSize = 7;

      foreach (DataPoint point in series.Points) {
        //calculates the relative size of the data point  0 <= relativeSize <= 1
        double relativeSize = (point.YValues[1] - minSizeValue);
        if (sizeRange > double.Epsilon) {
          relativeSize /= sizeRange;

          //invert bubble sizes if the value of the trackbar is negative
          if (sizeTrackBar.Value < 0) relativeSize = Math.Abs(relativeSize - 1);
        } else relativeSize = 1;

        double sizeChange = Math.Abs(sizeTrackBar.Value) * relativeSize;
        point.MarkerSize = (int)Math.Round(sizeChange + smallestBubbleSize);
      }
    }

    private void UpdateDataPointJitter() {
      var xAxis = this.chart.ChartAreas[0].AxisX;
      var yAxis = this.chart.ChartAreas[0].AxisY;

      double xAxisRange = xAxis.Maximum - xAxis.Minimum;
      double yAxisRange = yAxis.Maximum - yAxis.Minimum;

      foreach (DataPoint point in chart.Series[0].Points) {
        IRun run = (IRun)point.Tag;
        double xValue = GetValue(run, xAxisValue).Value;
        double yValue = GetValue(run, yAxisValue).Value;

        if (!xJitterFactor.IsAlmost(0.0))
          xValue += 0.1 * GetXJitter(run) * xJitterFactor * (xAxisRange);
        if (!yJitterFactor.IsAlmost(0.0))
          yValue += 0.1 * GetYJitter(run) * yJitterFactor * (yAxisRange);

        point.XValue = xValue;
        point.YValues[0] = yValue;
      }

      if (xJitterFactor.IsAlmost(0.0) && yJitterFactor.IsAlmost(0.0)) {
        SetAutomaticUpdateOfAxis(xAxis, true);
        SetAutomaticUpdateOfAxis(yAxis, true);
        chart.ChartAreas[0].RecalculateAxesScale();
      } else {
        SetAutomaticUpdateOfAxis(xAxis, false);
        SetAutomaticUpdateOfAxis(yAxis, false);
      }
    }

    // sets an axis to automatic or restrains it to its current values
    // this is used that none of the set values is changed when jitter is applied, so that the chart stays the same
    private void SetAutomaticUpdateOfAxis(Axis axis, bool enabled) {
      if (enabled) {
        axis.Maximum = double.NaN;
        axis.Minimum = double.NaN;
        axis.MajorGrid.Interval = double.NaN;
        axis.MajorTickMark.Interval = double.NaN;
        axis.LabelStyle.Interval = double.NaN;
      } else {
        axis.Minimum = axis.Minimum;
        axis.Maximum = axis.Maximum;
        axis.MajorGrid.Interval = axis.MajorGrid.Interval;
        axis.MajorTickMark.Interval = axis.MajorTickMark.Interval;
        axis.LabelStyle.Interval = axis.LabelStyle.Interval;
      }
    }

    private void AddDataPoint(IRun run) {
      double? xValue;
      double? yValue;
      double? sizeValue;
      Series series = this.chart.Series[0];

      xValue = GetValue(run, xAxisValue);
      yValue = GetValue(run, yAxisValue);
      sizeValue = GetValue(run, sizeAxisValue);

      if (xValue.HasValue && yValue.HasValue && sizeValue.HasValue) {
        xValue = xValue.Value;
        yValue = yValue.Value;

        if (run.Visible) {
          DataPoint point = new DataPoint(xValue.Value, new double[] { yValue.Value, sizeValue.Value });
          point.Tag = run;
          series.Points.Add(point);
          if (!runToDataPointMapping.ContainsKey(run)) runToDataPointMapping.Add(run, new List<DataPoint>());
          runToDataPointMapping[run].Add(point);
          UpdateRun(run);
        }
      }
    }
    private double? GetValue(IRun run, string columnName) {
      if (run == null || string.IsNullOrEmpty(columnName))
        return null;

      if (Enum.IsDefined(typeof(AxisDimension), columnName)) {
        AxisDimension axisDimension = (AxisDimension)Enum.Parse(typeof(AxisDimension), columnName);
        return GetValue(run, axisDimension);
      } else if (Enum.IsDefined(typeof(SizeDimension), columnName)) {
        SizeDimension sizeDimension = (SizeDimension)Enum.Parse(typeof(SizeDimension), columnName);
        return GetValue(run, sizeDimension);
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
    private double? GetCategoricalValue(int dimension, string value) {
      if (!this.categoricalMapping.ContainsKey(dimension)) {
        this.categoricalMapping[dimension] = new Dictionary<object, double>();
        var orderedCategories = Content.Where(r => r.Visible && Content.GetValue(r, dimension) != null).Select(r => Content.GetValue(r, dimension).ToString())
                                    .Distinct().OrderBy(x => x, new NaturalStringComparer());
        int count = 1;
        foreach (var category in orderedCategories) {
          this.categoricalMapping[dimension].Add(category, count);
          count++;
        }
      }
      if (!this.categoricalMapping[dimension].ContainsKey(value)) return null;
      return this.categoricalMapping[dimension][value];
    }

    private double GetValue(IRun run, AxisDimension axisDimension) {
      double value = double.NaN;
      switch (axisDimension) {
        case AxisDimension.Index: {
            value = runToIndexMapping[run];
            break;
          }
        default: {
            throw new ArgumentException("No handling strategy for " + axisDimension.ToString() + " is defined.");
          }
      }
      return value;
    }
    private double GetValue(IRun run, SizeDimension sizeDimension) {
      double value = double.NaN;
      switch (sizeDimension) {
        case SizeDimension.Constant: {
            value = 2;
            break;
          }
        default: {
            throw new ArgumentException("No handling strategy for " + sizeDimension.ToString() + " is defined.");
          }
      }
      return value;
    }
    private void UpdateCursorInterval() {
      double xMin = double.MaxValue;
      double xMax = double.MinValue;
      double yMin = double.MaxValue;
      double yMax = double.MinValue;

      foreach (var point in chart.Series[0].Points) {
        if (point.IsEmpty) continue;
        if (point.XValue < xMin) xMin = point.XValue;
        if (point.XValue > xMax) xMax = point.XValue;
        if (point.YValues[0] < yMin) yMin = point.YValues[0];
        if (point.YValues[0] > yMax) yMax = point.YValues[0];
      }

      double xRange = 0.0;
      double yRange = 0.0;
      if (xMin != double.MaxValue && xMax != double.MinValue) xRange = xMax - xMin;
      if (yMin != double.MaxValue && yMax != double.MinValue) yRange = yMax - yMin;

      if (xRange.IsAlmost(0.0)) xRange = 1.0;
      if (yRange.IsAlmost(0.0)) yRange = 1.0;
      double xDigits = (int)Math.Log10(xRange) - 3;
      double yDigits = (int)Math.Log10(yRange) - 3;
      double xZoomInterval = Math.Pow(10, xDigits);
      double yZoomInterval = Math.Pow(10, yDigits);
      this.chart.ChartAreas[0].CursorX.Interval = xZoomInterval;
      this.chart.ChartAreas[0].CursorY.Interval = yZoomInterval;

      //code to handle TimeSpanValues correct
      int axisDimensionCount = Enum.GetNames(typeof(AxisDimension)).Count();
      int columnIndex = xAxisComboBox.SelectedIndex - axisDimensionCount;
      if (columnIndex >= 0 && Content.GetValue(0, columnIndex) is TimeSpanValue)
        this.chart.ChartAreas[0].CursorX.Interval = 1;
      columnIndex = yAxisComboBox.SelectedIndex - axisDimensionCount;
      if (columnIndex >= 0 && Content.GetValue(0, columnIndex) is TimeSpanValue)
        this.chart.ChartAreas[0].CursorY.Interval = 1;
    }

    #region Drag & drop and tooltip
    private void chart_MouseDoubleClick(object sender, MouseEventArgs e) {
      HitTestResult h = this.chart.HitTest(e.X, e.Y, ChartElementType.DataPoint);
      if (h.ChartElementType == ChartElementType.DataPoint) {
        IRun run = (IRun)((DataPoint)h.Object).Tag;
        IContentView view = MainFormManager.MainForm.ShowContent(run);
        if (view != null) {
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
        }

        this.chart.ChartAreas[0].CursorX.SelectionStart = this.chart.ChartAreas[0].CursorX.SelectionEnd;
        this.chart.ChartAreas[0].CursorY.SelectionStart = this.chart.ChartAreas[0].CursorY.SelectionEnd;
      }
      UpdateAxisLabels();
    }

    private void chart_MouseUp(object sender, MouseEventArgs e) {
      if (!isSelecting) return;

      System.Windows.Forms.DataVisualization.Charting.Cursor xCursor = chart.ChartAreas[0].CursorX;
      System.Windows.Forms.DataVisualization.Charting.Cursor yCursor = chart.ChartAreas[0].CursorY;

      double minX = Math.Min(xCursor.SelectionStart, xCursor.SelectionEnd);
      double maxX = Math.Max(xCursor.SelectionStart, xCursor.SelectionEnd);
      double minY = Math.Min(yCursor.SelectionStart, yCursor.SelectionEnd);
      double maxY = Math.Max(yCursor.SelectionStart, yCursor.SelectionEnd);

      if (Control.ModifierKeys != Keys.Control) {
        ClearSelectedRuns();
      }

      //check for click to select a single model
      if (minX == maxX && minY == maxY) {
        HitTestResult hitTest = chart.HitTest(e.X, e.Y, ChartElementType.DataPoint);
        if (hitTest.ChartElementType == ChartElementType.DataPoint) {
          int pointIndex = hitTest.PointIndex;
          var point = chart.Series[0].Points[pointIndex];
          IRun run = (IRun)point.Tag;
          if (selectedRuns.Contains(run)) {
            point.MarkerStyle = MarkerStyle.Circle;
            point.Color = Color.FromArgb(255 - LogTransform(transparencyTrackBar.Value), run.Color);
            selectedRuns.Remove(run);
          } else {
            point.Color = Color.Red;
            point.MarkerStyle = MarkerStyle.Cross;
            selectedRuns.Add(run);
          }
        }
      } else {
        foreach (DataPoint point in this.chart.Series[0].Points) {
          if (point.XValue < minX || point.XValue > maxX) continue;
          if (point.YValues[0] < minY || point.YValues[0] > maxY) continue;
          point.MarkerStyle = MarkerStyle.Cross;
          point.Color = Color.Red;
          IRun run = (IRun)point.Tag;
          selectedRuns.Add(run);
        }
      }

      this.chart.ChartAreas[0].CursorX.SelectionStart = this.chart.ChartAreas[0].CursorX.SelectionEnd;
      this.chart.ChartAreas[0].CursorY.SelectionStart = this.chart.ChartAreas[0].CursorY.SelectionEnd;
      this.OnChanged();
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      if (Control.MouseButtons != MouseButtons.None) return;
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      string newTooltipText = string.Empty;
      string oldTooltipText;
      if (h.ChartElementType == ChartElementType.DataPoint) {
        IRun run = (IRun)((DataPoint)h.Object).Tag;
        newTooltipText = BuildTooltip(run);
      } else if (h.ChartElementType == ChartElementType.AxisLabels) {
        newTooltipText = ((CustomLabel)h.Object).ToolTip;
      }

      oldTooltipText = this.tooltip.GetToolTip(chart);
      if (newTooltipText != oldTooltipText)
        this.tooltip.SetToolTip(chart, newTooltipText);
    }

    private string BuildTooltip(IRun run) {
      string tooltip;
      tooltip = run.Name + System.Environment.NewLine;

      double? xValue = this.GetValue(run, (string)xAxisComboBox.SelectedItem);
      double? yValue = this.GetValue(run, (string)yAxisComboBox.SelectedItem);
      double? sizeValue = this.GetValue(run, (string)sizeComboBox.SelectedItem);

      string xString = xValue == null ? string.Empty : xValue.Value.ToString();
      string yString = yValue == null ? string.Empty : yValue.Value.ToString();
      string sizeString = sizeValue == null ? string.Empty : sizeValue.Value.ToString();

      //code to handle TimeSpanValues correct
      int axisDimensionCount = Enum.GetNames(typeof(AxisDimension)).Count();
      int columnIndex = xAxisComboBox.SelectedIndex - axisDimensionCount;
      if (xValue.HasValue && columnIndex > 0 && Content.GetValue(0, columnIndex) is TimeSpanValue) {
        TimeSpan time = TimeSpan.FromSeconds(xValue.Value);
        xString = string.Format("{0:00}:{1:00}:{2:00.00}", (int)time.TotalHours, time.Minutes, time.Seconds);
      }
      columnIndex = yAxisComboBox.SelectedIndex - axisDimensionCount;
      if (yValue.HasValue && columnIndex > 0 && Content.GetValue(0, columnIndex) is TimeSpanValue) {
        TimeSpan time = TimeSpan.FromSeconds(yValue.Value);
        yString = string.Format("{0:00}:{1:00}:{2:00.00}", (int)time.TotalHours, time.Minutes, time.Seconds);
      }

      tooltip += xAxisComboBox.SelectedItem + " : " + xString + Environment.NewLine;
      tooltip += yAxisComboBox.SelectedItem + " : " + yString + Environment.NewLine;
      tooltip += sizeComboBox.SelectedItem + " : " + sizeString + Environment.NewLine;

      return tooltip;
    }
    #endregion

    #region GUI events and updating
    private double GetXJitter(IRun run) {
      if (!this.xJitter.ContainsKey(run))
        this.xJitter[run] = random.NextDouble() * 2.0 - 1.0;
      return this.xJitter[run];
    }
    private double GetYJitter(IRun run) {
      if (!this.yJitter.ContainsKey(run))
        this.yJitter[run] = random.NextDouble() * 2.0 - 1.0;
      return this.yJitter[run];
    }
    private void jitterTrackBar_ValueChanged(object sender, EventArgs e) {
      this.xJitterFactor = xTrackBar.Value / 100.0;
      this.yJitterFactor = yTrackBar.Value / 100.0;
      UpdateDataPointJitter();
    }
    private void sizeTrackBar_ValueChanged(object sender, EventArgs e) {
      UpdateMarkerSizes();
    }

    private void AxisComboBox_SelectedValueChanged(object sender, EventArgs e) {
      bool axisSelected = xAxisComboBox.SelectedIndex != -1 && yAxisComboBox.SelectedIndex != -1;
      xTrackBar.Enabled = yTrackBar.Enabled = axisSelected;
      colorXAxisButton.Enabled = colorYAxisButton.Enabled = axisSelected;

      xAxisValue = (string)xAxisComboBox.SelectedItem;
      yAxisValue = (string)yAxisComboBox.SelectedItem;
      sizeAxisValue = (string)sizeComboBox.SelectedItem;

      UpdateDataPoints();
      UpdateAxisLabels();
    }
    private void UpdateAxisLabels() {
      Axis xAxis = this.chart.ChartAreas[0].AxisX;
      Axis yAxis = this.chart.ChartAreas[0].AxisY;
      int axisDimensionCount = Enum.GetNames(typeof(AxisDimension)).Count();
      //mkommend: combobox.SelectedIndex could not be used as this changes during hovering over possible values
      var xSAxisSelectedIndex = xAxisValue == null ? 0 : xAxisComboBox.Items.IndexOf(xAxisValue);
      var ySAxisSelectedIndex = yAxisValue == null ? 0 : xAxisComboBox.Items.IndexOf(yAxisValue);
      SetCustomAxisLabels(xAxis, xSAxisSelectedIndex - axisDimensionCount);
      SetCustomAxisLabels(yAxis, ySAxisSelectedIndex - axisDimensionCount);
      if (xAxisValue != null)
        xAxis.Title = xAxisValue;
      if (yAxisValue != null)
        yAxis.Title = yAxisValue;
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
        for (double i = axis.Minimum; i <= axis.Maximum; i += axis.LabelStyle.Interval) {
          TimeSpan time = TimeSpan.FromSeconds(i);
          string x = string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
          axis.CustomLabels.Add(i - axis.LabelStyle.Interval / 2, i + axis.LabelStyle.Interval / 2, x);
        }
      }
    }

    private void zoomButton_CheckedChanged(object sender, EventArgs e) {
      this.isSelecting = selectButton.Checked;
      this.colorRunsButton.Enabled = this.isSelecting;
      this.colorDialogButton.Enabled = this.isSelecting;
      this.hideRunsButton.Enabled = this.isSelecting;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !isSelecting;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !isSelecting;
      ClearSelectedRuns();
    }

    private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      hideRunsToolStripMenuItem.Visible = selectedRuns.Any();
    }

    private void unhideAllRunToolStripMenuItem_Click(object sender, EventArgs e) {
      visibilityConstraint.ConstraintData.Clear();
      if (Content.Constraints.Contains(visibilityConstraint)) Content.Constraints.Remove(visibilityConstraint);
    }
    private void hideRunsToolStripMenuItem_Click(object sender, EventArgs e) {
      //ToList is necessary to prevent lazy evaluation
      HideRuns(selectedRuns.ToList());
      //could not use ClearSelectedRuns as the runs are not visible anymore
      selectedRuns.Clear();
    }
    private void hideRunsButton_Click(object sender, EventArgs e) {
      //ToList is necessary to prevent lazy evaluation
      HideRuns(selectedRuns.ToList());
      //could not use ClearSelectedRuns as the runs are not visible anymore
      selectedRuns.Clear();
    }

    private void HideRuns(IEnumerable<IRun> runs) {
      visibilityConstraint.Active = false;
      if (!Content.Constraints.Contains(visibilityConstraint)) Content.Constraints.Add(visibilityConstraint);
      foreach (var run in runs) {
        visibilityConstraint.ConstraintData.Add(run);
      }
      visibilityConstraint.Active = true;
    }

    private void ClearSelectedRuns() {
      foreach (var run in selectedRuns) {
        foreach (var point in runToDataPointMapping[run]) {
          point.MarkerStyle = MarkerStyle.Circle;
          point.Color = Color.FromArgb(255 - LogTransform(transparencyTrackBar.Value), run.Color);
        }
      }
      selectedRuns.Clear();
    }

    // returns a value in [0..255]
    private int LogTransform(int x) {
      double min = transparencyTrackBar.Minimum;
      double max = transparencyTrackBar.Maximum;
      double r = (x - min) / (max - min);  // r \in [0..1]
      double l = Math.Max(Math.Min((1.0 - Math.Pow(0.05, r)) / 0.95, 1), 0); // l \in [0..1]
      return (int)Math.Round(255.0 * l);
    }

    private void openBoxPlotViewToolStripMenuItem_Click(object sender, EventArgs e) {
      RunCollectionBoxPlotView boxplotView = new RunCollectionBoxPlotView();
      boxplotView.Content = this.Content;
      boxplotView.xAxisComboBox.SelectedItem = xAxisComboBox.SelectedItem;
      boxplotView.yAxisComboBox.SelectedItem = yAxisComboBox.SelectedItem;
      boxplotView.Show();
    }

    private void getDataAsMatrixToolStripMenuItem_Click(object sender, EventArgs e) {
      int xCol = Matrix.ColumnNames.ToList().IndexOf(xAxisValue);
      int yCol = Matrix.ColumnNames.ToList().IndexOf(yAxisValue);

      var grouped = new Dictionary<string, List<string>>();
      Dictionary<double, string> reverseMapping = null;
      if (categoricalMapping.ContainsKey(xCol))
        reverseMapping = categoricalMapping[xCol].ToDictionary(x => x.Value, y => y.Key.ToString());
      foreach (var run in Content.Where(r => r.Visible)) {
        var x = GetValue(run, xAxisValue);
        object y;
        if (categoricalMapping.ContainsKey(yCol))
          y = Content.GetValue(run, yAxisValue);
        else y = GetValue(run, yAxisValue);
        if (!(x.HasValue && y != null)) continue;

        var category = reverseMapping == null ? x.Value.ToString() : reverseMapping[x.Value];
        if (!grouped.ContainsKey(category)) grouped[category] = new List<string>();
        grouped[category].Add(y.ToString());
      }

      if (!grouped.Any()) return;
      var matrix = new StringMatrix(grouped.Values.Max(x => x.Count), grouped.Count) {
        ColumnNames = grouped.Keys.ToArray()
      };
      int i = 0;
      foreach (var col in matrix.ColumnNames) {
        int j = 0;
        foreach (var y in grouped[col])
          matrix[j++, i] = y;
        i++;
      }
      matrix.SortableView = false;
      var view = MainFormManager.MainForm.ShowContent(matrix);
      view.ReadOnly = true;
    }

    private void transparencyTrackBar_ValueChanged(object sender, EventArgs e) {
      foreach (var run in Content)
        UpdateRun(run);
    }
    #endregion

    #region coloring
    private void colorResetToolStripMenuItem_Click(object sender, EventArgs e) {
      Content.UpdateOfRunsInProgress = true;

      IEnumerable<IRun> runs;
      if (selectedRuns.Any()) runs = selectedRuns;
      else runs = Content;

      foreach (var run in runs)
        run.Color = Color.Black;
      ClearSelectedRuns();

      Content.UpdateOfRunsInProgress = false;
    }
    private void colorDialogButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog(this) == DialogResult.OK) {
        this.colorRunsButton.Image = this.GenerateImage(16, 16, this.colorDialog.Color);
        colorRunsButton_Click(sender, e);
      }
    }
    private Image GenerateImage(int width, int height, Color fillColor) {
      Image colorImage = new Bitmap(width, height);
      using (Graphics gfx = Graphics.FromImage(colorImage)) {
        using (SolidBrush brush = new SolidBrush(fillColor)) {
          gfx.FillRectangle(brush, 0, 0, width, height);
        }
      }
      return colorImage;
    }

    private void colorRunsButton_Click(object sender, EventArgs e) {
      if (!selectedRuns.Any()) return;
      Content.UpdateOfRunsInProgress = true;
      foreach (var run in selectedRuns)
        run.Color = colorDialog.Color;

      ClearSelectedRuns();
      Content.UpdateOfRunsInProgress = false;
    }

    private void colorXAxisButton_Click(object sender, EventArgs e) {
      ColorRuns(xAxisValue);
    }
    private void colorYAxisButton_Click(object sender, EventArgs e) {
      ColorRuns(yAxisValue);
    }
    private void ColorRuns(string axisValue) {
      var runs = Content.Where(r => r.Visible).Select(r => new { Run = r, Value = GetValue(r, axisValue) }).Where(r => r.Value.HasValue).ToList();
      double minValue = runs.Min(r => r.Value.Value);
      double maxValue = runs.Max(r => r.Value.Value);
      double range = maxValue - minValue;
      // UpdateOfRunsInProgress has to be set to true, otherwise run_Changed is called all the time (also in other views)
      Content.UpdateOfRunsInProgress = true;
      if (range.IsAlmost(0)) {
        Color c = ColorGradient.Colors[0];
        runs.ForEach(r => r.Run.Color = c);
      } else {
        int maxColorIndex = ColorGradient.Colors.Count - 1;
        foreach (var r in runs) {
          int colorIndex = (int)(maxColorIndex * (r.Value - minValue) / (range));
          r.Run.Color = ColorGradient.Colors[colorIndex];
        }
      }
      Content.UpdateOfRunsInProgress = false;
    }
    #endregion
  }
}
