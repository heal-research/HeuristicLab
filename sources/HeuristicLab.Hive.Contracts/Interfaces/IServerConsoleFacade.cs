using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// Facade for the server management console
  /// </summary>
  [ServiceContract]
  public interface IServerConsoleFacade : IClientManager, IJobManager, IUserRoleManager {
  }
}
