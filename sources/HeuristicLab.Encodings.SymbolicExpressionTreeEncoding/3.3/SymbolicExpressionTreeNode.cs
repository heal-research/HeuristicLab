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
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Diagnostics;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeNode : ICloneable {
    [Storable]
    private List<SymbolicExpressionTreeNode> subTrees;
    [Storable]
    private Symbol symbol;
    public Symbol Symbol {
      get { return symbol; }
      protected set { symbol = value; }
    }

    // parent relation is not persisted or cloned (will be set on AddSubtree or RemoveSubtree)
    private SymbolicExpressionTreeNode parent;
    internal SymbolicExpressionTreeNode Parent {
      get { return parent; }
      set { parent = value; }
    }

    public SymbolicExpressionTreeNode() { }

    public SymbolicExpressionTreeNode(Symbol symbol) {
      subTrees = new List<SymbolicExpressionTreeNode>();
      this.symbol = symbol;
    }

    // copy constructor
    protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original) {
      symbol = original.symbol;
      subTrees = new List<SymbolicExpressionTreeNode>();
      foreach (var subtree in original.SubTrees) {
        AddSubTree((SymbolicExpressionTreeNode)subtree.Clone());
      }
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTreeNode> SubTrees {
      get { return subTrees; }
    }

    internal virtual ISymbolicExpressionGrammar Grammar {
      get { return parent.Grammar; }
      set { throw new NotSupportedException("Grammar can be set only for SymbolicExpressionTreeTopLevelNodes."); }
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
      subTrees.Add(tree);
      tree.Parent = this;
    }

    public virtual void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      subTrees.Insert(index, tree);
      tree.Parent = this;
    }

    public virtual void RemoveSubTree(int index) {
      subTrees[index].Parent = null;
      subTrees.RemoveAt(index);
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
    public IEnumerable<Symbol> GetAllowedSymbols(int argumentIndex) {
      return Grammar.Symbols.Where(s => Grammar.IsAllowedChild(Symbol, s, argumentIndex));
    }
    public int GetMinSubtreeCount() {
      return Grammar.GetMinSubtreeCount(Symbol);
    }
    public int GetMaxSubtreeCount() {
      return Grammar.GetMaxSubtreeCount(Symbol);
    }

    #region ICloneable Members

    public virtual object Clone() {
      return new SymbolicExpressionTreeNode(this);
    }

    #endregion

    public override string ToString() {
      return Symbol.Name;
    }

  }
}
