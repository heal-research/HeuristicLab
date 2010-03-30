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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeNode : DeepCloneable {
    private List<SymbolicExpressionTreeNode> subTrees;
    private Symbol symbol;

    public SymbolicExpressionTreeNode() {
    }

    public SymbolicExpressionTreeNode(Symbol symbol) {
      subTrees = new List<SymbolicExpressionTreeNode>();
      this.symbol = symbol;
    }

    //protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original) {
    //  this.symbol = original.Symbol;
    //  this.subTrees = new List<SymbolicExpressionTreeNode>(original.SubTrees.Count);
    //  foreach (SymbolicExpressionTreeNode originalSubTree in original.SubTrees) {
    //    this.SubTrees.Add((SymbolicExpressionTreeNode)originalSubTree.Clone());
    //  }
    //}

    internal virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTreeNode> SubTrees {
      get { return subTrees; }
    }

    public Symbol Symbol {
      get { return symbol; }
      protected set { symbol = value; }
    }

    internal int GetSize() {
      int size = 1;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) size += tree.GetSize();
      return size;
    }

    internal int GetHeight() {
      int maxHeight = 0;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) maxHeight = Math.Max(maxHeight, tree.GetHeight());
      return maxHeight + 1;
    }

    //public virtual IOperation CreateShakingOperation(IScope scope) {
    //  return null;
    //}

    //public virtual IOperation CreateInitOperation(IScope scope) {
    //  return null;
    //}

    protected internal virtual void AddSubTree(SymbolicExpressionTreeNode tree) {
      SubTrees.Add(tree);
    }

    protected internal virtual void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      SubTrees.Insert(index, tree);
    }

    protected internal virtual void RemoveSubTree(int index) {
      SubTrees.RemoveAt(index);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicExpressionTreeNode clone = new SymbolicExpressionTreeNode(symbol);
      cloner.RegisterClonedObject(this, clone);
      foreach (var subtree in SubTrees) {
        clone.AddSubTree((SymbolicExpressionTreeNode)subtree.Clone(cloner));
      }
      return clone;
    }
  }
}
