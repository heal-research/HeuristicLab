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
  /// <summary>
  /// Class to manage different plugins.
  /// </summary>
  public sealed class PluginManager {
    public event EventHandler<PluginInfrastructureEventArgs> PluginLoaded;
    public event EventHandler<PluginInfrastructureEventArgs> PluginUnloaded;
    public event EventHandler<PluginInfrastructureEventArgs> Initializing;
    public event EventHandler<PluginInfrastructureEventArgs> Initialized;

    private string pluginDir;

    private List<PluginDescription> plugins;
    /// <summary>
    /// Gets all installed plugins.
    /// </summary>
    public IEnumerable<PluginDescription> Plugins {
      get { return plugins; }
    }

    private object locker = new object();
    private bool initialized;

    public PluginManager(string pluginDir) {
      this.pluginDir = pluginDir;
      plugins = new List<PluginDescription>();
      initialized = false;
    }

    /// <summary>
    /// Determines installed plugins and checks if all plugins are loadable.
    /// </summary>
    public void DiscoverAndCheckPlugins() {
      OnInitializing(PluginInfrastructureEventArgs.Empty);

      try {
        PluginValidator pluginValidator = new PluginValidator();
        pluginValidator.PluginDir = pluginDir;
        // forward all events to listeners
        pluginValidator.PluginLoaded +=
          delegate (object sender, PluginInfrastructureEventArgs e) {
            OnPluginLoaded(e);
          };
        // get list of plugins and applications from the validator
        plugins.Clear();
        plugins.AddRange(pluginValidator.Plugins);
      } finally {        
        // unload all plugins
        foreach (var pluginDescription in plugins.Where(x => x.PluginState == PluginState.Loaded)) {
          pluginDescription.Unload();
          OnPluginUnloaded(new PluginInfrastructureEventArgs(pluginDescription));
        }
        initialized = true;
        OnInitialized(PluginInfrastructureEventArgs.Empty);
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
    #endregion
  }
}
