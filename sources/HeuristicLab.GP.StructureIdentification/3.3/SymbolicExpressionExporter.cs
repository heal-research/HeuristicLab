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

using System.Text;
using HeuristicLab.GP.Interfaces;
using System;
using System.Collections.Generic;

namespace HeuristicLab.GP.StructureIdentification {
  public class SymbolicExpressionExporter : IFunctionTreeSerializer, IFunctionTreeNameGenerator {
    private StringBuilder builder;
    private string currentIndent;

    #region IFunctionTreeExporter Members

    public string Name {
      get { return "Symbolic Expression Exporter"; }
    }


    public string Export(IFunctionTree tree) {
      builder = new StringBuilder();
      currentIndent = "";
      BuildExportString(tree);
      return builder.ToString();
    }

    public bool TryExport(IFunctionTree tree, out string exported) {
      try {
        exported = Export(tree);
        return true;
      } catch(UnknownFunctionException) {
        exported = "";
        return false;
      }
    }

    private void BuildExportString(IFunctionTree tree) {
      builder.Append(currentIndent);
      builder.Append("(" + ExportFunction(tree.Function, tree) + " ");
      currentIndent += "  ";
      foreach(IFunctionTree subTree in tree.SubTrees) {
        builder.Append("\n");
        BuildExportString(subTree);
      }
      if(tree.SubTrees.Count > 0) builder.Append(")");
      currentIndent = currentIndent.Remove(0, 2);
    }

    private string ExportFunction(IFunction function, IFunctionTree tree) {
      // this is smelly, if there is a cleaner way to have a 'dynamic' visitor 
      // please let me know! (gkronber 14.10.2008)
      if(function is Addition) return ((Addition)function).ExportToScheme();
      if(function is And) return ((And)function).ExportToScheme();
      if(function is Average) return ((Average)function).ExportToScheme();
      if(function is Constant) return ((Constant)function).ExportToScheme(tree);
      if(function is Cosinus) return ((Cosinus)function).ExportToScheme();
      if(function is Differential) return ((Differential)function).ExportToScheme(tree);
      if(function is Division) return ((Division)function).ExportToScheme();
      if(function is Equal) return ((Equal)function).ExportToScheme();
      if(function is Exponential) return ((Exponential)function).ExportToScheme();
      if(function is GreaterThan) return ((GreaterThan)function).ExportToScheme();
      if(function is IfThenElse) return ((IfThenElse)function).ExportToScheme();
      if(function is LessThan) return ((LessThan)function).ExportToScheme();
      if(function is Logarithm) return ((Logarithm)function).ExportToScheme();
      if(function is Multiplication) return ((Multiplication)function).ExportToScheme();
      if(function is Not) return ((Not)function).ExportToScheme();
      if(function is Or) return ((Or)function).ExportToScheme();
      if(function is Power) return ((Power)function).ExportToScheme();
      if(function is Signum) return ((Signum)function).ExportToScheme();
      if(function is Sinus) return ((Sinus)function).ExportToScheme();
      if(function is Sqrt) return ((Sqrt)function).ExportToScheme();
      if(function is Subtraction) return ((Subtraction)function).ExportToScheme();
      if(function is Tangens) return ((Tangens)function).ExportToScheme();
      if(function is Variable) return ((Variable)function).ExportToScheme(tree);
      if(function is Xor) return ((Xor)function).ExportToScheme();
      throw new UnknownFunctionException(function.Name);
    }


    #endregion

    #region IFunctionTreeNameGenerator Members

    string IFunctionTreeNameGenerator.Name {
      get { return "Symbolic Expression"; }
    }

    public string GetName(IFunctionTree tree) {
      string name = "";
      try {
        name = ExportFunction(tree.Function, tree);
      } catch(UnknownFunctionException) {
        name = "N/A";
      }
      return name;
    }

    #endregion
  }

  internal static class SchemeExporterExtensions {
    public static string ExportToScheme(this Addition addition) {
      return "+";
    }

    public static string ExportToScheme(this Constant constant, IFunctionTree tree) {
      return ((ConstantFunctionTree)tree).Value.ToString("r");
    }

    public static string ExportToScheme(this Cosinus cosinus) {
      return "cos";
    }

    public static string ExportToScheme(this Division division) {
      return "/";
    }

    public static string ExportToScheme(this Exponential exponential) {
      return "exp";
    }

    public static string ExportToScheme(this Logarithm logarithm) {
      return "log";
    }

    public static string ExportToScheme(this Multiplication multiplication) {
      return "*";
    }

    public static string ExportToScheme(this Power power) {
      return "expt";
    }

    public static string ExportToScheme(this Signum signum) {
      return "sign";
    }

    public static string ExportToScheme(this Sinus sinus) {
      return "sin";
    }

    public static string ExportToScheme(this Sqrt sqrt) {
      return "sqrt";
    }

    public static string ExportToScheme(this Subtraction subtraction) {
      return "-";
    }

    public static string ExportToScheme(this Tangens tangens) {
      return "tan";
    }

    public static string ExportToScheme(this Variable variable, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return "(variable " + varTree.Weight.ToString("r") + " " +
        varTree.VariableName + " " + varTree.SampleOffset + ")";
    }
    public static string ExportToScheme(this Differential differential, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return "(differential " + varTree.Weight.ToString("r") + " " +
        varTree.VariableName + " " + varTree.SampleOffset + ")";
    }

    public static string ExportToScheme(this And and) {
      return "and";
    }

    public static string ExportToScheme(this Average average) {
      return "mean";
    }

    public static string ExportToScheme(this IfThenElse ifThenElse) {
      return "if";
    }

    public static string ExportToScheme(this Not not) {
      return "not";
    }

    public static string ExportToScheme(this Or or) {
      return "or";
    }

    public static string ExportToScheme(this Xor xor) {
      return "xor";
    }

    public static string ExportToScheme(this Equal equal) {
      return "equ";
    }

    public static string ExportToScheme(this LessThan lessThan) {
      return "<";
    }

    public static string ExportToScheme(this GreaterThan greaterThan) {
      return ">";
    }
  }
}
