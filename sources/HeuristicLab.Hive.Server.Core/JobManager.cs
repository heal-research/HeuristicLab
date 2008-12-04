using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.Core {
  class JobManager: IJobManager {

    List<Job> jobs;

    #region IJobManager Members

    public JobManager() {
      jobs = new List<Job>();

      jobs.Add(new Job { JobId = 1, State = State.idle });
      jobs.Add(new Job { JobId = 2, State = State.idle });
      jobs.Add(new Job { JobId = 3, State = State.idle });
    }

    public ResponseList<Job> GetAllJobs() {
      return null;
    }

    #endregion
  }
}
