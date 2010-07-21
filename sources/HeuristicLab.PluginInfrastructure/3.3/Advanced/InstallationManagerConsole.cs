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
using HeuristicLab.PluginInfrastructure.Advanced.DeploymentService;
using HeuristicLab.PluginInfrastructure.Manager;


namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal class InstallationManagerConsole {
    private InstallationManager installManager;
    private string connectionString;
    public InstallationManagerConsole(string pluginDir) {

      // get default connection string
      using (var client = new UpdateClient()) {
        connectionString = client.Endpoint.Address.ToString();
      }

      this.installManager = new InstallationManager(pluginDir);
      installManager.PreInstallPlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installManager_PreInstallPlugin);
      installManager.PreRemovePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installManager_PreRemovePlugin);
      installManager.PreUpdatePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installManager_PreUpdatePlugin);
      installManager.PluginInstalled += new EventHandler<PluginInfrastructureEventArgs>(installManager_PluginInstalled);
      installManager.PluginRemoved += new EventHandler<PluginInfrastructureEventArgs>(installManager_PluginRemoved);
      installManager.PluginUpdated += new EventHandler<PluginInfrastructureEventArgs>(installManager_PluginUpdated);
    }

    void installManager_PreUpdatePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      Console.WriteLine("Following plugins are updated:");
      foreach (var info in e.Plugins) {
        Console.WriteLine(e);
      }
      if (GetUserConfirmation()) e.Cancel = false;
      else e.Cancel = true;
      return;
    }

    void installManager_PluginUpdated(object sender, PluginInfrastructureEventArgs e) {
      foreach (var info in (IEnumerable<IPluginDescription>)e.Entity)
        Console.WriteLine("Updated: {0}", info.Name);
    }

    void installManager_PreRemovePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      Console.WriteLine("Following files are deleted:");
      foreach (var plugin in e.Plugins) {
        foreach (var file in plugin.Files)
          Console.WriteLine(file);
      }
      if (GetUserConfirmation()) e.Cancel = false;
      else e.Cancel = true;
      return;
    }

    void installManager_PluginRemoved(object sender, PluginInfrastructureEventArgs e) {
      foreach (string fileName in (IEnumerable<string>)e.Entity)
        Console.WriteLine("Deleted: {0}", fileName);
    }

    void installManager_PreInstallPlugin(object sender, PluginInfrastructureCancelEventArgs e) {

    }

    void installManager_PluginInstalled(object sender, PluginInfrastructureEventArgs e) {

    }

    private static bool GetUserConfirmation() {
      Console.Write("Are you sure? (Y/n)");
      string input = Console.ReadLine().ToUpperInvariant();
      if (string.IsNullOrEmpty(input) || input == "Y") return true;
      else return false;
    }

    public void Show(IEnumerable<string> pluginNames) {
      //foreach (string pluginName in pluginNames)
      //  Console.WriteLine(installManager.GetInformation(pluginName));
    }

    public void Install(IEnumerable<string> pluginNames) {
      //installManager.Install(connectionString, pluginNames);
    }

    public void Remove(IEnumerable<string> pluginNames) {
      // installManager.Remove(pluginNames);
    }

    public void Update(IEnumerable<string> pluginNames) {
      // installManager.Update(connectionString, pluginNames);
    }
  }
}
