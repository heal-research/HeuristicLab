using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class InstallationManagerControl : UserControl {
    public InstallationManagerControl() {
      InitializeComponent();
    }

    internal virtual void ShowInForm() {
      Form form = new Form();
      form.ClientSize = this.Size;
      form.Text = this.Name;
      this.Dock = DockStyle.Fill;
      form.Controls.Add(this);
      form.Show();
    }
  }
}
