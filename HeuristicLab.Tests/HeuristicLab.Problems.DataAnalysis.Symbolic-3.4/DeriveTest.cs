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
      Assert.AreEqual("0", Derive("3", "x"));
      Assert.AreEqual("1", Derive("x", "x"));
      Assert.AreEqual("10", Derive("10*x", "x"));
      Assert.AreEqual("10", Derive("x*10", "x"));
      Assert.AreEqual("(2*'x')", Derive("x*x", "x"));
      Assert.AreEqual("((('x' * 'x') * 2) + ('x' * 'x'))", Derive("x*x*x", "x")); // simplifier does not merge (x*x)*2 + x*x  to 3*x*x
      Assert.AreEqual("0", Derive("10*x", "y"));
      Assert.AreEqual("20", Derive("10*x+20*y", "y"));
      Assert.AreEqual("6", Derive("2*3*x", "x"));
      Assert.AreEqual("(10*'y')", Derive("10*x*y+20*y", "x"));
      Assert.AreEqual("(1 / (SQR('x') * (-1)))",  Derive("1/x", "x"));
      Assert.AreEqual("('y' / (SQR('x') * (-1)))", Derive("y/x", "x"));
      Assert.AreEqual("((((-2*'x') + (-1)) * ('a' + 'b')) / SQR(('x' + ('x' * 'x'))))",
        Derive("(a+b)/(x+x*x)", "x"));
      Assert.AreEqual("((((-2*'x') + (-1)) * ('a' + 'b')) / SQR(('x' + SQR('x'))))", Derive("(a+b)/(x+SQR(x))", "x"));
      Assert.AreEqual("EXP('x')", Derive("exp(x)", "x"));
      Assert.AreEqual("(EXP((3*'x')) * 3)", Derive("exp(3*x)", "x"));
      Assert.AreEqual("(1 / 'x')", Derive("log(x)", "x"));
      Assert.AreEqual("(1 / 'x')", Derive("log(3*x)", "x"));   // 3 * 1/(3*x)
      Assert.AreEqual("(1 / ('x' + (0.333333333333333*'y')))", Derive("log(3*x+y)", "x"));  // simplifier does not try to keep fractions
      Assert.AreEqual("(1 / (SQRT(((3*'x') + 'y')) * 0.666666666666667))", Derive("sqrt(3*x+y)", "x"));   // 3 / (2 * sqrt(3*x+y)) = 1 / ((2/3) * sqrt(3*x+y)) 
      Assert.AreEqual("(COS((3*'x')) * 3)", Derive("sin(3*x)", "x"));
      Assert.AreEqual("(SIN((3*'x')) * (-3))", Derive("cos(3*x)", "x"));


      // special case: Inv(x) using only one argument to the division symbol
      // f(x) = 1/x
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
      Assert.AreEqual("(1 / (SQR('x') * (-1)))", 
        formatter.Format(DerivativeCalculator.Derive(t, "x")));
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
