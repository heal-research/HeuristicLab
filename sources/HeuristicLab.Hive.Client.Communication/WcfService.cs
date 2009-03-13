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
          new NetTcpBinding(),
          new EndpointAddress("net.tcp://" + ServerIP + ":" + ServerPort + "/HiveServer/ClientCommunicator")
        );

        proxy.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(proxy_LoginCompleted);
        proxy.PullJobCompleted += new EventHandler<PullJobCompletedEventArgs>(proxy_PullJobCompleted);
        proxy.SendJobResultCompleted += new EventHandler<SendJobResultCompletedEventArgs>(proxy_SendJobResultCompleted);
        proxy.SendHeartBeatCompleted += new EventHandler<SendHeartBeatCompletedEventArgs>(proxy_SendHeartBeatCompleted);
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
        //Todo: Rename to HandleNetworkError
        NetworkErrorHandling(ex);
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
    private void NetworkErrorHandling(Exception e) {
      ConnState = NetworkEnum.WcfConnState.Failed;
      Logging.GetInstance().Error(this.ToString(), "exception: ", e);
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
        NetworkErrorHandling(e.Error.InnerException);
    }

    public void LoginSync(ClientInfo clientInfo) {
      try {
        if (ConnState == NetworkEnum.WcfConnState.Connected) {
          Response res = proxy.Login(clientInfo);
          ConnState = NetworkEnum.WcfConnState.Loggedin;
          Logging.GetInstance().Info(this.ToString(), res.StatusMessage);
        }
      }
      catch (Exception e) {
        NetworkErrorHandling(e);
      }
    }

    #endregion

    /// <summary>
    /// Pull a Job from the Server
    /// </summary>
    #region PullJob
    public event System.EventHandler<PullJobCompletedEventArgs> PullJobCompleted;
    public void PullJobAsync(Guid guid) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)        
        proxy.PullJobAsync(guid);
    }
    void proxy_PullJobCompleted(object sender, PullJobCompletedEventArgs e) {
      if (e.Error == null)
        PullJobCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }
    #endregion

    /// <summary>
    /// Send back finished Job Results
    /// </summary>
    #region SendJobResults
    public event System.EventHandler<SendJobResultCompletedEventArgs> SendJobResultCompleted;
    public void SendJobResultAsync(Guid clientId, long jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        proxy.SendJobResultAsync(clientId, jobId, result, percentage, exception, finished);
    }
    private void proxy_SendJobResultCompleted(object sender, SendJobResultCompletedEventArgs e) {
      if (e.Error == null)
        SendJobResultCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }

    #endregion

    /// <summary>
    /// Methods for sending the periodically Heartbeat
    /// </summary>
    #region Heartbeat

    public event System.EventHandler<SendHeartBeatCompletedEventArgs> SendHeartBeatCompleted;
    public void SendHeartBeatAsync(HeartBeatData hbd) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        proxy.SendHeartBeatAsync(hbd);
    }

    private void proxy_SendHeartBeatCompleted(object sender, SendHeartBeatCompletedEventArgs e) {
      if (e.Error == null)
        SendHeartBeatCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }

    #endregion  

    /// <summary>
    /// Send back finished and Stored Job Results
    /// </summary>
    #region SendJobResults
    public event System.EventHandler<SendJobResultCompletedEventArgs> SendStoredJobResultCompleted;
    public void SendStoredJobResultAsync(Guid clientId, long jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin)
        //TODO: some sort of algo for the stored jobs
        proxy.SendJobResultAsync(clientId, jobId, result, percentage, exception, finished);
    }  
    #endregion

    public ResponseResultReceived SendStoredJobResultsSync(Guid clientId, long jobId, byte[] result, double percentage, Exception exception, bool finished) {
      return proxy.SendJobResult(clientId, jobId, result, percentage, exception, finished);
    }
  }
}
