#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class JobPermissionDao : GenericDao<Guid, JobPermission> {
    public JobPermissionDao(DataContext dataContext) : base(dataContext) { }

    public override JobPermission GetById(Guid id) {
      throw new NotImplementedException();
    }

    public JobPermission GetByJobAndUserId(Guid jobId, Guid userId) {
      return GetByJobAndUserIdQuery(DataContext, jobId, userId);
    }

    public IQueryable<JobPermission> GetByJobId(Guid jobId) {
      return Table.Where(x => x.JobId == jobId);
    }

    public void SetJobPermission(Guid jobId, Guid grantedByUserId, Guid grantedUserId, Permission permission) {
      var jobPermission = GetByJobAndUserId(jobId, grantedUserId);
      if (jobPermission != null) {
        if (permission == Permission.NotAllowed) {
          Delete(jobPermission);
        } else {
          jobPermission.Permission = permission;
          jobPermission.GrantedByUserId = grantedByUserId;
        }
      } else {
        if (permission != Permission.NotAllowed) {
          Save(new JobPermission {
            JobId = jobId,
            GrantedByUserId = grantedByUserId,
            GrantedUserId = grantedUserId,
            Permission = permission
          });
        }
      }
    }

    #region Compiled queries
    private static readonly Func<DataContext, Guid, Guid, JobPermission> GetByJobAndUserIdQuery =
      CompiledQuery.Compile((DataContext db, Guid jobId, Guid userId) =>
        (from jobPermission in db.GetTable<JobPermission>()
         where jobPermission.JobId == jobId && jobPermission.GrantedUserId == userId
         select jobPermission).SingleOrDefault());
    #endregion
  }
}
