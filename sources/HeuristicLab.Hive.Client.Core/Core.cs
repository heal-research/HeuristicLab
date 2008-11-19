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


namespace HeuristicLab.Hive.Client.Core {
  public class Core {

    Dictionary<long, Executor> engines = new Dictionary<long, Executor>();
    Dictionary<long, AppDomain> appDomains = new Dictionary<long, AppDomain>();

    public static StrongName CreateStrongName(Assembly assembly) {
      if (assembly == null)
        throw new ArgumentNullException("assembly");

      AssemblyName assemblyName = assembly.GetName();
      Debug.Assert(assemblyName != null, "Could not get assembly name");

      // get the public key blob
      byte[] publicKey = assemblyName.GetPublicKey();
      if (publicKey == null || publicKey.Length == 0)
        throw new InvalidOperationException("Assembly is not strongly named");

      StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

      // and create the StrongName
      return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
    }

    public void Start() {      
      Heartbeat beat = new Heartbeat();
      beat.Interval = 5000;
      beat.StartHeartbeat();

      MessageQueue queue = MessageQueue.GetInstance();
      queue.AddMessage(MessageContainer.MessageType.FetchJob);     
      while (true) {
        MessageContainer container = queue.GetMessage();
        Debug.WriteLine("Main loop received this message: " + container.Message.ToString());
        Logging.getInstance().Info(this.ToString(), container.Message.ToString()); 
        DetermineAction(container);
      }
    }

    private AppDomain CreateNewAppDomain(bool sandboxed) {
      PermissionSet pset;
      if (sandboxed) {
        pset = new PermissionSet(PermissionState.None);
        pset.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
      } else {
        pset = new PermissionSet(PermissionState.Unrestricted);
      }
      AppDomainSetup setup = new AppDomainSetup();
      setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
      //Temp Fix!
      setup.PrivateBinPath = "plugins";
      return System.AppDomain.CreateDomain("appD", AppDomain.CurrentDomain.Evidence, setup, pset, CreateStrongName(Assembly.GetExecutingAssembly())); 

    }

    private void DetermineAction(MessageContainer container) {
      switch (container.Message) {
        case MessageContainer.MessageType.AbortJob:
          engines[container.JobId].Abort();
          break;
        case MessageContainer.MessageType.JobAborted:
          Debug.WriteLine("-- Job Aborted Message received");
          break;
        
        
        case MessageContainer.MessageType.RequestSnapshot:
          engines[container.JobId].RequestSnapshot();
          break;
        case MessageContainer.MessageType.SnapshotReady:
          engines[container.JobId].GetSnapshot();
          break;
        
        
        case MessageContainer.MessageType.FetchJob:
          IJob job = CreateNewJob();
          
          AppDomain appDomain = CreateNewAppDomain(false);
          appDomains.Add(job.JobId, appDomain);
          
          Executor engine = (Executor)appDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.GetName().Name, typeof(Executor).FullName);
          engine.Job = job;
          engine.JobId = job.JobId;
          engine.Queue = MessageQueue.GetInstance();
          engine.Start();
          engines.Add(engine.JobId, engine);
          break;

        case MessageContainer.MessageType.FinishedJob:
          engines[container.JobId].GetFinishedJob();
          AppDomain.Unload(appDomains[container.JobId]);
          appDomains.Remove(container.JobId);
          engines.Remove(container.JobId);
          break;

      }
    }
    
    /// <summary>
    /// Simulator Class for new Jobs. will be replaced with fetching Jobs from the Interface
    /// </summary>
    /// <returns></returns>
    private IJob CreateNewJob() {
      Random random = new Random();
      IJob job = new TestJob();
      job.JobId = random.Next();
      return job;
    }
  }
}
