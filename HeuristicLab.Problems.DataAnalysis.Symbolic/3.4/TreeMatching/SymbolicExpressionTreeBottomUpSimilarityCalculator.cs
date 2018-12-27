#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

using NodeMap = System.Collections.Generic.Dictionary<HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ISymbolicExpressionTreeNode, HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ISymbolicExpressionTreeNode>;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("SymbolicExpressionTreeBottomUpSimilarityCalculator", "A similarity calculator which uses the tree bottom-up distance as a similarity metric.")]
  public class SymbolicExpressionTreeBottomUpSimilarityCalculator : SolutionSimilarityCalculator {
    private readonly HashSet<string> commutativeSymbols = new HashSet<string> { "Addition", "Multiplication", "Average", "And", "Or", "Xor" };

    public SymbolicExpressionTreeBottomUpSimilarityCalculator() { }
    protected override bool IsCommutative { get { return true; } }

    public bool MatchConstantValues { get; set; }
    public bool MatchVariableWeights { get; set; }

    [StorableConstructor]
    protected SymbolicExpressionTreeBottomUpSimilarityCalculator(bool deserializing)
      : base(deserializing) {
    }

    protected SymbolicExpressionTreeBottomUpSimilarityCalculator(SymbolicExpressionTreeBottomUpSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeBottomUpSimilarityCalculator(this, cloner);
    }

    #region static methods
    private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
      return tree.Root.GetSubtree(0).GetSubtree(0);
    }

    public static double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2, bool strict = false) {
      return CalculateSimilarity(ActualRoot(t1), ActualRoot(t2), strict);
    }

    public static double CalculateSimilarity(ISymbolicExpressionTreeNode n1, ISymbolicExpressionTreeNode n2, bool strict = false) {
      var calculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator { MatchConstantValues = strict, MatchVariableWeights = strict };
      return CalculateSimilarity(n1, n2, strict);
    }

    public static Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> ComputeBottomUpMapping(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2, bool strict = false) {
      return ComputeBottomUpMapping(ActualRoot(t1), ActualRoot(t2), strict);
    }

    public static Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> ComputeBottomUpMapping(ISymbolicExpressionTreeNode n1, ISymbolicExpressionTreeNode n2, bool strict = false) {
      var calculator = new SymbolicExpressionTreeBottomUpSimilarityCalculator { MatchConstantValues = strict, MatchVariableWeights = strict };
      return calculator.ComputeBottomUpMapping(n1, n2);
    }
    #endregion

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2) {
      return CalculateSimilarity(t1, t2, out Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> map);
    }

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2, out NodeMap map) {
      if (t1 == t2) {
        map = null;
        return 1;
      }
      map = ComputeBottomUpMapping(t1, t2);
      return 2.0 * map.Count / (t1.Length + t2.Length - 4); // -4 for skipping root and start symbols in the two trees
    }

    public override double CalculateSolutionSimilarity(IScope leftSolution, IScope rightSolution) {
      if (leftSolution == rightSolution)
        return 1.0;

      var t1 = leftSolution.Variables[SolutionVariableName].Value as ISymbolicExpressionTree;
      var t2 = rightSolution.Variables[SolutionVariableName].Value as ISymbolicExpressionTree;

      if (t1 == null || t2 == null)
        throw new ArgumentException("Cannot calculate similarity when one of the arguments is null.");

      var similarity = CalculateSimilarity(t1, t2);
      if (similarity > 1.0)
        throw new Exception("Similarity value cannot be greater than 1");

      return similarity;
    }

    public Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> ComputeBottomUpMapping(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2) {
      return ComputeBottomUpMapping(t1.Root.GetSubtree(0).GetSubtree(0), t2.Root.GetSubtree(0).GetSubtree(0));
    }

    public Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> ComputeBottomUpMapping(ISymbolicExpressionTreeNode n1, ISymbolicExpressionTreeNode n2) {
      var compactedGraph = Compact(n1, n2);

      IEnumerable<ISymbolicExpressionTreeNode> Subtrees(ISymbolicExpressionTreeNode node, bool commutative) {
        var subtrees = node.IterateNodesPrefix();
        return commutative ? subtrees.OrderBy(x => compactedGraph[x].Hash) : subtrees;
      }

      var nodes1 = n1.IterateNodesPostfix().OrderByDescending(x => x.GetLength()); // by descending length so that largest subtrees are mapped first
      var nodes2 = (List<ISymbolicExpressionTreeNode>)n2.IterateNodesPostfix();

      var forward = new NodeMap();
      var reverse = new NodeMap();

      foreach (ISymbolicExpressionTreeNode v in nodes1) {
        if (forward.ContainsKey(v))
          continue;

        var kv = compactedGraph[v];
        var commutative = v.SubtreeCount > 1 && commutativeSymbols.Contains(kv.Label);

        foreach (ISymbolicExpressionTreeNode w in nodes2) {
          if (w.GetLength() != kv.Length || w.GetDepth() != kv.Depth || reverse.ContainsKey(w) || compactedGraph[w] != kv)
            continue;

          // map one whole subtree to the other
          foreach (var t in Subtrees(v, commutative).Zip(Subtrees(w, commutative), Tuple.Create)) {
            forward[t.Item1] = t.Item2;
            reverse[t.Item2] = t.Item1;
          }

          break;
        }
      }

      return forward;
    }

    /// <summary>
    /// Creates a compact representation of the two trees as a directed acyclic graph
    /// </summary>
    /// <param name="n1">The root of the first tree</param>
    /// <param name="n2">The root of the second tree</param>
    /// <returns>The compacted DAG representing the two trees</returns>
    private Dictionary<ISymbolicExpressionTreeNode, GraphNode> Compact(ISymbolicExpressionTreeNode n1, ISymbolicExpressionTreeNode n2) {
      var nodeMap = new Dictionary<ISymbolicExpressionTreeNode, GraphNode>(); // K
      var labelMap = new Dictionary<string, GraphNode>(); // L

      var nodes = n1.IterateNodesPostfix().Concat(n2.IterateNodesPostfix()); // the disjoint union F
      var graph = new List<GraphNode>();

      IEnumerable<GraphNode> Subtrees(GraphNode g, bool commutative) {
        var subtrees = g.SymbolicExpressionTreeNode.Subtrees.Select(x => nodeMap[x]);
        return commutative ? subtrees.OrderBy(x => x.Hash) : subtrees;
      }

      foreach (var node in nodes) {
        var label = GetLabel(node);

        if (node.SubtreeCount == 0) {
          if (!labelMap.ContainsKey(label)) {
            labelMap[label] = new GraphNode(node, label);
          }
          nodeMap[node] = labelMap[label];
        } else {
          var v = new GraphNode(node, label);
          bool found = false;
          var commutative = node.SubtreeCount > 1 && commutativeSymbols.Contains(label);

          var vv = Subtrees(v, commutative);

          foreach (var w in graph) {
            if (v.Depth != w.Depth || v.SubtreeCount != w.SubtreeCount || v.Length != w.Length || v.Label != w.Label) {
              continue;
            }

            var ww = Subtrees(w, commutative);
            found = vv.SequenceEqual(ww);

            if (found) {
              nodeMap[node] = w;
              break;
            }
          }
          if (!found) {
            nodeMap[node] = v;
            graph.Add(v);
          }
        }
      }
      return nodeMap;
    }

    private string GetLabel(ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount > 0)
        return node.Symbol.Name;

      if (node is ConstantTreeNode constant)
        return MatchConstantValues ? constant.Value.ToString(CultureInfo.InvariantCulture) : constant.Symbol.Name;

      if (node is VariableTreeNode variable)
        return MatchVariableWeights ? variable.Weight + variable.VariableName : variable.VariableName;

      return node.ToString();
    }

    private class GraphNode {
      private GraphNode() { }

      public GraphNode(ISymbolicExpressionTreeNode node, string label) {
        SymbolicExpressionTreeNode = node;
        Label = label;
        Hash = GetHashCode();
        Depth = node.GetDepth();
        Length = node.GetLength();
      }

      public int Hash { get; }
      public ISymbolicExpressionTreeNode SymbolicExpressionTreeNode { get; }
      public string Label { get; }
      public int Depth { get; }
      public int SubtreeCount { get { return SymbolicExpressionTreeNode.SubtreeCount; } }
      public int Length { get; }
    }
  }
}
