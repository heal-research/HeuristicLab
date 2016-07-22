#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
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
      InitializeTimer();
    }

    protected DebugEngine(DebugEngine original, Cloner cloner)
      : base(original, cloner) {
      if (original.ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      Log = cloner.Clone(original.Log);
      ExecutionStack = cloner.Clone(original.ExecutionStack);
      OperatorTrace = cloner.Clone(original.OperatorTrace);
      InitializeTimer();
      currentOperation = cloner.Clone(original.currentOperation);
    }
    public DebugEngine()
      : base() {
      Log = new Log();
      ExecutionStack = new ExecutionStack();
      OperatorTrace = new OperatorTrace();
      InitializeTimer();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DebugEngine(this, cloner);
    }

    private void InitializeTimer() {
      timer = new System.Timers.Timer(250);
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

    private CancellationTokenSource cancellationTokenSource;
    private bool stopPending;
    private DateTime lastUpdateTime;
    private System.Timers.Timer timer;

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
      cancellationTokenSource = new CancellationTokenSource();
      stopPending = false;
      lastUpdateTime = DateTime.UtcNow;
      timer.Start();
      try {
        ProcessNextOperation(true, cancellationTokenSource.Token);
        while (skipStackOperations && !(CurrentOperation is IAtomicOperation) && CanContinue)
          ProcessNextOperation(true, cancellationTokenSource.Token);
      }
      catch (Exception ex) {
        OnExceptionOccurred(ex);
      }
      timer.Stop();
      ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      cancellationTokenSource.Dispose();
      cancellationTokenSource = null;
      if (stopPending) ExecutionStack.Clear();
      if (stopPending || !CanContinue) OnStopped();
      else OnPaused();
    }

    public override void Start() {
      base.Start();
      cancellationTokenSource = new CancellationTokenSource();
      stopPending = false;
      Task task = Task.Factory.StartNew(Run, cancellationTokenSource.Token, cancellationTokenSource.Token);
      task.ContinueWith(t => {
        try {
          t.Wait();
        }
        catch (AggregateException ex) {
          try {
            ex.Flatten().Handle(x => x is OperationCanceledException);
          }
          catch (AggregateException remaining) {
            if (remaining.InnerExceptions.Count == 1) OnExceptionOccurred(remaining.InnerExceptions[0]);
            else OnExceptionOccurred(remaining);
          }
        }
        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;

        if (stopPending) ExecutionStack.Clear();
        if (stopPending || !CanContinue) OnStopped();
        else OnPaused();
      });
    }

    protected override void OnStarted() {
      Log.LogMessage("Engine started");
      base.OnStarted();
    }

    public override void Pause() {
      base.Pause();
      cancellationTokenSource.Cancel();
    }

    protected override void OnPaused() {
      Log.LogMessage("Engine paused");
      base.OnPaused();
    }

    public override void Stop() {
      CurrentOperation = null;
      base.Stop();
      if (ExecutionState == ExecutionState.Paused) {
        ExecutionStack.Clear();
        OnStopped();
      } else {
        stopPending = true;
        cancellationTokenSource.Cancel();
      }
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
      CancellationToken cancellationToken = (CancellationToken)state;

      OnStarted();
      lastUpdateTime = DateTime.UtcNow;
      timer.Start();
      try {
        if (!cancellationToken.IsCancellationRequested && CanContinue)
          ProcessNextOperation(false, cancellationToken);
        while (!cancellationToken.IsCancellationRequested && CanContinue && !IsAtBreakpoint)
          ProcessNextOperation(false, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
      }
      finally {
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;

        if (IsAtBreakpoint)
          Log.LogMessage(string.Format("Breaking before: {0}", CurrentAtomicOperation.Operator.Name));
      }
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
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
    protected virtual void ProcessNextOperation(bool logOperations, CancellationToken cancellationToken) {
      IAtomicOperation atomicOperation = CurrentOperation as IAtomicOperation;
      OperationCollection operations = CurrentOperation as OperationCollection;
      if (atomicOperation != null && operations != null)
        throw new InvalidOperationException("Current operation is both atomic and an operation collection");

      if (atomicOperation != null) {
        if (logOperations)
          Log.LogMessage(string.Format("Performing atomic operation {0}", Utils.Name(atomicOperation)));
        PerformAtomicOperation(atomicOperation, cancellationToken);
      } else if (operations != null) {
        if (logOperations)
          Log.LogMessage("Expanding operation collection");
        ExecutionStack.AddRange(operations.Reverse());
        CurrentOperation = null;
      } else if (ExecutionStack.Count > 0) {
        if (logOperations)
          Log.LogMessage("Popping execution stack");
        CurrentOperation = ExecutionStack.Last();
        ExecutionStack.RemoveAt(ExecutionStack.Count - 1);
      } else {
        if (logOperations)
          Log.LogMessage("Nothing to do");
      }
      OperatorTrace.Regenerate(CurrentAtomicOperation);
    }

    protected virtual void PerformAtomicOperation(IAtomicOperation operation, CancellationToken cancellationToken) {
      if (operation != null) {
        try {
          IOperation successor = operation.Operator.Execute((IExecutionContext)operation, cancellationToken);
          if (successor != null) {
            OperatorTrace.RegisterParenthood(operation, successor);
            ExecutionStack.Add(successor);
          }
          CurrentOperation = null;
        }
        catch (Exception ex) {
          if (ex is OperationCanceledException) throw ex;
          else throw new OperatorExecutionException(operation.Operator, ex);
        }
      }
    }

    #endregion
  }
}
