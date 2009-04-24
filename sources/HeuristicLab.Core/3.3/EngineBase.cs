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
  /// <summary>
  /// Base class to represent an engine, which is an interpreter, holding the code, the data and 
  /// the actual state, which is the runtime stack and a pointer onto the next operation. It represents
  /// one execution and can handle parallel executions.
  /// </summary>
  public abstract class EngineBase : ItemBase, IEngine {
    /// <summary>
    /// Field of the current instance that represent the operator graph.
    /// </summary>
    protected IOperatorGraph myOperatorGraph;
    /// <summary>
    /// Gets the current operator graph.
    /// </summary>
    public IOperatorGraph OperatorGraph {
      get { return myOperatorGraph; }
    }
    /// <summary>
    /// Field of the current instance that represent the global scope.
    /// </summary>
    protected IScope myGlobalScope;
    /// <summary>
    /// Gets the current global scope.
    /// </summary>
    public IScope GlobalScope {
      get { return myGlobalScope; }
    }

    private TimeSpan myExecutionTime;
    /// <summary>
    /// Gets or sets the execution time.
    /// </summary>
    /// <remarks>Calls <see cref="OnExecutionTimeChanged"/> in the setter.</remarks>
    public TimeSpan ExecutionTime {
      get { return myExecutionTime; }
      protected set {
        myExecutionTime = value;
        OnExecutionTimeChanged();
      }
    }

    /// <summary>
    /// Field of the current instance that represent the execution stack.
    /// </summary>
    protected Stack<IOperation> myExecutionStack;
    /// <summary>
    /// Gets the current execution stack.
    /// </summary>
    public Stack<IOperation> ExecutionStack {
      get { return myExecutionStack; }
    }
    
    /// <summary>
    /// Flag of the current instance whether it is currently running.
    /// </summary>
    protected bool myRunning;
    /// <summary>
    /// Gets information whether the instance is currently running.
    /// </summary>
    public bool Running {
      get { return myRunning; }
    }

    /// <summary>
    /// Flag of the current instance whether it is canceled.
    /// </summary>
    protected bool myCanceled;
    /// <summary>
    /// Gets information whether the instance is currently canceled.
    /// </summary>
    public bool Canceled {
      get { return myCanceled; }
    }
    /// <summary>
    /// Gets information whether the instance has already terminated.
    /// </summary>
    public virtual bool Terminated {
      get { return ExecutionStack.Count == 0; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBase"/> with a new global scope.
    /// </summary>
    /// <remarks>Calls <see cref="Reset"/>.</remarks>
    protected EngineBase() {
      myOperatorGraph = new OperatorGraph();
      myGlobalScope = new Scope("Global");
      myExecutionStack = new Stack<IOperation>();
      Reset();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="EngineBase"/>.</returns>
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

    /// <inheritdoc/>
    /// <remarks>Calls <see cref="ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback, object)"/> 
    /// of class <see cref="ThreadPool"/>.</remarks>
    public virtual void Execute() {
      myRunning = true;
      myCanceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), null);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback, object)"/> 
    /// of class <see cref="ThreadPool"/>.</remarks>
    public virtual void ExecuteSteps(int steps) {
      myRunning = true;
      myCanceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), steps);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback, object)"/> 
    /// of class <see cref="ThreadPool"/>.</remarks>
    public void ExecuteStep() {
      ExecuteSteps(1);
    }
    /// <inheritdoc/>
    /// <remarks>Sets the protected flag <c>myCanceled</c> to <c>true</c>.</remarks>
    public virtual void Abort() {
      myCanceled = true;
    }
    /// <inheritdoc/>
    /// <remarks>Sets <c>myCanceled</c> and <c>myRunning</c> to <c>false</c>. The global scope is cleared,
    /// the execution time is reseted, the execution stack is cleared and a new <see cref="AtomicOperation"/>
    /// with the initial operator is added. <br/>
    /// Calls <see cref="OnInitialized"/>.</remarks>
    public virtual void Reset() {
      myCanceled = false;
      myRunning = false;
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

    /// <summary>
    /// Performs the next operation.
    /// </summary>
    protected abstract void ProcessNextOperation();

    /// <summary>
    /// Occurs when the current instance is initialized.
    /// </summary>
    public event EventHandler Initialized;
    /// <summary>
    /// Fires a new <c>Initialized</c> event.
    /// </summary>
    protected virtual void OnInitialized() {
      if (Initialized != null)
        Initialized(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when an operation is executed.
    /// </summary>
    public event EventHandler<OperationEventArgs> OperationExecuted;
    /// <summary>
    /// Fires a new <c>OperationExecuted</c> event.
    /// </summary>
    /// <param name="operation">The operation that has been executed.</param>
    protected virtual void OnOperationExecuted(IOperation operation) {
      if (OperationExecuted != null)
        OperationExecuted(this, new OperationEventArgs(operation));
    }
    /// <summary>
    /// Occurs when an exception occured during the execution.
    /// </summary>
    public event EventHandler<ExceptionEventArgs> ExceptionOccurred;
    /// <summary>
    /// Aborts the execution and fires a new <c>ExceptionOccurred</c> event.
    /// </summary>
    /// <param name="exception">The exception that was thrown.</param>
    protected virtual void OnExceptionOccurred(Exception exception) {
      Abort();
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new ExceptionEventArgs(exception));
    }
    /// <summary>
    /// Occurs when the execution time changed.
    /// </summary>
    public event EventHandler ExecutionTimeChanged;
    /// <summary>
    /// Fires a new <c>ExecutionTimeChanged</c> event.
    /// </summary>
    protected virtual void OnExecutionTimeChanged() {
      if (ExecutionTimeChanged != null)
        ExecutionTimeChanged(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the execution is finished.
    /// </summary>
    public event EventHandler Finished;
    /// <summary>
    /// Fires a new <c>Finished</c> event.
    /// </summary>
    protected virtual void OnFinished() {
      if (Finished != null)
        Finished(this, new EventArgs());
    }

    #region Persistence Methods
    /// <summary>
    /// Saves the current instance as <see cref="XmlNode"/> in the specified <paramref name="document"/>.
    /// </summary>
    /// <remarks>Calls <see cref="StorableBase.GetXmlNode"/> of base class <see cref="ItemBase"/>.<br/>
    /// A quick overview how the single elements of the current instance are saved:
    /// <list type="bullet">
    /// <item>
    /// <term>Operator graph: </term>
    /// <description>Saved as a child node with the tag name <c>OperatorGraph</c>.</description>
    /// </item>
    /// <item>
    /// <term>Global scope: </term>
    /// <description>Saved as a child node with the tag name <c>GlobalScope</c>.</description> 
    /// </item>
    /// <item>
    /// <term>Execution stack: </term>
    /// <description>A child node is created with the tag name <c>ExecutionStack</c>. Beyond this child node
    /// all operations of the execution stack are saved as child nodes.</description>
    /// </item>
    /// <item>
    /// <term>Execution time: </term>
    /// <description>Saved as a child node with the tag name <c>ExecutionTime</c>, where the execution
    /// time is saved as string in the node's inner text.</description>
    /// </item>
    /// </list></remarks>
    /// <param name="name">The (tag)name of the <see cref="XmlNode"/>.</param>
    /// <param name="document">The <see cref="XmlDocument"/> where to save the data.</param>
    /// <param name="persistedObjects">The dictionary of all already persisted objects. (Needed to avoid cycles.)</param>
    /// <returns>The saved <see cref="XmlNode"/>.</returns>
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
    /// <summary>
    ///  Loads the persisted instance from the specified <paramref name="node"/>.
    /// </summary>
    /// <remarks>See <see cref="GetXmlNode"/> to get information on how the instance must be saved. <br/>
    /// Calls <see cref="StorableBase.Populate"/> of base class <see cref="ItemBase"/>.</remarks>
    /// <param name="node">The <see cref="XmlNode"/> where the engine is saved.</param>
    /// <param name="restoredObjects">The dictionary of all already restored objects. 
    /// (Needed to avoid cycles.)</param>
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
