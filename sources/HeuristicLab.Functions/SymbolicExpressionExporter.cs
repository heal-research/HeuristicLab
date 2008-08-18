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
  public class SymbolicExpressionExporter : IFunctionVisitor {
    private IFunctionTree currentBranch;
    private StringBuilder builder;
    private string currentIndent;
    public SymbolicExpressionExporter() {
      Reset();
    }

    public void Reset() {
      currentIndent = "";
      builder = new StringBuilder();
    }

    public string GetStringRepresentation() {
      return builder.ToString();
    }

    private void VisitFunction(string name, IFunction f) {
      builder.Append(currentIndent);
      builder.Append("(" + name + " ");
    }

    #region IFunctionVisitor Members

    public void Visit(IFunction function) {
      builder.Append(function.Name);
    }

    public void Visit(Addition addition) {
      VisitFunction("+", addition);
    }

    public void Visit(Constant constant) {
      builder.Append(currentIndent+currentBranch.GetLocalVariable(Constant.VALUE).Value);
    }

    public void Visit(Cosinus cosinus) {
      VisitFunction("cos", cosinus);
    }

    public void Visit(Division division) {
      VisitFunction("/", division);
    }

    public void Visit(Exponential exponential) {
      VisitFunction("exp", exponential);
    }

    public void Visit(Logarithm logarithm) {
      VisitFunction("log", logarithm);
    }

    public void Visit(Multiplication multiplication) {
      VisitFunction("*", multiplication);
    }

    public void Visit(Power power) {
      VisitFunction("expt", power);
    }

    public void Visit(Signum signum) {
      VisitFunction("sign", signum);
    }

    public void Visit(Sinus sinus) {
      VisitFunction("sin", sinus);
    }

    public void Visit(Sqrt sqrt) {
      VisitFunction("sqrt", sqrt);
    }

    public void Visit(Subtraction subtraction) {
      VisitFunction("-", subtraction);
    }

    public void Visit(Tangens tangens) {
      VisitFunction("tan", tangens);
    }

    public void Visit(Variable variable) {
      builder.Append(currentIndent + "(variable " + currentBranch.GetLocalVariable(Variable.WEIGHT).Value + " " +
        currentBranch.GetLocalVariable(Variable.INDEX).Value + " " + currentBranch.GetLocalVariable(Variable.OFFSET).Value + ")");
    }
    public void Visit(Differential differential) {
      builder.Append(currentIndent+"(differential " + currentBranch.GetLocalVariable(Differential.WEIGHT).Value + " "+
        currentBranch.GetLocalVariable(Differential.INDEX).Value+ " " + currentBranch.GetLocalVariable(Differential.OFFSET).Value+")");
    }

    public void Visit(And and) {
      VisitFunction("and", and);
    }

    public void Visit(Average average) {
      VisitFunction("mean", average);
    }

    public void Visit(IfThenElse ifThenElse) {
      VisitFunction("if", ifThenElse);
    }

    public void Visit(Not not) {
      VisitFunction("not", not);
    }

    public void Visit(Or or) {
      VisitFunction("or", or);
    }

    public void Visit(Xor xor) {
      VisitFunction("xor", xor);
    }

    public void Visit(Equal equal) {
      VisitFunction("equ", equal);
    }

    public void Visit(LessThan lessThan) {
      VisitFunction("<", lessThan);
    }

    public void Visit(GreaterThan greaterThan) {
      VisitFunction(">", greaterThan);
    }
    #endregion

    public void Visit(IFunctionTree functionTree) {
      currentBranch = functionTree;
      functionTree.Function.Accept(this);
      currentIndent += "  ";

      foreach(IFunctionTree subTree in functionTree.SubTrees) {
        builder.Append("\n");
        Visit(subTree);
      }
      if(functionTree.SubTrees.Count > 0) builder.Append(")");
      currentIndent = currentIndent.Remove(0, 2);
    }
  }
}
