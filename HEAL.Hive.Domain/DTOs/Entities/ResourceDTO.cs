using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  
  [Serializable]
  public abstract class ResourceDTO {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
  }
}
