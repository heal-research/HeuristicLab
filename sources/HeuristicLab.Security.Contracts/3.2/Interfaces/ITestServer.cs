using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Security.Contracts.Interfaces {
  [ServiceContract]
  public interface ITestServer {
    [OperationContract]
    [FaultContractAttribute(typeof(CommunicationException))]
    void TestServer();
  }
}
