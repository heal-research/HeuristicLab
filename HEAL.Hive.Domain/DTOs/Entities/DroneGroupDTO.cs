using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class DroneGroupDTO : ComputingResourceDTO {
    public ICollection<Guid> ChildComputingResources { get; set; }
  }
}
