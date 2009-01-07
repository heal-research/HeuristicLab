#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.Interfaces;

namespace HeuristicLab.Hive.Server {
  [ClassInfo(Name = "Hive Server",
      Description = "Server application for the distributed hive engine.",
      AutoRestart = true)]
  class HiveServerApplication : ApplicationBase {
    const int port =
      9000;

    DiscoveryService discService =
        new DiscoveryService();

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

    private ServiceHost StartClientCommunicator(Uri uriTcp) {
      IClientCommunicator[] clientCommunicatorInstances =
        discService.GetInstances<IClientCommunicator>();

      if (clientCommunicatorInstances.Length > 0) {
        ServiceHost serviceHost =
                new ServiceHost(clientCommunicatorInstances[0].GetType(),
                  uriTcp);

        System.ServiceModel.Channels.Binding binding =
          new NetTcpBinding();

        serviceHost.AddServiceEndpoint(
          typeof(IClientCommunicator),
              binding,
              "ClientCommunicator");

        AddMexEndpoint(serviceHost);

        serviceHost.Open();

        return serviceHost;
      } else
        return null;
    }

    private ServiceHost StartServerConsoleFacade(Uri uriTcp) {
      IServerConsoleFacade[] serverConsoleInstances =
        discService.GetInstances<IServerConsoleFacade>();

      if (serverConsoleInstances.Length > 0) {
        ServiceHost serviceHost =
            new ServiceHost(serverConsoleInstances[0].GetType(),
                  uriTcp);

        System.ServiceModel.Channels.Binding binding =
          new NetTcpBinding();

        serviceHost.AddServiceEndpoint(
          typeof(IServerConsoleFacade),
            binding,
            "ServerConsoleFacade");

        AddMexEndpoint(serviceHost);

        serviceHost.Open();

        return serviceHost;
      } else
        return null;
    }

    public override void Run() {
      IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
      int index = 0;
      if (System.Environment.OSVersion.Version.Major >= 6) {
        for (index = addresses.Length - 1; index >= 0; index--)
          if (addresses[index].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            break;
      }

      Uri uriTcp =
          new Uri("net.tcp://" + addresses[index] + ":" + port + "/HiveServer/"); 

      ServiceHost clientCommunicator =
        StartClientCommunicator(uriTcp);

      uriTcp =
        new Uri("net.tcp://" + addresses[index] + ":" + port + "/HiveServerConsole/");

      ServiceHost serverConsoleFacade =
        StartServerConsoleFacade(uriTcp);

      ILifecycleManager[] lifecycleManagers =
         discService.GetInstances<ILifecycleManager>();

      if (lifecycleManagers.Length > 0) {
        ILifecycleManager lifecycleManager = 
          lifecycleManagers[0];

        lifecycleManager.Init();
        //sync with db every 5 minutes
        lifecycleManager.GetTransactionManager().EnableAutoUpdate(
          new TimeSpan(0, 0, 10));

        Form mainForm = new MainForm(clientCommunicator.BaseAddresses[0],
            serverConsoleFacade.BaseAddresses[0]);

         Application.Run(mainForm);

         lifecycleManager.Shutdown();
      }

      clientCommunicator.Close();
      serverConsoleFacade.Close();

      
    }
  }
}
