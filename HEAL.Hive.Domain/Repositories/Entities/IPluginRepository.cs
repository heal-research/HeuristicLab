using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IPluginRepository : IBaseRepository<Plugin> {

    Task UnassignTasksAsync(Plugin plugin);

  }
}
