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

using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.PluginInfrastructure;

/// <summary>
/// The service locator for the server core
/// </summary>
class ServiceLocator {
  private static DiscoveryService discoveryService =
    new DiscoveryService();

  private static IClientAdapter clientAdapter = null;

  private static IUserAdapter userAdapter = null;
  
  /// <summary>
  /// Gets the client database adapter
  /// </summary>
  /// <returns></returns>
  internal static IClientAdapter GetClientAdapter() {
    if (clientAdapter == null) {
      clientAdapter = discoveryService.GetInstances<IClientAdapter>()[0];
    }

    return clientAdapter;
  }

  /// <summary>
  /// Gets the user database adapter
  /// </summary>
  /// <returns></returns>
  internal static IUserAdapter GetUserAdapter() {
    if (userAdapter == null) {
      userAdapter = discoveryService.GetInstances<IUserAdapter>()[0];
    }

    return userAdapter;
  }
}