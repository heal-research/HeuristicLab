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
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Interface for application managers.
  /// </summary>
  public interface IApplicationManager {
    /// <summary>
    /// Gets all discovered plugins.
    /// </summary>
    IEnumerable<IPluginDescription> Plugins { get; }

    /// <summary>
    /// Gets all discovered applications.
    /// </summary>
    IEnumerable<IApplicationDescription> Applications { get; }

    /// <summary>
    /// Discovers and creates instances of <typeparamref name="T"/> and all types implementing or inheriting <typeparamref name="T"/> (directly and indirectly).
    /// </summary>
    /// <typeparam name="T">The type or super-type to discover.</typeparam>
    /// <returns>An enumerable of instances of the discovered types.</returns>
    IEnumerable<T> GetInstances<T>() where T : class;

    /// <summary>
    /// Discovers and creates instances of <paramref name="type"/> and all types implementing or inheriting <paramref name="type"/> (directly and indirectly).
    /// </summary>
    /// <param name="type">The type or super-type to discover.</param>
    /// <returns>An enumerable of instances of the discovered types.</returns>
    IEnumerable<object> GetInstances(Type type);

    /// <summary>
    /// Discovers all types implementing or inheriting <paramref name="type"/> (directly and indirectly).
    /// </summary>
    /// <param name="type">The type to discover.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(Type type, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false);

    /// <summary>
    /// Discovers all types implementing or inheriting all or any type in <paramref name="types"/> (directly and indirectly).
    /// </summary>
    /// <param name="types">The types to discover.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <param name="assignableToAllTypes">Specifies if discovered types must implement or inherit all given <paramref name="types"/>.</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(IEnumerable<Type> types, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true);

    /// <summary>
    /// Discovers all types implementing or inheriting <paramref name="type"/> (directly and indirectly) that are declared in any assembly of <paramref name="plugin"/>.
    /// </summary>
    /// <param name="type">The type to discover.</param>
    /// <param name="plugin">The declaring plugin.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(Type type, IPluginDescription plugin, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false);

    /// <summary>
    /// Discovers all types implementing or inheriting all or any type in <paramref name="types"/> (directly and indirectly) that are declared in any assembly of <paramref name="plugin"/>.
    /// </summary>
    /// <param name="types">The types to discover.</param>
    /// <param name="plugin">The declaring plugin.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// /// <param name="assignableToAllTypes">Specifies if discovered types must implement or inherit all given <paramref name="types"/>.</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(IEnumerable<Type> types, IPluginDescription plugin, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true);


    /// <summary>
    /// Discovers all types implementing or inheriting <paramref name="type"/> (directly and indirectly) that are declared in the<paramref name="assembly"/>.
    /// </summary>
    /// <param name="type">The type to discover.</param>
    /// <param name="assembly">The declaring assembly.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(Type type, Assembly assembly, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false);

    /// <summary>
    /// Discovers all types implementing or inheriting all or any type in <paramref name="types"/> (directly and indirectly) that are declaed in any assembly of <paramref name="plugin"/>.
    /// </summary>
    /// <param name="types">The types to discover.</param>
    /// <param name="assembly">The declaring assembly.</param>
    /// <param name="onlyInstantiable">Return only types that are instantiable (instance, abstract... are not returned)</param>
    /// /// <param name="assignableToAllTypes">Specifies if discovered types must implement or inherit all given <paramref name="types"/>.</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(IEnumerable<Type> types, Assembly assembly, bool onlyInstantiable = true, bool includeGenericTypeDefinitions = false, bool assignableToAllTypes = true);

    /// <summary>
    /// Finds the plugin that declares the <paramref name="type">type</paramref>.
    /// </summary>
    /// <param name="type">The type of interest.</param>
    /// <returns>The description of the plugin that declares the given type or null if the type has not been declared by a known plugin.</returns>
    IPluginDescription GetDeclaringPlugin(Type type);
  }
}
