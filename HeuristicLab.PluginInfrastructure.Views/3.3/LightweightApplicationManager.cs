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

namespace HeuristicLab.PluginInfrastructure {

  /// <summary>
  /// Lightweight application manager is set as the application manager as long as the plugin infrastructure is uninitialized.
  /// The list of plugins and applications is empty. The default application manager is necessary to provide the type discovery
  /// functionality in unit tests.
  /// </summary>
  internal sealed class LightweightApplicationManager : IApplicationManager {
    internal LightweightApplicationManager() {
      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
    }

    Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
      return null;
    }


    #region IApplicationManager Members
    /// <summary>
    /// Gets an empty list of plugins. (LightweightApplicationManager doesn't support plugin discovery)
    /// </summary>
    public IEnumerable<IPluginDescription> Plugins {
      get { return new IPluginDescription[0]; }
    }

    /// <summary>
    /// Gets an empty list of applications. (LightweightApplicationManager doesn't support application discovery)
    /// </summary>
    public IEnumerable<IApplicationDescription> Applications {
      get { return new IApplicationDescription[0]; }
    }

    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <returns>Enumerable of the created instances.</returns>
    public IEnumerable<T> GetInstances<T>() where T : class {
      return GetInstances(typeof(T)).Cast<T>();
    }

    /// <summary>
    /// Creates an instance of all types that are subtypes or the same type of the specified type
    /// </summary>
    /// <param name="type">Most general type.</param>
    /// <returns>Enumerable of the created instances.</returns>
    public IEnumerable<object> GetInstances(Type type) {
      List<object> instances = new List<object>();
      foreach (Type t in GetTypes(type)) {
        object instance = null;
        try { instance = Activator.CreateInstance(t); }
        catch { }
        if (instance != null) instances.Add(instance);
      }
      return instances;
    }

    /// <summary>
    /// Finds all instantiable types that are subtypes or equal to the specified types.
    /// </summary>
    /// <param name="types">Most general types for which to find matching types.</param>
    /// <remarks>Return only types that are instantiable 
    /// (interfaces, abstract classes... are not returned)</remarks>
    /// <param name="includeGenericTypeDefinitions">Specifies if generic type definitions shall be included</param>
    /// <returns>Enumerable of the discovered types.</returns>
    public IEnumerable<Type> GetTypes(IEnumerable<Type> types, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true) {
      IEnumerable<Type> result = GetTypes(types.First(), onlyInstantiable, includeGenericTypeDefinitions);
      foreach (Type type in types.Skip(1)) {
        IEnumerable<Type> discoveredTypes = GetTypes(type, onlyInstantiable, includeGenericTypeDefinitions);
        if (assignableToAllTypes) result = result.Intersect(discoveredTypes);
        else result = result.Union(discoveredTypes);
      }
      return result;
    }

    /// <summary>
    /// Finds all types that are subtypes or equal to the specified type.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable 
    /// (interfaces, abstract classes... are not returned)</param>
    /// <param name="includeGenericTypeDefinitions">Specifies if generic type definitions shall be included</param>
    /// <returns>Enumerable of the discovered types.</returns>
    public IEnumerable<Type> GetTypes(Type type, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false) {
      return from asm in AppDomain.CurrentDomain.GetAssemblies()
             from t in GetTypes(type, asm, onlyInstantiable, includeGenericTypeDefinitions)
             select t;
    }

    /// <summary>
    /// Gets types that are assignable (same of subtype) to the specified type only from the given assembly.
    /// </summary>
    /// <param name="type">Most general type we want to find.</param>
    /// <param name="assembly">Assembly that should be searched for types.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable 
    /// (interfaces, abstract classes...  are not returned)</param>
    /// <returns>Enumerable of the discovered types.</returns>
    public IEnumerable<Type> GetTypes(Type type, Assembly assembly, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false) {
      try {
        // necessary to make sure the exception is immediately thrown
        // instead of later when the enumerable is iterated?
        var assemblyTypes = assembly.GetTypes();

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
      catch (TypeLoadException) {
        return Enumerable.Empty<Type>();
      }
      catch (ReflectionTypeLoadException) {
        return Enumerable.Empty<Type>();
      }
    }

    /// <summary>
    /// Discovers all types implementing or inheriting all or any type in <paramref name="types"/> (directly and indirectly) that are declared in the assembly <paramref name="assembly"/>.
    /// </summary>
    /// <param name="types">The types to discover.</param>
    /// <param name="assembly">The declaring assembly.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// /// <param name="assignableToAllTypes">Specifies if discovered types must implement or inherit all given <paramref name="types"/>.</param>
    /// <returns>An enumerable of discovered types.</returns>
    public IEnumerable<Type> GetTypes(IEnumerable<Type> types, Assembly assembly, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true) {
      IEnumerable<Type> result = GetTypes(types.First(), assembly, onlyInstantiable, includeGenericTypeDefinitions);
      foreach (Type type in types.Skip(1)) {
        IEnumerable<Type> discoveredTypes = GetTypes(type, assembly, onlyInstantiable, includeGenericTypeDefinitions);
        if (assignableToAllTypes) result = result.Intersect(discoveredTypes);
        else result = result.Union(discoveredTypes);
      }
      return result;
    }

    /// <summary>
    /// Not supported by the LightweightApplicationManager
    /// </summary>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <returns></returns>
    /// <throws>NotSupportedException</throws>
    public IEnumerable<Type> GetTypes(Type type, IPluginDescription plugin) {
      throw new NotSupportedException("LightweightApplicationManager doesn't support type discovery for plugins.");
    }

    /// <summary>
    /// Not supported by the LightweightApplicationManager
    /// </summary>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <param name="onlyInstantiable"></param>
    /// <param name="includeGenericTypeDefinitions"></param>
    /// <returns></returns>
    /// <throws>NotSupportedException</throws>
    public IEnumerable<Type> GetTypes(Type type, IPluginDescription plugin, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false) {
      throw new NotSupportedException("LightweightApplicationManager doesn't support type discovery for plugins.");
    }

    /// <summary>
    /// Not supported by the LightweightApplicationManager
    /// </summary>
    /// <param name="type"></param>
    /// <param name="plugin"></param>
    /// <param name="onlyInstantiable"></param>
    /// <param name="includeGenericTypeDefinitions"></param>
    /// <returns></returns>
    /// <throws>NotSupportedException</throws>
    public IEnumerable<Type> GetTypes(IEnumerable<Type> types, IPluginDescription plugin, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true) {
      throw new NotSupportedException("LightweightApplicationManager doesn't support type discovery for plugins.");
    }

    /// <summary>
    /// Not supported by the LightweightApplicationManager
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <throws>NotSupportedException</throws>
    public IPluginDescription GetDeclaringPlugin(Type type) {
      throw new NotSupportedException("LightweightApplicationManager doesn't support type discovery for plugins.");
    }

    #endregion
  }
}
