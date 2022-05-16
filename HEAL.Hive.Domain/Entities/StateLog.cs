using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.Entities {
  public class StateLog : Resource {
    public HiveTaskState HiveTaskState { get; set; }
    public Guid? HiveTaskId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DroneId { get; set; }
    public string Exception { get; set; }
  }
}
