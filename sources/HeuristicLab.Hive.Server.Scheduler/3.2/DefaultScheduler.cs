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

        List<Job> allOfflineJobsForClient = new List<Job>(
          jobAdapter.FindJobs(State.offline, 
          hbData.FreeCores,
          hbData.FreeMemory, 
          hbData.ClientId));
        return (allOfflineJobsForClient != null && allOfflineJobsForClient.Count > 0);
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

        ClientInfo client = clientAdapter.GetById(clientId);
        LinkedList<Job> allOfflineJobsForClient = new LinkedList<Job>(
          jobAdapter.FindJobs(State.offline, 
          client.NrOfFreeCores, 
          client.FreeMemory, 
          client.Id));

        Job jobToCalculate = null;
        if (allOfflineJobsForClient != null && allOfflineJobsForClient.Count > 0) {
          jobToCalculate = allOfflineJobsForClient.First.Value;
          jobToCalculate.State = State.calculating;
          jobToCalculate.Client = client;
          jobToCalculate.Client.State = State.calculating;

          jobToCalculate.DateCalculated = DateTime.Now;
          jobAdapter.Update(jobToCalculate);
        }
        jobLock.ReleaseMutex();
        /// End Critical section ///

        return jobToCalculate;
      }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    #endregion
  }
}
