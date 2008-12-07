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
  /// The permission owner database adapter
  /// </summary>
  public interface IPermissionOwnerAdapter {
    /// <summary>
    /// Save or update the permission owner
    /// </summary>
    /// <param name="client"></param>
    void UpdatePermissionOwner(PermissionOwner permOwner);

    /// <summary>
    /// Gets the permission owner and updates the values of the object
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    bool GetPermissionOwnerById(PermissionOwner permOwner);

    /// <summary>
    /// Get the permission owner with the specified ID
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    PermissionOwner GetPermissionOwnerById(long permOwnerId);

    /// <summary>
    /// Get the permission owner with the specified name
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    PermissionOwner GetPermissionOwnerByName(String name);

    /// <summary>
    /// Get all permission owners
    /// </summary>
    /// <returns></returns>
    ICollection<PermissionOwner> GetAllPermissionOwners();

    /// <summary>
    /// Deletes the permission owner
    /// </summary>
    /// <param name="permOwner"></param>
    bool DeletePermissionOwner(PermissionOwner permOwner);
  }
}