using Microsoft.AspNetCore.Identity;
using System;

namespace HEAL.Hive.Domain.Entities.Identity {
  public class HiveRole : IdentityRole<Guid>, IResource {
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
  }
}
