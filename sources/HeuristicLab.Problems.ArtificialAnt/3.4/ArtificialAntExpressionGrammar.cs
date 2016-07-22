#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ArtificialAnt.Symbols;
namespace HeuristicLab.Problems.ArtificialAnt {
  [StorableClass]
  [Item(Name = "ArtificialAntExpressionGrammar", Description = "The default grammar for an artificial ant problem.")]
  public class ArtificialAntExpressionGrammar : SymbolicExpressionGrammar {

    public ArtificialAntExpressionGrammar()
      : base(ItemAttribute.GetName(typeof(ArtificialAntExpressionGrammar)), ItemAttribute.GetDescription(typeof(ArtificialAntExpressionGrammar))) {
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
      var allSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3, move, left, right };
      var nonTerminalSymbols = new List<Symbol>() { ifFoodAhead, prog2, prog3 };

      allSymbols.ForEach(s => AddSymbol(s));
      SetSubtreeCount(ifFoodAhead, 2, 2);
      SetSubtreeCount(prog2, 2, 2);
      SetSubtreeCount(prog3, 3, 3);
      SetSubtreeCount(move, 0, 0);
      SetSubtreeCount(left, 0, 0);
      SetSubtreeCount(right, 0, 0);

      // each symbols is allowed as child of the start symbol
      allSymbols.ForEach(s => AddAllowedChildSymbol(StartSymbol, s));
      allSymbols.ForEach(s => AddAllowedChildSymbol(DefunSymbol, s));

      // each symbol is allowed as child of all other symbols (except for terminals that have MaxSubtreeCount == 0
      foreach (var parent in nonTerminalSymbols) {
        foreach (var child in allSymbols) {
          AddAllowedChildSymbol(parent, child);
        }
      }
    }
  }
}
