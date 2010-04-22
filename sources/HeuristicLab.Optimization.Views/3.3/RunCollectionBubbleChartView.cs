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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.MainForm;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection BubbleChart")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionBubbleChartView : AsynchronousContentView {
    private const string constantLabel = "constant";
    private Dictionary<int, Dictionary<object, double>> categoricalMapping;
    private Dictionary<IRun, double> xJitter;
    private Dictionary<IRun, double> yJitter;
    private double xJitterFactor = 0.0;
    private double yJitterFactor = 0.0;
    private Random random;
    private bool isSelecting = false;

    public RunCollectionBubbleChartView() {
      InitializeComponent();
      Caption = "Run Collection Bubble Chart";

      this.categoricalMapping = new Dictionary<int, Dictionary<object, double>>();
      this.xJitter = new Dictionary<IRun, double>();
      this.yJitter = new Dictionary<IRun, double>();
      this.random = new Random();
      this.colorDialog.Color = Color.Black;
      this.colorButton.Image = this.GenerateImage(16, 16, this.colorDialog.Color);
      this.isSelecting = false;

      this.chart.Series[0]["BubbleMaxSize"] = "0";
      this.chart.Series[0]["BubbleMaxScale"] = "Auto";
      this.chart.Series[0]["BubbleMinScale"] = "Auto";
      this.chart.Series[0].SmartLabelStyle.Enabled = true;
      this.chart.Series[0].SmartLabelStyle.IsMarkerOverlappingAllowed = false;
      this.chart.Series[0].SmartLabelStyle.IsOverlappedHidden = true;

      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !this.isSelecting;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !this.isSelecting;
      this.chart.ChartAreas[0].CursorX.Interval = 0;
      this.chart.ChartAreas[0].CursorY.Interval = 0;

    }

    public RunCollectionBubbleChartView(RunCollection content)
      : this() {
      Content = content;
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
    protected virtual void RegisterRunEvents(IEnumerable<IRun> runs) {
      foreach (IRun run in runs)
        run.Changed += new EventHandler(run_Changed);
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
      IRun run = (IRun)sender;
      DataPoint point = this.chart.Series[0].Points.Where(p => p.Tag == run).SingleOrDefault();
      if (point != null) {
        point.Color = run.Color;
        if (!run.Visible)
          this.chart.Series[0].Points.Remove(point);
      } else
        AddDataPoint(run);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.categoricalMapping.Clear();
      this.UpdateComboBoxes();
    }
    private void Content_ColumnNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ColumnNamesChanged), sender, e);
      else
        UpdateComboBoxes();
    }

    private void UpdateComboBoxes() {
      this.xAxisComboBox.Items.Clear();
      this.yAxisComboBox.Items.Clear();
      this.sizeComboBox.Items.Clear();
      this.xAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
      this.yAxisComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
      this.sizeComboBox.Items.Add(constantLabel);
      this.sizeComboBox.Items.AddRange(Matrix.ColumnNames.ToArray());
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
      foreach (IRun run in this.Content)
        this.AddDataPoint(run);
    }
    private void AddDataPoint(IRun run) {
      double? xValue;
      double? yValue;
      double? sizeValue;
      Series series = this.chart.Series[0];
      int row = this.Content.ToList().IndexOf(run);
      xValue = GetValue(row, xAxisComboBox.SelectedIndex);
      yValue = GetValue(row, yAxisComboBox.SelectedIndex);
      sizeValue = 1.0;
      if (xValue.HasValue && yValue.HasValue) {
        if (sizeComboBox.SelectedIndex > 0)
          sizeValue = GetValue(row, sizeComboBox.SelectedIndex - 1);
        xValue = xValue.Value + xValue.Value * GetXJitter(Content.ElementAt(row)) * xJitterFactor;
        yValue = yValue.Value + yValue.Value * GetYJitter(Content.ElementAt(row)) * yJitterFactor;
        if (run.Visible) {
          DataPoint point = new DataPoint(xValue.Value, new double[] { yValue.Value, sizeValue.Value });
          point.Tag = run;
          point.Color = run.Color;
          series.Points.Add(point);
        }
      }
    }
    private double? GetValue(int row, int column) {
      if (column < 0 || row < 0)
        return null;

      IItem value = Content.GetValue(row, column);
      DoubleValue doubleValue = value as DoubleValue;
      IntValue intValue = value as IntValue;
      double ret;

      if (doubleValue != null)
        ret = doubleValue.Value;
      else if (intValue != null)
        ret = intValue.Value;
      else
        ret = GetCategoricalValue(column, Matrix.GetValue(row, column));

      return ret;
    }
    private double GetCategoricalValue(int dimension, object c) {
      if (!this.categoricalMapping.ContainsKey(dimension))
        this.categoricalMapping[dimension] = new Dictionary<object, double>();
      if (!this.categoricalMapping[dimension].ContainsKey(c)) {
        if (this.categoricalMapping[dimension].Values.Count == 0)
          this.categoricalMapping[dimension][c] = 1.0;
        else
          this.categoricalMapping[dimension][c] = this.categoricalMapping[dimension].Values.Max() + 1.0;
      }
      return this.categoricalMapping[dimension][c];
    }

    #region drag and drop
    private IRun draggedRun;
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      if (h.ChartElementType == ChartElementType.DataPoint) {
        IRun run = (IRun)((DataPoint)h.Object).Tag;
        if (e.Clicks >= 2) {
          IContentView view = MainFormManager.CreateDefaultView(run);
          view.ReadOnly = this.ReadOnly;
          view.Locked = this.Locked;
          view.Show();
        } else {
          this.draggedRun = run;
          this.chart.ChartAreas[0].CursorX.SetSelectionPosition(double.NaN, double.NaN);
          this.chart.ChartAreas[0].CursorY.SetSelectionPosition(double.NaN, double.NaN);
        }
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
        this.chart.ChartAreas[0].CursorX.SetSelectionPosition(double.NaN, double.NaN);
        this.chart.ChartAreas[0].CursorY.SetSelectionPosition(double.NaN, double.NaN);
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
      string newTooltipText;
      string oldTooltipText;
      if (h.ChartElementType == ChartElementType.DataPoint) {
        IRun run = (IRun)((DataPoint)h.Object).Tag;
        newTooltipText = run.Name + System.Environment.NewLine;
        newTooltipText += xAxisComboBox.SelectedItem + " : " + Content.GetValue(run, xAxisComboBox.SelectedIndex).ToString() + Environment.NewLine;
        newTooltipText += yAxisComboBox.SelectedItem + " : " + Content.GetValue(run, yAxisComboBox.SelectedIndex).ToString() + Environment.NewLine; ;
      } else
        newTooltipText = string.Empty;
      oldTooltipText = this.tooltip.GetToolTip(chart);
      if (newTooltipText != oldTooltipText)
        this.tooltip.SetToolTip(chart, newTooltipText);
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
      SetCustomAxisLabels(xAxis, xAxisComboBox.SelectedIndex);
      SetCustomAxisLabels(yAxis, yAxisComboBox.SelectedIndex);
    }
    private void SetCustomAxisLabels(Axis axis, int dimension) {
      axis.CustomLabels.Clear();
      if (categoricalMapping.ContainsKey(dimension)) {
        CustomLabel label = null;
        foreach (var pair in categoricalMapping[dimension]) {
          label = axis.CustomLabels.Add(pair.Value - 0.5, pair.Value + 0.5, pair.Key.ToString());
          label.GridTicks = GridTickTypes.TickMark;
        }
        axis.IsLabelAutoFit = false;
        axis.LabelStyle.Enabled = true;
        axis.LabelStyle.Angle = 0;
        axis.LabelStyle.TruncatedLabels = true;
      }
    }

    private string CreateTooltip(int runIndex) {
      StringBuilder builder = new StringBuilder();
      builder.AppendLine(this.Content.ElementAt(runIndex).Name);
      int columnIndex = 0;
      foreach (string columnName in Matrix.ColumnNames) {
        builder.Append(columnName);
        builder.Append(": ");
        builder.AppendLine(Matrix.GetValue(runIndex, columnIndex));
        columnIndex++;
      }
      return builder.ToString();
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
    #endregion
  }
}
