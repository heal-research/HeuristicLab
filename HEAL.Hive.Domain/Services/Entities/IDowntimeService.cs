using HEAL.Hive.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IDowntimeService : IBaseService<Downtime> {

    Task<IEnumerable<Downtime>> GetDowntimesForComputingResourceAsync(Guid resourceId);

  }
}
