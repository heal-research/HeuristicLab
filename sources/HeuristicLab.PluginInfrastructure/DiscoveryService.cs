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
using System.Text;
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Provides convenience methods to find specific types or to create instances of types.
  /// </summary>
  public class DiscoveryService {

    public PluginInfo[] Plugins {
      get {
        PluginInfo[] plugins = new PluginInfo[PluginManager.Manager.LoadedPlugins.Count];
        PluginManager.Manager.LoadedPlugins.CopyTo(plugins, 0);
        return plugins;
      }
    }

    /// <summary>
    ///  Find all types that are subtypes or equal to the specified type.
    /// </summary>
    /// <param name="type">Most general type for which to find matching types.</param>
    /// <returns></returns>
    public Type[] GetTypes(Type type) {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      List<Type> types = new List<Type>();
      foreach(Assembly asm in assemblies) {
        Array.ForEach<Type>(GetTypes(type, asm), delegate(Type t) {
          types.Add(t);
        });
      }
      return types.ToArray();
    }

    /// <summary>
    /// Create an instance of all types that are subtypes or the same type of the specified type
    /// </summary>
    /// <typeparam name="T">Most general type.</typeparam>
    /// <returns></returns>
    public T[] GetInstances<T>() where T : class {
      Type[] types = GetTypes(typeof(T));
      List<T> instances = new List<T>();
      foreach(Type t in types) {
        if(!t.IsAbstract && !t.IsInterface && !t.HasElementType) {
          instances.Add((T)Activator.CreateInstance(t));
        }
      }
      return instances.ToArray();
    }

    public Type[] GetTypes(Type type, PluginInfo plugin) {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      List<Type> types = new List<Type>();
      foreach(Assembly asm in assemblies) {
        if(plugin.Assemblies.Contains(asm.Location)) {
          Array.ForEach<Type>(GetTypes(type, asm), delegate(Type t) {
            types.Add(t);
          });
        }
      }

      return types.ToArray();
    }

    /// <summary>
    /// Get instances of all types that implement the specified interface only in the given assembly.
    /// </summary>
    /// <typeparam name="T">Interface type.</typeparam>
    /// <param name="assembly">Assembly that should be searched for types.</param>
    /// <returns></returns>
    internal T[] GetInstances<T>(Assembly assembly) {
      Type[] types = GetTypes(typeof(T), assembly);
      List<T> instances = new List<T>();
      foreach(Type t in types) {
        if(!t.IsAbstract && !t.IsInterface && !t.HasElementType) {
          instances.Add((T)Activator.CreateInstance(t));
        }
      }
      return instances.ToArray();
    }

    /// <summary>
    /// Get types that are assignable (same of subtype) to the specified type only from the given assembly.
    /// </summary>
    /// <param name="type">Most general type we want to find.</param>
    /// <param name="assembly">Assembly that should be searched for types.</param>
    /// <returns></returns>
    internal Type[] GetTypes(Type type, Assembly assembly) {
      List<Type> types = new List<Type>();
      foreach(Type t in assembly.GetTypes()) {
        if(type.IsAssignableFrom(t)) {
          types.Add(t);
        }
      }
      return types.ToArray();
    }
  }
}
