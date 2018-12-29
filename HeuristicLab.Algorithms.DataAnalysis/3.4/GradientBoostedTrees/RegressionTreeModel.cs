#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Persistence;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("C383410E-8707-486F-98F6-1DFB708B09B5")]
  [Item("RegressionTreeModel", "Represents a decision tree for regression.")]
  public sealed class RegressionTreeModel : RegressionModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return tree.Select(t => t.VarName).Where(v => v != TreeNode.NO_VARIABLE); }
    }

    // trees are represented as a flat array    
    internal struct TreeNode {
      public readonly static string NO_VARIABLE = null;

      public TreeNode(string varName, double val, int leftIdx = -1, int rightIdx = -1, double weightLeft = -1.0)
        : this() {
        VarName = varName;
        Val = val;
        LeftIdx = leftIdx;
        RightIdx = rightIdx;
        WeightLeft = weightLeft;
      }

      public string VarName { get; internal set; } // name of the variable for splitting or NO_VARIABLE if terminal node
      public double Val { get; internal set; } // threshold
      public int LeftIdx { get; internal set; }
      public int RightIdx { get; internal set; }
      public double WeightLeft { get; internal set; } // for partial dependence plots (value in range [0..1] describes the fraction of training samples for the left sub-tree


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
            WeightLeft.Equals(other.WeightLeft) &&
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

    #region old storable format
    // remove with HL 3.4
    [Storable(AllowOneWay = true)]
    // to prevent storing the references to data caches in nodes
    // seemingly, it is bad (performance-wise) to persist tuples (tuples are used as keys in a dictionary)
    private Tuple<string, double, int, int>[] SerializedTree {
      // get { return tree.Select(t => Tuple.Create(t.VarName, t.Val, t.LeftIdx, t.RightIdx)).ToArray(); }
      set { this.tree = value.Select(t => new TreeNode(t.Item1, t.Item2, t.Item3, t.Item4, -1.0)).ToArray(); } // use a weight of -1.0 to indicate that partial dependence cannot be calculated for old models 
    }
    #endregion
    #region new storable format
    [Storable]
    private string[] SerializedTreeVarNames {
      get { return tree.Select(t => t.VarName).ToArray(); }
      set {
        if (tree == null) tree = new TreeNode[value.Length];
        for (int i = 0; i < value.Length; i++) {
          tree[i].VarName = value[i];
        }
      }
    }
    [Storable]
    private double[] SerializedTreeValues {
      get { return tree.Select(t => t.Val).ToArray(); }
      set {
        if (tree == null) tree = new TreeNode[value.Length];
        for (int i = 0; i < value.Length; i++) {
          tree[i].Val = value[i];
        }
      }
    }
    [Storable]
    private int[] SerializedTreeLeftIdx {
      get { return tree.Select(t => t.LeftIdx).ToArray(); }
      set {
        if (tree == null) tree = new TreeNode[value.Length];
        for (int i = 0; i < value.Length; i++) {
          tree[i].LeftIdx = value[i];
        }
      }
    }
    [Storable]
    private int[] SerializedTreeRightIdx {
      get { return tree.Select(t => t.RightIdx).ToArray(); }
      set {
        if (tree == null) tree = new TreeNode[value.Length];
        for (int i = 0; i < value.Length; i++) {
          tree[i].RightIdx = value[i];
        }
      }
    }
    [Storable]
    private double[] SerializedTreeWeightLeft {
      get { return tree.Select(t => t.WeightLeft).ToArray(); }
      set {
        if (tree == null) tree = new TreeNode[value.Length];
        for (int i = 0; i < value.Length; i++) {
          tree[i].WeightLeft = value[i];
        }
      }
    }
    #endregion

    [StorableConstructor]
    private RegressionTreeModel(StorableConstructorFlag _) : base(_) { }
    // cloning ctor
    private RegressionTreeModel(RegressionTreeModel original, Cloner cloner)
      : base(original, cloner) {
      if (original.tree != null) {
        this.tree = new TreeNode[original.tree.Length];
        Array.Copy(original.tree, this.tree, this.tree.Length);
      }
    }

    internal RegressionTreeModel(TreeNode[] tree, string targetVariable)
      : base(targetVariable, "RegressionTreeModel", "Represents a decision tree for regression.") {
      this.tree = tree;
    }

    private static double GetPredictionForRow(TreeNode[] t, ReadOnlyCollection<double>[] columnCache, int nodeIdx, int row) {
      while (nodeIdx != -1) {
        var node = t[nodeIdx];
        if (node.VarName == TreeNode.NO_VARIABLE)
          return node.Val;
        if (columnCache[nodeIdx] == null || double.IsNaN(columnCache[nodeIdx][row])) {
          if (node.WeightLeft.IsAlmost(-1.0)) throw new InvalidOperationException("Cannot calculate partial dependence for trees loaded from older versions of HeuristicLab.");
          // weighted average for partial dependence plot (recursive here because we need to calculate both sub-trees)
          return node.WeightLeft * GetPredictionForRow(t, columnCache, node.LeftIdx, row) +
                 (1.0 - node.WeightLeft) * GetPredictionForRow(t, columnCache, node.RightIdx, row);
        } else if (columnCache[nodeIdx][row] <= node.Val)
          nodeIdx = node.LeftIdx;
        else
          nodeIdx = node.RightIdx;
      }
      throw new InvalidOperationException("Invalid tree in RegressionTreeModel");
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionTreeModel(this, cloner);
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset ds, IEnumerable<int> rows) {
      // lookup columns for variableNames in one pass over the tree to speed up evaluation later on
      ReadOnlyCollection<double>[] columnCache = new ReadOnlyCollection<double>[tree.Length];

      for (int i = 0; i < tree.Length; i++) {
        if (tree[i].VarName != TreeNode.NO_VARIABLE) {
          // tree models also support calculating estimations if not all variables used for training are available in the dataset
          if (ds.ColumnNames.Contains(tree[i].VarName))
            columnCache[i] = ds.GetReadOnlyDoubleValues(tree[i].VarName);
        }
      }
      return rows.Select(r => GetPredictionForRow(tree, columnCache, 0, r));
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, new RegressionProblemData(problemData));
    }

    // mainly for debugging
    public override string ToString() {
      return TreeToString(0, "");
    }

    /// <summary>
    /// Transforms the tree model to a symbolic regression solution
    /// </summary>
    /// <param name="problemData"></param>
    /// <returns>A new symbolic regression solution which matches the tree model</returns>
    public ISymbolicRegressionSolution CreateSymbolicRegressionSolution(IRegressionProblemData problemData) {
      return CreateSymbolicRegressionModel().CreateRegressionSolution(problemData);
    }

    /// <summary>
    /// Transforms the tree model to a symbolic regression model
    /// </summary>
    /// <returns>A new symbolic regression model which matches the tree model</returns>
    public SymbolicRegressionModel CreateSymbolicRegressionModel() {
      var rootSy = new ProgramRootSymbol();
      var startSy = new StartSymbol();
      var varCondSy = new VariableCondition() { IgnoreSlope = true };
      var constSy = new Constant();

      var startNode = startSy.CreateTreeNode();
      startNode.AddSubtree(CreateSymbolicRegressionTreeRecursive(tree, 0, varCondSy, constSy));
      var rootNode = rootSy.CreateTreeNode();
      rootNode.AddSubtree(startNode);
      return new SymbolicRegressionModel(TargetVariable, new SymbolicExpressionTree(rootNode), new SymbolicDataAnalysisExpressionTreeLinearInterpreter());
    }

    private ISymbolicExpressionTreeNode CreateSymbolicRegressionTreeRecursive(TreeNode[] treeNodes, int nodeIdx, VariableCondition varCondSy, Constant constSy) {
      var curNode = treeNodes[nodeIdx];
      if (curNode.VarName == TreeNode.NO_VARIABLE) {
        var node = (ConstantTreeNode)constSy.CreateTreeNode();
        node.Value = curNode.Val;
        return node;
      } else {
        var node = (VariableConditionTreeNode)varCondSy.CreateTreeNode();
        node.VariableName = curNode.VarName;
        node.Threshold = curNode.Val;

        var left = CreateSymbolicRegressionTreeRecursive(treeNodes, curNode.LeftIdx, varCondSy, constSy);
        var right = CreateSymbolicRegressionTreeRecursive(treeNodes, curNode.RightIdx, varCondSy, constSy);
        node.AddSubtree(left);
        node.AddSubtree(right);
        return node;
      }
    }


    private string TreeToString(int idx, string part) {
      var n = tree[idx];
      if (n.VarName == TreeNode.NO_VARIABLE) {
        return string.Format(CultureInfo.InvariantCulture, "{0} -> {1:F}{2}", part, n.Val, Environment.NewLine);
      } else {
        return
          TreeToString(n.LeftIdx, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2} <= {3:F} ({4:N3})", part, string.IsNullOrEmpty(part) ? "" : " and ", n.VarName, n.Val, n.WeightLeft))
        + TreeToString(n.RightIdx, string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}  >  {3:F} ({4:N3}))", part, string.IsNullOrEmpty(part) ? "" : " and ", n.VarName, n.Val, 1.0 - n.WeightLeft));
      }
    }

  }
}
