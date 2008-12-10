using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
//using HeuristicLab.Hive.Client.Communication.ClientConsole;

namespace HeuristicLab.Hive.Client.Console {
  public class ServiceLocator {
    private static ClientConsoleCommunicatorClient proxy = null;

    public static ClientConsoleCommunicatorClient ClientConsoleCommunicatorClient() {
      if (proxy == null) {
        proxy = new ClientConsoleCommunicatorClient(
          new NetTcpBinding(),
          new EndpointAddress("net.tcp://127.0.0.1:8000/ClientConsole/ClientConsoleCommunicator")
          );
      }

      return proxy;
    }
  }
}
