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
using HeuristicLab.Hive.Client.ExecutionEngine;
using HeuristicLab.Hive.Client.Common;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using System.Runtime.Remoting.Messaging;
using HeuristicLab.PluginInfrastructure;
using System.ServiceModel;
using System.ServiceModel.Description;
using HeuristicLab.Hive.Client.Core.ClientConsoleService;
using HeuristicLab.Hive.Client.Core.ConfigurationManager;
using HeuristicLab.Hive.Client.Communication.ServerService;
using HeuristicLab.Hive.JobBase;
using HeuristicLab.Hive.Client.Core.JobStorage;
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// The core component of the Hive Client
  /// </summary>
  public class Core : MarshalByRefObject {
    public static bool abortRequested { get; set; }

    private bool _currentlyFetching;
    private bool CurrentlyFetching { 
      get {
        return _currentlyFetching;
      } set {        
        _currentlyFetching = value;
        Logger.Debug("Set CurrentlyFetching to " + _currentlyFetching);
      } 
    }

    private Dictionary<Guid, Executor> engines = new Dictionary<Guid, Executor>();
    private Dictionary<Guid, AppDomain> appDomains = new Dictionary<Guid, AppDomain>();
    private Dictionary<Guid, JobDto> jobs = new Dictionary<Guid, JobDto>();

    private WcfService wcfService;
    private Heartbeat beat;

    /// <summary>
    /// Main Method for the client
    /// </summary>
    public void Start() {
      abortRequested = false;
      Logger.Info("Hive Client started");
      ClientConsoleServer server = new ClientConsoleServer();
      server.StartClientConsoleServer(new Uri("net.tcp://127.0.0.1:8000/ClientConsole/"));

      ConfigManager manager = ConfigManager.Instance;
      manager.Core = this;



      //Register all Wcf Service references
      wcfService = WcfService.Instance;
      wcfService.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(wcfService_LoginCompleted);
      wcfService.SendJobCompleted += new EventHandler<SendJobCompletedEventArgs>(wcfService_SendJobCompleted);
      wcfService.StoreFinishedJobResultCompleted += new EventHandler<StoreFinishedJobResultCompletedEventArgs>(wcfService_StoreFinishedJobResultCompleted);
      wcfService.ProcessSnapshotCompleted += new EventHandler<ProcessSnapshotCompletedEventArgs>(wcfService_ProcessSnapshotCompleted);
      wcfService.ConnectionRestored += new EventHandler(wcfService_ConnectionRestored);
      wcfService.ServerChanged += new EventHandler(wcfService_ServerChanged);
      wcfService.Connected += new EventHandler(wcfService_Connected);
      //Recover Server IP and Port from the Settings Framework
      ConnectionContainer cc = ConfigManager.Instance.GetServerIPAndPort();
      if (cc.IPAdress != String.Empty && cc.Port != 0)
        wcfService.SetIPAndPort(cc.IPAdress, cc.Port);

      if (!UptimeManager.Instance.CalendarAvailable || UptimeManager.Instance.IsOnline())
        wcfService.Connect();

      //Initialize the heartbeat
      beat = new Heartbeat { Interval = 10000 };
      beat.StartHeartbeat();

      MessageQueue queue = MessageQueue.GetInstance();

      //Main processing loop     
      //Todo: own thread for message handling
      //Rly?!
      while (!abortRequested) {
        MessageContainer container = queue.GetMessage();        
        DetermineAction(container);
      }
      Logger.Info("Program shutdown");
    }

    /// <summary>
    /// Reads and analyzes the Messages from the MessageQueue and starts corresponding actions
    /// </summary>
    /// <param name="container">The Container, containing the message</param>
    private void DetermineAction(MessageContainer container) {
      Logger.Info("Message: " + container.Message.ToString() + " for job: " + container.JobId);        
      switch (container.Message) {
        //Server requests to abort a job
        case MessageContainer.MessageType.AbortJob:          
          if (engines.ContainsKey(container.JobId))
            engines[container.JobId].Abort();
          else
            Logger.Error("AbortJob: Engine doesn't exist");
          break;
        //Job has been successfully aborted


        case MessageContainer.MessageType.JobAborted:          
        //todo: thread this          
          lock (engines) {
            Guid jobId = new Guid(container.JobId.ToString());
            if (engines.ContainsKey(jobId)) {
              appDomains[jobId].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
              AppDomain.Unload(appDomains[jobId]);
              appDomains.Remove(jobId);
              engines.Remove(jobId);
              jobs.Remove(jobId);
              GC.Collect();
            } else
              Logger.Error("JobAbort: Engine doesn't exist");
          }
          break;


        //Request a Snapshot from the Execution Engine
        case MessageContainer.MessageType.RequestSnapshot:          
          if (engines.ContainsKey(container.JobId))
            engines[container.JobId].RequestSnapshot();
          else
            Logger.Error("RequestSnapshot: Engine doesn't exist");
          break;


        //Snapshot is ready and can be sent back to the Server
        case MessageContainer.MessageType.SnapshotReady:          
          ThreadPool.QueueUserWorkItem(new WaitCallback(GetSnapshot), container.JobId);
          break;


        //Pull a Job from the Server
        case MessageContainer.MessageType.FetchJob:          
          if (!CurrentlyFetching) {
            wcfService.SendJobAsync(ConfigManager.Instance.GetClientInfo().Id);
            CurrentlyFetching = true;
          } else
            Logger.Info("Currently fetching, won't fetch this time!");
          break;          
        
        
        //A Job has finished and can be sent back to the server
        case MessageContainer.MessageType.FinishedJob:          
          ThreadPool.QueueUserWorkItem(new WaitCallback(GetFinishedJob), container.JobId);
          break;


        //When the timeslice is up
        case MessageContainer.MessageType.UptimeLimitDisconnect:
          Logger.Info("Uptime Limit reached, storing jobs and sending them back");

          //check if there are running jobs
          if (engines.Count > 0) {
            //make sure there is no more fetching of jobs while the snapshots get processed
            CurrentlyFetching = true;
            //request a snapshot of each running job
            foreach (KeyValuePair<Guid, Executor> kvp in engines) {
              kvp.Value.RequestSnapshot();
            }

          } else {
            //Disconnect afterwards
            WcfService.Instance.Disconnect();
          }
          break;

          //Fetch or Force Fetch Calendar!
        case MessageContainer.MessageType.FetchOrForceFetchCalendar:
          Logger.Info("Fetch Calendar from Server");
          FetchCalendarFromServer();  
        break;

        //Hard shutdown of the client
        case MessageContainer.MessageType.Shutdown:
          Logger.Info("Shutdown Signal received");
          lock (engines) {
            Logger.Debug("engines locked");
            foreach (KeyValuePair<Guid, AppDomain> kvp in appDomains) {
              Logger.Debug("Shutting down Appdomain for " + kvp.Key);
              appDomains[kvp.Key].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
              AppDomain.Unload(kvp.Value);
            }
          }
          Logger.Debug("Stopping heartbeat");
          abortRequested = true;
          beat.StopHeartBeat();
          Logger.Debug("Logging out");
          WcfService.Instance.Logout(ConfigManager.Instance.GetClientInfo().Id);
          break;
      }
    }

    //Asynchronous Threads for interaction with the Execution Engine
    #region Async Threads for the EE

    /// <summary>
    /// serializes the finished job and submits it to the server. If, at the time, a network connection is unavailable, the Job gets stored on the disk. 
    /// once the connection gets reestablished, the job gets submitted
    /// </summary>
    /// <param name="jobId"></param>
    private void GetFinishedJob(object jobId) {
      Guid jId = (Guid)jobId;
      Logger.Info("Getting the finished job with id: " + jId);
      try {
        if (!engines.ContainsKey(jId)) {
          Logger.Info("Engine doesn't exist");
          return;
        }

        byte[] sJob = engines[jId].GetFinishedJob();

        if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Loggedin) {
          Logger.Info("Sending the finished job with id: " + jId);
          wcfService.StoreFinishedJobResultAsync(ConfigManager.Instance.GetClientInfo().Id,
            jId,
            sJob,
            1,
            null,
            true);
        } else {
          Logger.Info("Storing the finished job with id: " + jId + " to hdd");
          JobStorageManager.PersistObjectToDisc(wcfService.ServerIP, wcfService.ServerPort, jId, sJob);
          lock (engines) {
            appDomains[jId].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
            AppDomain.Unload(appDomains[jId]);
            Logger.Debug("Unloaded appdomain");
            appDomains.Remove(jId);            
            engines.Remove(jId);            
            jobs.Remove(jId);
            Logger.Debug("Removed job from appDomains, Engines and Jobs");
          }
        }
      }
      catch (InvalidStateException ise) {
        Logger.Error("Invalid State while Snapshoting:", ise);
      }
    }

    private void GetSnapshot(object jobId) {
      Logger.Info("Fetching a snapshot for job " + jobId);
      Guid jId = (Guid)jobId;
      byte[] obj = engines[jId].GetSnapshot();
      Logger.Debug("BEGIN: Sending snapshot sync");
      wcfService.ProcessSnapshotSync(ConfigManager.Instance.GetClientInfo().Id,
        jId,
        obj,
        engines[jId].Progress,
        null);
      Logger.Debug("END: Sended snapshot sync");
      //Uptime Limit reached, now is a good time to destroy this jobs.
      Logger.Debug("Checking if uptime limit is reached");
      if (!UptimeManager.Instance.IsOnline()) {
        Logger.Debug("Uptime limit reached");
        Logger.Debug("Killing Appdomain");
        KillAppDomain(jId);
        //Still anything running?  
        if (engines.Count == 0) {
          Logger.Info("All jobs snapshotted and sent back, disconnecting");          
          WcfService.Instance.Disconnect();
        } else {
          Logger.Debug("There are still active Jobs in the Field, not disconnecting");
        }

      } else {
        Logger.Debug("Restarting the job" + jobId);
        engines[jId].StartOnlyJob();
        Logger.Info("Restarted the job" + jobId);
      }
    }

    #endregion

    //Eventhandlers for the communication with the wcf Layer 
    #region wcfService Events
    /// <summary>
    /// Login has returned
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Result.Success) {
        CurrentlyFetching = false;
        Logger.Info("Login completed to Hive Server @ " + DateTime.Now);
      } else
        Logger.Error("Error during login: " + e.Result.StatusMessage);
    }

    /// <summary>
    /// A new Job from the wcfService has been received and will be started within a AppDomain.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_SendJobCompleted(object sender, SendJobCompletedEventArgs e) {
      if (e.Result.StatusMessage != ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT) {
        Logger.Info("Received new job with id " + e.Result.Job.Id);      
        bool sandboxed = false;
        List<byte[]> files = new List<byte[]>();
        Logger.Debug("Fetching plugins for job " + e.Result.Job.Id);
        foreach (CachedHivePluginInfoDto plugininfo in PluginCache.Instance.GetPlugins(e.Result.Job.PluginsNeeded))
          files.AddRange(plugininfo.PluginFiles);
        Logger.Debug("Plugins fetched for job " + e.Result.Job.Id);
        AppDomain appDomain = HeuristicLab.PluginInfrastructure.Sandboxing.SandboxManager.CreateAndInitSandbox(e.Result.Job.Id.ToString(), files);
        appDomain.UnhandledException += new UnhandledExceptionEventHandler(appDomain_UnhandledException);
        lock (engines) {
          if (!jobs.ContainsKey(e.Result.Job.Id)) {
            jobs.Add(e.Result.Job.Id, e.Result.Job);
            appDomains.Add(e.Result.Job.Id, appDomain);
            Logger.Debug("Creating AppDomain");
            Executor engine = (Executor)appDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.GetName().Name, typeof(Executor).FullName);
            Logger.Debug("Created AppDomain");
            engine.JobId = e.Result.Job.Id;
            engine.Queue = MessageQueue.GetInstance();
            Logger.Debug("Starting Engine for job " + e.Result.Job.Id);
            engine.Start(e.Data);
            engines.Add(e.Result.Job.Id, engine);

            ClientStatusInfo.JobsFetched++;

            Logger.Info("Increment FetchedJobs to:" + ClientStatusInfo.JobsFetched);
          }
        }
      } else
        Logger.Info("No more jobs left!");
      CurrentlyFetching = false;
    }

    /// <summary>
    /// A finished job has been stored on the server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_StoreFinishedJobResultCompleted(object sender, StoreFinishedJobResultCompletedEventArgs e) {
      Logger.Info("Job submitted with id " + e.Result.JobId);
      KillAppDomain(e.Result.JobId);
      if (e.Result.Success) {
        ClientStatusInfo.JobsProcessed++;
        Logger.Info("Increased ProcessedJobs to:" + ClientStatusInfo.JobsProcessed);
      } else {
        Logger.Error("Sending of job " + e.Result.JobId + " failed, job has been wasted. Message: " + e.Result.StatusMessage);
      }
    }

    /// <summary>
    /// A snapshot has been stored on the server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_ProcessSnapshotCompleted(object sender, ProcessSnapshotCompletedEventArgs e) {
      Logger.Info("Snapshot " + e.Result.JobId + " has been transmitted according to plan.");
    }

    /// <summary>
    /// The server has been changed. All Appdomains and Jobs must be aborted!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_ServerChanged(object sender, EventArgs e) {
      Logger.Info("ServerChanged has been called");
      lock (engines) {
        foreach (KeyValuePair<Guid, Executor> entries in engines) {
          engines[entries.Key].Abort();
          //appDomains[entries.Key].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
          //AppDomain.Unload(appDomains[entries.Key]);
        }
        //appDomains = new Dictionary<Guid, AppDomain>();
        //engines = new Dictionary<Guid, Executor>();
        //jobs = new Dictionary<Guid, Job>();
      }
    }

    /// <summary>
    /// Connnection to the server has been estabilshed => Login and Send the persistet Jobs from the harddisk.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void wcfService_Connected(object sender, EventArgs e) {
      Logger.Info("WCF Service got a connection");
      if (!UptimeManager.Instance.CalendarAvailable) {
        Logger.Info("No local calendar available, fetch it");
        FetchCalendarFromServer();
      }
      //if the fetching from the server failed - still set the client online... maybe we get 
      //a result within the next few heartbeats      
      if (!UptimeManager.Instance.CalendarAvailable || UptimeManager.Instance.IsOnline()) {
        Logger.Info("CalendarAvailable is " + UptimeManager.Instance.CalendarAvailable + " and IsOnline is: " + UptimeManager.Instance.IsOnline());
        Logger.Info("Setting client online");
        wcfService.LoginSync(ConfigManager.Instance.GetClientInfo());
        JobStorageManager.CheckAndSubmitJobsFromDisc();
        CurrentlyFetching = false;
      }
    }

    private void FetchCalendarFromServer() {
      ResponseCalendar calres = wcfService.GetCalendarSync(ConfigManager.Instance.GetClientInfo().Id);
      if(calres.Success) {
        if (UptimeManager.Instance.SetAppointments(false, calres)) {
          Logger.Info("Remote calendar installed");
          wcfService.SetCalendarStatus(ConfigManager.Instance.GetClientInfo().Id, CalendarState.Fetched);
        } else {
          Logger.Info("Remote calendar installation failed, setting state to " + CalendarState.NotAllowedToFetch);
          wcfService.SetCalendarStatus(ConfigManager.Instance.GetClientInfo().Id, CalendarState.NotAllowedToFetch);
        }
      } else {
        Logger.Info("Remote calendar installation failed, setting state to " + CalendarState.NotAllowedToFetch);
        wcfService.SetCalendarStatus(ConfigManager.Instance.GetClientInfo().Id, CalendarState.NotAllowedToFetch);
      }
    }

    //this is a little bit tricky - 
    void wcfService_ConnectionRestored(object sender, EventArgs e) {
      Logger.Info("Reconnected to old server - checking currently running appdomains");

      foreach (KeyValuePair<Guid, Executor> execKVP in engines) {
        if (!execKVP.Value.Running && execKVP.Value.CurrentMessage == MessageContainer.MessageType.NoMessage) {
          Logger.Info("Checking for JobId: " + execKVP.Value.JobId);
          Thread finThread = new Thread(new ParameterizedThreadStart(GetFinishedJob));
          finThread.Start(execKVP.Value.JobId);
        }
      }
    }

    #endregion

    public Dictionary<Guid, Executor> GetExecutionEngines() {
      return engines;
    }

    void appDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      Logger.Error("Exception in AppDomain: " + e.ExceptionObject.ToString());    
    }

    internal Dictionary<Guid, JobDto> GetJobs() {
      return jobs;
    }

    /// <summary>
    /// Kill a appdomain with a specific id.
    /// </summary>
    /// <param name="id">the GUID of the job</param>
    private void KillAppDomain(Guid id) {
      Logger.Debug("Shutting down Appdomain for Job " + id);
      lock (engines) {
        try {
          appDomains[id].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
          AppDomain.Unload(appDomains[id]);
          appDomains.Remove(id);
          engines.Remove(id);
          jobs.Remove(id);
        }
        catch (Exception ex) {
          Logger.Error("Exception when unloading the appdomain: ", ex);
        }
      }
      GC.Collect();
    }
  }
}
