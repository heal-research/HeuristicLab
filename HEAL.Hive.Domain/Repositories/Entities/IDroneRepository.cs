using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IDroneRepository : IBaseRepository<Drone> {

    Task UnassignHiveJobsAsync(Drone drone);
    Task UnassignProjectsAsync(Drone drone);

  }
}
