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
using System.Globalization;

namespace HeuristicLab.GP.StructureIdentification {
  public class SymbolicExpressionExporter : IFunctionTreeSerializer {
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
      foreach (IFunctionTree t in FunctionTreeIterator.IteratePrefix(tree))
        if (!functionExporter.ContainsKey(t.Function.GetType())) {
          exported = null;
          return false;
        }
      exported = Export(tree);
      return true;
    }

    private void BuildExportString(IFunctionTree tree) {
      builder.Append(currentIndent);
      builder.Append("(" + ExportFunction(tree.Function, tree) + " ");
      currentIndent += "  ";
      foreach (IFunctionTree subTree in tree.SubTrees) {
        builder.Append("\n");
        BuildExportString(subTree);
      }
      builder.Append(")");
      currentIndent = currentIndent.Remove(0, 2);
    }

    // try-export checks the keys of this dictionary
    private static Dictionary<Type, Func<IFunction, IFunctionTree, string>> functionExporter = new Dictionary<Type, Func<IFunction, IFunctionTree, string>>() {
      { typeof(Addition), (function, tree) => ((Addition)function).ExportToScheme()},
      { typeof(And), (function, tree) => ((And)function).ExportToScheme()},
      { typeof(Average), (function, tree) => ((Average)function).ExportToScheme()},
      { typeof(Constant), (function, tree) => ((Constant)function).ExportToScheme(tree)},
      { typeof(Cosinus), (function, tree) => ((Cosinus)function).ExportToScheme()},
      { typeof(Differential), (function, tree) => ((Differential)function).ExportToScheme(tree)},
      { typeof(Division), (function, tree) => ((Division)function).ExportToScheme()},
      { typeof(Equal), (function, tree) => ((Equal)function).ExportToScheme()},
      { typeof(Exponential), (function, tree) => ((Exponential)function).ExportToScheme()},
      { typeof(GreaterThan), (function, tree) => ((GreaterThan)function).ExportToScheme()},
      { typeof(IfThenElse), (function, tree) => ((IfThenElse)function).ExportToScheme()},
      { typeof(LessThan), (function, tree) => ((LessThan)function).ExportToScheme()},
      { typeof(Logarithm), (function, tree) => ((Logarithm)function).ExportToScheme()},
      { typeof(Multiplication), (function, tree) => ((Multiplication)function).ExportToScheme()},
      { typeof(Not), (function, tree) => ((Not)function).ExportToScheme()},
      { typeof(Or), (function, tree) => ((Or)function).ExportToScheme()},
      { typeof(Power), (function, tree) => ((Power)function).ExportToScheme()},
      { typeof(Signum), (function, tree) => ((Signum)function).ExportToScheme()},
      { typeof(Sinus), (function, tree) => ((Sinus)function).ExportToScheme()},
      { typeof(Sqrt), (function, tree) => ((Sqrt)function).ExportToScheme()},
      { typeof(Subtraction), (function, tree) => ((Subtraction)function).ExportToScheme()},
      { typeof(Tangens), (function, tree) => ((Tangens)function).ExportToScheme()},
      { typeof(Variable), (function, tree) => ((Variable)function).ExportToScheme(tree)},
      { typeof(Xor), (function, tree) => ((Xor)function).ExportToScheme()},
    };
    private static string ExportFunction(IFunction function, IFunctionTree tree) {
      // this is smelly, if there is a cleaner way to have a 'dynamic' visitor 
      // please let me know! (gkronber 14.10.2008)
      if (functionExporter.ContainsKey(function.GetType())) return functionExporter[function.GetType()](function, tree);
      else return function.Name;
    }

    #endregion

    public static string GetName(IFunctionTree tree) {
      return ExportFunction(tree.Function, tree);
    }
  }

  internal static class SchemeExporterExtensions {
    public static string ExportToScheme(this Addition addition) {
      return "+";
    }

    public static string ExportToScheme(this Constant constant, IFunctionTree tree) {
      return ((ConstantFunctionTree)tree).Value.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
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
      return "variable " + varTree.Weight.ToString("r", CultureInfo.InvariantCulture.NumberFormat) + " " +
        varTree.VariableName + " " + varTree.SampleOffset;
    }
    public static string ExportToScheme(this Differential differential, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return "differential " + varTree.Weight.ToString("r", CultureInfo.InvariantCulture.NumberFormat) + " " +
        varTree.VariableName + " " + varTree.SampleOffset;
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
