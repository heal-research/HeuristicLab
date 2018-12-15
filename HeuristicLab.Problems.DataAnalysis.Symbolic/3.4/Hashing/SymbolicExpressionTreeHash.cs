#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

    public static ulong ComputeHash(this ISymbolicExpressionTree tree) {
      return ComputeHash(tree.Root.GetSubtree(0).GetSubtree(0));
    }

    public static double ComputeSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2, bool simplify = false) {
      return ComputeSimilarity(t1.Root.GetSubtree(0).GetSubtree(0), t2.Root.GetSubtree(0).GetSubtree(0), simplify);
    }

    public static double ComputeSimilarity(ISymbolicExpressionTreeNode t1, ISymbolicExpressionTreeNode t2, bool simplify = false) {
      HashNode<ISymbolicExpressionTreeNode>[] lhs;
      HashNode<ISymbolicExpressionTreeNode>[] rhs;

      ulong hashFunction(byte[] input) => HashUtil.DJBHash(input);

      if (simplify) {
        lhs = t1.MakeNodes().Simplify(hashFunction);
        rhs = t2.MakeNodes().Simplify(hashFunction);
      } else {
        lhs = t1.MakeNodes().Sort(hashFunction); // sort calculates hash values
        rhs = t2.MakeNodes().Sort(hashFunction);
      }

      var lh = lhs.Select(x => x.CalculatedHashValue).ToArray();
      var rh = rhs.Select(x => x.CalculatedHashValue).ToArray();

      Array.Sort(lh);
      Array.Sort(rh);

      return ComputeSimilarity(lh, rh);
    }

    // this will only work if lh and rh are sorted
    private static double ComputeSimilarity(ulong[] lh, ulong[] rh) {
      double count = 0;
      for (int i = 0, j = 0; i < lh.Length && j < rh.Length;) {
        var h1 = lh[i];
        var h2 = rh[j];
        if (h1 == h2) {
          ++count;
          ++i;
          ++j;
        } else if (h1 < h2) {
          ++i;
        } else if (h1 > h2) {
          ++j;
        }
      }

      return 2d * count / (lh.Length + rh.Length);
    }

    public static double ComputeAverageSimilarity(IList<ISymbolicExpressionTree> trees, bool simplify = false, bool strict = false) {
      var total = (double)trees.Count * (trees.Count - 1) / 2;
      double avg = 0;
      var hashes = new ulong[trees.Count][];
      // build hash arrays
      for (int i = 0; i < trees.Count; ++i) {
        var nodes = trees[i].MakeNodes(strict);
        hashes[i] = (simplify ? nodes.Simplify(HashUtil.DJBHash) : nodes.Sort(HashUtil.DJBHash)).Select(x => x.CalculatedHashValue).ToArray();
        Array.Sort(hashes[i]);
      }
      // compute similarity matrix
      for (int i = 0; i < trees.Count - 1; ++i) {
        for (int j = i + 1; j < trees.Count; ++j) {
          avg += ComputeSimilarity(hashes[i], hashes[j]);
        }
      }
      return avg / total;
    }

    public static double[,] ComputeSimilarityMatrix(IList<ISymbolicExpressionTree> trees, bool simplify = false, bool strict = false) {
      var sim = new double[trees.Count, trees.Count];
      var hashes = new ulong[trees.Count][];
      // build hash arrays
      for (int i = 0; i < trees.Count; ++i) {
        var nodes = trees[i].MakeNodes(strict);
        hashes[i] = (simplify ? nodes.Simplify(HashUtil.DJBHash) : nodes.Sort(HashUtil.DJBHash)).Select(x => x.CalculatedHashValue).ToArray();
        Array.Sort(hashes[i]);
      }
      // compute similarity matrix
      for (int i = 0; i < trees.Count - 1; ++i) {
        for (int j = i + 1; j < trees.Count; ++j) {
          sim[i, j] = sim[j, i] = ComputeSimilarity(hashes[i], hashes[j]);
        }
      }
      return sim;
    }

    public static ulong ComputeHash(this ISymbolicExpressionTreeNode treeNode, bool strict = false) {
      ulong hashFunction(byte[] input) => HashUtil.JSHash(input);
      var hashNodes = treeNode.MakeNodes(strict);
      var simplified = hashNodes.Simplify(hashFunction);
      return simplified.Last().CalculatedHashValue;
    }

    public static HashNode<ISymbolicExpressionTreeNode> ToHashNode(this ISymbolicExpressionTreeNode node, bool strict = false) {
      var symbol = node.Symbol;
      var name = symbol.Name;
      if (node is ConstantTreeNode constantNode) {
        name = strict ? constantNode.Value.ToString() : symbol.Name;
      } else if (node is VariableTreeNode variableNode) {
        name = strict ? variableNode.Weight.ToString() + variableNode.VariableName : variableNode.VariableName;
      }
      var hash = (ulong)name.GetHashCode();
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

    public static HashNode<ISymbolicExpressionTreeNode>[] MakeNodes(this ISymbolicExpressionTree tree, bool strict = false) {
      return MakeNodes(tree.Root.GetSubtree(0).GetSubtree(0), strict);
    }

    public static HashNode<ISymbolicExpressionTreeNode>[] MakeNodes(this ISymbolicExpressionTreeNode node, bool strict = false) {
      return node.IterateNodesPostfix().Select(x => x.ToHashNode(strict)).ToArray().UpdateNodeSizes();
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

        if (node.IsLeaf) {
          if (node.Data is VariableTreeNode variable) {
            var variableTreeNode = (VariableTreeNode)treeNodes[i];
            variableTreeNode.VariableName = variable.VariableName;
            variableTreeNode.Weight = variable.Weight;
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
      ulong hashFunction(byte[] bytes) => HashUtil.JSHash(bytes);
      var root = tree.Root.GetSubtree(0).GetSubtree(0);
      var nodes = root.MakeNodes();
      var simplified = nodes.Simplify(hashFunction);
      return simplified.ToTree();
    }

    public static void SimplifyAddition(ref HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      // simplify additions of terms by eliminating terms with the same symbol and hash
      var children = nodes.IterateChildren(i);

      // we always assume the child nodes are sorted
      var curr = children[0];
      var node = nodes[i];

      foreach (var j in children.Skip(1)) {
        if (nodes[j] == nodes[curr]) {
          nodes.SetEnabled(j, false);
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
    public static void SimplifyMultiplication(ref HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
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

    public static void SimplifyDivision(ref HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      var node = nodes[i];
      var children = nodes.IterateChildren(i);

      var tmp = nodes;

      if (children.All(x => tmp[x].Data.Symbol is Constant)) {
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

    public static void SimplifyUnaryNode(ref HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
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

    public static void SimplifyBinaryNode(ref HashNode<ISymbolicExpressionTreeNode>[] nodes, int i) {
      var children = nodes.IterateChildren(i);
      var tmp = nodes;
      if (children.All(x => tmp[x].Data.Symbol is Constant)) {
        foreach (var j in children) {
          nodes[j].Enabled = false;
        }
        nodes[i] = constant.CreateTreeNode().ToHashNode();
      }
    }
    #endregion
  }
}
