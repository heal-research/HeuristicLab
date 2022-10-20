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
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class BasicUpdateView : InstallationManagerControl {
    private BackgroundWorker updatePluginsBackgroundWorker;
    private const string CheckingPluginsMessage = "Checking for updated plugins...";
    private const string NoUpdatesAvailableMessage = "No updates available.";

    private PluginManager pluginManager;
    public PluginManager PluginManager {
      get { return pluginManager; }
      set { pluginManager = value; }
    }

    private InstallationManager installationManager;
    public InstallationManager InstallationManager {
      get { return installationManager; }
      set { installationManager = value; }
    }

    public BasicUpdateView() {
      InitializeComponent();
      updatePluginsBackgroundWorker = new BackgroundWorker();
      updatePluginsBackgroundWorker.DoWork += new DoWorkEventHandler(updatePluginsBackgroundWorker_DoWork);
      updatePluginsBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updatePluginsBackgroundWorker_RunWorkerCompleted);
    }

    #region event handlers for update plugins backgroundworker
    void updatePluginsBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      if (e.Error != null) {
        StatusView.ShowError("Connection Error",
          "There was an error while connecting to the server." + Environment.NewLine +
           "Please check your connection settings and user credentials.");
      } else if (e.Cancelled) {
        StatusView.ShowMessage(NoUpdatesAvailableMessage);
      }
      StatusView.RemoveMessage(CheckingPluginsMessage);
      StatusView.HideProgressIndicator();
      StatusView.UnlockUI();
    }

    void updatePluginsBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      IEnumerable<IPluginDescription> installedPlugins = (IEnumerable<IPluginDescription>)e.Argument;
      var remotePlugins = installationManager.GetRemotePluginList();
      // if there is a local plugin with same name and same major and minor version then it's an update
      var pluginsToUpdate = from remotePlugin in remotePlugins
                            let matchingLocalPlugins = from installedPlugin in installedPlugins
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
        if (!cancelled)
          pluginManager.DiscoverAndCheckPlugins();
        e.Cancel = false;
      } else {
        e.Cancel = true;
      }
    }

    // compares for two plugins with same major and minor version if plugin1 is newer than plugin2
    private static bool IsNewerThan(IPluginDescription plugin1, IPluginDescription plugin2) {
      // newer: build version is higher, or if build version is the same revision is higher
      return plugin1.Version.Build > plugin2.Version.Build ||
        (plugin1.Version.Build == plugin2.Version.Build && plugin1.Version.Revision > plugin2.Version.Revision);
    }

    #endregion
    private void updateAndInstallButton_Click(object sender, EventArgs e) {
      var installedPlugins = pluginManager.Plugins.OfType<IPluginDescription>().ToList();
      updatePluginsBackgroundWorker.RunWorkerAsync(installedPlugins);
      StatusView.LockUI();
      StatusView.ShowProgressIndicator();
      StatusView.RemoveMessage(NoUpdatesAvailableMessage);
      StatusView.ShowMessage(CheckingPluginsMessage);
    }
  }
}
