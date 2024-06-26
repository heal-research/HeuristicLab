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

using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableType("747A7784-EF15-4CEF-A621-79A9071A69F5")]
  [Item("ExternalEvaluationExpressionGrammar", "Represents a grammar for functional expressions using all available functions.")]
  public class ExternalEvaluationExpressionGrammar : DataAnalysisGrammar {
    [Storable]
    private HeuristicLab.Problems.DataAnalysis.Symbolic.Variable variableSymbol;
    [StorableConstructor]
    protected ExternalEvaluationExpressionGrammar(StorableConstructorFlag _) : base(_) { }
    protected ExternalEvaluationExpressionGrammar(ExternalEvaluationExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExternalEvaluationExpressionGrammar(this, cloner);
    }

    public ExternalEvaluationExpressionGrammar()
      : base("ExternalEvaluationExpressionGrammar", "Represents a grammar for functional expressions using all available functions.") {
      Initialize();
    }

    private void Initialize() {
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
      var number = new Number();
      number.MinValue = -20;
      number.MaxValue = 20;
      var constant = new Constant() { Enabled = false };


      variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Variable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, mean, sin, cos, tan, log, exp, @if, gt, lt, and, or, not, number, constant, variableSymbol };
      var unaryFunctionSymbols = new List<Symbol>() { sin, cos, tan, log, exp, not };
      var binaryFunctionSymbols = new List<Symbol>() { gt, lt };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div, mean, and, or };

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      foreach (var funSymb in functionSymbols) {
        SetSubtreeCount(funSymb, 1, 3);
      }
      foreach (var funSymb in unaryFunctionSymbols) {
        SetSubtreeCount(funSymb, 1, 1);
      }
      foreach (var funSymb in binaryFunctionSymbols) {
        SetSubtreeCount(funSymb, 2, 2);
      }

      SetSubtreeCount(@if, 3, 3);
      SetSubtreeCount(number, 0, 0);
      SetSubtreeCount(constant, 0, 0);
      SetSubtreeCount(variableSymbol, 0, 0);

      // allow each symbol as child of the start symbol
      foreach (var symb in allSymbols) {
        AddAllowedChildSymbol(StartSymbol, symb, 0);
      }

      // allow each symbol as child of every other symbol (except for terminals that have maxSubtreeCount == 0)
      foreach (var parent in allSymbols) {
        for (int i = 0; i < GetMaximumSubtreeCount(parent); i++)
          foreach (var child in allSymbols) {
            AddAllowedChildSymbol(parent, child, i);
          }
      }
    }
  }
}
