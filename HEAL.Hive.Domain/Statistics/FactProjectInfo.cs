
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class FactProjectInfo {

    public Guid ProjectId { get; set; }
    public DateTime Time { get; set; }
    public int TotalCores { get; set; }
    public int UsedCores { get; set; }
    public int TotalMemory { get; set; }
    public int UsedMemory { get; set; }

  }
}
