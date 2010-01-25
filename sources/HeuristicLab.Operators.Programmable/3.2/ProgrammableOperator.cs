#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text.RegularExpressions;
using HeuristicLab.Core;
using HeuristicLab.Data;
using System.Data.Linq;
using System.Xml.XPath;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Operators.Programmable {

  public class ProgrammableOperator : OperatorBase {

    #region Fields & Properties

    private MethodInfo executeMethod;
    public CompilerErrorCollection CompileErrors { get; private set; }
    public string CompilationUnitCode { get; private set; }

    private string description;
    public override string Description {
      get { return description; }
    }

    private string code;
    public string Code {
      get { return code; }
      set {
        if (value != code) {
          code = value;
          executeMethod = null;
          OnCodeChanged();
        }
      }
    }

    private object syncRoot = new object();

    public readonly Dictionary<string, List<Assembly>> Plugins;

    protected Dictionary<Assembly, bool> Assemblies;
    public IEnumerable<Assembly> AvailableAssemblies {
      get { return Assemblies.Keys; }
    }

    public IEnumerable<Assembly> SelectedAssemblies {
      get { return Assemblies.Where(kvp => kvp.Value).Select(kvp => kvp.Key); }
    }

    private HashSet<string> namespaces;
    public IEnumerable<string> Namespaces {
      get { return namespaces; }
    }

    #endregion

    #region Extended Accessors

    public void SelectAssembly(Assembly a) {
      if (a != null && Assemblies.ContainsKey(a))
        Assemblies[a] = true;
    }

    public void UnselectAssembly(Assembly a) {
      if (a != null && Assemblies.ContainsKey(a))
        Assemblies[a] = false;
    }

    public void SelectNamespace(string ns) {
      namespaces.Add(ns);
    }

    public void UnselectNamespace(string ns) {
      namespaces.Remove(ns);
    }

    public void SetDescription(string description) {
      if (description == null)
        throw new NullReferenceException("description must not be null");

      if (description != this.description) {
        this.description = description;
        OnDescriptionChanged();
      }
    }

    public IEnumerable<string> GetAllNamespaces(bool selectedAssembliesOnly) {
      var namespaces = new HashSet<string>();
      foreach (var a in Assemblies) {
        if (!selectedAssembliesOnly || a.Value) {
          foreach (var t in a.Key.GetTypes()) {
            if (t.IsPublic) {
              namespaces.Add(t.Namespace);
            }
          }
        }
      }
      return namespaces;
    }
    #endregion

    #region Construction & Initialization

    public ProgrammableOperator() {
      code = "";
      description = "An operator that can be programmed for arbitrary needs.";
      executeMethod = null;
      Assemblies = DiscoverAssemblies();
      namespaces = new HashSet<string>(DiscoverNamespaces());
      Plugins = GroupAssemblies();
    }

    private Dictionary<string, List<Assembly>> GroupAssemblies() {
      var plugins = new Dictionary<string, List<Assembly>>();
      var assemblyNames = Assemblies.ToDictionary(a => a.Key.Location, a => a.Key);
      foreach (var plugin in ApplicationManager.Manager.Plugins) {
        var aList = new List<Assembly>();
        foreach (var aName in plugin.Assemblies) {
          Assembly a;
          assemblyNames.TryGetValue(aName, out a);
          if (a != null) {
            aList.Add(a);
            assemblyNames.Remove(aName);
          }
        }
        plugins[plugin.Name] = aList;
      }
      plugins["other"] = assemblyNames.Values.ToList();
      return plugins;
    }

    protected static List<Assembly> defaultAssemblies = new List<Assembly>() {      
      typeof(System.Linq.Enumerable).Assembly,  // add reference to version 3.5 of System.dll
      typeof(System.Collections.Generic.List<>).Assembly,
      typeof(System.Text.StringBuilder).Assembly,      
      typeof(System.Data.Linq.DataContext).Assembly,
      typeof(HeuristicLab.Core.OperatorBase).Assembly,
      typeof(HeuristicLab.Data.IntData).Assembly,      
      
    };

    protected static Dictionary<Assembly, bool> DiscoverAssemblies() {
      var assemblies = new Dictionary<Assembly, bool>();
      foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
        try {
          if (File.Exists(a.Location)) {
            assemblies.Add(a, false);
          }
        } catch (NotSupportedException) {
          // NotSupportedException is thrown while accessing 
          // the Location property of the anonymously hosted
          // dynamic methods assembly, which is related to
          // LINQ queries
        }
      }
      foreach (var a in defaultAssemblies) {
        if (assemblies.ContainsKey(a)) {
          assemblies[a] = true;
        } else {
          assemblies.Add(a, true);
        }
      }
      return assemblies;
    }

    protected static List<string> DiscoverNamespaces() {
      return new List<string>() {
        "System",
        "System.Collections.Generic",
        "System.Text",
        "System.Linq",
        "System.Data.Linq",
        "HeuristicLab.Core",
        "HeuristicLab.Data",
      };
    }

    #endregion

    #region Compilation

    private static CSharpCodeProvider codeProvider =
      new CSharpCodeProvider(
        new Dictionary<string, string>() {
          { "CompilerVersion", "v3.5" },  // support C# 3.0 syntax
        });

    private CompilerResults DoCompile() {
      CompilerParameters parameters = new CompilerParameters();
      parameters.GenerateExecutable = false;
      parameters.GenerateInMemory = true;
      parameters.IncludeDebugInformation = false;
      parameters.ReferencedAssemblies.AddRange(SelectedAssemblies.Select(a => a.Location).ToArray());
      var unit = CreateCompilationUnit();
      var writer = new StringWriter();
      codeProvider.GenerateCodeFromCompileUnit(
        unit,
        writer,
        new CodeGeneratorOptions() {
          BracingStyle = "C",
          ElseOnClosing = true,
          IndentString = "  ",
        });
      CompilationUnitCode = writer.ToString();
      return codeProvider.CompileAssemblyFromDom(parameters, unit);
    }

    public virtual void Compile() {
      var results = DoCompile();
      executeMethod = null;
      if (results.Errors.HasErrors) {
        CompileErrors = results.Errors;
        StringBuilder sb = new StringBuilder();
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
        CompileErrors = null;
        Assembly assembly = results.CompiledAssembly;
        Type[] types = assembly.GetTypes();
        executeMethod = types[0].GetMethod("Execute");
      }
    }

    private CodeCompileUnit CreateCompilationUnit() {
      CodeNamespace ns = new CodeNamespace("HeuristicLab.Operators.Programmable.CustomOperators");
      ns.Types.Add(CreateType());
      ns.Imports.AddRange(
        GetSelectedAndValidNamespaces()
        .Select(n => new CodeNamespaceImport(n))
        .ToArray());
      CodeCompileUnit unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);
      return unit;
    }

    public IEnumerable<string> GetSelectedAndValidNamespaces() {
      var possibleNamespaces = new HashSet<string>(GetAllNamespaces(true));
      foreach (var ns in Namespaces)
        if (possibleNamespaces.Contains(ns))
          yield return ns;
    }

    public static readonly Regex SafeTypeNameCharRegex = new Regex("[_a-zA-Z0-9]+");
    public static readonly Regex SafeTypeNameRegex = new Regex("[_a-zA-Z][_a-zA-Z0-9]*");

    public string CompiledTypeName {
      get {
        var sb = new StringBuilder();
        foreach (string s in SafeTypeNameCharRegex.Matches(Name).Cast<Match>().Select(m => m.Value)) {
          sb.Append(s);
        }
        return SafeTypeNameRegex.Match(sb.ToString()).Value;
      }
    }

    private CodeTypeDeclaration CreateType() {
      CodeTypeDeclaration typeDecl = new CodeTypeDeclaration(CompiledTypeName) {
        IsClass = true,
        TypeAttributes = TypeAttributes.Public,
      };
      typeDecl.Members.Add(CreateMethod());
      return typeDecl;
    }

    public string Signature {
      get {
        var sb = new StringBuilder()
        .Append("public static IOperation Execute(IOperator op, IScope scope");
        foreach (var info in VariableInfos)
          sb.Append(String.Format(", {0} {1}", info.DataType.Name, info.FormalName));
        return sb.Append(")").ToString();
      }
    }

    private static Regex lineSplitter = new Regex(@"\r\n|\r|\n");

    private CodeMemberMethod CreateMethod() {
      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Execute";
      method.ReturnType = new CodeTypeReference(typeof(HeuristicLab.Core.IOperation));
      method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IOperator), "op"));
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IScope), "scope"));
      foreach (IVariableInfo info in VariableInfos)
        method.Parameters.Add(new CodeParameterDeclarationExpression(info.DataType, info.FormalName));
      string[] codeLines = lineSplitter.Split(code);
      for (int i = 0; i < codeLines.Length; i++) {
        codeLines[i] = string.Format("#line {0} \"ProgrammableOperator\"{1}{2}", i + 1, "\r\n", codeLines[i]);
      }
      method.Statements.Add(new CodeSnippetStatement(
        string.Join("\r\n", codeLines) +
        "\r\nreturn null;"));
      return method;
    }

    #endregion

    #region HeuristicLab interfaces

    public override IOperation Apply(IScope scope) {
      lock (syncRoot) {
        if (executeMethod == null) {
          Compile();
        }
      }

      var parameters = new List<object>() { this, scope };
      parameters.AddRange(VariableInfos.Select(info => GetParameter(info, scope)));
      return (IOperation)executeMethod.Invoke(null, parameters.ToArray());
    }

    private object GetParameter(IVariableInfo info, IScope scope) {
      if ((info.Kind & VariableKind.New) != VariableKind.New) {
        return GetVariableValue(info.FormalName, scope, true);
      } else {
        var parameter = GetVariableValue(info.FormalName, scope, false, false);
        if (parameter != null)
          return parameter;
        IItem value = (IItem)Activator.CreateInstance(info.DataType);
        if (info.Local) {
          AddVariable(new Variable(info.ActualName, value));
        } else {
          scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), value));
        }
        return value;
      }
    }

    public override IView CreateView() {
      return new ProgrammableOperatorView(this);
    }

    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      if (DescriptionChanged != null)
        DescriptionChanged(this, new EventArgs());
    }
    public event EventHandler CodeChanged;
    protected virtual void OnCodeChanged() {
      if (CodeChanged != null)
        CodeChanged(this, new EventArgs());
    }

    #endregion

    #region Persistence & Cloning

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ProgrammableOperator clone = (ProgrammableOperator)base.Clone(clonedObjects);
      clone.description = Description;
      clone.code = Code;
      clone.executeMethod = executeMethod;
      clone.Assemblies = Assemblies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
      clone.namespaces = namespaces;
      clone.CompilationUnitCode = CompilationUnitCode;
      clone.CompileErrors = CompileErrors;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlNode descriptionNode = document.CreateNode(XmlNodeType.Element, "Description", null);
      descriptionNode.InnerText = description;
      node.AppendChild(descriptionNode);

      XmlNode codeNode = document.CreateNode(XmlNodeType.Element, "Code", null);
      codeNode.InnerText = code;
      node.AppendChild(codeNode);

      XmlNode assembliesNode = document.CreateNode(XmlNodeType.Element, "Assemblies", null);
      foreach (var a in SelectedAssemblies) {
        var assemblyNode = document.CreateNode(XmlNodeType.Element, "Assembly", null);
        assemblyNode.InnerText = a.FullName;
        assembliesNode.AppendChild(assemblyNode);
      }
      node.AppendChild(assembliesNode);

      XmlNode namespacesNode = document.CreateNode(XmlNodeType.Element, "Namespaces", null);
      foreach (string ns in namespaces) {
        var nsNode = document.CreateNode(XmlNodeType.Element, "Namespace", null);
        nsNode.InnerText = ns;
        namespacesNode.AppendChild(nsNode);
      }
      node.AppendChild(namespacesNode);

      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      XmlNode descriptionNode = node.SelectSingleNode("Description");
      description = descriptionNode.InnerText;

      XmlNode codeNode = node.SelectSingleNode("Code");
      code = codeNode.InnerText;

      XmlNode assembliesNode = node.SelectSingleNode("Assemblies");
      if (assembliesNode != null) {
        var selectedAssemblyNames = new HashSet<string>();
        foreach (XmlNode assemblyNode in assembliesNode.ChildNodes) {
          selectedAssemblyNames.Add(assemblyNode.InnerText);
        }
        var selectedAssemblies = new List<Assembly>();
        foreach (var a in Assemblies.Keys.ToList()) {
          Assemblies[a] = selectedAssemblyNames.Contains(a.FullName);
        }
      }
      XmlNode namespacesNode = node.SelectSingleNode("Namespaces");
      if (namespacesNode != null) {
        namespaces.Clear();
        var possibleNamespaces = new HashSet<string>(GetAllNamespaces(true));
        foreach (XmlNode nsNode in namespacesNode.ChildNodes) {
          if (possibleNamespaces.Contains(nsNode.InnerText))
            SelectNamespace(nsNode.InnerText);
        }
      }
    }

    #endregion
  }
}
