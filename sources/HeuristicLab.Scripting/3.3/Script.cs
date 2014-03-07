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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using Microsoft.CSharp;

namespace HeuristicLab.Scripting {
  [Item("C# Script", "An empty C# script.")]
  [Creatable("Scripts")]
  [StorableClass]
  public sealed class Script : NamedItem, IStorableContent {
    #region Constants
    private const string ExecuteMethodName = "Execute";
    private const string CodeTemplate =
@"// use 'vars' to access global variables in the variable store

using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

public class UserScript : HeuristicLab.Scripting.UserScriptBase {
  public override void Main() {
    // type your code here
  }

  // further classes and methods

}";
    #endregion

    #region Fields & Properties
    private UserScriptBase compiledScript;

    public string Filename { get; set; }

    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [Storable]
    private string code;
    public string Code {
      get { return code; }
      set {
        if (value == code) return;
        code = value;
        compiledScript = null;
        OnCodeChanged();
      }
    }

    private string compilationUnitCode;
    public string CompilationUnitCode {
      get { return compilationUnitCode; }
    }

    private CompilerErrorCollection compileErrors;
    public CompilerErrorCollection CompileErrors {
      get { return compileErrors; }
      private set {
        compileErrors = value;
        OnCompileErrorsChanged();
      }
    }
    #endregion

    #region Construction & Initialization
    [StorableConstructor]
    private Script(bool deserializing) : base(deserializing) { }
    private Script(Script original, Cloner cloner)
      : base(original, cloner) {
      code = original.code;
      variableStore = new VariableStore();
      compilationUnitCode = original.compilationUnitCode;
      if (original.compileErrors != null)
        compileErrors = new CompilerErrorCollection(original.compileErrors);
    }

    public Script()
      : base("Script", "A HeuristicLab script.") {
      code = CodeTemplate;
      variableStore = new VariableStore();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Script(this, cloner);
    }
    #endregion

    private void RegisterScriptEvents() {
      if (compiledScript == null) return;
      compiledScript.ConsoleOutputChanged += compiledScript_ConsoleOutputChanged;
    }

    private void DeregisterScriptEvents() {
      if (compiledScript == null) return;
      compiledScript.ConsoleOutputChanged -= compiledScript_ConsoleOutputChanged;
    }

    #region Compilation
    private CSharpCodeProvider codeProvider =
      new CSharpCodeProvider(
        new Dictionary<string, string> {
          { "CompilerVersion", "v4.0" },  // support C# 4.0 syntax
        });

    private CompilerResults DoCompile() {
      var parameters = new CompilerParameters {
        GenerateExecutable = false,
        GenerateInMemory = true,
        IncludeDebugInformation = true,
        WarningLevel = 4
      };
      parameters.ReferencedAssemblies.AddRange(
        GetAssemblies()
        .Select(a => a.Location)
        .ToArray());
      var unit = CreateCompilationUnit();
      var writer = new StringWriter();
      codeProvider.GenerateCodeFromCompileUnit(
        unit,
        writer,
        new CodeGeneratorOptions {
          ElseOnClosing = true,
          IndentString = "  ",
        });
      compilationUnitCode = writer.ToString();
      return codeProvider.CompileAssemblyFromDom(parameters, unit);
    }

    public void Compile() {
      var results = DoCompile();
      compiledScript = null;
      CompileErrors = results.Errors;
      if (results.Errors.HasErrors) {
        var sb = new StringBuilder();
        foreach (CompilerError error in results.Errors) {
          sb.Append(error.Line).Append(':')
            .Append(error.Column).Append(": ")
            .AppendLine(error.ErrorText);
        }
        throw new Exception(string.Format(
          "Compilation of \"{0}\" failed:{1}{2}",
          Name, Environment.NewLine,
          sb.ToString()));
      } else {
        var assembly = results.CompiledAssembly;
        var types = assembly.GetTypes();
        DeregisterScriptEvents();
        compiledScript = (UserScriptBase)Activator.CreateInstance(types[0]);
        RegisterScriptEvents();
      }
    }

    public IEnumerable<Assembly> GetAssemblies() {
      var assemblies = new List<Assembly>();
      foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
        try {
          if (File.Exists(a.Location)) assemblies.Add(a);
        } catch (NotSupportedException) {
          // NotSupportedException is thrown while accessing 
          // the Location property of the anonymously hosted
          // dynamic methods assembly, which is related to
          // LINQ queries
        }
      }
      assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly); // for dlr functionality
      return assemblies;
    }

    private readonly Regex SafeTypeNameCharRegex = new Regex("[_a-zA-Z0-9]+");
    private readonly Regex SafeTypeNameRegex = new Regex("[_a-zA-Z][_a-zA-Z0-9]*");

    private CodeCompileUnit CreateCompilationUnit() {
      var unit = new CodeSnippetCompileUnit(code);
      return unit;
    }

    public string CompiledTypeName {
      get {
        var sb = new StringBuilder();
        var strings = SafeTypeNameCharRegex.Matches(Name)
                                           .Cast<Match>()
                                           .Select(m => m.Value);
        foreach (string s in strings)
          sb.Append(s);
        return SafeTypeNameRegex.Match(sb.ToString()).Value;
      }
    }
    #endregion

    private Thread scriptThread;
    public void Execute() {
      if (compiledScript == null) return;
      var executeMethod = typeof(UserScriptBase).GetMethod(ExecuteMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
      if (executeMethod != null) {
        scriptThread = new Thread(() => {
          Exception ex = null;
          try {
            OnScriptExecutionStarted();
            executeMethod.Invoke(compiledScript, new[] { VariableStore });
          } catch (ThreadAbortException) {
            // the execution was cancelled by the user
          } catch (TargetInvocationException e) {
            ex = e.InnerException;
          } finally {
            OnScriptExecutionFinished(ex);
          }
        });
        scriptThread.Start();
      }
    }

    public void Kill() {
      if (scriptThread.IsAlive)
        scriptThread.Abort();
    }

    private void compiledScript_ConsoleOutputChanged(object sender, EventArgs<string> e) {
      OnConsoleOutputChanged(e.Value);
    }

    public event EventHandler CodeChanged;
    private void OnCodeChanged() {
      var handler = CodeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CompileErrorsChanged;
    private void OnCompileErrorsChanged() {
      var handler = CompileErrorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ScriptExecutionStarted;
    private void OnScriptExecutionStarted() {
      var handler = ScriptExecutionStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<Exception>> ScriptExecutionFinished;
    private void OnScriptExecutionFinished(Exception e) {
      var handler = ScriptExecutionFinished;
      if (handler != null) handler(this, new EventArgs<Exception>(e));
    }

    public event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    private void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(this, new EventArgs<string>(args));
    }
  }
}
