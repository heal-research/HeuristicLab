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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Programmable {
  public class ProgrammableOperator : OperatorBase {
    private MethodInfo executeMethod;

    [Storable]
    private string myDescription;
    public override string Description {
      get { return myDescription; }
    }

    [Storable]
    private string myCode;    
    public string Code {
      get { return myCode; }
      set {
        if (value != myCode) {
          myCode = value;
          executeMethod = null;
          OnCodeChanged();
        }
      }
    }

    private object syncRoot = new object();

    public ProgrammableOperator() {
      myCode = "Result.Data = true;";
      myDescription = "An operator that can be programmed for arbitrary needs.";
      AddVariableInfo(new VariableInfo("Result", "A computed variable", typeof(BoolData), VariableKind.New | VariableKind.Out));
      executeMethod = null;
    }

    public void SetDescription(string description) {
      if (description == null)
        throw new NullReferenceException("description must not be null");

      if (description != myDescription) {
        myDescription = description;
        OnDescriptionChanged();
      }
    }

    public virtual void Compile() {
      CodeNamespace ns = new CodeNamespace("HeuristicLab.Operators.Programmable.CustomOperators");
      CodeTypeDeclaration typeDecl = new CodeTypeDeclaration("Operator");
      typeDecl.IsClass = true;
      typeDecl.TypeAttributes = TypeAttributes.Public;

      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Execute";
      method.ReturnType = new CodeTypeReference(typeof(IOperation));
      method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IOperator), "op"));
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IScope), "scope"));
      foreach (IVariableInfo info in VariableInfos)
        method.Parameters.Add(new CodeParameterDeclarationExpression(info.DataType, info.FormalName));
      string code = myCode + "\r\n" + "return null;";
      method.Statements.Add(new CodeSnippetStatement(code));
      typeDecl.Members.Add(method);

      ns.Types.Add(typeDecl);
      ns.Imports.Add(new CodeNamespaceImport("System"));
      ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
      ns.Imports.Add(new CodeNamespaceImport("System.Text"));
      ns.Imports.Add(new CodeNamespaceImport("System.Linq"));
      ns.Imports.Add(new CodeNamespaceImport("System.Data.Linq"));
      ns.Imports.Add(new CodeNamespaceImport("HeuristicLab.Core"));
      foreach (IVariableInfo variableInfo in VariableInfos)
        ns.Imports.Add(new CodeNamespaceImport(variableInfo.DataType.Namespace));

      CodeCompileUnit unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);
      CompilerParameters parameters = new CompilerParameters();
      parameters.GenerateExecutable = false;
      parameters.GenerateInMemory = true;
      parameters.IncludeDebugInformation = false;
      Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (Assembly loadedAssembly in loadedAssemblies)
        parameters.ReferencedAssemblies.Add(loadedAssembly.Location);
      parameters.ReferencedAssemblies.Add(typeof(Enumerable).Assembly.Location); // add reference to version 3.5 of System.dll
      parameters.ReferencedAssemblies.Add(typeof(DataContext).Assembly.Location); // add reference System.Data.Linq.Dll
      CodeDomProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });  // support C# 3.0 syntax
      CompilerResults results = provider.CompileAssemblyFromDom(parameters, unit);

      executeMethod = null;
      if (results.Errors.HasErrors) {
        StringWriter writer = new StringWriter();
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BlankLinesBetweenMembers = false;
        options.ElseOnClosing = true;
        options.IndentString = "  ";
        provider.GenerateCodeFromCompileUnit(unit, writer, options);
        writer.Flush();
        string[] source = writer.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < source.Length; i++)
          builder.AppendLine((i + 3).ToString("###") + "     " + source[i]);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine();
        foreach (CompilerError error in results.Errors) {
          builder.Append("Line " + error.Line.ToString());
          builder.Append(", Column " + error.Column.ToString());
          builder.AppendLine(": " + error.ErrorText);
        }
        throw new Exception("Compile Errors:\n\n" + builder.ToString());
      } else {
        Assembly assembly = results.CompiledAssembly;
        Type[] types = assembly.GetTypes();
        executeMethod = types[0].GetMethod("Execute");
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ProgrammableOperator clone = (ProgrammableOperator)base.Clone(clonedObjects);
      clone.myDescription = Description;
      clone.myCode = Code;
      clone.executeMethod = executeMethod;
      return clone;
    }

    public override IOperation Apply(IScope scope) {
      lock (syncRoot) {
        if (executeMethod == null) {
          Compile();
        }
      }

      // collect parameters
      object[] parameters = new object[VariableInfos.Count + 2];
      parameters[0] = this;
      parameters[1] = scope;
      int i = 2;
      foreach (IVariableInfo info in VariableInfos) {
        if ((info.Kind & VariableKind.New) == VariableKind.New) {
          parameters[i] = GetVariableValue(info.FormalName, scope, false, false);
          if (parameters[i] == null) {
            IItem value = (IItem)Activator.CreateInstance(info.DataType);
            if (info.Local) {
              AddVariable(new Variable(info.ActualName, value));
            } else {
              scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), value));
            }
            parameters[i] = value;
          }
        } else
          parameters[i] = GetVariableValue(info.FormalName, scope, true);
        i++;
      }

      return (IOperation)executeMethod.Invoke(null, parameters);
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
  }
}
