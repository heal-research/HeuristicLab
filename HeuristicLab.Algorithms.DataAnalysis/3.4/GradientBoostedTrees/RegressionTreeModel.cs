#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("RegressionTreeModel", "Represents a decision tree for regression.")]
  public sealed class RegressionTreeModel : NamedItem, IRegressionModel {

    // trees are represented as a flat array    
    internal struct TreeNode {
      public readonly static string NO_VARIABLE = null;

      public TreeNode(string varName, double val, int leftIdx = -1, int rightIdx = -1)
        : this() {
        VarName = varName;
        Val = val;
        LeftIdx = leftIdx;
        RightIdx = rightIdx;
      }

      public string VarName { get; private set; } // name of the variable for splitting or NO_VARIABLE if terminal node
      public double Val { get; private set; } // threshold
      public int LeftIdx { get; private set; }
      public int RightIdx { get; private set; }

      internal IList<double> Data { get; set; } // only necessary to improve efficiency of evaluation

      // necessary because the default implementation of GetHashCode for structs in .NET would only return the hashcode of val here
      public override int GetHashCode() {
        return LeftIdx ^ RightIdx ^ Val.GetHashCode();
      }
      // necessary because of GetHashCode override
      public override bool Equals(object obj) {
        if (obj is TreeNode) {
          var other = (TreeNode)obj;
          return Val.Equals(other.Val) &&
            LeftIdx.Equals(other.LeftIdx) &&
            RightIdx.Equals(other.RightIdx) &&
            EqualStrings(VarName, other.VarName);
        } else {
          return false;
        }
      }

      private bool EqualStrings(string a, string b) {
        return (a == null && b == null) ||
               (a != null && b != null && a.Equals(b));
      }
    }

    // not storable!
    private TreeNode[] tree;

    [Storable]
    // to prevent storing the references to data caches in nodes
    // seemingly it is bad (performance-wise) to persist tuples (tuples are used as keys in a dictionary) TODO
    private Tuple<string, double, int, int>[] SerializedTree {
      get { return tree.Select(t => Tuple.Create(t.VarName, t.Val, t.LeftIdx, t.RightIdx)).ToArray(); }
      set { this.tree = value.Select(t => new TreeNode(t.Item1, t.Item2, t.Item3, t.Item4)).ToArray(); }
    }

    [StorableConstructor]
    private RegressionTreeModel(bool serializing) : base(serializing) { }
    // cloning ctor
    private RegressionTreeModel(RegressionTreeModel original, Cloner cloner)
      : base(original, cloner) {
      if (original.tree != null) {
        this.tree = new TreeNode[original.tree.Length];
        Array.Copy(original.tree, this.tree, this.tree.Length);
      }
    }

    internal RegressionTreeModel(TreeNode[] tree)
      : base("RegressionTreeModel", "Represents a decision tree for regression.") {
      this.tree = tree;
    }

    private static double GetPredictionForRow(TreeNode[] t, int nodeIdx, int row) {
      while (nodeIdx != -1) {
        var node = t[nodeIdx];
        if (node.VarName == TreeNode.NO_VARIABLE)
          return node.Val;

        if (node.Data[row] <= node.Val)
          nodeIdx = node.LeftIdx;
        else
          nodeIdx = node.RightIdx;
      }
      throw new InvalidOperationException("Invalid tree in RegressionTreeModel");
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionTreeModel(this, cloner);
    }

    public IEnumerable<double> GetEstimatedValues(IDataset ds, IEnumerable<int> rows) {
      // lookup columns for variableNames in one pass over the tree to speed up evaluation later on
      for (int i = 0; i < tree.Length; i++) {
        if (tree[i].VarName != TreeNode.NO_VARIABLE) {
          tree[i].Data = ds.GetReadOnlyDoubleValues(tree[i].VarName);
        }
      }
      return rows.Select(r => GetPredictionForRow(tree, 0, r));
    }

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, new RegressionProblemData(problemData));
    }

    // mainly for debugging
    public override string ToString() {
      return TreeToString(0, "");
    }

    private string TreeToString(int idx, string part) {
      var n = tree[idx];
      if (n.VarName == TreeNode.NO_VARIABLE) {
        return string.Format(CultureInfo.InvariantCulture, "{0} -> {1:F}{2}", part, n.Val, Environment.NewLine);
      } else {
        return
          TreeToString(n.LeftIdx, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2} <= {3:F}", part, string.IsNullOrEmpty(part) ? "" : " and ", n.VarName, n.Val))
        + TreeToString(n.RightIdx, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2} >  {3:F}", part, string.IsNullOrEmpty(part) ? "" : " and ", n.VarName, n.Val));
      }
    }
  }
}
