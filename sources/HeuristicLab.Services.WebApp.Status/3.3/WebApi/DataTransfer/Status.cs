using System.Collections.Generic;

namespace HeuristicLab.Services.WebApp.Status.WebApi.DataTransfer {

  public class CoreStatus {
    public int TotalCores { get; set; }
    public int AvailableCores { get; set; }
    public int FreeCores { get; set; }
  }

  public class CpuUtilizationStatus {
    public double TotalCpuUtilization { get; set; }
    public double UsedCpuUtilization { get; set; }
  }

  public class MemoryStatus {
    public int TotalMemory { get; set; }
    public int FreeMemory { get; set; }
  }

  public class TaskStatus {
    public User User { get; set; }
    public int CalculatingTasks { get; set; }
    public int WaitingTasks { get; set; }
  }

  public class SlaveCpuStatus {
    public Slave Slave { get; set; }
    public double CpuUtilization { get; set; }
  }

  public class Status {
    public CoreStatus CoreStatus { get; set; }
    public CpuUtilizationStatus CpuUtilizationStatus { get; set; }
    public MemoryStatus MemoryStatus { get; set; }
    public IEnumerable<TaskStatus> TasksStatus { get; set; }
    public IEnumerable<SlaveCpuStatus> SlavesCpuStatus { get; set; }
    public long Timestamp { get; set; }
  }

}