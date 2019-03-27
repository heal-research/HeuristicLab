#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class AssignedResourceDao : GenericDao<Guid, AssignedResource> {
    public AssignedResourceDao(DataContext dataContext) : base(dataContext) { }

    public override AssignedResource GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IQueryable<AssignedResource> GetByTaskId(Guid taskId) {
      return Table.Where(x => x.TaskId == taskId);
    }

    public bool TaskIsAllowedToBeCalculatedBySlave(Guid taskId, Guid slaveId) {
      return DataContext.ExecuteQuery<int>(TaskIsAllowedToBeCalculatedBySlaveQueryString, slaveId, taskId).First() > 0;
    }

    #region String queries
    private const string TaskIsAllowedToBeCalculatedBySlaveQueryString = @"
      WITH pr AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r JOIN pr ON r.ResourceId = pr.ParentResourceId
      )
      SELECT COUNT(ar.TaskId)
      FROM pr JOIN AssignedResources ar ON pr.ResourceId = ar.ResourceId
      WHERE ar.TaskId = {1}
    ";
    #endregion
  }
}
