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
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Server {
  public partial class ServerForm : Form {
    private ServiceHost host;
    private ServiceHost rdfHost;
    private Database database;
    private Store store;
    private static readonly string dbFile = AppDomain.CurrentDomain.BaseDirectory + "/test.db3";
    private static readonly string connectionString = "Data Source=\"" + dbFile + "\";Pooling=False";
    private static readonly string rdfFile = AppDomain.CurrentDomain.BaseDirectory + "rdf_store.db3";
    private static readonly string rdfConnectionString = "sqlite:rdf:Data Source=\"" + rdfFile + "\"";
    public ServerForm() {
      InitializeComponent();
      // windows XP returns the external ip on index 0 while windows vista returns the external ip on index 2
      if(System.Environment.OSVersion.Version.Major >= 6) {
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

    private void InitRdfStore() {
      store = new Store(rdfConnectionString);
    }

    private void Start() {
      InitDatabase();
      InitRdfStore();
      //InitRunScheduler();

      host = new ServiceHost(database, new Uri(addressTextBox.Text));
      rdfHost = new ServiceHost(store, new Uri(addressTextBox.Text+"/RdfStore"));
      ServiceThrottlingBehavior throttlingBehavior = new ServiceThrottlingBehavior();
      throttlingBehavior.MaxConcurrentSessions = 20;
      host.Description.Behaviors.Add(throttlingBehavior);
      rdfHost.Description.Behaviors.Add(throttlingBehavior);
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;

        host.AddServiceEndpoint(typeof(IDatabase), binding, addressTextBox.Text);
        host.Open();
        rdfHost.AddServiceEndpoint(typeof(IStore), binding, addressTextBox.Text+"/RdfStore");
        rdfHost.Open();
      } catch(CommunicationException ex) {
        MessageBox.Show("An exception occurred: " + ex.Message);
        host.Abort();
        rdfHost.Abort();
      }
    }

    private void startButton_Click(object sender, EventArgs e) {
      Start();
      startButton.Enabled = false;
    }
  }
}
