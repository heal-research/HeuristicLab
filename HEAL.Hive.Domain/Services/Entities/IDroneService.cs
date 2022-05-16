using HEAL.Hive.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IDroneService : IBaseService<Drone> {

    Task HelloAsync(Drone droneInfo);
    Task GoodByeAsync(Guid droneId);
    Task<IEnumerable<Drone>> GetOnlineDronesAsync(bool resolveDependencies);
    Task<IEnumerable<Drone>> GetObsoleteDronesAsync(TimeSpan threshold, bool resolveDependencies);
    Task<int> GetHeartbeatIntervalAsync(Guid droneId);

  }
}
