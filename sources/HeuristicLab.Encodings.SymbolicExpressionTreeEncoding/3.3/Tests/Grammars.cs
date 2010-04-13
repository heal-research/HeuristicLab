using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

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
  }
}
