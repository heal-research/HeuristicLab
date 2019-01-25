using System.Collections.Generic;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("c4de8ab2-616d-4c73-9f9c-59545ac38199")]
  public interface ISymbolicExpressionTreeNodeSimilarityComparer : IEqualityComparer<ISymbolicExpressionTreeNode>, IItem {
    bool MatchConstantValues { get; set; }
    bool MatchVariableWeights { get; set; }
    bool MatchVariableNames { get; set; }
  }
}