#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  public static class Grammars {
    private class Addition : Symbol { }
    private class Subtraction : Symbol { }
    private class Multiplication : Symbol { }
    private class Division : Symbol { }
    private class Terminal : Symbol { }

    private class SimpleArithmeticGrammar : DefaultSymbolicExpressionGrammar {
      public SimpleArithmeticGrammar()
        : base() {
        Initialize();
      }

      private void Initialize() {
        var add = new Addition();
        var sub = new Subtraction();
        var mul = new Multiplication();
        var div = new Division();
        var terminal = new Terminal();

        var allSymbols = new List<Symbol>() { add, sub, mul, div, terminal };
        var functionSymbols = new List<Symbol>() { add, sub, mul, div };
        foreach (var symb in allSymbols)
          AddSymbol(symb);

        foreach (var funSymb in functionSymbols) {
          SetMinSubtreeCount(funSymb, 1);
          SetMaxSubtreeCount(funSymb, 3);
        }
        SetMinSubtreeCount(terminal, 0);
        SetMaxSubtreeCount(terminal, 0);

        // allow each symbol as child of the start symbol
        foreach (var symb in allSymbols) {
          SetAllowedChild(StartSymbol, symb, 0);
        }

        // allow each symbol as child of every other symbol (except for terminals that have maxSubtreeCount == 0)
        foreach (var parent in allSymbols) {
          for (int i = 0; i < GetMaxSubtreeCount(parent); i++)
            foreach (var child in allSymbols) {
              SetAllowedChild(parent, child, i);
            }
        }
      }
    }

    public static ISymbolicExpressionGrammar CreateSimpleArithmeticGrammar() {
      var g = new GlobalSymbolicExpressionGrammar(new SimpleArithmeticGrammar());
      g.MaxFunctionArguments = 0;
      g.MinFunctionArguments = 0;
      g.MaxFunctionDefinitions = 0;
      g.MinFunctionDefinitions = 0;
      return g;
    }

    public static ISymbolicExpressionGrammar CreateArithmeticAndAdfGrammar() {
      var g = new GlobalSymbolicExpressionGrammar(new SimpleArithmeticGrammar());
      g.MaxFunctionArguments = 3;
      g.MinFunctionArguments = 0;
      g.MaxFunctionDefinitions = 3;
      g.MinFunctionDefinitions = 0;
      return g;
    }

    public static void HasValidAdfGrammars(SymbolicExpressionTree tree) {
      //Assert.AreEqual(tree.Root.Grammar.Symbols.Count(), 8);
      //Assert.AreEqual(tree.Root.GetAllowedSymbols(0).Count(), 1); // only the start symbol is allowed
      //// we allow 3 ADF branches
      //Assert.AreEqual(tree.Root.GetAllowedSymbols(1).Count(), 1); // only the defun branch is allowed
      //Assert.AreEqual(tree.Root.GetAllowedSymbols(2).Count(), 1); // only the defun symbol is allowed
      //Assert.AreEqual(tree.Root.GetAllowedSymbols(3).Count(), 1); // only the defun symbol is allowed
      //foreach (var subtree in tree.Root.SubTrees) {
      //  // check consistency of each sub-tree grammar independently
      //  var allowedSymbols = subtree.GetAllowedSymbols(0);
      //  int numberOfAllowedSymbols = allowedSymbols.Count();
      //  foreach (var parent in allowedSymbols) {
      //    for (int argIndex = 0; argIndex < subtree.Grammar.GetMaxSubtreeCount(parent); argIndex++) {
      //      var allowedChildren = from child in subtree.Grammar.Symbols
      //                            where subtree.Grammar.IsAllowedChild(parent, child, argIndex)
      //                            select child;
      //      Assert.AreEqual(numberOfAllowedSymbols, allowedChildren.Count());
      //    }
      //  }
      //}
    }

  }
}
