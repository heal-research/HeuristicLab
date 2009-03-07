using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;

namespace HeuristicLab.Hive.Server.Scheduler {
  class DefaultScheduler : IScheduler {
    #region IScheduler Members

    public bool ExistsJobForClient(HeuristicLab.Hive.Contracts.BusinessObjects.HeartBeatData hbData) {
      throw new NotImplementedException();
    }

    public HeuristicLab.Hive.Contracts.BusinessObjects.Job GetNextJobForClient(Guid clientId) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
