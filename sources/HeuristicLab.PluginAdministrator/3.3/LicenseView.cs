using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.DeploymentService.AdminClient {
  public partial class LicenseView : HeuristicLab.MainForm.WindowsForms.View {

    public LicenseView() {
      InitializeComponent();
      Caption = "License";
    }

    public LicenseView(IPluginDescription plugin) {
      InitializeComponent();
      Caption = "License of: " + plugin;
      textBox.Text = plugin.LicenseText;
    }
  }
}
