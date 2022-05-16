using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class StateLogDTO : ResourceDTO {
    public HiveTaskState TaskState { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DroneId { get; set; }
    public string Exception { get; set; }
  }
}
