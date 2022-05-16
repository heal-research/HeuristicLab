using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {

  [Serializable]
  public class HiveTaskDTO : ResourceDTO {
    public HiveTaskState TaskState { get; set; }
    public double ExecutionTime { get; set; }
    public DateTime LastHeartbeatAt { get; set; }
    public int Priority { get; set; }
    public int CoresNeeded { get; set; }
    public int MemoryNeeded { get; set; }
    public Guid? ParentTaskId { get; set; }
    public IEnumerable<Guid> ChildHiveTasks { get; set; }
    public bool FinishWhenChildJobsFinished { get; set; }
    public HiveTaskCommand Command { get; set; }
    public Guid HiveJobId { get; set; }
    public Guid? TaskDataId { get; set; }
    public ICollection<Guid> RequiredPlugins { get; set; }
  }
}
