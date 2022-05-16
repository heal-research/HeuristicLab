using System;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class PluginDataDTO : ResourceDTO {
    public Guid PluginId { get; set; }
    public byte[] Data { get; set; }
    public string FileName { get; set; }
  }
}
