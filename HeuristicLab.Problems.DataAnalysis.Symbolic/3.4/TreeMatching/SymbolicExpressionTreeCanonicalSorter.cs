
using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public class SymbolicExpressionTreeCanonicalSorter {
    private readonly HashSet<Type> nonSymmetricSymbols = new HashSet<Type> { typeof(Subtraction), typeof(Division) };

    public void SortSubtrees(ISymbolicExpressionTree tree) {
      SortSubtrees(tree.Root);
    }

    public void SortSubtrees(ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount == 0) return;
      var subtrees = node.Subtrees as List<ISymbolicExpressionTreeNode> ?? node.Subtrees.ToList();
      if (IsSymmetric(node.Symbol)) {
        var comparer = new SymbolicExpressionTreeNodeComparer();
        subtrees.Sort(comparer);
      }
      foreach (var s in subtrees)
        SortSubtrees(s);
    }

    private bool IsSymmetric(ISymbol s) {
      return !nonSymmetricSymbols.Contains(s.GetType());
    }
  }
}
