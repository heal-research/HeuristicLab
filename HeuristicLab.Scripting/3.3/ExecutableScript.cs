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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Scripting {
  [Item("Executable Script", "An executable script.")]
  [StorableType("206BFDFE-A829-424E-BFAD-BB7993830325")]
  public abstract class ExecutableScript : Script {
    private Thread scriptThread;
    private DateTime lastUpdateTime;

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      protected set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    public bool Running { get; protected set; }

    #region Construction & Cloning
    [StorableConstructor]
    protected ExecutableScript(StorableConstructorFlag _) : base(_) { }
    protected ExecutableScript(ExecutableScript original, Cloner cloner)
      : base(original, cloner) {
      executionTime = original.executionTime;
    }
    protected ExecutableScript()
      : base() {
      executionTime = TimeSpan.Zero;
    }
    protected ExecutableScript(string code)
      : base(code) {
      executionTime = TimeSpan.Zero;
    }
    #endregion

    #region Execution
    protected abstract void ExecuteCode();

    public virtual void Execute() {
      if (Running)
        throw new InvalidOperationException("Script is already running");

      ExecutionTime = TimeSpan.Zero;
      Exception ex = null;
      var timer = new System.Timers.Timer(250) { AutoReset = true };
      timer.Elapsed += timer_Elapsed;
      try {
        Running = true;
        OnScriptExecutionStarted();
        lastUpdateTime = DateTime.UtcNow;
        timer.Start();
        ExecuteCode();
      } catch (Exception e) {
        ex = e;
      } finally {
        timer.Elapsed -= timer_Elapsed;
        timer.Stop();
        timer.Dispose();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
        Running = false;
        OnScriptExecutionFinished(ex);
      }
    }

    public virtual void ExecuteAsync() {
      scriptThread = new Thread(() => {
        try {
          Execute();
        } finally {
          scriptThread = null;
        }
      });
      scriptThread.SetApartmentState(ApartmentState.STA);
      scriptThread.Start();
    }

    public virtual void Kill() {
      if (!Running) return;

      scriptThread.Abort();
    }
    #endregion

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      var timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }

    public event EventHandler ScriptExecutionStarted;
    protected virtual void OnScriptExecutionStarted() {
      var handler = ScriptExecutionStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<Exception>> ScriptExecutionFinished;
    protected virtual void OnScriptExecutionFinished(Exception e) {
      var handler = ScriptExecutionFinished;
      if (handler != null) handler(this, new EventArgs<Exception>(e));
    }

    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
