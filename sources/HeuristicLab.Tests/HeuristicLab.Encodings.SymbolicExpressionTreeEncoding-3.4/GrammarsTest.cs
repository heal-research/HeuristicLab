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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Tests {
  [TestClass]
  public class GrammarsTest {
    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "short")]
    public void MinimumExpressionLengthTest() {
      var grammar = CreateAbstractGrammar();

      var prs = grammar.ProgramRootSymbol;
      var a = grammar.Symbols.First(s => s.Name == "<a>");
      var b = grammar.Symbols.First(s => s.Name == "<b>");

      Assert.AreEqual(4, grammar.GetMinimumExpressionLength(prs));
      Assert.AreEqual(4, grammar.GetMinimumExpressionLength(a));
      Assert.AreEqual(3, grammar.GetMinimumExpressionLength(b));
    }

    [TestMethod]
    [TestCategory("Encodings.SymbolicExpressionTree")]
    [TestProperty("Time", "short")]
    public void MinimumExpressionDepthTest() {
      var grammar = CreateAbstractGrammar();

      var prs = grammar.ProgramRootSymbol;
      var a = grammar.Symbols.First(s => s.Name == "<a>");
      var b = grammar.Symbols.First(s => s.Name == "<b>");

      Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(prs));
      Assert.AreEqual(4, grammar.GetMinimumExpressionDepth(a));
      Assert.AreEqual(3, grammar.GetMinimumExpressionDepth(b));
    }

    private static ISymbolicExpressionGrammar CreateAbstractGrammar() {
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
