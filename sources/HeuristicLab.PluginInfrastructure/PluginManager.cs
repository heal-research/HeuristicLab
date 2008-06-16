#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.PluginInfrastructure {

  // must extend MarshalByRefObject because of event passing between Loader and PluginManager (each in it's own AppDomain)
  public class PluginManager : MarshalByRefObject {

    // singleton: only one manager allowed in each AppDomain
    private static PluginManager manager = new PluginManager();
    public static PluginManager Manager {
      get { return manager; }
    }

    // singleton: only one control manager allowed in each applicatoin (i.e. AppDomain)
    private static IControlManager controlManager;
    public static IControlManager ControlManager {
      get { return controlManager; }
      set { controlManager = value; }
    }

    public event PluginManagerActionEventHandler Action;

    // holds a proxy for the loader in the special AppDomain for PluginManagament
    private Loader remoteLoader;
    private AppDomain pluginDomain;
    private string pluginDir;

    // singleton pattern
    private PluginManager() {
      this.pluginDir = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir;
    }

    public ICollection<PluginInfo> InstalledPlugins {
      get { return remoteLoader.InstalledPlugins; }
    }

    public ICollection<PluginInfo> DisabledPlugins {
      get { return remoteLoader.DisabledPlugins; }
    }

    public ICollection<PluginInfo> ActivePlugins {
      get { return remoteLoader.ActivePlugins; }
    }

    public ICollection<ApplicationInfo> InstalledApplications {
      get { return remoteLoader.InstalledApplications; }
    }

    private ICollection<PluginInfo> loadedPlugins;
    public ICollection<PluginInfo> LoadedPlugins {
      get { return loadedPlugins; }
      internal set { loadedPlugins = value; }
    }

    /// <summary>
    /// Creates a dedicated AppDomain for loading all plugins and checking dependencies.
    /// </summary>
    public void Initialize() {
      NotifyListeners(PluginManagerAction.Initializing, "-");
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.PrivateBinPath = pluginDir;
      pluginDomain = AppDomain.CreateDomain("plugin domain", null, setup);
      remoteLoader = (Loader)pluginDomain.CreateInstanceAndUnwrap("HeuristicLab.PluginInfraStructure", "HeuristicLab.PluginInfrastructure.Loader");
      remoteLoader.PluginAction += delegate(object sender, PluginManagerActionEventArgs args) { if(Action != null) Action(this, args); };
      remoteLoader.Init();
      NotifyListeners(PluginManagerAction.Initialized, "-");
    }

    /// <summary>
    /// Creates a separate AppDomain.
    /// Loads all active plugin assemblies and starts the application in the new AppDomain via a PluginRunner instance activated in the new AppDomain
    /// </summary>
    /// <param name="appInfo">application to run</param>
    public void Run(ApplicationInfo appInfo) {
      // create a separate AppDomain for the application
      // activate a PluginRunner instance in the application
      // and remotely tell it to start the application

      NotifyListeners(PluginManagerAction.Starting, appInfo.Name);
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.PrivateBinPath = pluginDir;
      AppDomain applicationDomain = AppDomain.CreateDomain(appInfo.Name + " AppDomain", null, setup);
      try {
        Runner remoteRunner = (Runner)applicationDomain.CreateInstanceAndUnwrap("HeuristicLab.PluginInfrastructure", "HeuristicLab.PluginInfrastructure.Runner");
        NotifyListeners(PluginManagerAction.Initializing, "All plugins");
        remoteRunner.LoadPlugins(remoteLoader.ActivePlugins);
        NotifyListeners(PluginManagerAction.Initialized, "All plugins");
        remoteRunner.Run(appInfo);
      } finally {
        // make sure domain is unloaded in all cases
        AppDomain.Unload(applicationDomain);
      }
    }

    /// <summary>
    /// Calculates a set of plugins that directly or transitively depend on the plugin given in the argument.
    /// </summary>
    /// <param name="pluginInfo"></param>
    /// <returns>a list of plugins that are directly of transitively dependent.</returns>
    public List<PluginInfo> GetDependentPlugins(PluginInfo pluginInfo) {
      List<PluginInfo> mergedList = new List<PluginInfo>();
      foreach(PluginInfo plugin in InstalledPlugins) {
        if(plugin.Dependencies.Contains(pluginInfo)) {
          if(!mergedList.Contains(plugin)) {
            mergedList.Add(plugin);
          }
          // for each of the dependent plugins add the list of transitively dependent plugins
          // make sure that only one entry for each plugin is added to the merged list
          GetDependentPlugins(plugin).ForEach(delegate(PluginInfo dependentPlugin) {
            if(!mergedList.Contains(dependentPlugin)) {
              mergedList.Add(dependentPlugin);
            }
          });
        }
      }
      return mergedList;
    }

    public void UnloadAllPlugins() {
      AppDomain.Unload(pluginDomain);
    }

    public void LoadAllPlugins() {
      Initialize();
    }

    public void OnDelete(PluginInfo pluginInfo) {
      remoteLoader.OnDelete(pluginInfo);
    }

    public void OnInstall(PluginInfo pluginInfo) {
      remoteLoader.OnInstall(pluginInfo);
    }

    public void OnPreUpdate(PluginInfo pluginInfo) {
      remoteLoader.OnPreUpdate(pluginInfo);
    }

    public void OnPostUpdate(PluginInfo pluginInfo) {
      remoteLoader.OnPostUpdate(pluginInfo);
    }

    private void NotifyListeners(PluginManagerAction action, string text) {
      if(Action != null) {
        Action(this, new PluginManagerActionEventArgs(text, action));
      }
    }
  }
}
