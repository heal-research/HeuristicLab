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
  internal partial class ConfirmationDialog : Form {
    public ConfirmationDialog()
      : base() {
      InitializeComponent();
      icon.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
      DialogResult = DialogResult.Cancel;
    }

    public ConfirmationDialog(string caption, string message, string text)
      : this() {
      this.Text = caption;
      messageLabel.Text = message;
      informationTextBox.Text = text;
    }

    private void okButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      this.Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
