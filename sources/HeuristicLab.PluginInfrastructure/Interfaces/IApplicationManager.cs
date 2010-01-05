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
using System.Windows.Forms;
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
    /// Dynamically loads assemblies given in binary form.
    /// </summary>
    /// <param name="assemblies">Assemblies that should be loaded in binary form.</param>
    void LoadAssemblies(IEnumerable<byte[]> assemblies);

    /// <summary>
    /// Discovers and creates instances of <typeparamref name="T"/> and all types implementing or inheriting <typeparamref name="T"/> (directly and indirectly) declared in any assembly of <paramref name="plugin"/>.
    /// </summary>
    /// <typeparam name="T">The type or super-type to discover.</typeparam>
    /// <param name="plugin">The declaring plugin.</param>
    /// <returns>An enumerable of instances of the discovered types.</returns>
    IEnumerable<T> GetInstances<T>(IPluginDescription plugin) where T : class;
    /// <summary>
    /// Discovers and creates instances of <typeparamref name="T"/> and all types implementing or inheriting <typeparamref name="T"/> (directly and indirectly).
    /// </summary>
    /// <typeparam name="T">The type or super-type to discover.</typeparam>
    /// <returns>An enumerable of instances of the discovered types.</returns>
    IEnumerable<T> GetInstances<T>() where T : class;

    /// <summary>
    /// Discovers all types implementing or inheriting <paramref name="type"/> (directly and indirectly).
    /// </summary>
    /// <param name="type">The type to discover.</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(Type type);
    /// <summary>
    /// Discovers all types implementing or inheriting <paramref name="type"/> (directly and indirectly) that are declaed in any assembly of <paramref name="plugin"/>.
    /// </summary>
    /// <param name="type">The type to discover.</param>
    /// <param name="plugin">The declaring plugin.</param>
    /// <returns>An enumerable of discovered types.</returns>
    IEnumerable<Type> GetTypes(Type type, IPluginDescription plugin);
  }
}
