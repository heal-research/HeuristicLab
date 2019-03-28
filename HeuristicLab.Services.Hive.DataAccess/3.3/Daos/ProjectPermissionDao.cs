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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class ProjectPermissionDao : GenericDao<Guid, ProjectPermission> {
    public ProjectPermissionDao(DataContext dataContext) : base(dataContext) { }

    public override ProjectPermission GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IEnumerable<ProjectPermission> GetByProjectId(Guid id) {
      return GetByProjectIdGetByIdQuery(DataContext, id);
    }

    public bool CheckUserGrantedForProject(Guid projectId, IEnumerable<Guid> userAndGroupIds) {
      string paramUserAndGroupIds = string.Join(",", userAndGroupIds.ToList().Select(x => string.Format("'{0}'", x)));
      if(!string.IsNullOrWhiteSpace(paramUserAndGroupIds)) {
        string queryString = string.Format(CheckUserGrantedForProjectQueryString, projectId, paramUserAndGroupIds);
        return DataContext.ExecuteQuery<int>(queryString).First() > 0;
      }
      return false;
    }

    public void DeleteByProjectIdAndGrantedUserIds(Guid projectId, IEnumerable<Guid> grantedUserIds) {
      string paramIds = string.Join(",", grantedUserIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramIds)) {
        string query = string.Format(DeleteByProjectIdAndGrantedUserIdsQueryString, projectId, paramIds);
        DataContext.ExecuteCommand(query);
      }
    }

    public void DeleteByProjectIdsAndGrantedUserIds(IEnumerable<Guid> projectIds, IEnumerable<Guid> grantedUserIds) {
      string paramProjectIds = string.Join(",", projectIds.ToList().Select(x => string.Format("'{0}'", x)));
      string paramUserIds = string.Join(",", grantedUserIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds) && !string.IsNullOrWhiteSpace(paramUserIds)) {
        string query = string.Format(DeleteByProjectIdsAndGrantedUserIdsQueryString, paramProjectIds, paramUserIds);
        DataContext.ExecuteCommand(query);
      }
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, IEnumerable<ProjectPermission>> GetByProjectIdGetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        from projectPermission in db.GetTable<ProjectPermission>()
        where projectPermission.ProjectId == projectId
        select projectPermission);
    #endregion

    #region String queries
    private const string DeleteByProjectIdAndGrantedUserIdsQueryString = @"
      DELETE FROM [ProjectPermission]
      WHERE ProjectId = '{0}'
      AND GrantedUserId IN ({1});
    ";
    private const string DeleteByProjectIdsAndGrantedUserIdsQueryString = @"
      DELETE FROM [ProjectPermission]
      WHERE ProjectId IN ({0})
      AND GrantedUserId IN ({1});
    ";
    private const string CheckUserGrantedForProjectQueryString = @"
      SELECT COUNT(pp.ProjectId)
      FROM [ProjectPermission] pp
      WHERE pp.ProjectId = '{0}'
      AND pp.GrantedUserId IN ({1})
    ";
    private const string GetGrantedProjectsForUserQueryString = @"
      SELECT DISTINCT p.*
      FROM [ProjectPermission] pp, [Project] p
      WHERE pp.GrantedUserId IN ({0})
      AND  pp.ProjectId = p.ProjectId
    ";
    #endregion
  }
}
