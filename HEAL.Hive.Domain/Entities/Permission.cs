using HEAL.Hive.Domain.Enums;

namespace HEAL.Hive.Domain.Entities {
  public abstract class Permission : Resource {
    public PermissionType PermissionType { get; set; }
  }
}