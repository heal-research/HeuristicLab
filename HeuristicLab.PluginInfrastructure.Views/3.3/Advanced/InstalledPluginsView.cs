#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class InstalledPluginsView : InstallationManagerControl {
    private ListViewGroup enabledPluginsGroup;
    private ListViewGroup disabledPluginsGroup;

    private IApplicationManager pluginManager;
    public IApplicationManager PluginManager {
      get { return pluginManager; }
      set {
        pluginManager = value;
        UpdateControl();
      }
    }

    public InstalledPluginsView()
      : base() {
      InitializeComponent();
      enabledPluginsGroup = localPluginsListView.Groups["activePluginsGroup"];
      disabledPluginsGroup = localPluginsListView.Groups["disabledPluginsGroup"];
      pluginImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Plugin);
     
      UpdateControl();
    }

    private void UpdateControl() {
      ClearPluginList();
      if (pluginManager != null) {
        localPluginsListView.SuppressItemCheckedEvents = true;
        foreach (var plugin in pluginManager.Plugins.Cast<PluginDescription>()) {
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
      Util.ResizeColumns(localPluginsListView.Columns.OfType<ColumnHeader>());
    }

    private void ClearPluginList() {
      List<ListViewItem> itemsToRemove = new List<ListViewItem>(localPluginsListView.Items.OfType<ListViewItem>());
      itemsToRemove.ForEach(item => localPluginsListView.Items.Remove(item));
    }

    private static ListViewItem CreateListViewItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), plugin.Description });
      item.Tag = plugin;
      item.ImageIndex = 0;
      return item;
    }

    private void pluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      // checked items are marked for removal
      if (e.Item.Checked) {
        List<ListViewItem> modifiedItems = new List<ListViewItem>();
        foreach (ListViewItem item in localPluginsListView.SelectedItems) {
          modifiedItems.Add(item);
          int oldItemsCount = 0;
          while (oldItemsCount < modifiedItems.Count) {
            oldItemsCount = modifiedItems.Count;
            var oldModifiedItems = new List<ListViewItem>(modifiedItems);
            foreach (var modifiedItem in oldModifiedItems) {
              var plugin = (IPluginDescription)modifiedItem.Tag;
              // also check all dependent plugins
              foreach (ListViewItem dependentItem in localPluginsListView.Items) {
                var dependent = (IPluginDescription)dependentItem.Tag;
                if (!modifiedItems.Contains(dependentItem) &&
                  !dependentItem.Checked && (from dep in dependent.Dependencies
                                             where dep.Name == plugin.Name
                                             where dep.Version == plugin.Version
                                             select dep).Any()) {
                  modifiedItems.Add(dependentItem);
                }
              }
            }
          }
        }
        localPluginsListView.CheckItems(modifiedItems);
      } else {
        List<ListViewItem> modifiedItems = new List<ListViewItem>();
        foreach (ListViewItem item in localPluginsListView.SelectedItems) {
          modifiedItems.Add(item);
        }
        localPluginsListView.UncheckItems(modifiedItems);
      }      
    }

    private void localPluginsListView_ItemActivate(object sender, EventArgs e) {
      if (localPluginsListView.SelectedItems.Count > 0) {
        var plugin = (PluginDescription)localPluginsListView.SelectedItems[0].Tag;
        PluginView pluginView = new PluginView(plugin);
        pluginView.Show(this);
      }
    }
  }
}
