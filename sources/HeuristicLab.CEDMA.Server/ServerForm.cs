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
using HeuristicLab.CEDMA.DB;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Data.Common;
using System.Threading;
using HeuristicLab.Grid;

namespace HeuristicLab.CEDMA.Server {
  public partial class ServerForm : Form {
    private ServiceHost host;
    private Database database;
    private static readonly string dbFile = AppDomain.CurrentDomain.BaseDirectory + "/test.db3";
    private static readonly string connectionString = "Data Source=\""+dbFile+"\";Pooling=False";
    public ServerForm() {
      InitializeComponent();
      // windows XP returns the external ip on index 0 while windows vista returns the external ip on index 2
      if (System.Environment.OSVersion.Version.Major >= 6) {
        addressTextBox.Text = "net.tcp://" + Dns.GetHostAddresses(Dns.GetHostName())[2] + ":8002/CEDMA/World";
      } else {
        addressTextBox.Text = "net.tcp://" + Dns.GetHostAddresses(Dns.GetHostName())[0] + ":8002/CEDMA/World";
      }
    }

    private void InitRunScheduler() {
      JobManager jobManager = new JobManager(gridAddress.Text);
      jobManager.Reset();
      RunScheduler scheduler = new RunScheduler(database, jobManager, addressTextBox.Text);
      Thread runSchedulerThread = new Thread(scheduler.Run);
      runSchedulerThread.Start();
    }

    private void InitDatabase() {
      DbProviderFactory fact;
      fact = DbProviderFactories.GetFactory("System.Data.SQLite");
      if(!System.IO.File.Exists(dbFile)) {
        database = new Database(connectionString);
        database.CreateNew();
      } else {
        database = new Database(connectionString);
      }
    }

    private void Start() {
      InitDatabase();
      InitRunScheduler();

      host = new ServiceHost(database, new Uri(addressTextBox.Text));
      ServiceThrottlingBehavior throttlingBehavior = new ServiceThrottlingBehavior();
      throttlingBehavior.MaxConcurrentSessions = 20;
      host.Description.Behaviors.Add(throttlingBehavior);
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;

        host.AddServiceEndpoint(typeof(IDatabase), binding, addressTextBox.Text);
        host.Open();
      } catch (CommunicationException ex) {
        MessageBox.Show("An exception occurred: " + ex.Message);
        host.Abort();
      }
    }

    private void startButton_Click(object sender, EventArgs e) {
      Start();
      startButton.Enabled = false;
    }
  }
}
