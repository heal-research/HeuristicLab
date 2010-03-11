using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class LicenseView : InstallationManagerControl {

    public LicenseView() {
      InitializeComponent();
      Name = "License";
    }

    public LicenseView(IPluginDescription plugin) {
      InitializeComponent();
      Name = "License of: " + plugin;
      richTextBox.Text = plugin.LicenseText;
    }
  }
}
