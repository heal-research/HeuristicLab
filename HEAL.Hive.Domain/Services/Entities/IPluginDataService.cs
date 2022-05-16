using HEAL.Hive.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IPluginDataService : IBaseService<PluginData> {

    Task<IEnumerable<PluginData>> GetPluginDataOfPluginAsync(Guid pluginId);
    Task<IEnumerable<PluginData>> GetPluginDataOfPluginsAsync(Guid[] pluginIds);

  }
}
