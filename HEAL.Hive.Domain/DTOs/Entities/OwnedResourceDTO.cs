using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public abstract class OwnedResourceDTO : ResourceDTO {
    public Guid? OwnerId { get; set; }
    public string Name { get; set; }
  }
}
