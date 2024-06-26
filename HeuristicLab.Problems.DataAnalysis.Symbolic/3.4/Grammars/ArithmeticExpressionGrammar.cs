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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [NonDiscoverableType]
  [StorableType("FCBA02B7-5D29-42F5-A64C-A60AD8EA475D")]
  [Item("ArithmeticExpressionGrammar", "Represents a grammar for functional expressions using only arithmetic operations.")]
  public class ArithmeticExpressionGrammar : DataAnalysisGrammar, ISymbolicDataAnalysisGrammar {

    [StorableConstructor]
    protected ArithmeticExpressionGrammar(StorableConstructorFlag _) : base(_) { }
    protected ArithmeticExpressionGrammar(ArithmeticExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public ArithmeticExpressionGrammar()
      : base(ItemAttribute.GetName(typeof(ArithmeticExpressionGrammar)), ItemAttribute.GetDescription(typeof(ArithmeticExpressionGrammar))) {
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
      var number = new Number();
      number.MinValue = -20;
      number.MaxValue = 20;
      var constant = new Constant();
      constant.Enabled = false;
      var variableSymbol = new HeuristicLab.Problems.DataAnalysis.Symbolic.Variable();
      var binFactorVariableSymbol = new BinaryFactorVariable();
      var factorVariableSymbol = new FactorVariable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, number, constant, variableSymbol, binFactorVariableSymbol, factorVariableSymbol };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div };

      foreach (var symb in allSymbols)
        AddSymbol(symb);

      foreach (var funSymb in functionSymbols) {
        SetSubtreeCount(funSymb, 1, 3);
      }
      SetSubtreeCount(number, 0, 0);
      SetSubtreeCount(constant, 0, 0);
      SetSubtreeCount(variableSymbol, 0, 0);
      SetSubtreeCount(binFactorVariableSymbol, 0, 0);
      SetSubtreeCount(factorVariableSymbol, 0, 0);

      // allow each symbol as child of the start symbol
      foreach (var symb in allSymbols) {
        AddAllowedChildSymbol(StartSymbol, symb);
        AddAllowedChildSymbol(DefunSymbol, symb);
      }

      // allow each symbol as child of every other symbol (except for terminals that have maxSubtreeCount == 0)
      foreach (var parent in functionSymbols) {
        foreach (var child in allSymbols)
          AddAllowedChildSymbol(parent, child);
      }
    }
  }
}
