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

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class ResourceDao : GenericDao<Guid, Resource> {
    public ResourceDao(DataContext dataContext) : base(dataContext) { }

    public override Resource GetById(Guid id) {
      return GetByIdQuery(DataContext, id);
    }

    public Resource GetByName(string name) {
      return GetByNameQuery(DataContext, name);
    }

    public IQueryable<Resource> GetResourcesWithValidOwner() {
      return Table.Where(x => x.OwnerUserId != null);
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
  }
}
