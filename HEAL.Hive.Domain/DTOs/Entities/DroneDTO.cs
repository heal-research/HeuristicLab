using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class DroneDTO : ComputingResourceDTO {
    public string OperatingSystem { get; set; }
    public bool IsAllowedToCalculate { get; set; }
    public bool DisposeOnInactivity { get; set; }
    public DroneState DroneState { get; set; }
    public int? CpuSpeed { get; set; }
    public double? CpuUtilization { get; set; }
    public CpuArchitecture CpuArchitecture { get; set; }
    public int? Cores { get; set; }
    public int? FreeCores { get; set; }
    public int? Memory { get; set; }
    public int? FreeMemory { get; set; }
    public DateTime? LastHeartbeatAt { get; set; }
    public DateTime LastLoginAt { get; set; }
  }
}
