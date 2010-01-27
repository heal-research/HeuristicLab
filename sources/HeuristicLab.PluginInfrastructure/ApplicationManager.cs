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
using System.Linq;
using HeuristicLab.PluginInfrastructure.Manager;
using System.IO;

namespace HeuristicLab.PluginInfrastructure {

  /// <summary>
  /// The ApplicationManager has a reference to the application manager singleton.
  /// The application manager provides
  /// </summary>
  public sealed class ApplicationManager : MarshalByRefObject, IApplicationManager {
    private static IApplicationManager appManager;
    /// <summary>
    /// Gets the application manager singleton.
    /// </summary>
    public static IApplicationManager Manager {
      get { return appManager; }
    }

    internal event EventHandler<PluginInfrastructureEventArgs> PluginLoaded;
    internal event EventHandler<PluginInfrastructureEventArgs> PluginUnloaded;

    // cache for the AssemblyResolveEvent 
    // which must be handled when assemblies are loaded dynamically after the application start
    private Dictionary<string, Assembly> loadedAssemblies;

    private List<IPlugin> loadedPlugins;

    private List<PluginDescription> plugins;
    /// <summary>
    /// Gets all plugins.
    /// </summary>
    public IEnumerable<IPluginDescription> Plugins {
      get { return plugins.Cast<IPluginDescription>(); }
    }

    private List<ApplicationDescription> applications;
    /// <summary>
    /// Gets all installed applications.
    /// </summary>
    public IEnumerable<IApplicationDescription> Applications {
      get { return applications.Cast<IApplicationDescription>(); }
    }

    internal ApplicationManager()
      : base() {
      loadedAssemblies = new Dictionary<string, Assembly>();
      loadedPlugins = new List<IPlugin>();
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
        if (loadedAssemblies.ContainsKey(args.Name)) {
          return loadedAssemblies[args.Name];
        }
        return null;
      };
    }

    internal void PrepareApplicationDomain(IEnumerable<ApplicationDescription> apps, IEnumerable<PluginDescription> plugins) {
      this.plugins = new List<PluginDescription>(plugins);
      this.applications = new List<ApplicationDescription>(apps);
      RegisterApplicationManager((IApplicationManager)this);
      LoadPlugins(plugins);
    }

    /// <summary>
    /// Registers a new application manager.
    /// </summary>
    /// <param name="manager"></param>
    internal static void RegisterApplicationManager(IApplicationManager manager) {
      if (appManager != null) throw new InvalidOperationException("The application manager has already been set.");
      appManager = manager;
    }

    private void LoadPlugins(IEnumerable<PluginDescription> plugins) {
      // load all loadable plugins (all dependencies available) into the execution context
      foreach (var desc in PluginDescriptionIterator.IterateDependenciesBottomUp(plugins.Where(x => x.PluginState != PluginState.Disabled))) {
        foreach (string fileName in desc.AssemblyLocations) {
          var asm = Assembly.LoadFrom(fileName);
          RegisterLoadedAssembly(asm);
          // instantiate and load all plugins in this assembly
          foreach (var plugin in GetInstances<IPlugin>(asm)) {
            plugin.OnLoad();
            loadedPlugins.Add(plugin);
          }
        }
        OnPluginLoaded(new PluginInfrastructureEventArgs("Plugin loaded", desc));
        desc.Load();
      }
    }

    internal void Run(ApplicationDescription appInfo) {
      IApplication runnablePlugin = (IApplication)Activator.CreateInstance(appInfo.DeclaringAssemblyName, appInfo.DeclaringTypeName).Unwrap();
      try {
        runnablePlugin.Run();
      }
      finally {
        // unload plugins in reverse order
        foreach (var plugin in loadedPlugins.Reverse<IPlugin>()) {
          plugin.OnUnload();
        }
        foreach (var desc in PluginDescriptionIterator.IterateDependenciesBottomUp(plugins.Where(x => x.PluginState != PluginState.Disabled))) {
          desc.Unload();
          OnPluginUnloaded(new PluginInfrastructureEventArgs("Plugin unloaded", desc));
        }
      }
    }

    /// <summary>
    /// Loads assemblies dynamically from a byte array
    /// </summary>
    /// <param name="plugins">bytearray of all assemblies that should be loaded</param>
    internal void LoadAssemblies(IEnumerable<byte[]> assemblies) {
      foreach (byte[] asm in assemblies) {
        Assembly loadedAsm = Assembly.Load(asm);
        RegisterLoadedAssembly(loadedAsm);
      }
    }

