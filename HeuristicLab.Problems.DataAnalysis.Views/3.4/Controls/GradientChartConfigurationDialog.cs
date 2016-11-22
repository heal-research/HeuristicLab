#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  public partial class GradientChartConfigurationDialog : Form {
    private readonly GradientChart chart;

    public GradientChartConfigurationDialog(GradientChart chart) {
      this.chart = chart;
      InitializeComponent();
    }

    private void GradientChartConfigurationDialog_Shown(object sender, EventArgs e) {
      if (chart.FixedXAxisMin.HasValue && chart.FixedXAxisMax.HasValue) {
        xAutomaticCheckBox.Checked = false;
        minXTextBox.Text = chart.FixedXAxisMin.Value.ToString(CultureInfo.CurrentUICulture);
        maxXTextBox.Text = chart.FixedXAxisMax.Value.ToString(CultureInfo.CurrentUICulture);
      } else xAutomaticCheckBox.Checked = true;
      if (chart.FixedYAxisMin.HasValue && chart.FixedYAxisMax.HasValue) {
        yAutomaticCheckBox.Checked = false;
        minYTextBox.Text = chart.FixedYAxisMin.Value.ToString(CultureInfo.CurrentUICulture);
        maxYTextBox.Text = chart.FixedYAxisMax.Value.ToString(CultureInfo.CurrentUICulture);
      } else yAutomaticCheckBox.Checked = true;
      StepsNumericUpDown.Value = chart.DrawingSteps;
    }

    private async void okButton_Click(object sender, System.EventArgs e) {
      try {
        Enabled = false;
        chart.SuspendRepaint();
        if (xAutomaticCheckBox.Checked) {
          chart.FixedXAxisMin = null;
          chart.FixedXAxisMax = null;
        } else {
          var min = double.Parse(minXTextBox.Text, CultureInfo.CurrentUICulture);
          var max = double.Parse(maxXTextBox.Text, CultureInfo.CurrentUICulture);
          chart.FixedXAxisMin = min;
          chart.FixedXAxisMax = max;
        }

        if (yAutomaticCheckBox.Checked) {
          chart.FixedYAxisMin = null;
          chart.FixedYAxisMax = null;
        } else {
          var min = double.Parse(minYTextBox.Text, CultureInfo.CurrentUICulture);
          var max = double.Parse(maxYTextBox.Text, CultureInfo.CurrentUICulture);
          chart.FixedYAxisMin = min;
          chart.FixedYAxisMax = max;
        }

        chart.DrawingSteps = (int)StepsNumericUpDown.Value;

        await chart.RecalculateAsync(resetYAxis: false);

        Close();
      }
      catch (FormatException) {
        MessageBox.Show(this, "Illegal number format", "Wrong format", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally {
        Enabled = true;
        chart.ResumeRepaint(true);
      }
    }

    private void automaticCheckBox_CheckedChanged(object sender, EventArgs e) {
      minXTextBox.Enabled = !xAutomaticCheckBox.Checked;
      maxXTextBox.Enabled = !xAutomaticCheckBox.Checked;

      minYTextBox.Enabled = !yAutomaticCheckBox.Checked;
      maxYTextBox.Enabled = !yAutomaticCheckBox.Checked;
    }

    private void numberTextBox_Validating(object sender, CancelEventArgs e) {
      var textBox = sender as TextBox;
      if (textBox != null) {
        double number;
        if (!double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out number)) {
          e.Cancel = true;
          applyButton.Enabled = false;
          errorProvider.SetIconAlignment(textBox, ErrorIconAlignment.MiddleLeft);
          errorProvider.SetIconPadding(textBox, 2);
          errorProvider.SetError(textBox, "Illegal number format");
          textBox.SelectAll();
        }
      }
    }

    private void numberTextBox_Validated(object sender, EventArgs e) {
      var textBox = sender as TextBox;
      if (textBox != null) {
        errorProvider.SetError(textBox, string.Empty);
        applyButton.Enabled = true;
      }
    }
  }
}
