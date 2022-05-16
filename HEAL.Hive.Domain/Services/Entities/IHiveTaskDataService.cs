using HEAL.Hive.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Entities {
  public interface IHiveTaskDataService : IBaseService<HiveTaskData> {

    Task<HiveTaskData> GetTaskDataOfHiveTaskAsync(Guid taskId);
    Task UpdateTaskDataOfHiveTaskAsync(Guid taskId, HiveTaskData taskData);

  }
}
