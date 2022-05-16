using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs {
  public class Heartbeat {
    public Guid DroneId { get; set; }
    public int FreeMemory { get; set; }
    public int FreeCores { get; set; }
    public IDictionary<Guid, TimeSpan> TaskProgress { get; set; }
    public bool AssignJob { get; set; }
    public float CpuUtilization { get; set; }
    public int HeartbeatInterval { get; set; }
  }
}
