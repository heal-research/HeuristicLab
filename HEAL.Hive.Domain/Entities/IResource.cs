using System;

namespace HEAL.Hive.Domain.Entities {
  public interface IResource {
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime ModifiedAt { get; set; }
    bool IsDeleted { get; set; }
  }
}
