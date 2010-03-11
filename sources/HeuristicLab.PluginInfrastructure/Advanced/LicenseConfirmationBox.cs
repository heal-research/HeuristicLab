using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class LicenseConfirmationBox : Form {
    public LicenseConfirmationBox() {
      InitializeComponent();
    }

    public LicenseConfirmationBox(IPluginDescription plugin) {
      InitializeComponent();
      richTextBox.Text = plugin.LicenseText;
      licenseLabel.Text = plugin.ToString() + " " + licenseLabel.Text;
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
  }
}
