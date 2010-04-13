#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeNode : ICloneable {
    [Storable]
    private List<SymbolicExpressionTreeNode> subTrees;
    [Storable]
    private Symbol symbol;

    public SymbolicExpressionTreeNode() {
      subTrees = new List<SymbolicExpressionTreeNode>();
    }

    public SymbolicExpressionTreeNode(Symbol symbol)
      : this() {
      this.symbol = symbol;
    }

    // copy constructor
    protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original) {
      symbol = original.symbol;
      subTrees = new List<SymbolicExpressionTreeNode>();
      grammar = original.grammar;
      foreach (var subtree in original.SubTrees) {
        SubTrees.Add((SymbolicExpressionTreeNode)subtree.Clone());
      }
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTreeNode> SubTrees {
      get { return subTrees; }
    }

    public Symbol Symbol {
      get { return symbol; }
      protected set { symbol = value; }
    }

    private ISymbolicExpressionGrammar grammar;
    public virtual ISymbolicExpressionGrammar Grammar {
      get { return grammar; }
      set {
        grammar = value;
        foreach (var subtree in subTrees)
          subtree.Grammar = value;
      }
    }

    public int GetSize() {
      int size = 1;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) size += tree.GetSize();
      return size;
    }

    public int GetHeight() {
      int maxHeight = 0;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) maxHeight = Math.Max(maxHeight, tree.GetHeight());
      return maxHeight + 1;
    }

    public virtual void ResetLocalParameters(IRandom random) { }
    public virtual void ShakeLocalParameters(IRandom random, double shakingFactor) { }

    public virtual void AddSubTree(SymbolicExpressionTreeNode tree) {
      SubTrees.Add(tree);
      //if (tree != null) 
      tree.Grammar = Grammar;
    }

    public virtual void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      SubTrees.Insert(index, tree);
      //if (tree != null) 
      tree.Grammar = Grammar;
    }

    public virtual void RemoveSubTree(int index) {
      //if (SubTrees[index] != null)
      SubTrees[index].Grammar = null;
      SubTrees.RemoveAt(index);
    }

    public IEnumerable<SymbolicExpressionTreeNode> IterateNodesPrefix() {
      yield return this;
      foreach (var subtree in subTrees) {
        foreach (var n in subtree.IterateNodesPrefix())
          yield return n;
      }
    }

    public IEnumerable<SymbolicExpressionTreeNode> IterateNodesPostfix() {
      foreach (var subtree in subTrees) {
        foreach (var n in subtree.IterateNodesPrefix())
          yield return n;
      }
      yield return this;
    }
    //public int GetMinExpressionLength() {
    //  return Grammar.GetMinExpressionLength(Symbol);
    //}
    //public int GetMaxExpressionLength() {
    //  return Grammar.GetMaxExpressionLength(Symbol);
    //}
    //public int GetMinExpressionDepth() {
    //  return Grammar.GetMinExpressionDepth(Symbol);
    //}
    public IEnumerable<Symbol> GetAllowedSymbols(int argumentIndex) {
      return Grammar.Symbols.Where(s => Grammar.IsAllowedChild(Symbol, s, argumentIndex));
    }
    public int GetMinSubtreeCount() {
      return Grammar.GetMinSubtreeCount(Symbol);
    }
    public int GetMaxSubtreeCount() {
      return Grammar.GetMaxSubtreeCount(Symbol);
    }
    //public int GetMaxExpressionLength(Symbol s) {
    //  return Grammar.GetMaxExpressionLength(s);
    //}
    //public int GetMinExpressionLength(Symbol s) {
    //  return Grammar.GetMinExpressionLength(s);
    //}
    //public int GetMinExpressionDepth(Symbol s) {
    //  return Grammar.GetMinExpressionDepth(s);
    //}

    #region ICloneable Members

    public virtual object Clone() {
      return new SymbolicExpressionTreeNode(this);
    }

    #endregion


    public bool IsValidTree() {
      var matchingSymbol = (from symb in Grammar.Symbols
                            where symb.Name == Symbol.Name
                            select symb).SingleOrDefault();

      if (SubTrees.Count < Grammar.GetMinSubtreeCount(matchingSymbol)) return false;
      else if (SubTrees.Count > Grammar.GetMaxSubtreeCount(matchingSymbol)) return false;
      else for (int i = 0; i < SubTrees.Count; i++) {
          if (!GetAllowedSymbols(i).Select(x => x.Name).Contains(SubTrees[i].Symbol.Name)) return false;
          if (!SubTrees[i].IsValidTree()) return false;
        }
      return true;
    }
  }
}
