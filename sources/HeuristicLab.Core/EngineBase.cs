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

namespace HeuristicLab.Core {
  public abstract class EngineBase : ItemBase, IEngine {
    protected IOperatorGraph myOperatorGraph;
    public IOperatorGraph OperatorGraph {
      get { return myOperatorGraph; }
    }
    protected IScope myGlobalScope;
    public IScope GlobalScope {
      get { return myGlobalScope; }
    }

    private TimeSpan myExecutionTime;
    public TimeSpan ExecutionTime {
      get { return myExecutionTime; }
      protected set {
        myExecutionTime = value;
        OnExecutionTimeChanged();
      }
    }

    protected Stack<IOperation> myExecutionStack;
    public Stack<IOperation> ExecutionStack {
      get { return myExecutionStack; }
    }
    protected bool myRunning;
    public bool Running {
      get { return myRunning; }
    }
    protected bool myCanceled;
    public bool Canceled {
      get { return myCanceled; }
    }
    public virtual bool Terminated {
      get { return ExecutionStack.Count == 0; }
    }

    protected EngineBase() {
      myOperatorGraph = new OperatorGraph();
      myGlobalScope = new Scope("Global");
      myExecutionStack = new Stack<IOperation>();
      Reset();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      EngineBase clone = (EngineBase)base.Clone(clonedObjects);
      clone.myOperatorGraph = (IOperatorGraph)Auxiliary.Clone(OperatorGraph, clonedObjects);
      clone.myGlobalScope = (IScope)Auxiliary.Clone(GlobalScope, clonedObjects);
      clone.myExecutionTime = ExecutionTime;
      IOperation[] operations = new IOperation[ExecutionStack.Count];
      ExecutionStack.CopyTo(operations, 0);
      for (int i = operations.Length - 1; i >= 0; i--)
        clone.myExecutionStack.Push((IOperation)Auxiliary.Clone(operations[i], clonedObjects));
      clone.myRunning = Running;
      clone.myCanceled = Canceled;
      return clone;
      
    }

    public virtual void Execute() {
      myRunning = true;
      myCanceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), null);
    }
    public virtual void ExecuteSteps(int steps) {
      myRunning = true;
      myCanceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), steps);
    }
    public void ExecuteStep() {
      ExecuteSteps(1);
    }
    public virtual void Abort() {
      myCanceled = true;
    }
    public virtual void Reset() {
      myCanceled = false;
      myRunning = false;
      OperatorGraph.Reset();
      GlobalScope.Clear();
      ExecutionTime = new TimeSpan();
      myExecutionStack.Clear();
      if (OperatorGraph.InitialOperator != null)
        myExecutionStack.Push(new AtomicOperation(OperatorGraph.InitialOperator, GlobalScope));
      OnInitialized();
    }

    private void Run(object state) {
      if (state == null) Run();
      else RunSteps((int)state);
      myRunning = false;
      OnFinished();
    }
    private void Run() {
      DateTime start = DateTime.Now;
      DateTime end;
      while ((!Canceled) && (!Terminated)) {
        ProcessNextOperation();
        end = DateTime.Now;
        ExecutionTime += end - start;
        start = end;
      }
      ExecutionTime += DateTime.Now - start;
    }
    private void RunSteps(int steps) {
      DateTime start = DateTime.Now;
      DateTime end;
      int step = 0;
      while ((!Canceled) && (!Terminated) && (step < steps)) {
        ProcessNextOperation();
        step++;
        end = DateTime.Now;
        ExecutionTime += end - start;
        start = end;
      }
      ExecutionTime += DateTime.Now - start;
    }

    protected abstract void ProcessNextOperation();

    public event EventHandler Initialized;
    protected virtual void OnInitialized() {
      if (Initialized != null)
        Initialized(this, new EventArgs());
    }
    public event EventHandler<OperationEventArgs> OperationExecuted;
    protected virtual void OnOperationExecuted(IOperation operation) {
      if (OperationExecuted != null)
        OperationExecuted(this, new OperationEventArgs(operation));
    }
    public event EventHandler<ExceptionEventArgs> ExceptionOccurred;
    protected virtual void OnExceptionOccurred(Exception exception) {
      Abort();
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new ExceptionEventArgs(exception));
    }
    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      if (ExecutionTimeChanged != null)
        ExecutionTimeChanged(this, new EventArgs());
    }
    public event EventHandler Finished;
    protected virtual void OnFinished() {
      if (Finished != null)
        Finished(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      node.AppendChild(PersistenceManager.Persist("OperatorGraph", OperatorGraph, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("GlobalScope", GlobalScope, document, persistedObjects));

      XmlNode stackNode = document.CreateNode(XmlNodeType.Element, "ExecutionStack", null);
      IOperation[] operations = new IOperation[ExecutionStack.Count];
      ExecutionStack.CopyTo(operations, 0);
      for (int i = 0; i < operations.Length; i++)
        stackNode.AppendChild(PersistenceManager.Persist(operations[i], document, persistedObjects));
      node.AppendChild(stackNode);

      XmlNode timeNode = document.CreateNode(XmlNodeType.Element, "ExecutionTime", null);
      timeNode.InnerText = ExecutionTime.ToString();
      node.AppendChild(timeNode);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myOperatorGraph = (IOperatorGraph)PersistenceManager.Restore(node.SelectSingleNode("OperatorGraph"), restoredObjects);
      myGlobalScope = (IScope)PersistenceManager.Restore(node.SelectSingleNode("GlobalScope"), restoredObjects);

      XmlNode stackNode = node.SelectSingleNode("ExecutionStack");
      for (int i = stackNode.ChildNodes.Count - 1; i >= 0; i--)
        myExecutionStack.Push((IOperation)PersistenceManager.Restore(stackNode.ChildNodes[i], restoredObjects));

      XmlNode timeNode = node.SelectSingleNode("ExecutionTime");
      myExecutionTime = TimeSpan.Parse(timeNode.InnerText);
    }
    #endregion
  }
}
