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
using System.Security.Policy;
using System.Reflection;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;

namespace HeuristicLab.PluginInfrastructure {

  // must extend MarshalByRefObject because of event passing between Loader and PluginManager (each in it's own AppDomain)
  /// <summary>
  /// Class to manage different plugins.
  /// </summary>
  public class PluginManager : MarshalByRefObject {

    // singleton: only one manager allowed in each AppDomain
    private static PluginManager manager = new PluginManager();
    /// <summary>
    /// Gets the plugin manager (is a singleton).
    /// </summary>
    public static PluginManager Manager {
      get { return manager; }
    }

    // singleton: only one control manager allowed in each applicatoin (i.e. AppDomain)
    private static IControlManager controlManager;
    /// <summary>
    /// Gets or sets the control manager (is a singleton).
    /// </summary>
    public static IControlManager ControlManager {
      get { return controlManager; }
      set { controlManager = value; }
    }

    /// <summary>
    /// Event handler for actions in the plugin manager.
    /// </summary>
    public event PluginManagerActionEventHandler Action;

    // holds a proxy for the loader in the special AppDomain for PluginManagament
    private Loader remoteLoader;
    private AppDomain pluginDomain;
    private string pluginDir;

    // singleton pattern
    private PluginManager() {
      this.pluginDir = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir;
    }

    /// <summary>
    /// Gets all installed plugins.
    /// </summary>
    /// <remarks>This information is provided by a <see cref="Loader"/>.</remarks>
    public ICollection<PluginInfo> InstalledPlugins {
      get { return remoteLoader.InstalledPlugins; }
    }

    /// <summary>
    /// Gets all disabled plugins.
    /// </summary>
    /// <remarks>This information is provided by a <see cref="Loader"/>.</remarks>
    public ICollection<PluginInfo> DisabledPlugins {
      get { return remoteLoader.DisabledPlugins; }
    }

    /// <summary>
    /// Gets all active plugins.
    /// </summary>
    /// <remarks>This information is provided by a <see cref="Loader"/>.</remarks>
    public ICollection<PluginInfo> ActivePlugins {
      get { return remoteLoader.ActivePlugins; }
    }

    /// <summary>
    /// Gets all installed applications.
    /// </summary>
    /// <remarks>This information is provided by a <see cref="Loader"/>.</remarks>
    public ICollection<ApplicationInfo> InstalledApplications {
      get { return remoteLoader.InstalledApplications; }
    }

