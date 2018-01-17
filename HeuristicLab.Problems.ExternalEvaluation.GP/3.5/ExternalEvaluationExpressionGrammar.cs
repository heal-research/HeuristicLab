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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  [Item("ExternalEvaluationExpressionGrammar", "Represents a grammar for functional expressions using all available functions.")]
  public class ExternalEvaluationExpressionGrammar : SymbolicExpressionGrammar, ISymbolicDataAnalysisGrammar {
    [Storable]
    private HeuristicLab.Problems.DataAnalysis.Symbolic.Variable variableSymbol;
    [StorableConstructor]
    protected ExternalEvaluationExpressionGrammar(bool deserializing) : base(deserializing) { }
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
      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Variable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, mean, sin, cos, tan, log, exp, @if, gt, lt, and, or, not, constant, variableSymbol };
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
