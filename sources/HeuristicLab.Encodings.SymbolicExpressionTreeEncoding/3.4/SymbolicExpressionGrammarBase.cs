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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// The default symbolic expression grammar stores symbols and syntactic constraints for symbols.
  /// Symbols are treated as equvivalent if they have the same name.
  /// Syntactic constraints limit the number of allowed sub trees for a node with a symbol and which symbols are allowed 
  /// in the sub-trees of a symbol (can be specified for each sub-tree index separately).
  /// </summary>
  [StorableClass]
  public abstract class SymbolicExpressionGrammarBase : NamedItem, ISymbolicExpressionGrammarBase {
    #region properties for separation between implementation and persistence
    [Storable(Name = "Symbols")]
    private IEnumerable<ISymbol> StorableSymbols {
      get { return symbols.Values.ToArray(); }
      set { symbols = value.ToDictionary(sym => sym.Name); }
    }

    [Storable(Name = "SymbolSubtreeCount")]
    private IEnumerable<KeyValuePair<ISymbol, Tuple<int, int>>> StorableSymbolSubtreeCount {
      get { return symbolSubtreeCount.Select(x => new KeyValuePair<ISymbol, Tuple<int, int>>(GetSymbol(x.Key), x.Value)).ToArray(); }
      set { symbolSubtreeCount = value.ToDictionary(x => x.Key.Name, x => x.Value); }
    }

    [Storable(Name = "AllowedChildSymbols")]
    private IEnumerable<KeyValuePair<ISymbol, IEnumerable<ISymbol>>> StorableAllowedChildSymbols {
      get { return allowedChildSymbols.Select(x => new KeyValuePair<ISymbol, IEnumerable<ISymbol>>(GetSymbol(x.Key), x.Value.Select(y => GetSymbol(y)).ToArray())).ToArray(); }
      set { allowedChildSymbols = value.ToDictionary(x => x.Key.Name, x => x.Value.Select(y => y.Name).ToList()); }
    }

    [Storable(Name = "AllowedChildSymbolsPerIndex")]
    private IEnumerable<KeyValuePair<Tuple<ISymbol, int>, IEnumerable<ISymbol>>> StorableAllowedChildSymbolsPerIndex {
      get { return allowedChildSymbolsPerIndex.Select(x => new KeyValuePair<Tuple<ISymbol, int>, IEnumerable<ISymbol>>(Tuple.Create<ISymbol, int>(GetSymbol(x.Key.Item1), x.Key.Item2), x.Value.Select(y => GetSymbol(y)).ToArray())).ToArray(); }
      set { allowedChildSymbolsPerIndex = value.ToDictionary(x => Tuple.Create(x.Key.Item1.Name, x.Key.Item2), x => x.Value.Select(y => y.Name).ToList()); }
    }
    #endregion

    protected Dictionary<string, ISymbol> symbols;
    protected Dictionary<string, Tuple<int, int>> symbolSubtreeCount;
    protected Dictionary<string, List<string>> allowedChildSymbols;
    protected Dictionary<Tuple<string, int>, List<string>> allowedChildSymbolsPerIndex;

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [StorableConstructor]
    protected SymbolicExpressionGrammarBase(bool deserializing)
      : base(deserializing) {
      cachedMinExpressionLength = new Dictionary<string, int>();
      cachedMaxExpressionLength = new Dictionary<string, int>();
      cachedMinExpressionDepth = new Dictionary<string, int>();
    }

    protected SymbolicExpressionGrammarBase(SymbolicExpressionGrammarBase original, Cloner cloner)
      : base(original, cloner) {
      cachedMinExpressionLength = new Dictionary<string, int>();
      cachedMaxExpressionLength = new Dictionary<string, int>();
      cachedMinExpressionDepth = new Dictionary<string, int>();

      symbols = original.symbols.ToDictionary(x => x.Key, y => (ISymbol)cloner.Clone(y.Value));
      symbolSubtreeCount = new Dictionary<string, Tuple<int, int>>(original.symbolSubtreeCount);

      allowedChildSymbols = new Dictionary<string, List<string>>();
      foreach (var element in original.allowedChildSymbols)
        allowedChildSymbols.Add(element.Key, new List<string>(element.Value));

      allowedChildSymbolsPerIndex = new Dictionary<Tuple<string, int>, List<string>>();
      foreach (var element in original.allowedChildSymbolsPerIndex)
        allowedChildSymbolsPerIndex.Add(element.Key, new List<string>(element.Value));
    }

    protected SymbolicExpressionGrammarBase(string name, string description)
      : base(name, description) {
      cachedMinExpressionLength = new Dictionary<string, int>();
      cachedMaxExpressionLength = new Dictionary<string, int>();
      cachedMinExpressionDepth = new Dictionary<string, int>();

      symbols = new Dictionary<string, ISymbol>();
      symbolSubtreeCount = new Dictionary<string, Tuple<int, int>>();
      allowedChildSymbols = new Dictionary<string, List<string>>();
      allowedChildSymbolsPerIndex = new Dictionary<Tuple<string, int>, List<string>>();
    }

    #region protected grammar manipulation methods
    protected virtual void AddSymbol(ISymbol symbol) {
      if (ContainsSymbol(symbol)) throw new ArgumentException("Symbol " + symbol + " is already defined.");
      symbols.Add(symbol.Name, symbol);
      symbolSubtreeCount.Add(symbol.Name, Tuple.Create(0, 0));
      ClearCaches();
    }

    protected virtual void RemoveSymbol(ISymbol symbol) {
      symbols.Remove(symbol.Name);
      allowedChildSymbols.Remove(symbol.Name);
      for (int i = 0; i < GetMaximumSubtreeCount(symbol); i++)
        allowedChildSymbolsPerIndex.Remove(Tuple.Create(symbol.Name, i));
      symbolSubtreeCount.Remove(symbol.Name);

      foreach (var parent in Symbols) {
        List<string> allowedChilds;
        if (allowedChildSymbols.TryGetValue(parent.Name, out allowedChilds))
          allowedChilds.Remove(symbol.Name);

        for (int i = 0; i < GetMaximumSubtreeCount(parent); i++) {
          if (allowedChildSymbolsPerIndex.TryGetValue(Tuple.Create(parent.Name, i), out allowedChilds))
            allowedChilds.Remove(symbol.Name);
        }
      }
      ClearCaches();
    }

    public virtual ISymbol GetSymbol(string symbolName) {
      ISymbol symbol;
      if (symbols.TryGetValue(symbolName, out symbol)) return symbol;
      return null;
    }

    protected void AddAllowedChildSymbol(ISymbol parent, ISymbol child) {
      List<string> childSymbols;
      if (!allowedChildSymbols.TryGetValue(parent.Name, out childSymbols)) {
        childSymbols = new List<string>();
        allowedChildSymbols.Add(parent.Name, childSymbols);
      }
      if (childSymbols.Contains(child.Name)) throw new ArgumentException();
      childSymbols.Add(child.Name);
      ClearCaches();
    }

    protected void AddAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      var key = Tuple.Create(parent.Name, argumentIndex);
      List<string> childSymbols;
      if (!allowedChildSymbolsPerIndex.TryGetValue(key, out childSymbols)) {
        childSymbols = new List<string>();
        allowedChildSymbolsPerIndex.Add(key, childSymbols);
      }

      if (IsAllowedChildSymbol(parent, child)) throw new ArgumentException();
      if (childSymbols.Contains(child.Name)) throw new ArgumentException();
      childSymbols.Add(child.Name);
      ClearCaches();
    }

    protected void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child) {
      List<string> childSymbols;
      if (allowedChildSymbols.TryGetValue(child.Name, out childSymbols)) {
        if (allowedChildSymbols[parent.Name].Remove(child.Name))
          ClearCaches();
      }
    }

    protected void RemoveAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      var key = Tuple.Create(parent.Name, argumentIndex);
      List<string> childSymbols;
      if (allowedChildSymbolsPerIndex.TryGetValue(key, out childSymbols)) {
        if (allowedChildSymbolsPerIndex[key].Remove(child.Name))
          ClearCaches();
      }
    }

    protected void SetSubtreeCount(ISymbol symbol, int minimumSubtreeCount, int maximumSubtreeCount) {
      for (int i = GetMaximumSubtreeCount(symbol) - 1; i >= maximumSubtreeCount; i--) {
        var key = Tuple.Create(symbol.Name, i);
        allowedChildSymbolsPerIndex.Remove(key);
      }

      symbolSubtreeCount[symbol.Name] = Tuple.Create(minimumSubtreeCount, maximumSubtreeCount);
      ClearCaches();
    }
    #endregion

    #region ISymbolicExpressionGrammarBase Members
    public virtual IEnumerable<ISymbol> Symbols {
      get { return symbols.Values; }
    }
    public virtual IEnumerable<ISymbol> AllowedSymbols {
      get { return Symbols.Where(s => !s.InitialFrequency.IsAlmost(0.0)); }
    }
    public virtual bool ContainsSymbol(ISymbol symbol) {
      return symbols.ContainsKey(symbol.Name);
    }

    public virtual bool IsAllowedChildSymbol(ISymbol parent, ISymbol child) {
      List<string> temp;
      if (allowedChildSymbols.TryGetValue(parent.Name, out temp))
        if (temp.Contains(child.Name)) return true;
      return false;
    }

    public virtual bool IsAllowedChildSymbol(ISymbol parent, ISymbol child, int argumentIndex) {
      List<string> temp;
      if (allowedChildSymbols.TryGetValue(parent.Name, out temp))
        if (temp.Contains(child.Name)) return true;

      var key = Tuple.Create(parent.Name, argumentIndex);
      if (allowedChildSymbolsPerIndex.TryGetValue(key, out temp))
        return temp.Contains(child.Name);
      return false;
    }

    public virtual IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent) {
      return from s in AllowedSymbols where IsAllowedChildSymbol(parent, s) select s;
    }

    public virtual IEnumerable<ISymbol> GetAllowedChildSymbols(ISymbol parent, int argumentIndex) {
      var result = Enumerable.Empty<string>();

      List<string> temp;
      if (allowedChildSymbols.TryGetValue(parent.Name, out temp))
        result = result.Union(temp);
      var key = Tuple.Create(parent.Name, argumentIndex);
      if (allowedChildSymbolsPerIndex.TryGetValue(key, out temp))
        result = result.Union(temp);

      return result.Select(x => GetSymbol(x));
    }

    public virtual int GetMinimumSubtreeCount(ISymbol symbol) {
      return symbolSubtreeCount[symbol.Name].Item1;
    }
    public virtual int GetMaximumSubtreeCount(ISymbol symbol) {
      return symbolSubtreeCount[symbol.Name].Item2;
    }


    protected void ClearCaches() {
      cachedMinExpressionLength.Clear();
      cachedMaxExpressionLength.Clear();
      cachedMinExpressionDepth.Clear();
    }

    private Dictionary<string, int> cachedMinExpressionLength;
    public int GetMinimumExpressionLength(ISymbol symbol) {
      int temp;
      if (!cachedMinExpressionLength.TryGetValue(symbol.Name, out temp)) {
        cachedMinExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMinExpressionLengths = 1 + (from argIndex in Enumerable.Range(0, GetMinimumSubtreeCount(symbol))
                                              let minForSlot = (long)(from s in AllowedSymbols
                                                                      where IsAllowedChildSymbol(symbol, s, argIndex)
                                                                      select GetMinimumExpressionLength(s)).DefaultIfEmpty(0).Min()
                                              select minForSlot).DefaultIfEmpty(0).Sum();

        cachedMinExpressionLength[symbol.Name] = (int)Math.Min(sumOfMinExpressionLengths, int.MaxValue);
        return cachedMinExpressionLength[symbol.Name];
      }
      return temp;
    }

    private Dictionary<string, int> cachedMaxExpressionLength;
    public int GetMaximumExpressionLength(ISymbol symbol) {
      int temp;
      if (!cachedMaxExpressionLength.TryGetValue(symbol.Name, out temp)) {
        cachedMaxExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMaxTrees = 1 + (from argIndex in Enumerable.Range(0, GetMaximumSubtreeCount(symbol))
                                  let maxForSlot = (long)(from s in AllowedSymbols
                                                          where IsAllowedChildSymbol(symbol, s, argIndex)
                                                          select GetMaximumExpressionLength(s)).DefaultIfEmpty(0).Max()
                                  select maxForSlot).DefaultIfEmpty(0).Sum();
        cachedMaxExpressionLength[symbol.Name] = (int)Math.Min(sumOfMaxTrees, int.MaxValue);
        return cachedMaxExpressionLength[symbol.Name];
      }
      return temp;
    }

    private Dictionary<string, int> cachedMinExpressionDepth;
    public int GetMinimumExpressionDepth(ISymbol symbol) {
      int temp;
      if (!cachedMinExpressionDepth.TryGetValue(symbol.Name, out temp)) {
        cachedMinExpressionDepth[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long minDepth = 1 + (from argIndex in Enumerable.Range(0, GetMinimumSubtreeCount(symbol))
                             let minForSlot = (long)(from s in AllowedSymbols
                                                     where IsAllowedChildSymbol(symbol, s, argIndex)
                                                     select GetMinimumExpressionDepth(s)).DefaultIfEmpty(0).Min()
                             select minForSlot).DefaultIfEmpty(0).Max();
        cachedMinExpressionDepth[symbol.Name] = (int)Math.Min(minDepth, int.MaxValue);
        return cachedMinExpressionDepth[symbol.Name];
      }
      return temp;
    }
    #endregion
  }
}
