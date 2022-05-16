
using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class FactHiveTask {

    public Guid HiveTaskId { get; set; }
    public long CalculatingTime { get; set; }
    public long WaitingTime { get; set; }
    public long TransferTime { get; set; }
    public int CalculationRuns { get; set; }
    public int Retries { get; set; }
    public int CoresRequired { get; set; }
    public int MemoryRequired { get; set; }
    public int Priority { get; set; }
    public Guid? LastComputingResourceId { get; set; }
    public Guid HiveJobId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public HiveTaskState HiveTaskState { get; set; }
    public string Exception { get; set; }
    public long InitialWaitingTime { get; set; }

  }
}
