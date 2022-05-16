using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IHiveJobRepository : IBaseRepository<HiveJob> {

    Task UnassignComputingResourcesAsync(HiveJob hiveJob);

  }
}
