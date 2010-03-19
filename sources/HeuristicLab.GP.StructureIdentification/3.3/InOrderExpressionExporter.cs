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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace HeuristicLab.GP.StructureIdentification {
  public class InOrderExpressionExporter : IFunctionTreeSerializer {
    private StringBuilder builder;
    private string currentIndent;

    #region IFunctionTreeExporter Members

    public string Name {
      get { return "InOrder Expression Exporter"; }
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
      //builder.Append(currentIndent);
      builder.Append("( ");
      //currentIndent += "  ";
      if (tree.SubTrees.Count > 0) {
        if (tree.SubTrees.Count == 1) {
          builder.Append(ExportFunction(tree.Function, tree));
          builder.Append("(");
          BuildExportString(tree.SubTrees[0]);
          builder.Append(")");
        } else {
          BuildExportString(tree.SubTrees[0]);
          foreach (IFunctionTree subTree in tree.SubTrees.Skip(1)) {
            builder.Append(ExportFunction(tree.Function, tree));
            BuildExportString(subTree);
          }
        }
      } else {
        builder.Append(ExportFunction(tree.Function, tree));
      }
      builder.Append(")");
      //currentIndent = currentIndent.Remove(0, 2);
    }

    // try-export checks the keys of this dictionary
    private static Dictionary<Type, Func<IFunction, IFunctionTree, string>> functionExporter = new Dictionary<Type, Func<IFunction, IFunctionTree, string>>() {
      { typeof(Addition), (function, tree) => ((Addition)function).ExportToExcel()},
      { typeof(Constant), (function, tree) => ((Constant)function).ExportToExcel(tree)},
      { typeof(Differential), (function, tree) => ((Differential)function).ExportToExcel(tree)},
      { typeof(Division), (function, tree) => ((Division)function).ExportToExcel()},
      { typeof(Multiplication), (function, tree) => ((Multiplication)function).ExportToExcel()},
      { typeof(Subtraction), (function, tree) => ((Subtraction)function).ExportToExcel()},
      { typeof(Variable), (function, tree) => ((Variable)function).ExportToExcel(tree)},
      { typeof(Logarithm), (function, tree) => ((Logarithm)function).ExportToExcel()},
      { typeof(Exponential), (function, tree) => ((Exponential)function).ExportToExcel()},
      { typeof(Cosinus), (function, tree) => ((Cosinus)function).ExportToExcel()},
      { typeof(Sinus), (function, tree) => ((Sinus)function).ExportToExcel()},
      { typeof(Tangens), (function, tree) => ((Tangens)function).ExportToExcel()},
      { typeof(Sqrt), (function, tree) => ((Sqrt)function).ExportToExcel()},
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

  internal static class ExcelExporterExtensions {
    public static string ExportToExcel(this Addition addition) {
      return "+";
    }

    public static string ExportToExcel(this Constant constant, IFunctionTree tree) {
      return ((ConstantFunctionTree)tree).Value.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
    }

    public static string ExportToExcel(this Division division) {
      return "/";
    }

    public static string ExportToExcel(this Multiplication multiplication) {
      return "*";
    }

    public static string ExportToExcel(this Subtraction subtraction) {
      return "-";
    }

    public static string ExportToExcel(this Exponential exp) {
      return "Exp";
    }

    public static string ExportToExcel(this Logarithm log) {
      return "Log";
    }
    public static string ExportToExcel(this Cosinus cos) {
      return "Cos";
    }
    public static string ExportToExcel(this Sinus sin) {
      return "Sin";
    }
    public static string ExportToExcel(this Tangens tan) {
      return "Tan";
    }
    public static string ExportToExcel(this Sqrt sqrt) {
      return "Sqrt";
    }
    public static string ExportToExcel(this Variable variable, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return " " + varTree.Weight.ToString("r", CultureInfo.InvariantCulture.NumberFormat) + " * " +
        varTree.VariableName + " " + varTree.SampleOffset;
    }
    public static string ExportToExcel(this Differential differential, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return varTree.Weight.ToString("r", CultureInfo.InvariantCulture.NumberFormat) + " * d(" +
        varTree.VariableName + ") " + varTree.SampleOffset;
    }
  }
}
