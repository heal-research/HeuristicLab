#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.Instances.TSPLIB.Views {
  public sealed partial class TSPLIBImportDialog : Form {
    private string tspFileName;
    public string TSPFileName {
      get { return tspFileName; }
    }
    private double? quality;
    public double? Quality {
      get { return quality; }
    }
    private string tourFileName;
    public string TourFileName {
      get { return tourFileName; }
    }

    public TSPLIBImportDialog() {
      InitializeComponent();
      tspFileName = string.Empty;
      tourFileName = string.Empty;
      quality = null;
      errorProvider.SetIconAlignment(qualityTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(qualityTextBox, 2);
    }

    private void openTSPFileButton_Click(object sender, EventArgs e) {
      if (openTSPFileDialog.ShowDialog(this) == DialogResult.OK) {
        tspFileTextBox.Text = openTSPFileDialog.FileName;
        tspFileTextBox.Enabled = true;
        tspFileName = openTSPFileDialog.FileName;
        okButton.Enabled = true;
      }
    }
    private void openTourFileButton_Click(object sender, EventArgs e) {
      if (openTourFileDialog.ShowDialog(this) == DialogResult.OK) {
        tourFileTextBox.Text = openTourFileDialog.FileName;
        tourFileTextBox.Enabled = true;
        tourFileName = openTourFileDialog.FileName;
      }
    }
    private void clearTourFileButton_Click(object sender, EventArgs e) {
      tourFileTextBox.Text = string.Empty;
      tourFileTextBox.Enabled = false;
      tourFileName = string.Empty;
    }

    private void qualityTextBox_Validating(object sender, CancelEventArgs e) {
      double val;
      if ((!string.IsNullOrEmpty(qualityTextBox.Text)) && (!double.TryParse(qualityTextBox.Text, out val))) {
        e.Cancel = true;
        StringBuilder sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetDoubleFormatPattern());
        sb.Append("\")");
        errorProvider.SetError(qualityTextBox, sb.ToString());
        qualityTextBox.SelectAll();
        CancelButton = null;
      }
    }
    private void qualityTextBox_Validated(object sender, EventArgs e) {
      if (string.IsNullOrEmpty(qualityTextBox.Text)) quality = null;
      else quality = double.Parse(qualityTextBox.Text);
      errorProvider.SetError(qualityTextBox, string.Empty);
      CancelButton = cancelButton;
    }
    private void qualityTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape) {
        qualityTextBox.Text = quality == null ? string.Empty : quality.ToString();
        qualityLabel.Focus();  // set focus on label to validate data
      }
    }
  }
}
