using HEAL.Hive.Domain.Enums;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class PermissionDTO : ResourceDTO {
    public PermissionType PermissionType { get; set; }
  }
}
