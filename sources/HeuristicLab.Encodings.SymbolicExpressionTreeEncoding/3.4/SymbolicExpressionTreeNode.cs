#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeNode : DeepCloneable, ISymbolicExpressionTreeNode {
    [Storable]
    private IList<ISymbolicExpressionTreeNode> subtrees;
    [Storable]
    private ISymbol symbol;

    // cached values to prevent unnecessary tree iterations
    private ushort length;
    private ushort depth;

    public ISymbol Symbol {
      get { return symbol; }
      protected set { symbol = value; }
    }

    // parent relation is not persisted or cloned (will be set on AddSubtree or RemoveSubtree)
    [Storable]
    private ISymbolicExpressionTreeNode parent;
    public ISymbolicExpressionTreeNode Parent {
      get { return parent; }
      set { parent = value; }
    }

    [StorableConstructor]
    protected SymbolicExpressionTreeNode(bool deserializing) { }
    protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original, Cloner cloner)
      : base(original, cloner) {
      symbol = original.symbol; // symbols are reused
      length = original.length;
      depth = original.depth;
      if (original.subtrees != null) {
        subtrees = new List<ISymbolicExpressionTreeNode>(original.subtrees.Count);
        foreach (var subtree in original.subtrees) {
          var clonedSubtree = cloner.Clone(subtree);
          subtrees.Add(clonedSubtree);
          clonedSubtree.Parent = this;
        }
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

    public SymbolicExpressionTreeNode(ISymbol symbol)
      : base() {
      subtrees = new List<ISymbolicExpressionTreeNode>(3);
      this.symbol = symbol;
    }


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (subtrees != null) {
        foreach (var subtree in subtrees)
          if (subtree.Parent != null)
            subtree.Parent = this;
      }
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IEnumerable<ISymbolicExpressionTreeNode> Subtrees {
      get { return subtrees; }
    }

    public virtual ISymbolicExpressionTreeGrammar Grammar {
      get { return parent.Grammar; }
    }

    public int GetLength() {
      if (length > 0) return length;
      else {
        ushort l = 1;
        if (subtrees != null) {
          for (int i = 0; i < subtrees.Count; i++) {
            checked { l += (ushort)subtrees[i].GetLength(); }
          }
        }
        length = l;
        return length;
      }
    }

    public int GetDepth() {
      if (depth > 0) return depth;
      else {
        ushort d = 0;
        if (subtrees != null) {
          for (int i = 0; i < subtrees.Count; i++) d = Math.Max(d, (ushort)subtrees[i].GetDepth());
        }
        d++;
        depth = d;
        return depth;
      }
    }

    public int GetBranchLevel(ISymbolicExpressionTreeNode child) {
      return GetBranchLevel(this, child);
    }

    private static int GetBranchLevel(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode point) {
      if (root == point)
        return 0;
      foreach (var subtree in root.Subtrees) {
        int branchLevel = GetBranchLevel(subtree, point);
        if (branchLevel < int.MaxValue)
          return 1 + branchLevel;
      }
      return int.MaxValue;
    }

    public virtual void ResetLocalParameters(IRandom random) { }
    public virtual void ShakeLocalParameters(IRandom random, double shakingFactor) { }

    public int SubtreeCount {
      get {
        if (subtrees == null) return 0;
        return subtrees.Count;
      }
    }

    public virtual ISymbolicExpressionTreeNode GetSubtree(int index) {
      return subtrees[index];
    }
    public virtual int IndexOfSubtree(ISymbolicExpressionTreeNode tree) {
      return subtrees.IndexOf(tree);
    }
    public virtual void AddSubtree(ISymbolicExpressionTreeNode tree) {
      subtrees.Add(tree);
      tree.Parent = this;
      ResetCachedValues();
    }
    public virtual void InsertSubtree(int index, ISymbolicExpressionTreeNode tree) {
      subtrees.Insert(index, tree);
      tree.Parent = this;
      ResetCachedValues();
    }
    public virtual void RemoveSubtree(int index) {
      subtrees[index].Parent = null;
      subtrees.RemoveAt(index);
      ResetCachedValues();
    }

    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth() {
      var list = new List<ISymbolicExpressionTreeNode>(GetLength()) { this };
      int i = 0;
      while (i != list.Count) {
        for (int j = 0; j != list[i].SubtreeCount; ++j)
          list.Add(list[i].GetSubtree(j));
        ++i;
      }
      return list;
    }

    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix() {
      List<ISymbolicExpressionTreeNode> list = new List<ISymbolicExpressionTreeNode>();
      ForEachNodePrefix((n) => list.Add(n));
      return list;
    }

    public void ForEachNodePrefix(Action<ISymbolicExpressionTreeNode> a) {
      a(this);
      if (subtrees != null) {
        //avoid linq to reduce memory pressure
        for (int i = 0; i < subtrees.Count; i++) {
          subtrees[i].ForEachNodePrefix(a);
        }
      }
    }

    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix() {
      List<ISymbolicExpressionTreeNode> list = new List<ISymbolicExpressionTreeNode>();
      ForEachNodePostfix((n) => list.Add(n));
      return list;
    }

    public void ForEachNodePostfix(Action<ISymbolicExpressionTreeNode> a) {
      if (subtrees != null) {
        //avoid linq to reduce memory pressure
        for (int i = 0; i < subtrees.Count; i++) {
          subtrees[i].ForEachNodePostfix(a);
        }
      }
      a(this);
    }

    public override string ToString() {
      if (Symbol != null) return Symbol.Name;
      return "SymbolicExpressionTreeNode";
    }

    private void ResetCachedValues() {
      length = 0; depth = 0;
      SymbolicExpressionTreeNode parentNode = parent as SymbolicExpressionTreeNode;
      if (parentNode != null) parentNode.ResetCachedValues();
    }
  }
}
