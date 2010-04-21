#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion
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
  internal partial class LocalPluginManagerView : UserControl {

    public event ItemCheckedEventHandler ItemChecked;

    private BackgroundWorker refreshPluginListBackgroundWorker = new BackgroundWorker();

    private ListViewGroup enabledPluginsGroup;
    private ListViewGroup disabledPluginsGroup;

    public LocalPluginManagerView() {
      InitializeComponent();

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
      localPluginsListView.SuppressItemCheckedEvents = true;
      foreach (var plugin in plugins) {
        var item = CreateListViewItem(plugin);
        if (plugin.PluginState == PluginState.Enabled) {
          item.Group = enabledPluginsGroup;
        } else if (plugin.PluginState == PluginState.Disabled) {
          item.Group = disabledPluginsGroup;
        }
        localPluginsListView.Items.Add(item);
      }
      localPluginsListView.SuppressItemCheckedEvents = false;
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
        List<ListViewItem> modifiedItems = new List<ListViewItem>();
        foreach (ListViewItem item in localPluginsListView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          modifiedItems.Add(item);
          // also uncheck all dependent plugins
          foreach (ListViewItem dependentItem in localPluginsListView.Items) {
            var dependent = (IPluginDescription)dependentItem.Tag;
            if (!dependentItem.Checked && (from dep in dependent.Dependencies
                                           where dep.Name == plugin.Name
                                           where dep.Version == plugin.Version
                                           select dep).Any()) {
              modifiedItems.Add(dependentItem);
            }
          }
        }
        localPluginsListView.CheckItems(modifiedItems);
      } else {
        List<ListViewItem> modifiedItems = new List<ListViewItem>();
        foreach (ListViewItem item in localPluginsListView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          modifiedItems.Add(item);
        }
        localPluginsListView.UncheckItems(modifiedItems);
      }
      OnItemChecked(e);
    }

    private void OnItemChecked(ItemCheckedEventArgs e) {
      if (ItemChecked != null) ItemChecked(this, e);
    }

    private void localPluginsListView_ItemActivate(object sender, EventArgs e) {
      if (localPluginsListView.SelectedItems.Count > 0) {
        var plugin = (PluginDescription)localPluginsListView.SelectedItems[0].Tag;
        PluginView pluginView = new PluginView(plugin);
        pluginView.Show();
      }
    }
  }
}
