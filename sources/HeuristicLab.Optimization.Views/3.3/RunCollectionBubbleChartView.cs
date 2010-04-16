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
    private Dictionary<int, Dictionary<object, double>> categoricalMapping;

    public RunCollectionBubbleChartView() {
      InitializeComponent();
      Caption = "Run Collection Bubble Chart";
      this.categoricalMapping = new Dictionary<int, Dictionary<object, double>>();
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
      this.xAxisComboBox.Items.AddRange(Content.ColumnNames.ToArray());
      this.yAxisComboBox.Items.AddRange(Content.ColumnNames.ToArray());
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
      for (int row = 0; row < Content.Count; row++) {
        xValue = GetValue(row, xAxisComboBox.SelectedIndex);
        yValue = GetValue(row, yAxisComboBox.SelectedIndex);
        if (xValue.HasValue && yValue.HasValue)
          series.Points.Add(new DataPoint(xValue.Value, yValue.Value));
      }
    }

    private void AxisComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateDataPoints();
      UpdateAxisLabels();
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

    private void UpdateAxisLabels() {
      Axis xAxis = this.chart.ChartAreas[0].AxisX;
      Axis yAxis = this.chart.ChartAreas[0].AxisY;
      SetAxisLabels(xAxis, xAxisComboBox.SelectedIndex);
      SetAxisLabels(yAxis, yAxisComboBox.SelectedIndex);
    }
    private void SetAxisLabels(Axis axis, int dimension) {
      if (categoricalMapping.ContainsKey(dimension)) {
        axis.CustomLabels.Clear();
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
  }
}
