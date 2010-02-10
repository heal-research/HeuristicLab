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
using System.Drawing;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Base class to represent an engine, which is an interpreter, holding the code, the data and 
  /// the actual state, which is the runtime stack and a pointer onto the next operation. It represents
  /// one execution and can handle parallel executions.
  /// </summary>
  [Item("Engine", "A base class for engines.")]
  public abstract class Engine : Item, IEngine {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Event; }
    }

    /// <summary>
    /// Field of the current instance that represent the operator graph.
    /// </summary>
    private OperatorGraph operatorGraph;
    /// <summary>
    /// Gets the current operator graph.
    /// </summary>
    [Storable]
    public OperatorGraph OperatorGraph {
      get { return operatorGraph; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (value != operatorGraph) {
          if (operatorGraph != null) operatorGraph.InitialOperatorChanged -= new EventHandler(operatorGraph_InitialOperatorChanged);
          operatorGraph = value;
          if (operatorGraph != null) operatorGraph.InitialOperatorChanged += new EventHandler(operatorGraph_InitialOperatorChanged);
          OnOperatorGraphChanged();
          Initialize();
        }
      }
    }

    /// <summary>
    /// Field of the current instance that represent the global scope.
    /// </summary>
    [Storable]
    private Scope globalScope;
    /// <summary>
    /// Gets the current global scope.
    /// </summary>
    public IScope GlobalScope {
      get { return globalScope; }
    }

    [Storable]
    private TimeSpan executionTime;
    /// <summary>
    /// Gets or sets the execution time.
    /// </summary>
    /// <remarks>Calls <see cref="OnExecutionTimeChanged"/> in the setter.</remarks>
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      protected set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    /// <summary>
    /// Field of the current instance that represent the execution stack.
    /// </summary>
    [Storable]
    private Stack<IExecutionSequence> executionStack;
    /// <summary>
    /// Gets the current execution stack.
    /// </summary>
    protected Stack<IExecutionSequence> ExecutionStack {
      get { return executionStack; }
    }

    /// <summary>
    /// Flag of the current instance whether it is currently running.
    /// </summary>
    private bool running;
    /// <summary>
    /// Gets information whether the instance is currently running.
    /// </summary>
    public bool Running {
      get { return running; }
    }

    /// <summary>
    /// Flag of the current instance whether it is canceled.
    /// </summary>
    private bool canceled;
    /// <summary>
    /// Gets information whether the instance is currently canceled.
    /// </summary>
    protected bool Canceled {
      get { return canceled; }
    }
    /// <summary>
    /// Gets information whether the instance has already terminated.
    /// </summary>
    public bool Finished {
      get { return executionStack.Count == 0; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBase"/> with a new global scope.
    /// </summary>
    /// <remarks>Calls <see cref="Reset"/>.</remarks>
    protected Engine() {
      globalScope = new Scope("Global");
      executionStack = new Stack<IExecutionSequence>();
      OperatorGraph = new OperatorGraph();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="EngineBase"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      Engine clone = (Engine)base.Clone(cloner);
      clone.OperatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      clone.globalScope = (Scope)cloner.Clone(globalScope);
      clone.executionTime = executionTime;
      IExecutionSequence[] contexts = executionStack.ToArray();
      for (int i = contexts.Length - 1; i >= 0; i--)
        clone.executionStack.Push((IExecutionSequence)cloner.Clone(contexts[i]));
      clone.running = running;
      clone.canceled = canceled;
      return clone;
    }

    /// <inheritdoc/>
    /// <remarks>Sets <c>myCanceled</c> and <c>myRunning</c> to <c>false</c>. The global scope is cleared,
    /// the execution time is reseted, the execution stack is cleared and a new <see cref="AtomicOperation"/>
    /// with the initial operator is added. <br/>
    /// Calls <see cref="OnInitialized"/>.</remarks>
    public void Initialize() {
      canceled = false;
      running = false;
      globalScope.Clear();
      ExecutionTime = new TimeSpan();
      executionStack.Clear();
      if (OperatorGraph.InitialOperator != null)
        executionStack.Push(new ExecutionContext(null, OperatorGraph.InitialOperator, GlobalScope));
      OnInitialized();
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback, object)"/> 
    /// of class <see cref="ThreadPool"/>.</remarks>
    public void Start() {
      running = true;
      canceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), null);
    }
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback, object)"/> 
    /// of class <see cref="ThreadPool"/>.</remarks>
    public void Step() {
      running = true;
      canceled = false;
      ThreadPool.QueueUserWorkItem(new WaitCallback(RunStep), null);
    }
    /// <inheritdoc/>
    /// <remarks>Sets the protected flag <c>myCanceled</c> to <c>true</c>.</remarks>
    public virtual void Stop() {
      canceled = true;
    }

    private void Run(object state) {
      OnStarted();
      DateTime start = DateTime.Now;
      DateTime end;
      while ((!Canceled) && (!Finished)) {
        ProcessNextOperator();
        end = DateTime.Now;
        ExecutionTime += end - start;
        start = end;
      }
      ExecutionTime += DateTime.Now - start;
      running = false;
      OnStopped();
    }
    private void RunStep(object state) {
      OnStarted();
      DateTime start = DateTime.Now;
      if ((!Canceled) && (!Finished))
        ProcessNextOperator();
      ExecutionTime += DateTime.Now - start;
      running = false;
      OnStopped();
    }

    /// <summary>
    /// Performs the next operation.
    /// </summary>
    protected abstract void ProcessNextOperator();

    private void operatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      Initialize();
    }

    public event EventHandler OperatorGraphChanged;
    protected virtual void OnOperatorGraphChanged() {
      if (OperatorGraphChanged != null)
        OperatorGraphChanged(this, EventArgs.Empty);
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
    /// Occurs when the execution is initialized.
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
    /// Occurs when the execution is executed.
    /// </summary>
    public event EventHandler Started;
    /// <summary>
    /// Fires a new <c>Started</c> event.
    /// </summary>
    protected virtual void OnStarted() {
      if (Started != null)
        Started(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the execution is finished.
    /// </summary>
    public event EventHandler Stopped;
    /// <summary>
    /// Fires a new <c>Stopped</c> event.
    /// </summary>
    protected virtual void OnStopped() {
      if (Stopped != null)
        Stopped(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when an exception occured during the execution.
    /// </summary>
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    /// <summary>
    /// Aborts the execution and fires a new <c>ExceptionOccurred</c> event.
    /// </summary>
    /// <param name="exception">The exception that was thrown.</param>
    protected virtual void OnExceptionOccurred(Exception exception) {
      if (ExceptionOccurred != null)
        ExceptionOccurred(this, new EventArgs<Exception>(exception));
    }
  }
}
