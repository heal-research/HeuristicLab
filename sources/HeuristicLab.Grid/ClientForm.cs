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
using System.Net;
using System.Diagnostics;

namespace HeuristicLab.Grid {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public partial class ClientForm : Form {
    private ChannelFactory<IEngineStore> factory;
    private System.Timers.Timer fetchOperationTimer;
    private IEngineStore engineStore;
    private Guid currentGuid;
    private ProcessingEngine currentEngine;
    private object connectionLock = new object();
    private bool stopped;
    private const int CONNECTION_RETRY_TIMEOUT_SEC = 10;
    private const int MAX_RETRIES = 10;

    public ClientForm() {
      InitializeComponent();
      fetchOperationTimer = new System.Timers.Timer();
      fetchOperationTimer.Interval = 200;
      fetchOperationTimer.Elapsed += new System.Timers.ElapsedEventHandler(fetchOperationTimer_Elapsed);
      statusTextBox.Text = "Stopped";
      stopped = true;
      currentGuid = Guid.Empty;
    }

    private void startButton_Click(object sender, EventArgs e) {
      try {
        Trace.Listeners.Clear();
        Trace.Listeners.Add(new EventLogTraceListener("HeuristicLab.Grid"));

        ResetConnection();
        fetchOperationTimer.Start();
        startButton.Enabled = false;
        stopButton.Enabled = true;
        statusTextBox.Text = "Waiting for engine";
        stopped = false;

      } catch(CommunicationException ex) {
        MessageBox.Show("Exception while connecting to the server: " + ex.Message);
        startButton.Enabled = true;
        stopButton.Enabled = false;
        fetchOperationTimer.Stop();
      }
    }

    private void ResetConnection() {
      Trace.TraceInformation("Reset connection in GridClient");
      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;
      factory = new ChannelFactory<IEngineStore>(binding);
      engineStore = factory.CreateChannel(new EndpointAddress(addressTextBox.Text));
    }

    private void stopButton_Click(object sender, EventArgs e) {
      stopped = true;
      fetchOperationTimer.Stop();
      if(currentEngine != null)
        currentEngine.Abort();
      lock(connectionLock) {
        if(factory.State == CommunicationState.Opened || factory.State == CommunicationState.Opening) {
          IAsyncResult closeResult = factory.BeginClose(null, null);
          factory.EndClose(closeResult);
        }
      }
      statusTextBox.Text = "Stopped";
      stopButton.Enabled = false;
      startButton.Enabled = true;
    }

    private void fetchOperationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      byte[] engineXml = null;
      // first stop the timer!
      fetchOperationTimer.Stop();
      bool gotEngine = false;
      lock(connectionLock) {
        if(stopped) return;
        try {
          gotEngine = engineStore.TryTakeEngine(out currentGuid, out engineXml);
        } catch(TimeoutException) {
          Trace.TraceWarning("TimeoutException while trying to get an engine");
          currentEngine = null;
          currentGuid = Guid.Empty;
          // timeout -> just start the timer again
          fetchOperationTimer.Interval = 5000;
          fetchOperationTimer.Start();
        } catch(CommunicationException) {
          Trace.TraceWarning("CommunicationException while trying to get an engine");
          // connection problem -> reset connection and start the timer again
          ResetConnection();
          currentEngine = null;
          currentGuid = Guid.Empty;
          fetchOperationTimer.Interval = 5000;
          fetchOperationTimer.Start();
        }
      }
      // got engine from server and user didn't press stop -> execute the engine
      if(gotEngine && !stopped) {
        currentEngine = RestoreEngine(engineXml);
        if(InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Executing engine"; }); } else statusTextBox.Text = "Executing engine";
        currentEngine.Finished += new EventHandler(currentEngine_Finished); // register event-handler that sends result back to server and restarts timer
        currentEngine.Execute();
      } else {
        // ok we didn't get engine -> if the user didn't press stop this means that the server doesn't have engines for us
        // if the user pressed stop we must not start the timer
        if(!stopped) {
          if(InvokeRequired) { Invoke((MethodInvoker)delegate() { statusTextBox.Text = "Waiting for engine"; }); } else statusTextBox.Text = "Waiting for engine";
          // start the timer again
          fetchOperationTimer.Interval = 5000;
          fetchOperationTimer.Start();
        }
      }
    }

    void currentEngine_Finished(object sender, EventArgs e) {
      ProcessingEngine engine = (ProcessingEngine)sender;

      // if the engine was stopped because of an error (not suspended because of a breakpoint) 
      // it's not necessary to return the whole engine
      // instead just return an empty engine that has the aborted flag set
      if(engine.Canceled && !engine.Suspended) {
        engine.Reset();
        engine.OperatorGraph.Clear();
        engine.Abort();
      }

      byte[] resultXml = SaveEngine(engine);
      bool success = false;
      int retries = 0;
      do {
        lock(connectionLock) {
          if(!stopped) {
            try {
              engineStore.StoreResult(currentGuid, resultXml);
              success = true;
            } catch(TimeoutException) {
              Trace.TraceWarning("TimeoutException while trying to store the result of an engine");
              success = false;
              retries++;
              Thread.Sleep(TimeSpan.FromSeconds(CONNECTION_RETRY_TIMEOUT_SEC));
            } catch(CommunicationException) {
              Trace.TraceWarning("CommunicationException while trying to store the result of an engine");
              ResetConnection();
              success = false;
              retries++;
              Thread.Sleep(TimeSpan.FromSeconds(CONNECTION_RETRY_TIMEOUT_SEC));
            }
          }
        }
      } while(!stopped && !success && retries < MAX_RETRIES);
      // ok if we could store the result it's probable that the server can send us another engine use a small time-interval
      if(success)
        fetchOperationTimer.Interval = 100;
      else fetchOperationTimer.Interval = CONNECTION_RETRY_TIMEOUT_SEC; // if there were problems -> sleep for a longer time
      // clear state
      currentEngine = null;
      currentGuid = Guid.Empty;
      // start the timer
      fetchOperationTimer.Start();
    }

    private ProcessingEngine RestoreEngine(byte[] engine) {
      return (ProcessingEngine)PersistenceManager.RestoreFromGZip(engine);
    }
    private byte[] SaveEngine(IEngine engine) {
      return PersistenceManager.SaveToGZip(engine);
    }
  }
}
