using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using PluginDeploymentService = HeuristicLab.PluginInfrastructure.Advanced.DeploymentService;
using HeuristicLab.PluginInfrastructure.Manager;
using System.ServiceModel;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace HeuristicLab.DeploymentService.AdminClient {
  public partial class PluginComparisonView : HeuristicLab.MainForm.WindowsForms.View {

    public PluginComparisonView(IPluginDescription localPlugin, IPluginDescription serverPlugin) {
      InitializeComponent();
      Caption = "Compare plugins";

      var localPluginView = new PluginView(localPlugin);
      localPluginView.Dock = DockStyle.Fill;
      tableLayoutPanel.Controls.Add(localPluginView, 0, 0);
      if (serverPlugin != null) {
        var serverPluginView = new PluginView(serverPlugin);
        serverPluginView.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(serverPluginView, 1, 0);
      } else {
        Label emptyLabel = new Label();
        emptyLabel.Text = "No matching server plugin";
        emptyLabel.TextAlign = ContentAlignment.MiddleCenter;
        emptyLabel.Dock = DockStyle.Fill;
        tableLayoutPanel.Controls.Add(emptyLabel, 1, 0);
      }
    }
  }
}
