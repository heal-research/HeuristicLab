using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core.InternalInterfaces {
  public interface IScheduler {
    bool ExistsJobForClient(HeartBeatData hbData);
    Job GetNextJobForClient(Guid clientId);
  }
}
