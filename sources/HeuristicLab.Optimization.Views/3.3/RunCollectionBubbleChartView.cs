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

      this.chart.Series[0]["BubbleMaxSize"] = "0";
      this.chart.Series[0]["BubbleMaxScale"] = "Auto";
      this.chart.Series[0]["BubbleMinScale"] = "Auto";
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !this.isSelecting;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !this.isSelecting;
      this.chart.ChartAreas[0].CursorX.Interval = 0;
      this.chart.ChartAreas[0].CursorY.Interval = 0;

      base.ReadOnly = true;
    }

    public RunCollectionBubbleChartView(RunCollection content)
      : this() {
      Content = content;
    }

    public new RunCollection Content {
      get { return (RunCollection)base.Content; }
      set { base.Content = value; }
    }
    public override bool ReadOnly {
      get { return base.ReadOnly; }
      set { /*not needed because results are always readonly */}
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Reset += new EventHandler(Content_Reset);
      Content.ColumnNamesChanged += new EventHandler(Content_ColumnNamesChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.Reset -= new EventHandler(Content_Reset);
      Content.ColumnNamesChanged -= new EventHandler(Content_ColumnNamesChanged);
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
      this.xAxisComboBox.Items.AddRange(Content.ColumnNames.ToArray());
      this.yAxisComboBox.Items.AddRange(Content.ColumnNames.ToArray());
      this.sizeComboBox.Items.Add(constantLabel);
      this.sizeComboBox.Items.AddRange(Content.ColumnNames.ToArray());
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

      double? xValue;
      double? yValue;
      double? sizeValue;
      for (int row = 0; row < Content.Count; row++) {
        xValue = GetValue(row, xAxisComboBox.SelectedIndex);
        yValue = GetValue(row, yAxisComboBox.SelectedIndex);
        sizeValue = 1.0;
        if (xValue.HasValue && yValue.HasValue) {
          if (sizeComboBox.SelectedIndex > 0)
            sizeValue = GetValue(row, sizeComboBox.SelectedIndex-1);
          xValue = xValue.Value + xValue.Value * GetXJitter(Content.ElementAt(row)) * xJitterFactor;
          yValue = yValue.Value + yValue.Value * GetYJitter(Content.ElementAt(row)) * yJitterFactor;
          DataPoint point = new DataPoint(xValue.Value, new double[] { yValue.Value, sizeValue.Value });
          point.ToolTip = this.CreateTooltip(row);
          point.Tag = this.Content.ElementAt(row);
          series.Points.Add(point);
        }
      }
    }
    private double? GetValue(int row, int column) {
      if (column < 0 || row < 0)
        return null;

      string value = Content.GetValue(row, column);
      double d;
      double? ret = null;
      if (double.TryParse(value, out d))
        ret = d;
      else if (value != null)
        ret = GetCategoricalValue(column, value);
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
    private bool isDragOperationInProgress = false;

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      if (h.ChartElementType == ChartElementType.DataPoint) {
        this.draggedRun = (IRun)((DataPoint)h.Object).Tag;
        this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
        this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
        this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
        this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
      }
    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult h = this.chart.HitTest(e.X, e.Y);
      if (this.draggedRun != null && h.ChartElementType != ChartElementType.DataPoint) {
        //this.isDragOperationInProgress = true;
        DataObject data = new DataObject();
        data.SetData("Type", draggedRun.GetType());
        data.SetData("Value", draggedRun);
        if (ReadOnly) {
          DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
        } else {
          DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
          if ((result & DragDropEffects.Move) == DragDropEffects.Move)
            Content.Remove(draggedRun);
        }
        this.draggedRun = null;
      }
    }
    private void chart_LostFocus(object sender, EventArgs e) {
      if (this.isDragOperationInProgress) {
        this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = !isSelecting;
        this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = !isSelecting;
        this.chart.ChartAreas[0].CursorX.SetSelectionPosition(0, 0);
        this.chart.ChartAreas[0].CursorY.SetSelectionPosition(0, 0);
        this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
        this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
        this.isDragOperationInProgress = false;
      }
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
      foreach (string columnName in this.Content.ColumnNames) {
        builder.Append(columnName);
        builder.Append(": ");
        builder.AppendLine(this.Content.GetValue(runIndex, columnIndex));
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
