#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("Engine", "A base class for engines.")]
  [StorableClass]
  public abstract class Engine : Executable, IEngine {
    [Storable]
    protected ILog log;
    public ILog Log {
      get { return log; }
    }

    [Storable]
    private Stack<IOperation> executionStack;
    protected Stack<IOperation> ExecutionStack {
      get { return executionStack; }
    }

    #region Variables for communication between threads
    private CancellationTokenSource cancellationTokenSource;
    private bool stopPending;
    private DateTime lastUpdateTime;
    #endregion

    [StorableConstructor]
    protected Engine(bool deserializing) : base(deserializing) { }
    protected Engine(Engine original, Cloner cloner)
      : base(original, cloner) {
      log = cloner.Clone(original.log);
      executionStack = new Stack<IOperation>();
      IOperation[] contexts = original.executionStack.ToArray();
      for (int i = contexts.Length - 1; i >= 0; i--)
        executionStack.Push(cloner.Clone(contexts[i]));
    }
    protected Engine()
      : base() {
      log = new Log();
      executionStack = new Stack<IOperation>();
    }

    public sealed override void Prepare() {
      base.Prepare();
      executionStack.Clear();
      OnPrepared();
    }
    public void Prepare(IOperation initialOperation) {
      base.Prepare();
      executionStack.Clear();
      if (initialOperation != null)
        executionStack.Push(initialOperation);
      OnPrepared();
    }
    protected override void OnPrepared() {
      Log.LogMessage("Engine prepared");
      base.OnPrepared();
    }

    public override void Start(CancellationToken cancellationToken) {
      base.Start(cancellationToken);
      cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      stopPending = false;

      try {
        Run((object)cancellationTokenSource.Token);
      } catch (OperationCanceledException) {
      } catch (AggregateException ae) {
        ae.FlattenAndHandle(new[] { typeof(OperationCanceledException) }, e => OnExceptionOccurred(e));
      } catch (Exception e) {
        OnExceptionOccurred(e);
      }

      cancellationTokenSource.Dispose();
      cancellationTokenSource = null;
      if (stopPending) executionStack.Clear();
      if (executionStack.Count == 0) OnStopped();
      else OnPaused();
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
      base.Stop();
      if (ExecutionState == ExecutionState.Paused) {
        executionStack.Clear();
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
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        Run(cancellationToken);
      } finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      }

      cancellationToken.ThrowIfCancellationRequested();
    }
    protected abstract void Run(CancellationToken cancellationToken);

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }
  }
}
