using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public class DroneGroup : ComputingResource {
    public ICollection<ComputingResource> ChildComputingResources { get; set; }
  }
}