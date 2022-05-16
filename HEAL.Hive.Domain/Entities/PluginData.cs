using System;

namespace HEAL.Hive.Domain.Entities {
  public class PluginData : Resource {
    public Guid PluginId { get; set; }
    public byte[] Data { get; set; }
    public string FileName { get; set; }
  }
}
