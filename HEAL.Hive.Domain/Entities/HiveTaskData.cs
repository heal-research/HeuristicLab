using System;

namespace HEAL.Hive.Domain.Entities {
  public class HiveTaskData : Resource {
    public Guid HiveTaskId { get; set; }
    public byte[] Data { get; set; }
  }
}
