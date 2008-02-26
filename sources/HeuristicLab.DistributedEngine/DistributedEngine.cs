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
    // currently executed operators
    private IOperator[] currentOperators;
    private int operatorIndex;
    private IGridServer server;

    private string serverAddress;
    public string ServerAddress {
      get { return serverAddress; }
      set {
        if(value != serverAddress) {
          serverAddress = value;
        }
      }
    }

    public DistributedEngine() {
      currentOperators = new IOperator[1000];
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
      base.Abort();
      for(int i = 0; i < currentOperators.Length; i++) {
        if(currentOperators[i] != null)
          currentOperators[i].Abort();
      }
    }

    protected override void ProcessNextOperation() {
      operatorIndex = 1;
      ProcessNextOperation(myExecutionStack, 0);
    }
    private void ProcessNextOperation(Stack<IOperation> stack, int currentOperatorIndex) {
      IOperation operation = stack.Pop();
      if(operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          currentOperators[currentOperatorIndex] = atomicOperation.Operator;
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        } catch(Exception ex) {
          // push operation on stack again
          stack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
        }
        if(next != null)
          stack.Push(next);
        OnOperationExecuted(atomicOperation);
        if(atomicOperation.Operator.Breakpoint) Abort();
      } else if(operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        if(compositeOperation.ExecuteInParallel) {
          Dictionary<Guid, AtomicOperation> runningEngines = new Dictionary<Guid, AtomicOperation>();
          foreach(AtomicOperation parOperation in compositeOperation.Operations) {
            ProcessingEngine engine = new ProcessingEngine(OperatorGraph, GlobalScope, parOperation); // OperatorGraph not needed?
            MemoryStream memStream = new MemoryStream();
            GZipStream stream = new GZipStream(memStream, CompressionMode.Compress, true);
            PersistenceManager.Save(engine, stream);
            stream.Close();
            Guid currentEngineGuid = server.BeginExecuteEngine(memStream.ToArray());
            runningEngines[currentEngineGuid] = parOperation;
          }
          foreach(Guid engineGuid in runningEngines.Keys) {
            byte[] scopeXml = server.EndExecuteEngine(engineGuid);
            GZipStream stream = new GZipStream(new MemoryStream(scopeXml), CompressionMode.Decompress);
            IScope newScope = (IScope)PersistenceManager.Load(stream);
            IScope oldScope = runningEngines[engineGuid].Scope;
            oldScope.Clear();
            foreach(IVariable variable in newScope.Variables) {
              oldScope.AddVariable(variable);
            }
            foreach(IScope subScope in newScope.SubScopes) {
              oldScope.AddSubScope(subScope);
            }
          }

          // TASK (gkronber 12.2.08)
          //if (Canceled) {
          //  // write back not finished tasks
          //  CompositeOperation remaining = new CompositeOperation();
          //  remaining.ExecuteInParallel = true;
          //  for (int i = 0; i < list.tasks.Length; i++) {
          //    if (list.tasks[i].Count > 0) {
          //      CompositeOperation task = new CompositeOperation();
          //      while (list.tasks[i].Count > 0)
          //        task.AddOperation(list.tasks[i].Pop());
          //      remaining.AddOperation(task);
          //    }
          //  }
          //  if (remaining.Operations.Count > 0)
          //    stack.Push(remaining);
          //}
        } else {
          for(int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            stack.Push(compositeOperation.Operations[i]);
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
