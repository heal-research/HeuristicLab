using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using static HeuristicLab.Problems.DataAnalysis.Symbolic.SymbolicExpressionHashExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public static class SymbolicExpressionTreeHash {
    private static readonly Addition add = new Addition();
    private static readonly Subtraction sub = new Subtraction();
    private static readonly Multiplication mul = new Multiplication();
    private static readonly Division div = new Division();
    private static readonly Logarithm log = new Logarithm();
    private static readonly Exponential exp = new Exponential();
    private static readonly Sine sin = new Sine();
    private static readonly Cosine cos = new Cosine();
    private static readonly Constant constant = new Constant();

    private static readonly ISymbolicExpressionTreeNodeComparer comparer = new SymbolicExpressionTreeNodeComparer();

    public static int ComputeHash(this ISymbolicExpressionTree tree) {
      return ComputeHash(tree.Root.GetSubtree(0).GetSubtree(0));
    }

    public static Dictionary<ISymbolicExpressionTreeNode, int> ComputeNodeHashes(this ISymbolicExpressionTree tree) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var nodes = root.MakeNodes();
      nodes.UpdateNodeSizes();

      for (int i = 0; i < nodes.Length; ++i) {
        if (nodes[i].IsChild)
          continue;
        nodes[i].CalculatedHashValue = nodes.ComputeHash(i);
      }
      return nodes.ToDictionary(x => x.Data, x => x.CalculatedHashValue);
    }

    public static int ComputeHash(this ISymbolicExpressionTreeNode treeNode) {
      var hashNodes = treeNode.MakeNodes();
      var simplified = hashNodes.Simplify();
      return ComputeHash(simplified);
    }

    public static int ComputeHash(this HashNode<ISymbolicExpressionTreeNode>[] nodes) {
      int hash = 1315423911;
      foreach (var node in nodes)
        hash ^= (hash << 5) + node.CalculatedHashValue + (hash >> 2);
      return hash;
    }

    public static HashNode<ISymbolicExpressionTreeNode> ToHashNode(this ISymbolicExpressionTreeNode node) {
      var symbol = node.Symbol;
      var name = symbol.Name;
      if (symbol is Variable) {
        var variableTreeNode = (VariableTreeNode)node;
        name = variableTreeNode.VariableName;
      }
      var hash = name.GetHashCode();
      var hashNode = new HashNode<ISymbolicExpressionTreeNode>(comparer) {
        Data = node,
        Arity = node.SubtreeCount,
        Size = node.SubtreeCount,
        IsCommutative = node.Symbol is Addition || node.Symbol is Multiplication,
        Enabled = true,
        HashValue = hash,
        CalculatedHashValue = hash
      };
      if (symbol is Addition) {
        hashNode.Simplify = SimplifyAddition;
      } else if (symbol is Multiplication) {
        hashNode.Simplify = SimplifyMultiplication;
      } else if (symbol is Division) {
        hashNode.Simplify = SimplifyDivision;
      } else if (symbol is Logarithm || symbol is Exponential || symbol is Sine || symbol is Cosine) {
        hashNode.Simplify = SimplifyUnaryNode;
      } else if (symbol is Subtraction) {
        hashNode.Simplify = SimplifyBinaryNode;
      }
      return hashNode;
    }

    public static HashNode<ISymbolicExpressionTreeNode>[] MakeNodes(this ISymbolicExpressionTreeNode node) {
      return node.IterateNodesPostfix().Select(ToHashNode).ToArray();
    }

    #region parse a nodes array back into a tree
    public static ISymbolicExpressionTree ToTree(this HashNode<ISymbolicExpressionTreeNode>[] nodes) {
      var root = new ProgramRootSymbol().CreateTreeNode();
      var start = new StartSymbol().CreateTreeNode();
      root.AddSubtree(start);
      start.AddSubtree(nodes.ToSubtree());
      return new SymbolicExpressionTree(root);
    }

    public static ISymbolicExpressionTreeNode ToSubtree(this HashNode<ISymbolicExpressionTreeNode>[] nodes) {
      var treeNodes = nodes.Select(x => x.Data.Symbol.CreateTreeNode()).ToArray();

      for (int i = nodes.Length - 1; i >= 0; --i) {
        var node = nodes[i];

        if (node.IsChild) {
          if (node.Data is VariableTreeNode variable) {
            var variableTreeNode = (VariableTreeNode)treeNodes[i];
            variableTreeNode.VariableName = variable.VariableName;
            variableTreeNode.Weight = 1;
          } else if (node.Data is ConstantTreeNode @const) {
            var constantTreeNode = (ConstantTreeNode)treeNodes[i];
            constantTreeNode.Value = @const.Value;
          }
          continue;
        }

        var treeNode = treeNodes[i];

        foreach (var j in nodes.IterateChildren(i)) {
          treeNode.AddSubtree(treeNodes[j]);
        }
      }

      return treeNodes.Last();
    }

    private static T CreateTreeNode<T>(this ISymbol symbol) where T : class, ISymbolicExpressionTreeNode {
      return (T)symbol.CreateTreeNode();
    }
    #endregion

    #region tree simplification
    // these simplification methods rely on the assumption that child nodes of the current node have already been simplified
    // (in other words simplification should be applied in a bottom-up fashion) 
    public static ISymbolicExpressionTree Simplify(ISymbolicExpressionTree tree) {
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var nodes = root.MakeNodes();
      var simplified = nodes.Simplify();
      return simplified.ToTree();
    }

    public static void SimplifyAddition(HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      // simplify additions of terms by eliminating terms with the same symbol and hash
      var children = nodes.IterateChildren(i);

      var curr = children[0];
      var node = nodes[i];

      foreach (var j in children.Skip(1)) {
        if (nodes[j] == nodes[curr]) {
          for (int k = j - nodes[j].Size; k <= j; ++k) {
            nodes[k].Enabled = false;
          }
          node.Arity--;
        } else {
          curr = j;
        }
      }
      if (node.Arity == 1) { // if the arity is 1 we don't need the addition node at all
        node.Enabled = false;
      }
    }

    // simplify multiplications by reducing constants and div terms  
    public static void SimplifyMultiplication(HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      var node = nodes[i];
      var children = nodes.IterateChildren(i);

      for (int j = 0; j < children.Length; ++j) {
        var c = children[j];
        var child = nodes[c];

        if (!child.Enabled)
          continue;

        var symbol = child.Data.Symbol;
        if (symbol is Constant) {
          for (int k = j + 1; k < children.Length; ++k) {
            var d = children[k];
            if (nodes[d].Data.Symbol is Constant) {
              ((ConstantTreeNode)child.Data).Value *= ((ConstantTreeNode)nodes[d].Data).Value;
              nodes[d].Enabled = false;
              node.Arity--;
            } else {
              break;
            }
          }
        } else if (symbol is Division) {
          var div = nodes[c];
          var denominator =
            div.Arity == 1 ?
            nodes[c - 1] :                    // 1 / x is expressed as div(x) (with a single child)
            nodes[c - nodes[c - 1].Size - 2]; // assume division always has arity 1 or 2

          foreach (var d in children) {
            if (nodes[d].Enabled && nodes[d] == denominator) {
              nodes[c].Enabled = nodes[d].Enabled = denominator.Enabled = false;
              node.Arity -= 2; // matching child + division node
              break;
            }
          }
        }

        if (node.Arity == 0) { // if everything is simplified this node becomes constant
          var constantTreeNode = constant.CreateTreeNode<ConstantTreeNode>();
          constantTreeNode.Value = 1;
          nodes[i] = constantTreeNode.ToHashNode();
        } else if (node.Arity == 1) { // when i have only 1 arg left i can skip this node
          node.Enabled = false;
        }
      }
    }

    public static void SimplifyDivision(HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      var node = nodes[i];
      var children = nodes.IterateChildren(i);

      if (children.All(x => nodes[x].Data.Symbol is Constant)) {
        var v = ((ConstantTreeNode)nodes[children.First()].Data).Value;
        if (node.Arity == 1) {
          v = 1 / v;
        } else if (node.Arity > 1) {
          foreach (var j in children.Skip(1)) {
            v /= ((ConstantTreeNode)nodes[j].Data).Value;
          }
        }
        var constantTreeNode = constant.CreateTreeNode<ConstantTreeNode>();
        constantTreeNode.Value = v;
        nodes[i] = constantTreeNode.ToHashNode();
        return;
      }

      var nominator = nodes[children[0]];
      foreach (var j in children.Skip(1)) {
        var denominator = nodes[j];
        if (nominator == denominator) {
          // disable all the children of the division node (nominator and children + denominator and children)
          nominator.Enabled = denominator.Enabled = false;
          node.Arity -= 2; // nominator + denominator
        }
        if (node.Arity == 0) {
          var constantTreeNode = constant.CreateTreeNode<ConstantTreeNode>();
          constantTreeNode.Value = 1; // x / x = 1
          nodes[i] = constantTreeNode.ToHashNode();
        }
      }
    }

    public static void SimplifyUnaryNode(HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      // check if the child of the unary node is a constant, then the whole node can be simplified
      var parent = nodes[i];
      var child = nodes[i - 1];

      var parentSymbol = parent.Data.Symbol;
      var childSymbol = child.Data.Symbol;

      if (childSymbol is Constant) {
        nodes[i].Enabled = false;
      } else if ((parentSymbol is Exponential && childSymbol is Logarithm) || (parentSymbol is Logarithm && childSymbol is Exponential)) {
        child.Enabled = parent.Enabled = false;
      }
    }

    public static void SimplifyBinaryNode(HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      var children = nodes.IterateChildren(i);
      if (children.All(x => nodes[x].Data.Symbol is Constant)) {
        foreach (var j in children) {
          nodes[j].Enabled = false;
        }
        nodes[i] = constant.CreateTreeNode().ToHashNode();
      }
    }
    #endregion
  }
}
