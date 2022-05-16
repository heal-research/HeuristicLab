
using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class FactComputingResourceInfo {

    public Guid ComputingResourceId { get; set; }
    public DateTime Time { get; set; }
    public Guid UserId { get; set; }
    public int UsedCores { get; set; }
    public int TotalCores { get; set; }
    public int UsedMemory { get; set; }
    public int TotalMemory { get; set; }
    public double CpuUtilization { get; set; }
    public DroneState DroneState { get; set; }
    public int IdleTime { get; set; }
    public int OfflineTime { get; set; }
    public int UnavailableTime { get; set; }
    public bool IsAllowedToCalculate { get; set; }

  }
}
