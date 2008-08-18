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
using System.Linq;
using System.Text;
using HeuristicLab.Data;

namespace HeuristicLab.Functions {
  public class ModelAnalyzerExporter : IFunctionVisitor {
    private string prefix;
    private string currentIndent = "";
    private IFunctionTree currentBranch;
    public string ModelAnalyzerPrefix {
      get { return prefix; }
    }
    public void Reset() {
      prefix = "";
    }

    private void VisitFunction(string name, IFunction f) {
      prefix += currentIndent + "[F]" + name + "(\n";
    }

    #region IFunctionVisitor Members

    public void Visit(IFunction function) {
      prefix += function.Name;
    }

    public void Visit(Addition addition) {
      VisitFunction("Addition[0]", addition);
    }

    public void Visit(Constant constant) {
      double value = ((ConstrainedDoubleData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Constant.VALUE).Value).Data;
      prefix += currentIndent + "[T]Constant(" + value.ToString("r") + ";0;0)";
    }

    public void Visit(Cosinus cosinus) {
      VisitFunction("Trigonometrics[1]", cosinus);
    }

    public void Visit(Differential differential) {
      double weight = ((ConstrainedDoubleData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.WEIGHT).Value).Data;
      double index = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.INDEX).Value).Data;
      double offset = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Differential.OFFSET).Value).Data;

      prefix += currentIndent + "[T]Differential(" + weight.ToString("r") + ";" + index + ";" + -offset + ")";
    }

    public void Visit(Division division) {
      VisitFunction("Division[0]", division);
    }

    public void Visit(Exponential exponential) {
      VisitFunction("Exponential[0]", exponential);
    }

    public void Visit(Logarithm logarithm) {
      VisitFunction("Logarithm[0]", logarithm);
    }

    public void Visit(Multiplication multiplication) {
      VisitFunction("Multiplication[0]", multiplication);
    }

    public void Visit(Power power) {
      VisitFunction("Power[0]", power);
    }

    public void Visit(Signum signum) {
      VisitFunction("Signum[0]", signum);
    }

    public void Visit(Sinus sinus) {
      VisitFunction("Trigonometrics[0]", sinus);
    }

    public void Visit(Sqrt sqrt) {
      VisitFunction("Sqrt[0]", sqrt);
    }

    public void Visit(Subtraction substraction) {
      VisitFunction("Subtraction[0]", substraction);
    }

    public void Visit(Tangens tangens) {
      VisitFunction("Trigonometrics[2]", tangens);
    }

    public void Visit(HeuristicLab.Functions.Variable variable) {
      double weight = ((ConstrainedDoubleData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.WEIGHT).Value).Data;
      double index = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.INDEX).Value).Data;
      double offset = ((ConstrainedIntData)currentBranch.GetLocalVariable(HeuristicLab.Functions.Variable.OFFSET).Value).Data;

      prefix += currentIndent + "[T]Variable(" + weight.ToString("r") + ";" + index + ";" + -offset + ")";
    }

    public void Visit(And and) {
      VisitFunction("Logical[0]", and);
    }

    public void Visit(Average average) {
      VisitFunction("N/A (average)", average);
    }

    public void Visit(IfThenElse ifThenElse) {
      VisitFunction("Conditional[0]", ifThenElse);
    }

    public void Visit(Not not) {
      VisitFunction("Logical[2]", not);
    }

    public void Visit(Or or) {
      VisitFunction("Logical[1]", or);
    }

    public void Visit(Xor xor) {
      VisitFunction("N/A (xor)", xor);
    }

    public void Visit(Equal equal) {
      VisitFunction("Boolean[2]", equal);
    }

    public void Visit(LessThan lessThan) {
      VisitFunction("Boolean[0]", lessThan);
    }

    public void Visit(GreaterThan greaterThan) {
      VisitFunction("Boolean[4]", greaterThan);
    }
    #endregion

    public void Visit(IFunctionTree functionTree) {
      currentBranch = functionTree;
      functionTree.Function.Accept(this);
      currentIndent += "  ";
      foreach(IFunctionTree subTree in functionTree.SubTrees) {
        Visit(subTree);
        prefix += ";\n";
      }
      prefix = prefix.TrimEnd(';', '\n');
      if(functionTree.SubTrees.Count>0) prefix += ")";
      currentIndent = currentIndent.Remove(0, 2);
    }
  }
}
