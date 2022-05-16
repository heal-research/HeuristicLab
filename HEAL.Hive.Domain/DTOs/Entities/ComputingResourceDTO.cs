using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public abstract class ComputingResourceDTO : OwnedResourceDTO {
    public int HeartbeatInterval { get; set; }
    public Guid? ParentDroneGroupId { get; set; }
    public IEnumerable<Guid> Downtimes { get; set; }
    public ICollection<Guid> AssignedHiveJobs { get; set; }
    public ICollection<Guid> AssignedProjects { get; set; }
  }
}
