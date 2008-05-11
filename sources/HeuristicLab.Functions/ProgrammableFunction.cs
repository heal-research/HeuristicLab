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
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.Constraints;
using HeuristicLab.DataAnalysis;
using System.Xml;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Data;
using System.Collections;

namespace HeuristicLab.Functions {
  public sealed class ProgrammableFunction : ProgrammableOperator, IFunction {
    private MethodInfo applyMethod;
    public ProgrammableFunction() {
      // clear the variableinfo that was added by the default constructor of ProgrammableOperator 
      RemoveVariableInfo("Result"); 
      Code = "return 0.0;";
      SetDescription("A function that can be programmed for arbitrary needs.");
      applyMethod = null;
    }

    public override void Compile() {
      CodeNamespace ns = new CodeNamespace("HeuristicLab.Functions.CustomFunctions");
      CodeTypeDeclaration typeDecl = new CodeTypeDeclaration("Function");
      typeDecl.IsClass = true;
      typeDecl.TypeAttributes = TypeAttributes.Public;

      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Apply";
      method.ReturnType = new CodeTypeReference(typeof(double));
      method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Dataset), "dataset"));
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
      foreach(IVariableInfo info in VariableInfos)
        method.Parameters.Add(new CodeParameterDeclarationExpression(info.DataType, info.FormalName));
      method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(double[]), "args"));
      string code = Code;
      method.Statements.Add(new CodeSnippetStatement(code));
      typeDecl.Members.Add(method);

      ns.Types.Add(typeDecl);
      ns.Imports.Add(new CodeNamespaceImport("System"));
      ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
      ns.Imports.Add(new CodeNamespaceImport("System.Text"));
      ns.Imports.Add(new CodeNamespaceImport("HeuristicLab.Core"));
      ns.Imports.Add(new CodeNamespaceImport("HeuristicLab.Functions"));
      foreach(IVariableInfo variableInfo in VariableInfos)
        ns.Imports.Add(new CodeNamespaceImport(variableInfo.DataType.Namespace));

      CodeCompileUnit unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);
      CompilerParameters parameters = new CompilerParameters();
      parameters.GenerateExecutable = false;
      parameters.GenerateInMemory = true;
      parameters.IncludeDebugInformation = false;
      Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach(Assembly loadedAssembly in loadedAssemblies)
        parameters.ReferencedAssemblies.Add(loadedAssembly.Location);
      CodeDomProvider provider = new CSharpCodeProvider();
      CompilerResults results = provider.CompileAssemblyFromDom(parameters, unit);

      applyMethod = null;
      if(results.Errors.HasErrors) {
        StringWriter writer = new StringWriter();
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BlankLinesBetweenMembers = false;
        options.ElseOnClosing = true;
        options.IndentString = "  ";
        provider.GenerateCodeFromCompileUnit(unit, writer, options);
        writer.Flush();
        string[] source = writer.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < source.Length; i++)
          builder.AppendLine((i + 1).ToString("###") + "     " + source[i]);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine();
        foreach(CompilerError error in results.Errors) {
          builder.Append("Line " + error.Line.ToString());
          builder.Append(", Column " + error.Column.ToString());
          builder.AppendLine(": " + error.ErrorText);
        }
        throw new Exception("Compile Errors:\n\n" + builder.ToString());
      } else {
        Assembly assembly = results.CompiledAssembly;
        Type[] types = assembly.GetTypes();
        applyMethod = types[0].GetMethod("Apply");
      }
    }

    #region IFunction Members
    public void Accept(IFunctionVisitor visitor) {
      visitor.Visit(this);
    }

    public IFunctionTree GetTreeNode() {
      return new BakedFunctionTree(this);
    }

    public double Apply(Dataset dataset, int sampleIndex, double[] args) {
      // collect parameters
      ArrayList parameters = new ArrayList(args.Length);
      parameters.Add(dataset);
      parameters.Add(sampleIndex);
      int j = 0;
      List<double> backupValues = new List<double>();
      // all local variables are available in the custom function
      // values of local variables need to be adjusted based on the arguments we recieve from the evaluator
      // because each instance of programmable-function can have different values for the local variables
      foreach(IVariableInfo info in VariableInfos) {
        if(info.Local) {
          IVariable localVariable = GetVariable(info.FormalName);
          double curValue = GetDoubleValue(localVariable);
          backupValues.Add(curValue);
          SetFromDoubleValue(localVariable, args[j++]);
          parameters.Add(localVariable.Value);
        }
      }
      // copy the evaluation results of the sub-branches into a separate array (last argument of user-defined function)
      double[] evaluationResults = new double[args.Length - j];
      Array.Copy(args, j, evaluationResults, 0, evaluationResults.Length);
      parameters.Add(evaluationResults);
      // lazy activation of the user-programmed code
      if(applyMethod == null) {
        Compile();
      }
      double result = (double)applyMethod.Invoke(null, parameters.ToArray());

      // restore backed up values of variables (just a savety measure. changes of the function-library are unwanted)
      j = 0;
      foreach(IVariableInfo info in VariableInfos) {
        if(info.Local) {
          SetFromDoubleValue(GetVariable(info.FormalName), backupValues[j]);
        }
      }
      return result;
    }

    private double GetDoubleValue(IVariable localVariable) {
      IItem value = localVariable.Value;
      if(value is ConstrainedDoubleData) {
        return ((ConstrainedDoubleData)value).Data;
      } else if(value is ConstrainedIntData) {
        return (double)((ConstrainedIntData)value).Data;
      } else if(value is DoubleData) {
        return ((DoubleData)value).Data;
      } else if(value is IntData) {
        return (double)((IntData)value).Data;
      } else throw new NotSupportedException("Datatype of variable " + localVariable.Name + " is not supported as local variable for programmable-functions.");
    }

    private void SetFromDoubleValue(IVariable variable, double x) {
      IItem value = variable.Value;
      if(value is ConstrainedDoubleData) {
        ((ConstrainedDoubleData)value).Data = x;
      } else if(value is ConstrainedIntData) {
        ((ConstrainedIntData)value).Data = (int)x;
      } else if(value is DoubleData) {
        ((DoubleData)value).Data = x;
      } else if(value is IntData) {
        ((IntData)value).Data = (int)x;
      } else throw new NotSupportedException("Datatype of variable " + variable.Name + " is not supported as local variable for programmable-functions.");
    }

    #endregion

    #region disabled operator functionality
    // operator-tree style evaluation is not supported for functions.
    public override IOperation Apply(IScope scope) {
      throw new NotSupportedException();
    }

    private static readonly List<IOperator> emptySubOperatorList = new List<IOperator>();
    public override IList<IOperator> SubOperators {
      get { return emptySubOperatorList; }
    }

    public override void AddSubOperator(IOperator subOperator) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, int index, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }

    public override bool TryAddSubOperator(IOperator subOperator, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }

    public override void AddSubOperator(IOperator subOperator, int index) {
      throw new NotSupportedException();
    }

    public override void RemoveSubOperator(int index) {
      throw new NotSupportedException();
    }

    public override bool TryRemoveSubOperator(int index) {
      throw new NotSupportedException();
    }

    public override bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints) {
      throw new NotSupportedException();
    }
    #endregion
  }
}
