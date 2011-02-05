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

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  [Item("FullFunctionalExpressionGrammar", "Represents a grammar for functional expressions using all available functions.")]
  public class FullFunctionalExpressionGrammar : DefaultSymbolicExpressionGrammar {
    [Storable]
    private HeuristicLab.Problems.ExternalEvaluation.GP.Variable variableSymbol;
    [StorableConstructor]
    protected FullFunctionalExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected FullFunctionalExpressionGrammar(FullFunctionalExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new FullFunctionalExpressionGrammar(this, cloner);
    }

    public FullFunctionalExpressionGrammar()
      : base() {
      Initialize();
    }

    private void Initialize() {
      var originalStart = StartSymbol;
      if (!(originalStart is ProgramRootSymbol)) {
        var root = new ProgramRootSymbol();
        AddSymbol(root);
        SetMinSubtreeCount(root, 1);
        SetMaxSubtreeCount(root, 1);
        SetAllowedChild(root, originalStart, 0);

        StartSymbol = root;
      }

      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var mean = new Average();
      var sin = new Sine();
      var cos = new Cosine();
      var tan = new Tangent();
      var log = new Logarithm();
      var exp = new Exponential();
      var @if = new IfThenElse();
      var gt = new GreaterThan();
      var lt = new LessThan();
      var and = new And();
      var or = new Or();
      var not = new Not();
      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      variableSymbol = new Variable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, mean, sin, cos, tan, log, exp, @if, gt, lt, and, or, not, constant, variableSymbol };
      var unaryFunctionSymbols = new List<Symbol>() { sin, cos, tan, log, exp, not };
      var binaryFunctionSymbols = new List<Symbol>() { gt, lt };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div, mean, and, or };

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      foreach (var funSymb in functionSymbols) {
        SetMinSubtreeCount(funSymb, 1);
        SetMaxSubtreeCount(funSymb, 3);
      }
      foreach (var funSymb in unaryFunctionSymbols) {
        SetMinSubtreeCount(funSymb, 1);
        SetMaxSubtreeCount(funSymb, 1);
      }
      foreach (var funSymb in binaryFunctionSymbols) {
        SetMinSubtreeCount(funSymb, 2);
        SetMaxSubtreeCount(funSymb, 2);
      }

      SetMinSubtreeCount(@if, 3);
      SetMaxSubtreeCount(@if, 3);
      SetMinSubtreeCount(constant, 0);
      SetMaxSubtreeCount(constant, 0);
      SetMinSubtreeCount(variableSymbol, 0);
      SetMaxSubtreeCount(variableSymbol, 0);

      // allow each symbol as child of the start symbol
      foreach (var symb in allSymbols) {
        SetAllowedChild(originalStart, symb, 0);
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
