using HEAL.Hive.Domain.Entities;
using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IProjectPermissionService : IBaseService<ProjectPermission> {

    Task GrantPermissionToUserAsync(Guid projectId, Guid userId, PermissionType permissionType);
    Task GrantPermissionToRoleAsync(Guid projectId, Guid roleId, PermissionType permissionType);
    Task RevokePermissionFromUserAsync(Guid projectId, Guid userId);
    Task RevokePermissionFromRoleAsync(Guid projectId, Guid roleId);
    Task<IEnumerable<ProjectPermission>> GetPermissionsOfProjectAsync(Guid projectId);
    Task<IEnumerable<ProjectPermission>> GetPermissionsOfUserAsync(Guid userId);
    Task<IEnumerable<ProjectPermission>> GetPermissionsOfRoleAsync(Guid roleId);

  }
}
