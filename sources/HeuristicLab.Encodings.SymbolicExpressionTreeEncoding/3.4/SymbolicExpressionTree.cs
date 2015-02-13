#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  [Item("SymbolicExpressionTree", "Represents a symbolic expression tree.")]
  public class SymbolicExpressionTree : Item, ISymbolicExpressionTree {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }
    [Storable]
    private ISymbolicExpressionTreeNode root;
    public ISymbolicExpressionTreeNode Root {
      get { return root; }
      set {
        if (value == null) throw new ArgumentNullException();
        else if (value != root) {
          root = value;
          OnToStringChanged();
        }
      }
    }

    public int Length {
      get {
        if (root == null)
          return 0;
        return root.GetLength();
      }
    }

    public int Depth {
      get {
        if (root == null)
          return 0;
        return root.GetDepth();
      }
    }

    [StorableConstructor]
    protected SymbolicExpressionTree(bool deserializing) : base(deserializing) { }
    protected SymbolicExpressionTree(SymbolicExpressionTree original, Cloner cloner)
      : base(original, cloner) {
      root = cloner.Clone(original.Root);
    }
    public SymbolicExpressionTree() : base() { }
    public SymbolicExpressionTree(ISymbolicExpressionTreeNode root)
      : base() {
      this.Root = root;
    }

    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesBreadth() {
      if (root == null)
        return Enumerable.Empty<SymbolicExpressionTreeNode>();
      return root.IterateNodesBreadth();
    }

    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPrefix() {
      if (root == null)
        return Enumerable.Empty<SymbolicExpressionTreeNode>();
      return root.IterateNodesPrefix();
    }
    public IEnumerable<ISymbolicExpressionTreeNode> IterateNodesPostfix() {
      if (root == null)
        return Enumerable.Empty<SymbolicExpressionTreeNode>();
      return root.IterateNodesPostfix();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTree(this, cloner);
    }
  }
}
