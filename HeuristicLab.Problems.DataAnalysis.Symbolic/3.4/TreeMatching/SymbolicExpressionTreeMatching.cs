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
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
//using HeuristicLab.EvolutionTracking;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class SymbolicExpressionTreeMatching {
    public static bool ContainsSubtree(this ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeEqualityComparer comparer) {
      return FindMatches(root, subtree, comparer).Any();
    }
    public static IEnumerable<ISymbolicExpressionTreeNode> FindMatches(ISymbolicExpressionTree tree, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeEqualityComparer comparer) {
      return FindMatches(tree.Root, subtree, comparer);
    }

    public static IEnumerable<ISymbolicExpressionTreeNode> FindMatches(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeEqualityComparer comp) {
      var fragmentLength = subtree.GetLength();
      // below, we use ">=" for Match(n, subtree, comp) >= fragmentLength because in case of relaxed conditions, 
      // we can have multiple matches of the same node

      return root.IterateNodesBreadth().Where(n => n.GetLength() >= fragmentLength && Match(n, subtree, comp) == fragmentLength);
    }

    ///<summary>
    /// Finds the longest common subsequence in quadratic time and linear space
    /// Variant of:
    /// D. S. Hirschberg. A linear space algorithm for or computing maximal common subsequences. 1975.
    /// http://dl.acm.org/citation.cfm?id=360861
    /// </summary>
    /// <returns>Number of pairs that were matched</returns>
    public static int Match(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b, ISymbolicExpressionTreeNodeSimilarityComparer comp) {
      if (!comp.Equals(a, b)) return 0;
      int m = a.SubtreeCount;
      int n = b.SubtreeCount;
      if (m == 0 || n == 0) return 1;
      var matrix = new int[m + 1, n + 1];
      for (int i = 1; i <= m; ++i) {
        var ai = a.GetSubtree(i - 1);
        for (int j = 1; j <= n; ++j) {
          var bj = b.GetSubtree(j - 1);
          int match = Match(ai, bj, comp);
          matrix[i, j] = Math.Max(Math.Max(matrix[i, j - 1], matrix[i - 1, j]), matrix[i - 1, j - 1] + match);
        }
      }
      return matrix[m, n] + 1;
    }

    /// <summary>
    /// Calculates the difference between two symbolic expression trees.
    /// </summary>
    /// <param name="tree">The first symbolic expression tree</param>
    /// <param name="other">The second symbolic expression tree</param>
    /// <returns>Returns the root of the subtree (from T1) by which T1 differs from T2, or null if no difference is found.</returns>
    public static ISymbolicExpressionTreeNode Difference(this ISymbolicExpressionTree tree, ISymbolicExpressionTree other) {
      return Difference(tree.Root, other.Root);
    }

    public static ISymbolicExpressionTreeNode Difference(this ISymbolicExpressionTreeNode node, ISymbolicExpressionTreeNode other) {
      var a = node.IterateNodesPrefix().ToList();
      var b = other.IterateNodesPrefix().ToList();
      var list = new List<ISymbolicExpressionTreeNode>();
      for (int i = 0, j = 0; i < a.Count && j < b.Count; ++i, ++j) {
        var s1 = a[i].ToString();
        var s2 = b[j].ToString();
        if (s1 == s2) continue;
        list.Add(a[i]);
        // skip subtrees since the parents are already different
        i += a[i].SubtreeCount;
        j += b[j].SubtreeCount;
      }
      ISymbolicExpressionTreeNode result = list.Count > 0 ? LowestCommonAncestor(node, list) : null;
      return result;
    }

    private static ISymbolicExpressionTreeNode LowestCommonAncestor(ISymbolicExpressionTreeNode root, List<ISymbolicExpressionTreeNode> nodes) {
      if (nodes.Count == 0)
        throw new ArgumentException("The nodes list should contain at least one element.");

      if (nodes.Count == 1)
        return nodes[0];

      int minLevel = nodes.Min(x => root.GetBranchLevel(x));

      // bring the nodes in the nodes to the same level (relative to the root)
      for (int i = 0; i < nodes.Count; ++i) {
        var node = nodes[i];
        var level = root.GetBranchLevel(node);
        for (int j = minLevel; j < level; ++j)
          node = node.Parent;
        nodes[i] = node;
      }

      // while not all the elements in the nodes are equal, go one level up
      while (nodes.Any(x => x != nodes[0])) {
        for (int i = 0; i < nodes.Count; ++i)
          nodes[i] = nodes[i].Parent;
      }

      return nodes[0];
    }
  }
}
