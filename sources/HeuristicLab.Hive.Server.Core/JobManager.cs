using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  class JobManager: IJobManager {
    #region IJobManager Members

    public List<Job> GetAllJobs() {
      throw new NotImplementedException();
    }

    #endregion
  }
}
