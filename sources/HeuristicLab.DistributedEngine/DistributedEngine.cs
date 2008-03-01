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
using System.Text;
using System.Xml;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Grid;
using System.ServiceModel;
using System.IO;
using System.IO.Compression;

namespace HeuristicLab.DistributedEngine {
  public class DistributedEngine : EngineBase, IEditable {
    private IGridServer server;
    private Dictionary<Guid, AtomicOperation> engineOperations = new Dictionary<Guid, AtomicOperation>();
    private List<Guid> runningEngines = new List<Guid>();
    private string serverAddress;
    private bool cancelRequested;
    private CompositeOperation waitingOperations;
    public string ServerAddress {
      get { return serverAddress; }
      set {
        if(value != serverAddress) {
          serverAddress = value;
        }
      }
    }
    public override bool Terminated {
      get {
        return myExecutionStack.Count == 0 && runningEngines.Count == 0 && waitingOperations==null;
      }
    }
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      DistributedEngine clone = (DistributedEngine)base.Clone(clonedObjects);
      clone.ServerAddress = serverAddress;
      return clone;
    }

    public override IView CreateView() {
      return new DistributedEngineEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new DistributedEngineEditor(this);
    }

    public override void Execute() {
      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 100000000; // 100Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 100000000; // also 100M chars
      binding.ReaderQuotas.MaxArrayLength = 100000000; // also 100M elements;
      binding.Security.Mode = SecurityMode.None;
      ChannelFactory<IGridServer> factory = new ChannelFactory<IGridServer>(binding);
      server = factory.CreateChannel(new EndpointAddress(serverAddress));

      base.Execute();
    }

    public override void ExecuteSteps(int steps) {
      throw new InvalidOperationException("DistributedEngine doesn't support stepwise execution");
    }

    public override void Abort() {
      lock(runningEngines) {
        cancelRequested = true;
        foreach(Guid engineGuid in runningEngines) {
          server.AbortEngine(engineGuid);
        }
      }
    }
    public override void Reset() {
      base.Reset();
      engineOperations.Clear();
      runningEngines.Clear();
      cancelRequested = false;
    }

    protected override void ProcessNextOperation() {
      lock(runningEngines) {
        if(runningEngines.Count == 0 && cancelRequested) {
          base.Abort();
          cancelRequested = false;
          if(waitingOperations != null && waitingOperations.Operations.Count != 0) {
            myExecutionStack.Push(waitingOperations);
            waitingOperations = null;
          }
          return;
        }
        if(runningEngines.Count != 0) {
          Guid engineGuid = runningEngines[0];
          byte[] resultXml = server.TryEndExecuteEngine(engineGuid, 100);
          if(resultXml != null) {
            GZipStream stream = new GZipStream(new MemoryStream(resultXml), CompressionMode.Decompress);
            ProcessingEngine resultEngine = (ProcessingEngine)PersistenceManager.Load(stream);
            IScope oldScope = engineOperations[engineGuid].Scope;
            oldScope.Clear();
            foreach(IVariable variable in resultEngine.InitialOperation.Scope.Variables) {
              oldScope.AddVariable(variable);
            }
            foreach(IScope subScope in resultEngine.InitialOperation.Scope.SubScopes) {
              oldScope.AddSubScope(subScope);
            }
            OnOperationExecuted(engineOperations[engineGuid]);

            if(cancelRequested & resultEngine.ExecutionStack.Count != 0) {
              if(waitingOperations == null) {
                waitingOperations = new CompositeOperation();
                waitingOperations.ExecuteInParallel = false;
              }
              CompositeOperation task = new CompositeOperation();
              while(resultEngine.ExecutionStack.Count > 0) {
                AtomicOperation oldOperation = (AtomicOperation)resultEngine.ExecutionStack.Pop();
                if(oldOperation.Scope == resultEngine.InitialOperation.Scope) {
                  oldOperation = new AtomicOperation(oldOperation.Operator, oldScope);
                }
                task.AddOperation(oldOperation);
              }
              waitingOperations.AddOperation(task);
            }
            runningEngines.Remove(engineGuid);
            engineOperations.Remove(engineGuid);
          }
          return;
        }
        IOperation operation = myExecutionStack.Pop();
        if(operation is AtomicOperation) {
          AtomicOperation atomicOperation = (AtomicOperation)operation;
          IOperation next = null;
          try {
            next = atomicOperation.Operator.Execute(atomicOperation.Scope);
          } catch(Exception ex) {
            // push operation on stack again
            myExecutionStack.Push(atomicOperation);
            Abort();
            ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
          }
          if(next != null)
            myExecutionStack.Push(next);
          OnOperationExecuted(atomicOperation);
          if(atomicOperation.Operator.Breakpoint) Abort();
        } else if(operation is CompositeOperation) {
          CompositeOperation compositeOperation = (CompositeOperation)operation;
          if(compositeOperation.ExecuteInParallel) {
            foreach(AtomicOperation parOperation in compositeOperation.Operations) {
              ProcessingEngine engine = new ProcessingEngine(OperatorGraph, GlobalScope, parOperation); // OperatorGraph not needed?
              MemoryStream memStream = new MemoryStream();
              GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
              PersistenceManager.Save(engine, stream);
              stream.Close();
              Guid currentEngineGuid = server.BeginExecuteEngine(memStream.ToArray());
              runningEngines.Add(currentEngineGuid);
              engineOperations[currentEngineGuid] = parOperation;
            }
          } else {
            for(int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
              myExecutionStack.Push(compositeOperation.Operations[i]);
          }
        }
      }
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute addressAttribute = document.CreateAttribute("ServerAddress");
      addressAttribute.Value = ServerAddress;
      node.Attributes.Append(addressAttribute);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      ServerAddress = node.Attributes["ServerAddress"].Value;
    }
    #endregion
  }
}
