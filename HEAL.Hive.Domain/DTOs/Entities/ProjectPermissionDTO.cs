using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class ProjectPermissionDTO : PermissionDTO {
    public Guid? GrantedUserId { get; set; }
    public Guid? GrantedRoleId { get; set; }
    public Guid GrantedByUserId { get; set; }
    public Guid ProjectId { get; set; }
  }
}
