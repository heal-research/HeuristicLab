using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using System.Threading;

namespace HeuristicLab.Hive.Server.Scheduler {
  class DefaultScheduler: IScheduler {

    IJobAdapter jobAdapter;
    IClientAdapter clientAdapter;

    private static Mutex jobLock =
      new Mutex();

    #region IScheduler Members

    public DefaultScheduler() {
      jobAdapter = ServiceLocator.GetJobAdapter();
      clientAdapter = ServiceLocator.GetClientAdapter();
    }

    public bool ExistsJobForClient(HeuristicLab.Hive.Contracts.BusinessObjects.HeartBeatData hbData) {
      List<Job> allOfflineJobs = new List<Job>(jobAdapter.GetJobsByState(State.offline));
      return (allOfflineJobs.Count > 0);
    }

    public HeuristicLab.Hive.Contracts.BusinessObjects.Job GetNextJobForClient(Guid clientId) {

      /// Critical section ///
      jobLock.WaitOne();

      LinkedList<Job> allOfflineJobs = new LinkedList<Job>(jobAdapter.GetJobsByState(State.offline));

      Job job2Calculate = null;
      if (allOfflineJobs != null && allOfflineJobs.Count > 0) {
        job2Calculate = allOfflineJobs.First.Value;
        job2Calculate.State = State.calculating;
        job2Calculate.Client = clientAdapter.GetById(clientId);
        job2Calculate.Client.State = State.calculating;

        job2Calculate.DateCalculated = DateTime.Now;
        jobAdapter.Update(job2Calculate);
      }
      jobLock.ReleaseMutex();
      /// End Critical section ///

      return job2Calculate;
    }

    #endregion
  }
}
