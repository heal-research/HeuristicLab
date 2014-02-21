#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  // adapter class that provides some conversion methods from symbolic expression trees to layout nodes (preserving the tree structure)
  public class SymbolicExpressionTreeLayoutAdapter : ILayoutAdapter<ISymbolicExpressionTreeNode> {
    // default conversion function between ISymbolicExpressionTreeNode and LayoutNode<ISymbolicExpressionTree>
    LayoutNode<ISymbolicExpressionTreeNode> defaultConvert(ISymbolicExpressionTreeNode node) {
      var layoutNode = new LayoutNode<ISymbolicExpressionTreeNode> { Content = node };
      layoutNode.Ancestor = layoutNode;
      return layoutNode;
    }

    public IEnumerable<LayoutNode<ISymbolicExpressionTreeNode>> Convert(ISymbolicExpressionTree tree, Func<ISymbolicExpressionTreeNode, LayoutNode<ISymbolicExpressionTreeNode>> convertFunc = null) {
      return Convert(tree.Root, convertFunc);
    }
    // translate the symbolic expression tree structure to a layout node tree structure
    // return an enumerable containing all the layout nodes
    public IEnumerable<LayoutNode<ISymbolicExpressionTreeNode>> Convert(ISymbolicExpressionTreeNode root, Func<ISymbolicExpressionTreeNode, LayoutNode<ISymbolicExpressionTreeNode>> convertFunc = null) {
      var rootLayoutNode = convertFunc == null ? defaultConvert(root) : convertFunc(root);
      rootLayoutNode.Ancestor = rootLayoutNode;

      if (root.SubtreeCount > 0) {
        rootLayoutNode.Children = new List<LayoutNode<ISymbolicExpressionTreeNode>>(root.SubtreeCount);
        Expand(rootLayoutNode, convertFunc);
      }

      var list = new List<LayoutNode<ISymbolicExpressionTreeNode>> { rootLayoutNode };
      int i = 0;
      while (i < list.Count) {
        if (list[i].Children != null) list.AddRange(list[i].Children);
        ++i;
      }
      return list;
    }

    private void Expand(LayoutNode<ISymbolicExpressionTreeNode> layoutNode, Func<ISymbolicExpressionTreeNode, LayoutNode<ISymbolicExpressionTreeNode>> convertFunc = null) {
      if (layoutNode.Children == null) return;
      for (int i = 0; i < layoutNode.Content.SubtreeCount; ++i) {
        var subtree = layoutNode.Content.GetSubtree(i);
        var childLayoutNode = convertFunc == null ? defaultConvert(subtree) : convertFunc(subtree);
        childLayoutNode.Parent = layoutNode;
        childLayoutNode.Number = i;
        childLayoutNode.Level = layoutNode.Level + 1;
        childLayoutNode.Ancestor = childLayoutNode;
        childLayoutNode.Children = subtree.SubtreeCount > 0 ? new List<LayoutNode<ISymbolicExpressionTreeNode>>(subtree.SubtreeCount) : null;
        layoutNode.Children.Add(childLayoutNode);
        Expand(childLayoutNode, convertFunc);
      }
    }
  }
}
