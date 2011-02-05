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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ArtificialAnt.Symbols;
namespace HeuristicLab.Problems.ArtificialAnt {
  [StorableClass]
  public class ArtificialAntExpressionGrammar : DefaultSymbolicExpressionGrammar {

    public ArtificialAntExpressionGrammar()
      : base() {
      Initialize();
    }
    [StorableConstructor]
    protected ArtificialAntExpressionGrammar(bool deserializing) : base(deserializing) { }
    protected ArtificialAntExpressionGrammar(ArtificialAntExpressionGrammar original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ArtificialAntExpressionGrammar(this, cloner);
    }

    private void Initialize() {
      var ifFoodAhead = new IfFoodAhead();
      var prog2 = new Prog2();
      var prog3 = new Prog3();
      var move = new Move();
      var left = new Left();
      var right = new Right();
      var defun = new Defun();
      var allSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3, move, left, right };
      var nonTerminalSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3 };

      allSymbols.ForEach(s => AddSymbol(s));
      SetMinSubtreeCount(ifFoodAhead, 2);
      SetMaxSubtreeCount(ifFoodAhead, 2);
      SetMinSubtreeCount(prog2, 2);
      SetMaxSubtreeCount(prog2, 2);
      SetMinSubtreeCount(prog3, 3);
      SetMaxSubtreeCount(prog3, 3);
      SetMinSubtreeCount(move, 0);
      SetMaxSubtreeCount(move, 0);
      SetMinSubtreeCount(left, 0);
      SetMaxSubtreeCount(left, 0);
      SetMinSubtreeCount(right, 0);
      SetMaxSubtreeCount(right, 0);

      // each symbols is allowed as child of the start symbol
      allSymbols.ForEach(s => SetAllowedChild(StartSymbol, s, 0));

      // each symbol is allowed as child of all other symbols (except for terminals that have MaxSubtreeCount == 0
      foreach (var parent in allSymbols) {
        for (int argIndex = 0; argIndex < GetMaxSubtreeCount(parent); argIndex++) {
          foreach (var child in allSymbols) {
            SetAllowedChild(parent, child, argIndex);
          }
        }
      }
    }
  }
}
