using System;
using System.Threading;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  public interface IHiveClientImplementation : IHiveClient {
    void Store(IHiveItem item, CancellationToken cancellationToken);
    void StoreAsync(Action<Exception> exceptionCallback, IHiveItem item, CancellationToken cancellationToken);
    void Delete(IHiveItem item);
    void StartJob(Action<Exception> exceptionCallback, RefreshableJob refreshableJob, CancellationToken cancellationToken);
    void PauseJob(RefreshableJob refreshableJob);
    void StopJob(RefreshableJob refreshableJob);
    void ResumeJob(RefreshableJob refreshableJob);
    void UpdateJob(Action<Exception> exceptionCallback, RefreshableJob refreshableJob, CancellationToken cancellationToken);
    void UpdateJob(RefreshableJob refreshableJob);
    void LoadJob(RefreshableJob refreshableJob);
    ItemTask LoadItemJob(Guid jobId);
    void TryAndRepeat(Action action, int repetitions, string errorMessage, ILog log = null);
    HiveItemCollection<JobPermission> GetJobPermissions(Guid jobId);
  }
}