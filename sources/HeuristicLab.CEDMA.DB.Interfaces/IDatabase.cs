using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.CEDMA.DB.Interfaces {
  [ServiceContract(Namespace = "http://HeuristicLab.Cedma")]
  public interface IDatabase {
    [OperationContract]
    IList<IAgent> GetAgents();

    [OperationContract]
    IAgent CreateAgent();
  }
}
