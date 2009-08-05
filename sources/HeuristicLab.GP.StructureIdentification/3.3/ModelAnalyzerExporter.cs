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

namespace HeuristicLab.GP.StructureIdentification {
  public class ModelAnalyzerExporter : IFunctionTreeSerializer {
    #region IFunctionTreeExporter Members

    public string Name {
      get {
        return "HL2 ModelAnalyzer Exporter";
      }
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

    public string Export(IFunctionTree tree) {
      string result = ExportFunction(tree.Function, tree);
      if(tree.SubTrees.Count>0) 
        result += "(\n";
      foreach(IFunctionTree subTree in tree.SubTrees) {
        result += Export(subTree);
        result += ";\n";
      }
      result = result.TrimEnd(';', '\n');
      if(tree.SubTrees.Count > 0) result += ")";
      return result;
    }

    public IFunctionTree Import(string tree) {
      throw new UnknownFunctionException(tree);
    }

    public bool TryImport(string tree, out IFunctionTree importedTree) {
      try {
        importedTree = Import(tree);
        return true;
      }
      catch (UnknownFunctionException) {
        importedTree = null;
        return false;
      }
    }

    #endregion

    private static string ExportFunction(IFunction function, IFunctionTree tree) {
      // this is smelly, if there is a cleaner way to have a 'dynamic' visitor 
      // please let me know! (gkronber 14.10.2008)
      if(function is Addition) return ((Addition)function).ExportToHL2(tree);
      if(function is And) return ((And)function).ExportToHL2(tree);
      if(function is Average) return ((Average)function).ExportToHL2(tree);
      if(function is Constant) return ((Constant)function).ExportToHL2(tree);
      if(function is Cosinus) return ((Cosinus)function).ExportToHL2(tree);
      if(function is Differential) return ((Differential)function).ExportToHL2(tree);
      if(function is Division) return ((Division)function).ExportToHL2(tree);
      if(function is Equal) return ((Equal)function).ExportToHL2(tree);
      if(function is Exponential) return ((Exponential)function).ExportToHL2(tree);
      if(function is GreaterThan) return ((GreaterThan)function).ExportToHL2(tree);
      if(function is IfThenElse) return ((IfThenElse)function).ExportToHL2(tree);
      if(function is LessThan) return ((LessThan)function).ExportToHL2(tree);
      if(function is Logarithm) return ((Logarithm)function).ExportToHL2(tree);
      if(function is Multiplication) return ((Multiplication)function).ExportToHL2(tree);
      if(function is Not) return ((Not)function).ExportToHL2(tree);
      if(function is Or) return ((Or)function).ExportToHL2(tree);
      if(function is Power) return ((Power)function).ExportToHL2(tree);
      if(function is Signum) return ((Signum)function).ExportToHL2(tree);
      if(function is Sinus) return ((Sinus)function).ExportToHL2(tree);
      if(function is Sqrt) return ((Sqrt)function).ExportToHL2(tree);
      if(function is Subtraction) return ((Subtraction)function).ExportToHL2(tree);
      if(function is Tangens) return ((Tangens)function).ExportToHL2(tree);
      if(function is Variable) return ((Variable)function).ExportToHL2(tree);
      if(function is Xor) return ((Xor)function).ExportToHL2(tree);
      throw new UnknownFunctionException(function.Name);
    }

    public static string GetName(IFunctionTree tree) {
      string name = "";
      try {
        name = ExportFunction(tree.Function, tree);
      } catch(UnknownFunctionException) {
        name = "N/A";
      }
      return name;
    }
  }

  internal static class HL2ExporterExtensions {
    private static string GetHL2FunctionName(string name) {
      return "[F]" + name ;
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
