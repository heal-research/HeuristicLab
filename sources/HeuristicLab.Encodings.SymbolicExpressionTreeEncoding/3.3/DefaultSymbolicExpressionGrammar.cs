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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("DefaultSymbolicExpressionGrammar", "Represents a grammar that defines the syntax of symbolic expression trees.")]
  public class DefaultSymbolicExpressionGrammar : Item, ISymbolicExpressionGrammar {
    [Storable]
    private Dictionary<string, int> minSubTreeCount;
    [Storable]
    private Dictionary<string, int> maxSubTreeCount;
    [Storable]
    private Dictionary<string, List<HashSet<string>>> allowedChildSymbols;
    [Storable]
    private HashSet<Symbol> allSymbols;

    public DefaultSymbolicExpressionGrammar()
      : base() {
      Reset();
    }

    private void Initialize() {
      startSymbol = new StartSymbol();
      AddSymbol(startSymbol);
      SetMinSubtreeCount(startSymbol, 1);
      SetMaxSubtreeCount(startSymbol, 1);
    }

    #region ISymbolicExpressionGrammar Members

    private Symbol startSymbol;
    public Symbol StartSymbol {
      get { return startSymbol; }
      set { startSymbol = value; }
    }

    protected void Reset() {
      minSubTreeCount = new Dictionary<string, int>();
      maxSubTreeCount = new Dictionary<string, int>();
      allowedChildSymbols = new Dictionary<string, List<HashSet<string>>>();
      allSymbols = new HashSet<Symbol>();
      cachedMinExpressionLength = new Dictionary<string, int>();
      cachedMaxExpressionLength = new Dictionary<string, int>();
      cachedMinExpressionDepth = new Dictionary<string, int>();
      Initialize();
    }

    public void AddSymbol(Symbol symbol) {
      if (allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Symbol " + symbol + " is already defined.");
      allSymbols.Add(symbol);
      allowedChildSymbols[symbol.Name] = new List<HashSet<string>>();
      ClearCaches();
    }

    public void RemoveSymbol(Symbol symbol) {
      allSymbols.RemoveWhere(s => s.Name == symbol.Name);
      minSubTreeCount.Remove(symbol.Name);
      maxSubTreeCount.Remove(symbol.Name);
      allowedChildSymbols.Remove(symbol.Name);
      ClearCaches();
    }

    public IEnumerable<Symbol> Symbols {
      get { return allSymbols.AsEnumerable(); }
    }

    public void SetAllowedChild(Symbol parent, Symbol child, int argumentIndex) {
      if (!allSymbols.Any(s => s.Name == parent.Name)) throw new ArgumentException("Unknown symbol: " + parent, "parent");
      if (!allSymbols.Any(s => s.Name == child.Name)) throw new ArgumentException("Unknown symbol: " + child, "child");
      if (argumentIndex >= GetMaxSubtreeCount(parent)) throw new ArgumentException("Symbol " + parent + " can have only " + GetMaxSubtreeCount(parent) + " subtrees.");
      allowedChildSymbols[parent.Name][argumentIndex].Add(child.Name);
      ClearCaches();
    }

    public bool IsAllowedChild(Symbol parent, Symbol child, int argumentIndex) {
      if (!allSymbols.Any(s => s.Name == parent.Name)) throw new ArgumentException("Unknown symbol: " + parent, "parent");
      if (!allSymbols.Any(s => s.Name == child.Name)) throw new ArgumentException("Unknown symbol: " + child, "child");
      if (argumentIndex >= GetMaxSubtreeCount(parent)) throw new ArgumentException("Symbol " + parent + " can have only " + GetMaxSubtreeCount(parent) + " subtrees.");
      if (allowedChildSymbols.ContainsKey(parent.Name)) return allowedChildSymbols[parent.Name][argumentIndex].Contains(child.Name);
      return false;
    }

    private Dictionary<string, int> cachedMinExpressionLength;
    public int GetMinExpressionLength(Symbol symbol) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMinExpressionLength.ContainsKey(symbol.Name)) {
        cachedMinExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMinExpressionLengths = 1 + (from argIndex in Enumerable.Range(0, GetMinSubtreeCount(symbol))
                                              let minForSlot = (long)(from s in allSymbols
                                                                      where IsAllowedChild(symbol, s, argIndex)
                                                                      select GetMinExpressionLength(s)).DefaultIfEmpty(0).Min()
                                              select minForSlot).DefaultIfEmpty(0).Sum();

        cachedMinExpressionLength[symbol.Name] = (int)Math.Min(sumOfMinExpressionLengths, int.MaxValue);
      }
      return cachedMinExpressionLength[symbol.Name];
    }

    private Dictionary<string, int> cachedMaxExpressionLength;
    public int GetMaxExpressionLength(Symbol symbol) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMaxExpressionLength.ContainsKey(symbol.Name)) {
        cachedMaxExpressionLength[symbol.Name] = int.MaxValue; // prevent infinite recursion
        long sumOfMaxTrees = 1 + (from argIndex in Enumerable.Range(0, GetMaxSubtreeCount(symbol))
                                  let maxForSlot = (long)(from s in allSymbols
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
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      if (!cachedMinExpressionDepth.ContainsKey(symbol.Name)) {
        cachedMinExpressionDepth[symbol.Name] = int.MaxValue; // prevent infinite recursion
        cachedMinExpressionDepth[symbol.Name] = 1 + (from argIndex in Enumerable.Range(0, GetMinSubtreeCount(symbol))
                                                     let minForSlot = (from s in allSymbols
                                                                       where IsAllowedChild(symbol, s, argIndex)
                                                                       select GetMinExpressionDepth(s)).DefaultIfEmpty(0).Min()
                                                     select minForSlot).DefaultIfEmpty(0).Max();
      }
      return cachedMinExpressionDepth[symbol.Name];
    }

    public void SetMaxSubtreeCount(Symbol symbol, int nSubTrees) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      maxSubTreeCount[symbol.Name] = nSubTrees;
      while (allowedChildSymbols[symbol.Name].Count <= nSubTrees)
        allowedChildSymbols[symbol.Name].Add(new HashSet<string>());
      while (allowedChildSymbols[symbol.Name].Count > nSubTrees) {
        allowedChildSymbols[symbol.Name].RemoveAt(allowedChildSymbols[symbol.Name].Count - 1);
      }
      ClearCaches();
    }

    public void SetMinSubtreeCount(Symbol symbol, int nSubTrees) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      minSubTreeCount[symbol.Name] = nSubTrees;
      ClearCaches();
    }

    public int GetMinSubtreeCount(Symbol symbol) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      return minSubTreeCount[symbol.Name];
    }

    public int GetMaxSubtreeCount(Symbol symbol) {
      if (!allSymbols.Any(s => s.Name == symbol.Name)) throw new ArgumentException("Unknown symbol: " + symbol);
      return maxSubTreeCount[symbol.Name];
    }

    #endregion

    private void ClearCaches() {
      cachedMinExpressionLength.Clear();
      cachedMaxExpressionLength.Clear();
      cachedMinExpressionDepth.Clear();
    }

    //private void symbol_ToStringChanged(object sender, EventArgs e) {
    //  OnToStringChanged();
    //}

    //private bool IsValidExpression(SymbolicExpressionTreeNode root) {
    //  if (root.SubTrees.Count < root.GetMinSubtreeCount()) return false;
    //  else if (root.SubTrees.Count > root.GetMaxSubtreeCount()) return false;
    //  else for (int i = 0; i < root.SubTrees.Count; i++) {
    //      if (!root.GetAllowedSymbols(i).Select(x => x.Name).Contains(root.SubTrees[i].Symbol.Name)) return false;
    //      if (!IsValidExpression(root.SubTrees[i])) return false;
    //    }
    //  return true;
    //}

    public override IDeepCloneable Clone(Cloner cloner) {
      DefaultSymbolicExpressionGrammar clone = (DefaultSymbolicExpressionGrammar)base.Clone(cloner);
      clone.maxSubTreeCount = new Dictionary<string, int>(maxSubTreeCount);
      clone.minSubTreeCount = new Dictionary<string, int>(minSubTreeCount);
      clone.startSymbol = startSymbol;
      clone.allowedChildSymbols = new Dictionary<string, List<HashSet<string>>>(allowedChildSymbols);
      clone.allSymbols = new HashSet<Symbol>(allSymbols);
      return clone;
    }
  }
}
