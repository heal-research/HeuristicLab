#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [View("DataRow Visual Properties")]
  public partial class DataRowVisualPropertiesControl : UserControl {
    protected bool SuppressEvents { get; set; }

    private DataRowVisualProperties content;
    public DataRowVisualProperties Content {
      get { return content; }
      set {
        bool changed = (value != content);
        content = value;
        if (changed) OnContentChanged();
      }
    }

    public DataRowVisualPropertiesControl() {
      InitializeComponent();
      chartTypeComboBox.DataSource = Enum.GetValues(typeof(DataRowVisualProperties.DataRowChartType));
      lineStyleComboBox.DataSource = Enum.GetValues(typeof(DataRowVisualProperties.DataRowLineStyle));
    }

    protected virtual void OnContentChanged() {
      SuppressEvents = true;
      try {
        if (Content == null) {
          chartTypeComboBox.SelectedIndex = -1;
          colorButton.BackColor = SystemColors.Control;
          colorButton.Text = "?";
          yAxisPrimaryRadioButton.Checked = false;
          yAxisSecondaryRadioButton.Checked = false;
          xAxisPrimaryRadioButton.Checked = false;
          xAxisSecondaryRadioButton.Checked = false;
          lineStyleComboBox.SelectedIndex = -1;
          startIndexZeroCheckBox.Checked = false;
          lineWidthNumericUpDown.Value = 1;
          binsNumericUpDown.Value = 1;
          binsApproximatelyRadioButton.Checked = false;
          binsExactRadioButton.Checked = false;
          displayNameTextBox.Text = String.Empty;
        } else {
          chartTypeComboBox.SelectedItem = Content.ChartType;
          if (Content.Color.IsEmpty) {
            colorButton.BackColor = SystemColors.Control;
            colorButton.Text = "?";
          } else {
            colorButton.BackColor = Content.Color;
            colorButton.Text = String.Empty;
          }
          yAxisPrimaryRadioButton.Checked = !Content.SecondYAxis;
          yAxisSecondaryRadioButton.Checked = Content.SecondYAxis;
          xAxisPrimaryRadioButton.Checked = !Content.SecondXAxis;
          xAxisSecondaryRadioButton.Checked = Content.SecondXAxis;
          lineStyleComboBox.SelectedItem = Content.LineStyle;
          startIndexZeroCheckBox.Checked = Content.StartIndexZero;
          if (Content.LineWidth < lineWidthNumericUpDown.Minimum)
            lineWidthNumericUpDown.Value = lineWidthNumericUpDown.Minimum;
          else if (Content.LineWidth > lineWidthNumericUpDown.Maximum)
            lineWidthNumericUpDown.Value = lineWidthNumericUpDown.Maximum;
          else lineWidthNumericUpDown.Value = Content.LineWidth;
          if (Content.Bins < binsNumericUpDown.Minimum)
            binsNumericUpDown.Value = binsNumericUpDown.Minimum;
          else if (Content.Bins > binsNumericUpDown.Maximum)
            binsNumericUpDown.Value = binsNumericUpDown.Maximum;
          else binsNumericUpDown.Value = Content.Bins;
          binsApproximatelyRadioButton.Checked = !Content.ExactBins;
          binsExactRadioButton.Checked = Content.ExactBins;
          displayNameTextBox.Text = Content.DisplayName;
        }
      } finally { SuppressEvents = false; }
      SetEnabledStateOfControls();
    }

    protected virtual void SetEnabledStateOfControls() {
      commonGroupBox.Enabled = Content != null;
      lineChartGroupBox.Enabled = Content != null && Content.ChartType == DataRowVisualProperties.DataRowChartType.Line;
      histoGramGroupBox.Enabled = Content != null && Content.ChartType == DataRowVisualProperties.DataRowChartType.Histogram;
    }

    #region Event Handlers
    private void chartTypeComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        DataRowVisualProperties.DataRowChartType selected = (DataRowVisualProperties.DataRowChartType)chartTypeComboBox.SelectedValue;
        Content.ChartType = selected;
        if (Content.ChartType != selected) {
          MessageBox.Show("There may be incompatibilities with other series or the data is not suited to be displayed as " + selected.ToString() + ".", "Failed to set type to " + selected.ToString());
          SuppressEvents = true;
          try {
            chartTypeComboBox.SelectedItem = Content.ChartType;
          } finally { SuppressEvents = false; }
        }
        SetEnabledStateOfControls();
      }
    }

    private void colorButton_Click(object sender, EventArgs e) {
      if (colorDialog.ShowDialog() == DialogResult.OK) {
        Content.Color = colorDialog.Color;
        colorButton.BackColor = Content.Color;
        colorButton.Text = String.Empty;
      }
    }

    private void yAxisRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondYAxis = yAxisSecondaryRadioButton.Checked;
        } finally { SuppressEvents = false; }
      }
    }

    private void xAxisRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondXAxis = xAxisSecondaryRadioButton.Checked;
        } finally { SuppressEvents = false; }
      }
    }

    private void lineStyleComboBox_SelectedValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.LineStyle = (DataRowVisualProperties.DataRowLineStyle)lineStyleComboBox.SelectedValue;
      }
    }

    private void startIndexZeroCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.StartIndexZero = startIndexZeroCheckBox.Checked;
      }
    }

    private void lineWidthNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.LineWidth = (int)lineWidthNumericUpDown.Value;
      }
    }

    private void binsNumericUpDown_ValueChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.Bins = (int)binsNumericUpDown.Value;
      }
    }

    private void binNumberRadioButton_CheckedChanged(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.ExactBins = binsExactRadioButton.Checked;
        } finally { SuppressEvents = false; }
      }
    }

    private void displayNameTextBox_Validated(object sender, EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.DisplayName = displayNameTextBox.Text;
        } finally { SuppressEvents = false; }
      }
    }
    #endregion
  }
}
