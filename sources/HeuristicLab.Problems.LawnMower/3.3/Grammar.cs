#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  [Item("Lawn Mower Grammar", "The grammar for the lawn mower GP problem.")]
  public sealed class Grammar : SymbolicExpressionGrammar {
    [StorableConstructor]
    private Grammar(bool deserializing) : base(deserializing) { }
    private Grammar(Grammar original, Cloner cloner)
      : base(original, cloner) {
    }

    public Grammar()
      : base("Grammar", "Gammar for the lawn mower demo GP problem.") {
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Grammar(this, cloner);
    }
    private void Initialize() {
      // create all symbols
      var sum = new Sum();
      var prog = new Prog();
      var frog = new Frog();
      var left = new Left();
      var forward = new Forward();
      var constant = new Constant();

      var allSymbols = new List<ISymbol>() { sum, prog, frog, left, forward, constant };

      // add all symbols to the grammar
      foreach (var s in allSymbols)
        AddSymbol(s);

      // define grammar rules
      foreach (var s in allSymbols) {
        AddAllowedChildSymbol(sum, s);
        AddAllowedChildSymbol(sum, s);
        AddAllowedChildSymbol(prog, s);
        AddAllowedChildSymbol(prog, s);
        AddAllowedChildSymbol(frog, s);
        AddAllowedChildSymbol(StartSymbol, s);
      }
    }
  }
}
