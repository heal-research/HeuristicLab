#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginAdministrator {
  internal partial class ConnectionSetupView : HeuristicLab.MainForm.WindowsForms.View {
    public ConnectionSetupView() {
      InitializeComponent();
      Caption = "Edit Connection Settings";
      updateAddressTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation;
      adminAddressTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationAdministrationAddress;
      userTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName;
      passwordTextBox.Text = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword;
    }

    private void applyButton_Click(object sender, EventArgs e) {
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation = updateAddressTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationAdministrationAddress = adminAddressTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName = userTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword = passwordTextBox.Text;
    }

    private void saveCredentialsButton_Click(object sender, EventArgs e) {
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation = updateAddressTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationAdministrationAddress = adminAddressTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName = userTextBox.Text;
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword = passwordTextBox.Text;
      // save
      HeuristicLab.PluginInfrastructure.Properties.Settings.Default.Save();
    }
  }
}
