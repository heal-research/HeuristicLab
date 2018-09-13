using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class AssignedJobResourceDao : GenericDao<Guid, AssignedJobResource> {
    public AssignedJobResourceDao(DataContext dataContext) : base(dataContext) { }

    public override AssignedJobResource GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IQueryable<AssignedJobResource> GetByJobId(Guid jobId) {
      return Table.Where(x => x.JobId == jobId);
    }

    public void DeleteByProjectId(Guid projectId) {
      DataContext.ExecuteCommand(DeleteByProjectIdQueryString, projectId);
    }

    public void DeleteByProjectIdAndUserIds(Guid projectId, IEnumerable<Guid> userIds) {
      string paramUserIds = string.Join(",", userIds.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramUserIds)) {
        string queryString = string.Format(DeleteByProjectIdAndUserIdsQueryString, projectId, paramUserIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public void DeleteByProjectIds(IEnumerable<Guid> projectIds) {
      string paramProjectIds = string.Join(",", projectIds.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds)) {
        string queryString = string.Format(DeleteByProjectIdsQueryString, paramProjectIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public void DeleteByProjectIdsAndUserIds(IEnumerable<Guid> projectIds, IEnumerable<Guid> userIds) {
      string paramProjectIds = string.Join(",", projectIds.Select(x => string.Format("'{0}'", x)));
      string paramUserIds = string.Join(",", userIds.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds) && !string.IsNullOrWhiteSpace(paramUserIds)) {
        string queryString = string.Format(DeleteByProjectIdsAndUserIdsQueryString, paramProjectIds, paramUserIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public void DeleteByProjectIdAndResourceIds(Guid projectId, IEnumerable<Guid> resourceIds) {
      string paramResourceIds = string.Join(",", resourceIds.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(DeleteByProjectIdAndResourceIdsQueryString, projectId, paramResourceIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public void DeleteByProjectIdsAndResourceIds(IEnumerable<Guid> projectIds, IEnumerable<Guid> resourceIds) {
      string paramProjectIds = string.Join(",", projectIds.Select(x => string.Format("'{0}'", x)));
      string paramResourceIds = string.Join(",", resourceIds.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramProjectIds) && !string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(DeleteByProjectIdsAndResourceIdsQueryString, paramProjectIds, paramResourceIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public bool CheckJobGrantedForResource(Guid jobId, Guid resourceId) {
      return DataContext.ExecuteQuery<int>(CheckJobGrantedForResourceQueryString, jobId, resourceId).First() > 0;
    }

    public bool CheckJobGrantedForResources(Guid jobId, IEnumerable<Guid> resourceIds) {
      string paramResourceIds = string.Join(",", resourceIds.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(CheckJobGrantedForResourcesQueryString, jobId, paramResourceIds);
        return DataContext.ExecuteQuery<int>(queryString).Count() == 0;
      }
      return false;
    }

    public bool CheckTaskGrantedForResource(Guid taskId, Guid resourceId) {
      return DataContext.ExecuteQuery<int>(CheckTaskGrantedForResourceQueryString, taskId, resourceId).First() > 0;
    }

    public IEnumerable<Resource> GetAllGrantedResourcesByJobId(Guid jobId) {
      return DataContext.ExecuteQuery<Resource>(GetAllGrantedResourcesByJobIdQueryString, jobId);
    }

    public IEnumerable<Guid> GetAllGrantedResourceIdsByJobId(Guid jobId) {
      return DataContext.ExecuteQuery<Guid>(GetAllGrantedResourceIdsByJobIdQueryString, jobId);
    }

    #region String queries
    private const string DeleteByProjectIdQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId = {0}
	      )
    ";
    private const string DeleteByProjectIdAndUserIdsQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId = '{0}'
          AND j.OwnerUserId IN ({1})
	      )
    ";
    private const string DeleteByProjectIdsQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId IN ({0})
	      )
    ";
    private const string DeleteByProjectIdsAndUserIdsQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId IN ({0})
          AND j.OwnerUserId IN ({1})
	      )
    ";
    private const string DeleteByProjectIdAndResourceIdsQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId = '{0}'
	      )
      AND ResourceId IN ({1})
    ";
    private const string DeleteByProjectIdsAndResourceIdsQueryString = @"
      DELETE FROM [AssignedJobResource]
      WHERE JobId IN
	      (
		      SELECT j.JobId
		      FROM [Job] j
		      WHERE j.ProjectId IN ({0})
	      )
      AND ResourceId IN ({1})
    ";
    private const string CheckJobGrantedForResourceQueryString = @"
      WITH rbranch AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {1}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r
        JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
      )
      SELECT COUNT(ajr.JobId)
      FROM rbranch, [AssignedJobResource] ajr
      WHERE rbranch.ResourceId = ajr.ResourceId
      AND ajr.JobId = {0}
    ";
    private const string CheckTaskGrantedForResourceQueryString = @"
      WITH rbranch AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {1}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r
        JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
      )
      SELECT COUNT(ajr.JobId)
      FROM rbranch, [AssignedJobResource] ajr, [Task] t
      WHERE rbranch.ResourceId = ajr.ResourceId
      AND ajr.JobId = t.JobId
      AND t.JobId = {0}
    ";
    private const string CheckJobGrantedForResourcesQueryString = @"
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
	      FROM rtree, [AssignedJobResource] ajr
	      WHERE rtree.ParentResourceId = ajr.ResourceId
	      AND ajr.JobId = {0}
	      UNION
	      SELECT ajr.ResourceId
	      FROM [AssignedJobResource] ajr
	      WHERE ajr.JobId = {0}
      )
    ";
    private const string GetAllGrantedResourcesByJobIdQueryString = @"
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
      FROM rtree, [AssignedJobResource] ajr, [Resource] res
      WHERE rtree.ParentResourceId = ajr.ResourceId
      AND rtree.ResourceId = res.ResourceId
      AND ajr.JobId = {0}
      UNION
      SELECT res.*
      FROM [AssignedJobResource] ajr, [Resource] res
      WHERE ajr.ResourceId = res.ResourceId
      AND ajr.JobId = {0}
    ";
    private const string GetAllGrantedResourceIdsByJobIdQueryString = @"
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
    FROM rtree, [AssignedJobResource] ajr
    WHERE rtree.ParentResourceId = ajr.ResourceId
    AND ajr.JobId = {0}
    UNION
    SELECT ajr.ResourceId
    FROM [AssignedJobResource] ajr
    WHERE ajr.JobId = {0}
    ";
    #endregion
  }
}
