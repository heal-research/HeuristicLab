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
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess {
  /// <summary>
  /// The client database adapter
  /// </summary>
  public interface IClientAdapter {
    /// <summary>
    /// Save or update the client
    /// </summary>
    /// <param name="client"></param>
    void UpdateClient(ClientInfo client);

    /// <summary>
    /// Get the client with the specified ID
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    ClientInfo GetClientById(Guid clientId);

    /// <summary>
    /// Get the client with the specified name
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    ClientInfo GetClientByName(string name);

    /// <summary>
    /// Get the client with the specified ID
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    ClientInfo GetClientById(long id);

    /// <summary>
    /// Get all clients
    /// </summary>
    /// <returns></returns>
    ICollection<ClientInfo> GetAllClients();

    /// <summary>
    /// Deletes the client
    /// </summary>
    /// <param name="client"></param>
    bool DeleteClient(ClientInfo client);
  }
}
