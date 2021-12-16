using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class ConstantsToNumberConverter {

    public static ISymbolicExpressionTree Convert(ISymbolicExpressionTree original) {
      var tree = original.Clone() as SymbolicExpressionTree;

      foreach (var constantNode in tree.IterateNodesPostfix().OfType<ConstantTreeNode>()) {
        var parent = constantNode.Parent;
        var index = parent.IndexOfSubtree(constantNode);
        var value = constantNode.Value;
        var newNode = new NumberTreeNode(value);

        parent.RemoveSubtree(index);
        parent.InsertSubtree(index, newNode);
      }

      return tree;
    }
  }
}
