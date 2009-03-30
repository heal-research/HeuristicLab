using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.DataAccess;
using System.Threading;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.Hive.Server.Scheduler {
  class DefaultScheduler: IScheduler {

    private ISessionFactory factory;

    private static Mutex jobLock =
      new Mutex();

    #region IScheduler Members

    public DefaultScheduler() {
      factory = ServiceLocator.GetSessionFactory();
    }

    public bool ExistsJobForClient(HeuristicLab.Hive.Contracts.BusinessObjects.HeartBeatData hbData) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();

        List<Job> allOfflineJobs = new List<Job>(jobAdapter.GetJobsByState(State.offline));
        return (allOfflineJobs.Count > 0);
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    public HeuristicLab.Hive.Contracts.BusinessObjects.Job GetNextJobForClient(Guid clientId) {
      ISession session = factory.GetSessionForCurrentThread();

      try {
        IJobAdapter jobAdapter =
          session.GetDataAdapter<Job, IJobAdapter>();

        IClientAdapter clientAdapter =
          session.GetDataAdapter<ClientInfo, IClientAdapter>();

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
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    #endregion
  }
}
