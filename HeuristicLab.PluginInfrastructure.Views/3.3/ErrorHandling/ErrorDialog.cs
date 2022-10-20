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

namespace HeuristicLab.PluginInfrastructure {
  public partial class ErrorDialog : Form {
    public ErrorDialog() {
      InitializeComponent();
      Initialize(null, null);
    }
    public ErrorDialog(Exception exception)
      : this() {
      Initialize(null, exception);
    }
    public ErrorDialog(string message, Exception exception)
      : this() {
      Initialize(message, exception);
    }

    private void Initialize(string message, Exception exception) {
      Text = "Error";
      if (exception != null) Text += "  -  " + exception.GetType().Name;

      if (!string.IsNullOrEmpty(message))
        messageTextBox.Text = message;
      else if (exception != null)
        messageTextBox.Text = exception.Message;
      else
        messageTextBox.Text = "Sorry, but something went wrong.";
      messageTextBox.Enabled = !string.IsNullOrEmpty(messageTextBox.Text);

      detailsTextBox.Text = ErrorHandling.BuildErrorMessage(exception);
      detailsGroupBox.Enabled = !string.IsNullOrEmpty(detailsTextBox.Text);
    }

    private void supportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      try {
        System.Diagnostics.Process.Start("mailto:" + supportLinkLabel.Text);
        supportLinkLabel.LinkVisited = true;
      }
      catch (Exception) { }
    }
  }
}
