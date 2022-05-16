using System;

namespace HEAL.Hive.Domain.Scheduler {
  public class UserPriority {
    public Guid UserId { get; set; }
    public DateTime EnqueuedAt { get; set; }
  }
}
