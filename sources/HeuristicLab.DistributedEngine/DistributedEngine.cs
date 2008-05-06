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
using HeuristicLab.PluginInfrastructure;
using System.Windows.Forms;

namespace HeuristicLab.DistributedEngine {
  public class DistributedEngine : EngineBase, IEditable {
    private JobManager jobManager;
    private CompositeOperation waitingOperations;
    private string serverAddress;
    public string ServerAddress {
      get { return serverAddress; }
      set {
        if(value != serverAddress) {
          serverAddress = value;
        }
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
      if(jobManager == null) this.jobManager = new JobManager(serverAddress);
      jobManager.Reset();
      base.Execute();
    }

    public override void ExecuteSteps(int steps) {
      throw new InvalidOperationException("DistributedEngine doesn't support stepwise execution");
    }

    protected override void ProcessNextOperation() {
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
          WaitHandle[] waithandles = new WaitHandle[compositeOperation.Operations.Count];
          int i = 0;
          foreach(AtomicOperation parOperation in compositeOperation.Operations) {
            waithandles[i++] = jobManager.BeginExecuteOperation(OperatorGraph, GlobalScope, parOperation);
          }
          WaitHandle.WaitAll(waithandles);
          if(jobManager.Exception != null) {
            myExecutionStack.Push(compositeOperation);
            Abort();
            ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(jobManager.Exception); });
          }
        } else {
          for(int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            myExecutionStack.Push(compositeOperation.Operations[i]);
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
