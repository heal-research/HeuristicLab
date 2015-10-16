#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Scripting {
  [Item("C# Script", "An empty C# script.")]
  [Creatable(CreatableAttribute.Categories.Scripts, Priority = 100)]
  [StorableClass]
  public class CSharpScript : Script, IStorableContent {
    #region Constants
    protected const string ExecuteMethodName = "Execute";
    protected override string CodeTemplate { get { return ScriptTemplates.CSharpScriptTemplate; } }
    #endregion

    #region Fields & Properties
    private CSharpScriptBase compiledScript;
    private Thread scriptThread;
    private DateTime lastUpdateTime;

    public string Filename { get; set; }

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      protected set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }
    #endregion

    #region Construction & Initialization
    [StorableConstructor]
    protected CSharpScript(bool deserializing) : base(deserializing) { }
    protected CSharpScript(CSharpScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
      executionTime = original.executionTime;
    }
    public CSharpScript() {
      variableStore = new VariableStore();
      executionTime = TimeSpan.Zero;
      Code = CodeTemplate;
    }
    public CSharpScript(string code)
      : base(code) {
      variableStore = new VariableStore();
      executionTime = TimeSpan.Zero;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CSharpScript(this, cloner);
    }
    #endregion

    protected virtual void RegisterScriptEvents() {
      if (compiledScript != null)
        compiledScript.ConsoleOutputChanged += CompiledScriptOnConsoleOutputChanged;
    }

    protected virtual void DeregisterScriptEvents() {
      if (compiledScript != null)
        compiledScript.ConsoleOutputChanged -= CompiledScriptOnConsoleOutputChanged;
    }

    #region Compilation

    public override Assembly Compile() {
      DeregisterScriptEvents();
      compiledScript = null;
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      compiledScript = (CSharpScriptBase)Activator.CreateInstance(types.Single(x => typeof(CSharpScriptBase).IsAssignableFrom(x)));
      RegisterScriptEvents();
      return assembly;
    }
    #endregion

    public virtual void ExecuteAsync() {
      if (compiledScript == null) return;
      executionTime = TimeSpan.Zero;
      scriptThread = new Thread(() => {
        Exception ex = null;
        var timer = new System.Timers.Timer(250) { AutoReset = true };
        timer.Elapsed += timer_Elapsed;
        try {
          OnScriptExecutionStarted();
          lastUpdateTime = DateTime.UtcNow;
          timer.Start();
          compiledScript.Execute(VariableStore);
        } catch (Exception e) {
          ex = e;
        } finally {
          scriptThread = null;
          timer.Elapsed -= timer_Elapsed;
          timer.Stop();
          ExecutionTime += DateTime.UtcNow - lastUpdateTime;
          OnScriptExecutionFinished(ex);
        }
      });
      scriptThread.SetApartmentState(ApartmentState.STA);
      scriptThread.Start();
    }

    public virtual void Kill() {
      if (scriptThread == null) return;
      scriptThread.Abort();
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      var timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }

    protected virtual void CompiledScriptOnConsoleOutputChanged(object sender, EventArgs<string> e) {
      OnConsoleOutputChanged(e.Value);
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

    public event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    protected virtual void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(this, new EventArgs<string>(args));
    }

    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
