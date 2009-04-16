using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Client.Core.ClientConsoleService.Interfaces;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Client.Core.ClientConsoleService {
  public class ClientConsoleServer {

    DiscoveryService discService = new DiscoveryService();
   
    private bool AddMexEndpoint(ServiceHost serviceHost) {
      if (serviceHost != null) {
        ServiceMetadataBehavior behavior =
            new ServiceMetadataBehavior();
        serviceHost.Description.Behaviors.Add(behavior);

        return serviceHost.AddServiceEndpoint(
          typeof(IMetadataExchange),
          MetadataExchangeBindings.CreateMexTcpBinding(),
          "mex") != null;
      } else
        return false;
    }
    
    public ServiceHost StartClientConsoleServer(Uri uriTcp) {
      IClientConsoleCommunicator[] clientConsoleServerInstances =
        discService.GetInstances<IClientConsoleCommunicator>();

      if (clientConsoleServerInstances.Length > 0) {
        ServiceHost serviceHost =
                new ServiceHost(clientConsoleServerInstances[0].GetType(),
                  uriTcp);

        serviceHost.AddServiceEndpoint(
          typeof(IClientConsoleCommunicator),
              WcfSettings.GetBinding(),
              "ClientConsoleCommunicator");

        AddMexEndpoint(serviceHost);

        serviceHost.Open();
        return serviceHost;
      } else {
        return null;
      }
    }
  }
}
