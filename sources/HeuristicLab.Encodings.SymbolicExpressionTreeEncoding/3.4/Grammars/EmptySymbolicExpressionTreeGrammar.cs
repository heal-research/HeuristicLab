#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {

  [StorableClass]
  internal sealed class EmptySymbolicExpressionTreeGrammar : NamedItem, ISymbolicExpressionTreeGrammar {
    [Storable]
    private ISymbolicExpressionGrammar grammar;

    [StorableConstructor]
    internal EmptySymbolicExpressionTreeGrammar(bool deserializing) : base(deserializing) {}
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

    IEnumerable<ISymbol> ISymbolicExpressionGrammarBase.GetAllowedChildSymbols(ISymbol parent) {
      return grammar.GetAllowedChildSymbols(parent);
    }

    IEnumerable<ISymbol> ISymbolicExpressionGrammarBase.GetAllowedChildSymbols(ISymbol parent, int argumentIndex) {
      return grammar.GetAllowedChildSymbols(parent, argumentIndex);
    }

    public int GetMinimumSubtreeCount(ISymbol symbol) {
      return grammar.GetMinimumSubtreeCount(symbol);
    }
    public int GetMaximumSubtreeCount(ISymbol symbol) {
      return grammar.GetMaximumSubtreeCount(symbol);
    }

    int ISymbolicExpressionGrammarBase.GetMinimumExpressionDepth(ISymbol symbol) {
      return grammar.GetMinimumExpressionDepth(symbol);
    }
    int ISymbolicExpressionGrammarBase.GetMaximumExpressionDepth(ISymbol symbol) {
      return grammar.GetMaximumExpressionDepth(symbol);
    }
    int ISymbolicExpressionGrammarBase.GetMinimumExpressionLength(ISymbol symbol) {
      return grammar.GetMinimumExpressionLength(symbol);
    }
    int ISymbolicExpressionGrammarBase.GetMaximumExpressionLength(ISymbol symbol, int maxDepth) {
      return grammar.GetMaximumExpressionLength(symbol, maxDepth);
    }


    #region ISymbolicExpressionTreeGrammar Members
    IEnumerable<ISymbol> ISymbolicExpressionTreeGrammar.ModifyableSymbols {
      get { return Enumerable.Empty<ISymbol>(); }
    }

    bool ISymbolicExpressionTreeGrammar.IsModifyableSymbol(ISymbol symbol) {
      return false;
    }

    void ISymbolicExpressionTreeGrammar.AddSymbol(ISymbol symbol) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.RemoveSymbol(ISymbol symbol) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.AddAllowedChildSymbol(ISymbol parent, ISymbol child) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.RemoveAllowedChildSymbol(ISymbol parent, ISymbol child) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      throw new NotSupportedException();
    }

    void ISymbolicExpressionTreeGrammar.SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount) {
      throw new NotSupportedException();
    }
    
    #pragma warning disable 0169 //disable usage warning
    public event EventHandler Changed;
    #pragma warning restore 0169
    #endregion
  }
}
