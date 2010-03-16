using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.DeploymentService.AdminClient {
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