    // register assembly in the assembly cache for the AssemblyResolveEvent
    private void RegisterLoadedAssembly(Assembly asm) {
      loadedAssemblies.Add(asm.FullName, asm);
      loadedAssemblies.Add(asm.GetName().Name, asm); // add short name
    }

    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type and declared in <paramref name="plugin"/>
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <returns>Enumerable of the created instances.</returns>
    internal static IEnumerable<T> GetInstances<T>(IPluginDescription plugin) where T : class {
      return from t in GetTypes(typeof(T), plugin, true)
             select (T)Activator.CreateInstance(t);
    }
    /// <summary>
    /// Creates an instance of all types declared in assembly <param name="asm"/> that are subtypes or the same type of the specified type and declared in <paramref name="plugin"/>
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <param name="asm">Declaring assembly.</param>
    /// <returns>Enumerable of the created instances.</returns>
    private static IEnumerable<T> GetInstances<T>(Assembly asm) where T : class {
      return from t in GetTypes(typeof(T), asm, true)
             select (T)Activator.CreateInstance(t);
    }
    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <returns>Enumerable of the created instances.</returns>
    internal static IEnumerable<T> GetInstances<T>() where T : class {
      return from i in GetInstances(typeof(T))
             select (T)i;
    }

    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type
    /// </summary>
    /// <typeparam name="type">Most general type.</typeparam>
    /// <returns>Enumerable of the created instances.</returns>
    internal static IEnumerable<object> GetInstances(Type type) {
      return (from t in GetTypes(type, true)
              select Activator.CreateInstance(t)).ToList();
    }

    /// <summary>
    /// Finds all types that are subtypes or equal to the specified type.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(Type type, bool onlyInstantiable) {
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             from t in GetTypes(type, asm, onlyInstantiable)
             select t;
    }

    /// <summary>
    /// Finds all types that are subtypes or equal to the specified type if they are part of the given
    /// <paramref name="plugin"/>.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <param name="plugin">The plugin the subtypes must be part of.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(Type type, IPluginDescription pluginDescription, bool onlyInstantiable) {
      PluginDescription pluginDesc = (PluginDescription)pluginDescription;
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             where !string.IsNullOrEmpty(asm.Location) // ignore dynamically generated assemblies
             where pluginDesc.AssemblyLocations.Any(location => location.Equals(Path.GetFullPath(asm.Location), StringComparison.CurrentCultureIgnoreCase))
             from t in GetTypes(type, asm, onlyInstantiable)
             select t;
    }

    /// <summary>
    /// Gets types that are assignable (same of subtype) to the specified type only from the given assembly.
    /// </summary>
    /// <param name="type">Most general type we want to find.</param>
    /// <param name="assembly">Assembly that should be searched for types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    private static IEnumerable<Type> GetTypes(Type type, Assembly assembly, bool onlyInstantiable) {
      return from t in assembly.GetTypes()
             where type.IsAssignableFrom(t)
             where onlyInstantiable == false || (!t.IsAbstract && !t.IsInterface && !t.HasElementType)
             select t;
    }

    private void OnPluginLoaded(PluginInfrastructureEventArgs e) {
      if (PluginLoaded != null) PluginLoaded(this, e);
    }

    private void OnPluginUnloaded(PluginInfrastructureEventArgs e) {
      if (PluginUnloaded != null) PluginUnloaded(this, e);
    }

    // infinite lease time
    /// <summary>
    /// Initializes the life time service with infinite lease time.
    /// </summary>
    /// <returns><c>null</c>.</returns>
    public override object InitializeLifetimeService() {
      return null;
    }

    #region IApplicationManager Members

    IEnumerable<T> IApplicationManager.GetInstances<T>() {
      return GetInstances<T>();
    }

    IEnumerable<object> IApplicationManager.GetInstances(Type type) {
      return GetInstances(type);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type) {
      return GetTypes(type, true);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, bool onlyInstantiable) {
      return GetTypes(type, onlyInstantiable);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, IPluginDescription plugin) {
      return GetTypes(type, plugin, true);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, IPluginDescription plugin, bool onlyInstantiable) {
      return GetTypes(type, plugin, onlyInstantiable);
    }


    /// <summary>
    /// Finds the plugin that declares the <paramref name="type">type</paramref>.
    /// </summary>
    /// <param name="type">The type of interest.</param>
    /// <returns>The description of the plugin that declares the given type or null if the type has not been declared by a known plugin.</returns>
    public IPluginDescription GetDeclaringPlugin(Type type) {
      foreach (PluginDescription info in Plugins) {
        if (info.AssemblyLocations.Contains(Path.GetFullPath(type.Assembly.Location))) return info;
      }
      return null;
    }
    #endregion
  }
}
