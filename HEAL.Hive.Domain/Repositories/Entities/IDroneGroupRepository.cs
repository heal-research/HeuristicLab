using HEAL.Hive.Domain.Entities;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Repositories.Entities {
  public interface IDroneGroupRepository : IBaseRepository<DroneGroup> {

    Task UnassignHiveJobsAsync(DroneGroup droneGroup);
    Task UnassignProjectsAsync(DroneGroup droneGroup);

  }
}
