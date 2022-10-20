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
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class UploadPluginsView : InstallationManagerControl {
    private const string UploadMessage = "Uploading plugins...";
    private const string RefreshMessage = "Downloading plugin information from deployment service...";

    private Dictionary<IPluginDescription, IPluginDescription> localAndServerPlugins;
    private BackgroundWorker pluginUploadWorker;
    private BackgroundWorker refreshPluginsWorker;

    private PluginManager pluginManager;
    public PluginManager PluginManager {
      get { return pluginManager; }
      set { pluginManager = value; }
    }

    public UploadPluginsView() {
      InitializeComponent();
      pluginImageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Plugin);
      localAndServerPlugins = new Dictionary<IPluginDescription, IPluginDescription>();

      #region initialize backgroundworkers
      pluginUploadWorker = new BackgroundWorker();
      pluginUploadWorker.DoWork += new DoWorkEventHandler(pluginUploadWorker_DoWork);
      pluginUploadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pluginUploadWorker_RunWorkerCompleted);

      refreshPluginsWorker = new BackgroundWorker();
      refreshPluginsWorker.DoWork += new DoWorkEventHandler(refreshPluginsWorker_DoWork);
      refreshPluginsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshPluginsWorker_RunWorkerCompleted);
      #endregion
    }

    #region refresh plugins from server backgroundworker
    void refreshPluginsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
                   "Please check your connection settings and user credentials.");
      } else {
        UpdatePluginListView((IEnumerable<IPluginDescription>)e.Result);
      }
      StatusView.HideProgressIndicator();
      StatusView.RemoveMessage(RefreshMessage);
      StatusView.UnlockUI();
    }

    void refreshPluginsWorker_DoWork(object sender, DoWorkEventArgs e) {
      // refresh available plugins
      var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        e.Result = client.GetPlugins();
        client.Close();
      }
      catch (TimeoutException) {
        client.Abort();
        throw;
      }
      catch (FaultException) {
        client.Abort();
        throw;
      }
      catch (CommunicationException) {
        client.Abort();
        throw;
      }
    }
    #endregion

    #region upload plugins to server backgroundworker
    void pluginUploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
                   "Please check your connection settings and user credentials.");
      } else {
        UpdatePluginListView((IEnumerable<IPluginDescription>)e.Result);
      }
      StatusView.RemoveMessage(UploadMessage);
      StatusView.HideProgressIndicator();
      StatusView.UnlockUI();
    }

    void pluginUploadWorker_DoWork(object sender, DoWorkEventArgs e) {
      // upload plugins
      var selectedPlugins = (IEnumerable<IPluginDescription>)e.Argument;
      DeploymentService.AdminServiceClient adminClient = DeploymentService.AdminServiceClientFactory.CreateClient();
      Dictionary<IPluginDescription, DeploymentService.PluginDescription> cachedPluginDescriptions =
        new Dictionary<IPluginDescription, DeploymentService.PluginDescription>();
      try {
        foreach (var plugin in IteratePlugins(selectedPlugins)) {
          adminClient.DeployPlugin(MakePluginDescription(plugin, cachedPluginDescriptions), CreateZipPackage(plugin));
        }
        adminClient.Close();
      }
      catch (TimeoutException) {
        adminClient.Abort();
        throw;
      }
      catch (FaultException) {
        adminClient.Abort();
        throw;
      }
      catch (CommunicationException) {
        adminClient.Abort();
        throw;
      }
      // refresh available plugins
      var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        e.Result = client.GetPlugins();
        client.Close();
      }
      catch (TimeoutException) {
        client.Abort();
        throw;
      }
      catch (FaultException) {
        client.Abort();
        throw;
      }
      catch (CommunicationException) {
        client.Abort();
        throw;
      }
    }
    #endregion


    #region button events
    private void uploadButton_Click(object sender, EventArgs e) {
      var selectedPlugins = from item in listView.Items.Cast<ListViewItem>()
                            where item.Checked
                            where item.Tag is IPluginDescription
                            select item.Tag as IPluginDescription;
      if (selectedPlugins.Count() > 0) {
        StatusView.LockUI();
        StatusView.ShowProgressIndicator();
        StatusView.ShowMessage(UploadMessage);
        pluginUploadWorker.RunWorkerAsync(selectedPlugins.ToList());
      }
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.ShowMessage(RefreshMessage);
      refreshPluginsWorker.RunWorkerAsync();
    }

    #endregion

    #region item list events
    private bool ignoreItemCheckedEvents = false;
    private void listView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      if (ignoreItemCheckedEvents) return;
      List<IPluginDescription> modifiedPlugins = new List<IPluginDescription>();
      if (e.Item.Checked) {
        foreach (ListViewItem item in listView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also check all dependencies
          if (!modifiedPlugins.Contains(plugin))
            modifiedPlugins.Add(plugin);
          foreach (var dep in Util.GetAllDependencies(plugin)) {
            if (!modifiedPlugins.Contains(dep))
              modifiedPlugins.Add(dep);
          }
        }
        listView.CheckItems(modifiedPlugins.Select(x => FindItemForPlugin(x)));
      } else {
        foreach (ListViewItem item in listView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also uncheck all dependent plugins
          if (!modifiedPlugins.Contains(plugin))
            modifiedPlugins.Add(plugin);
          foreach (var dep in Util.GetAllDependents(plugin, localAndServerPlugins.Keys)) {
            if (!modifiedPlugins.Contains(dep))
              modifiedPlugins.Add(dep);
          }
        }
        listView.UncheckItems(modifiedPlugins.Select(x => FindItemForPlugin(x)));
      }
      uploadButton.Enabled = (from i in listView.Items.OfType<ListViewItem>()
                              where i.Checked
                              select i).Any();
    }
    #endregion

    #region helper methods
    private void UpdatePluginListView(IEnumerable<IPluginDescription> remotePlugins) {
      // refresh local plugins
      localAndServerPlugins.Clear();
      foreach (var plugin in pluginManager.Plugins) {
        localAndServerPlugins.Add(plugin, null);
      }
      foreach (var plugin in remotePlugins) {
        var matchingLocalPlugin = (from localPlugin in localAndServerPlugins.Keys
                                   where localPlugin.Name == plugin.Name
                                   where localPlugin.Version == plugin.Version
                                   select localPlugin).SingleOrDefault();
        if (matchingLocalPlugin != null) {
          localAndServerPlugins[matchingLocalPlugin] = plugin;
        }
      }
      // refresh the list view with plugins
      listView.Items.Clear();
      ignoreItemCheckedEvents = true;
      foreach (var pair in localAndServerPlugins) {
        var item = MakeListViewItem(pair.Key);
        listView.Items.Add(item);
      }
      Util.ResizeColumns(listView.Columns.OfType<ColumnHeader>());
      ignoreItemCheckedEvents = false;
    }

    private IEnumerable<IPluginDescription> IteratePlugins(IEnumerable<IPluginDescription> plugins) {
      HashSet<IPluginDescription> yieldedItems = new HashSet<IPluginDescription>();
      foreach (var plugin in plugins) {
        foreach (var dependency in IteratePlugins(plugin.Dependencies)) {
          if (!yieldedItems.Contains(dependency)) {
            yieldedItems.Add(dependency);
            yield return dependency;
          }
        }
        if (!yieldedItems.Contains(plugin)) {
          yieldedItems.Add(plugin);
          yield return plugin;
        }
      }
    }

    private static byte[] CreateZipPackage(IPluginDescription plugin) {
      using (MemoryStream stream = new MemoryStream()) {
        ZipArchive zipFile = new ZipArchive(stream, ZipArchiveMode.Create);
        foreach (var file in plugin.Files) {
          zipFile.CreateEntry(file.Name);
        }
        stream.Seek(0, SeekOrigin.Begin);
        return stream.GetBuffer();
      }
    }

    private ListViewItem MakeListViewItem(IPluginDescription plugin) {
      ListViewItem item;
      if (localAndServerPlugins[plugin] != null) {
        item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), 
          localAndServerPlugins[plugin].Version.ToString(), localAndServerPlugins[plugin].Description });
        if (plugin.Version <= localAndServerPlugins[plugin].Version)
          item.ForeColor = Color.Gray;
      } else {
        item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), 
          string.Empty, plugin.Description });
      }
      item.Tag = plugin;
      item.ImageIndex = 0;
      item.Checked = false;
      return item;
    }

    private ListViewItem FindItemForPlugin(IPluginDescription dep) {
      return (from i in listView.Items.Cast<ListViewItem>()
              where i.Tag == dep
              select i).Single();
    }

    private DeploymentService.PluginDescription MakePluginDescription(IPluginDescription plugin, Dictionary<IPluginDescription, DeploymentService.PluginDescription> cachedPluginDescriptions) {
      if (!cachedPluginDescriptions.ContainsKey(plugin)) {
        var dependencies = (from dep in plugin.Dependencies
                            select MakePluginDescription(dep, cachedPluginDescriptions))
                           .ToList();
        cachedPluginDescriptions.Add(plugin,
          new DeploymentService.PluginDescription(plugin.Name, plugin.Version, dependencies, plugin.ContactName, plugin.ContactEmail, plugin.LicenseText));
      }
      return cachedPluginDescriptions[plugin];
    }
    #endregion
  }
}
