using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public class HiveTask : Resource {
    public HiveTaskState TaskState { get; set; }
    public DateTime LastHeartbeatAt { get; set; }
    public Guid? ParentHiveTaskId { get; set; }
    public ICollection<HiveTask> ChildHiveTasks { get; set; }
    public HiveTaskCommand Command { get; set; }
    public Guid HiveJobId { get; set; }
    public Guid? HiveTaskDataId { get; set; }
    public HiveTaskData HiveTaskData { get; set; }
    public double ExecutionTime { get; set; }
    public int Priority { get; set; }
    public int CoresNeeded { get; set; }
    public int MemoryNeeded { get; set; }
    public bool FinishWhenChildJobsFinished { get; set; }
    public ICollection<Plugin> RequiredPlugins { get; set; }
  }
}
