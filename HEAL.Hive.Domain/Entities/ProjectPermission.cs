using System;

namespace HEAL.Hive.Domain.Entities {
  public class ProjectPermission : Permission {
    public Guid ProjectId { get; set; }
    public Guid? GrantedUserId { get; set; }
    public Guid? GrantedRoleId { get; set; }
    public Guid GrantedByUserId { get; set; }
  }
}