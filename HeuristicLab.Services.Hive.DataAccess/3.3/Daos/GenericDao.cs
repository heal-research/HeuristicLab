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
  public abstract class GenericDao<PK, T> : IGenericDao<PK, T> where T : class {
    private readonly DataContext dataContext;
    protected DataContext DataContext {
      get { return dataContext; }
    }

    protected Table<T> Table {
      get { return dataContext.GetTable<T>(); }
    }

    protected GenericDao(DataContext dataContext) {
      this.dataContext = dataContext;
    }

    #region IGenericDao<PK,T> Members
    public abstract T GetById(PK id);

    public IQueryable<T> Get(Expression<Func<T, bool>> predicate) {
      return Table.Where(predicate);
    }

    public IQueryable<T> GetAll() {
      return Table;
    }

    public T Save(T entity) {
      Table.InsertOnSubmit(entity);
      return entity;
    }

    public IEnumerable<T> Save(params T[] entities) {
      return entities.Select(Save).ToList();
    }

    public IEnumerable<T> Save(IEnumerable<T> entities) {
      return entities.Select(Save).ToList();
    }

    public T SaveOrAttach(T entity) {
      if (Table.Contains(entity)) {
        if (Table.GetOriginalEntityState(entity) == null) {
          Table.Attach(entity);
        }
      } else {
        Table.InsertOnSubmit(entity);
      }
      return entity;
    }

    public IEnumerable<T> SaveOrAttach(params T[] entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public IEnumerable<T> SaveOrAttach(IEnumerable<T> entities) {
      return entities.Select(SaveOrAttach).ToList();
    }

    public void Delete(PK id) {
      T entity = GetById(id);
      if (entity != null) {
        Table.DeleteOnSubmit(entity);
      }
    }

    public virtual void Delete(IEnumerable<PK> ids) {
      foreach (var id in ids) {
        Delete(id);
      }
    }

    public void Delete(T entity) {
      Table.DeleteOnSubmit(entity);
    }

    public void Delete(IEnumerable<T> entities) {
      foreach (var entity in entities) {
        Delete(entity);
      }
    }

    public bool Exists(T entity) {
      return ExistsQuery(DataContext, entity);
    }

    public bool Exists(PK id) {
      return GetById(id) != null;
    }
    #endregion

    #region Compiled queries
    private static readonly Func<DataContext, T, bool> ExistsQuery =
      CompiledQuery.Compile((DataContext db, T entity) =>
        db.GetTable<T>().Contains(entity));
    #endregion
  }
}
