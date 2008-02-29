using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Grid {
  [ServiceContract(Namespace = "http://HeuristicLab.Grid")]
  interface IClient {
    [OperationContract]
    void Abort(Guid guid);

    [OperationContract]
    bool IsRunningEngine(Guid guid);
  }
}
