using System;

namespace HEAL.Hive.Domain.Entities {
  public abstract class OwnedResource : Resource {
    public Guid? OwnerId { get; set; }
    public string Name { get; set; }
  }
}
