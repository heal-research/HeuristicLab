using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public class Plugin : OwnedResource {
    public Version Version { get; set; }
    public byte[] Hash { get; set; }
    public ICollection<PluginData> PluginData { get; set; }
    public ICollection<HiveTask> RequiringTasks { get; set; }
  }
}
