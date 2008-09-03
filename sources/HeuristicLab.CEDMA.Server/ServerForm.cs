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
        InitDefaultOntology();
      } else {
        database = new Database(connectionString);
        if(database.GetOntologyItems().Count==0) InitDefaultOntology();
      }
    }

    private void InitDefaultOntology() {
      // init default ontology
      StringData cedmaOntology = new StringData("CedmaOntology");
      StringData definesEntity = new StringData("definesEntity");
      StringData classGpFunctionTree = new StringData("class:GpFunctionTree");
      StringData classDataset = new StringData("class:Dataset");
      StringData instanceOf = new StringData("instanceOf");
      StringData hasModel = new StringData("hasModel");
      StringData modelOf = new StringData("modelOf");
      StringData targetVariable = new StringData("targetVariable");
      StringData trainingMse = new StringData("trainingMSE");
      StringData validationMse = new StringData("validationMSE");

      LinkItems(cedmaOntology, definesEntity, cedmaOntology);
      LinkItems(cedmaOntology, definesEntity, definesEntity);
      LinkItems(cedmaOntology, definesEntity, classGpFunctionTree);
      LinkItems(cedmaOntology, definesEntity, classDataset);
      LinkItems(cedmaOntology, definesEntity, instanceOf);
      LinkItems(cedmaOntology, definesEntity, hasModel);
      LinkItems(cedmaOntology, definesEntity, modelOf);
      LinkItems(cedmaOntology, definesEntity, targetVariable);
      LinkItems(cedmaOntology, definesEntity, trainingMse);
      LinkItems(cedmaOntology, definesEntity, validationMse);
    }

    private void LinkItems(StringData subj, StringData pred, StringData prop) {
      ItemEntry subjEntry = new ItemEntry();
      ItemEntry predEntry = new ItemEntry();
      ItemEntry propEntry = new ItemEntry();
      subjEntry.Guid = subj.Guid;
      subjEntry.RawData = PersistenceManager.SaveToGZip(subj);
      predEntry.Guid = pred.Guid;
      predEntry.RawData = PersistenceManager.SaveToGZip(pred);
      propEntry.Guid = prop.Guid;
      propEntry.RawData = PersistenceManager.SaveToGZip(prop);

      database.LinkItems(subjEntry, predEntry, propEntry);
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
