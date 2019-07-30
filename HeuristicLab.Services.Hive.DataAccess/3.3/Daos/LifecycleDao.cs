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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class LifecycleDao : GenericDao<int, Lifecycle> {
    public LifecycleDao(DataContext dataContext) : base(dataContext) { }

    public override Lifecycle GetById(int id) {
      return GetByIdQuery(DataContext, id);
    }

    public Lifecycle GetLastLifecycle() {
      return GetLastLifecycleQuery(DataContext);
    }

    public void UpdateLifecycle() {
      var entity = GetLastLifecycle();
      if (entity != null) {
        entity.LastCleanup = DateTime.Now;
      } else {
        Save(new Lifecycle {
          LifecycleId = 0,
          LastCleanup = DateTime.Now
        });
      }
    }

    #region Compiled queries
    private static readonly Func<DataContext, int, Lifecycle> GetByIdQuery =
     CompiledQuery.Compile((DataContext db, int lifecycleId) =>
       (from lifecycle in db.GetTable<Lifecycle>()
        where lifecycle.LifecycleId == lifecycleId
        select lifecycle).SingleOrDefault());

    private static readonly Func<DataContext, Lifecycle> GetLastLifecycleQuery =
      CompiledQuery.Compile((DataContext db) =>
        (from lifecycle in db.GetTable<Lifecycle>()
         select lifecycle).SingleOrDefault());
    #endregion
  }
}
