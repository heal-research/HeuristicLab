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
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic {
  [StorableClass]
  [Item("SymbolicVectorRegressionGrammar", "Represents a grammar for symbolic vector regression using all available functions.")]
  public class SymbolicVectorRegressionGrammar : MultiVariateExpressionGrammar {
    public SymbolicVectorRegressionGrammar() : this(1) { }

    public SymbolicVectorRegressionGrammar(int dimension)
      : base(dimension) {
      Initialize();
    }

    protected SymbolicVectorRegressionGrammar(SymbolicVectorRegressionGrammar original) : base(original) { }

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
      var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable();

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

      SetMinSubtreeCount(StartSymbol, Dimension);
      SetMaxSubtreeCount(StartSymbol, Dimension);

      SetMinSubtreeCount(constant, 0);
      SetMaxSubtreeCount(constant, 0);
      SetMinSubtreeCount(variableSymbol, 0);
      SetMaxSubtreeCount(variableSymbol, 0);

      // allow all symbols as children of the start-symbol
      foreach (Symbol symb in allSymbols) {
        for (int i = 0; i < GetMaxSubtreeCount(StartSymbol); i++)
          SetAllowedChild(StartSymbol, symb, i);
      }

      // allow all symbols as children of all symbols
      foreach (var parent in allSymbols) {
        for (int i = 0; i < GetMaxSubtreeCount(parent); i++)
          foreach (var child in allSymbols) {
            SetAllowedChild(parent, child, i);
          }
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      var clone = new SymbolicVectorRegressionGrammar(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }
  }
}
