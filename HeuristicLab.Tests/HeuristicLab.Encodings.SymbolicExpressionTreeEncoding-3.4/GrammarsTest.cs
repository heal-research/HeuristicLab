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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Tests {
  [TestClass]
  public class GrammarsTest {
    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "short")]
    public void MinimumExpressionLengthTest() {
      {
        var grammar = CreateTestGrammar1();

        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");

        Assert.AreEqual(8, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(7, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(6, grammar.GetMinimumExpressionLength(x));
      }

      {
        var grammar = CreateTestGrammar2();

        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");

        Assert.AreEqual(13, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(12, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(11, grammar.GetMinimumExpressionLength(x));
      }

      {
        var grammar = CreateTestGrammar3();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(x));
      }

      {
        var grammar = CreateTestGrammar4();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var y = grammar.Symbols.First(s => s.Name == "<y>");
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(x));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionLength(y));
      }

      {
        var grammar = CreateTestGrammar5();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var y = grammar.Symbols.First(s => s.Name == "<y>");
        Assert.AreEqual(5, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(4, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(3, grammar.GetMinimumExpressionLength(x));
        Assert.AreEqual(2, grammar.GetMinimumExpressionLength(y));
      }

      {
        var grammar = CreateTestGrammar6();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var s_ = grammar.Symbols.First(s => s.Name == "<s>");
        var a = grammar.Symbols.First(s => s.Name == "<a>");
        var b = grammar.Symbols.First(s => s.Name == "<b>");
        var c = grammar.Symbols.First(s => s.Name == "<c>");
        var d = grammar.Symbols.First(s => s.Name == "<d>");
        Assert.AreEqual(4, grammar.GetMinimumExpressionLength(prs));
        Assert.AreEqual(3, grammar.GetMinimumExpressionLength(ss));
        Assert.AreEqual(2, grammar.GetMinimumExpressionLength(x));
        Assert.AreEqual(5, grammar.GetMinimumExpressionLength(s_));
        Assert.AreEqual(4, grammar.GetMinimumExpressionLength(a));
        Assert.AreEqual(3, grammar.GetMinimumExpressionLength(b));
        Assert.AreEqual(4, grammar.GetMinimumExpressionLength(c));
        Assert.AreEqual(3, grammar.GetMinimumExpressionLength(d));
      }
    }

    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "short")]
    public void MinimumExpressionDepthTest() {
      {
        var grammar = CreateTestGrammar1();

        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var a = grammar.Symbols.First(s => s.Name == "<a>");
        var b = grammar.Symbols.First(s => s.Name == "<b>");
        var c = grammar.Symbols.First(s => s.Name == "<c>");
        var d = grammar.Symbols.First(s => s.Name == "<d>");
        var x = grammar.Symbols.First(s => s.Name == "x");
        var y = grammar.Symbols.First(s => s.Name == "y");

        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(a));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(b));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(c));
        Assert.AreEqual(2, grammar.GetMinimumExpressionDepth(d));
        Assert.AreEqual(1, grammar.GetMinimumExpressionDepth(x));
        Assert.AreEqual(1, grammar.GetMinimumExpressionDepth(y));
      }

      {
        var grammar = CreateTestGrammar2();

        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var a = grammar.Symbols.First(s => s.Name == "<a>");
        var b = grammar.Symbols.First(s => s.Name == "<b>");
        var c = grammar.Symbols.First(s => s.Name == "<c>");
        var d = grammar.Symbols.First(s => s.Name == "<d>");
        var x = grammar.Symbols.First(s => s.Name == "x");
        var y = grammar.Symbols.First(s => s.Name == "y");

        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(a));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(b));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(c));
        Assert.AreEqual(2, grammar.GetMinimumExpressionDepth(d));
        Assert.AreEqual(1, grammar.GetMinimumExpressionDepth(x));
        Assert.AreEqual(1, grammar.GetMinimumExpressionDepth(y));
      }

      {
        var grammar = CreateTestGrammar3();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(x));
      }

      {
        var grammar = CreateTestGrammar4();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var y = grammar.Symbols.First(s => s.Name == "<y>");
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(x));
        Assert.AreEqual(int.MaxValue, grammar.GetMinimumExpressionDepth(y));
      }

      {
        var grammar = CreateTestGrammar5();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var y = grammar.Symbols.First(s => s.Name == "<y>");
        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(x));
        Assert.AreEqual(2, grammar.GetMinimumExpressionDepth(y));
      }

      {
        var grammar = CreateTestGrammar6();
        var prs = grammar.ProgramRootSymbol;
        var ss = grammar.StartSymbol;
        var x = grammar.Symbols.First(s => s.Name == "<x>");
        var s_ = grammar.Symbols.First(s => s.Name == "<s>");
        var a = grammar.Symbols.First(s => s.Name == "<a>");
        var b = grammar.Symbols.First(s => s.Name == "<b>");
        var c = grammar.Symbols.First(s => s.Name == "<c>");
        var d = grammar.Symbols.First(s => s.Name == "<d>");
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(prs));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(ss));
        Assert.AreEqual(2, grammar.GetMinimumExpressionDepth(x));
        Assert.AreEqual(5, grammar.GetMinimumExpressionDepth(s_));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(a));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(b));
        Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(c));
        Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(d));
      }
    }

    private static ISymbolicExpressionGrammar CreateTestGrammar1() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 1);
      var z = new SimpleSymbol("<z>", 6);
      var a = new SimpleSymbol("<a>", 1);
      var b = new SimpleSymbol("<b>", 1);
      var c = new SimpleSymbol("<c>", 1);
      var d = new SimpleSymbol("<d>", 1);

      var _x = new SimpleSymbol("x", 0);
      var _y = new SimpleSymbol("y", 0);

      grammar.AddSymbol(x);
      grammar.AddSymbol(z);
      grammar.AddSymbol(a);
      grammar.AddSymbol(b);
      grammar.AddSymbol(c);
      grammar.AddSymbol(d);
      grammar.AddSymbol(_x);
      grammar.AddSymbol(_y);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      //uncommenting the line below changes the minimum expression length for the symbol <x>
      grammar.AddAllowedChildSymbol(x, z);
      grammar.AddAllowedChildSymbol(z, _x);
      grammar.AddAllowedChildSymbol(x, a);
      grammar.AddAllowedChildSymbol(a, b);
      grammar.AddAllowedChildSymbol(b, c);
      grammar.AddAllowedChildSymbol(c, d);
      grammar.AddAllowedChildSymbol(d, _y);

      return grammar;
    }

    private static ISymbolicExpressionGrammar CreateTestGrammar2() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 2);
      var z = new SimpleSymbol("<z>", 6);
      var a = new SimpleSymbol("<a>", 1);
      var b = new SimpleSymbol("<b>", 1);
      var c = new SimpleSymbol("<c>", 1);
      var d = new SimpleSymbol("<d>", 1);

      var _x = new SimpleSymbol("x", 0);
      var _y = new SimpleSymbol("y", 0);

      grammar.AddSymbol(x);
      grammar.AddSymbol(z);
      grammar.AddSymbol(a);
      grammar.AddSymbol(b);
      grammar.AddSymbol(c);
      grammar.AddSymbol(d);
      grammar.AddSymbol(_x);
      grammar.AddSymbol(_y);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      //uncommenting the line below changes the minimum expression length for the symbol <x>
      grammar.AddAllowedChildSymbol(x, z);
      grammar.AddAllowedChildSymbol(z, _x);
      grammar.AddAllowedChildSymbol(x, a);
      grammar.AddAllowedChildSymbol(a, b);
      grammar.AddAllowedChildSymbol(b, c);
      grammar.AddAllowedChildSymbol(c, d);
      grammar.AddAllowedChildSymbol(d, _y);

      return grammar;
    }

    private static ISymbolicExpressionGrammar CreateTestGrammar3() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 1);

      grammar.AddSymbol(x);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      grammar.AddAllowedChildSymbol(x, x);
      return grammar;
    }


    private static ISymbolicExpressionGrammar CreateTestGrammar4() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 1);
      var y = new SimpleSymbol("<y>", 1);

      grammar.AddSymbol(x);
      grammar.AddSymbol(y);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      grammar.AddAllowedChildSymbol(x, x);
      grammar.AddAllowedChildSymbol(x, y);
      grammar.AddAllowedChildSymbol(y, x);
      grammar.AddAllowedChildSymbol(y, y);
      return grammar;
    }

    private static ISymbolicExpressionGrammar CreateTestGrammar5() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 1);
      var y = new SimpleSymbol("<y>", 1);
      var _x = new SimpleSymbol("x", 0);

      grammar.AddSymbol(x);
      grammar.AddSymbol(y);
      grammar.AddSymbol(_x);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      grammar.AddAllowedChildSymbol(x, x);
      grammar.AddAllowedChildSymbol(x, y);
      grammar.AddAllowedChildSymbol(y, x);
      grammar.AddAllowedChildSymbol(y, y);
      grammar.AddAllowedChildSymbol(y, _x);
      return grammar;
    }

    private static ISymbolicExpressionGrammar CreateTestGrammar6() {
      var grammar = new SimpleSymbolicExpressionGrammar();
      var x = new SimpleSymbol("<x>", 1);
      var s = new SimpleSymbol("<s>", 1);
      var a = new SimpleSymbol("<a>", 1);
      var b = new SimpleSymbol("<b>", 1);
      var c = new SimpleSymbol("<c>", 1);
      var d = new SimpleSymbol("<d>", 1);
      var e = new SimpleSymbol("<e>", 1);

      var _x = new SimpleSymbol("x", 0);
      var _y = new SimpleSymbol("y", 0);

      grammar.AddSymbol(x);
      grammar.AddSymbol(s);
      grammar.AddSymbol(a);
      grammar.AddSymbol(b);
      grammar.AddSymbol(c);
      grammar.AddSymbol(d);
      grammar.AddSymbol(e);
      grammar.AddSymbol(_x);
      grammar.AddSymbol(_y);

      grammar.AddAllowedChildSymbol(grammar.StartSymbol, x);
      grammar.AddAllowedChildSymbol(x, s);
      grammar.AddAllowedChildSymbol(x, _x);
      grammar.AddAllowedChildSymbol(s, a);
      grammar.AddAllowedChildSymbol(a, b);
      grammar.AddAllowedChildSymbol(a, c);
      grammar.AddAllowedChildSymbol(b, x);
      grammar.AddAllowedChildSymbol(c, d);
      grammar.AddAllowedChildSymbol(d, e);
      grammar.AddAllowedChildSymbol(e, _y);

      return grammar;
    }
  }
}