    private ICollection<PluginInfo> loadedPlugins;
    /// <summary>
    /// Gets or (internally) sets the loaded plugins.
    /// </summary>
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
      remoteLoader.PluginAction += delegate(object sender, PluginManagerActionEventArgs args) { if (Action != null) Action(this, args); };
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
      AppDomain applicationDomain = null;
      try {
        applicationDomain = CreateAndInitAppDomain(appInfo.Name);
        Runner remoteRunner = (Runner)applicationDomain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.GetName().Name, typeof(Runner).FullName);
        remoteRunner.Run(appInfo);
      }
      finally {
        // make sure domain is unloaded in all cases
        if (applicationDomain != null) AppDomain.Unload(applicationDomain);
      }
    }

    /// <summary>
    /// Creates a new AppDomain with all plugins preloaded.
    /// </summary>
    /// <param name="friendlyName">Name of the new AppDomain</param>
    /// <returns>the new AppDomain with all plugins preloaded.</returns>
    public AppDomain CreateAndInitAppDomain(string friendlyName) {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.PrivateBinPath = pluginDir;
      AppDomain applicationDomain = AppDomain.CreateDomain(friendlyName, null, setup);
      Runner remoteRunner = (Runner)applicationDomain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.GetName().Name, typeof(Runner).FullName);
      NotifyListeners(PluginManagerAction.Initializing, "All plugins");
      if (remoteLoader != null) {
        remoteRunner.LoadPlugins(remoteLoader.ActivePlugins);
      } else if (LoadedPlugins != null && LoadedPlugins.Count > 0) {
        remoteRunner.LoadPlugins(LoadedPlugins);
      }
      NotifyListeners(PluginManagerAction.Initialized, "All plugins");
      return applicationDomain;
    }

    /// <summary>
    /// Creates a new AppDomain with all plugins preloaded and Sandboxing capability.
    /// </summary>
    /// <param name="assembly">Assembly reference</param>
    /// <returns>the strongname of the assembly</returns>
    private StrongName CreateStrongName(Assembly assembly) {
      if (assembly == null)
        throw new ArgumentNullException("assembly");

      AssemblyName assemblyName = assembly.GetName();
      Debug.Assert(assemblyName != null, "Could not get assembly name");

      // get the public key blob
      byte[] publicKey = assemblyName.GetPublicKey();
      if (publicKey == null || publicKey.Length == 0)
        throw new InvalidOperationException("Assembly is not strongly named");

      StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

      // and create the StrongName
      return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
    }

    public AppDomain CreateAndInitAppDomainWithSandbox(string friendlyName, bool sandboxed, Type jobType, ICollection<byte[]> assemblyFiles) {
      PermissionSet pset;

      

      //DiscoveryService dService = new DiscoveryService();
      //get the declaring plugin of the job
      //PluginInfo jobPlugin = dService.GetDeclaringPlugin(jobType);

      //get all the plugins that have dependencies with the jobplugin
      //List<PluginInfo> depPlugins = GetDependentPluginsRec(jobPlugin);
      //insert all jobs into one list
      //depPlugins.Add(jobPlugin);
      
      if (sandboxed) {
        pset = new PermissionSet(PermissionState.None);
        pset.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
        pset.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
        FileIOPermission fPerm = new FileIOPermission(PermissionState.None);
             
        /*foreach (PluginInfo plugin in depPlugins) {
            foreach(String assemblies in plugin.Assemblies)
              fPerm.AddPathList(FileIOPermissionAccess.AllAccess, assemblies);
        }
        
        pset.AddPermission(fPerm);*/

      } else {
        pset = new PermissionSet(PermissionState.Unrestricted);
      }
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.PrivateBinPath = pluginDir;
      setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;      
      AppDomain applicationDomain = AppDomain.CreateDomain(friendlyName, AppDomain.CurrentDomain.Evidence, setup, pset, CreateStrongName(Assembly.GetExecutingAssembly()));
                      
      Runner remoteRunner = (Runner)applicationDomain.CreateInstanceAndUnwrap(typeof(Runner).Assembly.GetName().Name, typeof(Runner).FullName);
      NotifyListeners(PluginManagerAction.Initializing, "All plugins");

      if (assemblyFiles != null && assemblyFiles.Count > 0)
        remoteRunner.LoadPlugins(assemblyFiles);
      
      /*if (depPlugins != null && depPlugins.Count > 0) {        
        remoteRunner.LoadPlugins(depPlugins);
      }*/
      NotifyListeners(PluginManagerAction.Initialized, "All plugins");
      return applicationDomain;
    }

    /// <summary>
    /// Calculates a set of plugins that directly or transitively depend on the plugin given in the argument.
    /// </summary>
    /// <param name="pluginInfo">The plugin the other depend on.</param>
    /// <returns>a list of plugins that are directly of transitively dependent.</returns>
    public List<PluginInfo> GetDependentPlugins(PluginInfo pluginInfo) {
      List<PluginInfo> mergedList = new List<PluginInfo>();
      foreach (PluginInfo plugin in InstalledPlugins) {
        if (plugin.Dependencies.Contains(pluginInfo)) {
          if (!mergedList.Contains(plugin)) {
            mergedList.Add(plugin);
          }
          // for each of the dependent plugins add the list of transitively dependent plugins
          // make sure that only one entry for each plugin is added to the merged list
          GetDependentPlugins(plugin).ForEach(delegate(PluginInfo dependentPlugin) {
            if (!mergedList.Contains(dependentPlugin)) {
              mergedList.Add(dependentPlugin);
            }
          });
        }
      }
      return mergedList;
    }

    /// <summary>
    /// Unloads all plugins.
    /// </summary>
    public void UnloadAllPlugins() {
      AppDomain.Unload(pluginDomain);
    }

    /// <summary>
    /// Loads all plugins.
    /// </summary>
    public void LoadAllPlugins() {
      Initialize();
    }

    /// <inheritdoc cref="Loader.OnDelete"/>
    public void OnDelete(PluginInfo pluginInfo) {
      remoteLoader.OnDelete(pluginInfo);
    }

    /// <inheritdoc cref="Loader.OnInstall"/>
    public void OnInstall(PluginInfo pluginInfo) {
      remoteLoader.OnInstall(pluginInfo);
    }

    /// <inheritdoc cref="Loader.OnPreUpdate"/>
    public void OnPreUpdate(PluginInfo pluginInfo) {
      remoteLoader.OnPreUpdate(pluginInfo);
    }

    /// <inheritdoc cref="Loader.OnPostUpdate"/>
    public void OnPostUpdate(PluginInfo pluginInfo) {
      remoteLoader.OnPostUpdate(pluginInfo);
    }

    private void NotifyListeners(PluginManagerAction action, string text) {
      if (Action != null) {
        Action(this, new PluginManagerActionEventArgs(text, action));
      }
    }
  }
}
