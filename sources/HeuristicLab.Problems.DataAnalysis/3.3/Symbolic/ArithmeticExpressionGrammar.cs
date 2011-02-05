#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("ArithmeticExpressionGrammar", "Represents a grammar for functional expressions using only arithmetic operations.")]
  public class ArithmeticExpressionGrammar : DefaultSymbolicExpressionGrammar {

    [StorableConstructor]
    protected ArithmeticExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected ArithmeticExpressionGrammar(ArithmeticExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public ArithmeticExpressionGrammar()
      : base() {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArithmeticExpressionGrammar(this, cloner);
    }

    private void Initialize() {
      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, constant, variableSymbol };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div };

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      foreach (var funSymb in functionSymbols) {
        SetMinSubtreeCount(funSymb, 1);
        SetMaxSubtreeCount(funSymb, 3);
      }
      SetMinSubtreeCount(constant, 0);
      SetMaxSubtreeCount(constant, 0);
      SetMinSubtreeCount(variableSymbol, 0);
      SetMaxSubtreeCount(variableSymbol, 0);

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
}
