using System;

namespace HEAL.Hive.Domain.Entities {
  public class HiveJobPermission : Permission {
    public Guid HiveJobId { get; set; }
    public Guid GrantedUserId { get; set; }
    public Guid GrantedByUserId { get; set; }
  }
}