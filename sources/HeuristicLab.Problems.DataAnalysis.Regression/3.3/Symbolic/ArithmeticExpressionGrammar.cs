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


using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System.Collections.Generic;
using System;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols;
using HeuristicLab.Data;
namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  [Item("ArithmeticExpressionGrammar", "Represents a grammar for functional expressions using only arithmetic operations.")]
  public class ArithmeticExpressionGrammar : DefaultSymbolicExpressionGrammar {
    [Storable]
    private List<string> variableNames = new List<string>();
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
      set {
        variableNames = new List<string>(value);
        variableSymbol.VariableNames = variableNames;
      }
    }

    [Storable]
    private HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable variableSymbol;

    public ArithmeticExpressionGrammar()
      : base(0, 0, 0, 0) {
      Initialize();
    }

    private void Initialize() {
      var add = new Addition();
      var sub = new Subtraction();
      var mul = new Multiplication();
      var div = new Division();
      var constant = new Constant();
      variableSymbol = new HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols.Variable();

      var allSymbols = new List<Symbol>() { add, sub, mul, div, constant, variableSymbol };
      var functionSymbols = new List<Symbol>() { add, sub, mul, div };
      allSymbols.ForEach(s => AddAllowedSymbols(StartSymbol, 0, s));


      SetMinSubTreeCount(constant, 0);
      SetMaxSubTreeCount(constant, 0);
      SetMinSubTreeCount(variableSymbol, 0);
      SetMaxSubTreeCount(variableSymbol, 0);
      int maxSubTrees = 3;
      foreach (var functionSymbol in functionSymbols) {
        SetMinSubTreeCount(functionSymbol, 1);
        SetMaxSubTreeCount(functionSymbol, maxSubTrees);
        foreach (var childSymbol in allSymbols) {
          for (int argumentIndex = 0; argumentIndex < maxSubTrees; argumentIndex++) {
            AddAllowedSymbols(functionSymbol, argumentIndex, childSymbol);
          }
        }
      }
    }
  }
}
