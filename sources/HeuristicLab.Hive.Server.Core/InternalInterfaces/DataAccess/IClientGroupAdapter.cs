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
  /// The client group database adapter
  /// </summary>
  public interface IClientGroupAdapter {
    /// <summary>
    /// Save or update the client group
    /// </summary>
    /// <param name="client"></param>
    void UpdateClientGroup(ClientGroup group);

    /// <summary>
    /// Get the client group with the specified ID
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    ClientGroup GetClientGroupById(long clientGroupId);

    /// <summary>
    /// Get all client groups
    /// </summary>
    /// <returns></returns>
    ICollection<ClientGroup> GetAllClientGroups();

    /// <summary>
    /// Gets all client groups where the resource is member of
    /// </summary>
    /// <param name="permOwner"></param>
    /// <returns></returns>
    ICollection<ClientGroup> MemberOf(Resource resource);

    /// <summary>
    /// Deletes the client group
    /// </summary>
    /// <param name="client"></param>
    bool DeleteClientGroup(ClientGroup group);
  }
}
