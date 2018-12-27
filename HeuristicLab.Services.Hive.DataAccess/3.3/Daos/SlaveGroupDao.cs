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
using System.Linq.Expressions;
using HeuristicLab.Services.Hive.DataAccess.Interfaces;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class SlaveGroupDao : IGenericDao<Guid, SlaveGroup> {
    private readonly DataContext dataContext;
    private Table<Resource> Table {
      get { return dataContext.GetTable<Resource>(); }
    }

    private IQueryable<SlaveGroup> Entities {
      get { return Table.OfType<SlaveGroup>(); }
    }

    public SlaveGroupDao(DataContext dataContext) {
      this.dataContext = dataContext;
    }

    #region IGenericDao<Guid,SlaveGroup> Members
    public SlaveGroup GetById(Guid id) {
      return GetByIdQuery(dataContext, id);
    }

    public IQueryable<SlaveGroup> Get(Expression<Func<SlaveGroup, bool>> predicate) {
      return Entities.Where(predicate);
    }

    public IQueryable<SlaveGroup> GetAll() {
      return Entities;
    }

    public SlaveGroup Save(SlaveGroup entity) {
      Table.InsertOnSubmit(entity);
      return entity;
    }

    public IEnumerable<SlaveGroup> Save(params SlaveGroup[] entities) {
      return entities.Select(Save).ToList();
    }

    public IEnumerable<SlaveGroup> Save(IEnumerable<SlaveGroup> entities) {
      return entities.Select(Save).ToList();
    }

    public SlaveGroup SaveOrAttach(SlaveGroup entity) {
      if (Table.Contains(entity)) {
        if (Table.GetOriginalEntityState(entity) == null) {
          Table.Attach(entity);
        }
      } else {
        Table.InsertOnSubmit(entity);
      }
      return entity;
    }

    public IEnumerable<SlaveGroup> SaveOrAttach(params SlaveGroup[] entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public IEnumerable<SlaveGroup> SaveOrAttach(IEnumerable<SlaveGroup> entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public void Delete(Guid id) {
      SlaveGroup entity = GetById(id);
      if (entity != null) {
        Table.DeleteOnSubmit(entity);
      }
    }

    public void Delete(IEnumerable<Guid> ids) {
      foreach (var id in ids) {
        Delete(id);
      }
    }

    public void Delete(SlaveGroup entity) {
      Table.DeleteOnSubmit(entity);
    }

    public void Delete(IEnumerable<SlaveGroup> entities) {
      foreach (var entity in entities) {
        Delete(entity);
      }
    }

    public bool Exists(SlaveGroup entity) {
      return ExistsQuery(dataContext, entity);
    }

    public bool Exists(Guid id) {
      return GetById(id) != null;
    }
    #endregion

    #region Compiled queries
    private static readonly Func<DataContext, Guid, SlaveGroup> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid slaveGroupId) =>
        (from slaveGroup in db.GetTable<Resource>().OfType<SlaveGroup>()
         where slaveGroup.ResourceId == slaveGroupId
         select slaveGroup).SingleOrDefault());

    private static readonly Func<DataContext, SlaveGroup, bool> ExistsQuery =
      CompiledQuery.Compile((DataContext db, SlaveGroup slaveGroup) =>
        db.GetTable<Resource>().OfType<SlaveGroup>().Contains(slaveGroup));
    #endregion
  }
}
