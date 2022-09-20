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
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

namespace HeuristicLab.PluginInfrastructure.Manager {

  // must extend MarshalByRefObject because of event passing between Loader and PluginManager (each in it's own AppDomain)
  /// <summary>
  /// Class to manage different plugins.
  /// </summary>
  public sealed class PluginManager : MarshalByRefObject {
    public event EventHandler<PluginInfrastructureEventArgs> PluginLoaded;
    public event EventHandler<PluginInfrastructureEventArgs> PluginUnloaded;
    public event EventHandler<PluginInfrastructureEventArgs> Initializing;
    public event EventHandler<PluginInfrastructureEventArgs> Initialized;
    public event EventHandler<PluginInfrastructureEventArgs> ApplicationStarting;
    public event EventHandler<PluginInfrastructureEventArgs> ApplicationStarted;

    private string pluginDir;

    private List<PluginDescription> plugins;
    /// <summary>
    /// Gets all installed plugins.
    /// </summary>
    public IEnumerable<PluginDescription> Plugins {
      get { return plugins; }
    }

    private List<ApplicationDescription> applications;
    /// <summary>
    /// Gets all installed applications.
    /// </summary>
    public IEnumerable<ApplicationDescription> Applications {
      get { return applications; }
    }

    private object locker = new object();
    private bool initialized;

    public PluginManager(string pluginDir) {
      this.pluginDir = pluginDir;
      plugins = new List<PluginDescription>();
      applications = new List<ApplicationDescription>();
      initialized = false;
    }

    /// <summary>
    /// Determines installed plugins and checks if all plugins are loadable.
    /// </summary>
    public void DiscoverAndCheckPlugins() {
      OnInitializing(PluginInfrastructureEventArgs.Empty);
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = pluginDir;
      AppDomain pluginDomain = null;
      try {
        pluginDomain = AppDomain.CreateDomain("plugin domain", null, setup);
        Type pluginValidatorType = typeof(PluginValidator);
        PluginValidator remoteValidator = (PluginValidator)pluginDomain.CreateInstanceAndUnwrap(pluginValidatorType.Assembly.FullName, pluginValidatorType.FullName, true, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
        remoteValidator.PluginDir = pluginDir;
        // forward all events from the remoteValidator to listeners
        remoteValidator.PluginLoaded +=
          delegate(object sender, PluginInfrastructureEventArgs e) {
            OnPluginLoaded(e);
          };
        // get list of plugins and applications from the validator
        plugins.Clear(); applications.Clear();
        plugins.AddRange(remoteValidator.Plugins);
        applications.AddRange(remoteValidator.Applications);
      }
      finally {
        // discard the AppDomain that was used for plugin discovery
        AppDomain.Unload(pluginDomain);
        // unload all plugins
        foreach (var pluginDescription in plugins.Where(x => x.PluginState == PluginState.Loaded)) {
          pluginDescription.Unload();
          OnPluginUnloaded(new PluginInfrastructureEventArgs(pluginDescription));
        }
        initialized = true;
        OnInitialized(PluginInfrastructureEventArgs.Empty);
      }
    }


    /// <summary>
    /// Starts an application in a separate AppDomain.
    /// Loads all enabled plugins and starts the application via an ApplicationManager instance activated in the new AppDomain.
    /// </summary>
    /// <param name="appInfo">application to run</param>
    public void Run(ApplicationDescription appInfo, ICommandLineArgument[] args) {
      if (!initialized) throw new InvalidOperationException("PluginManager is not initialized. DiscoverAndCheckPlugins() must be called before Run()");
      // create a separate AppDomain for the application
      // initialize the static ApplicationManager in the AppDomain
      // and remotely tell it to start the application

      OnApplicationStarting(new PluginInfrastructureEventArgs(appInfo));
      AppDomain applicationDomain = null;
      try {
        AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
        setup.PrivateBinPath = pluginDir;
        applicationDomain = AppDomain.CreateDomain(AppDomain.CurrentDomain.FriendlyName, null, setup);
        Type applicationManagerType = typeof(DefaultApplicationManager);
        DefaultApplicationManager applicationManager =
          (DefaultApplicationManager)applicationDomain.CreateInstanceAndUnwrap(applicationManagerType.Assembly.FullName, applicationManagerType.FullName, true, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
        applicationManager.PluginLoaded += applicationManager_PluginLoaded;
        applicationManager.PluginUnloaded += applicationManager_PluginUnloaded;
        applicationManager.PrepareApplicationDomain(applications, plugins);
        OnApplicationStarted(new PluginInfrastructureEventArgs(appInfo));
        applicationManager.Run(appInfo, args);
      }
      finally {
        // make sure domain is unloaded in all cases
        AppDomain.Unload(applicationDomain);
      }
    }

    private void applicationManager_PluginUnloaded(object sender, PluginInfrastructureEventArgs e) {
      // unload the matching plugin description (
      PluginDescription desc = (PluginDescription)e.Entity;

      // access to plugin descriptions has to be synchronized because multiple applications 
      // can be started or stopped at the same time
      lock (locker) {
        // also unload the matching plugin description in this AppDomain
        plugins.First(x => x.Equals(desc)).Unload();
      }
      OnPluginUnloaded(e);
    }

    private void applicationManager_PluginLoaded(object sender, PluginInfrastructureEventArgs e) {
      // load the matching plugin description (
      PluginDescription desc = (PluginDescription)e.Entity;
      // access to plugin descriptions has to be synchronized because multiple applications 
      // can be started or stopped at the same time
      lock (locker) {
        // also load the matching plugin description in this AppDomain
        plugins.First(x => x.Equals(desc)).Load();
      }
      OnPluginLoaded(e);
    }

    #region event raising methods
    private void OnPluginLoaded(PluginInfrastructureEventArgs e) {
      if (PluginLoaded != null) {
        PluginLoaded(this, e);
      }
    }

    private void OnPluginUnloaded(PluginInfrastructureEventArgs e) {
      if (PluginUnloaded != null) {
        PluginUnloaded(this, e);
      }
    }

    private void OnInitializing(PluginInfrastructureEventArgs e) {
      if (Initializing != null) {
        Initializing(this, e);
      }
    }

    private void OnInitialized(PluginInfrastructureEventArgs e) {
      if (Initialized != null) {
        Initialized(this, e);
      }
    }

    private void OnApplicationStarting(PluginInfrastructureEventArgs e) {
      if (ApplicationStarting != null) {
        ApplicationStarting(this, e);
      }
    }

    private void OnApplicationStarted(PluginInfrastructureEventArgs e) {
      if (ApplicationStarted != null) {
        ApplicationStarted(this, e);
      }
    }
    #endregion

    // infinite lease time
    /// <summary>
    /// Make sure that the plugin manager is never disposed (necessary for cross-app-domain events)
    /// </summary>
    /// <returns><c>null</c>.</returns>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService() {
      return null;
    }
  }
}
