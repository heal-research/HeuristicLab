#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class ResourceDao : GenericDao<Guid, Resource> {
    public ResourceDao(DataContext dataContext) : base(dataContext) { }

    public override Resource GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public Resource GetByName(string name) {
      return GetByNameQuery(DataContext, name);
    }

    public void DeleteByIds(IEnumerable<Guid> ids) {
      string paramResourceIds = string.Join(",", ids.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(DeleteByIdsQueryString, paramResourceIds);
        DataContext.ExecuteCommand(queryString);
      }
    }

    public bool CheckExistence(IEnumerable<Guid> ids) {
      string paramResourceIds = string.Join(",", ids.ToList().Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramResourceIds)) {
        string queryString = string.Format(CountExistenceQuery, paramResourceIds);
        return DataContext.ExecuteQuery<int>(queryString).SingleOrDefault() == ids.Count();
      }
      return false;
    }

    public IQueryable<Resource> GetResourcesWithValidOwner() {
      return Table.Where(x => x.OwnerUserId != null);
    }

    public IEnumerable<Resource> GetChildResourcesById(Guid id) {
      return DataContext.ExecuteQuery<Resource>(GetChildResourcesByIdQuery, id);
    }

    public IEnumerable<Guid> GetChildResourceIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetChildResourceIdsByIdQuery, id);
    }

    public IEnumerable<Resource> GetParentResourcesById(Guid id) {
      return DataContext.ExecuteQuery<Resource>(GetParentResourcesByIdQuery, id);
    }

    public IEnumerable<Guid> GetParentResourceIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetParentResourceIdsByIdQuery, id);
    }

    public IEnumerable<Resource> GetCurrentAndParentResourcesById(Guid id) {
      return DataContext.ExecuteQuery<Resource>(GetCurrentAndParentResourcesByIdQuery, id);
    }

    public IEnumerable<Guid> GetCurrentAndParentResourceIdsById(Guid id) {
      return DataContext.ExecuteQuery<Guid>(GetCurrentAndParentResourceIdsByIdQuery, id);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Resource> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid resourceId) =>
        (from resource in db.GetTable<Resource>()
         where resource.ResourceId == resourceId
         select resource).SingleOrDefault());

    private static readonly Func<DataContext, string, Resource> GetByNameQuery =
     CompiledQuery.Compile((DataContext db, string name) =>
       (from resource in db.GetTable<Resource>()
        where resource.Name == name
        select resource).FirstOrDefault());
    #endregion

    #region String queries
    private const string DeleteByIdsQueryString = @"
      DELETE FROM [Resource]
      WHERE ResourceId IN ({0})
    ";
    private const string CountExistenceQuery = @"
      SELECT COUNT(DISTINCT r.ResourceId)
      FROM [Resource] r
      WHERE r.ResourceId IN ({0})
    ";
    private const string GetChildResourcesByIdQuery = @"
    	WITH rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId AND r.ParentResourceId <> r.ResourceId AND rt.ParentResourceId <> rt.ResourceId
      )
      SELECT DISTINCT res.*
      FROM rtree, [Resource] res
      WHERE rtree.ParentResourceId = {0}
	    AND rtree.ResourceId = res.ResourceId
    ";
    private const string GetChildResourceIdsByIdQuery = @"
      WITH rtree AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rt.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rtree rt ON rt.ParentResourceId = r.ResourceId AND r.ParentResourceId <> r.ResourceId AND rt.ParentResourceId <> rt.ResourceId
      )
      SELECT DISTINCT rtree.ResourceId
      FROM rtree
      WHERE rtree.ParentResourceId = {0}
    ";
    private const string GetParentResourcesByIdQuery = @"
      WITH rbranch AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rb.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId AND r.ParentResourceId <> r.ResourceId AND rb.ParentResourceId <> rb.ResourceId
      )
      SELECT DISTINCT res.*
      FROM rbranch, [Resource] res
      WHERE rbranch.ResourceId = {0}
      AND rbranch.ParentResourceId = res.ResourceId
    ";
    private const string GetParentResourceIdsByIdQuery = @"
      WITH rbranch AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      UNION ALL
	      SELECT rb.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId AND r.ParentResourceId <> r.ResourceId AND rb.ParentResourceId <> rb.ResourceId
      )
      SELECT DISTINCT rbranch.ParentResourceId
      FROM rbranch
      WHERE rbranch.ResourceId = {0}
    ";
    private const string GetCurrentAndParentResourcesByIdQuery = @"
      WITH rbranch AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      WHERE ResourceId = {0}
	      UNION ALL
	      SELECT r.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
      )
      SELECT DISTINCT res.*
      FROM rbranch, [Resource] res
      WHERE rbranch.ResourceId = res.ResourceId
    ";
    private const string GetCurrentAndParentResourceIdsByIdQuery = @"
      WITH rbranch AS
      (
	      SELECT ResourceId, ParentResourceId
	      FROM [Resource]
	      WHERE ResourceId = {0}
	      UNION ALL
	      SELECT r.ResourceId, r.ParentResourceId
	      FROM [Resource] r
	      JOIN rbranch rb ON rb.ParentResourceId = r.ResourceId
      )
      SELECT DISTINCT rbranch.ResourceId
      FROM rbranch
    ";
    #endregion
  }
}
