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

    private bool pausePending, stopPending;
    private DateTime lastUpdateTime;
    private System.Timers.Timer timer;

    [StorableConstructor]
    protected Engine(bool deserializing)
      : base(deserializing) {
      pausePending = stopPending = false;
      timer = new System.Timers.Timer(100);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
    }
    protected Engine(Engine original, Cloner cloner)
      : base(original, cloner) {
      if (original.ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      log = cloner.Clone(original.log);
      executionStack = new Stack<IOperation>();
      IOperation[] contexts = original.executionStack.ToArray();
      for (int i = contexts.Length - 1; i >= 0; i--)
        executionStack.Push(cloner.Clone(contexts[i]));
      pausePending = original.pausePending;
      stopPending = original.stopPending;
      timer = new System.Timers.Timer(100);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
    }
    protected Engine()
      : base() {
      log = new Log();
      executionStack = new Stack<IOperation>();
      pausePending = stopPending = false;
      timer = new System.Timers.Timer(100);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
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
    }
    protected override void OnPaused() {
      Log.LogMessage("Engine paused");
      base.OnPaused();
    }

    public override void Stop() {
      base.Stop();
      stopPending = true;
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
      while (!pausePending && !stopPending && (executionStack.Count > 0)) {
        ProcessNextOperation();
      }
      timer.Stop();
      ExecutionTime += DateTime.Now - lastUpdateTime;

      if (pausePending) OnPaused();
      else OnStopped();
    }

    protected abstract void ProcessNextOperation();

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      DateTime now = DateTime.Now;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
    }
  }
}
