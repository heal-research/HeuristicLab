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
namespace HeuristicLab.Problems.ArtificialAnt {
  [StorableClass]
  public class ArtificialAntExpressionGrammar : DefaultSymbolicExpressionGrammar {

    public ArtificialAntExpressionGrammar()
      : base(0, 3, 0, 3) {
      Initialize();
    }

    private void Initialize() {
      var ifFoodAhead = new IfFoodAhead();
      var prog2 = new Prog2();
      var prog3 = new Prog3();
      var move = new Move();
      var left = new Left();
      var right = new Right();
      var defun = new Defun();
      var invoke = new InvokeFunction();
      var allSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3, move, left, right };
      var nonTerminalSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3 };
      SetMinSubTreeCount(ifFoodAhead, 2);
      SetMaxSubTreeCount(ifFoodAhead, 2);
      SetMinSubTreeCount(prog2, 2);
      SetMaxSubTreeCount(prog2, 2);
      SetMinSubTreeCount(prog3, 3);
      SetMaxSubTreeCount(prog3, 3);
      SetMinSubTreeCount(move, 0);
      SetMaxSubTreeCount(move, 0);
      SetMinSubTreeCount(left, 0);
      SetMaxSubTreeCount(left, 0);
      SetMinSubTreeCount(right, 0);
      SetMaxSubTreeCount(right, 0);
      foreach (var sym in allSymbols) {
        AddAllowedSymbols(StartSymbol, 0, sym);
        AddAllowedSymbols(defun, 0, sym);

        for (int i = 0; i < GetMaxSubTreeCount(invoke); i++) {
          AddAllowedSymbols(invoke, i, sym);
        }
      }
      foreach (var sym in nonTerminalSymbols) {
        for (int argIndex = 0; argIndex < GetMaxSubTreeCount(sym); argIndex++) {
          AddAllowedSymbols(sym, argIndex, invoke);
        }
      }
      foreach (var nonTerminal in nonTerminalSymbols) {
        foreach (var child in allSymbols) {
          for (int argIndex = 0; argIndex < GetMaxSubTreeCount(nonTerminal); argIndex++) {
            AddAllowedSymbols(nonTerminal, argIndex, child);
          }
        }
      }

    }
  }
}
