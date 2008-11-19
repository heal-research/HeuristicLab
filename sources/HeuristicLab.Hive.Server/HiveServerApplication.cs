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
    const int port = 9000;

    public override void Run() {
      IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
      string externalIP = IPHost.AddressList[0].ToString();

      DiscoveryService discService =
        new DiscoveryService();
      IClientCommunicator[] instances = 
        discService.GetInstances<IClientCommunicator>();

      if (instances.Length > 0) {
        Uri uriTcp =
          new Uri("net.tcp://" + externalIP + ":" + port +"/HiveServer/"); 

        ServiceHost serviceHost =
                new ServiceHost(instances[0].GetType(),
                  uriTcp);

        System.ServiceModel.Channels.Binding binding = 
          new NetTcpBinding();

        serviceHost.AddServiceEndpoint(
          typeof(IClientCommunicator),
              binding,
              "ClientCommunicator");

        ServiceMetadataBehavior behavior =
          new ServiceMetadataBehavior();
        serviceHost.Description.Behaviors.Add(behavior);

        serviceHost.AddServiceEndpoint(
          typeof(IMetadataExchange),
          MetadataExchangeBindings.CreateMexTcpBinding(),
          "mex");

        serviceHost.Open();

        Form mainForm = new MainForm(serviceHost.BaseAddresses[0]);
        Application.Run(mainForm);

        serviceHost.Close();
      } else {
        MessageBox.Show("Error - no ClientCommunicator instance");
      }
    }
  }
}
