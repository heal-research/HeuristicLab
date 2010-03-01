using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public interface IClientDao: IGenericDao<ClientInfo, Client> {
  }
}
