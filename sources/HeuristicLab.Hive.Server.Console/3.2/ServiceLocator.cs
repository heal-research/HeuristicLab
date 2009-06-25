using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HeuristicLab.Hive.Contracts.Interfaces;
using System.ServiceModel;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Server.ServerConsole {
  internal class ServiceLocator {
    private static IServerConsoleFacade serverConsoleFacade = null;

    internal static string Address { get; set; }
    internal static string Port { get; set; }

    internal static IClientManager GetClientManager() {
      return GetServerConsoleFacade() as IClientManager;
    }

    internal static IJobManager GetJobManager() {
      return GetServerConsoleFacade() as IJobManager;
    }

    internal static IServerConsoleFacade GetServerConsoleFacade() {
      if (serverConsoleFacade == null &&
        Address != String.Empty &&
        Port != String.Empty) {

        //binding.MaxReceivedMessageSize = 5000000;

        ChannelFactory<IServerConsoleFacade> factory =
          new ChannelFactory<IServerConsoleFacade>(
            WcfSettings.GetBinding(),
            new EndpointAddress("net.tcp://" + Address + ":" + Port + "/HiveServerConsole/ServerConsoleFacade"));
                         
        serverConsoleFacade = factory.CreateChannel();
      }
      
      return serverConsoleFacade;
    }

    internal static void ShutDownFacade() {
      serverConsoleFacade = null;
    }
  }
}
