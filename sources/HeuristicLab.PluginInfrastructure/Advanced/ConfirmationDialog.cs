using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class ConfirmationDialog : Form {
    public ConfirmationDialog() {
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
