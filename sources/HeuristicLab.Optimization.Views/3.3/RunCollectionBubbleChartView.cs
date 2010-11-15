#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    private Dictionary<IRun, DataPoint> runToDataPointMapping;
    private Dictionary<int, Dictionary<object, double>> categoricalMapping;
    private Dictionary<IRun, double> xJitter;
    private Dictionary<IRun, double> yJitter;
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private Random random;
    private bool isSelecting = false;

    public RunCollectionBubbleChartView() {
      InitializeComponent();

      runToDataPointMapping = new Dictionary<IRun, DataPoint>();
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
      RegisterRunEvents(Content);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Reset -= new EventHandler(Content_Reset);
      Content.ColumnNamesChanged -= new EventHandler(Content_ColumnNamesChanged);
      Content.ItemsAdded -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsAdded);
      Content.ItemsRemoved -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_ItemsRemoved);
      Content.CollectionReset -= new HeuristicLab.Collections.CollectionItemsChangedEventHandler<IRun>(Content_CollectionReset);
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
      DataPoint point = runToDataPointMapping[run];
      if (point != null) {
        point.Color = run.Color;
        if (!run.Visible) {
          this.chart.Series[0].Points.Remove(point);
          runToDataPointMapping.Remove(run);
          UpdateCursorInterval();
          chart.ChartAreas[0].RecalculateAxesScale();
        }
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

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.categoricalMapping.Clear();
      UpdateComboBoxes();
      UpdateDataPoints();
    }
    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else
        UpdateComboBoxes();
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
      if (Content != null) {
        foreach (IRun run in this.Content)
          this.AddDataPoint(run);

        //check to correct max bubble size
        if (this.chart.Series[0].Points.Select(p => p.YValues[1]).Distinct().Count() == 1)
          this.chart.Series[0]["BubbleMaxSize"] = "2";
        else
          this.chart.Series[0]["BubbleMaxSize"] = "7";

        if (this.chart.Series[0].Points.Count == 0)
          noRunsLabel.Visible = true;
        else {
          noRunsLabel.Visible = false;
          UpdateCursorInterval();
        }
      }
    }
    private void AddDataPoint(IRun run) {
      double? xValue;
      double? yValue;
      double? sizeValue;
      Series series = this.chart.Series[0];
      int row = this.Content.ToList().IndexOf(run);

      if (!xAxisComboBox.DroppedDown)
        this.xAxisValue = (string)xAxisComboBox.SelectedItem;
      if (!yAxisComboBox.DroppedDown)
        this.yAxisValue = (string)yAxisComboBox.SelectedItem;
      if (!sizeComboBox.DroppedDown)
        this.sizeAxisValue = (string)sizeComboBox.SelectedItem;

      xValue = GetValue(run, this.xAxisValue);
      yValue = GetValue(run, this.yAxisValue);
      sizeValue = GetValue(run, this.sizeAxisValue);

      if (xValue.HasValue && yValue.HasValue && sizeValue.HasValue) {
        xValue = xValue.Value;
        if (!xJitterFactor.IsAlmost(0.0))
          xValue += 0.1 * GetXJitter(run) * xJitterFactor * (this.chart.ChartAreas[0].AxisX.Maximum - this.chart.ChartAreas[0].AxisX.Minimum);
        yValue = yValue.Value;
        if (!yJitterFactor.IsAlmost(0.0))
          yValue += 0.1 * GetYJitter(run) * yJitterFactor * (this.chart.ChartAreas[0].AxisY.Maximum - this.chart.ChartAreas[0].AxisY.Minimum);
        if (run.Visible) {
          DataPoint point = new DataPoint(xValue.Value, new double[] { yValue.Value, sizeValue.Value });
          point.Tag = run;
          point.Color = run.Color;
          series.Points.Add(point);
          runToDataPointMapping[run] = point;
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

    #region drag and drop and tooltip
    private IRun draggedRun;
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      if (h.ChartElementType == ChartElementType.DataPoint) {
        IRun run = (IRun)((DataPoint)h.Object).Tag;
        if (e.Clicks >= 2) {
          IContentView view = MainFormManager.MainForm.ShowContent(run);
          if (view != null) {
            view.ReadOnly = this.ReadOnly;
            view.Locked = this.Locked;
          }
        } else
          this.draggedRun = run;
        this.chart.ChartAreas[0].CursorX.SetSelectionPosition(double.NaN, double.NaN);
        this.chart.ChartAreas[0].CursorY.SetSelectionPosition(double.NaN, double.NaN);
      }
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
      if (!Locked) {
        if (this.draggedRun != null && h.ChartElementType != ChartElementType.DataPoint) {
          DataObject data = new DataObject();
          data.SetData("Type", draggedRun.GetType());
          data.SetData("Value", draggedRun);
          if (ReadOnly)
            DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
          else {
            DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
            if ((result & DragDropEffects.Move) == DragDropEffects.Move)
              Content.Remove(draggedRun);
          }
          this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !isSelecting;
          this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !isSelecting;
          this.draggedRun = null;
        }
      }

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
      this.UpdateDataPoints();
    }

    private void AxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
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
          string x = string.Format("{0:00}:{1:00}:{2:00}", (int)time.Hours, time.Minutes, time.Seconds);
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

    private void openBoxPlotViewToolStripMenuItem_Click(object sender, EventArgs e) {
      RunCollectionBoxPlotView boxplotView = new RunCollectionBoxPlotView();
      boxplotView.Content = this.Content;
      boxplotView.xAxisComboBox.SelectedItem = xAxisComboBox.SelectedItem;
      boxplotView.yAxisComboBox.SelectedItem = yAxisComboBox.SelectedItem;
      boxplotView.Show();
    }
    #endregion

    #region automatic coloring
    private static Color[] colors;
    protected static Color[] Colors {
      get {
        if (colors == null) InitializeColors();
        return colors;
      }
    }
    private static void InitializeColors() {
      colors = new Color[256];
      int stepWidth = (256 * 4) / colors.Length;
      int currentValue;
      int currentClass;
      for (int i = 0; i < colors.Length; i++) {
        currentValue = (i * stepWidth) % 256;
        currentClass = (i * stepWidth) / 256;
        switch (currentClass) {
          case 0: { colors[i] = Color.FromArgb(0, currentValue, 255); break; }        // blue -> cyan
          case 1: { colors[i] = Color.FromArgb(0, 255, 255 - currentValue); break; }  // cyan -> green
          case 2: { colors[i] = Color.FromArgb(currentValue, 255, 0); break; }        // green -> yellow
          case 3: { colors[i] = Color.FromArgb(255, 255 - currentValue, 0); break; }  // yellow -> red
        }
      }
    }

    private void colorXAxisButton_Click(object sender, EventArgs e) {
      double minValue = chart.Series[0].Points.Min(p => p.XValue);
      double maxValue = chart.Series[0].Points.Max(p => p.XValue);
      foreach (DataPoint point in chart.Series[0].Points) {
        int colorIndex = (int)((Colors.Length - 1) * (point.XValue - minValue) / (maxValue - minValue));
        IRun run = point.Tag as IRun;
        if (run != null)
          run.Color = Colors[colorIndex];
      }
    }

    private void colorYAxisButton_Click(object sender, EventArgs e) {
      double minValue = chart.Series[0].Points.Min(p => p.YValues[0]);
      double maxValue = chart.Series[0].Points.Max(p => p.YValues[0]);
      foreach (DataPoint point in chart.Series[0].Points) {
        int colorIndex = (int)((Colors.Length - 1) * (point.YValues[0] - minValue) / (maxValue - minValue));
        IRun run = point.Tag as IRun;
        if (run != null)
          run.Color = Colors[colorIndex];
      }
    }
    #endregion
  }
}
