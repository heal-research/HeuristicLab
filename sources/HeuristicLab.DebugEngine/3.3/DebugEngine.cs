#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.DebugEngine {

  [StorableClass]
  [Item("Debug Engine", "Engine for debugging algorithms.")]
  public class DebugEngine : Executable, IEngine {

    #region Construction and Cloning

    [StorableConstructor]
    protected DebugEngine(bool deserializing)
      : base(deserializing) {
      pausePending = stopPending = false;
      InitializeTimer();
    }

    protected DebugEngine(DebugEngine original, Cloner cloner)
      : base(original, cloner) {
      if (original.ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      Log = cloner.Clone(original.Log);
      ExecutionStack = cloner.Clone(original.ExecutionStack);
      OperatorTrace = cloner.Clone(original.OperatorTrace);
      pausePending = original.pausePending;
      stopPending = original.stopPending;
      InitializeTimer();
      currentOperation = cloner.Clone(original.currentOperation);
      currentOperator = cloner.Clone(original.currentOperator);
    }
    public DebugEngine()
      : base() {
      Log = new Log();
      ExecutionStack = new ExecutionStack();
      OperatorTrace = new OperatorTrace();
      pausePending = stopPending = false;
      InitializeTimer();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DebugEngine(this, cloner);
    }

    private void InitializeTimer() {
      timer = new System.Timers.Timer(100);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
    }

    #endregion

    #region Fields and Properties

    [Storable]
    public ILog Log { get; private set; }

    [Storable]
    public ExecutionStack ExecutionStack { get; private set; }

    [Storable]
    public OperatorTrace OperatorTrace { get; private set; }

    private bool pausePending, stopPending;
    private DateTime lastUpdateTime;
    private System.Timers.Timer timer;

    [Storable]
    private IOperator currentOperator;

    [Storable]
    private IOperation currentOperation;
    public IOperation CurrentOperation {
      get { return currentOperation; }
      private set {
        if (value != currentOperation) {
          currentOperation = value;
          OnOperationChanged(value);
        }
      }
    }

    public virtual IAtomicOperation CurrentAtomicOperation {
      get { return CurrentOperation as IAtomicOperation; }
    }

    public virtual IExecutionContext CurrentExecutionContext {
      get { return CurrentOperation as IExecutionContext; }
    }

    public virtual bool CanContinue {
      get { return CurrentOperation != null || ExecutionStack.Count > 0; }
    }

    public virtual bool IsAtBreakpoint {
      get { return CurrentAtomicOperation != null && CurrentAtomicOperation.Operator != null && CurrentAtomicOperation.Operator.Breakpoint; }
    }

    #endregion

    #region Events

    public event EventHandler<OperationChangedEventArgs> CurrentOperationChanged;
    protected virtual void OnOperationChanged(IOperation newOperation) {
      EventHandler<OperationChangedEventArgs> handler = CurrentOperationChanged;
      if (handler != null) {
        handler(this, new OperationChangedEventArgs(newOperation));
      }
    }

    #endregion

    #region Std Methods
    public sealed override void Prepare() {
      base.Prepare();
      ExecutionStack.Clear();
      CurrentOperation = null;
      OperatorTrace.Reset();
      OnPrepared();
    }
    public void Prepare(IOperation initialOperation) {
      base.Prepare();
      ExecutionStack.Clear();
      if (initialOperation != null)
        ExecutionStack.Add(initialOperation);
      CurrentOperation = null;
      OperatorTrace.Reset();
      OnPrepared();
    }
    protected override void OnPrepared() {
      Log.LogMessage("Engine prepared");
      base.OnPrepared();
    }

    public virtual void Step(bool skipStackOperations) {
      OnStarted();
      lastUpdateTime = DateTime.Now;
      timer.Start();
      ProcessNextOperation();
      while (skipStackOperations && !(CurrentOperation is IAtomicOperation) && CanContinue)
        ProcessNextOperation();
      timer.Stop();
      ExecutionTime += DateTime.Now - lastUpdateTime;
      OnPaused();
    }

    public override void Start() {
      base.Start();
      ThreadPool.QueueUserWorkItem(new WaitCallback(Run), null);
    }

    protected override void OnStarted() {
      Log.LogMessage("Engine started");
      base.OnStarted();
    }

    public override void Pause() {
      base.Pause();
      pausePending = true;
      if (currentOperator != null) currentOperator.Abort();
    }

    protected override void OnPaused() {
      Log.LogMessage("Engine paused");
      base.OnPaused();
    }

    public override void Stop() {
      CurrentOperation = null;
      base.Stop();
      stopPending = true;
      if (currentOperator != null) currentOperator.Abort();
      if (ExecutionState == ExecutionState.Paused) OnStopped();
    }

    protected override void OnStopped() {
      Log.LogMessage("Engine stopped");
      base.OnStopped();
    }

    protected override void OnExceptionOccurred(Exception exception) {
      Log.LogException(exception);
      base.OnExceptionOccurred(exception);
    }

    private void Run(object state) {
      OnStarted();
      pausePending = stopPending = false;

      lastUpdateTime = DateTime.Now;
      timer.Start();
      if (!pausePending && !stopPending && CanContinue)
        ProcessNextOperation();
      while (!pausePending && !stopPending && CanContinue && !IsAtBreakpoint)
        ProcessNextOperation();
      timer.Stop();
      ExecutionTime += DateTime.Now - lastUpdateTime;

      if (IsAtBreakpoint)
        Log.LogMessage(string.Format("Breaking before: {0}", CurrentAtomicOperation.Operator.Name));
      if (pausePending || IsAtBreakpoint)
        OnPaused();
      else
        OnStopped();
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      DateTime now = DateTime.Now;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
    }
    #endregion

    #region Methods



    /// <summary>
    /// Deals with the next operation, if it is an <see cref="AtomicOperation"/> it is executed,
    /// if it is a <see cref="CompositeOperation"/> its single operations are pushed on the execution stack.
    /// </summary>
    /// <remarks>If an error occurs during the execution the operation is aborted and the operation
    /// is pushed on the stack again.<br/>
    /// If the execution was successful <see cref="EngineBase.OnOperationExecuted"/> is called.</remarks>
    protected virtual void ProcessNextOperation() {
      try {
        IAtomicOperation atomicOperation = CurrentOperation as IAtomicOperation;
        OperationCollection operations = CurrentOperation as OperationCollection;
        if (atomicOperation != null && operations != null)
          throw new InvalidOperationException("Current operation is both atomic and an operation collection");

        if (atomicOperation != null) {
          Log.LogMessage(string.Format("Performing atomic operation {0}", Utils.Name(atomicOperation)));
          PerformAtomicOperation(atomicOperation);
        } else if (operations != null) {
          Log.LogMessage("Expanding operation collection");
          ExecutionStack.AddRange(operations.Reverse());
          CurrentOperation = null;
        } else if (ExecutionStack.Count > 0) {
          Log.LogMessage("Popping execution stack");
          CurrentOperation = ExecutionStack.Last();
          ExecutionStack.RemoveAt(ExecutionStack.Count - 1);
        } else {
          Log.LogMessage("Nothing to do");
        }
        OperatorTrace.Regenerate(CurrentAtomicOperation);
      } catch (Exception x) {
        OnExceptionOccurred(x);
      }
    }

    protected virtual void PerformAtomicOperation(IAtomicOperation operation) {
      if (operation != null) {
        try {
          currentOperator = operation.Operator;
          IOperation successor = operation.Operator.Execute((IExecutionContext)operation);
          if (successor != null) {
            OperatorTrace.RegisterParenthood(operation, successor);
            ExecutionStack.Add(successor);
          }
          currentOperator = null;
          CurrentOperation = null;
        } catch (Exception ex) {
          OnExceptionOccurred(new OperatorExecutionException(operation.Operator, ex));
          Pause();
        }
      }
    }

    #endregion
  }
}
