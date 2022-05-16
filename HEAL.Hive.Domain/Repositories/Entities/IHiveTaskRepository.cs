using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IHiveTaskRepository : IBaseRepository<HiveTask> {

    Task UnassignPluginsAsync(HiveTask hiveTask);

  }
}
