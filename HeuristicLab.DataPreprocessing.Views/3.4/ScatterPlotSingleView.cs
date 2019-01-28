#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using RegressionType = HeuristicLab.Analysis.ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Scatter Plot Single View")]
  [Content(typeof(SingleScatterPlotContent), true)]
  public sealed partial class ScatterPlotSingleView : ItemView {
    private readonly string NoGroupItem = "";

    public new SingleScatterPlotContent Content {
      get { return (SingleScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    public ScatterPlotSingleView() {
      InitializeComponent();

      regressionTypeComboBox.DataSource = Enum.GetValues(typeof(RegressionType));
      regressionTypeComboBox.SelectedItem = RegressionType.None;
      orderComboBox.DataSource = Enum.GetValues(typeof(PreprocessingChartContent.LegendOrder));
      orderComboBox.SelectedItem = PreprocessingChartContent.LegendOrder.Alphabetically;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      useGradientCheckBox.Enabled = (string)comboBoxGroup.SelectedItem != NoGroupItem;
      gradientPanel.Visible = useGradientCheckBox.Enabled && useGradientCheckBox.Checked; ;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
      }
    }

    private void InitData() {
      IEnumerable<string> variables = Content.PreprocessingData.GetDoubleVariableNames();

      comboBoxXVariable.Items.Clear();
      comboBoxYVariable.Items.Clear();
      comboBoxGroup.Items.Clear();

      comboBoxXVariable.Items.AddRange(variables.ToArray());
      comboBoxYVariable.Items.AddRange(variables.ToArray());
      comboBoxGroup.Items.Add(NoGroupItem);
      foreach (string var in PreprocessingChartContent.GetVariableNamesForGrouping(Content.PreprocessingData, 50)) {
        comboBoxGroup.Items.Add(var);
      }
      comboBoxGroup.SelectedItem = Content.GroupingVariable ?? NoGroupItem;

      // use x and y variable from content
      if (Content.SelectedXVariable != null && Content.SelectedYVariable != null && Content.GroupingVariable != null) {
        comboBoxXVariable.SelectedItem = Content.SelectedXVariable;
        comboBoxYVariable.SelectedItem = Content.SelectedYVariable;
        comboBoxGroup.SelectedItem = Content.GroupingVariable;
      } else {
        if (variables.Count() >= 2) {
          comboBoxXVariable.SelectedIndex = 0;
          comboBoxYVariable.SelectedIndex = 1;
          comboBoxGroup.SelectedIndex = 0;
          UpdateScatterPlot();
        }
      }
    }

    private void UpdateScatterPlot() {
      if (comboBoxXVariable.SelectedItem != null && comboBoxYVariable.SelectedItem != null && comboBoxGroup.SelectedItem != null) {
        var xVariable = (string)comboBoxXVariable.SelectedItem;
        var yVariable = (string)comboBoxYVariable.SelectedItem;
        var groupVariable = (string)comboBoxGroup.SelectedItem;
        var legendOrder = (PreprocessingChartContent.LegendOrder)orderComboBox.SelectedItem;

        ScatterPlot scatterPlot = ScatterPlotContent.CreateScatterPlot(Content.PreprocessingData, xVariable, yVariable, groupVariable, legendOrder);
        //rows are saved and removed to avoid firing of visual property changed events
        var rows = scatterPlot.Rows.ToList();
        scatterPlot.Rows.Clear();
        var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
        int order = (int)polynomialRegressionOrderNumericUpDown.Value;
        foreach (var row in rows) {
          row.VisualProperties.PointSize = 6;
          row.VisualProperties.IsRegressionVisibleInLegend = false;
          row.VisualProperties.RegressionType = regressionType;
          row.VisualProperties.PolynomialRegressionOrder = order;
          row.VisualProperties.IsVisibleInLegend = !useGradientCheckBox.Checked;
        }
        scatterPlot.Rows.AddRange(rows);
        var vp = scatterPlot.VisualProperties;
        vp.Title = string.Empty;
        vp.XAxisTitle = xVariable;
        vp.YAxisTitle = yVariable;

        scatterPlotView.Content = scatterPlot;

        //save selected x and y variable in content
        this.Content.SelectedXVariable = (string)comboBoxXVariable.SelectedItem;
        this.Content.SelectedYVariable = (string)comboBoxYVariable.SelectedItem;
        this.Content.GroupingVariable = (string)comboBoxGroup.SelectedItem;
      }
    }

    private void comboBoxXVariable_SelectedIndexChanged(object sender, EventArgs e) {
      var oldPlot = scatterPlotView.Content;
      UpdateScatterPlot();
      var newPlot = scatterPlotView.Content;

      if (oldPlot == null || newPlot == null) return;
      newPlot.VisualProperties.YAxisMinimumAuto = oldPlot.VisualProperties.YAxisMinimumAuto;
      newPlot.VisualProperties.YAxisMaximumAuto = oldPlot.VisualProperties.YAxisMaximumAuto;
      newPlot.VisualProperties.YAxisMinimumFixedValue = oldPlot.VisualProperties.YAxisMinimumFixedValue;
      newPlot.VisualProperties.YAxisMaximumFixedValue = oldPlot.VisualProperties.YAxisMaximumFixedValue;

      foreach (var x in newPlot.Rows.Zip(oldPlot.Rows, (nr, or) => new { nr, or })) {
        var newVisuapProperties = (ScatterPlotDataRowVisualProperties)x.or.VisualProperties.Clone();
        newVisuapProperties.DisplayName = x.nr.VisualProperties.DisplayName;
        x.nr.VisualProperties = newVisuapProperties;
      }
    }

    private void comboBoxYVariable_SelectedIndexChanged(object sender, EventArgs e) {
      SuspendRepaint();
      var oldPlot = scatterPlotView.Content;
      UpdateScatterPlot();
      var newPlot = scatterPlotView.Content;

      if (oldPlot == null || newPlot == null) return;
      newPlot.VisualProperties.XAxisMinimumAuto = oldPlot.VisualProperties.XAxisMinimumAuto;
      newPlot.VisualProperties.XAxisMaximumAuto = oldPlot.VisualProperties.XAxisMaximumAuto;
      newPlot.VisualProperties.XAxisMinimumFixedValue = oldPlot.VisualProperties.XAxisMinimumFixedValue;
      newPlot.VisualProperties.XAxisMaximumFixedValue = oldPlot.VisualProperties.XAxisMaximumFixedValue;

      foreach (var x in newPlot.Rows.Zip(oldPlot.Rows, (nr, or) => new { nr, or })) {
        var newVisuapProperties = (ScatterPlotDataRowVisualProperties)x.or.VisualProperties.Clone();
        newVisuapProperties.DisplayName = x.nr.VisualProperties.DisplayName;
        x.nr.VisualProperties = newVisuapProperties;
      }
      ResumeRepaint(true);
    }

    private void comboBoxGroup_SelectedIndexChanged(object sender, EventArgs e) {
      useGradientCheckBox.Enabled = (string)comboBoxGroup.SelectedItem != NoGroupItem && Content.PreprocessingData.GetDoubleVariableNames().Contains((string)comboBoxGroup.SelectedItem);
      gradientPanel.Visible = useGradientCheckBox.Enabled && useGradientCheckBox.Checked;
      UpdateScatterPlot();
    }

    #region Regression Line
    private void regressionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
      polynomialRegressionOrderNumericUpDown.Enabled = regressionType == RegressionType.Polynomial;

      UpdateRegressionLine();
    }

    private void polynomialRegressionOrderNumericUpDown_ValueChanged(object sender, EventArgs e) {
      UpdateRegressionLine();
    }

    private void UpdateRegressionLine() {
      if (Content == null) return;

      var regressionType = (RegressionType)regressionTypeComboBox.SelectedValue;
      int order = (int)polynomialRegressionOrderNumericUpDown.Value;

      foreach (var row in scatterPlotView.Content.Rows) {
        row.VisualProperties.IsRegressionVisibleInLegend = false;
        row.VisualProperties.RegressionType = regressionType;
        row.VisualProperties.PolynomialRegressionOrder = order;
      }
    }
    #endregion

    private void useGradientCheckBox_CheckedChanged(object sender, EventArgs e) {
      gradientPanel.Visible = useGradientCheckBox.Enabled && useGradientCheckBox.Checked;

      // remove rows and re-add them later to avoid firing visual property changd events
      var rows = scatterPlotView.Content.Rows.ToDictionary(r => r.Name, r => r);
      scatterPlotView.Content.Rows.Clear();

      if (useGradientCheckBox.Checked) {
        var groupVariable = (string)comboBoxGroup.SelectedItem;
        if (groupVariable == NoGroupItem) return;

        var groupValues = Content.PreprocessingData.GetValues<double>(Content.PreprocessingData.GetColumnIndex(groupVariable))
          .Distinct().OrderBy(x => x).ToList();
        double min = groupValues.FirstOrDefault(x => !double.IsNaN(x)), max = groupValues.LastOrDefault(x => !double.IsNaN(x));
        foreach (var group in groupValues) {
          ScatterPlotDataRow row;
          if (rows.TryGetValue(group.ToString("R"), out row)) {
            row.VisualProperties.Color = GetColor(group, min, max);
            row.VisualProperties.IsVisibleInLegend = false;
          }
        }
        gradientMinimumLabel.Text = min.ToString("G5");
        gradientMaximumLabel.Text = max.ToString("G5");
      } else {
        foreach (var row in rows.Values) {
          row.VisualProperties.Color = Color.Empty;
          row.VisualProperties.IsVisibleInLegend = true;
        }
      }
      scatterPlotView.Content.Rows.AddRange(rows.Values);
    }

    private static Color GetColor(double value, double min, double max) {
      if (double.IsNaN(value)) {
        return Color.Black;
      }
      var colors = ColorGradient.Colors;
      int index = (int)((colors.Count - 1) * (value - min) / (max - min));
      if (index >= colors.Count) index = colors.Count - 1;
      if (index < 0) index = 0;
      return colors[index];
    }

    private void orderComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      UpdateScatterPlot();
    }
  }
}

