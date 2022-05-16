using HEAL.Hive.Domain.Entities;
using HEAL.Hive.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IHiveTaskService : IBaseService<HiveTask> {

    Task<IEnumerable<HiveTask>> GetHiveTasksOfJobAsync(Guid hiveJobId);
    Task<HiveTask> UpdateHiveTaskState(Guid id, HiveTaskState taskState, Guid? droneId, Guid? userId, string exception);
    Task StopHiveTaskAsync(Guid id);
    Task PauseHiveTaskAsync(Guid id);
    Task RestartHiveTaskAsync(Guid id);
    Task<IEnumerable<HiveTask>> GetTimeoutHiveTasksAsync(TimeSpan calculatingTreshold, TimeSpan transferringTreshold, bool resolveDependencies);
    Task<IEnumerable<HiveTask>> GetObsoleteHiveTasksAsync(bool resolveDependencies);
    Task<IEnumerable<HiveTask>> GetFinishedParentHiveTasksAsync(bool resolveDependencies);

  }

}
