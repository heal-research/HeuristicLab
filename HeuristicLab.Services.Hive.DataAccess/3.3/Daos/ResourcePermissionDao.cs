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

namespace HeuristicLab.Services.Hive.DataAccess.Daos {
  public class ResourcePermissionDao : GenericDao<Guid, ResourcePermission> {
    public ResourcePermissionDao(DataContext dataContext) : base(dataContext) { }

    public override ResourcePermission GetById(Guid id) {
      throw new NotImplementedException();
    }

    public IEnumerable<ResourcePermission> GetByResourceId(Guid id) {
      return GetByResourceIdGetByIdQuery(DataContext, id);
    }

    public void DeleteByResourceAndGrantedUserId(Guid resourceId, IEnumerable<Guid> grantedUserId) {
      string paramIds = string.Join(",", grantedUserId.Select(x => string.Format("'{0}'", x)));
      if (!string.IsNullOrWhiteSpace(paramIds)) {
        string query = string.Format(DeleteByGrantedUserQuery, resourceId, paramIds);
        DataContext.ExecuteCommand(query);
      }
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, IEnumerable<ResourcePermission>> GetByResourceIdGetByIdQuery =
      CompiledQuery.Compile((DataContext db, Guid resourceId) =>
        from resourcePermission in db.GetTable<ResourcePermission>()
        where resourcePermission.ResourceId == resourceId
        select resourcePermission);
    #endregion

    #region String queries
    private const string DeleteByGrantedUserQuery =
      @"DELETE FROM [ResourcePermission]
         WHERE ResourceId = '{0}'
           AND GrantedUserId IN ({1});";
    #endregion
  }
}
