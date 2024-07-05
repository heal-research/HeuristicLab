#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class LicenseConfirmationDialog : Form {
    public LicenseConfirmationDialog()
      : base() {
      InitializeComponent();
    }

    public LicenseConfirmationDialog(IPluginDescription plugin)
      : base() {
      InitializeComponent();
      richTextBox.Text = plugin.LicenseText;
      this.Text = plugin.ToString();
      this.DialogResult = DialogResult.Cancel;
    }

    private void acceptButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      this.Close();
    }

    private void rejectButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e) {
      acceptButton.Enabled = acceptRadioButton.Checked;
    }
  }
}
