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
  [Item("FullFunctionalExpressionGrammar", "Represents a grammar for functional expressions using all available functions.")]
  public class FullFunctionalExpressionGrammar : DefaultSymbolicExpressionGrammar {
    [StorableConstructor]
    protected FullFunctionalExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected FullFunctionalExpressionGrammar(FullFunctionalExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public FullFunctionalExpressionGrammar()
      : base() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FullFunctionalExpressionGrammar(this, cloner);
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
      var pow = new Power();
      pow.InitialFrequency = 0.0;
      var root = new Root();
      root.InitialFrequency = 0.0;
      var exp = new Exponential();
      var @if = new IfThenElse();
      var gt = new GreaterThan();
      var lt = new LessThan();
      var and = new And();
      var or = new Or();
      var not = new Not();

      var timeLag = new TimeLag();
      timeLag.InitialFrequency = 0.0;
      var integral = new Integral();
      integral.InitialFrequency = 0.0;
      var derivative = new Derivative();
      derivative.InitialFrequency = 0.0;

      var variableCondition = new VariableCondition();
      variableCondition.InitialFrequency = 0.0;

      var constant = new Constant();
      constant.MinValue = -20;
      constant.MaxValue = 20;
      var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable();
      var laggedVariable = new LaggedVariable();
      laggedVariable.InitialFrequency = 0.0;

      var allSymbols = new List<Symbol>() { add, sub, mul, div, mean, sin, cos, tan, log, pow, root, exp, @if, gt, lt, and, or, not, timeLag, integral, derivative, constant, variableSymbol, laggedVariable, variableCondition };
      var unaryFunctionSymbols = new List<Symbol>() { sin, cos, tan, log, exp, not, timeLag, integral, derivative };

      var binaryFunctionSymbols = new List<Symbol>() { pow, root, gt, lt, variableCondition };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div, mean, and, or };
      var terminalSymbols = new List<Symbol>() { variableSymbol, constant, laggedVariable };

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

      foreach (var terminalSymbol in terminalSymbols) {
        SetMinSubtreeCount(terminalSymbol, 0);
        SetMaxSubtreeCount(terminalSymbol, 0);
      }

      SetMinSubtreeCount(@if, 3);
      SetMaxSubtreeCount(@if, 3);


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
