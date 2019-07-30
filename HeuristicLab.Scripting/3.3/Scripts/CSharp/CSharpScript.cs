#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Scripting {
  [Item("C# Script", "An empty C# script.")]
  [Creatable(CreatableAttribute.Categories.Scripts, Priority = 100)]
  [StorableType("1630933C-AB5B-44EC-A967-C299CC57E4E3")]
  public class CSharpScript : ExecutableScript, IStorableContent {
    #region Fields & Properties
    private CSharpScriptBase compiledScript;
    public dynamic Instance {
      get { return compiledScript; }
    }

    public string Filename { get; set; }

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected CSharpScript(StorableConstructorFlag _) : base(_) { }
    protected CSharpScript(CSharpScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
    }
    public CSharpScript()
      : base(ScriptTemplates.CSharpScriptTemplate) {
      variableStore = new VariableStore();
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

    protected override void ExecuteCode() {
      if (compiledScript == null) return;

      compiledScript.Execute(VariableStore);
    }

    protected virtual void CompiledScriptOnConsoleOutputChanged(object sender, EventArgs<string> e) {
      OnConsoleOutputChanged(e.Value);
    }

    public event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    protected virtual void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(this, new EventArgs<string>(args));
    }
  }
}
