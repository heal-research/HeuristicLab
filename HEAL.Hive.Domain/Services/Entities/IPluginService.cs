using HEAL.Hive.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IPluginService : IBaseService<Plugin> {

    Task<IEnumerable<Plugin>> GetValidPluginsAsync(bool resolveDependencies);

  }
}
