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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HeuristicLab.Tracing;

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
          Logger.Debug("New WcfService Instance created");
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

    public ClientFacadeClient proxy = null;

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
        Logger.Debug("Starting the Connection Process");
        if (String.Empty.Equals(ServerIP) || ServerPort == 0) {
          Logger.Info("No Server IP or Port set!");
          return;
        }

        Logger.Debug("Creating the new connection proxy");
        proxy = new ClientFacadeClient(
          WcfSettings.GetStreamedBinding(),
          new EndpointAddress("net.tcp://" + ServerIP + ":" + ServerPort + "/HiveServer/ClientCommunicator")
        );
        Logger.Debug("Created the new connection proxy");

        Logger.Debug("Registring new Events");
        proxy.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(proxy_LoginCompleted);
        proxy.SendStreamedJobCompleted += new EventHandler<SendStreamedJobCompletedEventArgs>(proxy_SendStreamedJobCompleted);
        proxy.StoreFinishedJobResultStreamedCompleted += new EventHandler<StoreFinishedJobResultStreamedCompletedEventArgs>(proxy_StoreFinishedJobResultStreamedCompleted);
        proxy.ProcessSnapshotStreamedCompleted += new EventHandler<ProcessSnapshotStreamedCompletedEventArgs>(proxy_ProcessSnapshotStreamedCompleted);
        proxy.ProcessHeartBeatCompleted += new EventHandler<ProcessHeartBeatCompletedEventArgs>(proxy_ProcessHeartBeatCompleted);
        Logger.Debug("Registered new Events");
        Logger.Debug("Opening the Connection");
        proxy.Open();
        Logger.Debug("Opened the Connection");

        ConnState = NetworkEnum.WcfConnState.Connected;
        ConnectedSince = DateTime.Now;

        if (Connected != null) {
          Logger.Debug("Calling the connected Event");
          Connected(this, new EventArgs());
          //Todo: This won't be hit. EVER        
        }
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
      Logger.Debug("Called Connected with " + serverIP + ":" + serverPort);
      String oldIp = this.ServerIP;
      int oldPort = this.ServerPort;
      this.ServerIP = serverIP;
      this.ServerPort = serverPort;      
      Connect();
      if (oldIp != serverIP || oldPort != ServerPort)
        if(ServerChanged != null) 
          ServerChanged(this, new EventArgs());
    }

    public void SetIPAndPort(String serverIP, int serverPort) {
      Logger.Debug("Called with " + serverIP + ":" + serverPort);
      this.ServerIP = serverIP;
      this.ServerPort = serverPort;
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
      Logger.Error("Network exception occurred: " + e);
    }

    

    /// <summary>
    /// Methods for the Server Login
    /// </summary>
    #region Login
    public event System.EventHandler<LoginCompletedEventArgs> LoginCompleted;
    public void LoginAsync(ClientDto clientInfo) {
      if (ConnState == NetworkEnum.WcfConnState.Connected) {
        Logger.Debug("STARTED: Login Async");
        proxy.LoginAsync(clientInfo);
      }
    }
    private void proxy_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Error == null) {
        Logger.Debug("ENDED: Login Async");
        LoginCompleted(sender, e);
      } else
        HandleNetworkError(e.Error.InnerException);
    }

    public void LoginSync(ClientDto clientInfo) {
      try {
        if (ConnState == NetworkEnum.WcfConnState.Connected) {
          Logger.Debug("STARTED: Login Sync");
          Response res = proxy.Login(clientInfo);
          if (!res.Success) {
            Logger.Error("FAILED: Login Failed! " + res.StatusMessage);
            throw new Exception(res.StatusMessage);
          } else {
            Logger.Info("ENDED: Login succeeded" + res.StatusMessage);
            ConnState = NetworkEnum.WcfConnState.Loggedin;            
          }
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
      if (ConnState == NetworkEnum.WcfConnState.Loggedin) {
        Logger.Debug("STARTED: Fetching of Jobs from Server for Client");
        proxy.SendStreamedJobAsync(guid);
      }
    }

    void proxy_SendStreamedJobCompleted(object sender, SendStreamedJobCompletedEventArgs e) {
      if (e.Error == null) {
        Logger.Debug("ENDED: Fetching of Jobs from Server for Client");
        Stream stream = null;
        MemoryStream memStream = null;

        try {
          stream = (Stream)e.Result;

          //first deserialize the response
          BinaryFormatter formatter =
            new BinaryFormatter();
          ResponseJob response =
            (ResponseJob)formatter.Deserialize(stream);

          //second deserialize the BLOB
          memStream = new MemoryStream();

          byte[] buffer = new byte[3024];
          int read = 0;
          while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
            memStream.Write(buffer, 0, read);
          }

          memStream.Close();

          SendJobCompletedEventArgs completedEventArgs =
            new SendJobCompletedEventArgs(new object[] { response, memStream.GetBuffer() }, e.Error, e.Cancelled, e.UserState);
          SendJobCompleted(sender, completedEventArgs);
        } catch (Exception ex) {
          Logger.Error(ex);
        } finally {
          if(stream != null)
            stream.Dispose();

          if (memStream != null)
            memStream.Dispose();
        }
      } else
        HandleNetworkError(e.Error);
    }

    #endregion

    /// <summary>
    /// Send back finished Job Results
    /// </summary>
    #region SendJobResults
    public event System.EventHandler<StoreFinishedJobResultCompletedEventArgs> StoreFinishedJobResultCompleted;
    public void StoreFinishedJobResultAsync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin) {
        Logger.Debug("STARTED: Sending back the finished job results");
        Logger.Debug("Building stream");
        Stream stream =
          GetStreamedJobResult(clientId, jobId, result, percentage, exception);
        Logger.Debug("Builded stream");
        Logger.Debug("Making the call");
        proxy.StoreFinishedJobResultStreamedAsync(stream, stream);
      }
     }
    private void proxy_StoreFinishedJobResultStreamedCompleted(object sender, StoreFinishedJobResultStreamedCompletedEventArgs e) {
      Logger.Debug("Finished storing the job");
      Stream stream =
        (Stream)e.UserState;
      if (stream != null) {
        Logger.Debug("Stream not null, disposing it");
        stream.Dispose();
      }
      if (e.Error == null) {
        StoreFinishedJobResultCompletedEventArgs args =
          new StoreFinishedJobResultCompletedEventArgs(
            new object[] { e.Result }, e.Error, e.Cancelled, e.UserState);
        Logger.Debug("calling the Finished Job Event");
        StoreFinishedJobResultCompleted(sender, args);
        Logger.Debug("ENDED: Sending back the finished job results");
      } else
        HandleNetworkError(e.Error);
    }

    #endregion

    #region Processsnapshots
    public event System.EventHandler<ProcessSnapshotCompletedEventArgs> ProcessSnapshotCompleted;
    public void ProcessSnapshotAsync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {
      if (ConnState == NetworkEnum.WcfConnState.Loggedin) {
        Stream stream = GetStreamedJobResult(
            clientId, jobId, result, percentage, exception);

        proxy.ProcessSnapshotStreamedAsync(stream, stream);
      }
    }
    void proxy_ProcessSnapshotStreamedCompleted(object sender, ProcessSnapshotStreamedCompletedEventArgs e) {
      Stream stream =
        (Stream)e.UserState;
      if (stream != null)
        stream.Dispose();
      
      if (e.Error == null) {
        ProcessSnapshotCompletedEventArgs args =
          new ProcessSnapshotCompletedEventArgs(
            new object[] { e.Result }, e.Error, e.Cancelled, e.UserState);

        ProcessSnapshotCompleted(sender, args);
      } else
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
        Logger.Debug("STARTING: sending heartbeat");
        proxy.ProcessHeartBeatAsync(hbd);
    }

    private void proxy_ProcessHeartBeatCompleted(object sender, ProcessHeartBeatCompletedEventArgs e) {
      if (e.Error == null && e.Result.Success == true) {
        SendHeartBeatCompleted(sender, e);
        Logger.Debug("ENDED: sending heartbeats");
      } else {
        try {
          Logger.Error("Error: " + e.Result.StatusMessage);
        }
        catch (Exception ex) {
          Logger.Error("Error: ", ex);
        }
        HandleNetworkError(e.Error);
      }
    }

    #endregion  

    /// <summary>
    /// Send back finished and Stored Job Results
    /// </summary>
    private Stream GetStreamedJobResult(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      JobResult jobResult =
          new JobResult();
      jobResult.ClientId = clientId;
      jobResult.JobId = jobId;
      jobResult.Percentage = percentage;
      jobResult.Exception = exception;

      MultiStream stream =
              new MultiStream();

      //first send result
      stream.AddStream(
        new StreamedObject<JobResult>(jobResult));

      //second stream the job binary data
      MemoryStream memStream =
        new MemoryStream(result, false);
      stream.AddStream(memStream);

      return stream;
    }

    public ResponseResultReceived SendStoredJobResultsSync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception, bool finished) {      
      return proxy.StoreFinishedJobResultStreamed(
        GetStreamedJobResult(clientId, jobId, result, percentage, exception));
    }

    public Response IsJobStillNeeded(Guid jobId) {
      try {
        Logger.Debug("STARTING: Sync call: IsJobStillNeeded");
        Response res = proxy.IsJobStillNeeded(jobId);
        Logger.Debug("ENDED: Sync call: IsJobStillNeeded");
        return res;
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
      
    }

    public ResponseResultReceived ProcessSnapshotSync(Guid clientId, Guid jobId, byte[] result, double percentage, Exception exception) {
      try {        
        return proxy.ProcessSnapshotStreamed(
          GetStreamedJobResult(clientId, jobId, result, percentage, exception));
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

    public List<CachedHivePluginInfoDto> RequestPlugins(List<HivePluginInfoDto> requestedPlugins) {
      try {
        Logger.Debug("STARTED: Requesting Plugins for Job");
        Logger.Debug("STARTED: Getting the stream");
        Stream stream = proxy.SendStreamedPlugins(requestedPlugins.ToArray());
        Logger.Debug("ENDED: Getting the stream");
        BinaryFormatter formatter =
          new BinaryFormatter();
        Logger.Debug("STARTED: Deserializing the stream");
        ResponsePlugin response = (ResponsePlugin)formatter.Deserialize(stream);
        Logger.Debug("ENDED: Deserializing the stream");
        if (stream != null)
          stream.Dispose();        
        return response.Plugins;        
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

    public void Logout(Guid guid) {
      try {
        Logger.Debug("STARTED: Logout");
        proxy.Logout(guid);
        Logger.Debug("ENDED: Logout");
      }
      catch (Exception e) {
        HandleNetworkError(e);
      }
    }

    public ResponseCalendar GetCalendarSync(Guid clientId) {
      try {
        Logger.Debug("STARTED: Syncing Calendars");
        ResponseCalendar cal = proxy.GetCalendar(clientId);
        Logger.Debug("ENDED: Syncing Calendars");
        return cal;
      }
      catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

    public Response SetCalendarStatus (Guid clientId, CalendarState state) {
      try {
        Logger.Debug("STARTED: Setting Calendar status to: " + state);
        Response resp = proxy.SetCalendarStatus(clientId, state);
        Logger.Debug("ENDED: Setting Calendar status to: " + state);
        return resp;
      } catch (Exception e) {
        HandleNetworkError(e);
        return null;
      }
    }

  }
}
