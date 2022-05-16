using HEAL.Hive.Domain.Entities;
using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IHiveJobPermissionService : IBaseService<HiveJobPermission> {

    Task GrantPermissionAsync(Guid hiveJobId, Guid grantedUserId, PermissionType permissionType);
    Task RevokePermissionAsync(Guid hiveJobId, Guid grantedUserId);
    Task<IEnumerable<HiveJobPermission>> GetPermissionsOfJobAsync(Guid jobId);
    Task<IEnumerable<HiveJobPermission>> GetPermissionsOfUserAsync(Guid userId);

  }
}
