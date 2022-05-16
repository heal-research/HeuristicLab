using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  
  [Serializable]
  public class HiveTaskDataDTO : ResourceDTO {
    public Guid HiveTaskId { get; set; }
    public byte[] Data { get; set; }
  }
}
