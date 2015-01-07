#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Creatable("Scripts")]
  [StorableClass]
  public class CSharpScript : Script, IStorableContent {
    #region Constants
    protected const string ExecuteMethodName = "Execute";
    protected override string CodeTemplate { get { return ReadCodeTemplate("HeuristicLab.Scripting.CSharpScriptCode.cs"); } }
    #endregion

    #region Fields & Properties
    private CSharpScriptBase compiledScript;

    public string Filename { get; set; }

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }
    #endregion

    #region Construction & Initialization
    [StorableConstructor]
    protected CSharpScript(bool deserializing) : base(deserializing) { }
    protected CSharpScript(CSharpScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
    }
    public CSharpScript() {
      variableStore = new VariableStore();
      Code = CodeTemplate;
    }
    public CSharpScript(string code)
      : base(code) {
      variableStore = new VariableStore();
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

    private Thread scriptThread;
    public virtual void ExecuteAsync() {
      if (compiledScript == null) return;
      scriptThread = new Thread(() => {
        Exception ex = null;
        try {
          OnScriptExecutionStarted();
          compiledScript.Execute(VariableStore);
        } catch (ThreadAbortException) {
          // the execution was cancelled by the user
        } catch (Exception e) {
          ex = e;
        } finally {
          OnScriptExecutionFinished(ex);
        }
      });
      scriptThread.SetApartmentState(ApartmentState.STA);
      scriptThread.Start();
    }

    public virtual void Kill() {
      if (scriptThread.IsAlive)
        scriptThread.Abort();
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
  }
}
