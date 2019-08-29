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

namespace HeuristicLab.Services.Hive.DataAccess.Daos.HiveStatistics {

  public class LastUpdateTimestamp {
    public Guid ResourceId { get; set; }
    public DateTime? Timestamp { get; set; }
  }

  public class FactClientInfoDao : GenericDao<Guid, FactClientInfo> {
    private Table<DimClient> DimClientTable {
      get { return DataContext.GetTable<DimClient>(); }
    }

    private Table<DimUser> DimUserTable {
      get { return DataContext.GetTable<DimUser>(); }
    }

    private Table<DimTime> DimTimeTable {
      get { return DataContext.GetTable<DimTime>(); }
    }

    public FactClientInfoDao(DataContext dataContext) : base(dataContext) { }

    public override FactClientInfo GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IQueryable<LastUpdateTimestamp> GetLastUpdateTimestamps() {
      return from cf in Table
             join r in DimClientTable on cf.ClientId equals r.Id
             group cf by r.ResourceId
               into grpFacts
               select new LastUpdateTimestamp {
                 ResourceId = grpFacts.Key,
                 Timestamp = grpFacts.Max(x => x.Time),
               };
    }

    public IQueryable<FactClientInfo> GetByClientId(Guid id) {
      return Table.Where(x => x.ClientId == id);
    }
  }
}
