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
  internal partial class ConnectionSetupView : Form {
    public ConnectionSetupView() {
      InitializeComponent();

      urlTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation;
      userTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName;
      passwordTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword;
      savePasswordCheckbox.Checked = !string.IsNullOrEmpty(passwordTextBox.Text);
    }

    private void applyButton_Click(object sender, EventArgs e) {
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation = urlTextBox.Text;
      if (!savePasswordCheckbox.Checked) {
        // make sure we don't save username or password
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName = string.Empty;
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword = string.Empty;
        // save
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.Save();
        // set user name and password for current process
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName = userTextBox.Text;
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword = passwordTextBox.Text;
      } else {
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName = userTextBox.Text;
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword = passwordTextBox.Text;
        HeuristicLab.PluginInfrastructure.Properties.Settings.Default.Save();
      }
      Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
