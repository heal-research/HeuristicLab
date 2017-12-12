using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public interface ISymbolicExpressionTreeNodeSimilarityComparer : IEqualityComparer<ISymbolicExpressionTreeNode>, IItem {
    bool MatchConstantValues { get; set; }
    bool MatchVariableWeights { get; set; }
    bool MatchVariableNames { get; set; }
  }
}