using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class PluginDTO : OwnedResourceDTO {
    public Version Version { get; set; }
    public byte[] Hash { get; set; }
    public ICollection<Guid> PluginData { get; set; }
    public ICollection<Guid> RequiringTasks { get; set; }
  }
}
