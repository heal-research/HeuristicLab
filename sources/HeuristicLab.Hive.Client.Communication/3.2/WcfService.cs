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
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication.ServerService;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Client.Communication {
  /// <summary>
  /// WcfService class is implemented as a Singleton and works as a communication Layer with the Server
  /// </summary>
  public class WcfService {
    private static WcfService instance;
    /// <summary>
    /// Getter for the Instance of the WcfService
    /// </summary>
    /// <returns>the Instance of the WcfService class</returns>
    public static WcfService Instance {
      get {
        if (instance == null) {
          instance = new WcfService();
        }
        return instance;
      }
    }

    public DateTime ConnectedSince { get; private set; }    
    public NetworkEnum.WcfConnState ConnState { get; private set; }
    public string ServerIP { get; private set; }
    public int ServerPort { get; private set; }

    public event EventHandler ConnectionRestored;    
    public event EventHandler ServerChanged;
    public event EventHandler Connected;    

    public ClientCommunicatorClient proxy = null;

    /// <summary>
    /// Constructor
    /// </summary>
    private WcfService() {
      ConnState = NetworkEnum.WcfConnState.Disconnected;
    }

    /// <summary>
    /// Connects with the Server, registers the events and fires the Connected (and quiet possibly the ConnectionRestored) Event.
    /// </summary>
    public void Connect() {
      try {
        proxy = new ClientCommunicatorClient(
          WcfSettings.GetBinding(),
          new EndpointAddress("net.tcp://" + ServerIP + ":" + ServerPort + "/HiveServer/ClientCommunicator")
        );

        proxy.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(proxy_LoginCompleted);
        proxy.SendJobCompleted += new EventHandler<SendJobCompletedEventArgs>(proxy_SendJobCompleted);
        proxy.StoreFinishedJobResultCompleted += new EventHandler<StoreFinishedJobResultCompletedEventArgs>(proxy_StoreFinishedJobResultCompleted);
        proxy.ProcessSnapshotCompleted += new EventHandler<ProcessSnapshotCompletedEventArgs>(proxy_ProcessSnapshotCompleted);
        proxy.ProcessHeartBeatCompleted += new EventHandler<ProcessHeartBeatCompletedEventArgs>(proxy_ProcessHeartBeatCompleted);
        proxy.Open();

        ConnState = NetworkEnum.WcfConnState.Connected;
        ConnectedSince = DateTime.Now;
        
        if (Connected != null)
          Connected(this, new EventArgs());                               
        //Todo: This won't be hit. EVER        
        if (ConnState == NetworkEnum.WcfConnState.Failed)
          ConnectionRestored(this, new EventArgs());        
      }
      catch (Exception ex) {      
        HandleNetworkError(ex);
      }
    }



    /// <summary>
    /// Changes the Connectionsettings (serverIP & serverPort) and reconnects
    /// </summary>
    /// <param name="serverIP">current Server IP</param>
    /// <param name="serverPort">current Server Port</param>
    public void Connect(String serverIP, int serverPort) {
      String oldIp = this.ServerIP;
      int oldPort = this.ServerPort;
      this.ServerIP = serverIP;
      this.ServerPort = serverPort;      
      Connect();
      if (oldIp != serverIP || oldPort != ServerPort)
        if(ServerChanged != null) 
          ServerChanged(this, new EventArgs());
    }
    
    /// <summary>
    /// Disconnects the Client from the Server
    /// </summary>
    public void Disconnect() {
      ConnState = NetworkEnum.WcfConnState.Disconnected;
    }

    /// <summary>
    /// Network communication Error Handler - Every network error gets logged and the connection switches to faulted state
    /// </summary>
    /// <param name="e">The Exception</param>
    private void HandleNetworkError(Exception e) {
      ConnState = NetworkEnum.WcfConnState.Failed;
      Logging.Instance.Error(this.ToString(), "exception: ", e);
    }

    

    /// <summary>
    /// Methods for the Server Login
    /// </summary>
    #region Login
    public event System.EventHandler<LoginCompletedEventArgs> LoginCompleted;
    public void LoginAsync(ClientInfo clientInfo) {
      if (ConnState == NetworkEnum.WcfConnState.Connected)
        proxy.LoginAsync(clientInfo);
    }
    private void proxy_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Error == null)
        LoginCompleted(sender, e);
      else
        HandleNetworkError(e.Error.InnerException);
    }

    public void LoginSync(ClientInfo clientInfo) {
      try {
        if (ConnState == NetworkEnum.WcfConnState.Connected) {
          Response res = proxy.Login(clientInfo);
          ConnState = NetworkEnum.WcfConnState.Loggedin;
          Logging.Instance.Info(this.ToString(), res.StatusMessage);
        }
      }
      catch (Exception e) {
        HandleNetworkError(e);
      }
    }

    #endregion

    /// <summary>
    /// Pull a Job from the Server
    /// </summary>
    #region PullJob
    public event System.EventHandler<SendJobCompletedEventArgs> SendJobCompleted;
    public void SendJobAsync(Guid guid) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)        
        proxy.SendJobAsync(guid);
    }
    void proxy_SendJobCompleted(object sender, SendJobCompletedEventArgs e) {
      if (e.Error == null)
        SendJobCompleted(sender, e);
      else
        HandleNetworkError(e.Error);
    }
    #endregion

    /// <summary>
    /// Send back finished Job Results
    /// </summary>
    #region SendJobResults
    public event System.EventHandler<StoreFinishedJobResultCompletedEventArgs> StoreFinishedJobResultCompleted;
    public void StoreFinishedJobResultAsync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        proxy.StoreFinishedJobResultAsync(clientId, jobId, result, percentage, exception, finished);
    }
    private void proxy_StoreFinishedJobResultCompleted(object sender, StoreFinishedJobResultCompletedEventArgs e) {
      if (e.Error == null)
        StoreFinishedJobResultCompleted(sender, e);
      else
        HandleNetworkError(e.Error);
    }

    #endregion

    #region Processsnapshots
    public event System.EventHandler<ProcessSnapshotCompletedEventArgs> ProcessSnapshotCompleted;
    public void ProcessSnapshotAsync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if(ConnState == NetworkEnum.WcfConnState.Loggedin)
        proxy.ProcessSnapshotAsync(clientId, jobId, result, percentage, exception);
    }
    void proxy_ProcessSnapshotCompleted(object sender, ProcessSnapshotCompletedEventArgs e) {
      if (e.Error == null)
        ProcessSnapshotCompleted(sender, e);
      else
        HandleNetworkError(e.Error);
    }    
    
    #endregion

    /// <summary>
    /// Methods for sending the periodically Heartbeat
    /// </summary>
    #region Heartbeat

    public event System.EventHandler<ProcessHeartBeatCompletedEventArgs> SendHeartBeatCompleted;
    public void SendHeartBeatAsync(HeartBeatData hbd) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        proxy.ProcessHeartBeatAsync(hbd);
    }

    private void proxy_ProcessHeartBeatCompleted(object sender, ProcessHeartBeatCompletedEventArgs e) {
      if (e.Error == null && e.Result.Success == true)
        SendHeartBeatCompleted(sender, e);
      else
        HandleNetworkError(e.Error);
    }

    #endregion  

    /// <summary>
    /// Send back finished and Stored Job Results
    /// </summary>
    /*#region SendJobResults
    public event System.EventHandler<StoreFinishedJobResultCompletedEventArgs> ProcessStoredJobResultCompleted;
    public void ProcessStoredJobResultAsync(Guid clientId, long jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        //TODO: some sort of algo for the stored jobs
        proxy.ProcessJobResultAsync(clientId, jobId, result, percentage, exception, finished);
    }  
    #endregion  */

    public ResponseResultReceived SendStoredJobResultsSync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {      
      return proxy.StoreFinishedJobResult(clientId, jobId, result, percentage, exception);    
    }

    public ResponseResultReceived ProcessSnapshotSync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      try {
        ResponseResultReceived res = proxy.ProcessSnapshot(clientId, jobId, result, percentage, null);
        Logging.Instance.Info(this.ToString(), "Snapshot for Job " + jobId + " submitted");
        return res;
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

    public List<CachedHivePluginInfo> RequestPlugins(List<HivePluginInfo> requestedPlugins) {
      try {
        ResponsePlugin response = proxy.SendPlugins(requestedPlugins.ToArray());
        return response.Plugins;        
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

    public void Logout(Guid guid) {
      try {
        proxy.Logout(guid);
      }
      catch (Exception e) {
        HandleNetworkError(e);
      }
    }
  }
}
