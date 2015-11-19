#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq.Expressions;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class SlaveDao : IGenericDao<Guid, Slave> {

    private readonly DataContext dataContext;
    private Table<Resource> Table {
      get { return dataContext.GetTable<Resource>(); }
    }

    private IQueryable<Slave> Entities {
      get { return Table.OfType<Slave>(); }
    }

    public SlaveDao(DataContext dataContext) {
      this.dataContext = dataContext;
    }

    #region IGenericDao<Guid,Slave> Members
    public Slave GetById(Guid id) {
      return GetByIdQuery(dataContext, id);
    }

    public IQueryable<Slave> Get(Expression<Func<Slave, bool>> predicate) {
      return Entities.Where(predicate);
    }

    public IQueryable<Slave> GetAll() {
      return Entities;
    }

    public IQueryable<Slave> GetOnlineSlaves() {
      return Entities.Where(x => x.SlaveState != SlaveState.Offline);
    }

    public Slave Save(Slave entity) {
      Table.InsertOnSubmit(entity);
      return entity;
    }

    public IEnumerable<Slave> Save(params Slave[] entities) {
      return entities.Select(Save).ToList();
    }

    public IEnumerable<Slave> Save(IEnumerable<Slave> entities) {
      return entities.Select(Save).ToList();
    }

    public Slave SaveOrAttach(Slave entity) {
      if (Table.Contains(entity)) {
        if (Table.GetOriginalEntityState(entity) == null) {
          Table.Attach(entity);
        }
      } else {
        Table.InsertOnSubmit(entity);
      }
      return entity;
    }

    public IEnumerable<Slave> SaveOrAttach(params Slave[] entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public IEnumerable<Slave> SaveOrAttach(IEnumerable<Slave> entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public void Delete(Guid id) {
      Slave entity = GetById(id);
      if (entity != null) {
        Table.DeleteOnSubmit(entity);
      }
    }

    public void Delete(IEnumerable<Guid> ids) {
      foreach (var id in ids) {
        Delete(id);
      }
    }

    public void Delete(Slave entity) {
      Table.DeleteOnSubmit(entity);
    }

    public void Delete(IEnumerable<Slave> entities) {
      foreach (var entity in entities) {
        Delete(entity);
      }
    }

    public bool Exists(Slave entity) {
      return ExistsQuery(dataContext, entity);
    }

    public bool Exists(Guid id) {
      return GetById(id) != null;
    }
    #endregion

    public bool SlaveHasToShutdownComputer(Guid slaveId) {
      return dataContext.ExecuteQuery<int>(DowntimeQueryString, slaveId, DateTime.Now, DowntimeType.Shutdown.ToString()).FirstOrDefault() > 0;
    }

    public bool SlaveIsAllowedToCalculate(Guid slaveId) {
      return dataContext.ExecuteQuery<int>(DowntimeQueryString, slaveId, DateTime.Now, DowntimeType.Offline.ToString()).FirstOrDefault() == 0;
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Slave> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid slaveId) =>
        (from slave in db.GetTable<Resource>().OfType<Slave>()
         where slave.ResourceId == slaveId
         select slave).SingleOrDefault());

    private static readonly Func<DataContext, Slave, bool> ExistsQuery =
      CompiledQuery.Compile((DataContext db, Slave entity) =>
        db.GetTable<Resource>().OfType<Slave>().Contains(entity));
    #endregion

    #region String queries
    private const string DowntimeQueryString = @"
      WITH pr AS (
        SELECT ResourceId, ParentResourceId
        FROM [Resource]
        WHERE ResourceId = {0}
        UNION ALL
        SELECT r.ResourceId, r.ParentResourceId
        FROM [Resource] r JOIN pr ON r.ResourceId = pr.ParentResourceId
      )
      SELECT COUNT(dt.DowntimeId)
      FROM pr JOIN [Downtime] dt ON pr.ResourceId = dt.ResourceId
      WHERE {1} BETWEEN dt.StartDate AND dt.EndDate
        AND dt.DowntimeType = {2}
    ";
    #endregion
  }
}
