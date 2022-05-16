using HEAL.Hive.Domain.Entities;
using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IHiveJobService : IBaseService<HiveJob> {

    Task<IEnumerable<HiveJob>> GetHiveJobsOfProjectAsync(Guid projectId);
    Task UpdateHiveJobStateAsync(Guid jobId, HiveJobState jobState);
    Task<IEnumerable<HiveJob>> GetByStateAsync(HiveJobState state, bool resolveDependencies);

  }
}
