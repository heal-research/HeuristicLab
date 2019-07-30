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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {

  [StorableType("59B5B5B9-0F49-4994-B221-856AA43C28B4")]
  internal sealed class EmptySymbolicExpressionTreeGrammar : NamedItem, ISymbolicExpressionTreeGrammar {
    [Storable]
    private ISymbolicExpressionGrammar grammar;

    [StorableConstructor]
    private EmptySymbolicExpressionTreeGrammar(StorableConstructorFlag _) : base(_) { }
    internal EmptySymbolicExpressionTreeGrammar(ISymbolicExpressionGrammar grammar)
      : base() {
      if (grammar == null) throw new ArgumentNullException();
      this.grammar = grammar;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return this;
    }

    public IEnumerable<ISymbol> Symbols {
      get { return grammar.Symbols; }
    }
    public IEnumerable<ISymbol> AllowedSymbols {
      get { return grammar.AllowedSymbols; }
    }

    public ISymbol GetSymbol(string symbolName) {
      return grammar.GetSymbol(symbolName);
    }

    public bool ContainsSymbol(ISymbol symbol) {
      return grammar.ContainsSymbol(symbol);
    }

    public bool IsAllowedChildSymbol(ISymbol parent, ISymbol child) {
      return grammar.IsAllowedChildSymbol(parent, child);
    }
    public bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      return grammar.IsAllowedChildSymbol(parent, child, argumentIndex);
    }

    public IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent) {
      return grammar.GetAllowedChildSymbols(parent);
    }

    public IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int argumentIndex) {
      return grammar.GetAllowedChildSymbols(parent, argumentIndex);
    }

    public int GetMinimumSubtreeCount(ISymbol symbol) {
      return grammar.GetMinimumSubtreeCount(symbol);
    }
    public int GetMaximumSubtreeCount(ISymbol symbol) {
      return grammar.GetMaximumSubtreeCount(symbol);
    }

    public int GetMinimumExpressionDepth(ISymbol symbol) {
      return grammar.GetMinimumExpressionDepth(symbol);
    }
    public int GetMaximumExpressionDepth(ISymbol symbol) {
      return grammar.GetMaximumExpressionDepth(symbol);
    }
    public int GetMinimumExpressionLength(ISymbol symbol) {
      return grammar.GetMinimumExpressionLength(symbol);
    }
    public int GetMaximumExpressionLength(ISymbol symbol, int maxDepth) {
      return grammar.GetMaximumExpressionLength(symbol, maxDepth);
    }

    public void AddSymbol(ISymbol symbol) { throw new NotSupportedException(); }
    public void RemoveSymbol(ISymbol symbol) { throw new NotSupportedException(); }
    public void AddAllowedChildSymbol(ISymbol parent, ISymbol child) { throw new NotSupportedException(); }
    public void AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) { throw new NotSupportedException(); }
    public void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child) { throw new NotSupportedException(); }
    public void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) { throw new NotSupportedException(); }
    public void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount) { throw new NotSupportedException(); }


    #region ISymbolicExpressionTreeGrammar Members
    public IEnumerable<ISymbol> ModifyableSymbols {
      get { return Enumerable.Empty<ISymbol>(); }
    }

    public bool IsModifyableSymbol(ISymbol symbol) {
      return false;
    }

#pragma warning disable 0067 //disable usage warning
    public event EventHandler Changed;
#pragma warning restore 0067
    #endregion
  }
}
