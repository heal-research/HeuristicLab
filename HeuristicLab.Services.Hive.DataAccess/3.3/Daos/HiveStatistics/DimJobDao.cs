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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {
  public class DimJobDao : GenericDao<Guid, DimJob> {
    public DimJobDao(DataContext dataContext) : base(dataContext) { }

    public override DimJob GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public IQueryable<DimJob> GetByUserId(Guid id) {
      return Table.Where(x => x.UserId == id);
    }

    public IQueryable<DimJob> GetNotCompletedJobs() {
      return Table.Where(x => x.DateCompleted == null);
    }

    public IQueryable<DimJob> GetCompletedJobs() {
      return Table.Where(x => x.DateCompleted != null);
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, DimJob> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid id) =>
        (from dimJob in db.GetTable<DimJob>()
         where dimJob.JobId == id
         select dimJob).SingleOrDefault());
    #endregion
  }
}
