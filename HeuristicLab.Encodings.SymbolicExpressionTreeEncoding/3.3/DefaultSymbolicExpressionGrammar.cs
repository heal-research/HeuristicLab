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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  /// <summary>
  /// The default symbolic expression grammar stores symbols and syntactic constraints for symbols.
  /// Symbols are treated as equvivalent if they have the same name.
  /// Syntactic constraints limit the number of allowed sub trees for a node with a symbol and which symbols are allowed 
  /// in the sub-trees of a symbol (can be specified for each sub-tree index separately).
  /// </summary>
  [StorableClass]
  [Item("DefaultSymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public abstract class DefaultSymbolicExpressionGrammar : Item, ISymbolicExpressionGrammar {

    #region properties for separation between implementation and persistence
    [Storable]
    private IEnumerable<KeyValuePair<string, int>> MinSubTreeCount {
      get { return minSubTreeCount.AsEnumerable(); }
      set { minSubTreeCount = value.ToDictionary(x => x.Key, x => x.Value); }
    }

    [Storable]
    private IEnumerable<KeyValuePair<string, int>> MaxSubTreeCount {
      get { return maxSubTreeCount.AsEnumerable(); }
      set { maxSubTreeCount = value.ToDictionary(x => x.Key, x => x.Value); }
    }

    [Storable]
    private IEnumerable<KeyValuePair<string, IEnumerable<IEnumerable<string>>>> AllowedChildSymbols {
      get {
        return (from parentEntry in allowedChildSymbols
                let setEnumeration = parentEntry.Value.Select(set => set.AsEnumerable()).ToList()
                select new KeyValuePair<string, IEnumerable<IEnumerable<string>>>(parentEntry.Key, setEnumeration))
                .ToList();
      }
      set {
        allowedChildSymbols = new Dictionary<string, List<List<string>>>();
        foreach (var pair in value) {
          allowedChildSymbols[pair.Key] = new List<List<string>>();
          foreach (var entry in pair.Value) {
            var hashSet = new List<string>();
            foreach (string child in entry) {
              hashSet.Add(child);
            }
            allowedChildSymbols[pair.Key].Add(hashSet);
          }
        }
      }
    }
    [Storable]
    private IEnumerable<KeyValuePair<string, Symbol>> AllSymbols {
      get { return allSymbols.AsEnumerable(); }
      set { allSymbols = value.ToDictionary(x => x.Key, x => x.Value); }
    }
    #endregion

    private Dictionary<string, int> minSubTreeCount;
    private Dictionary<string, int> maxSubTreeCount;
    private Dictionary<string, List<List<string>>> allowedChildSymbols;
    private Dictionary<string, Symbol> allSymbols;
    [Storable]
    private Symbol startSymbol;

    [StorableConstructor]
    protected DefaultSymbolicExpressionGrammar(bool deserializing)
      : base(deserializing) {
      cachedMinExpressionLength = new Dictionary<string, int>();
      cachedMaxExpressionLength = new Dictionary<string, int>();
      cachedMinExpressionDepth = new Dictionary<string, int>();
    }
    // cloning ctor
    protected DefaultSymbolicExpressionGrammar(DefaultSymbolicExpressionGrammar original, Cloner cloner)
      : base(original, cloner) {
      this.cachedMinExpressionLength = new Dictionary<string, int>();
      this.cachedMaxExpressionLength = new Dictionary<string, int>();
      this.cachedMinExpressionDepth = new Dictionary<string, int>();
      minSubTreeCount = new Dictionary<string, int>(original.minSubTreeCount);
      maxSubTreeCount = new Dictionary<string, int>(original.maxSubTreeCount);

      allSymbols = new Dictionary<string, Symbol>();
      foreach (Symbol symbol in original.allSymbols.Values.Select(s => cloner.Clone(s)))
        allSymbols.Add(symbol.Name, symbol);

      startSymbol = cloner.Clone<Symbol>(original.startSymbol);
      allowedChildSymbols = new Dictionary<string, List<List<string>>>();
      foreach (var entry in original.allowedChildSymbols) {
        allowedChildSymbols[entry.Key] = new List<List<string>>(entry.Value.Count);
        foreach (var set in entry.Value) {
          allowedChildSymbols[entry.Key].Add(new List<string>(set));
        }
      }
    }
    protected DefaultSymbolicExpressionGrammar()
      : base() {
      this.minSubTreeCount = new Dictionary<string, int>();
      this.maxSubTreeCount = new Dictionary<string, int>();
      this.allowedChildSymbols = new Dictionary<string, List<List<string>>>();
      this.allSymbols = new Dictionary<string, Symbol>();
      this.cachedMinExpressionLength = new Dictionary<string, int>();
      this.cachedMaxExpressionLength = new Dictionary<string, int>();
      this.cachedMinExpressionDepth = new Dictionary<string, int>();

      this.startSymbol = new StartSymbol();
      this.AddSymbol(startSymbol);
      this.SetMinSubtreeCount(startSymbol, 1);
      this.SetMaxSubtreeCount(startSymbol, 1);
    }

    protected DefaultSymbolicExpressionGrammar(ISymbolicExpressionGrammar grammar)
      : base() {
      Cloner cloner = new Cloner();
      this.cachedMinExpressionLength = new Dictionary<string, int>();
      this.cachedMaxExpressionLength = new Dictionary<string, int>();
      this.cachedMinExpressionDepth = new Dictionary<string, int>();

      this.minSubTreeCount = new Dictionary<string, int>();
      this.maxSubTreeCount = new Dictionary<string, int>();
      this.allowedChildSymbols = new Dictionary<string, List<List<string>>>();
      this.allSymbols = new Dictionary<string, Symbol>();

      this.StartSymbol = (Symbol)cloner.Clone(grammar.StartSymbol);

      foreach (Symbol symbol in grammar.Symbols) {
        Symbol clonedSymbol = (Symbol)cloner.Clone(symbol);
        this.AddSymbol(clonedSymbol);
        this.SetMinSubtreeCount(clonedSymbol, grammar.GetMinSubtreeCount(symbol));
        this.SetMaxSubtreeCount(clonedSymbol, grammar.GetMaxSubtreeCount(symbol));
      }

      foreach (Symbol parent in grammar.Symbols) {
        for (int i = 0; i < grammar.GetMaxSubtreeCount(parent); i++) {
          foreach (Symbol child in grammar.Symbols) {
            if (grammar.IsAllowedChild(parent, child, i)) {
              this.SetAllowedChild((Symbol)cloner.Clone(parent), (Symbol)cloner.Clone(child), i);
            }
          }
        }
      }
    }

    public void Clear() {
      minSubTreeCount.Clear();
      maxSubTreeCount.Clear();
      allowedChildSymbols.Clear();
      allSymbols.Clear();

      cachedMaxExpressionLength.Clear();
      cachedMinExpressionLength.Clear();
      cachedMinExpressionDepth.Clear();

      startSymbol = new StartSymbol();
      AddSymbol(startSymbol);
      SetMinSubtreeCount(startSymbol, 1);
      SetMaxSubtreeCount(startSymbol, 1);
    }

    #region ISymbolicExpressionGrammar Members
    public Symbol StartSymbol {
      get { return startSymbol; }
      set { startSymbol = value; }
    }

    public void AddSymbol(Symbol symbol) {
      if (ContainsSymbol(symbol)) throw new ArgumentException("Symbol " + symbol + " is already defined.");
      allSymbols.Add(symbol.Name, symbol);
      allowedChildSymbols[symbol.Name] = new List<List<string>>();
      ClearCaches();
    }

    public void RemoveSymbol(Symbol symbol) {
      foreach (var parent in Symbols) {
        for (int i = 0; i < GetMaxSubtreeCount(parent); i++)
          if (IsAllowedChild(parent, symbol, i))
            allowedChildSymbols[parent.Name][i].Remove(symbol.Name);
      }
      allSymbols.Remove(symbol.Name);
      minSubTreeCount.Remove(symbol.Name);
      maxSubTreeCount.Remove(symbol.Name);
      allowedChildSymbols.Remove(symbol.Name);
      ClearCaches();
    }

    public IEnumerable<Symbol> Symbols {
      get { return allSymbols.Values.AsEnumerable(); }
    }

    public bool ContainsSymbol(Symbol symbol) {
      return allSymbols.ContainsKey(symbol.Name);
    }

    public void SetAllowedChild(Symbol parent, Symbol child, int argumentIndex) {
      if (!ContainsSymbol(parent)) throw new ArgumentException("Unknown symbol: " + parent, "parent");
      if (!ContainsSymbol(child)) throw new ArgumentException("Unknown symbol: " + child, "child");
      if (argumentIndex >= GetMaxSubtreeCount(parent)) throw new ArgumentException("Symbol " + parent + " can have only " + GetMaxSubtreeCount(parent) + " subtrees.");
      allowedChildSymbols[parent.Name][argumentIndex].Add(child.Name);
      ClearCaches();
    }

    public bool IsAllowedChild(Symbol parent, Symbol child, int argumentIndex) {
      if (!ContainsSymbol(parent)) throw new ArgumentException("Unknown symbol: " + parent, "parent");
      if (!ContainsSymbol(child)) throw new ArgumentException("Unknown symbol: " + child, "child");
      if (argumentIndex >= GetMaxSubtreeCount(parent)) throw new ArgumentException("Symbol " + parent + " can have only " + GetMaxSubtreeCount(parent) + " subtrees.");
      return allowedChildSymbols[parent.Name][argumentIndex].Contains(child.Name);
    }

    private Dictionary<string, int> cachedMinExpressionLength;
    public int GetMinExpressionLength(Symbol symbol) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMinExpressionLength.ContainsKey(symbol.Name)) {
        cachedMinExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMinExpressionLengths = 1 + (from argIndex in Enumerable.Range(0, GetMinSubtreeCount(symbol))
                                              let minForSlot = (long)(from s in Symbols
                                                                      where IsAllowedChild(symbol, s, argIndex)
                                                                      select GetMinExpressionLength(s)).DefaultIfEmpty(0).Min()
                                              select minForSlot).DefaultIfEmpty(0).Sum();

        cachedMinExpressionLength[symbol.Name] = (int)Math.Min(sumOfMinExpressionLengths, int.MaxValue);
      }
      return cachedMinExpressionLength[symbol.Name];
    }

    private Dictionary<string, int> cachedMaxExpressionLength;
    public int GetMaxExpressionLength(Symbol symbol) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMaxExpressionLength.ContainsKey(symbol.Name)) {
        cachedMaxExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMaxTrees = 1 + (from argIndex in Enumerable.Range(0, GetMaxSubtreeCount(symbol))
                                  let maxForSlot = (long)(from s in Symbols
                                                          where IsAllowedChild(symbol, s, argIndex)
                                                          select GetMaxExpressionLength(s)).DefaultIfEmpty(0).Max()
                                  select maxForSlot).DefaultIfEmpty(0).Sum();
        long limit = int.MaxValue;
        cachedMaxExpressionLength[symbol.Name] = (int)Math.Min(sumOfMaxTrees, limit);
      }
      return cachedMaxExpressionLength[symbol.Name];
    }

    private Dictionary<string, int> cachedMinExpressionDepth;
    public int GetMinExpressionDepth(Symbol symbol) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMinExpressionDepth.ContainsKey(symbol.Name)) {
        cachedMinExpressionDepth[symbol.Name] = int.MaxValue; // prevent infinite recursion
        cachedMinExpressionDepth[symbol.Name] = 1 + (from argIndex in Enumerable.Range(0, GetMinSubtreeCount(symbol))
                                                     let minForSlot = (from s in Symbols
                                                                       where IsAllowedChild(symbol, s, argIndex)
                                                                       select GetMinExpressionDepth(s)).DefaultIfEmpty(0).Min()
                                                     select minForSlot).DefaultIfEmpty(0).Max();
      }
      return cachedMinExpressionDepth[symbol.Name];
    }

    public void SetMaxSubtreeCount(Symbol symbol, int nSubTrees) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      maxSubTreeCount[symbol.Name] = nSubTrees;
      while (allowedChildSymbols[symbol.Name].Count <= nSubTrees)
        allowedChildSymbols[symbol.Name].Add(new List<string>());
      while (allowedChildSymbols[symbol.Name].Count > nSubTrees) {
        allowedChildSymbols[symbol.Name].RemoveAt(allowedChildSymbols[symbol.Name].Count - 1);
      }
      ClearCaches();
    }

    public void SetMinSubtreeCount(Symbol symbol, int nSubTrees) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      minSubTreeCount[symbol.Name] = nSubTrees;
      ClearCaches();
    }

    public int GetMinSubtreeCount(Symbol symbol) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      return minSubTreeCount[symbol.Name];
    }

    public int GetMaxSubtreeCount(Symbol symbol) {
      if (!ContainsSymbol(symbol)) throw new ArgumentException("Unknown symbol: " + symbol);
      return maxSubTreeCount[symbol.Name];
    }
    #endregion

    private void ClearCaches() {
      cachedMinExpressionLength.Clear();
      cachedMaxExpressionLength.Clear();
      cachedMinExpressionDepth.Clear();
    }

    protected void InitializeShallowClone(DefaultSymbolicExpressionGrammar original) {
      minSubTreeCount = new Dictionary<string, int>(original.minSubTreeCount);
      maxSubTreeCount = new Dictionary<string, int>(original.maxSubTreeCount);

      allSymbols = new Dictionary<string, Symbol>(original.allSymbols);
      startSymbol = original.startSymbol;
      allowedChildSymbols = new Dictionary<string, List<List<string>>>(original.allowedChildSymbols.Count);
      foreach (var entry in original.allowedChildSymbols) {
        allowedChildSymbols[entry.Key] = new List<List<string>>(entry.Value.Count);
        foreach (var set in entry.Value) {
          allowedChildSymbols[entry.Key].Add(new List<string>(set));
        }
      }
    }

  }
}
