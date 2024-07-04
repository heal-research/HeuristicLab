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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace HeuristicLab.Scripting {
  [StorableType("0FA4F218-E1F5-4C09-9C2F-12B32D4EC373")]
  public abstract class Script : NamedItem, IProgrammableItem {
    #region Fields & Properties
    public static readonly HashSet<string> ExcludedAssemblyFileNames = new HashSet<string> { "IKVM.OpenJDK.ClassLibrary.dll" };
    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    [Storable]
    private string code;
    public string Code {
      get { return code; }
      set {
        if (value == code) return;
        code = value;
        OnCodeChanged();
      }
    }

    private List<Diagnostic> compileErrors;
    public List<Diagnostic> CompileErrors {
      get { return compileErrors; }
      private set {
        compileErrors = value;
        OnCompileErrorsChanged();
      }
    }
    #endregion

    #region Construction & Initialization
    [StorableConstructor]
    protected Script(StorableConstructorFlag _) : base(_) { }
    protected Script(Script original, Cloner cloner)
      : base(original, cloner) {
      code = original.code;
      if (original.compileErrors != null)
        compileErrors = new List<Diagnostic>(original.compileErrors);
    }
    protected Script()
      : base("Script", "An empty script.") {
    }
    protected Script(string code)
      : this() {
      this.code = code;
    }
    #endregion

    #region Compilation
    protected virtual EmitResult DoCompile(out Assembly assembly) {
      var syntaxTree = CSharpSyntaxTree.ParseText(code,new CSharpParseOptions(LanguageVersion.CSharp10));

      var references = GetAssemblies()
        .Select(a => MetadataReference.CreateFromFile(a.Location))
        .Cast<MetadataReference>();

      var compilation = CSharpCompilation.Create(
        assemblyName: Path.GetRandomFileName(),
        syntaxTrees: new[] { syntaxTree },
        references: references,
        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        
          .WithOptimizationLevel(OptimizationLevel.Debug)
          .WithWarningLevel(4)
      );

      using (var ms = new MemoryStream()) {
        var result = compilation.Emit(ms);
        if (result.Success) {
          ms.Seek(0, SeekOrigin.Begin);
          assembly = Assembly.Load(ms.ToArray());
        } else {
          assembly = null;
        }
        return result;
      }
    }

    public virtual Assembly Compile() {
      var result = DoCompile(out var assembly);
      CompileErrors = result.Diagnostics.ToList();
      if (!result.Success) {
        var sb = new StringBuilder();
        foreach (var diagnostic in CompileErrors) {
          sb.Append(diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1).Append(':')
            .Append(diagnostic.Location.GetLineSpan().StartLinePosition.Character + 1).Append(": ")
            .AppendLine(diagnostic.GetMessage());
        }
        throw new CompilationException($"Compilation of \"{Name}\" failed:{Environment.NewLine}{sb}");
      }
      return assembly;
    }

    public virtual IEnumerable<Assembly> GetAssemblies() {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => !a.IsDynamic && File.Exists(a.Location)
                 && !ExcludedAssemblyFileNames.Contains(Path.GetFileName(a.Location)))
        .GroupBy(x => Regex.Replace(Path.GetFileName(x.Location), @"-[\d.]+\.dll$", ""))
        .Select(x => x.OrderByDescending(y => y.GetName().Version).First())
        .ToList();
      assemblies.Add(typeof(object).Assembly); // include mscorlib
      assemblies.Add(typeof(Enumerable).Assembly); // include System.Linq
      assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly); // for dynamic functionality include Microsoft.CSharp  
      return assemblies;
    }
    #endregion

    public event EventHandler CodeChanged;
    protected virtual void OnCodeChanged() {
      var handler = CodeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler CompileErrorsChanged;
    protected virtual void OnCompileErrorsChanged() {
      var handler = CompileErrorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}