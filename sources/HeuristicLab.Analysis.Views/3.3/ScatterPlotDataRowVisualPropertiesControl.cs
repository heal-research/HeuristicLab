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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Analysis.Views {
  [View("ScatterPlot DataRow Visual Properties")]
  public partial class ScatterPlotDataRowVisualPropertiesControl : UserControl {
    protected bool SuppressEvents { get; set; }

    private ScatterPlotDataRowVisualProperties content;
    public ScatterPlotDataRowVisualProperties Content {
      get { return content; }
      set {
        bool changed = (value != content);
        content = value;
        if (changed) OnContentChanged();
      }
    }

    public ScatterPlotDataRowVisualPropertiesControl() {
      InitializeComponent();
      pointStyleComboBox.DataSource = Enum.GetValues(typeof(ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle));
      SetEnabledStateOfControls();
    }

    protected virtual void OnContentChanged() {
      SuppressEvents = true;
      try {
        if (Content == null) {
          pointStyleComboBox.SelectedIndex = -1;
          colorButton.BackColor = SystemColors.Control;
          colorButton.Text = "?";
          isVisibleInLegendCheckBox.Checked = false;
          pointSizeNumericUpDown.Value = 1;
          displayNameTextBox.Text = String.Empty;
        } else {
          displayNameTextBox.Text = Content.DisplayName;
          pointStyleComboBox.SelectedItem = Content.PointStyle;
          if (Content.Color.IsEmpty) {
            colorButton.BackColor = SystemColors.Control;
            colorButton.Text = "?";
          } else {
            colorButton.BackColor = Content.Color;
            colorButton.Text = String.Empty;
          }
          pointSizeNumericUpDown.Value = Content.PointSize;
          isVisibleInLegendCheckBox.Checked = Content.IsVisibleInLegend;
        }
      }
      finally { SuppressEvents = false; }
      SetEnabledStateOfControls();
    }

    protected virtual void SetEnabledStateOfControls() {
      pointStyleComboBox.Enabled = Content != null;
      colorButton.Enabled = Content != null;
      colorButton.Enabled = Content != null;
      isVisibleInLegendCheckBox.Enabled = Content != null;
      pointSizeNumericUpDown.Enabled = Content != null;
      displayNameTextBox.Enabled = Content != null;
    }

    #region Event Handlers
    private void pointStyleComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle selected = (ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle)pointStyleComboBox.SelectedValue;
        Content.PointStyle = selected;
      }
    }

    private void colorButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog() == DialogResult.OK) {
        Content.Color = colorDialog.Color;
        colorButton.BackColor = Content.Color;
        colorButton.Text = String.Empty;
      }
    }

    private void displayNameTextBox_Validated(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.DisplayName = displayNameTextBox.Text;
        }
        finally { SuppressEvents = false; }
      }
    }

    private void pointSizeNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.PointSize = (int)pointSizeNumericUpDown.Value;
      }
    }

    private void isVisibleInLegendCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.IsVisibleInLegend = isVisibleInLegendCheckBox.Checked;
      }
    }
    #endregion
  }
}
