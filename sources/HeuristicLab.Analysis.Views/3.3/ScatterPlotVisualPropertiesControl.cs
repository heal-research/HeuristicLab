#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Analysis.Views {
  [View("Scatter Plot Visual Properties")]
  public partial class ScatterPlotVisualPropertiesControl : UserControl {
    protected bool SuppressEvents { get; set; }

    private ScatterPlotVisualProperties content;
    public ScatterPlotVisualProperties Content {
      get { return content; }
      set {
        bool changed = (value != content);
        content = value;
        if (changed) OnContentChanged();
      }
    }

    public ScatterPlotVisualPropertiesControl() {
      InitializeComponent();
      errorProvider.SetIconAlignment(xAxisMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(xAxisMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(xAxisMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(xAxisMaximumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisMaximumFixedTextBox, 2);
    }

    protected virtual void OnContentChanged() {
      SuppressEvents = true;
      try {
        if (Content == null) {
          titleFontLabel.Text = "( none )";
          axisFontLabel.Text = "( none )";
          titleTextBox.Text = string.Empty;

          xAxisTitleTextBox.Text = string.Empty;
          xAxisMinimumAutoRadioButton.Checked = false;
          xAxisMinimumFixedRadioButton.Checked = false;
          xAxisMinimumFixedTextBox.Text = string.Empty;
          xAxisMaximumAutoRadioButton.Checked = false;
          xAxisMaximumFixedRadioButton.Checked = false;
          xAxisMaximumFixedTextBox.Text = string.Empty;
          xAxisGridCheckBox.Checked = false;

          yAxisTitleTextBox.Text = string.Empty;
          yAxisMinimumAutoRadioButton.Checked = false;
          yAxisMinimumFixedRadioButton.Checked = false;
          yAxisMinimumFixedTextBox.Text = string.Empty;
          yAxisMaximumAutoRadioButton.Checked = false;
          yAxisMaximumFixedRadioButton.Checked = false;
          yAxisMaximumFixedTextBox.Text = string.Empty;
          yAxisGridCheckBox.Checked = false;
        } else {
          titleFontLabel.Text = "( " + FormatFont(Content.TitleFont) + " )";
          axisFontLabel.Text = "( " + FormatFont(Content.AxisTitleFont) + " )";
          titleTextBox.Text = Content.Title;

          xAxisTitleTextBox.Text = Content.XAxisTitle;
          xAxisMinimumAutoRadioButton.Checked = Content.XAxisMinimumAuto;
          xAxisMinimumFixedRadioButton.Checked = !Content.XAxisMinimumAuto;
          xAxisMinimumFixedTextBox.Text = Content.XAxisMinimumFixedValue.ToString();
          xAxisMaximumAutoRadioButton.Checked = Content.XAxisMaximumAuto;
          xAxisMaximumFixedRadioButton.Checked = !Content.XAxisMaximumAuto;
          xAxisMaximumFixedTextBox.Text = Content.XAxisMaximumFixedValue.ToString();
          xAxisGridCheckBox.Checked = Content.XAxisGrid;

          yAxisTitleTextBox.Text = Content.YAxisTitle;
          yAxisMinimumAutoRadioButton.Checked = Content.YAxisMinimumAuto;
          yAxisMinimumFixedRadioButton.Checked = !Content.YAxisMinimumAuto;
          yAxisMinimumFixedTextBox.Text = Content.YAxisMinimumFixedValue.ToString();
          yAxisMaximumAutoRadioButton.Checked = Content.YAxisMaximumAuto;
          yAxisMaximumFixedRadioButton.Checked = !Content.YAxisMaximumAuto;
          yAxisMaximumFixedTextBox.Text = Content.YAxisMaximumFixedValue.ToString();
          yAxisGridCheckBox.Checked = Content.YAxisGrid;
        }
      }
      finally { SuppressEvents = false; }
      SetEnabledStateOfControls();
    }

    protected virtual void SetEnabledStateOfControls() {
      axisTabControl.Enabled = Content != null;
      xAxisMinimumFixedTextBox.Enabled = xAxisMinimumFixedRadioButton.Checked;
      xAxisMaximumFixedTextBox.Enabled = xAxisMaximumFixedRadioButton.Checked;

      yAxisMinimumFixedTextBox.Enabled = yAxisMinimumFixedRadioButton.Checked;
      yAxisMaximumFixedTextBox.Enabled = yAxisMaximumFixedRadioButton.Checked;
    }

    #region Event Handlers
    private void yTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.YAxisTitle = yAxisTitleTextBox.Text;
      }
    }

    private void xTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.XAxisTitle = xAxisTitleTextBox.Text;
      }
    }

    private void xAxisMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val >= Content.XAxisMaximumFixedValue) {
            errorProvider.SetError(tb, "Number must be smaller than maximum.");
            e.Cancel = true;
          } else {
            Content.XAxisMinimumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void xAxisMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val <= Content.XAxisMinimumFixedValue) {
            errorProvider.SetError(tb, "Number must be greater than minimum.");
            e.Cancel = true;
          } else {
            Content.XAxisMaximumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void yAxisMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val >= Content.YAxisMaximumFixedValue) {
            errorProvider.SetError(tb, "Number must be smaller than maximum.");
            e.Cancel = true;
          } else {
            Content.YAxisMinimumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void yAxisMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val <= Content.YAxisMinimumFixedValue) {
            errorProvider.SetError(tb, "Number must be greater than minimum.");
            e.Cancel = true;
          } else {
            Content.YAxisMaximumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void xAxisMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.XAxisMinimumAuto = xAxisMinimumAutoRadioButton.Checked;
          if (Content.XAxisMinimumAuto) xAxisMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void xAxisMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.XAxisMaximumAuto = xAxisMaximumAutoRadioButton.Checked;
          if (Content.XAxisMaximumAuto) xAxisMaximumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.YAxisMinimumAuto = yAxisMinimumAutoRadioButton.Checked;
          if (Content.YAxisMinimumAuto) yAxisMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.YAxisMaximumAuto = yAxisMaximumAutoRadioButton.Checked;
          if (Content.YAxisMaximumAuto) yAxisMaximumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void titleFontButton_Click(object sender, System.EventArgs e) {
      titleFontDialog.Font = Content.TitleFont;
      titleFontDialog.Color = Content.TitleColor;
      if (titleFontDialog.ShowDialog() == DialogResult.OK) {
        Content.TitleFont = titleFontDialog.Font;
        Content.TitleColor = titleFontDialog.Color;
        titleFontLabel.Text = "( " + FormatFont(Content.TitleFont) + " )";
      }
    }

    private void axisFontButton_Click(object sender, System.EventArgs e) {
      axisFontDialog.Font = Content.AxisTitleFont;
      axisFontDialog.Color = Content.AxisTitleColor;
      if (axisFontDialog.ShowDialog() == DialogResult.OK) {
        Content.AxisTitleFont = axisFontDialog.Font;
        Content.AxisTitleColor = axisFontDialog.Color;
        axisFontLabel.Text = "( " + FormatFont(Content.AxisTitleFont) + " )";
      }
    }

    private void titleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.Title = titleTextBox.Text;
      }
    }

    private void yAxisGridCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.YAxisGrid = yAxisGridCheckBox.Checked;
      }
    }

    private void xAxisGridCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.XAxisGrid = xAxisGridCheckBox.Checked;
      }
    }
    #endregion

    private string FormatFont(Font f) {
      if (f == null) return "default";
      else return f.Name + ", " + f.SizeInPoints.ToString() + "pt, " + f.Style.ToString();
    }
  }
}
