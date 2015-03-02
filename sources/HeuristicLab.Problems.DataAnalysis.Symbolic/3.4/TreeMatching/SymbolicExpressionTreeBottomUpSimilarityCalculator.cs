#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
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

      var map = ComputeBottomUpMapping(t1.Root, t2.Root);
      return 2.0 * map.Count / (t1.Length + t2.Length);
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
      var nodes1 = n1.IterateNodesPrefix().OrderByDescending(x => x.GetDepth()).ToList();
      var nodes2 = n2.IterateNodesPrefix().ToList();
      for (int i = 0; i < nodes1.Count; ++i) {
        var v = nodes1[i];
        if (forwardMap.ContainsKey(v))
          continue;
        var kv = compactedGraph[v];
        ISymbolicExpressionTreeNode w = null;
        for (int j = 0; j < nodes2.Count; ++j) {
          var t = nodes2[j];
          if (reverseMap.ContainsKey(t) || compactedGraph[t] != kv)
            continue;
          w = t;
          break;
        }
        if (w == null) continue;

        // at this point we know that v and w are isomorphic, however, the mapping cannot be done directly
        // (as in the paper) because the trees are unordered (subtree order might differ). the solution is 
        // to sort subtrees from under commutative labels (this will work because the subtrees are isomorphic!)
        // while iterating over the two subtrees
        var vv = IterateBreadthOrdered(v, comparer).ToList();
        var ww = IterateBreadthOrdered(w, comparer).ToList();
        int len = Math.Min(vv.Count, ww.Count);
        for (int j = 0; j < len; ++j) {
          var s = vv[j];
          var t = ww[j];
          Debug.Assert(!reverseMap.ContainsKey(t));

          forwardMap[s] = t;
          reverseMap[t] = s;
        }
      }

      return forwardMap;
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
      var childrenCount = new Dictionary<ISymbolicExpressionTreeNode, int>(); // Children

      var nodes = n1.IterateNodesPostfix().Concat(n2.IterateNodesPostfix()); // the disjoint union F
      var list = new List<GraphNode>();
      var queue = new Queue<ISymbolicExpressionTreeNode>();

      foreach (var n in nodes) {
        if (n.SubtreeCount == 0) {
          var label = GetLabel(n);
          if (!labelMap.ContainsKey(label)) {
            var z = new GraphNode { SymbolicExpressionTreeNode = n, Label = label };
            labelMap[z.Label] = z;
          }
          nodeMap[n] = labelMap[label];
          queue.Enqueue(n);
        } else {
          childrenCount[n] = n.SubtreeCount;
        }
      }
      while (queue.Any()) {
        var n = queue.Dequeue();
        if (n.SubtreeCount > 0) {
          bool found = false;
          var label = n.Symbol.Name;
          var depth = n.GetDepth();

          bool sort = n.SubtreeCount > 1 && commutativeSymbols.Contains(label);
          var nSubtrees = n.Subtrees.Select(x => nodeMap[x]).ToList();
          if (sort) nSubtrees.Sort((a, b) => string.CompareOrdinal(a.Label, b.Label));

          for (int i = list.Count - 1; i >= 0; --i) {
            var w = list[i];
            if (!(n.SubtreeCount == w.SubtreeCount && label == w.Label && depth == w.Depth))
              continue;

            // sort V and W when the symbol is commutative because we are dealing with unordered trees
            var m = w.SymbolicExpressionTreeNode;
            var mSubtrees = m.Subtrees.Select(x => nodeMap[x]).ToList();
            if (sort) mSubtrees.Sort((a, b) => string.CompareOrdinal(a.Label, b.Label));

            found = nSubtrees.SequenceEqual(mSubtrees);
            if (found) {
              nodeMap[n] = w;
              break;
            }
          }

          if (!found) {
            var w = new GraphNode { SymbolicExpressionTreeNode = n, Label = label, Depth = depth };
            list.Add(w);
            nodeMap[n] = w;
          }
        }

        if (n == n1 || n == n2)
          continue;

        var p = n.Parent;
        if (p == null)
          continue;

        childrenCount[p]--;

        if (childrenCount[p] == 0)
          queue.Enqueue(p);
      }

      return nodeMap;
    }

    private IEnumerable<ISymbolicExpressionTreeNode> IterateBreadthOrdered(ISymbolicExpressionTreeNode node, ISymbolicExpressionTreeNodeComparer comparer) {
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

    private static string GetLabel(ISymbolicExpressionTreeNode node) {
      if (node.SubtreeCount > 0)
        return node.Symbol.Name;

      var constant = node as ConstantTreeNode;
      if (constant != null)
        return constant.Value.ToString(CultureInfo.InvariantCulture);

      var variable = node as VariableTreeNode;
      if (variable != null)
        return variable.Weight + variable.VariableName;

      return node.ToString();
    }

    private class GraphNode {
      public ISymbolicExpressionTreeNode SymbolicExpressionTreeNode;
      public string Label;
      public int Depth;
      public int SubtreeCount { get { return SymbolicExpressionTreeNode.SubtreeCount; } }
    }
  }
}
