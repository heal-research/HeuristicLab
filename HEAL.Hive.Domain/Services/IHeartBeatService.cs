using HEAL.Hive.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services {
  public interface IHeartbeatService {

    Task<IEnumerable<MessageContainer>> HeartbeatAsync(Heartbeat heartbeat);

  }
}
