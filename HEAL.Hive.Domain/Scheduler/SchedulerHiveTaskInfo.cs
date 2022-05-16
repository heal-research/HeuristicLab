using System;

namespace HEAL.Hive.Domain.Scheduler {
  public class SchedulerHiveTaskInfo {
    public Guid HiveJobId { get; set; }
    public Guid HiveTaskId { get; set; }
    public int Priority { get; set; }
  }
}
