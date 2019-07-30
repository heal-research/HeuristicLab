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
using System.Linq;
using System.Linq.Expressions;

namespace HeuristicLab.Services.Hive.DataAccess.Interfaces {
  public interface IGenericDao<in PK, T> where T : class {
    T GetById(PK id);
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetAll();
    T Save(T entity);
    IEnumerable<T> Save(params T[] entities);
    IEnumerable<T> Save(IEnumerable<T> entities);
    T SaveOrAttach(T entity);
    IEnumerable<T> SaveOrAttach(params T[] entities);
    IEnumerable<T> SaveOrAttach(IEnumerable<T> entities);
    void Delete(PK id);
    void Delete(IEnumerable<PK> ids);
    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
    bool Exists(T entity);
    bool Exists(PK id);
  }
}
