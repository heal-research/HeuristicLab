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
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Security.DataAccess {
  /// <summary>
  /// The permission database adapter
  /// </summary>
  public interface IPermissionAdapter: IDataAdapter<Permission> {
    /// <summary>
    /// Determines, if the permission Owner has the permission on the entity
    /// </summary>
    /// <param name="permissionOwnerId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    GrantedPermission getPermission(PermissionOwner permissionOwner, Permission permission, Guid entityId);

    /// <summary>
    /// Adds a permission
    /// </summary>
    /// <param name="permissionOwnerId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    bool grantPermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);

    /// <summary>
    /// Removes a permission
    /// </summary>
    /// <param name="permissionOwnerId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    bool revokePermission(Guid permissionOwnerId, Guid permissionId, Guid entityId);
  }
}