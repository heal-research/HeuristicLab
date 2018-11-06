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
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

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

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2) {
      if (t1 == t2)
        return 1;

      var actualRoot1 = t1.Root.GetSubtree(0).GetSubtree(0); // skip root and start symbols
      var actualRoot2 = t2.Root.GetSubtree(0).GetSubtree(0); // skip root and start symbols
      var map = ComputeBottomUpMapping(actualRoot1, actualRoot2);
      return 2.0 * map.Count / (t1.Length + t2.Length - 4); // -4 for skipping root and start symbols in the two trees
    }

    public double CalculateSimilarity(ISymbolicExpressionTree t1, ISymbolicExpressionTree t2, out Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> map) {
      if (t1 == t2) {
        map = null;
        return 1;
      }

      var actualRoot1 = t1.Root.GetSubtree(0).GetSubtree(0); // skip root and start symbols
      var actualRoot2 = t2.Root.GetSubtree(0).GetSubtree(0); // skip root and start symbols
      map = ComputeBottomUpMapping(actualRoot1, actualRoot2);

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

    public Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> ComputeBottomUpMapping(ISymbolicExpressionTreeNode n1, ISymbolicExpressionTreeNode n2) {
      var comparer = new SymbolicExpressionTreeNodeComparer(); // use a node comparer because it's faster than calling node.ToString() (strings are expensive) and comparing strings
      var compactedGraph = Compact(n1, n2);

      var forwardMap = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>(); // nodes of t1 => nodes of t2
      var reverseMap = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>(); // nodes of t2 => nodes of t1

      // visit nodes in order of decreasing height to ensure correct mapping
      var nodes1 = (List<ISymbolicExpressionTreeNode>)n1.IterateNodesPrefix();
      var nodes2 = (List<ISymbolicExpressionTreeNode>)n2.IterateNodesPrefix();

      foreach (var v in nodes1) {
        if (forwardMap.ContainsKey(v)) {
          continue;
        }

        var kv = compactedGraph[v];

        var w = nodes2.Last();
        int k = nodes2.Count - 1;

        for (int j = 0; j < nodes2.Count; ++j) {
          var t = nodes2[j];

          if (reverseMap.ContainsKey(t) || kv != compactedGraph[t])
            continue;

          if (j < k) {
            w = t;
            k = j;
          }
        }

        if (kv != compactedGraph[w]) {
          continue;
        }

        // at this point we know that v and w are isomorphic, however, the mapping cannot be done directly
        // (as in the paper) because the trees are unordered (subtree order might differ). the solution is 
        // to sort subtrees from under commutative labels (this will work because the subtrees are isomorphic!)
        // while iterating over the two subtrees
        var vv = IterateBreadthOrdered(v, comparer);
        var ww = IterateBreadthOrdered(w, comparer);
        int len = Math.Min(vv.Count, ww.Count);
        for (int j = 0; j < len; ++j) {
          var s = vv[j];
          var t = ww[j];

          if (reverseMap.ContainsKey(t))
            continue;

          forwardMap[s] = t;
          reverseMap[t] = s;
        }
      }

      return forwardMap;
    }

    private List<ISymbolicExpressionTreeNode> IterateBreadthOrdered(ISymbolicExpressionTreeNode node, ISymbolicExpressionTreeNodeComparer comparer) {
      var list = new List<ISymbolicExpressionTreeNode> { node };
      int i = 0;
      while (i < list.Count) {
        var n = list[i];
        if (n.SubtreeCount > 0) {
          var subtrees = commutativeSymbols.Contains(node.Symbol.Name) ? n.Subtrees.OrderBy(x => x, comparer) : n.Subtrees;
          list.AddRange(subtrees);
        }
        i++;
      }
      return list;
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
      var comparer = new SymbolicExpressionTreeNodeComparer();

      var nodes = n1.IterateNodesPostfix().Concat(n2.IterateNodesPostfix()); // the disjoint union F
      var graph = new List<GraphNode>();

      foreach (var node in nodes) {
        var label = GetLabel(node);

        if (node.SubtreeCount == 0) {
          if (!labelMap.ContainsKey(label)) {
            var g = new GraphNode(node, label);
            graph.Add(g);
            labelMap[label] = g;
          }
          nodeMap[node] = labelMap[label];
        } else {
          var v = new GraphNode(node, label);
          bool found = false;
          var commutative = node.SubtreeCount > 1 && commutativeSymbols.Contains(label);

          IEnumerable<GraphNode> vv, ww;

          for (int i = graph.Count - 1; i >= 0; --i) {
            var w = graph[i];

            if (v.Depth != w.Depth || v.SubtreeCount != w.SubtreeCount || v.Length != w.Length || v.Label != w.Label) {
              continue;
            }

            vv = commutative ? v.SymbolicExpressionTreeNode.Subtrees.Select(x => nodeMap[x]).OrderBy(x => x.Hash) : v.SymbolicExpressionTreeNode.Subtrees.Select(x => nodeMap[x]);
            ww = commutative ? w.SymbolicExpressionTreeNode.Subtrees.Select(x => nodeMap[x]).OrderBy(x => x.Hash) : w.SymbolicExpressionTreeNode.Subtrees.Select(x => nodeMap[x]);
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
