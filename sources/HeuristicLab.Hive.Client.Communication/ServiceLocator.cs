using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Hive.Client.Communication {
  public class ServiceLocator {
    private static ClientCommunicatorClient proxy = null;
    
    public static ClientCommunicatorClient GetClientCommunicator() {
      if (proxy == null) {
        proxy = new ClientCommunicatorClient(
          new NetTcpBinding(),
          new EndpointAddress("net.tcp://10.20.53.1:9000/HiveServer/ClientCommunicator")
          );
      }

      return proxy;
    }
  }
}
