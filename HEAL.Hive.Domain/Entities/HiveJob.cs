using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public class HiveJob : OwnedResource {
    public Guid ProjectId { get; set; }
    public HiveJobState JobState { get; set; }
    public string Description { get; set; }
    public ICollection<HiveTask> HiveTasks { get; set; }
    public ICollection<ComputingResource> AssignedComputingResources { get; set; }
    public ICollection<HiveJobPermission> HiveJobPermissions { get; set; }
  }
}
