using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.DTOs {
  public class HiveTaskUpdateDTO {
    public Guid HiveTaskId { get; set; }
    public HiveTaskState TaskState { get; set; }
    public Guid? DroneId { get; set; }
    public Guid? UserId { get; set; }
    public string Exception { get; set; }

  }
}
