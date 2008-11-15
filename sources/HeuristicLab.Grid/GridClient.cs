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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Grid {
  class GridClient {
    private string uri;
    private ChannelFactory<IEngineStore> factory;
    private System.Timers.Timer fetchOperationTimer;
    private IEngineStore engineStore;
    private object connectionLock = new object();
    private const int CONNECTION_RETRY_TIMEOUT_SEC = 10;
    private const int MAX_RETRIES = 10;

    public bool Waiting {
      get {
        return !executing && !stopped;
      }
    }

    private bool executing;
    public bool Executing {
      get {
        return executing;
      }
    }

    private bool stopped;
    public bool Stopped {
      get {
        return stopped;
      }
    }

    private string statusMessage = "";
    public string StatusMessage {
      get {
        return statusMessage;
      }
    }

    internal GridClient() {
      fetchOperationTimer = new System.Timers.Timer();
      fetchOperationTimer.Interval = 200;
      fetchOperationTimer.Elapsed += new System.Timers.ElapsedEventHandler(fetchOperationTimer_Elapsed);
      stopped = true;
    }

    internal void Start(string uri) {
      try {
        this.uri = uri;
        ResetConnection();
        fetchOperationTimer.Start();
        stopped = false;
      } catch(CommunicationException ex) {
        statusMessage = DateTime.Now.ToShortTimeString()+": Exception while connecting to the server: " + ex.Message;
        fetchOperationTimer.Stop();
      }
    }

    internal void Stop() {
      fetchOperationTimer.Stop();
      lock(connectionLock) {
        if(factory.State == CommunicationState.Opened || factory.State == CommunicationState.Opening) {
          IAsyncResult closeResult = factory.BeginClose(null, null);
          factory.EndClose(closeResult);
        }
      }
      stopped = true;
    }

    private void ResetConnection() {
      Trace.TraceInformation("Reset connection in GridClient");
      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;
      factory = new ChannelFactory<IEngineStore>(binding);
      engineStore = factory.CreateChannel(new EndpointAddress(uri));
    }

    private void fetchOperationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      try {
        byte[] engineXml = null;
        Guid currentGuid;
        // first stop the timer!
        fetchOperationTimer.Stop();
        bool gotEngine = false;
        lock(connectionLock) {
          if(stopped) return;
          try {
            gotEngine = engineStore.TryTakeEngine(out currentGuid, out engineXml);
          } catch(TimeoutException) {
            ChangeStatusMessage("TimeoutException while trying to get an engine");
            currentGuid = Guid.Empty;
            // timeout -> just start the timer again
            fetchOperationTimer.Interval = 5000;
            fetchOperationTimer.Start();
          } catch(CommunicationException) {
            ChangeStatusMessage("CommunicationException while trying to get an engine");
            // connection problem -> reset connection and start the timer again
            ResetConnection();
            currentGuid = Guid.Empty;
            fetchOperationTimer.Interval = 5000;
            fetchOperationTimer.Start();
          }
        }
        // got engine from server and user didn't press stop -> execute the engine
        if(gotEngine && !stopped) {
          executing = true;
          AppDomain engineDomain = PluginManager.Manager.CreateAndInitAppDomain("Engine domain");
          Type engineRunnerType = typeof(EngineRunner);
          
          EngineRunner runner = (EngineRunner)engineDomain.CreateInstanceAndUnwrap(engineRunnerType.Assembly.GetName().Name, engineRunnerType.FullName);
          byte[] resultXml = runner.Execute(engineXml);
          bool success = false;
          int retries = 0;
          do {
            lock(connectionLock) {
              if(!stopped) {
                try {
                  engineStore.StoreResult(currentGuid, resultXml);
                  success = true;
                } catch(TimeoutException) {
                  ChangeStatusMessage("TimeoutException while trying to store the result of an engine");
                  success = false;
                  retries++;
                  Thread.Sleep(TimeSpan.FromSeconds(CONNECTION_RETRY_TIMEOUT_SEC));
                } catch(CommunicationException) {
                  ChangeStatusMessage("CommunicationException while trying to store the result of an engine");
                  ResetConnection();
                  success = false;
                  retries++;
                  Thread.Sleep(TimeSpan.FromSeconds(CONNECTION_RETRY_TIMEOUT_SEC));
                }
              }
            }
          } while(!stopped && !success && retries < MAX_RETRIES);
          // dispose the AppDomain that was created to run the job
          AppDomain.Unload(engineDomain);
          executing = false;
          // ok if we could store the result it's probable that the server can send us another engine use a small time-interval
          if(success)
            fetchOperationTimer.Interval = 100;
          else fetchOperationTimer.Interval = 30000; // if there were problems -> sleep for a longer time
          // clear state
          currentGuid = Guid.Empty;
          // start the timer
          fetchOperationTimer.Start();
        } else {
          // ok we didn't get engine -> if the user didn't press stop this means that the server doesn't have engines for us
          // if the user pressed stop we must not start the timer
          if(!stopped) {
            // start the timer again
            fetchOperationTimer.Interval = 5000;
            fetchOperationTimer.Start();
          }
        }
      } catch(Exception ex) {
        // in case something goes wrong when creating / unloading the AppDomain
        ChangeStatusMessage("Uncaught exception " + ex.Message);
        Stop();
      }
    }

    private void ChangeStatusMessage(string msg) {
      Trace.TraceWarning(msg);
      statusMessage = DateTime.Now.ToShortTimeString() + ": " + msg;
    }
  }
}
