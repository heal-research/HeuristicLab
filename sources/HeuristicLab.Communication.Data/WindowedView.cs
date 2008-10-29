using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Communication.Data {
  public partial class WindowedView : Form {
    public WindowedView() {
      InitializeComponent();
    }

    public WindowedView(UserControl control)
      : this() {
      this.SuspendLayout();
      this.Size = new Size(control.Width + 8, control.Height + 70);
      control.Dock = DockStyle.Fill;
      viewPanel.Controls.Add(control);
      this.ResumeLayout();
    }

    private void saveButton_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
