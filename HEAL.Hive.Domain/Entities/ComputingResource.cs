using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public abstract class ComputingResource : OwnedResource {
    public int HeartbeatInterval { get; set; }
    public Guid? ParentDroneGroupId { get; set; }
    public ICollection<Downtime> Downtimes { get; set; }
    public ICollection<HiveJob> AssignedHiveJobs { get; set; }
    public ICollection<Project> AssignedProjects { get; set; }
  }
}