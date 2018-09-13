#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ProjectDao : GenericDao<Guid, Project> {
    public ProjectDao(DataContext dataContext) : base(dataContext) { }

    public class ProjectStats {
      public Guid ProjectId { get; set; }
      public int Cores { get; set; }
      public int Memory { get; set; }
    }

    public override Project GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public void DeleteByIds(IEnumerable<Guid> ids) {
      string paramProjectIds = string.Join(",", ids.ToList().Select(x => string.Format("'{0}'", x)));
      if(!string.IsNullOrWhiteSpace(paramProjectIds)) {
        string queryString = string.Format(DeleteByIdsQueryString, paramProjectIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public IEnumerable<Project> GetUsageGrantedProjectsForUser(IEnumerable<Guid> userAndGroupIds) {
      string paramUserAndGroupIds = string.Join(",", userAndGroupIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramUserAndGroupIds)) {
        string queryString = string.Format(GetUsageGrantedProjectsForUserQueryString, paramUserAndGroupIds);
        return DataContext.ExecuteQuery<Project>(queryString);
      }
      return Enumerable.Empty<Project>();
    }

    public IEnumerable<Project> GetAdministrationGrantedProjectsForUser(Guid userId) {
      return DataContext.ExecuteQuery<Project>(GetAdministrationGrantedProjectsForUserQueryString, userId);
    }

    public IEnumerable<Project> GetChildProjectsById(Guid id) {
      return DataContext.ExecuteQuery<Project>(GetChildProjectsByIdQuery, id);
    }

    public IEnumerable<Guid> GetChildProjectIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetChildProjectIdsByIdQuery, id);
    }

    public IEnumerable<Project> GetParentProjectsById(Guid id) {
      return DataContext.ExecuteQuery<Project>(GetParentProjectsByIdQuery, id);
    }

    public IEnumerable<Guid> GetParentProjectIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetParentProjectIdsByIdQuery, id);
    }

    public IEnumerable<Project> GetCurrentAndParentProjectsById(Guid id) {
      return DataContext.ExecuteQuery<Project>(GetCurrentAndParentProjectsByIdQuery, id);
    }

    public IEnumerable<Guid> GetCurrentAndParentProjectIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetCurrentAndParentProjectIdsByIdQuery, id);
    }

    public IEnumerable<ProjectStats> GetUsageStatsPerProject() {
      return DataContext.ExecuteQuery<ProjectStats>(GetUsageStatsPerProjectQueryString).ToList();
    }

    public IEnumerable<ProjectStats> GetAvailabilityStatsPerProject() {
      return DataContext.ExecuteQuery<ProjectStats>(GetAvailabilityStatsPerProjectQueryString).ToList();
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Project> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        (from project in db.GetTable<Project>()
         where project.ProjectId == projectId
         select project).SingleOrDefault());
    #endregion

    #region String queries
    private const string DeleteByIdsQueryString = @"
      DELETE FROM [Project]
      WHERE ProjectId IN ({0})
    ";

    private const string GetUsageStatsPerProjectQueryString = @"
      SELECT j.ProjectId, SUM(t.CoresNeeded) AS Cores, SUM(t.MemoryNeeded) AS Memory
      FROM [Task] t, [Job] j
      WHERE t.TaskState = 'Calculating'
      AND t.JobId = j.JobId
      GROUP BY j.ProjectId
    ";

    private const string GetAvailabilityStatsPerProjectQueryString = @"
      WITH rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId
      )
      SELECT apr.ProjectId, SUM(res.Cores) AS Cores, SUM(res.Memory) AS Memory
      FROM rtree, [AssignedProjectResource] apr, [Resource] res
      WHERE rtree.ResourceId = res.ResourceId
      AND res.ResourceType = 'Slave'
      AND (res.SlaveState = 'Idle' OR SlaveState = 'Calculating')
      AND rtree.ParentResourceId = apr.ResourceId
      GROUP BY apr.ProjectId
      UNION
      SELECT apr.ProjectId, SUM(res.Cores) AS Cores, SUM(res.Memory) AS Memory
      FROM [AssignedProjectResource] apr, [Resource] res
      WHERE apr.ResourceId = res.ResourceId
      AND res.ResourceType = 'Slave'
      AND (res.SlaveState = 'Idle' OR SlaveState = 'Calculating')
      GROUP BY apr.ProjectId
    ";

    private const string GetUsageGrantedProjectsForUserQueryString = @"
      SELECT DISTINCT p.*
      FROM [Project] p, [ProjectPermission] pp
      WHERE p.ProjectId = pp.ProjectId
      AND pp.GrantedUserId IN ({0})
    ";

    private const string GetAdministrationGrantedProjectsForUserQueryString = @"
      WITH ptree AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM [Project]
	      UNION ALL
	      SELECT pt.ProjectId, p.ParentProjectId
	      FROM [Project] p
	      JOIN ptree pt ON pt.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pt.ParentProjectId <> pt.ProjectId
      )
      SELECT DISTINCT parent.*
      FROM [Project] parent
      WHERE parent.OwnerUserId = {0}
      UNION
      SELECT DISTINCT child.* 
      FROM ptree, [Project] parent, [Project] child
      WHERE ptree.ParentProjectId = parent.ProjectId
      AND ptree.ProjectId = child.ProjectId
      AND parent.OwnerUserId = {0}
    ";

    private const string GetChildProjectsByIdQuery = @"
    	WITH ptree AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM [Project]
	      UNION ALL
	      SELECT pt.ProjectId, p.ParentProjectId
	      FROM [Project] p
	      JOIN ptree pt ON pt.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pt.ParentProjectId <> pt.ProjectId
      )
      SELECT DISTINCT pro.*
      FROM ptree, [Project] pro
      WHERE ptree.ParentProjectId = {0}
	    AND ptree.ProjectId = pro.ProjectId
    ";
    private const string GetChildProjectIdsByIdQuery = @"
      WITH ptree AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM [Project]
	      UNION ALL
	      SELECT pt.ProjectId, r.ParentProjectId
	      FROM [Project] r
	      JOIN ptree pt ON pt.ParentProjectId = r.ProjectId AND r.ParentProjectId <> r.ProjectId AND pt.ParentProjectId <> pt.ProjectId
      )
      SELECT DISTINCT ptree.ProjectId
      FROM ptree
      WHERE ptree.ParentProjectId = {0}
    ";
    private const string GetParentProjectsByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId
        FROM [Project]
	      UNION ALL
	      SELECT pb.ProjectId, p.ParentProjectId
	      FROM [Project] p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pb.ParentProjectId <> pb.ProjectId
      )
      SELECT DISTINCT pro.*
      FROM pbranch, [Project] pro
      WHERE pbranch.ProjectId = {0}
      AND pbranch.ParentProjectId = pro.ProjectId
    ";
    private const string GetParentProjectIdsByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId
        FROM [Project]
	      UNION ALL
	      SELECT pb.ProjectId, p.ParentProjectId
	      FROM [Project] p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pb.ParentProjectId <> pb.ProjectId
      )
      SELECT DISTINCT pbranch.ParentProjectId
      FROM pbranch
      WHERE pbranch.ProjectId = {0}
    ";
    private const string GetCurrentAndParentProjectsByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM Project
	      WHERE ProjectId = {0}
	      UNION ALL
	      SELECT p.ProjectId, p.ParentProjectId
	      FROM Project p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId
      )
      SELECT DISTINCT pro.*
      FROM pbranch, Project pro
      WHERE pbranch.ProjectId = pro.ProjectId
    ";
    private const string GetCurrentAndParentProjectIdsByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId
	      FROM Project
	      WHERE ProjectId = {0}
	      UNION ALL
	      SELECT p.ProjectId, p.ParentProjectId
	      FROM Project p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId
      )
      SELECT DISTINCT pbranch.ProjectId
      FROM pbranch
    ";
    private const string GetNearestOwnedParentProjectByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId, CAST(ProjectId AS NVARCHAR(MAX)) Path, 1 Distance
	      FROM [Project]
	      WHERE ProjectId = {0}
	      UNION ALL
	      SELECT pb.ProjectId, p.ParentProjectId, pb.Path + N', ' + CAST(pb.ProjectId AS NVARCHAR(MAX)), pb.Distance + 1
	      FROM [Project] p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pb.ParentProjectId <> pb.ProjectId
      )
      SELECT TOP(1) pro.*
      FROM pbranch, [Project] pro
      WHERE pbranch.ParentProjectId = pro.ProjectId
      AND pro.OwnerUserId = {1}
      ORDER BY pbranch.Distance
    ";
    private const string GetFarestOwnedParentProjectIdByIdQuery = @"
      WITH pbranch AS
      (
	      SELECT ProjectId, ParentProjectId, CAST(ProjectId AS NVARCHAR(MAX)) Path, 1 Distance
	      FROM [Project]
	      WHERE ProjectId = {0}
	      UNION ALL
	      SELECT pb.ProjectId, p.ParentProjectId, pb.Path + N', ' + CAST(pb.ProjectId AS NVARCHAR(MAX)), pb.Distance + 1
	      FROM [Project] p
	      JOIN pbranch pb ON pb.ParentProjectId = p.ProjectId AND p.ParentProjectId <> p.ProjectId AND pb.ParentProjectId <> pb.ProjectId
      )
      SELECT TOP(1) pro.*
      FROM pbranch, [Project] pro
      WHERE pbranch.ParentProjectId = pro.ProjectId
      AND pro.OwnerUserId = {1}
      ORDER BY pbranch.Distance DESC
    ";
    #endregion
  }
}
