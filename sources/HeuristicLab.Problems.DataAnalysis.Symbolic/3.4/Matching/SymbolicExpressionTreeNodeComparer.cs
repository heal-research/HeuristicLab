using System;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  // this comparer considers that a < b if the type of a is "greater" than the type of b, for example:
  // - A function node is "greater" than a terminal node
  // - A variable terminal is "greater" than a constant terminal
  // - used for bringing subtrees to a "canonical" form when the operation allows reordering of arguments
  public class SymbolicExpressionTreeNodeComparer : ISymbolicExpressionTreeNodeComparer {
    public int Compare(ISymbolicExpressionTreeNode a, ISymbolicExpressionTreeNode b) {
      if (!(a is SymbolicExpressionTreeTerminalNode)) {
        return b is SymbolicExpressionTreeTerminalNode
          ? -1
          : string.Compare(a.Symbol.Name, b.Symbol.Name, StringComparison.Ordinal);
      }
      if (!(b is SymbolicExpressionTreeTerminalNode)) return 1;
      // at this point we know a and b are terminal nodes
      var va = a as VariableTreeNode;
      if (va != null) {
        if (b is ConstantTreeNode) return -1;
        var vb = (VariableTreeNode)b;
        return (va.VariableName.Equals(vb.VariableName)
          ? va.Weight.CompareTo(vb.Weight)
          : string.Compare(va.VariableName, vb.VariableName, StringComparison.Ordinal));
      }
      // at this point we know for sure that a is a constant tree node
      if (b is VariableTreeNode) return 1;
      var ca = (ConstantTreeNode)a;
      var cb = (ConstantTreeNode)b;
      return ca.Value.CompareTo(cb.Value);
    }
  }
}
