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
  /// The resource database adapter
  /// </summary>
  public interface IResourceAdapter {
    /// <summary>
    /// Save or update the resource
    /// </summary>
    /// <param name="client"></param>
    void UpdateResource(Resource resource);

    /// <summary>
    /// Get the resource with the specified ID
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Resource GetResourceById(long resourceId);

    /// <summary>
    /// Get the resource with the specified name
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Resource GetResourceByName(string name);

    /// <summary>
    /// Gets the resource and updates the values of the object
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    bool GetResourceById(Resource resource);

    /// <summary>
    /// Get all resuorces
    /// </summary>
    /// <returns></returns>
    ICollection<Resource> GetAllResources();

    /// <summary>
    /// Deletes the resource
    /// </summary>
    /// <param name="client"></param>
    bool DeleteResource(Resource resource);
  }
}