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
  public class AssignedProjectResourceDao : GenericDao<Guid, AssignedProjectResource> {
    public AssignedProjectResourceDao(DataContext dataContext) : base(dataContext) { }

    public override AssignedProjectResource GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IQueryable<AssignedProjectResource> GetByProjectId(Guid projectId) {
      return Table.Where(x => x.ProjectId == projectId);
    }

    public void DeleteByProjectIdAndResourceIds(Guid projectId, IEnumerable<Guid> resourceIds) {
      string paramIds = string.Join(",", resourceIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramIds)) {
        string query = string.Format(DeleteByProjectIdAndResourceIdsQueryString, projectId, paramIds);
        DataContext.ExecuteCommand(query);
      }
    }

    public void DeleteByProjectIdsAndResourceIds(IEnumerable<Guid> projectIds, IEnumerable<Guid> resourceIds) {
      string paramProjectIds = string.Join(",", projectIds.ToList().Select(x => string.Format("'{0}'", x)));
      string paramResourceIds = string.Join(",", resourceIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds) && !string.IsNullOrWhiteSpace(paramResourceIds)) {
        string query = string.Format(DeleteByProjectIdsAndResourceIdsQueryString, paramProjectIds, paramResourceIds);
        DataContext.ExecuteCommand(query);
      }
    }

    public void DeleteByProjectIds(IEnumerable<Guid> projectIds) {
      string paramProjectIds = string.Join(",", projectIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds)) {
        string query = string.Format(DeleteByProjectIdsQueryString, paramProjectIds);
        DataContext.ExecuteCommand(query);
      }
    }

    public bool CheckProjectGrantedForResources(Guid projectId, IEnumerable<Guid> resourceIds) {
      string paramResourceIds = string.Join(",", resourceIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(CheckProjectGrantedForResourcesQueryString, projectId, paramResourceIds);
        return DataContext.ExecuteQuery<int>(queryString).Count() == 0;
      }
      return false;
    }

    public IEnumerable<Resource> GetAllGrantedResourcesByProjectId(Guid projectId) {
      return DataContext.ExecuteQuery<Resource>(GetAllGrantedResourcesByProjectIdQueryString, projectId);
    }

    public IEnumerable<Guid> GetAllGrantedResourceIdsByProjectId(Guid projectId) {
      return DataContext.ExecuteQuery<Guid>(GetAllGrantedResourceIdsByProjectIdQueryString, projectId);
    }

    public IEnumerable<Resource> GetAllGrantedResourcesByProjectIds(IEnumerable<Guid> projectIds) {
      string paramProjectIds = string.Join(",", projectIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds)) {
        string queryString = string.Format(GetAllGrantedResourcesByProjectIdsQueryString, paramProjectIds);
        return DataContext.ExecuteQuery<Resource>(queryString);
      }
      return Enumerable.Empty<Resource>();
    }

    public IEnumerable<Guid> GetAllGrantedResourceIdsOfOwnedParentProjects(Guid projectId, Guid userId) {
      return DataContext.ExecuteQuery<Guid>(GetAllGrantedResourceIdsOfOwnedParentProjectsQueryString, projectId, userId);
    }


    #region Compiled queries
    private static readonly Func<DataContext, Guid, IEnumerable<AssignedProjectResource>> GetByProjectIdGetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        from projectPermission in db.GetTable<AssignedProjectResource>()
        where projectPermission.ProjectId == projectId
        select projectPermission);
    #endregion

    #region String queries
    private const string DeleteByProjectIdAndResourceIdsQueryString = @"
      DELETE FROM [AssignedProjectResource]
        WHERE ProjectId = '{0}'
        AND ResourceId IN ({1});
    ";
    private const string DeleteByProjectIdsAndResourceIdsQueryString = @"
      DELETE FROM [AssignedProjectResource]
        WHERE ProjectId IN ({0})
        AND ResourceId IN ({1});
    ";
    private const string DeleteByProjectIdsQueryString = @"
      DELETE FROM [AssignedProjectResource]
        WHERE ProjectId IN ({0})
    ";
    private const string CheckProjectGrantedForResourcesQueryString = @"
    WITH rtree AS
    (
	    SELECT ResourceId, ParentResourceId
	    FROM [Resource]
	    UNION ALL
	    SELECT rt.ResourceId, r.ParentResourceId
	    FROM [Resource] r
	    JOIN rtree rt ON rt.ParentResourceId = r.ResourceId
    )
    SELECT r.ResourceId
    FROM [Resource] r
    WHERE r.ResourceId IN ({1})
    EXCEPT
    (
	    SELECT rtree.ResourceId
	    FROM rtree, [AssignedProjectResource] apr
	    WHERE rtree.ParentResourceId = apr.ResourceId
	    AND apr.ProjectId = '{0}'
	    UNION
	    SELECT apr.ResourceId
	    FROM [AssignedProjectResource] apr
	    WHERE apr.ProjectId = '{0}'
    )
    ";
    private const string GetAllGrantedResourcesByProjectIdQueryString = @"
      WITH rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId
      )
      SELECT res.*
      FROM rtree, [AssignedProjectResource] apr, [Resource] res
      WHERE rtree.ParentResourceId = apr.ResourceId
      AND rtree.ResourceId = res.ResourceId
      AND apr.ProjectId = '{0}'
      UNION
      SELECT res.*
      FROM [AssignedProjectResource] apr, [Resource] res
      WHERE apr.ResourceId = res.ResourceId
      AND apr.ProjectId = '{0}'
    ";
    private const string GetAllGrantedResourceIdsByProjectIdQueryString = @"
    WITH rtree AS
    (
	    SELECT ResourceId, ParentResourceId
	    FROM [Resource]
	    UNION ALL
	    SELECT rt.ResourceId, r.ParentResourceId
	    FROM [Resource] r
	    JOIN rtree rt ON rt.ParentResourceId = r.ResourceId
    )
    SELECT rtree.ResourceId
    FROM rtree, [AssignedProjectResource] apr
    WHERE rtree.ParentResourceId = apr.ResourceId
    AND apr.ProjectId = '{0}'
    UNION
    SELECT apr.ResourceId
    FROM [AssignedProjectResource] apr
    WHERE apr.ProjectId = '{0}'
    ";
    private const string GetAllGrantedResourcesByProjectIdsQueryString = @"
      WITH rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId
      )
      SELECT res.*
      FROM rtree, [AssignedProjectResource] apr, [Resource] res
      WHERE rtree.ParentResourceId = apr.ResourceId
      AND rtree.ResourceId = res.ResourceId
      AND apr.ProjectId IN ({0})
      UNION
      SELECT res.*
      FROM [AssignedProjectResource] apr, [Resource] res
      WHERE apr.ResourceId = res.ResourceId
      AND apr.ProjectId IN ({0})
    ";
    private const string GetAllGrantedResourceIdsOfOwnedParentProjectsQueryString = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM [Project]
	      UNION ALL
	      SELECT pb.ProjectId, p.ParentProjectId
	      FROM [Project] p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pb.ParentProjectId <> pb.ProjectId
      ),
      rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId AND r.ParentResourceId <> r.ResourceId AND rt.ParentResourceId <> rt.ResourceId
      )
      SELECT DISTINCT rtree.ResourceId
      FROM pbranch, rtree, [Project] pro, [AssignedProjectResource] apr
      WHERE pbranch.ProjectId = {0}
      AND pbranch.ParentProjectId = pro.ProjectId
      AND pro.OwnerUserId = {1}
      AND pbranch.ParentProjectId = apr.ProjectId
      AND apr.ResourceId = rtree.ParentResourceId
    ";
    #endregion
  }
}
