using System;

namespace HEAL.Hive.Domain.Entities {
  public abstract class Resource : IResource {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
  }
}
