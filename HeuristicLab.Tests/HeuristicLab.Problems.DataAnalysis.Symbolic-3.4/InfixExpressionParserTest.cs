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
      Assert.AreEqual("3 * 4",formatter.Format(parser.Parse("3 * 4")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123E-03")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123e-03")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123e+03")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123E+03")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123.0E-03")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123.0e-03")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123.0e+03")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123.0E+03")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123.0E-3")));
      Assert.AreEqual("0.123",formatter.Format(parser.Parse("123.0e-3")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123.0e+3")));
      Assert.AreEqual("123000",formatter.Format(parser.Parse("123.0E+3")));

      Assert.AreEqual("3.1415 + 2",formatter.Format(parser.Parse("3.1415+2.0")));
      Assert.AreEqual("3.1415 / 2",formatter.Format(parser.Parse("3.1415/2.0")));
      Assert.AreEqual("3.1415 * 2",formatter.Format(parser.Parse("3.1415*2.0")));
      Assert.AreEqual("3.1415 - 2", formatter.Format(parser.Parse("3.1415-2.0")));
      // round-trip
      Assert.AreEqual("3.1415 - 2",formatter.Format(parser.Parse(formatter.Format(parser.Parse("3.1415-2.0")))));
      Assert.AreEqual("3.1415 + 2",formatter.Format(parser.Parse("3.1415+(2.0)")));
      Assert.AreEqual("3.1415 + 2", formatter.Format(parser.Parse("(3.1415+(2.0))")));


      Assert.AreEqual("LOG(3)",formatter.Format(parser.Parse("log(3)")));
      Assert.AreEqual("LOG(-3)",formatter.Format(parser.Parse("log(-3)")));
      Assert.AreEqual("EXP(3)",formatter.Format(parser.Parse("exp(3)")));
      Assert.AreEqual("EXP(-3)",formatter.Format(parser.Parse("exp(-3)")));
      Assert.AreEqual("SQRT(3)", formatter.Format(parser.Parse("sqrt(3)")));

      Assert.AreEqual("SQR(-3)", formatter.Format(parser.Parse("sqr((-3))")));

      Assert.AreEqual("3 / 3 + 2 / 2 + 1 / 1",formatter.Format(parser.Parse("3/3+2/2+1/1")));
      Assert.AreEqual("-3 + 30 - 2 + 20 - 1 + 10", formatter.Format(parser.Parse("-3+30-2+20-1+10")));

      // 'flattening' of nested addition, subtraction, multiplication, or division
      Assert.AreEqual("1 + 2 + 3 + 4", formatter.Format(parser.Parse("1 + 2 + 3 + 4")));
      Assert.AreEqual("1 - 2 - 3 - 4", formatter.Format(parser.Parse("1 - 2 - 3 - 4")));
      Assert.AreEqual("1 * 2 * 3 * 4", formatter.Format(parser.Parse("1 * 2 * 3 * 4")));
      Assert.AreEqual("1 / 2 / 3 / 4", formatter.Format(parser.Parse("1 / 2 / 3 / 4")));

      // signed variables / constants
      Assert.AreEqual("-1*'x1'", formatter.Format(parser.Parse("-x1")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("--1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("----1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("-+-1.0")));
      Assert.AreEqual("1", formatter.Format(parser.Parse("+-+-1.0")));
      Assert.AreEqual("-3 + -1", formatter.Format(parser.Parse("-3 + -1.0")));


      Assert.AreEqual("'x1'",formatter.Format(parser.Parse("\"x1\"")));
      Assert.AreEqual("'var name'",formatter.Format(parser.Parse("\'var name\'")));
      Assert.AreEqual("'var name'",formatter.Format(parser.Parse("\"var name\"")));
      Assert.AreEqual("'1'",formatter.Format(parser.Parse("\"1\"")));

      Assert.AreEqual("'var \" name'",formatter.Format(parser.Parse("'var \" name\'")));
      Assert.AreEqual("\"var ' name\"", formatter.Format(parser.Parse("\"var \' name\"")));


      Assert.AreEqual("'x1' * 'x2'",formatter.Format(parser.Parse("\"x1\"*\"x2\"")));
      Assert.AreEqual("'x1' * 'x2' + 'x3' * 'x4'",formatter.Format(parser.Parse("\"x1\"*\"x2\"+\"x3\"*\"x4\"")));
      Assert.AreEqual("'x1' * 'x2' + 'x3' * 'x4'", formatter.Format(parser.Parse("x1*x2+x3*x4")));


      Assert.AreEqual("3 ^ 2",formatter.Format(parser.Parse("POW(3, 2)")));
      Assert.AreEqual("3.1 ^ 2.1",formatter.Format(parser.Parse("POW(3.1, 2.1)")));
      Assert.AreEqual("3.1 ^ 2.1",formatter.Format(parser.Parse("POW(3.1 , 2.1)")));
      Assert.AreEqual("3.1 ^ 2.1",formatter.Format(parser.Parse("POW(3.1 ,2.1)")));
      Assert.AreEqual("-3.1 ^ -2.1",formatter.Format(parser.Parse("POW(-3.1 , - 2.1)")));
      Assert.AreEqual("ROOT(3, 2)",formatter.Format(parser.Parse("ROOT(3, 2)")));
      Assert.AreEqual("ROOT(3.1, 2.1)",formatter.Format(parser.Parse("ROOT(3.1, 2.1)")));
      Assert.AreEqual("ROOT(3.1, 2.1)",formatter.Format(parser.Parse("ROOT(3.1 , 2.1)")));
      Assert.AreEqual("ROOT(3.1, 2.1)",formatter.Format(parser.Parse("ROOT(3.1 ,2.1)")));
      Assert.AreEqual("ROOT(-3.1, -2.1)", formatter.Format(parser.Parse("ROOT(-3.1 , -2.1)")));

      Assert.AreEqual("IF(GT(0, 1), 1, 0)",formatter.Format(parser.Parse("IF(GT( 0, 1), 1, 0)")));
      Assert.AreEqual("IF(LT(0, 1), 1, 0)",formatter.Format(parser.Parse("IF(LT(0,1), 1 , 0)")));
      Assert.AreEqual("LAG('x', 1)",formatter.Format(parser.Parse("LAG(x, 1)")));
      Assert.AreEqual("LAG('x', -1)",formatter.Format(parser.Parse("LAG(x, -1)")));
      Assert.AreEqual("LAG('x', 1)",formatter.Format(parser.Parse("LAG(x, +1)")));
      Assert.AreEqual("'x' * LAG('x', 1)", formatter.Format(parser.Parse("x * LAG('x', +1)")));

      // factor variables
      Assert.AreEqual("'x'[1] * 'y'",formatter.Format(parser.Parse("x [1.0] * y")));
      Assert.AreEqual("'x'[1, 2] * 'y'[1, 2]",formatter.Format(parser.Parse("x [1.0, 2.0] * y [1.0, 2.0]")));
      Assert.AreEqual("'x'[1] * 'y'",formatter.Format(parser.Parse("x[1] * y")));
      Assert.AreEqual("'x'[1, 2] * 'y'[1, 2]",formatter.Format(parser.Parse("x[1, 2] * y [1, 2]")));
      Assert.AreEqual("'x'[1] * 'y'",formatter.Format(parser.Parse("x [+1.0] * y")));
      Assert.AreEqual("'x'[-1] * 'y'",formatter.Format(parser.Parse("x [-1.0] * y")));
      Assert.AreEqual("'x'[-1, -2] * 'y'[1, 2]", formatter.Format(parser.Parse("x [-1.0, -2.0] * y [+1.0, +2.0]")));

      // one-hot for factor
      Assert.AreEqual("'x' = 'val' * 'y'",formatter.Format(parser.Parse("x='val' * y")));
      Assert.AreEqual("'x' = 'val'",formatter.Format(parser.Parse("x = 'val'")));
      Assert.AreEqual("'x' = 'val'",formatter.Format(parser.Parse("x = \"val\"")));
      Assert.AreEqual("1 * 'x' = 'val'",formatter.Format(parser.Parse("1.0 * x = val")));
      Assert.AreEqual("-1 * 'x' = 'val'",formatter.Format(parser.Parse("-1.0 * x = val")));
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
    }
  }
}
