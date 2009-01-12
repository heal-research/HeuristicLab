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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.Diagnostics;

namespace HeuristicLab.Grid {
  public partial class ServerForm : Form {

    private EngineStore jobStore;
    private GridServer server;
    private ServiceHost externalHost;
    private ServiceHost internalHost;

    public ServerForm() {
      InitializeComponent();

      IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
      // windows XP returns the external ip on index 0 while windows vista returns the external ip as one of the last entries
      // also if IPv6 protocol is installed we want to find an entry that is IPv4
      int index = 0;
      if (System.Environment.OSVersion.Version.Major >= 6) {
        for (index = addresses.Length - 1; index >= 0; index--)
          if (addresses[index].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            break;
      }
      externalAddressTextBox.Text = "net.tcp://" + addresses[index] + ":8000/Grid/Service";
      internalAddressTextBox.Text = "net.tcp://" + addresses[index] + ":8001/Grid/JobStore";

      jobStore = new EngineStore();
      server = new GridServer(jobStore);
      Start();
    }

    private void Start() {
      externalHost = new ServiceHost(server, new Uri(externalAddressTextBox.Text));
      internalHost = new ServiceHost(jobStore, new Uri(internalAddressTextBox.Text));
      ServiceThrottlingBehavior throttlingBehavior = new ServiceThrottlingBehavior();
      throttlingBehavior.MaxConcurrentSessions = 20;
      internalHost.Description.Behaviors.Add(throttlingBehavior);
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;
        

        externalHost.AddServiceEndpoint(typeof(IGridServer), binding, externalAddressTextBox.Text);
        externalHost.Open();

        internalHost.AddServiceEndpoint(typeof(IEngineStore), binding, internalAddressTextBox.Text);
        internalHost.Open();
      } catch (CommunicationException ex) {
        MessageBox.Show("An exception occurred: " + ex.Message);
        externalHost.Abort();
        internalHost.Abort();
      }
    }
    private void statusUpdateTimer_Tick(object sender, EventArgs e) {
      waitingJobsTextBox.Text = jobStore.WaitingJobs+"";
      waitingResultsTextBox.Text = jobStore.WaitingResults+"";
      runningJobsTextBox.Text = jobStore.RunningJobs+"";
    }
  }
}
