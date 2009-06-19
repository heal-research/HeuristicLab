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
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using HeuristicLab.Grid;
using HeuristicLab.Grid.HiveBridge;

namespace HeuristicLab.CEDMA.Server {
  public class Server {
    private static readonly string rdfFile = AppDomain.CurrentDomain.BaseDirectory + "rdf_store.db3";
    private static readonly string rdfConnectionString = "sqlite:rdf:Data Source=\"" + rdfFile + "\"";

    private ServiceHost host;
    private Store store;
    private IDispatcher dispatcher;
    private IExecuter executer;

    private string gridServiceUrl;
    public string GridServiceUrl {
      get { return gridServiceUrl; }
      set { gridServiceUrl = value; }
    }

    private string cedmaServiceUrl;
    public string CedmaServiceUrl {
      get { return cedmaServiceUrl; }
      set { cedmaServiceUrl = value; }
    }

    private int maxActiveJobs;
    public int MaxActiveJobs {
      get { return maxActiveJobs; }
      set {
        if (value > 0) {
          maxActiveJobs = value;
          if (executer != null) {
            executer.MaxActiveJobs = value;
          }
        }
      }
    }

    public Server() {
      IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
      // windows XP returns the external ip on index 0 while windows vista returns the external ip as one of the last entries
      // also if IPv6 protocol is installed we want to find an entry that is IPv4
      int index = 0;
      if (System.Environment.OSVersion.Version.Major >= 6) {
        for (index = addresses.Length - 1; index >= 0; index--)
          if (addresses[index].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            break;
      }
      cedmaServiceUrl = "net.tcp://" + addresses[index] + ":8002/CEDMA";
      store = new Store(rdfConnectionString);
      maxActiveJobs = 10;
    }

    public void Start() {
      host = new ServiceHost(store, new Uri(cedmaServiceUrl));
      ServiceThrottlingBehavior throttlingBehavior = new ServiceThrottlingBehavior();
      throttlingBehavior.MaxConcurrentSessions = 20;
      host.Description.Behaviors.Add(throttlingBehavior);
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.SendTimeout = new TimeSpan(10, 0, 0);
        binding.MaxReceivedMessageSize = int.MaxValue;
        binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
        binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
        host.AddServiceEndpoint(typeof(IStore), binding, cedmaServiceUrl);
        host.Open();
      }
      catch (CommunicationException ex) {
        MessageBox.Show("An exception occurred: " + ex.Message);
        host.Abort();
      }
    }

    internal string[] GetActiveJobDescriptions() {
      if (executer != null) return executer.GetJobs();
      else return new string[] { };
    }

    internal void Connect(string serverUrl) {
      dispatcher = new SimpleDispatcher(store);
      IGridServer gridServer = null;
      if (serverUrl.Contains("ExecutionEngine")) {
        gridServer = new HiveGridServerWrapper(serverUrl);
      } else {
        // default is grid backend
        gridServer = new GridServerProxy(serverUrl);
      }
      executer = new GridExecuter(dispatcher, store, gridServer);
      executer.MaxActiveJobs = MaxActiveJobs;
      executer.Start();
    }
  }
}
