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
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_3._3.Tests {
  public static class Grammars {
    private class Addition : Symbol {
      protected Addition(Addition original, Cloner cloner) : base(original, cloner) { }
      public Addition() : base("Addition", "") { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new Addition(this, cloner);
      }
    }
    private class Subtraction : Symbol {
      protected Subtraction(Subtraction original, Cloner cloner) : base(original, cloner) { }
      public Subtraction() : base("Subtraction", "") { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new Subtraction(this, cloner);
      }
    }
    private class Multiplication : Symbol {
      protected Multiplication(Multiplication original, Cloner cloner) : base(original, cloner) { }
      public Multiplication() : base("Multiplication", "") { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new Multiplication(this, cloner);
      }
    }
    private class Division : Symbol {
      protected Division(Division original, Cloner cloner) : base(original, cloner) { }
      public Division() : base("Division", "") { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new Division(this, cloner);
      }
    }
    private class Terminal : Symbol {
      protected Terminal(Terminal original, Cloner cloner) : base(original, cloner) { }
      public Terminal() : base("Terminal", "") { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new Terminal(this, cloner);
      }
    }

    private class SimpleArithmeticGrammar : DefaultSymbolicExpressionGrammar {
      protected SimpleArithmeticGrammar(SimpleArithmeticGrammar original, Cloner cloner) : base(original, cloner) { }
      public SimpleArithmeticGrammar()
        : base() {
        Initialize();
      }

      public override IDeepCloneable Clone(Cloner cloner) {
        return new SimpleArithmeticGrammar(this, cloner);
      }

      private void Initialize() {
        var add = new Addition();
        var sub = new Subtraction();
        var mul = new Multiplication();
        var div = new Division();
        div.InitialFrequency = 0.0; // disable division symbol
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
