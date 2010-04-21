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
using System.ServiceModel;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class PluginEditor : InstallationManagerControl {
    private Dictionary<IPluginDescription, DeploymentService.PluginDescription> localAndServerPlugins;
    private BackgroundWorker pluginUploadWorker;
    private BackgroundWorker updateServerPluginsWorker;

    public PluginEditor() {
      InitializeComponent();
      // Caption = "Upload Plugins";

      localAndServerPlugins = new Dictionary<IPluginDescription, DeploymentService.PluginDescription>();

      #region initialize backgroundworkers
      pluginUploadWorker = new BackgroundWorker();
      pluginUploadWorker.DoWork += new DoWorkEventHandler(pluginUploadWorker_DoWork);
      pluginUploadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(pluginUploadWorker_RunWorkerCompleted);

      updateServerPluginsWorker = new BackgroundWorker();
      updateServerPluginsWorker.DoWork += new DoWorkEventHandler(updateServerPluginsWorker_DoWork);
      updateServerPluginsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateServerPluginsWorker_RunWorkerCompleted);
      #endregion
    }

    #region refresh plugins from server backgroundworker
    void updateServerPluginsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (!e.Cancelled && e.Result != null) {
        // refresh local plugins
        localAndServerPlugins.Clear();
        foreach (var plugin in ApplicationManager.Manager.Plugins) {
          localAndServerPlugins.Add(plugin, null);
        }
        // refresh server plugins (find matching local plugins)
        var plugins = (DeploymentService.PluginDescription[])e.Result;
        foreach (var plugin in plugins) {
          var matchingLocalPlugin = (from localPlugin in localAndServerPlugins.Keys
                                     where localPlugin.Name == plugin.Name
                                     where localPlugin.Version == localPlugin.Version
                                     select localPlugin).SingleOrDefault();
          if (matchingLocalPlugin != null) {
            localAndServerPlugins[matchingLocalPlugin] = plugin;
          }
        }
        // refresh the list view with plugins
        listView.Items.Clear();
        listView.CheckBoxes = false;
        //suppressCheckedEvents = true;
        foreach (var pair in localAndServerPlugins) {
          var item = MakeListViewItem(pair.Key);
          listView.Items.Add(item);
        }
        foreach (ColumnHeader column in listView.Columns)
          column.Width = -1;
        //listView.suppressCheckedEvents = false;
        listView.CheckBoxes = true;
        UpdateControlsConnectedState();
      } else {
        UpdateControlsDisconnectedState();
      }
      // make sure cursor is set correctly
      Cursor = Cursors.Default;
    }

    void updateServerPluginsWorker_DoWork(object sender, DoWorkEventArgs e) {
      try {
        var client = DeploymentService.UpdateClientFactory.CreateClient();
        e.Result = client.GetPlugins();
        e.Cancel = false;
      }
      catch (EndpointNotFoundException) {
        e.Result = null;
        e.Cancel = true;
      }
      catch (FaultException) {
        e.Result = null;
        e.Cancel = true;
      }
    }
    #endregion

    #region upload plugins to server backgroundworker
    void pluginUploadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      Cursor = Cursors.Default;
      if (e.Cancelled) {
        UpdateControlsDisconnectedState();
      } else {
        UpdateControlsConnectedState();
        // start another async call to refresh plugin information from server
        RefreshPluginsAsync();
      }
    }

    void pluginUploadWorker_DoWork(object sender, DoWorkEventArgs e) {
      try {
        var selectedPlugins = (IEnumerable<IPluginDescription>)e.Argument;
        DeploymentService.AdminClient adminClient = DeploymentService.AdminClientFactory.CreateClient();

        foreach (var plugin in IteratePlugins(selectedPlugins)) {
          SetMainFormStatusBar("Uploading", plugin);
          adminClient.DeployPlugin(MakePluginDescription(plugin), CreateZipPackage(plugin));
        }
        e.Cancel = false;
      }
      catch (EndpointNotFoundException) {
        e.Cancel = true;
      }
      catch (FaultException) {
        e.Cancel = true;
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
        Cursor = Cursors.AppStarting;
        DisableControl();
        pluginUploadWorker.RunWorkerAsync(selectedPlugins.ToList());
      }
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      DisableControl();
      RefreshPluginsAsync();
    }

    #endregion

    #region item list events
    private void listView_ItemActivate(object sender, EventArgs e) {
      foreach (var item in listView.SelectedItems) {
        var plugin = (IPluginDescription)((ListViewItem)item).Tag;
        var compView = new PluginComparisonView(plugin, localAndServerPlugins[plugin]);
        compView.Show();
      }
    }

    private void listView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      List<IPluginDescription> modifiedPlugins = new List<IPluginDescription>();
      if (e.Item.Checked) {
        foreach (ListViewItem item in listView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also check all dependencies
          if (!modifiedPlugins.Contains(plugin))
            modifiedPlugins.Add(plugin);
          foreach (var dep in GetAllDependencies(plugin)) {
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
          foreach (var dep in GetAllDependents(plugin)) {
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
    private IEnumerable<IPluginDescription> GetAllDependents(IPluginDescription plugin) {
      return from p in localAndServerPlugins.Keys
             let matchingEntries = from dep in GetAllDependencies(p)
                                   where dep.Name == plugin.Name
                                   where dep.Version == plugin.Version
                                   select dep
             where matchingEntries.Any()
             select p;
    }

    private IEnumerable<IPluginDescription> GetAllDependencies(IPluginDescription plugin) {
      foreach (var dep in plugin.Dependencies) {
        foreach (var recDep in GetAllDependencies(dep)) {
          yield return recDep;
        }
        yield return dep;
      }
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

    private byte[] CreateZipPackage(IPluginDescription plugin) {
      using (MemoryStream stream = new MemoryStream()) {
        ZipFile zipFile = new ZipFile(stream);
        zipFile.BeginUpdate();
        foreach (var file in plugin.Files) {
          zipFile.Add(file.Name);
        }
        zipFile.CommitUpdate();
        stream.Seek(0, SeekOrigin.Begin);
        return stream.GetBuffer();
      }
    }

    private ListViewItem MakeListViewItem(IPluginDescription plugin) {
      ListViewItem item;
      if (localAndServerPlugins[plugin] != null) {
        item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), 
          localAndServerPlugins[plugin].Version.ToString(), localAndServerPlugins[plugin].Description });
      } else {
        item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), 
          string.Empty, plugin.Description });
      }
      item.Tag = plugin;
      item.Checked = false;
      return item;
    }

    private ListViewItem FindItemForPlugin(IPluginDescription dep) {
      return (from i in listView.Items.Cast<ListViewItem>()
              where i.Tag == dep
              select i).Single();
    }

    private DeploymentService.PluginDescription MakePluginDescription(IPluginDescription plugin) {
      var dependencies = from dep in plugin.Dependencies
                         select MakePluginDescription(dep);
      return new DeploymentService.PluginDescription(plugin.Name, plugin.Version, dependencies, plugin.ContactName, plugin.ContactEmail, plugin.LicenseText);
    }

    // start background process to refresh the plugin list (local and server)
    private void RefreshPluginsAsync() {
      Cursor = Cursors.AppStarting;
      DisableControl();
      updateServerPluginsWorker.RunWorkerAsync();
    }

    // is called by all methods that start a background process
    // controls must be enabled manuall again when the backgroundworker finishes
    private void DisableControl() {
      //MainFormManager.GetMainForm<MainForm>().ShowProgressBar();
      foreach (Control ctrl in Controls)
        ctrl.Enabled = false;
    }

    private void UpdateControlsDisconnectedState() {
      refreshButton.Enabled = false;

      localAndServerPlugins.Clear();
      listView.Items.Clear();
      listView.Enabled = false;
      uploadButton.Enabled = false;
      //MainFormManager.GetMainForm<MainForm>().HideProgressBar();
    }

    private void UpdateControlsConnectedState() {
      refreshButton.Enabled = true;
      listView.Enabled = true;
      uploadButton.Enabled = false;
      //MainFormManager.GetMainForm<MainForm>().HideProgressBar();
    }
    private void SetMainFormStatusBar(string p, IPluginDescription plugin) {
      if (InvokeRequired) Invoke((Action<string, IPluginDescription>)SetMainFormStatusBar, p, plugin);
      else {
        //MainFormManager.GetMainForm<MainForm>().SetStatusBarText(p + " " + plugin.ToString());
      }
    }

    #endregion
  }
}
