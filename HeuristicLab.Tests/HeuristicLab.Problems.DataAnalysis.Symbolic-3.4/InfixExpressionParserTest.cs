#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      Console.WriteLine(formatter.Format(parser.Parse("3")));
      Console.WriteLine(formatter.Format(parser.Parse("3*3")));
      Console.WriteLine(formatter.Format(parser.Parse("3 * 4")));
      Console.WriteLine(formatter.Format(parser.Parse("123E-03")  ));
      Console.WriteLine(formatter.Format(parser.Parse("123e-03")));
      Console.WriteLine(formatter.Format(parser.Parse("123e+03")));
      Console.WriteLine(formatter.Format(parser.Parse("123E+03")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0E-03")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0e-03")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0e+03")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0E+03")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0E-3")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0e-3")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0e+3")));
      Console.WriteLine(formatter.Format(parser.Parse("123.0E+3")));

      Console.WriteLine(formatter.Format(parser.Parse("3.1415+2.0")));
      Console.WriteLine(formatter.Format(parser.Parse("3.1415/2.0")));
      Console.WriteLine(formatter.Format(parser.Parse("3.1415*2.0")));
      Console.WriteLine(formatter.Format(parser.Parse("3.1415-2.0")));
      // round-trip
      Console.WriteLine(formatter.Format(parser.Parse(formatter.Format(parser.Parse("3.1415-2.0")))));
      Console.WriteLine(formatter.Format(parser.Parse("3.1415+(2.0)")));
      Console.WriteLine(formatter.Format(parser.Parse("(3.1415+(2.0))")));


      Console.WriteLine(formatter.Format(parser.Parse("log(3)")));
      Console.WriteLine(formatter.Format(parser.Parse("log(-3)")));
      Console.WriteLine(formatter.Format(parser.Parse("exp(3)")));
      Console.WriteLine(formatter.Format(parser.Parse("exp(-3)")));
      Console.WriteLine(formatter.Format(parser.Parse("sqrt(3)")));

      Console.WriteLine(formatter.Format(parser.Parse("sqr((-3))")));

      Console.WriteLine(formatter.Format(parser.Parse("3/3+2/2+1/1")));
      Console.WriteLine(formatter.Format(parser.Parse("-3+30-2+20-1+10")));
      // round trip
      Console.WriteLine(formatter.Format(parser.Parse(formatter.Format(parser.Parse("-3+30-2+20-1+10")))));

      Console.WriteLine(formatter.Format(parser.Parse("\"x1\"")));
      Console.WriteLine(formatter.Format(parser.Parse("\'var name\'")));
      Console.WriteLine(formatter.Format(parser.Parse("\"var name\"")));
      Console.WriteLine(formatter.Format(parser.Parse("\"1\"")));

      Console.WriteLine(formatter.Format(parser.Parse("'var \" name\'")));
      Console.WriteLine(formatter.Format(parser.Parse("\"var \' name\"")));


      Console.WriteLine(formatter.Format(parser.Parse("\"x1\"*\"x2\"")));
      Console.WriteLine(formatter.Format(parser.Parse("\"x1\"*\"x2\"+\"x3\"*\"x4\"")));
      Console.WriteLine(formatter.Format(parser.Parse("x1*x2+x3*x4")));

    }
  }
}
