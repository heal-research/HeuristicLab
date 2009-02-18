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
using HeuristicLab.Hive.Client.Core.JobStorrage;


namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// The core component of the Hive Client
  /// </summary>
  public class Core: MarshalByRefObject {
    public delegate string GetASnapshotDelegate();

    public static Object Locker { get; set; }

    public static bool ShutdownFlag { get; set; }

    Dictionary<long, Executor> engines = new Dictionary<long, Executor>();
    Dictionary<long, AppDomain> appDomains = new Dictionary<long, AppDomain>();
    Dictionary<long, Job> jobs = new Dictionary<long, Job>();

    private WcfService wcfService;
    private Heartbeat beat;
    
    /// <summary>
    /// Main Method for the client
    /// </summary>
    public void Start() {
      Core.Locker = new Object();
      ShutdownFlag = false;

      Logging.GetInstance().Info(this.ToString(), "Hive Client started");
      ClientConsoleServer server = new ClientConsoleServer();
      server.StartClientConsoleServer(new Uri("net.tcp://127.0.0.1:8000/ClientConsole/"));

      ConfigManager manager = ConfigManager.Instance;
      manager.Core = this;
      
      //Register all Wcf Service references
      wcfService = WcfService.Instance;
      wcfService.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(wcfService_LoginCompleted);
      wcfService.PullJobCompleted += new EventHandler<PullJobCompletedEventArgs>(wcfService_PullJobCompleted);
      wcfService.SendJobResultCompleted += new EventHandler<SendJobResultCompletedEventArgs>(wcfService_SendJobResultCompleted);
      wcfService.ConnectionRestored += new EventHandler(wcfService_ConnectionRestored);
      wcfService.ServerChanged += new EventHandler(wcfService_ServerChanged);
      wcfService.Connected += new EventHandler(wcfService_Connected);
      //Recover Server IP and Port from the Settings Framework
      ConnectionContainer cc = ConfigManager.Instance.GetServerIPAndPort();     
      if (cc.IPAdress != String.Empty && cc.Port != 0) {
        wcfService.Connect(cc.IPAdress, cc.Port);
      }
   
      //Initialize the heartbeat
      beat = new Heartbeat { Interval = 10000 };
      beat.StartHeartbeat();     

      MessageQueue queue = MessageQueue.GetInstance();
      
      //Main processing loop
      while (!ShutdownFlag) {
        MessageContainer container = queue.GetMessage();
        Debug.WriteLine("Main loop received this message: " + container.Message.ToString());
        Logging.GetInstance().Info(this.ToString(), container.Message.ToString());
        DetermineAction(container);
      }
    }

    /// <summary>
    /// Reads and analyzes the Messages from the MessageQueue and starts corresponding actions
    /// </summary>
    /// <param name="container">The Container, containing the message</param>
    private void DetermineAction(MessageContainer container) {
      switch (container.Message) {
        //Server requests to abort a job
        case MessageContainer.MessageType.AbortJob:
          engines[container.JobId].Abort();
          break;
        //Job has been successfully aborted
        case MessageContainer.MessageType.JobAborted:
          Debug.WriteLine("-- Job Aborted Message received");
          break;
        //Request a Snapshot from the Execution Engine
        case MessageContainer.MessageType.RequestSnapshot:
          engines[container.JobId].RequestSnapshot();
          break;
        //Snapshot is ready and can be sent back to the Server
        case MessageContainer.MessageType.SnapshotReady:
          Thread ssr = new Thread(new ParameterizedThreadStart(GetSnapshot));
          ssr.Start(container.JobId);          
          break;
        //Pull a Job from the Server
        case MessageContainer.MessageType.FetchJob: 
          wcfService.PullJobAsync(ConfigManager.Instance.GetClientInfo().ClientId);
          break;          
        //A Job has finished and can be sent back to the server
        case MessageContainer.MessageType.FinishedJob:
          Thread finThread = new Thread(new ParameterizedThreadStart(GetFinishedJob));
          finThread.Start(container.JobId);          
          break;     
        //Hard shutdown of the client
        case MessageContainer.MessageType.Shutdown:
          ShutdownFlag = true;
          beat.StopHeartBeat();
          break;
      }
    }

    //Asynchronous Threads for interaction with the Execution Engine
    #region Async Threads for the EE
    
    private void GetFinishedJob(object jobId) {
      long jId = (long)jobId;
      byte[] sJob = engines[jId].GetFinishedJob();

      if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Connected) {
        wcfService.SendJobResultAsync(ConfigManager.Instance.GetClientInfo().ClientId,
          jId,
          sJob,
          1,
          null,
          true);
      } else {
        JobStorrageManager.PersistObjectToDisc(wcfService.ServerIP, wcfService.ServerPort, jId, sJob);
        AppDomain.Unload(appDomains[jId]);
        appDomains.Remove(jId);
        engines.Remove(jId);
        jobs.Remove(jId);
      }
    }

    private void GetSnapshot(object jobId) {
      long jId = (long)jobId;
      byte[] obj = engines[jId].GetSnapshot();
      wcfService.SendJobResultAsync(ConfigManager.Instance.GetClientInfo().ClientId,
        jId,
        obj,
        engines[jId].Progress,
        null,
        false);
    }

    #endregion

    //Eventhandlers for the communication with the wcf Layer 
    #region wcfService Events

    void wcfService_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Result.Success) {
        Logging.GetInstance().Info(this.ToString(), "Login completed to Hive Server @ " + DateTime.Now);        
      } else
        Logging.GetInstance().Error(this.ToString(), e.Result.StatusMessage);
    }    

    void wcfService_PullJobCompleted(object sender, PullJobCompletedEventArgs e) {
      if (e.Result.StatusMessage != ApplicationConstants.RESPONSE_COMMUNICATOR_NO_JOBS_LEFT) {
        bool sandboxed = true;

        PluginManager.Manager.Initialize();
        AppDomain appDomain = PluginManager.Manager.CreateAndInitAppDomainWithSandbox(e.Result.Job.Id.ToString(), sandboxed, typeof(TestJob));
        appDomain.UnhandledException += new UnhandledExceptionEventHandler(appDomain_UnhandledException);
        lock (Locker) {                    
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
    }

    void wcfService_SendJobResultCompleted(object sender, SendJobResultCompletedEventArgs e) {
      if (e.Result.Success) {        
        lock (Locker) {
          //if the engine is running again -> we sent an snapshot. Otherwise the job was finished
          //this method has a risk concerning race conditions.
          //better expand the sendjobresultcompltedeventargs with a boolean "snapshot?" flag
          if (e.Result.finished == false) {
            Logging.GetInstance().Info(this.ToString(), "Snapshot for Job " + e.Result.JobId + " transmitted");
          } else {
            AppDomain.Unload(appDomains[e.Result.JobId]);
            appDomains.Remove(e.Result.JobId);
            engines.Remove(e.Result.JobId);
            jobs.Remove(e.Result.JobId);
            ClientStatusInfo.JobsProcessed++;
            Debug.WriteLine("ProcessedJobs to:" + ClientStatusInfo.JobsProcessed);
          }
        }        
      } else {
        Logging.GetInstance().Error(this.ToString(), "Sending of job " + e.Result.JobId + " failed");
      }
    }

    void wcfService_ServerChanged(object sender, EventArgs e) {
      Logging.GetInstance().Info(this.ToString(), "ServerChanged has been called");
      lock (Locker) {
        foreach (KeyValuePair<long, AppDomain> entries in appDomains)
          AppDomain.Unload(appDomains[entries.Key]);
        appDomains = new Dictionary<long, AppDomain>();
        engines = new Dictionary<long, Executor>();
      }
    }

    void wcfService_Connected(object sender, EventArgs e) {
      wcfService.LoginSync(ConfigManager.Instance.GetClientInfo());
    }

    //this is a little bit tricky - 
    void wcfService_ConnectionRestored(object sender, EventArgs e) {
      Logging.GetInstance().Info(this.ToString(), "Reconnected to old server - checking currently running appdomains");                 

      foreach (KeyValuePair<long, Executor> execKVP in engines) {
        if (!execKVP.Value.Running && execKVP.Value.CurrentMessage == MessageContainer.MessageType.NoMessage) {
          Logging.GetInstance().Info(this.ToString(), "Checking for JobId: " + execKVP.Value.JobId);
          Thread finThread = new Thread(new ParameterizedThreadStart(GetFinishedJob));
          finThread.Start(execKVP.Value.JobId);
        }
      }
    }

    #endregion

    public Dictionary<long, Executor> GetExecutionEngines() {
      return engines;
    }

    void appDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
      Logging.GetInstance().Error(this.ToString(), " Exception: " + e.ExceptionObject.ToString());
    }
  }
}
