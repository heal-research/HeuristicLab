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
  internal partial class RemotePluginInstallerView : InstallationManagerControl {
    private class RefreshBackgroundWorkerResult {
      public IEnumerable<IPluginDescription> RemotePlugins { get; set; }
      public IEnumerable<DeploymentService.ProductDescription> RemoteProducts { get; set; }
    }
    private class UpdateOrInstallPluginsBackgroundWorkerArgument {
      public IEnumerable<IPluginDescription> PluginsToUpdate { get; set; }
      public IEnumerable<IPluginDescription> PluginsToInstall { get; set; }
    }
    private const string PluginDiscoveryMessage = "Looking for new plugins...";
    private BackgroundWorker refreshServerPluginsBackgroundWorker;
    private BackgroundWorker updateOrInstallPluginsBackgroundWorker;

    private ListViewGroup newPluginsGroup;
    private ListViewGroup productsGroup;
    private ListViewGroup allPluginsGroup;

    private bool showAllPlugins;
    public bool ShowAllPlugins {
      get { return showAllPlugins; }
      set {
        if (value != showAllPlugins) {
          showAllPlugins = value;
          UpdateControl();
        }
      }
    }

    private IEnumerable<DeploymentService.ProductDescription> products;
    public IEnumerable<DeploymentService.ProductDescription> Products {
      get { return products ?? Enumerable.Empty<DeploymentService.ProductDescription>(); }
      set {
        if (value != this.products) {
          this.products = value;
          UpdateControl();
        }
      }
    }

    private IEnumerable<IPluginDescription> plugins;
    public IEnumerable<IPluginDescription> AllPlugins {
      get { return plugins ?? Enumerable.Empty<IPluginDescription>(); }
      set {
        if (value != this.plugins) {
          this.plugins = value;
          UpdateControl();
        }
      }
    }

    private IEnumerable<IPluginDescription> newPlugins;
    public IEnumerable<IPluginDescription> NewPlugins {
      get { return newPlugins ?? Enumerable.Empty<IPluginDescription>(); }
      set {
        if (value != this.newPlugins) {
          this.newPlugins = value;
          UpdateControl();
        }
      }
    }

    public IEnumerable<IPluginDescription> CheckedPlugins {
      get {
        return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
                where item.Checked
                let plugin = item.Tag as IPluginDescription
                where plugin != null
                select plugin).ToList();
      }
    }

    private InstallationManager installationManager;
    public InstallationManager InstallationManager {
      get { return installationManager; }
      set { installationManager = value; }
    }
    private PluginManager pluginManager;
    public PluginManager PluginManager {
      get { return pluginManager; }
      set { pluginManager = value; }
    }
    public RemotePluginInstallerView() {
      InitializeComponent();

      refreshServerPluginsBackgroundWorker = new BackgroundWorker();
      refreshServerPluginsBackgroundWorker.DoWork += new DoWorkEventHandler(refreshServerPluginsBackgroundWorker_DoWork);
      refreshServerPluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshServerPluginsBackgroundWorker_RunWorkerCompleted);

      updateOrInstallPluginsBackgroundWorker = new BackgroundWorker();
      updateOrInstallPluginsBackgroundWorker.DoWork += new DoWorkEventHandler(updateOrInstallPluginsBackgroundWorker_DoWork);
      updateOrInstallPluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateOrInstallPluginsBackgroundWorker_RunWorkerCompleted);

      newPluginsGroup = remotePluginsListView.Groups["newPluginsGroup"];
      productsGroup = remotePluginsListView.Groups["productsGroup"];
      allPluginsGroup = remotePluginsListView.Groups["allPluginsGroup"];
    }


    #region event handlers for refresh server plugins background worker
    void refreshServerPluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
          "Please check your connection settings and user credentials.");
      } else {
        RefreshBackgroundWorkerResult refreshResult = (RefreshBackgroundWorkerResult)e.Result;
        UpdateRemotePluginList(refreshResult.RemoteProducts, refreshResult.RemotePlugins);
      }
      StatusView.UnlockUI();
      StatusView.RemoveMessage(PluginDiscoveryMessage);
      StatusView.HideProgressIndicator();
    }

    void refreshServerPluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      RefreshBackgroundWorkerResult result = new RefreshBackgroundWorkerResult();
      result.RemotePlugins = installationManager.GetRemotePluginList();
      result.RemoteProducts = installationManager.GetRemoteProductList();
      e.Result = result;
    }
    #endregion
    #region event handlers for plugin update background worker
    void updateOrInstallPluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
          "Please check your connection settings and user credentials.");
      } else {
        UpdateControl();
      }
      StatusView.UnlockUI();
      StatusView.HideProgressIndicator();
    }

    void updateOrInstallPluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      UpdateOrInstallPluginsBackgroundWorkerArgument info = (UpdateOrInstallPluginsBackgroundWorkerArgument)e.Argument;
      if (info.PluginsToInstall.Count() > 0)
        installationManager.Install(info.PluginsToInstall);
      if (info.PluginsToUpdate.Count() > 0)
        installationManager.Update(info.PluginsToUpdate);

      if (info.PluginsToInstall.Count() > 0 || info.PluginsToUpdate.Count() > 0)
        pluginManager.DiscoverAndCheckPlugins();
    }
    #endregion


    #region button events
    private void refreshRemoteButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.ShowMessage(PluginDiscoveryMessage);
      refreshServerPluginsBackgroundWorker.RunWorkerAsync();
    }
    private void installButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      var updateOrInstallInfo = new UpdateOrInstallPluginsBackgroundWorkerArgument();
      // if there is a local plugin with same name and same major and minor version then it's an update
      var pluginsToUpdate = from remotePlugin in CheckedPlugins
                            let matchingLocalPlugins = from localPlugin in pluginManager.Plugins
                                                       where localPlugin.Name == remotePlugin.Name
                                                       where localPlugin.Version.Major == remotePlugin.Version.Major
                                                       where localPlugin.Version.Minor == remotePlugin.Version.Minor
                                                       where IsNewerThan(remotePlugin, localPlugin)
                                                       select localPlugin
                            where matchingLocalPlugins.Count() > 0
                            select remotePlugin;

      // otherwise install a new plugin
      var pluginsToInstall = CheckedPlugins.Except(pluginsToUpdate);

      updateOrInstallInfo.PluginsToInstall = pluginsToInstall;
      updateOrInstallInfo.PluginsToUpdate = pluginsToUpdate;
      updateOrInstallPluginsBackgroundWorker.RunWorkerAsync(updateOrInstallInfo);
    }
    #endregion

    private void UpdateControl() {
      ClearListView();
      remotePluginsListView.SuppressItemCheckedEvents = true;
      foreach (var newPlugin in NewPlugins) {
        var item = CreateListViewItem(newPlugin);
        item.Group = newPluginsGroup;
        remotePluginsListView.Items.Add(item);
      }

      foreach (var product in Products) {
        var item = CreateListViewItem(product);
        item.Group = productsGroup;
        remotePluginsListView.Items.Add(item);
      }

      if (showAllPlugins) {
        foreach (var plugin in AllPlugins) {
          var item = CreateListViewItem(plugin);
          item.Group = allPluginsGroup;
          remotePluginsListView.Items.Add(item);
        }
      }
      foreach (ColumnHeader column in remotePluginsListView.Columns)
        if (remotePluginsListView.Items.Count > 0)
          column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        else column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);

      remotePluginsListView.SuppressItemCheckedEvents = false;
    }

    private void ClearListView() {
      List<ListViewItem> itemsToDelete = new List<ListViewItem>(remotePluginsListView.Items.OfType<ListViewItem>());
      itemsToDelete.ForEach(item => remotePluginsListView.Items.Remove(item));
    }

    private ListViewItem CreateListViewItem(DeploymentService.ProductDescription product) {
      ListViewItem item = new ListViewItem(new string[] { product.Name, product.Version.ToString(), string.Empty });
      item.Tag = product;
      return item;
    }

    private ListViewItem CreateListViewItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), plugin.Description });
      item.Tag = plugin;
      return item;
    }

    #region item checked event handler
    private void remotePluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      foreach (ListViewItem item in remotePluginsListView.SelectedItems) {
        // dispatch by check state and type of item (product/plugin)
        IPluginDescription plugin = item.Tag as IPluginDescription;
        if (plugin != null)
          if (e.Item.Checked)
            HandlePluginChecked(plugin);
          else
            HandlePluginUnchecked(plugin);
        else {
          DeploymentService.ProductDescription product = item.Tag as DeploymentService.ProductDescription;
          if (product != null)
            if (e.Item.Checked)
              HandleProductChecked(product);
            else
              HandleProductUnchecked(product);
        }
      }
      installButton.Enabled = remotePluginsListView.CheckedItems.Count > 0;
    }

    private void HandleProductUnchecked(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product) {
      // also uncheck the plugins of the product
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForProduct(product));
      foreach (var plugin in product.Plugins) {
        // there can be multiple entries for a single plugin in different groups
        foreach (var item in FindItemsForPlugin(plugin)) {
          if (item != null && item.Checked)
            modifiedItems.Add(item);
        }
      }
      remotePluginsListView.UncheckItems(modifiedItems);
    }

    private void HandleProductChecked(HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.ProductDescription product) {
      // also check all plugins of the product
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.Add(FindItemForProduct(product));
      foreach (var plugin in product.Plugins) {
        // there can be multiple entries for a single plugin in different groups
        foreach (var item in FindItemsForPlugin(plugin)) {
          if (item != null && !item.Checked) {
            if (!modifiedItems.Contains(item))
              modifiedItems.Add(item);
          }
        }
      }
      remotePluginsListView.CheckItems(modifiedItems);
    }

    private void HandlePluginUnchecked(IPluginDescription plugin) {
      // also uncheck all dependent plugins
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.AddRange(FindItemsForPlugin(plugin));
      var dependentPlugins = from otherPlugin in plugins
                             where otherPlugin.Dependencies.Any(dep => dep.Name == plugin.Name && dep.Version == plugin.Version)
                             select otherPlugin;
      foreach (var dependentPlugin in dependentPlugins) {
        // there can be multiple entries for a single plugin in different groups
        foreach (var item in FindItemsForPlugin(dependentPlugin)) {
          if (item != null && item.Checked) {
            if (!modifiedItems.Contains(item))
              modifiedItems.Add(item);
          }
        }
      }
      // also uncheck all products containing this plugin
      var dependentProducts = from product in products
                              where product.Plugins.Any(p => p.Name == plugin.Name && p.Version == plugin.Version)
                              select product;
      foreach (var dependentProduct in dependentProducts) {
        var item = FindItemForProduct(dependentProduct);
        if (item != null && item.Checked) {
          if (!modifiedItems.Contains(item))
            modifiedItems.Add(item);
        }
      }
      remotePluginsListView.UncheckItems(modifiedItems);
    }

    private void HandlePluginChecked(IPluginDescription plugin) {
      // also check all dependencies
      List<ListViewItem> modifiedItems = new List<ListViewItem>();
      modifiedItems.AddRange(FindItemsForPlugin(plugin));
      foreach (var dep in plugin.Dependencies) {
        // there can be multiple entries for a single plugin in different groups
        foreach (ListViewItem item in FindItemsForPlugin(dep)) {
          if (item != null && !item.Checked) {
            if (!modifiedItems.Contains(item))
              modifiedItems.Add(item);
          }
        }
      }
      remotePluginsListView.CheckItems(modifiedItems);
    }

    #endregion

    #region helper methods
    private IEnumerable<ListViewItem> FindItemsForPlugin(IPluginDescription plugin) {
      return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
              let otherPlugin = item.Tag as IPluginDescription
              where otherPlugin != null && otherPlugin.Name == plugin.Name && otherPlugin.Version == plugin.Version
              select item);
    }

    private ListViewItem FindItemForProduct(DeploymentService.ProductDescription product) {
      return (from item in remotePluginsListView.Items.OfType<ListViewItem>()
              let otherProduct = item.Tag as DeploymentService.ProductDescription
              where otherProduct != null && otherProduct.Name == product.Name && otherProduct.Version == product.Version
              select item).SingleOrDefault();
    }

    private void UpdateRemotePluginList(
      IEnumerable<DeploymentService.ProductDescription> remoteProducts,
      IEnumerable<IPluginDescription> remotePlugins) {

      var mostRecentRemotePlugins = from remote in remotePlugins
                                    where !remotePlugins.Any(x => x.Name == remote.Name && x.Version > remote.Version) // same name and higher version
                                    select remote;

      var newPlugins = from remote in mostRecentRemotePlugins
                       let matchingLocal = (from local in pluginManager.Plugins
                                            where local.Name == remote.Name
                                            where local.Version < remote.Version
                                            select local).FirstOrDefault()
                       where matchingLocal != null
                       select remote;

      NewPlugins = newPlugins;
      Products = remoteProducts;
      AllPlugins = remotePlugins;
    }
    private bool IsNewerThan(IPluginDescription plugin1, IPluginDescription plugin2) {
      // newer: build version is higher, or if build version is the same revision is higher
      if (plugin1.Version.Build < plugin2.Version.Build) return false;
      else if (plugin1.Version.Build > plugin2.Version.Build) return true;
      else return plugin1.Version.Revision > plugin2.Version.Revision;
    }
    #endregion


  }
}
