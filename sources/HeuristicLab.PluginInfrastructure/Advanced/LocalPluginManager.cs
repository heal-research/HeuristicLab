using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  public partial class LocalPluginManager : UserControl {

    public event ItemCheckedEventHandler ItemChecked;

    private BackgroundWorker refreshPluginListBackgroundWorker = new BackgroundWorker();

    private ListViewGroup enabledPluginsGroup;
    private ListViewGroup disabledPluginsGroup;

    public LocalPluginManager() {
      InitializeComponent();

      imageListForLocalItems.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Assembly);
      imageListForLocalItems.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Resources.Remove);

      enabledPluginsGroup = localPluginsListView.Groups["activePluginsGroup"];
      disabledPluginsGroup = localPluginsListView.Groups["disabledPluginsGroup"];
    }

    private IEnumerable<PluginDescription> plugins;
    public IEnumerable<PluginDescription> Plugins {
      get { return plugins; }
      set {
        if (value != plugins) {
          this.plugins = value;
          UpdateControl();
        }
      }
    }

    public IEnumerable<IPluginDescription> CheckedPlugins {
      get {
        return (from item in localPluginsListView.Items.OfType<ListViewItem>()
                where item.Checked
                let plugin = item.Tag as IPluginDescription
                where plugin != null
                select plugin).ToList();
      }
    }

    private void UpdateControl() {
      ClearPluginList();
      foreach (var plugin in plugins) {
        var item = CreateListViewItem(plugin);
        if (plugin.PluginState == PluginState.Enabled) {
          item.Group = enabledPluginsGroup;
        } else if (plugin.PluginState == PluginState.Disabled) {
          item.Group = disabledPluginsGroup;
        }
        localPluginsListView.Items.Add(item);
      }
    }

    private void ClearPluginList() {
      List<ListViewItem> itemsToRemove = new List<ListViewItem>(from item in localPluginsListView.Items.OfType<ListViewItem>()
                                                                select item);
      itemsToRemove.ForEach(item => localPluginsListView.Items.Remove(item));
    }

    private ListViewItem CreateListViewItem(PluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), plugin.Description });
      item.Tag = plugin;
      return item;
    }

    private void pluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      // checked items are marked for removal
      if (e.Item.Checked) {
        var plugin = (IPluginDescription)e.Item.Tag;
        foreach (ListViewItem item in localPluginsListView.Items) {
          var dep = (IPluginDescription)item.Tag;
          if (!item.Checked && dep.Dependencies.Contains(plugin)) {
            item.Checked = true;
          }
        }
      }
      OnItemChecked(e);
    }

    private void OnItemChecked(ItemCheckedEventArgs e) {
      if (ItemChecked != null) ItemChecked(this, e);
    }
  }
}
