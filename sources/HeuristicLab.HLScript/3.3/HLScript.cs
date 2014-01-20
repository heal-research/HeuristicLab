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

namespace HeuristicLab.HLScript {
  [Item("HL Script", "A HeuristicLab script.")]
  [Creatable("Scripts")]
  [StorableClass]
  public sealed class HLScript : NamedItem, IStorableContent {
    #region Constants
    private const string ScriptNamespaceName = "HeuristicLab.HLScript";
    private const string ExecuteMethodName = "Execute";
    private const string CodeTemplate =
@"// use 'vars' to access global variables in the variable store

using System;

public override void Main() {
  // type your code here
}

// further classes and methods";
    #endregion

    #region Fields & Properties
    private HLScriptGeneration compiledHLScript;

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
        compiledHLScript = null;
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
    private HLScript(bool deserializing) : base(deserializing) { }
    private HLScript(HLScript original, Cloner cloner)
      : base(original, cloner) {
      code = original.code;
      variableStore = new VariableStore();
      compilationUnitCode = original.compilationUnitCode;
      if (original.compileErrors != null)
        compileErrors = new CompilerErrorCollection(original.compileErrors);
    }

    public HLScript()
      : base("HL Script", "A HeuristicLab script.") {
      code = CodeTemplate;
      variableStore = new VariableStore();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HLScript(this, cloner);
    }
    #endregion

    private void RegisterScriptEvents() {
      if (compiledHLScript == null) return;
      compiledHLScript.ConsoleOutputChanged += compiledHLScript_ConsoleOutputChanged;
    }

    private void DeregisterScriptEvents() {
      if (compiledHLScript == null) return;
      compiledHLScript.ConsoleOutputChanged -= compiledHLScript_ConsoleOutputChanged;
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
        IncludeDebugInformation = false
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
      compiledHLScript = null;
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
        compiledHLScript = (HLScriptGeneration)Activator.CreateInstance(types[0]);
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

    private readonly Regex LineSplitter = new Regex(@"\r\n|\r|\n");
    private readonly Regex SafeTypeNameCharRegex = new Regex("[_a-zA-Z0-9]+");
    private readonly Regex SafeTypeNameRegex = new Regex("[_a-zA-Z][_a-zA-Z0-9]*");
    private readonly Regex NamespaceDeclarationRegex = new Regex(@"using\s+(@?[a-z_A-Z]\w+(?:\s*\.\s*@?[a-z_A-Z]\w*)*)\s*;");
    private readonly Regex NamespaceRegex = new Regex(@"(@?[a-z_A-Z]\w+(?:\s*\.\s*@?[a-z_A-Z]\w*)*)");
    private readonly Regex CommentRegex = new Regex(@"((/\*)[^/]+(\*/))|(//.*)");

    private CodeCompileUnit CreateCompilationUnit() {
      var ns = new CodeNamespace(ScriptNamespaceName);
      ns.Types.Add(CreateScriptClass());
      ns.Imports.AddRange(
        GetNamespaces()
        .Select(n => new CodeNamespaceImport(n))
        .ToArray());
      var unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);
      return unit;
    }

    private IEnumerable<string> GetNamespaces() {
      var strings = NamespaceDeclarationRegex.Matches(CommentRegex.Replace(code, string.Empty))
                                             .Cast<Match>()
                                             .Select(m => m.Value);
      foreach (var s in strings) {
        var match = NamespaceRegex.Match(s.Replace("using", string.Empty));
        yield return match.Value;
      }
    }

    private IEnumerable<string> GetCodeLines() {
      var lines = LineSplitter.Split(code);
      foreach (var line in lines) {
        string trimmedLine = line.Trim();
        if (!NamespaceDeclarationRegex.IsMatch(trimmedLine))
          yield return trimmedLine;
      }
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

    private CodeTypeDeclaration CreateScriptClass() {
      var typeDecl = new CodeTypeDeclaration(CompiledTypeName) {
        IsClass = true,
        TypeAttributes = TypeAttributes.Public,
      };
      typeDecl.BaseTypes.Add(typeof(HLScriptGeneration));
      typeDecl.Members.Add(new CodeSnippetTypeMember(string.Join(Environment.NewLine, GetCodeLines())));
      return typeDecl;
    }
    #endregion

    private Thread scriptThread;
    public void Execute() {
      if (compiledHLScript == null) return;
      var executeMethod = typeof(HLScriptGeneration).GetMethod(ExecuteMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
      if (executeMethod != null) {
        scriptThread = new Thread(() => {
          try {
            OnScriptExecutionStarted();
            executeMethod.Invoke(compiledHLScript, new[] { VariableStore });
          } finally {
            OnScriptExecutionFinished();
          }
        });
        scriptThread.Start();
      }
    }

    public void Kill() {
      if (scriptThread.IsAlive)
        scriptThread.Abort();
    }

    private void compiledHLScript_ConsoleOutputChanged(object sender, EventArgs<string> e) {
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

    public event EventHandler ScriptExecutionFinished;
    private void OnScriptExecutionFinished() {
      var handler = ScriptExecutionFinished;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs<string>> ConsoleOutputChanged;
    private void OnConsoleOutputChanged(string args) {
      var handler = ConsoleOutputChanged;
      if (handler != null) handler(this, new EventArgs<string>(args));
    }
  }
}
