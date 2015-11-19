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
using System.Data.Linq;
using System.Linq;

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {
  public class DimTimeDao : GenericDao<DateTime, DimTime> {
    public DimTimeDao(DataContext dataContext) : base(dataContext) { }

    public override DimTime GetById(DateTime id) {
      return GetByIdQuery(DataContext, id);
    }

    public DimTime GetLastEntry() {
      return GetLastEntryQuery(DataContext);
    }

    public int DeleteUnusedTimes() {
      return DataContext.ExecuteCommand(DeleteUnusedTimeEntriesStringQuery);
    }

    #region Compiled queries
    private static readonly Func<DataContext, DateTime, DimTime> GetByIdQuery =
      CompiledQuery.Compile((DataContext db, DateTime id) =>
        (from dimTime in db.GetTable<DimTime>()
         where dimTime.Time == id
         select dimTime).SingleOrDefault());

    private static readonly Func<DataContext, DimTime> GetLastEntryQuery =
      CompiledQuery.Compile((DataContext db) =>
        (from dimTime in db.GetTable<DimTime>()
         orderby dimTime.Time descending
         select dimTime).FirstOrDefault());
    #endregion

    #region String queries
    private const string DeleteUnusedTimeEntriesStringQuery = @"
      DELETE FROM [statistics].[DimTime]
      WHERE NOT EXISTS (SELECT [Time] FROM [statistics].[FactClientInfo] fci
                        WHERE fci.[Time] = [statistics].[DimTime].[Time]);    
    ";
    #endregion
  }
}
