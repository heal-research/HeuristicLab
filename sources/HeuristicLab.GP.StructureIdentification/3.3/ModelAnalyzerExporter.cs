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

using HeuristicLab.GP.Interfaces;
using System;
using System.Collections.Generic;

namespace HeuristicLab.GP.StructureIdentification {
  public class ModelAnalyzerExporter : IFunctionTreeSerializer {
    #region IFunctionTreeExporter Members

    public string Name {
      get {
        return "HL2 ModelAnalyzer Exporter";
      }
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

    public string Export(IFunctionTree tree) {
      string result = ExportFunction(tree.Function, tree);
      if (tree.SubTrees.Count > 0)
        result += "(\n";
      foreach (IFunctionTree subTree in tree.SubTrees) {
        result += Export(subTree);
        result += ";\n";
      }
      result = result.TrimEnd(';', '\n');
      if (tree.SubTrees.Count > 0) result += ")";
      return result;
    }
    #endregion

    // try-export checks the keys of this dictionary
    private static Dictionary<Type, Func<IFunction, IFunctionTree, string>> functionExporter = new Dictionary<Type, Func<IFunction, IFunctionTree, string>>() {
      { typeof(Addition), (function, tree) => ((Addition)function).ExportToHL2(tree)},
      { typeof(And), (function, tree) => ((And)function).ExportToHL2(tree)},
      { typeof(Average), (function, tree) => ((Average)function).ExportToHL2(tree)},
      { typeof(Constant), (function, tree) => ((Constant)function).ExportToHL2(tree)},
      { typeof(Cosinus), (function, tree) => ((Cosinus)function).ExportToHL2(tree)},
      { typeof(Differential), (function, tree) => ((Differential)function).ExportToHL2(tree)},
      { typeof(Division), (function, tree) => ((Division)function).ExportToHL2(tree)},
      { typeof(Equal), (function, tree) => ((Equal)function).ExportToHL2(tree)},
      { typeof(Exponential), (function, tree) => ((Exponential)function).ExportToHL2(tree)},
      { typeof(GreaterThan), (function, tree) => ((GreaterThan)function).ExportToHL2(tree)},
      { typeof(IfThenElse), (function, tree) => ((IfThenElse)function).ExportToHL2(tree)},
      { typeof(LessThan), (function, tree) => ((LessThan)function).ExportToHL2(tree)},
      { typeof(Logarithm), (function, tree) => ((Logarithm)function).ExportToHL2(tree)},
      { typeof(Multiplication), (function, tree) => ((Multiplication)function).ExportToHL2(tree)},
      { typeof(Not), (function, tree) => ((Not)function).ExportToHL2(tree)},
      { typeof(Or), (function, tree) => ((Or)function).ExportToHL2(tree)},
      { typeof(Power), (function, tree) => ((Power)function).ExportToHL2(tree)},
      { typeof(Signum), (function, tree) => ((Signum)function).ExportToHL2(tree)},
      { typeof(Sinus), (function, tree) => ((Sinus)function).ExportToHL2(tree)},
      { typeof(Sqrt), (function, tree) => ((Sqrt)function).ExportToHL2(tree)},
      { typeof(Subtraction), (function, tree) => ((Subtraction)function).ExportToHL2(tree)},
      { typeof(Tangens), (function, tree) => ((Tangens)function).ExportToHL2(tree)},
      { typeof(Variable), (function, tree) => ((Variable)function).ExportToHL2(tree)},
      { typeof(Xor), (function, tree) => ((Xor)function).ExportToHL2(tree)},
    };
    private static string ExportFunction(IFunction function, IFunctionTree tree) {
      // this is smelly, if there is a cleaner way to have a 'dynamic' visitor 
      // please let me know! (gkronber 14.10.2008)
      if (functionExporter.ContainsKey(function.GetType())) return functionExporter[function.GetType()](function, tree);
      else return function.Name;
    }

    public static string GetName(IFunctionTree tree) {
      return ExportFunction(tree.Function, tree);
    }
  }

