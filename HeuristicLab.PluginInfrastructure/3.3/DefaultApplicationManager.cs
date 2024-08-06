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
using System.IO;
using System.Linq;
using System.Reflection;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// The DefaultApplicationManager is registered as ApplicationManager.Manager singleton for each HL application
  /// started via the plugin infrastructure.
  /// It provides properties to retrieve the list of available plugins.
  /// It also provides methods for type discovery and instantiation for types declared in plugins.  
  /// </summary>
  public class DefaultApplicationManager : IApplicationManager {
    private List<IPlugin> loadedPlugins;

    private List<PluginDescription> plugins;
    /// <summary>
    /// Gets all plugins.
    /// </summary>
    public IEnumerable<IPluginDescription> Plugins {
      get { return plugins.Cast<IPluginDescription>(); }
    }

    public DefaultApplicationManager()
     : base() {
      loadedPlugins = new List<IPlugin>();
    }

    /// <summary>
    /// Prepares the application domain for the execution of an HL application.
    /// Pre-loads all <paramref name="plugins"/>.
    /// </summary>    
    /// <param name="plugins">Enumerable of plugins that should be pre-loaded.</param>  
    public void PrepareApplicationDomain(IEnumerable<PluginDescription> plugins) {
      this.plugins = new List<PluginDescription>(plugins);

      ApplicationManager.RegisterApplicationManager(this);
    }

    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type and declared in <paramref name="plugin"/>
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <returns>Enumerable of the created instances.</returns>
    internal static IEnumerable<T> GetInstances<T>(IPluginDescription plugin) where T : class {
      List<T> instances = new List<T>();
      foreach (Type t in GetTypes(typeof(T), plugin, onlyInstantiable: true, includeGenericTypeDefinitions: false)) {
        T instance = null;
        try { instance = (T)Activator.CreateInstance(t); } catch { }
        if (instance != null) instances.Add(instance);
      }
      return instances;
    }
    /// <summary>
    /// Creates an instance of all types declared in assembly <paramref name="asm"/> that are subtypes or the same type of the specified <typeparamref name="type"/>. 
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <param name="asm">Declaring assembly.</param>
    /// <returns>Enumerable of the created instances.</returns>
    private static IEnumerable<T> GetInstances<T>(Assembly asm) where T : class {
      List<T> instances = new List<T>();
      foreach (Type t in GetTypes(typeof(T), asm, onlyInstantiable: true, includeGenericTypeDefinitions: false)) {
        T instance = null;
        try { instance = (T)Activator.CreateInstance(t); } catch { }
        if (instance != null) instances.Add(instance);
      }
      return instances;
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
    /// <param name="type">Most general type.</param>
    /// <returns>Enumerable of the created instances.</returns>
    internal static IEnumerable<object> GetInstances(Type type) {
      List<object> instances = new List<object>();
      foreach (Type t in GetTypes(type, onlyInstantiable: true, includeGenericTypeDefinitions: false)) {
        object instance = null;
        try { instance = Activator.CreateInstance(t); } catch { }
        if (instance != null) instances.Add(instance);
      }
      return instances;
    }

    /// <summary>
    /// Finds all types that are subtypes or equal to the specified type.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable 
    /// <param name="includeGenericTypeDefinitions">Specifies if generic type definitions shall be included</param>
    /// (interfaces, abstract classes... are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(Type type, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             where !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location)
             from t in GetTypes(type, asm, onlyInstantiable, includeGenericTypeDefinitions)
             select t;
    }

    internal static IEnumerable<Type> GetTypes(IEnumerable<Type> types, bool onlyInstantiable, bool includeGenericTypeDefinitions, bool assignableToAllTypes) {
      IEnumerable<Type> result = GetTypes(types.First(), onlyInstantiable, includeGenericTypeDefinitions);
      foreach (Type type in types.Skip(1)) {
        IEnumerable<Type> discoveredTypes = GetTypes(type, onlyInstantiable, includeGenericTypeDefinitions);
        if (assignableToAllTypes) result = result.Intersect(discoveredTypes);
        else result = result.Union(discoveredTypes);
      }
      return result;
    }

    /// <summary>
    /// Finds all types that are subtypes or equal to the specified type if they are part of the given
    /// <paramref name="pluginDescription"/>.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <param name="pluginDescription">The plugin the subtypes must be part of.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable 
    /// <param name="includeGenericTypeDefinitions">Specifies if generic type definitions shall be included</param>
    /// (interfaces, abstract classes... are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(Type type, IPluginDescription pluginDescription, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      PluginDescription pluginDesc = (PluginDescription)pluginDescription;
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             where !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location)
             where pluginDesc.AssemblyLocations.Any(location => location.Equals(Path.GetFullPath(asm.Location), StringComparison.CurrentCultureIgnoreCase))
             from t in GetTypes(type, asm, onlyInstantiable, includeGenericTypeDefinitions)
             select t;
    }

    internal static IEnumerable<Type> GetTypes(IEnumerable<Type> types, IPluginDescription pluginDescription, bool onlyInstantiable, bool includeGenericTypeDefinitions, bool assignableToAllTypes) {
      IEnumerable<Type> result = GetTypes(types.First(), pluginDescription, onlyInstantiable, includeGenericTypeDefinitions);
      foreach (Type type in types.Skip(1)) {
        IEnumerable<Type> discoveredTypes = GetTypes(type, pluginDescription, onlyInstantiable, includeGenericTypeDefinitions);
        if (assignableToAllTypes) result = result.Intersect(discoveredTypes);
        else result = result.Union(discoveredTypes);
      }
      return result;
    }

    /// <summary>
    /// Gets types that are assignable (same of subtype) to the specified type only from the given assembly.
    /// </summary>
    /// <param name="type">Most general type we want to find.</param>
    /// <param name="assembly">Assembly that should be searched for types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable 
    /// (interfaces, abstract classes...  are not returned)</param>
    /// <param name="includeGenericTypeDefinitions">Specifies if generic type definitions shall be included</param>
    /// <returns>Enumerable of the discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(Type type, Assembly assembly, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      var matchingTypes = from assemblyType in assembly.GetTypes()
                          let t = assemblyType.BuildType(type)
                          where t != null
                          where t.IsSubTypeOf(type)
                          where !t.IsNonDiscoverableType()
                          where onlyInstantiable == false || (!t.IsAbstract && !t.IsInterface && !t.HasElementType)
                          where includeGenericTypeDefinitions || !t.IsGenericTypeDefinition
                          select t;

      return matchingTypes;
    }

    /// <summary>
    /// Discovers all types implementing or inheriting all or any type in <paramref name="types"/> (directly and indirectly) that are declared in the assembly <paramref name="assembly"/>.
    /// </summary>
    /// <param name="types">The types to discover.</param>
    /// <param name="assembly">The declaring assembly.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// /// <param name="assignableToAllTypes">Specifies if discovered types must implement or inherit all given <paramref name="types"/>.</param>
    /// <returns>An enumerable of discovered types.</returns>
    internal static IEnumerable<Type> GetTypes(IEnumerable<Type> types, Assembly assembly, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true) {
      IEnumerable<Type> result = GetTypes(types.First(), assembly, onlyInstantiable, includeGenericTypeDefinitions);
      foreach (Type type in types.Skip(1)) {
        IEnumerable<Type> discoveredTypes = GetTypes(type, assembly, onlyInstantiable, includeGenericTypeDefinitions);
        if (assignableToAllTypes) result = result.Intersect(discoveredTypes);
        else result = result.Union(discoveredTypes);
      }
      return result;
    }       

    #region IApplicationManager Members

    IEnumerable<T> IApplicationManager.GetInstances<T>() {
      return GetInstances<T>();
    }

    IEnumerable<object> IApplicationManager.GetInstances(Type type) {
      return GetInstances(type);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      return GetTypes(type, onlyInstantiable, includeGenericTypeDefinitions);
    }
    IEnumerable<Type> IApplicationManager.GetTypes(IEnumerable<Type> types, bool onlyInstantiable, bool includeGenericTypeDefinitions, bool assignableToAllTypes) {
      return GetTypes(types, onlyInstantiable, includeGenericTypeDefinitions, assignableToAllTypes);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, IPluginDescription plugin, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      return GetTypes(type, plugin, onlyInstantiable, includeGenericTypeDefinitions);
    }
    IEnumerable<Type> IApplicationManager.GetTypes(IEnumerable<Type> types, IPluginDescription plugin, bool onlyInstantiable, bool includeGenericTypeDefinitions, bool assignableToAllTypes) {
      return GetTypes(types, plugin, onlyInstantiable, includeGenericTypeDefinitions, assignableToAllTypes);
    }

    IEnumerable<Type> IApplicationManager.GetTypes(Type type, Assembly assembly, bool onlyInstantiable, bool includeGenericTypeDefinitions) {
      return GetTypes(type, assembly, onlyInstantiable, includeGenericTypeDefinitions);
    }
    IEnumerable<Type> IApplicationManager.GetTypes(IEnumerable<Type> types, Assembly assembly, bool onlyInstantiable, bool includeGenericTypeDefinitions, bool assignableToAllTypes) {
      return GetTypes(types, assembly, onlyInstantiable, includeGenericTypeDefinitions, assignableToAllTypes);
    }

    /// <summary>
    /// Finds the plugin that declares the <paramref name="type">type</paramref>.
    /// </summary>
    /// <param name="type">The type of interest.</param>
    /// <returns>The description of the plugin that declares the given type or null if the type has not been declared by a known plugin.</returns>
    public IPluginDescription GetDeclaringPlugin(Type type) {
      if (type == null) throw new ArgumentNullException("type");
      foreach (PluginDescription info in Plugins) {
        if (info.AssemblyLocations.Contains(Path.GetFullPath(type.Assembly.Location))) return info;
      }
      return null;
    }
    #endregion
  }
}

