using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class HiveJobDTO : OwnedResourceDTO {
    public Guid ProjectId { get; set; }
    public HiveJobState HiveJobState { get; set; }
    public string Description { get; set; }
    public ICollection<Guid> HiveTasks { get; set; }
    public ICollection<Guid> AssignedComputingResources { get; set; }
    public ICollection<Guid> HiveJobPermissions { get; set; }
  }
}
