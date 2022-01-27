#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Tests {


  [TestClass]
  public class InfixExpressionParserTest {
    [TestMethod]
    [TestCategory("Problems.DataAnalysis.Symbolic")]
    [TestProperty("Time", "short")]
    public void InfixExpressionParserTestFormatting() {
      var formatter = new InfixExpressionFormatter();
      var parser = new InfixExpressionParser();
      Assert.AreEqual("3", formatter.Format(parser.Parse("3")));
      Assert.AreEqual("3 * 3", formatter.Format(parser.Parse("3*3")));
      Assert.AreEqual("3 * 4", formatter.Format(parser.Parse("3 * 4")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123E-03")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123e-03")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123e+03")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123E+03")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123.0E-03")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123.0e-03")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123.0e+03")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123.0E+03")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123.0E-3")));
      Assert.AreEqual("0.123", formatter.Format(parser.Parse("123.0e-3")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123.0e+3")));
      Assert.AreEqual("123000", formatter.Format(parser.Parse("123.0E+3")));

      Assert.AreEqual("3.1415 + 2", formatter.Format(parser.Parse("3.1415+2.0")));
      Assert.AreEqual("3.1415 / 2", formatter.Format(parser.Parse("3.1415/2.0")));
      Assert.AreEqual("3.1415 * 2", formatter.Format(parser.Parse("3.1415*2.0")));
      Assert.AreEqual("3.1415 - 2", formatter.Format(parser.Parse("3.1415-2.0")));
      // round-trip
      Assert.AreEqual("3.1415 - 2", formatter.Format(parser.Parse(formatter.Format(parser.Parse("3.1415-2.0")))));
      Assert.AreEqual("3.1415 + 2", formatter.Format(parser.Parse("3.1415+(2.0)")));
      Assert.AreEqual("3.1415 + 2", formatter.Format(parser.Parse("(3.1415+(2.0))")));


      Assert.AreEqual("LOG(3)", formatter.Format(parser.Parse("log(3)")));
      Assert.AreEqual("LOG(-3)", formatter.Format(parser.Parse("log(-3)")));
      Assert.AreEqual("EXP(3)", formatter.Format(parser.Parse("exp(3)")));
      Assert.AreEqual("EXP(-3)", formatter.Format(parser.Parse("exp(-3)")));
      Assert.AreEqual("SQRT(3)", formatter.Format(parser.Parse("sqrt(3)")));

      Assert.AreEqual("SQR(-3)", formatter.Format(parser.Parse("sqr((-3))")));

      Assert.AreEqual("3 / 3 + 2 / 2 + 1 / 1", formatter.Format(parser.Parse("3/3+2/2+1/1")));
      Assert.AreEqual("-3 + 30 - 2 + 20 - 1 + 10", formatter.Format(parser.Parse("-3+30-2+20-1+10")));

      // 'flattening' of nested addition, subtraction, multiplication, or division
      Assert.AreEqual("1 + 2 + 3 + 4", formatter.Format(parser.Parse("1 + 2 + 3 + 4")));
      Assert.AreEqual("1 - 2 - 3 - 4", formatter.Format(parser.Parse("1 - 2 - 3 - 4")));
      Assert.AreEqual("1 * 2 * 3 * 4", formatter.Format(parser.Parse("1 * 2 * 3 * 4")));
      Assert.AreEqual("1 / 2 / 3 / 4", formatter.Format(parser.Parse("1 / 2 / 3 / 4")));

      // signed variables / constants
      Assert.AreEqual("-1 * 'x1'", formatter.Format(parser.Parse("-x1")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("--1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("----1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("-+-1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("+-+-1.0")));
      Assert.AreEqual("-3 + -1", formatter.Format(parser.Parse("-3 + -1.0")));


      Assert.AreEqual("'x1'", formatter.Format(parser.Parse("\"x1\"")));
      Assert.AreEqual("'var name'", formatter.Format(parser.Parse("\'var name\'")));
      Assert.AreEqual("'var name'", formatter.Format(parser.Parse("\"var name\"")));
      Assert.AreEqual("'1'", formatter.Format(parser.Parse("\"1\"")));

      Assert.AreEqual("'var \" name'", formatter.Format(parser.Parse("'var \" name\'")));
      Assert.AreEqual("\"var ' name\"", formatter.Format(parser.Parse("\"var \' name\"")));


      Assert.AreEqual("'x1' * 'x2'", formatter.Format(parser.Parse("\"x1\"*\"x2\"")));
      Assert.AreEqual("'x1' * 'x2' + 'x3' * 'x4'", formatter.Format(parser.Parse("\"x1\"*\"x2\"+\"x3\"*\"x4\"")));
      Assert.AreEqual("'x1' * 'x2' + 'x3' * 'x4'", formatter.Format(parser.Parse("x1*x2+x3*x4")));


      Assert.AreEqual("3 ^ 2", formatter.Format(parser.Parse("POW(3, 2)")));
      Assert.AreEqual("3.1 ^ 2.1", formatter.Format(parser.Parse("POW(3.1, 2.1)")));
      Assert.AreEqual("3.1 ^ 2.1", formatter.Format(parser.Parse("POW(3.1 , 2.1)")));
      Assert.AreEqual("3.1 ^ 2.1", formatter.Format(parser.Parse("POW(3.1 ,2.1)")));
      Assert.AreEqual("-3.1 ^ -2.1", formatter.Format(parser.Parse("POW(-3.1 , - 2.1)")));
      Assert.AreEqual("ROOT(3, 2)", formatter.Format(parser.Parse("ROOT(3, 2)")));
      Assert.AreEqual("ROOT(3.1, 2.1)", formatter.Format(parser.Parse("ROOT(3.1, 2.1)")));
      Assert.AreEqual("ROOT(3.1, 2.1)", formatter.Format(parser.Parse("ROOT(3.1 , 2.1)")));
      Assert.AreEqual("ROOT(3.1, 2.1)", formatter.Format(parser.Parse("ROOT(3.1 ,2.1)")));
      Assert.AreEqual("ROOT(-3.1, -2.1)", formatter.Format(parser.Parse("ROOT(-3.1 , -2.1)")));

      Assert.AreEqual("IF(GT(0, 1), 1, 0)", formatter.Format(parser.Parse("IF(GT( 0, 1), 1, 0)")));
      Assert.AreEqual("IF(LT(0, 1), 1, 0)", formatter.Format(parser.Parse("IF(LT(0,1), 1 , 0)")));
      Assert.AreEqual("LAG('x', 1)", formatter.Format(parser.Parse("LAG(x, 1)")));
      Assert.AreEqual("LAG('x', -1)", formatter.Format(parser.Parse("LAG(x, -1)")));
      Assert.AreEqual("LAG('x', 1)", formatter.Format(parser.Parse("LAG(x, +1)")));
      Assert.AreEqual("'x' * LAG('x', 1)", formatter.Format(parser.Parse("x * LAG('x', +1)")));

      // factor variables
      Assert.AreEqual("'x'[1] * 'y'", formatter.Format(parser.Parse("x [1.0] * y")));
      Assert.AreEqual("'x'[1, 2] * 'y'[1, 2]", formatter.Format(parser.Parse("x [1.0, 2.0] * y [1.0, 2.0]")));
      Assert.AreEqual("'x'[1] * 'y'", formatter.Format(parser.Parse("x[1] * y")));
      Assert.AreEqual("'x'[1, 2] * 'y'[1, 2]", formatter.Format(parser.Parse("x[1, 2] * y [1, 2]")));
      Assert.AreEqual("'x'[1] * 'y'", formatter.Format(parser.Parse("x [+1.0] * y")));
      Assert.AreEqual("'x'[-1] * 'y'", formatter.Format(parser.Parse("x [-1.0] * y")));
      Assert.AreEqual("'x'[-1, -2] * 'y'[1, 2]", formatter.Format(parser.Parse("x [-1.0, -2.0] * y [+1.0, +2.0]")));

      // one-hot for factor
      Assert.AreEqual("'x' = 'val' * 'y'", formatter.Format(parser.Parse("x='val' * y")));
      Assert.AreEqual("'x' = 'val'", formatter.Format(parser.Parse("x = 'val'")));
      Assert.AreEqual("'x' = 'val'", formatter.Format(parser.Parse("x = \"val\"")));
      Assert.AreEqual("1 * 'x' = 'val'", formatter.Format(parser.Parse("1.0 * x = val")));
      Assert.AreEqual("-1 * 'x' = 'val'", formatter.Format(parser.Parse("-1.0 * x = val")));
      Assert.AreEqual("1 * 'x' = 'val1' + 'y' = 'val2'", formatter.Format(parser.Parse("+1.0 * \"x\" = val1 + y = \"val2\"")));

      // numeric parameters
      Assert.AreEqual("0", formatter.Format(parser.Parse("<num>"))); // default initial value is zero
      Assert.AreEqual("0", formatter.Format(parser.Parse("< num >")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("< num=1.0>")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("< num = 1.0>")));
      Assert.AreEqual("-1", formatter.Format(parser.Parse("< num =-1.0>")));
      Assert.AreEqual("-1", formatter.Format(parser.Parse("< num = - 1.0>")));

      // numeric parameter with sign
      Assert.AreEqual("1", formatter.Format(parser.Parse("-<num=-1.0>")));

      // nested functions
      Assert.AreEqual("SIN(SIN(SIN('X1')))", formatter.Format(parser.Parse("SIN(SIN(SIN(X1)))")));

      {
        // a tree with single-argument multiplication and addition
        //   ...
        //    *
        //    |
        //    +
        //   / \
        //  v1 v2
        // 
        // is still formatted as (v1 + v2) even though it is not strictly necessary
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var mul = new Multiplication().CreateTreeNode();
        var add = new Addition().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        add.AddSubtree(var1);
        add.AddSubtree(var2);
        mul.AddSubtree(add);
        start.AddSubtree(mul);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("'x1' + 'x2'", formatter.Format(t));
      }
      {
        //    *
        //    |\
        //    * v3
        //    |
        //    +
        //   / \
        //  v1 v2
        // 
        // is still formatted as (v1 + v2) even though it is not strictly necessary
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var mul1 = new Multiplication().CreateTreeNode();
        var mul2 = new Multiplication().CreateTreeNode();
        var add = new Addition().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        add.AddSubtree(var1);
        add.AddSubtree(var2);
        mul2.AddSubtree(add);
        mul1.AddSubtree(mul2);
        mul1.AddSubtree(var3);
        start.AddSubtree(mul1);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("('x1' + 'x2') * 'x3'", formatter.Format(t));
      }

      {
        //   sin
        //    |
        //    * 
        //    |
        //    +
        //   / \
        //  v1 v2
        // 
        // is still formatted as (v1 + v2) even though it is not strictly necessary
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var sin = new Sine().CreateTreeNode();
        var mul = new Multiplication().CreateTreeNode();
        var add = new Addition().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        add.AddSubtree(var1);
        add.AddSubtree(var2);
        mul.AddSubtree(add);
        sin.AddSubtree(mul);
        start.AddSubtree(sin);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("SIN('x1' + 'x2')", formatter.Format(t));
      }
      {
        // single-argument subtraction
        //    -
        //    |
        //    v1
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var sub = new Subtraction().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = -1.0;
        sub.AddSubtree(var1);
        start.AddSubtree(sub);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("-(-1 * 'x1')", formatter.Format(t));       // TODO: same as --1 * 'x1' and just 'x1'
      }
      {
        // single-argument subtraction
        //    -
        //    |
        //    +
        //   / \
        //  v1 v2
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var sub = new Subtraction().CreateTreeNode();
        var add = new Addition().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        add.AddSubtree(var1);
        add.AddSubtree(var2);
        sub.AddSubtree(add);
        start.AddSubtree(sub);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("-('x1' + 'x2')", formatter.Format(t));
      }
      {
        //     ^
        //    / \
        //    *  v3
        //   / \
        //  v1 v2
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var pow = new Power().CreateTreeNode();
        var mul = new Multiplication().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        mul.AddSubtree(var1);
        mul.AddSubtree(var2);
        pow.AddSubtree(mul);
        pow.AddSubtree(var3);
        start.AddSubtree(pow);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("('x1' * 'x2') ^ 'x3'", formatter.Format(t));
      }
      {
        //       ^
        //     /   \
        //    *     *
        //   / \   / \
        //  v1 v2 v3 v4
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var pow = new Power().CreateTreeNode();
        var mul1 = new Multiplication().CreateTreeNode();
        var mul2 = new Multiplication().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        var var4 = (VariableTreeNode)new Variable().CreateTreeNode(); var4.VariableName = "x4"; var4.Weight = 1.0;
        mul1.AddSubtree(var1);
        mul1.AddSubtree(var2);
        mul2.AddSubtree(var3);
        mul2.AddSubtree(var4);
        pow.AddSubtree(mul1);
        pow.AddSubtree(mul2);
        start.AddSubtree(pow);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("('x1' * 'x2') ^ ('x3' * 'x4')", formatter.Format(t));
      }
      {
        //       *
        //     /   \
        //    *     /
        //   / \   / \
        //  v1 v2 v3 v4
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var mul1 = new Multiplication().CreateTreeNode();
        var mul2 = new Multiplication().CreateTreeNode();
        var div = new Division().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        var var4 = (VariableTreeNode)new Variable().CreateTreeNode(); var4.VariableName = "x4"; var4.Weight = 1.0;
        mul2.AddSubtree(var1);
        mul2.AddSubtree(var2);
        div.AddSubtree(var3);
        div.AddSubtree(var4);
        mul1.AddSubtree(mul2);
        mul1.AddSubtree(div);
        start.AddSubtree(mul1);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("'x1' * 'x2' * 'x3' / 'x4'", formatter.Format(t));     // same as x1 * x2 * (x3 / x4)
      }
      {
        //       *
        //     /   \
        //    div  div
        //   / \   / \
        //  v1 v2 v3 v4
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var mul = new Multiplication().CreateTreeNode();
        var div1 = new Division().CreateTreeNode();
        var div2 = new Division().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        var var4 = (VariableTreeNode)new Variable().CreateTreeNode(); var4.VariableName = "x4"; var4.Weight = 1.0;
        div1.AddSubtree(var1);
        div1.AddSubtree(var2);
        div2.AddSubtree(var3);
        div2.AddSubtree(var4);
        mul.AddSubtree(div1);
        mul.AddSubtree(div2);
        start.AddSubtree(mul);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("'x1' / 'x2' * 'x3' / 'x4'", formatter.Format(t)); // same as x1 / x2 * (x3 / x4)
      }
      {
        //      div 
        //     /   \
        //    div   *
        //   / \   / \ 
        //  v1 v2 v3 v4
        //         
        var root = new ProgramRootSymbol().CreateTreeNode();
        var start = new StartSymbol().CreateTreeNode();
        var mul = new Multiplication().CreateTreeNode();
        var div2 = new Division().CreateTreeNode();
        var div1 = new Division().CreateTreeNode();
        var var1 = (VariableTreeNode)new Variable().CreateTreeNode(); var1.VariableName = "x1"; var1.Weight = 1.0;
        var var2 = (VariableTreeNode)new Variable().CreateTreeNode(); var2.VariableName = "x2"; var2.Weight = 1.0;
        var var3 = (VariableTreeNode)new Variable().CreateTreeNode(); var3.VariableName = "x3"; var3.Weight = 1.0;
        var var4 = (VariableTreeNode)new Variable().CreateTreeNode(); var4.VariableName = "x4"; var4.Weight = 1.0;
        div2.AddSubtree(var1);
        div2.AddSubtree(var2);
        mul.AddSubtree(var3);
        mul.AddSubtree(var4);
        div1.AddSubtree(div2);
        div1.AddSubtree(mul);
        start.AddSubtree(div1);
        root.AddSubtree(start);
        var t = new SymbolicExpressionTree(root);

        Assert.AreEqual("'x1' / 'x2' / ('x3' * 'x4')", formatter.Format(t));
      }

      {
        // check random trees after formatting & parsing again

        var rand = new Random.MersenneTwister(1234);
        var interpreter = new SymbolicDataAnalysisExpressionTreeInterpreter(); // use an interpreter that supports boolean functions
        var xy = new double[100, 4];
        for (int i = 0; i < xy.GetLength(0); i++)
          for (int j = 0; j < xy.GetLength(1); j++)
            xy[i, j] = rand.NextDouble();

        var ds = new Dataset(new string[] { "a", "b", "c", "y" }, xy);
        var rows = Enumerable.Range(0, xy.GetLength(1)).ToArray();
        var grammar = new TypeCoherentExpressionGrammar();
        grammar.ConfigureAsDefaultRegressionGrammar();
        grammar.Symbols.OfType<Logarithm>().First().Enabled = true; // enable a function
        grammar.Symbols.OfType<Exponential>().First().Enabled = true; // enable a function
        grammar.Symbols.OfType<AnalyticQuotient>().First().Enabled = true; // enable a function with two arguments
        grammar.Symbols.OfType<Power>().First().Enabled = true; // another function with two arguments
        grammar.Symbols.OfType<And>().First().Enabled = true; // enable Boolean operators
        grammar.Symbols.OfType<Or>().First().Enabled = true; 
        grammar.Symbols.OfType<Xor>().First().Enabled = true;
        grammar.Symbols.OfType<Not>().First().Enabled = true;
        grammar.Symbols.OfType<IfThenElse>().First().Enabled = true; // enable if-then-else function
        // test multi-argument versions of operators
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Division>().First(), 1, 4);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Subtraction>().First(), 1, 2);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Addition>().First(), 1, 4);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Multiplication>().First(), 1, 4);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<And>().First(), 1, 4);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Or>().First(), 1, 4);
        grammar.SetSubtreeCount(grammar.Symbols.OfType<Xor>().First(), 1, 4);
        grammar.ConfigureVariableSymbols(new RegressionProblemData(ds, new string[] { "a", "b", "c" }, "y"));
        var fmt = new SymbolicExpressionTreeStringFormatter();
        for (int i = 0; i < 100000; i++) {
          var t = ProbabilisticTreeCreator.Create(rand, grammar, 15, 8);
          var p1 = interpreter.GetSymbolicExpressionTreeValues(t, ds, rows).ToArray();
          var p2 = interpreter.GetSymbolicExpressionTreeValues(parser.Parse(formatter.Format(t)), ds, rows).ToArray(); // test formatter, and that parser can read the expression again, and that the evaluation is the same 
          for (int j = 0; j < p1.Length; j++) {
            if (double.IsNaN(p1[j]) && double.IsNaN(p2[j])) continue;
            Assert.AreEqual(p1[j], p2[j], Math.Abs(p1[j] * 1e-6), $"Problem in formatted expression:\n{formatter.Format(t)}\n{fmt.Format(t)}");
          }
        }
      }
    }
  }
}
