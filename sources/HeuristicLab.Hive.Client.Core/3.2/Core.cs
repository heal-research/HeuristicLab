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

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// The core component of the Hive Client
  /// </summary>
  public class Core: MarshalByRefObject {       
    public static bool abortRequested { get; set; }
    private bool currentlyFetching = false;

    private Dictionary<Guid, Executor> engines = new Dictionary<Guid, Executor>();
    private Dictionary<Guid, AppDomain> appDomains = new Dictionary<Guid, AppDomain>();
    private Dictionary<Guid, Job> jobs = new Dictionary<Guid, Job>();

    private WcfService wcfService;
    private Heartbeat beat;
    
    /// <summary>
    /// Main Method for the client
    /// </summary>
    public void Start() {      
      abortRequested = false;
      PluginManager.Manager.Initialize();
      Logging.Instance.Info(this.ToString(), "Hive Client started");
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

      if (UptimeManager.Instance.isOnline())
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
        Debug.WriteLine("Main loop received this message: " + container.Message.ToString());
        Logging.Instance.Info(this.ToString(), container.Message.ToString());
        DetermineAction(container);
      }
      Console.WriteLine("ended!");
    }    

    /// <summary>
    /// Reads and analyzes the Messages from the MessageQueue and starts corresponding actions
    /// </summary>
    /// <param name="container">The Container, containing the message</param>
    private void DetermineAction(MessageContainer container) {           
      switch (container.Message) {
        //Server requests to abort a job
        case MessageContainer.MessageType.AbortJob:
          if(engines.ContainsKey(container.JobId))
            engines[container.JobId].Abort();
          else
            Logging.Instance.Error(this.ToString(), "AbortJob: Engine doesn't exist");
          break;
        //Job has been successfully aborted


        case MessageContainer.MessageType.JobAborted:          
        //todo: thread this
          Debug.WriteLine("Job aborted, he's dead");
          lock (engines) {            
            Guid jobId = new Guid(container.JobId.ToString());
            if(engines.ContainsKey(jobId)) {
              appDomains[jobId].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
              AppDomain.Unload(appDomains[jobId]);
              appDomains.Remove(jobId);
              engines.Remove(jobId);
              jobs.Remove(jobId);
              GC.Collect();
            } else
              Logging.Instance.Error(this.ToString(), "JobAbort: Engine doesn't exist");
          }
          break;
        
        
        //Request a Snapshot from the Execution Engine
        case MessageContainer.MessageType.RequestSnapshot:
          if (engines.ContainsKey(container.JobId)) 
            engines[container.JobId].RequestSnapshot();
          else
            Logging.Instance.Error(this.ToString(), "RequestSnapshot: Engine doesn't exist");
          break;
        
        
        //Snapshot is ready and can be sent back to the Server
        case MessageContainer.MessageType.SnapshotReady:
          ThreadPool.QueueUserWorkItem(new WaitCallback(GetSnapshot), container.JobId);          
          break;
        
        
        //Pull a Job from the Server
        case MessageContainer.MessageType.FetchJob:
          if (!currentlyFetching) {
            wcfService.SendJobAsync(ConfigManager.Instance.GetClientInfo().Id);
            currentlyFetching = true;
          }          
          break;          
        
        
        //A Job has finished and can be sent back to the server
        case MessageContainer.MessageType.FinishedJob:
          ThreadPool.QueueUserWorkItem(new WaitCallback(GetFinishedJob), container.JobId);          
          break;     
        
        
        case MessageContainer.MessageType.UptimeLimitDisconnect:
          Logging.Instance.Info(this.ToString(), "Uptime Limit reached, storing jobs and sending them back");
          WcfService.Instance.Disconnect();
          break;
        
        
        //Hard shutdown of the client
        case MessageContainer.MessageType.Shutdown:
          lock (engines) {
            foreach (KeyValuePair<Guid, AppDomain> kvp in appDomains) {
              appDomains[kvp.Key].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
              AppDomain.Unload(kvp.Value);
            }
          }
          abortRequested = true;
          beat.StopHeartBeat();
          WcfService.Instance.Logout(ConfigManager.Instance.GetClientInfo().Id);
          break;
      }
    }

    //Asynchronous Threads for interaction with the Execution Engine
    #region Async Threads for the EE
    
    private void GetFinishedJob(object jobId) {
      Guid jId = (Guid)jobId;      
      try {
        if (!engines.ContainsKey(jId)) {
          Logging.Instance.Error(this.ToString(), "GetFinishedJob: Engine doesn't exist");
          return;
        }
        
        byte[] sJob = engines[jId].GetFinishedJob();

        if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Loggedin) {
          wcfService.StoreFinishedJobResultAsync(ConfigManager.Instance.GetClientInfo().Id,
            jId,
            sJob,
            1,
            null,
            true);
        } else {
          JobStorageManager.PersistObjectToDisc(wcfService.ServerIP, wcfService.ServerPort, jId, sJob);
          lock (engines) {
            appDomains[jId].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
            AppDomain.Unload(appDomains[jId]);
            appDomains.Remove(jId);
            engines.Remove(jId);
            jobs.Remove(jId);
          }
        }
      }
      catch (InvalidStateException ise) {
        Logging.Instance.Error(this.ToString(), "Exception: ", ise);
      }
    }

    private void GetSnapshot(object jobId) {
      Guid jId = (Guid)jobId;
      byte[] obj = engines[jId].GetSnapshot();
      wcfService.ProcessSnapshotSync(ConfigManager.Instance.GetClientInfo().Id,
        jId,
        obj,
        engines[jId].Progress,
        null);
      engines[jId].StartOnlyJob();
    }

    #endregion

    //Eventhandlers for the communication with the wcf Layer 
    #region wcfService Events

    void wcfService_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Result.Success) {
        Logging.Instance.Info(this.ToString(), "Login completed to Hive Server @ " + DateTime.Now);        
      } else
        Logging.Instance.Error(this.ToString(), e.Result.StatusMessage);
    }    

    void wcfService_SendJobCompleted(object sender, SendJobCompletedEventArgs e) {
      if (e.Result.StatusMessage != ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT) {        
        bool sandboxed = false;
        //todo: For testing!!!
        //beat.StopHeartBeat();        
        //Todo: make a set & override the equals method
        List<byte[]> files = new List<byte[]>();
        foreach (CachedHivePluginInfo plugininfo in PluginCache.Instance.GetPlugins(e.Result.Job.PluginsNeeded))
          files.AddRange(plugininfo.PluginFiles);
        
        AppDomain appDomain = PluginManager.Manager.CreateAndInitAppDomainWithSandbox(e.Result.Job.Id.ToString(), sandboxed, null, files);
        appDomain.UnhandledException += new UnhandledExceptionEventHandler(appDomain_UnhandledException);
        lock (engines) {                    
          if (!jobs.ContainsKey(e.Result.Job.Id)) {
            jobs.Add(e.Result.Job.Id, e.Result.Job);
            appDomains.Add(e.Result.Job.Id, appDomain);

            Executor engine = (Executor)appDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.GetName().Name, typeof(Executor).FullName);
            engine.JobId = e.Result.Job.Id;
            engine.Queue = MessageQueue.GetInstance();            
            engine.Start(e.Result.Job.SerializedJob);
            engines.Add(e.Result.Job.Id, engine);

            ClientStatusInfo.JobsFetched++;

            Debug.WriteLine("Increment FetchedJobs to:" + ClientStatusInfo.JobsFetched);
          }
        }        
      }
      currentlyFetching = false;
    }

    void wcfService_StoreFinishedJobResultCompleted(object sender, StoreFinishedJobResultCompletedEventArgs e) {
      lock(engines) {
        try {
          appDomains[e.Result.JobId].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
          AppDomain.Unload(appDomains[e.Result.JobId]);
          appDomains.Remove(e.Result.JobId);
          engines.Remove(e.Result.JobId);
          jobs.Remove(e.Result.JobId);
        }
        catch (Exception ex) {
          Logging.Instance.Error(this.ToString(), "Exception when unloading the appdomain: ", ex);
        }
      }
      if (e.Result.Success) {        
      
        //if the engine is running again -> we sent an snapshot. Otherwise the job was finished
        //this method has a risk concerning race conditions.
        //better expand the sendjobresultcompltedeventargs with a boolean "snapshot?" flag

        ClientStatusInfo.JobsProcessed++;
        Debug.WriteLine("ProcessedJobs to:" + ClientStatusInfo.JobsProcessed);                
      } else {        
        Logging.Instance.Error(this.ToString(), "Sending of job " + e.Result.JobId + " failed, job has been wasted. Message: " + e.Result.StatusMessage);
      }
    }

    void wcfService_ProcessSnapshotCompleted(object sender, ProcessSnapshotCompletedEventArgs e) {
      Logging.Instance.Info(this.ToString(), "Snapshot " + e.Result.JobId + " has been transmitted according to plan.");
    }

    //Todo: First stop all threads, then terminate
    void wcfService_ServerChanged(object sender, EventArgs e) {
      Logging.Instance.Info(this.ToString(), "ServerChanged has been called");
      lock (engines) {
        foreach (KeyValuePair<Guid, AppDomain> entries in appDomains) {
          appDomains[entries.Key].UnhandledException -= new UnhandledExceptionEventHandler(appDomain_UnhandledException);
          AppDomain.Unload(appDomains[entries.Key]);
        }
        appDomains = new Dictionary<Guid, AppDomain>();
        engines = new Dictionary<Guid, Executor>();
      }
    }

    void wcfService_Connected(object sender, EventArgs e) {
      wcfService.LoginSync(ConfigManager.Instance.GetClientInfo());
      JobStorageManager.CheckAndSubmitJobsFromDisc();
    }

    //this is a little bit tricky - 
    void wcfService_ConnectionRestored(object sender, EventArgs e) {
      Logging.Instance.Info(this.ToString(), "Reconnected to old server - checking currently running appdomains");                 

      foreach (KeyValuePair<Guid, Executor> execKVP in engines) {
        if (!execKVP.Value.Running && execKVP.Value.CurrentMessage == MessageContainer.MessageType.NoMessage) {
          Logging.Instance.Info(this.ToString(), "Checking for JobId: " + execKVP.Value.JobId);
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
      Logging.Instance.Error(this.ToString(), "Exception in AppDomain: " + e.ExceptionObject.ToString());      
    }

    internal Dictionary<Guid, Job> GetJobs() {           
      return jobs;
    }
  }
}
