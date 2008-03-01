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
using HeuristicLab.Core;
using System.Xml;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace HeuristicLab.Grid {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public partial class ClientForm : Form, IClient {
    private ChannelFactory<IEngineStore> factory;
    private ServiceHost clientHost;
    private System.Timers.Timer fetchOperationTimer;
    private IEngineStore engineStore;
    private Guid currentGuid;
    private ProcessingEngine currentEngine;
    private string clientUrl;
    private object locker = new object();

    public ClientForm() {
      InitializeComponent();
      fetchOperationTimer = new System.Timers.Timer();
      fetchOperationTimer.Interval = 200;
      fetchOperationTimer.Elapsed += new System.Timers.ElapsedEventHandler(fetchOperationTimer_Elapsed);
      statusTextBox.Text = "Stopped";
      currentGuid = Guid.Empty;
    }

    private void startButton_Click(object sender, EventArgs e) {
      clientUrl = "net.tcp://" + Dns.GetHostAddresses(Dns.GetHostName())[0] + ":8002/Grid/Client";
      clientHost = new ServiceHost(this, new Uri(clientUrl));
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;

        clientHost.AddServiceEndpoint(typeof(IClient), binding, clientUrl);
        clientHost.Open();

        factory = new ChannelFactory<IEngineStore>(binding);
        engineStore = factory.CreateChannel(new EndpointAddress(addressTextBox.Text));

        fetchOperationTimer.Start();
        startButton.Enabled = false;
        stopButton.Enabled = true;
        statusTextBox.Text = "Waiting for engine";

      } catch (CommunicationException ex) {
        MessageBox.Show("Exception while connecting to the server: " + ex.Message);
        clientHost.Abort();
        startButton.Enabled = true;
        stopButton.Enabled = false;
        fetchOperationTimer.Stop();
      }
    }

    private void stopButton_Click(object sender, EventArgs e) {
      fetchOperationTimer.Stop();
      factory.Abort();
      clientHost.Close();
      statusTextBox.Text = "Stopped";
      stopButton.Enabled = false;
      startButton.Enabled = true;
    }

    private void fetchOperationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      lock(locker) {
        byte[] engineXml;
        fetchOperationTimer.Stop();
        if(engineStore.TryTakeEngine(clientUrl, out currentGuid, out engineXml)) {
          currentEngine = RestoreEngine(engineXml);
          if(InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Executing engine"; }); } else statusTextBox.Text = "Executing engine";
          currentEngine.Finished += delegate(object src, EventArgs args) {
            byte[] resultXml = SaveEngine(currentEngine);
            engineStore.StoreResult(currentGuid, resultXml);
            currentGuid = Guid.Empty;
            currentEngine = null;
            fetchOperationTimer.Interval = 100;
            fetchOperationTimer.Start();
          };
          currentEngine.Execute();
        } else {
          if(InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Waiting for engine"; }); } else statusTextBox.Text = "Waiting for engine";
          fetchOperationTimer.Interval = 5000;
          fetchOperationTimer.Start();
        }
      }
    }
    public void Abort(Guid guid) {
      lock(locker) {
        if(!IsRunningEngine(guid)) return;
        currentEngine.Abort();
      }
    }
    public bool IsRunningEngine(Guid guid) {
      return currentGuid == guid;
    }
    private ProcessingEngine RestoreEngine(byte[] engine) {
      GZipStream stream = new GZipStream(new MemoryStream(engine), CompressionMode.Decompress);
      return (ProcessingEngine)PersistenceManager.Load(stream);
    }
    private byte[] SaveEngine(IEngine engine) {
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      PersistenceManager.Save(engine, stream);
      stream.Close();
      return memStream.ToArray();
    }
  }
}
