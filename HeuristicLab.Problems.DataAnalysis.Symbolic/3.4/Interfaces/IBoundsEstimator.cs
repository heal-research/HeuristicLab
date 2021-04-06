using System.Collections.Generic;
using HEAL.Attic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("C94A360D-5A9F-48A3-A6D3-CF920C77E50D")]
  public interface IBoundsEstimator : INamedItem, IStatefulItem {
    Interval GetModelBound(ISymbolicExpressionTree tree, IntervalCollection variableRanges);

    IDictionary<ISymbolicExpressionTreeNode, Interval> GetModelNodeBounds(
      ISymbolicExpressionTree tree, IntervalCollection variableRanges);

    // returns the size of the violation which is the distance to one of the bounds
    double GetConstraintViolation(
      ISymbolicExpressionTree tree, IntervalCollection variableRanges, ShapeConstraint constraint);

    bool IsCompatible(ISymbolicExpressionTree tree);

    int EvaluatedSolutions { get; set; }
  }
}