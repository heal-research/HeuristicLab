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
  [StorableClass]
  public class SymbolicExpressionTreeNode : DeepCloneable {
    [Storable]
    private IList<SymbolicExpressionTreeNode> subTrees;
    [Storable]
    private Symbol symbol;

    // cached values to prevent unnecessary tree iterations
    private ushort size;
    private ushort height;

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

    [StorableConstructor]
    protected SymbolicExpressionTreeNode(bool deserializing) { }
    protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original, Cloner cloner)
      : base() {
      symbol = original.symbol; // symbols are reused
      subTrees = new List<SymbolicExpressionTreeNode>(original.SubTrees.Count);
      foreach (var subtree in original.SubTrees) {
        var clonedSubTree = cloner.Clone(subtree);
        subTrees.Add(clonedSubTree);
        clonedSubTree.Parent = this;
      }
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeNode(this, cloner);
    }

    internal SymbolicExpressionTreeNode()
      : base() {
      // don't allocate subtrees list here!
      // because we don't want to allocate it in terminal nodes
    }

    public SymbolicExpressionTreeNode(Symbol symbol)
      : base() {
      subTrees = new List<SymbolicExpressionTreeNode>(3);
      this.symbol = symbol;
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var subtree in SubTrees) {
        subtree.Parent = this;
      }
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTreeNode> SubTrees {
      get { return subTrees; }
    }

    public virtual ISymbolicExpressionGrammar Grammar {
      get { return parent.Grammar; }
    }

    public int GetSize() {
      if (size > 0) return size;
      else {
        size = 1;
        if (SubTrees != null) {
          for (int i = 0; i < SubTrees.Count; i++) {
            checked { size += (ushort)SubTrees[i].GetSize(); }
          }
        }
        return size;
      }
    }

    public int GetHeight() {
      if (height > 0) return height;
      else {
        if (SubTrees != null) {
          for (int i = 0; i < SubTrees.Count; i++) height = Math.Max(height, (ushort)SubTrees[i].GetHeight());
        }
        height++;
        return height;
      }
    }

    public virtual void ResetLocalParameters(IRandom random) { }
    public virtual void ShakeLocalParameters(IRandom random, double shakingFactor) { }

    public virtual void AddSubTree(SymbolicExpressionTreeNode tree) {
      SubTrees.Add(tree);
      tree.Parent = this;
      ResetCachedValues();
    }

    public virtual void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      SubTrees.Insert(index, tree);
      tree.Parent = this;
      ResetCachedValues();
    }

    public virtual void RemoveSubTree(int index) {
      SubTrees[index].Parent = null;
      SubTrees.RemoveAt(index);
      ResetCachedValues();
    }

    public IEnumerable<SymbolicExpressionTreeNode> IterateNodesPrefix() {
      List<SymbolicExpressionTreeNode> list = new List<SymbolicExpressionTreeNode>();
      ForEachNodePrefix((n) => list.Add(n));
      return list;
    }

    public void ForEachNodePrefix(Action<SymbolicExpressionTreeNode> a) {
      a(this);
      if (SubTrees != null) {
        for (int i = 0; i < SubTrees.Count; i++) {
          SubTrees[i].ForEachNodePrefix(a);
        }
      }
    }

    public IEnumerable<SymbolicExpressionTreeNode> IterateNodesPostfix() {
      List<SymbolicExpressionTreeNode> list = new List<SymbolicExpressionTreeNode>();
      ForEachNodePostfix((n) => list.Add(n));
      return list;
    }

    public void ForEachNodePostfix(Action<SymbolicExpressionTreeNode> a) {
      if (SubTrees != null) {
        for (int i = 0; i < SubTrees.Count; i++) {
          SubTrees[i].ForEachNodePostfix(a);
        }
      }
      a(this);
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
    public override string ToString() {
      return Symbol.Name;
    }

    private void ResetCachedValues() {
      size = 0; height = 0;
      if (parent != null) parent.ResetCachedValues();
    }
  }
}
