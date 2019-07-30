#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {
  public class DimProjectDao : GenericDao<Guid, DimProject> {
    public DimProjectDao(DataContext dataContext) : base(dataContext) { }

    public override DimProject GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IEnumerable<DimProject> GetByProjectId(Guid projectId) {
      return GetByProjectIdQuery(DataContext, projectId);
    }

    public Guid GetLastValidIdByProjectId(Guid projectId) {
      return GetLastValidIdByProjectIdQuery(DataContext, projectId);
    }

    public IEnumerable<DimProject> GetAllOnlineProjects() {
      return GetAllOnlineProjectsQuery(DataContext);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, DimProject> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid id) =>
        (from dimProject in db.GetTable<DimProject>()
         where dimProject.Id == id
         select dimProject).SingleOrDefault());
    private static readonly Func<DataContext, Guid, IEnumerable<DimProject>> GetByProjectIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        (from dimProject in db.GetTable<DimProject>()
         where dimProject.ProjectId == projectId
         select dimProject));
    private static readonly Func<DataContext, Guid, Guid> GetLastValidIdByProjectIdQuery =
      CompiledQuery.Compile((DataContext db, Guid projectId) =>
        (from dimProject in db.GetTable<DimProject>()
         where dimProject.ProjectId == projectId
         && dimProject.DateExpired == null
         orderby dimProject.DateCreated descending
         select dimProject.Id).SingleOrDefault());
    private static readonly Func<DataContext, IEnumerable<DimProject>> GetAllOnlineProjectsQuery =
      CompiledQuery.Compile((DataContext db) =>
        (from dimProject in db.GetTable<DimProject>()
         where dimProject.DateExpired == null
         select dimProject));
    #endregion
  }
}
