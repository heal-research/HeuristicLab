#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  internal sealed class SymbolicExpressionTreeGrammar : SymbolicExpressionGrammarBase, ISymbolicExpressionTreeGrammar {
    [StorableConstructor]
    private SymbolicExpressionTreeGrammar(bool deserializing) : base(deserializing) { }
    private SymbolicExpressionTreeGrammar(SymbolicExpressionTreeGrammar original, Cloner cloner)
      : base(original, cloner) {
      this.grammar = original.grammar;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      foreach (ISymbol symbol in base.Symbols)
        if (!cloner.ClonedObjectRegistered(symbol))
          cloner.RegisterClonedObject(symbol, symbol);
      return new SymbolicExpressionTreeGrammar(this, cloner);
    }

    [Storable]
    private ISymbolicExpressionGrammar grammar;
    public SymbolicExpressionTreeGrammar(ISymbolicExpressionGrammar grammar)
      : base("SymbolicExpressionTreeGrammar", "A grammar that is used held by symbolic expression trees and allows extensions to the wrapped grammar.") {
      if (grammar == null) throw new ArgumentNullException();
      this.grammar = grammar;
    }

    public override IEnumerable<ISymbol> Symbols {
      get {
        foreach (var s in grammar.Symbols) yield return s;
        foreach (var s in base.symbols.Values) yield return s;
      }
    }
    public IEnumerable<ISymbol> ModifyableSymbols {
      get { return base.symbols.Values; }
    }
    public bool IsModifyableSymbol(ISymbol symbol) {
      return base.symbols.ContainsKey(symbol.Name);
    }

    public override bool ContainsSymbol(ISymbol symbol) {
      return grammar.ContainsSymbol(symbol) || base.ContainsSymbol(symbol);
    }
    public override ISymbol GetSymbol(string symbolName) {
      var symbol = grammar.GetSymbol(symbolName);
      if (symbol != null) return symbol;
      symbol = base.GetSymbol(symbolName);
      if (symbol != null) return symbol;
      throw new ArgumentException();
    }

    public override void RemoveSymbol(ISymbol symbol) {
      if (!IsModifyableSymbol(symbol)) throw new InvalidOperationException();
      base.RemoveSymbol(symbol);
    }

    public override int GetMinimumSubtreeCount(ISymbol symbol) {
      if (grammar.ContainsSymbol(symbol)) return grammar.GetMinimumSubtreeCount(symbol);
      return base.GetMinimumSubtreeCount(symbol);
    }
    public override int GetMaximumSubtreeCount(ISymbol symbol) {
      if (grammar.ContainsSymbol(symbol)) return grammar.GetMaximumSubtreeCount(symbol);
      return base.GetMaximumSubtreeCount(symbol);
    }
    public override void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount) {
      if (!IsModifyableSymbol(symbol)) throw new InvalidOperationException();
      base.SetSubtreeCount(symbol, minimumSubtreeCount, maximumSubtreeCount);
    }

    public override bool IsAllowedChildSymbol(ISymbol parent, ISymbol child) {
      return grammar.IsAllowedChildSymbol(parent, child) || base.IsAllowedChildSymbol(parent, child);
    }
    public override bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      return grammar.IsAllowedChildSymbol(parent, child, argumentIndex) || base.IsAllowedChildSymbol(parent, child, argumentIndex);
    }
  }
}
