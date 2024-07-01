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
    private const string CheckingPluginsMessage = "Checking for updated plugins...";
    private const string NoUpdatesAvailableMessage = "No updates available.";
    private BackgroundWorker removePluginsBackgroundWorker;
    private BackgroundWorker updatePluginsBackgroundWorker;

    private ListViewGroup enabledPluginsGroup;
    private ListViewGroup disabledPluginsGroup;

    private PluginManager pluginManager;
    public PluginManager PluginManager {
      get { return pluginManager; }
      set {
        pluginManager = value;
        UpdateControl();
      }
    }

    private InstallationManager installationManager;
    public InstallationManager InstallationManager {
      get { return installationManager; }
      set { installationManager = value; }
    }

    public InstalledPluginsView()
      : base() {
      InitializeComponent();
      enabledPluginsGroup = localPluginsListView.Groups["activePluginsGroup"];
      disabledPluginsGroup = localPluginsListView.Groups["disabledPluginsGroup"];
      pluginImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Plugin);
      removePluginsBackgroundWorker = new BackgroundWorker();
      removePluginsBackgroundWorker.DoWork += new DoWorkEventHandler(removePluginsBackgroundWorker_DoWork);
      removePluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(removePluginsBackgroundWorker_RunWorkerCompleted);
      updatePluginsBackgroundWorker = new BackgroundWorker();
      updatePluginsBackgroundWorker.DoWork += new DoWorkEventHandler(updatePluginsBackgroundWorker_DoWork);
      updatePluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updatePluginsBackgroundWorker_RunWorkerCompleted);

      UpdateControl();
    }



    #region event handlers for plugin removal background worker
    void removePluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("File Deletion Error", "There was problem while deleting files." + Environment.NewLine + e.Error.Message);
      }
      UpdateControl();
      StatusView.HideProgressIndicator();
      StatusView.UnlockUI();
    }

    void removePluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      IEnumerable<IPluginDescription> pluginsToRemove = (IEnumerable<IPluginDescription>)e.Argument;
      if (pluginsToRemove.Count() > 0) {
        installationManager.Remove(pluginsToRemove);
        pluginManager.DiscoverAndCheckPlugins();
      }
    }
    #endregion

    #region event handlers for update plugins backgroundworker
    void updatePluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
           "Please check your connection settings and user credentials.");
      }
      StatusView.RemoveMessage(CheckingPluginsMessage);
      StatusView.HideProgressIndicator();
      UpdateControl();
      StatusView.UnlockUI();
    }

    void updatePluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      IEnumerable<IPluginDescription> selectedPlugins = (IEnumerable<IPluginDescription>)e.Argument;
      var remotePlugins = installationManager.GetRemotePluginList();
      // if there is a local plugin with same name and same major and minor version then it's an update
      var pluginsToUpdate = from remotePlugin in remotePlugins
                            let matchingLocalPlugins = from installedPlugin in selectedPlugins
                                                       where installedPlugin.Name == remotePlugin.Name
                                                       where installedPlugin.Version.Major == remotePlugin.Version.Major
                                                       where installedPlugin.Version.Minor == remotePlugin.Version.Minor
                                                       where IsNewerThan(remotePlugin, installedPlugin)
                                                       select installedPlugin
                            where matchingLocalPlugins.Count() > 0
                            select remotePlugin;
      if (pluginsToUpdate.Count() > 0) {
        bool cancelled;
        installationManager.Update(pluginsToUpdate, out cancelled);
        if (!cancelled) pluginManager.DiscoverAndCheckPlugins();
      }
    }

    // compares for two plugins with same major and minor version if plugin1 is newer than plugin2
    private static bool IsNewerThan(IPluginDescription plugin1, IPluginDescription plugin2) {
      // newer: build version is higher, or if build version is the same revision is higher
      return plugin1.Version.Build > plugin2.Version.Build ||
        (plugin1.Version.Build == plugin2.Version.Build && plugin1.Version.Revision > plugin2.Version.Revision);
    }
    #endregion

    private void UpdateControl() {
      ClearPluginList();
      if (pluginManager != null) {
        localPluginsListView.SuppressItemCheckedEvents = true;
        foreach (var plugin in pluginManager.Plugins) {
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
      removeButton.Enabled = localPluginsListView.CheckedItems.Count > 0;
      updateSelectedButton.Enabled = localPluginsListView.CheckedItems.Count > 0;
      Util.ResizeColumns(localPluginsListView.Columns.OfType<ColumnHeader>());
    }

    private void ClearPluginList() {
      List<ListViewItem> itemsToRemove = new List<ListViewItem>(localPluginsListView.Items.OfType<ListViewItem>());
      itemsToRemove.ForEach(item => localPluginsListView.Items.Remove(item));
    }

    private static ListViewItem CreateListViewItem(PluginDescription plugin) {
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
      OnItemsCheckedChanged(EventArgs.Empty);
    }

    private void localPluginsListView_ItemActivate(object sender, EventArgs e) {
      if (localPluginsListView.SelectedItems.Count > 0) {
        var plugin = (PluginDescription)localPluginsListView.SelectedItems[0].Tag;
        PluginView pluginView = new PluginView(plugin);
        pluginView.Show(this);
      }
    }

    private void OnItemsCheckedChanged(EventArgs eventArgs) {
      removeButton.Enabled = localPluginsListView.CheckedItems.Count > 0;
      updateSelectedButton.Enabled = localPluginsListView.CheckedItems.Count > 0;
    }

    private void updateSelectedButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.RemoveMessage(NoUpdatesAvailableMessage);
      StatusView.ShowMessage(CheckingPluginsMessage);
      var checkedPlugins = localPluginsListView.CheckedItems.OfType<ListViewItem>()
        .Select(item => item.Tag)
        .OfType<IPluginDescription>()
        .ToList();
      updatePluginsBackgroundWorker.RunWorkerAsync(checkedPlugins);
    }

    private void removeButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      var checkedPlugins = localPluginsListView.CheckedItems.OfType<ListViewItem>()
        .Select(item => item.Tag)
        .OfType<IPluginDescription>()
        .ToList();
      removePluginsBackgroundWorker.RunWorkerAsync(checkedPlugins);
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      // refresh = update empty list of plugins (plugins are reloaded)
      updatePluginsBackgroundWorker.RunWorkerAsync(new IPluginDescription[0]);
    }
  }
}
