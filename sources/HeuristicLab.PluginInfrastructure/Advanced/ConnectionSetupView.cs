using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class ConnectionSetupView : InstallationManagerControl {
    private Form form;
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
      form.Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      form.Close();
    }

    internal override void ShowInForm() {
      if (this.form == null) {
        form = new Form();
        form.Name = this.Name;
        form.Size = this.Size;
        this.Dock = DockStyle.Fill;
        form.Controls.Add(this);
      } 
      form.Show();
    }
  }
}
