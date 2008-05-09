using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Grid;
using System.Threading;
using HeuristicLab.Core;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace HeuristicLab.DistributedEngine {
  class JobManager {
    private IGridServer server;
    private string address;
    private Dictionary<Guid, AtomicOperation> engineOperations = new Dictionary<Guid, AtomicOperation>();
    private Dictionary<Guid, byte[]> runningEngines = new Dictionary<Guid, byte[]>();
    private Dictionary<Guid, ManualResetEvent> waithandles = new Dictionary<Guid, ManualResetEvent>();
    private object locker = new object();
    private const int MAX_RESTARTS = 5;
    private Exception exception;
    private ChannelFactory<IGridServer> factory;
    public Exception Exception {
      get { return exception; }
    }

    public JobManager(string address) {
      this.address = address;
    }

    internal void Reset() {
      lock(locker) {
        ResetConnection();
        foreach(WaitHandle wh in waithandles.Values) wh.Close();
        waithandles.Clear();
        engineOperations.Clear();
        runningEngines.Clear();
        exception = null;
      }
    }

    private void ResetConnection() {
      // open a new channel
      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;
      factory = new ChannelFactory<IGridServer>(binding);
      server = factory.CreateChannel(new EndpointAddress(address));
    }

    public WaitHandle BeginExecuteOperation(IOperatorGraph operatorGraph, IScope globalScope, AtomicOperation operation) {
      ProcessingEngine engine = new ProcessingEngine(operatorGraph, globalScope, operation); // OperatorGraph not needed?
      MemoryStream memStream = new MemoryStream();
      GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
      PersistenceManager.Save(engine, stream);
      stream.Close();
      if(factory.State != CommunicationState.Opened)
        ResetConnection();
      Guid currentEngineGuid = server.BeginExecuteEngine(memStream.ToArray());
      lock(locker) {
        runningEngines[currentEngineGuid] = memStream.ToArray();
        engineOperations[currentEngineGuid] = operation;
        waithandles[currentEngineGuid] = new ManualResetEvent(false);
        ThreadPool.QueueUserWorkItem(new WaitCallback(TryGetResult), currentEngineGuid);
      }
      return waithandles[currentEngineGuid];
    }

    private void TryGetResult(object state) {
      Guid engineGuid = (Guid)state;
      int restartCounter = 0;
      do {
        try {
          if(factory.State != CommunicationState.Opened) ResetConnection();
          byte[] resultXml = server.TryEndExecuteEngine(engineGuid, 100);
          if(resultXml != null) {
            // restore the engine 
            GZipStream stream = new GZipStream(new MemoryStream(resultXml), CompressionMode.Decompress);
            ProcessingEngine resultEngine = (ProcessingEngine)PersistenceManager.Load(stream);

            // merge the results
            IScope oldScope = engineOperations[engineGuid].Scope;
            oldScope.Clear();
            foreach(IVariable variable in resultEngine.InitialOperation.Scope.Variables) {
              oldScope.AddVariable(variable);
            }
            foreach(IScope subScope in resultEngine.InitialOperation.Scope.SubScopes) {
              oldScope.AddSubScope(subScope);
            }
            foreach(KeyValuePair<string, string> alias in resultEngine.InitialOperation.Scope.Aliases) {
              oldScope.AddAlias(alias.Key, alias.Value);
            }

            lock(locker) {
              // signal the wait handle and clean up then return
              waithandles[engineGuid].Set();
              engineOperations.Remove(engineGuid);
              waithandles.Remove(engineGuid);
              runningEngines.Remove(engineGuid);
            }
            return;
          } else {
            // check if the server is still working on the job
            JobState jobState = server.JobState(engineGuid);
            if(jobState == JobState.Unkown) {
              // restart job
              byte[] packedEngine;
              lock(locker) {
                packedEngine = runningEngines[engineGuid];
              }
              server.BeginExecuteEngine(packedEngine);
              restartCounter++;
            }
          }
        } catch(Exception e) {
          // catch all exceptions set an exception flag, signal the wait-handle and exit the routine
          this.exception = new Exception("There was a problem with parallel execution", e);
          waithandles[engineGuid].Set();
          return;
        }

        // when we reach a maximum amount of restarts => signal the wait-handle and set a flag that there was a problem
        if(restartCounter > MAX_RESTARTS) {
          this.exception = new Exception("Maximal number of parallel job restarts reached");
          waithandles[engineGuid].Set();
          return;
        }

        Thread.Sleep(TimeSpan.FromSeconds(10.0));
      } while(true);
    }
  }
}
