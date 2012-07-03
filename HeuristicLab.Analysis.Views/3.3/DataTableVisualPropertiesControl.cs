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

using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Analysis.Views {
  [View("DataRow Visual Properties")]
  public partial class DataTableVisualPropertiesControl : UserControl {
    protected bool SuppressEvents { get; set; }

    private DataTableVisualProperties content;
    public DataTableVisualProperties Content {
      get { return content; }
      set {
        bool changed = (value != content);
        content = value;
        if (changed) OnContentChanged();
      }
    }

    public DataTableVisualPropertiesControl() {
      InitializeComponent();
      errorProvider.SetIconAlignment(xAxisPrimaryMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(xAxisPrimaryMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(xAxisSecondaryMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(xAxisSecondaryMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisPrimaryMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisPrimaryMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisSecondaryMinimumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconAlignment(yAxisSecondaryMaximumFixedTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(xAxisPrimaryMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(xAxisPrimaryMaximumFixedTextBox, 2);
      errorProvider.SetIconPadding(xAxisSecondaryMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(xAxisSecondaryMaximumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisPrimaryMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisPrimaryMaximumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisSecondaryMinimumFixedTextBox, 2);
      errorProvider.SetIconPadding(yAxisSecondaryMaximumFixedTextBox, 2);
    }

    protected virtual void OnContentChanged() {
      SuppressEvents = true;
      try {
        if (Content == null) {
          titleFontLabel.Text = "( none )";
          axisFontLabel.Text = "( none )";
          titleTextBox.Text = string.Empty;

          xAxisPrimaryTitleTextBox.Text = string.Empty;
          xAxisPrimaryMinimumAutoRadioButton.Checked = false;
          xAxisPrimaryMinimumFixedRadioButton.Checked = false;
          xAxisPrimaryMinimumFixedTextBox.Text = string.Empty;
          xAxisPrimaryMaximumAutoRadioButton.Checked = false;
          xAxisPrimaryMaximumFixedRadioButton.Checked = false;
          xAxisPrimaryMaximumFixedTextBox.Text = string.Empty;
          xAxisSecondaryTitleTextBox.Text = string.Empty;
          xAxisSecondaryMinimumAutoRadioButton.Checked = false;
          xAxisSecondaryMinimumFixedRadioButton.Checked = false;
          xAxisSecondaryMinimumFixedTextBox.Text = string.Empty;
          xAxisSecondaryMaximumAutoRadioButton.Checked = false;
          xAxisSecondaryMaximumFixedRadioButton.Checked = false;
          xAxisSecondaryMaximumFixedTextBox.Text = string.Empty;

          yAxisPrimaryTitleTextBox.Text = string.Empty;
          yAxisPrimaryMinimumAutoRadioButton.Checked = false;
          yAxisPrimaryMinimumFixedRadioButton.Checked = false;
          yAxisPrimaryMinimumFixedTextBox.Text = string.Empty;
          yAxisPrimaryMaximumAutoRadioButton.Checked = false;
          yAxisPrimaryMaximumFixedRadioButton.Checked = false;
          yAxisPrimaryMaximumFixedTextBox.Text = string.Empty;
          yAxisSecondaryTitleTextBox.Text = string.Empty;
          yAxisSecondaryMinimumAutoRadioButton.Checked = false;
          yAxisSecondaryMinimumFixedRadioButton.Checked = false;
          yAxisSecondaryMinimumFixedTextBox.Text = string.Empty;
          yAxisSecondaryMaximumAutoRadioButton.Checked = false;
          yAxisSecondaryMaximumFixedRadioButton.Checked = false;
          yAxisSecondaryMaximumFixedTextBox.Text = string.Empty;
        } else {
          titleFontLabel.Text = "( " + FormatFont(Content.TitleFont) + " )";
          axisFontLabel.Text = "( " + FormatFont(Content.AxisTitleFont) + " )";
          titleTextBox.Text = Content.Title;

          xAxisPrimaryTitleTextBox.Text = Content.XAxisTitle;
          xAxisPrimaryMinimumAutoRadioButton.Checked = Content.XAxisMinimumAuto;
          xAxisPrimaryMinimumFixedRadioButton.Checked = !Content.XAxisMinimumAuto;
          xAxisPrimaryMinimumFixedTextBox.Text = Content.XAxisMinimumFixedValue.ToString();
          xAxisPrimaryMaximumAutoRadioButton.Checked = Content.XAxisMaximumAuto;
          xAxisPrimaryMaximumFixedRadioButton.Checked = !Content.XAxisMaximumAuto;
          xAxisPrimaryMaximumFixedTextBox.Text = Content.XAxisMaximumFixedValue.ToString();
          xAxisSecondaryTitleTextBox.Text = Content.SecondXAxisTitle;
          xAxisSecondaryMinimumAutoRadioButton.Checked = Content.SecondXAxisMinimumAuto;
          xAxisSecondaryMinimumFixedRadioButton.Checked = !Content.SecondXAxisMinimumAuto;
          xAxisSecondaryMinimumFixedTextBox.Text = Content.SecondXAxisMinimumFixedValue.ToString();
          xAxisSecondaryMaximumAutoRadioButton.Checked = Content.SecondXAxisMaximumAuto;
          xAxisSecondaryMaximumFixedRadioButton.Checked = !Content.SecondXAxisMaximumAuto;
          xAxisSecondaryMaximumFixedTextBox.Text = Content.SecondXAxisMaximumFixedValue.ToString();

          yAxisPrimaryTitleTextBox.Text = Content.YAxisTitle;
          yAxisPrimaryMinimumAutoRadioButton.Checked = Content.YAxisMinimumAuto;
          yAxisPrimaryMinimumFixedRadioButton.Checked = !Content.YAxisMinimumAuto;
          yAxisPrimaryMinimumFixedTextBox.Text = Content.YAxisMinimumFixedValue.ToString();
          yAxisPrimaryMaximumAutoRadioButton.Checked = Content.YAxisMaximumAuto;
          yAxisPrimaryMaximumFixedRadioButton.Checked = !Content.YAxisMaximumAuto;
          yAxisPrimaryMaximumFixedTextBox.Text = Content.YAxisMaximumFixedValue.ToString();
          yAxisSecondaryTitleTextBox.Text = Content.SecondYAxisTitle;
          yAxisSecondaryMinimumAutoRadioButton.Checked = Content.SecondYAxisMinimumAuto;
          yAxisSecondaryMinimumFixedRadioButton.Checked = !Content.SecondYAxisMinimumAuto;
          yAxisSecondaryMinimumFixedTextBox.Text = Content.SecondYAxisMinimumFixedValue.ToString();
          yAxisSecondaryMaximumAutoRadioButton.Checked = Content.SecondYAxisMaximumAuto;
          yAxisSecondaryMaximumFixedRadioButton.Checked = !Content.SecondYAxisMaximumAuto;
          yAxisSecondaryMaximumFixedTextBox.Text = Content.SecondYAxisMaximumFixedValue.ToString();
        }
      }
      finally { SuppressEvents = false; }
      SetEnabledStateOfControls();
    }

    protected virtual void SetEnabledStateOfControls() {
      axisTabControl.Enabled = Content != null;
      xAxisPrimaryMinimumFixedTextBox.Enabled = xAxisPrimaryMinimumFixedRadioButton.Checked;
      xAxisPrimaryMaximumFixedTextBox.Enabled = xAxisPrimaryMaximumFixedRadioButton.Checked;
      xAxisSecondaryMinimumFixedTextBox.Enabled = xAxisSecondaryMinimumFixedRadioButton.Checked;
      xAxisSecondaryMaximumFixedTextBox.Enabled = xAxisSecondaryMaximumFixedRadioButton.Checked;

      yAxisPrimaryMinimumFixedTextBox.Enabled = yAxisPrimaryMinimumFixedRadioButton.Checked;
      yAxisPrimaryMaximumFixedTextBox.Enabled = yAxisPrimaryMaximumFixedRadioButton.Checked;
      yAxisSecondaryMinimumFixedTextBox.Enabled = yAxisSecondaryMinimumFixedRadioButton.Checked;
      yAxisSecondaryMaximumFixedTextBox.Enabled = yAxisSecondaryMaximumFixedRadioButton.Checked;
    }

    #region Event Handlers
    private void yPrimaryTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.YAxisTitle = yAxisPrimaryTitleTextBox.Text;
      }
    }

    private void ySecondaryTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.SecondYAxisTitle = yAxisSecondaryTitleTextBox.Text;
      }
    }

    private void xPrimaryTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.XAxisTitle = xAxisPrimaryTitleTextBox.Text;
      }
    }

    private void xSecondaryTitleTextBox_Validated(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        Content.SecondXAxisTitle = xAxisSecondaryTitleTextBox.Text;
      }
    }

    private void xAxisPrimaryMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
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

    private void xAxisPrimaryMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
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

    private void xAxisSecondaryMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val >= Content.SecondXAxisMaximumFixedValue) {
            errorProvider.SetError(tb, "Number must be smaller than maximum.");
            e.Cancel = true;
          } else {
            Content.SecondXAxisMinimumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void xAxisSecondaryMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val <= Content.SecondXAxisMinimumFixedValue) {
            errorProvider.SetError(tb, "Number must be greater than minimum.");
            e.Cancel = true;
          } else {
            Content.SecondXAxisMaximumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void yAxisPrimaryMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
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

    private void yAxisPrimaryMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
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

    private void yAxisSecondaryMinimumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val >= Content.SecondYAxisMaximumFixedValue) {
            errorProvider.SetError(tb, "Number must be smaller than maximum.");
            e.Cancel = true;
          } else {
            Content.SecondYAxisMinimumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void yAxisSecondaryMaximumFixedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!SuppressEvents && Content != null) {
        TextBox tb = (TextBox)sender;
        double val;
        if (double.TryParse(tb.Text, out val)) {
          if (val <= Content.SecondYAxisMinimumFixedValue) {
            errorProvider.SetError(tb, "Number must be greater than minimum.");
            e.Cancel = true;
          } else {
            Content.SecondYAxisMaximumFixedValue = val;
            errorProvider.SetError(tb, string.Empty);
          }
        } else {
          errorProvider.SetError(tb, "Not a valid number.");
          e.Cancel = true;
        }
      }
    }

    private void xAxisPrimaryMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.XAxisMinimumAuto = xAxisPrimaryMinimumAutoRadioButton.Checked;
          if (Content.XAxisMinimumAuto) xAxisPrimaryMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void xAxisPrimaryMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.XAxisMaximumAuto = xAxisPrimaryMaximumAutoRadioButton.Checked;
          if (Content.XAxisMaximumAuto) xAxisPrimaryMaximumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void xAxisSecondaryMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondXAxisMinimumAuto = xAxisSecondaryMinimumAutoRadioButton.Checked;
          if (Content.SecondXAxisMinimumAuto) xAxisSecondaryMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void xAxisSecondaryMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondXAxisMaximumAuto = xAxisSecondaryMaximumAutoRadioButton.Checked;
          if (Content.SecondXAxisMaximumAuto) xAxisSecondaryMaximumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisPrimaryMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.YAxisMinimumAuto = yAxisPrimaryMinimumAutoRadioButton.Checked;
          if (Content.YAxisMinimumAuto) yAxisPrimaryMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisPrimaryMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.YAxisMaximumAuto = yAxisPrimaryMaximumAutoRadioButton.Checked;
          if (Content.YAxisMaximumAuto) yAxisPrimaryMaximumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisSecondaryMinimumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondYAxisMinimumAuto = yAxisSecondaryMinimumAutoRadioButton.Checked;
          if (Content.SecondYAxisMinimumAuto) yAxisSecondaryMinimumFixedTextBox.Text = double.NaN.ToString();
        }
        finally { SuppressEvents = false; }
        SetEnabledStateOfControls();
      }
    }

    private void yAxisSecondaryMaximumRadioButton_CheckedChanged(object sender, System.EventArgs e) {
      if (!SuppressEvents && Content != null) {
        SuppressEvents = true;
        try {
          Content.SecondYAxisMaximumAuto = yAxisSecondaryMaximumAutoRadioButton.Checked;
          if (Content.SecondYAxisMaximumAuto) yAxisSecondaryMaximumFixedTextBox.Text = double.NaN.ToString();
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
    #endregion

    private string FormatFont(Font f) {
      if (f == null) return "default";
      else return f.Name + ", " + f.SizeInPoints.ToString() + "pt, " + f.Style.ToString();
    }
  }
}