  internal static class HL2ExporterExtensions {
    private static string GetHL2FunctionName(string name) {
      return "[F]" + name;
    }

    public static string ExportToHL2(this Addition addition, IFunctionTree tree) {
      return GetHL2FunctionName("Addition[0]");
    }

    public static string ExportToHL2(this Constant constant, IFunctionTree tree) {
      double value = ((ConstantFunctionTree)tree).Value;
      return "[T]Constant(" + value.ToString("r") + ";0;0)";
    }

    public static string ExportToHL2(this Cosinus cosinus, IFunctionTree tree) {
      return GetHL2FunctionName("Trigonometrics[1]");
    }

    public static string ExportToHL2(this Differential differential, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return "[T]Differential(" + varTree.Weight.ToString("r") + ";" + varTree.VariableName + ";" + -varTree.SampleOffset + ")";
    }

    public static string ExportToHL2(this Division division, IFunctionTree tree) {
      return GetHL2FunctionName("Division[0]");
    }

    public static string ExportToHL2(this Exponential exponential, IFunctionTree tree) {
      return GetHL2FunctionName("Exponential[0]");
    }

    public static string ExportToHL2(this Logarithm logarithm, IFunctionTree tree) {
      return GetHL2FunctionName("Logarithm[0]");
    }

    public static string ExportToHL2(this Multiplication multiplication, IFunctionTree tree) {
      return GetHL2FunctionName("Multiplication[0]");
    }

    public static string ExportToHL2(this Power power, IFunctionTree tree) {
      return GetHL2FunctionName("Power[0]");
    }

    public static string ExportToHL2(this Signum signum, IFunctionTree tree) {
      return GetHL2FunctionName("Signum[0]");
    }

    public static string ExportToHL2(this Sinus sinus, IFunctionTree tree) {
      return GetHL2FunctionName("Trigonometrics[0]");
    }

    public static string ExportToHL2(this Sqrt sqrt, IFunctionTree tree) {
      return GetHL2FunctionName("Sqrt[0]");
    }

    public static string ExportToHL2(this Subtraction substraction, IFunctionTree tree) {
      return GetHL2FunctionName("Subtraction[0]");
    }

    public static string ExportToHL2(this Tangens tangens, IFunctionTree tree) {
      return GetHL2FunctionName("Trigonometrics[2]");
    }

    public static string ExportToHL2(this Variable variable, IFunctionTree tree) {
      var varTree = (VariableFunctionTree)tree;
      return "[T]Variable(" + varTree.Weight.ToString("r") + ";" + varTree.VariableName + ";" + -varTree.SampleOffset + ")";
    }

    public static string ExportToHL2(this And and, IFunctionTree tree) {
      return GetHL2FunctionName("Logical[0]");
    }

    public static string ExportToHL2(this Average average, IFunctionTree tree) {
      return GetHL2FunctionName("Average[0]");
    }

    public static string ExportToHL2(this IfThenElse ifThenElse, IFunctionTree tree) {
      return GetHL2FunctionName("Conditional[0]");
    }

    public static string ExportToHL2(this Not not, IFunctionTree tree) {
      return GetHL2FunctionName("Logical[2]");
    }

    public static string ExportToHL2(this Or or, IFunctionTree tree) {
      return GetHL2FunctionName("Logical[1]");
    }

    public static string ExportToHL2(this Xor xor, IFunctionTree tree) {
      return GetHL2FunctionName("Logical[3]");
    }

    public static string ExportToHL2(this Equal equal, IFunctionTree tree) {
      return GetHL2FunctionName("Boolean[2]");
    }

    public static string ExportToHL2(this LessThan lessThan, IFunctionTree tree) {
      return GetHL2FunctionName("Boolean[0]");
    }

    public static string ExportToHL2(this GreaterThan greaterThan, IFunctionTree tree) {
      return GetHL2FunctionName("Boolean[4]");
    }
  }
}
