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

namespace HeuristicLab.Grid {
  public partial class ClientForm : Form {

    private ChannelFactory<IEngineStore> factory;
    private System.Timers.Timer fetchOperationTimer;
    private IEngineStore engineStore;

    public ClientForm() {
      InitializeComponent();
      fetchOperationTimer = new System.Timers.Timer();
      fetchOperationTimer.Interval = 200;
      fetchOperationTimer.Elapsed += new System.Timers.ElapsedEventHandler(fetchOperationTimer_Elapsed);
      statusTextBox.Text = "Stopped";
    }

    private void startButton_Click(object sender, EventArgs e) {
      try {
        NetTcpBinding binding = new NetTcpBinding();
        binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
        binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
        binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
        binding.Security.Mode = SecurityMode.None;        
        factory = new ChannelFactory<IEngineStore>(binding);
        engineStore = factory.CreateChannel(new EndpointAddress(addressTextBox.Text));

        fetchOperationTimer.Start();
        startButton.Enabled = false;
        stopButton.Enabled = true;
        statusTextBox.Text = "Waiting for engine";

      } catch (Exception ex) {
        MessageBox.Show("Exception while connecting to the server: " + ex.Message);
        startButton.Enabled = true;
        stopButton.Enabled = false;
        fetchOperationTimer.Stop();
      }
    }

    private void stopButton_Click(object sender, EventArgs e) {
      fetchOperationTimer.Stop();
      factory.Abort();
      statusTextBox.Text = "Stopped";
      stopButton.Enabled = false;
      startButton.Enabled = true;
    }

    private void fetchOperationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      byte[] engineXml;
      Guid guid;
      fetchOperationTimer.Stop();
      if (engineStore.TryTakeEngine(out guid, out engineXml)) {
        ProcessingEngine engine = RestoreEngine(engineXml);
        if (InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Executing engine"; }); } else statusTextBox.Text = "Executing engine";
        engine.Finished += delegate(object src, EventArgs args) {
          byte[] resultScopeXml = SaveScope(engine.InitialOperation.Scope);
          engineStore.StoreResult(guid, resultScopeXml);
          fetchOperationTimer.Interval = 100;
          fetchOperationTimer.Start();
        };
        engine.Execute();
      } else {
        if(InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Waiting for engine"; }); } else statusTextBox.Text = "Waiting for engine";
        fetchOperationTimer.Interval = 5000;
        fetchOperationTimer.Start();
      }
    }
    private ProcessingEngine RestoreEngine(byte[] engine) {
      GZipStream stream = new GZipStream(new MemoryStream(engine), CompressionMode.Decompress);
      return (ProcessingEngine)PersistenceManager.Load(stream);
    }
    private byte[] SaveScope(IScope scope) {
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      PersistenceManager.Save(scope, stream);
      stream.Close();
      return memStream.ToArray();
    }
  }
}
