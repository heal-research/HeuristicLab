using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class InstallationManagerForm : Form {
    private InstallationManager installationManager;

    public InstallationManagerForm() {
      InitializeComponent();
      this.installationManager = new InstallationManager(Path.GetDirectoryName(Application.ExecutablePath));

      UpdatePluginsList();
    }

    private void UpdatePluginsList() {
      foreach (var plugin in installationManager.Plugins) {
        pluginsListView.Items.Add(CreatePluginItem(plugin));
      }
    }

    private static ListViewItem CreatePluginItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem();
      item.Tag = plugin;
      item.Text = plugin.Name + "-" + plugin.Version.ToString();
      return item;
    }

    private void pluginsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (pluginsListView.SelectedItems.Count > 0) {
        ListViewItem selecteditem = pluginsListView.SelectedItems[0];
        IPluginDescription desc = (IPluginDescription)selecteditem.Tag;
        UpdateDetailsBox((PluginDescription)desc);
      }
    }

    private void UpdateDetailsBox(PluginDescription desc) {
      detailsTextBox.Text = installationManager.GetInformation(desc.Name);
    }
  }
}
