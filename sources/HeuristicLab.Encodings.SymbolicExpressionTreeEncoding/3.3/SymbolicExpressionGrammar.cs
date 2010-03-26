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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("SymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public class SymbolicExpressionGrammar : Item, ISymbolicExpressionGrammar {
    //private List<Symbol> symbols = new List<Symbol>();
    //public IEnumerable<Symbol> Symbols {
    //  get { return symbols; }
    //}

    // internal implementation of a symbol for the start symbol
    private class EmptySymbol : Symbol {
    }

    private Dictionary<Symbol, Dictionary<int, IEnumerable<Symbol>>> allowedSymbols;

    public SymbolicExpressionGrammar()
      : base() {
      startSymbol = new EmptySymbol();
      allowedSymbols = new Dictionary<Symbol, Dictionary<int, IEnumerable<Symbol>>>();
    }

    //public void AddSymbol(Symbol symbol) {
    //  if (!symbols.Contains(symbol)) {
    //    symbols.Add(symbol);
    //    symbol.ToStringChanged += new EventHandler(symbol_ToStringChanged);

    //    OnToStringChanged();
    //  }
    //}

    //public void RemoveSymbol(Symbol symbol) {
    //  symbols.Remove(symbol);
    //  symbol.ToStringChanged -= new EventHandler(symbol_ToStringChanged);

    //  // remove the operator from the allowed sub-functions of all functions
    //  foreach (Symbol f in Symbols) {
    //    for (int i = 0; i < f.MaxSubTrees; i++) {
    //      f.RemoveAllowedSubFunction(symbol, i);
    //    }
    //  }
    //  OnToStringChanged();
    //}

    private void symbol_ToStringChanged(object sender, EventArgs e) {
      OnToStringChanged();
    }

    #region ISymbolicExpressionGrammar Members

    public Symbol startSymbol;
    public Symbol StartSymbol {
      get { return startSymbol; }
    }
   
    public IEnumerable<Symbol> AllowedSymbols(Symbol parent, int argumentIndex) {
      return allowedSymbols[parent][argumentIndex];
    }

    public int MinimalExpressionLength(Symbol start) {
      throw new NotImplementedException();
    }

    public int MaximalExpressionLength(Symbol start) {
      throw new NotImplementedException();
    }

    public int MinimalExpressionDepth(Symbol start) {
      throw new NotImplementedException();
    }

    public int MinSubTrees(Symbol start) {
      throw new NotImplementedException();
    }

    public int MaxSubTrees(Symbol start) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
