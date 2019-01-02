using System;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("535830d4-551e-4b53-97e3-9605bd7e785f")]
  public class SymbolicExpressionTreeEqualityComparer : IEqualityComparer<ISymbolicExpressionTree> {
    [Storable]
    public SymbolicExpressionTreeNodeEqualityComparer SimilarityComparer { get; set; }

    public bool Equals(ISymbolicExpressionTree a, ISymbolicExpressionTree b) {
      if (SimilarityComparer == null) throw new Exception("SimilarityComparer needs to be initialized first.");
      return a.Length == b.Length && SymbolicExpressionTreeMatching.Match(a.Root, b.Root, SimilarityComparer) == Math.Min(a.Length, b.Length);
    }

    public int GetHashCode(ISymbolicExpressionTree tree) {
      return (tree.Length << 8) % 12345;
    }
  }
}
