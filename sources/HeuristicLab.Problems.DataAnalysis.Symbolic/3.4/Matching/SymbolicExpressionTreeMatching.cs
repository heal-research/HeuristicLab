using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
//using HeuristicLab.EvolutionTracking;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class SymbolicExpressionTreeMatching {
    public static bool ContainsSubtree(this ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeSimilarityComparer comparer) {
      return FindMatches(root, subtree, comparer).Any();
    }
    public static IEnumerable<ISymbolicExpressionTreeNode> FindMatches(ISymbolicExpressionTree tree, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeSimilarityComparer comparer) {
      return FindMatches(tree.Root, subtree, comparer);
    }

    public static IEnumerable<ISymbolicExpressionTreeNode> FindMatches(ISymbolicExpressionTreeNode root, ISymbolicExpressionTreeNode subtree, SymbolicExpressionTreeNodeSimilarityComparer comp) {
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
    public static int Match(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b, SymbolicExpressionTreeNodeSimilarityComparer comp) {
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
  }
}
