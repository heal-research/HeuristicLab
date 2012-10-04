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
  [View("RunCollection BubbleChart")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionBubbleChartView : AsynchronousContentView {
    private enum SizeDimension { Constant = 0 }
    private enum AxisDimension { Index = 0 }

    private string xAxisValue;
    private string yAxisValue;
    private string sizeAxisValue;

    private Dictionary<IRun, List<DataPoint>> runToDataPointMapping;
    private Dictionary<int, Dictionary<object, double>> categoricalMapping;
    private Dictionary<IRun, double> xJitter;
    private Dictionary<IRun, double> yJitter;
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private Random random;
    private bool isSelecting = false;
    private bool suppressUpdates = false;

    public RunCollectionBubbleChartView() {
      InitializeComponent();

      chart.ContextMenuStrip.Items.Insert(0, hideRunToolStripMenuItem);
      chart.ContextMenuStrip.Items.Insert(1, openBoxPlotViewToolStripMenuItem);
      chart.ContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(ContextMenuStrip_Opening);

      runToDataPointMapping = new Dictionary<IRun, List<DataPoint>>();
      categoricalMapping = new Dictionary<int, Dictionary<object, double>>();
      xJitter = new Dictionary<IRun, double>();
      yJitter = new Dictionary<IRun, double>();
      random = new Random();

      colorDialog.Color = Color.Black;
      colorButton.Image = this.GenerateImage(16, 16, this.colorDialog.Color);
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
    private void run_Changed(object sender, EventArgs e) {
      if (InvokeRequired)
        this.Invoke(new EventHandler(run_Changed), sender, e);
      else {
        IRun run = (IRun)sender;
        UpdateRun(run);
      }
    }

    private void UpdateRun(IRun run) {
      if (!suppressUpdates) {
        if (runToDataPointMapping.ContainsKey(run)) {
          foreach (DataPoint point in runToDataPointMapping[run]) {
            point.Color = run.Color;
            if (!run.Visible) {
              this.chart.Series[0].Points.Remove(point);
              UpdateCursorInterval();
              chart.ChartAreas[0].RecalculateAxesScale();
            }
          }
          if (!run.Visible) runToDataPointMapping.Remove(run);
        } else {
          AddDataPoint(run);
          UpdateCursorInterval();
          chart.ChartAreas[0].RecalculateAxesScale();
        }

        if (this.chart.Series[0].Points.Count == 0)
          noRunsLabel.Visible = true;
        else
          noRunsLabel.Visible = false;
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.categoricalMapping.Clear();
      UpdateComboBoxes();
      UpdateDataPoints();
      UpdateCaption();
    }
    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else
        UpdateComboBoxes();
    }

    private void UpdateCaption() {
      Caption = Content != null ? Content.AlgorithmName + "Bubble Chart" : ViewAttribute.GetViewName(GetType());
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
        if (changed)
          UpdateDataPoints();
      }
    }


    private void Content_UpdateOfRunsInProgressChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_UpdateOfRunsInProgressChanged), sender, e);
      else {
        suppressUpdates = Content.UpdateOfRunsInProgress;
        if (!suppressUpdates) UpdateDataPoints();
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
        this.categoricalMapping.Clear();
        UpdateDataPoints();
      }
    }

    private void UpdateDataPoints() {
      Series series = this.chart.Series[0];
      series.Points.Clear();
      runToDataPointMapping.Clear();

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
      var xAxis = chart.ChartAreas[0].AxisX;
      var yAxis = chart.ChartAreas[0].AxisY;
      xTrackBar.Value = 0;
      yTrackBar.Value = 0;
      SetAutomaticUpdateOfAxis(xAxis, true);
      SetAutomaticUpdateOfAxis(yAxis, true);
    }

    private void UpdateMarkerSizes() {
      double[] sizeValues = this.chart.Series[0].Points.Select(p => p.YValues[1]).ToArray();
      double minSizeValue = sizeValues.Min();
      double maxSizeValue = sizeValues.Max();

      for (int i = 0; i < sizeValues.Length; i++) {
        DataPoint point = this.chart.Series[0].Points[i];
        double sizeRange = maxSizeValue - minSizeValue;
        double relativeSize = (point.YValues[1] - minSizeValue);

        if (sizeRange > double.Epsilon) relativeSize /= sizeRange;
        else relativeSize = 1;

        point.MarkerSize = (int)Math.Round((sizeTrackBar.Value - sizeTrackBar.Minimum) * relativeSize + sizeTrackBar.Minimum);
      }
    }

    private void UpdateDataPointJitter() {
      var xAxis = this.chart.ChartAreas[0].AxisX;
      var yAxis = this.chart.ChartAreas[0].AxisY;
      SetAutomaticUpdateOfAxis(xAxis, false);
      SetAutomaticUpdateOfAxis(yAxis, false);

      foreach (DataPoint point in chart.Series[0].Points) {
        IRun run = (IRun)point.Tag;
        double xValue = GetValue(run, xAxisValue).Value;
        double yValue = GetValue(run, yAxisValue).Value;

        if (!xJitterFactor.IsAlmost(0.0))
          xValue += 0.1 * GetXJitter(run) * xJitterFactor * (xAxis.Maximum - xAxis.Minimum);
        if (!yJitterFactor.IsAlmost(0.0))
          yValue += 0.1 * GetYJitter(run) * yJitterFactor * (yAxis.Maximum - yAxis.Minimum);

        point.XValue = xValue;
        point.YValues[0] = yValue;
      }
    }

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
          point.Color = run.Color;
          series.Points.Add(point);
          if (!runToDataPointMapping.ContainsKey(run)) runToDataPointMapping.Add(run, new List<DataPoint>());
          runToDataPointMapping[run].Add(point);
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
        case AxisDimension.Index: {
            value = Content.ToList().IndexOf(run);
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
      Series series = chart.Series[0];
      double[] xValues = (from point in series.Points
                          where !point.IsEmpty
                          select point.XValue)
                    .DefaultIfEmpty(1.0)
                    .ToArray();
      double[] yValues = (from point in series.Points
                          where !point.IsEmpty
                          select point.YValues[0])
                    .DefaultIfEmpty(1.0)
                    .ToArray();

      double xRange = xValues.Max() - xValues.Min();
      double yRange = yValues.Max() - yValues.Min();
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
      if (isSelecting) {
        System.Windows.Forms.DataVisualization.Charting.Cursor xCursor = chart.ChartAreas[0].CursorX;
        System.Windows.Forms.DataVisualization.Charting.Cursor yCursor = chart.ChartAreas[0].CursorY;

        double minX = Math.Min(xCursor.SelectionStart, xCursor.SelectionEnd);
        double maxX = Math.Max(xCursor.SelectionStart, xCursor.SelectionEnd);
        double minY = Math.Min(yCursor.SelectionStart, yCursor.SelectionEnd);
        double maxY = Math.Max(yCursor.SelectionStart, yCursor.SelectionEnd);

        //check for click to select model
        if (minX == maxX && minY == maxY) {
          HitTestResult hitTest = chart.HitTest(e.X, e.Y);
          if (hitTest.ChartElementType == ChartElementType.DataPoint) {
            int pointIndex = hitTest.PointIndex;
            IRun run = (IRun)this.chart.Series[0].Points[pointIndex].Tag;
            run.Color = colorDialog.Color;
          }
        } else {
          List<DataPoint> selectedPoints = new List<DataPoint>();
          foreach (DataPoint p in this.chart.Series[0].Points) {
            if (p.XValue >= minX && p.XValue < maxX &&
              p.YValues[0] >= minY && p.YValues[0] < maxY) {
              selectedPoints.Add(p);
            }
          }
          foreach (DataPoint p in selectedPoints) {
            IRun run = (IRun)p.Tag;
            run.Color = colorDialog.Color;
          }
        }
        this.chart.ChartAreas[0].CursorX.SelectionStart = this.chart.ChartAreas[0].CursorX.SelectionEnd;
        this.chart.ChartAreas[0].CursorY.SelectionStart = this.chart.ChartAreas[0].CursorY.SelectionEnd;
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
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
        for (double i = axis.Minimum; i <= axis.Maximum; i += axis.LabelStyle.Interval) {
          TimeSpan time = TimeSpan.FromSeconds(i);
          string x = string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
          axis.CustomLabels.Add(i - axis.LabelStyle.Interval / 2, i + axis.LabelStyle.Interval / 2, x);
        }
      }
    }

    private void zoomButton_CheckedChanged(object sender, EventArgs e) {
      this.isSelecting = selectButton.Checked;
      this.colorButton.Enabled = this.isSelecting;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !isSelecting;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !isSelecting;
    }
    private void colorButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog(this) == DialogResult.OK) {
        this.colorButton.Image = this.GenerateImage(16, 16, this.colorDialog.Color);
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

    private IRun runToHide = null;
    private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      var pos = Control.MousePosition;
      var chartPos = chart.PointToClient(pos);

      HitTestResult h = this.chart.HitTest(chartPos.X, chartPos.Y);
      if (h.ChartElementType == ChartElementType.DataPoint) {
        runToHide = (IRun)((DataPoint)h.Object).Tag;
        hideRunToolStripMenuItem.Visible = true;
      } else {
        runToHide = null;
        hideRunToolStripMenuItem.Visible = false;
      }

    }
    private void hideRunToolStripMenuItem_Click(object sender, EventArgs e) {
      var constraint = Content.Constraints.OfType<RunCollectionContentConstraint>().Where(c => c.Active).FirstOrDefault();
      if (constraint == null) {
        constraint = new RunCollectionContentConstraint();
        Content.Constraints.Add(constraint);
        constraint.Active = true;
      }
      constraint.ConstraintData.Add(runToHide);
    }

    private void openBoxPlotViewToolStripMenuItem_Click(object sender, EventArgs e) {
      RunCollectionBoxPlotView boxplotView = new RunCollectionBoxPlotView();
      boxplotView.Content = this.Content;
      boxplotView.xAxisComboBox.SelectedItem = xAxisComboBox.SelectedItem;
      boxplotView.yAxisComboBox.SelectedItem = yAxisComboBox.SelectedItem;
      boxplotView.Show();
    }
    #endregion

    #region Automatic coloring
    private void colorXAxisButton_Click(object sender, EventArgs e) {
      ColorRuns(xAxisValue);
    }

    private void colorYAxisButton_Click(object sender, EventArgs e) {
      ColorRuns(yAxisValue);
    }

    private void ColorRuns(string axisValue) {
      var runs = Content.Where(r => r.Visible).Select(r => new { Run = r, Value = GetValue(r, axisValue) }).Where(r => r.Value.HasValue);
      double minValue = runs.Min(r => r.Value.Value);
      double maxValue = runs.Max(r => r.Value.Value);
      double range = maxValue - minValue;

      foreach (var r in runs) {
        int colorIndex = 0;
        if (!range.IsAlmost(0)) colorIndex = (int)((ColorGradient.Colors.Count - 1) * (r.Value.Value - minValue) / (range));
        r.Run.Color = ColorGradient.Colors[colorIndex];
      }
    }
    #endregion
  }
}
