
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class DimHiveJob {

    public Guid HiveJobId { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid ProjectId { get; set; }
    public string JobName { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalHiveTasks { get; set; }
    public int CompletedHiveTasks { get; set; }
    public DateTime? CompletedAt { get; set; }

  }
}
