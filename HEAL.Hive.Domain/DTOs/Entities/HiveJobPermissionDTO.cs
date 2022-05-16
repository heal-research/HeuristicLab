using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class HiveJobPermissionDTO : PermissionDTO {
    public Guid JobId { get; set; }
    public Guid GrantedUserId { get; set; }
    public Guid GrantedByUserId { get; set; }
  }
}
