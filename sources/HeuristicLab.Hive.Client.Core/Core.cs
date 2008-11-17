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
      Logging.getInstance().Info(this.ToString(), "Info Message");
      //Logging.getInstance().Error(this.ToString(), "Error Message");
      //Logging.getInstance().Error(this.ToString(), "Exception Message", new Exception("Exception"));      

      Heartbeat beat = new Heartbeat();
      beat.Interval = 5000;
      beat.StartHeartbeat();

      MessageQueue queue = MessageQueue.GetInstance();
      
      TestJob job = new TestJob();

      AppDomain appDomain = CreateNewAppDomain(false);
      
      //This is a HACK. remove static directory ASAP
      //Executor engine = (Executor)appDomain.CreateInstanceFromAndUnwrap(@"C:\Program Files\HeuristicLab 3.0\plugins\HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll", "HeuristicLab.Hive.Client.ExecutionEngine.Executor");
      
      Executor engine = (Executor)appDomain.CreateInstanceAndUnwrap(typeof(Executor).Assembly.GetName().Name, typeof(Executor).FullName);
      //ExecutionEngine.Executor engine = new ExecutionEngine.Executor();
      engine.Job = job;
      engine.JobId = 1L;
      engine.Queue = queue;      
      engine.Start();
      engines.Add(engine.JobId, engine);

      Thread.Sleep(15000);
      engine.RequestSnapshot();
      while (true) {
        MessageContainer container = queue.GetMessage();
        Logging.getInstance().Info(this.ToString(), container.Message.ToString()); 
        DetermineAction(container);

        
      }
    }

    Assembly appDomain_TypeResolve(object sender, ResolveEventArgs args) {
      throw new NotImplementedException();
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
      if(container.Message == MessageContainer.MessageType.AbortJob)
        engines[container.JobId].Abort();
      else if (container.Message == MessageContainer.MessageType.JobAborted)
        //kill appdomain
        Console.WriteLine("tmp");
      else if (container.Message == MessageContainer.MessageType.RequestSnapshot)
        engines[container.JobId].RequestSnapshot();
      else if (container.Message == MessageContainer.MessageType.SnapshotReady)
        // must be async!
        engines[container.JobId].GetSnapshot();
    }        
  }
}
