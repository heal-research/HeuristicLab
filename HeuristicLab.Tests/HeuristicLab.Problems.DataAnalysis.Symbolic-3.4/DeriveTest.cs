#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {


  [TestClass]
  public class DerivativeTest {
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void DeriveExpressions() {
      var formatter = new InfixExpressionFormatter();
      var parser = new InfixExpressionParser();
      Console.WriteLine(Derive("3", "x"));
      Console.WriteLine(Derive("x", "x"));
      Console.WriteLine(Derive("10*x", "x"));
      Console.WriteLine(Derive("x*10", "x"));
      Console.WriteLine(Derive("x*x", "x"));
      Console.WriteLine(Derive("x*x*x", "x"));
      Console.WriteLine(Derive("10*x", "y"));
      Console.WriteLine(Derive("10*x+20*y", "y"));
      Console.WriteLine(Derive("2*3*x", "x"));
      Console.WriteLine(Derive("10*x*y+20*y", "x"));
      Console.WriteLine(Derive("1/x", "x"));
      Console.WriteLine(Derive("y/x", "x"));
      Console.WriteLine(Derive("(a+b)/(x+x*x)", "x"));
      Console.WriteLine(Derive("(a+b)/(x+SQR(x))", "x"));
      Console.WriteLine(Derive("exp(x)", "x"));
      Console.WriteLine(Derive("exp(3*x)", "x"));
      Console.WriteLine(Derive("log(x)", "x"));
      Console.WriteLine(Derive("log(3*x)", "x"));
      Console.WriteLine(Derive("log(3*x+y)", "x"));
      Console.WriteLine(Derive("sqrt(3*x+y)", "x"));
      Console.WriteLine(Derive("sin(3*x)", "x"));
      Console.WriteLine(Derive("cos(3*x)", "x"));

      // special case: Inv(x) using only one argument to the division symbol
      var root = new ProgramRootSymbol().CreateTreeNode();
      var start = new StartSymbol().CreateTreeNode();
      var div = new Division().CreateTreeNode();
      var varNode = (VariableTreeNode)(new Variable().CreateTreeNode());
      varNode.Weight = 1.0;
      varNode.VariableName = "x";
      div.AddSubtree(varNode);
      start.AddSubtree(div);
      root.AddSubtree(start);
      var t = new SymbolicExpressionTree(root);
      Console.WriteLine(formatter.Format(DerivativeCalculator.Derive(t, "x")));
    }

    private string Derive(string expr, string variable) {
      var parser = new InfixExpressionParser();
      var formatter = new InfixExpressionFormatter();

      var t = parser.Parse(expr);
      var tPrime = DerivativeCalculator.Derive(t, variable);

      return formatter.Format(tPrime);
    }
  }
}
