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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class InstallationManagerForm : Form, IStatusView {   
    private string pluginDir;

    public InstallationManagerForm(IApplicationManager appManager)
      : base() {
      InitializeComponent();
      Text = "HeuristicLab Plugin Manager " + AssemblyHelpers.GetFileVersion(GetType().Assembly);

      pluginDir = Application.StartupPath;
      
      localPluginsView.StatusView = this;
      localPluginsView.PluginManager = appManager;
    }    
   
    #region button events
    private void connectionSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
      using (var conSetupView = new ConnectionSetupView()) {
        conSetupView.ShowDialog(this);
      }
    }
    private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
      toolStripStatusLabel.Text = string.Empty;
    }
    #endregion
    
    private void SetStatusStrip(string msg) {
      if (InvokeRequired) Invoke((Action<string>)SetStatusStrip, msg);
      else {
        toolStripStatusLabel.Text = msg;
        logTextBox.Text += DateTime.Now + ": " + msg + Environment.NewLine;
      }
    }

    public void ShowProgressIndicator(double percentProgress) {
      if (percentProgress < 0.0 || percentProgress > 1.0) throw new ArgumentException("percentProgress");
      toolStripProgressBar.Visible = true;
      toolStripProgressBar.Style = ProgressBarStyle.Continuous;
      int range = toolStripProgressBar.Maximum - toolStripProgressBar.Minimum;
      toolStripProgressBar.Value = (int)(percentProgress * range + toolStripProgressBar.Minimum);
    }

    public void ShowProgressIndicator() {
      toolStripProgressBar.Visible = true;
      toolStripProgressBar.Style = ProgressBarStyle.Marquee;
    }

    public void HideProgressIndicator() {
      toolStripProgressBar.Visible = false;
    }

    public void ShowMessage(string message) {
      if (string.IsNullOrEmpty(toolStripStatusLabel.Text))
        toolStripStatusLabel.Text = message;
      else
        toolStripStatusLabel.Text += "; " + message;
    }

    public void RemoveMessage(string message) {
      if (toolStripStatusLabel.Text.IndexOf("; " + message) > 0) {
        toolStripStatusLabel.Text = toolStripStatusLabel.Text.Replace("; " + message, "");
      }
      toolStripStatusLabel.Text = toolStripStatusLabel.Text.Replace(message, "");
      toolStripStatusLabel.Text = toolStripStatusLabel.Text.TrimStart(' ', ';');
    }
    public void LockUI() {
      Cursor = Cursors.AppStarting;
      tabControl.Enabled = false;
    }
    public void UnlockUI() {
      tabControl.Enabled = true;
      Cursor = Cursors.Default;
    }
    public void ShowError(string shortMessage, string description) {
      logTextBox.Text += DateTime.Now + ": " + shortMessage + Environment.NewLine + description + Environment.NewLine;
      MessageBox.Show(description, shortMessage);
    }
    
  }
}
