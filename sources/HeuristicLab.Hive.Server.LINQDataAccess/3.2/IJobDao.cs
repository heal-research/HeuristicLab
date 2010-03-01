using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public interface IJobDao: IGenericDao<HeuristicLab.Hive.Contracts.BusinessObjects.Job, Job> {

  }
}
