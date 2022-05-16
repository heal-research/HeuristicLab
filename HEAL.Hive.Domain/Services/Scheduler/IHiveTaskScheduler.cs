using HEAL.Hive.Domain.Scheduler;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEAL.Hive.Domain.Services.Scheduler {
  public interface IHiveTaskScheduler {
    Task<IEnumerable<SchedulerHiveTaskInfo>> ScheduleAsync(IEnumerable<SchedulerHiveTaskInfo> tasks, int count = 1);
  }
}
