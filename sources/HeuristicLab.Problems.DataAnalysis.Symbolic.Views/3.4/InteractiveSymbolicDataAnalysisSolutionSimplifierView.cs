#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public abstract partial class InteractiveSymbolicDataAnalysisSolutionSimplifierView : AsynchronousContentView {
    private Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> foldedNodes;
    private Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> changedNodes;
    private Dictionary<ISymbolicExpressionTreeNode, double> nodeImpacts;

    private enum TreeState { Valid, Invalid }
    private TreeState treeState;

    protected InteractiveSymbolicDataAnalysisSolutionSimplifierView() {
      InitializeComponent();
      foldedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      changedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      nodeImpacts = new Dictionary<ISymbolicExpressionTreeNode, double>();
      this.Caption = "Interactive Solution Simplifier";

      // initialize the tree modifier that will be used to perform edit operations over the tree
      treeChart.ModifyTree = Modify;
    }

    /// <summary>
    /// Remove, Replace or Insert subtrees
    /// </summary>
    /// <param name="tree">The symbolic expression tree</param>
    /// <param name="parent">The insertion point (ie, the parent node who will receive a new child)</param>
    /// <param name="oldChild">The subtree to be replaced</param>
    /// <param name="newChild">The replacement subtree</param>
    /// <param name="removeSubtree">Flag used to indicate if whole subtrees should be removed (default behavior), or just the subtree root</param>
    private void Modify(ISymbolicExpressionTree tree, ISymbolicExpressionTreeNode parent,
      ISymbolicExpressionTreeNode oldChild, ISymbolicExpressionTreeNode newChild, bool removeSubtree = true) {
      if (oldChild == null && newChild == null)
        throw new ArgumentNullException("Cannot deduce operation type from the arguments. Please provide non null operands.");
      if (oldChild == null) {
        // insertion operation
        parent.AddSubtree(newChild);
        newChild.Parent = parent;
      } else if (newChild == null) {
        // removal operation
        parent.RemoveSubtree(parent.IndexOfSubtree(oldChild));
        if (!removeSubtree) {
          for (int i = oldChild.SubtreeCount - 1; i >= 0; --i) {
            var subtree = oldChild.GetSubtree(i);
            oldChild.RemoveSubtree(i);
            parent.AddSubtree(subtree);
          }
        }
      } else {
        // replacement operation
        var replacementIndex = parent.IndexOfSubtree(oldChild);
        parent.RemoveSubtree(replacementIndex);
        parent.InsertSubtree(replacementIndex, newChild);
        newChild.Parent = parent;
        if (changedNodes.ContainsKey(oldChild)) {
          changedNodes.Add(newChild, changedNodes[oldChild]); // so that on double click the original node is restored
          changedNodes.Remove(oldChild);
        } else {
          changedNodes.Add(newChild, oldChild);
        }
      }
      treeState = IsValid(tree) ? TreeState.Valid : TreeState.Invalid;
      switch (treeState) {
        case TreeState.Valid:
          this.grpViewHost.Enabled = true;
          UpdateModel(Content.Model.SymbolicExpressionTree);
          break;
        case TreeState.Invalid:
          this.grpViewHost.Enabled = false;
          break;
      }
    }

    // the optimizer always assumes 2 children for multiplication and addition nodes
    // thus, we enforce that the tree stays valid so that the constant optimization won't throw an exception
    // by returning 2 as the minimum allowed arity for addition and multiplication symbols
    private readonly Func<ISymbol, int> GetMinArity = symbol => {
      var min = symbol.MinimumArity;
      if (symbol is Multiplication || symbol is Division) return Math.Max(2, min);
      return min;
    };
    private bool IsValid(ISymbolicExpressionTree tree) {
      treeChart.Tree = tree;
      treeChart.Repaint();
      bool valid = !tree.IterateNodesPostfix().Any(node => node.SubtreeCount < GetMinArity(node.Symbol) || node.SubtreeCount > node.Symbol.MaximumArity);
      if (valid) {
        btnOptimizeConstants.Enabled = true;
        btnSimplify.Enabled = true;
        treeStatusValue.Visible = false;
      } else {
        btnOptimizeConstants.Enabled = false;
        btnSimplify.Enabled = false;
        treeStatusValue.Visible = true;
      }
      this.Refresh();
      return valid;
    }

    public new ISymbolicDataAnalysisSolution Content {
      get { return (ISymbolicDataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += Content_Changed;
      Content.ProblemDataChanged += Content_Changed;
      treeChart.Repainted += treeChart_Repainted;
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= Content_Changed;
      Content.ProblemDataChanged -= Content_Changed;
      treeChart.Repainted -= treeChart_Repainted;
    }

    private void Content_Changed(object sender, EventArgs e) {
      UpdateView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      foldedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      UpdateView();
      viewHost.Content = this.Content;
    }

    private void treeChart_Repainted(object sender, EventArgs e) {
      if (nodeImpacts != null && nodeImpacts.Count > 0)
        PaintNodeImpacts();
    }

    private void UpdateView() {
      if (Content == null || Content.Model == null || Content.ProblemData == null) return;
      var tree = Content.Model.SymbolicExpressionTree;
      treeChart.Tree = tree.Root.SubtreeCount > 1 ? new SymbolicExpressionTree(tree.Root) : new SymbolicExpressionTree(tree.Root.GetSubtree(0).GetSubtree(0));

      var impactAndReplacementValues = CalculateImpactAndReplacementValues(tree);
      var replacementValues = impactAndReplacementValues.ToDictionary(x => x.Key, x => x.Value.Item2);
      foreach (var pair in replacementValues.Where(pair => !(pair.Key is ConstantTreeNode))) {
        foldedNodes[pair.Key] = MakeConstantTreeNode(pair.Value);
      }
      nodeImpacts = impactAndReplacementValues.ToDictionary(x => x.Key, x => x.Value.Item1);
      PaintNodeImpacts();
    }

    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree);
    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree);
    protected abstract Dictionary<ISymbolicExpressionTreeNode, Tuple<double, double>> CalculateImpactAndReplacementValues(ISymbolicExpressionTree tree);
    protected abstract void UpdateModel(ISymbolicExpressionTree tree);

    private static ConstantTreeNode MakeConstantTreeNode(double value) {
      var constant = new Constant { MinValue = value - 1, MaxValue = value + 1 };
      var constantTreeNode = (ConstantTreeNode)constant.CreateTreeNode();
      constantTreeNode.Value = value;
      return constantTreeNode;
    }

    private void treeChart_SymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      if (treeState == TreeState.Invalid) return;
      var visualNode = (VisualTreeNode<ISymbolicExpressionTreeNode>)sender;
      if (visualNode.Content == null) { throw new Exception("VisualNode content cannot be null."); }
      var symbExprTreeNode = (SymbolicExpressionTreeNode)visualNode.Content;
      var tree = Content.Model.SymbolicExpressionTree;
      var parent = symbExprTreeNode.Parent;
      int indexOfSubtree = parent.IndexOfSubtree(symbExprTreeNode);
      if (changedNodes.ContainsKey(symbExprTreeNode)) {
        // undo node change
        parent.RemoveSubtree(indexOfSubtree);
        var originalNode = changedNodes[symbExprTreeNode];
        parent.InsertSubtree(indexOfSubtree, originalNode);
        changedNodes.Remove(symbExprTreeNode);
      } else if (foldedNodes.ContainsKey(symbExprTreeNode)) {
        // undo node folding
        SwitchNodeWithReplacementNode(parent, indexOfSubtree);
      }
      UpdateModel(tree);
    }

    private void SwitchNodeWithReplacementNode(ISymbolicExpressionTreeNode parent, int subTreeIndex) {
      ISymbolicExpressionTreeNode subTree = parent.GetSubtree(subTreeIndex);
      if (foldedNodes.ContainsKey(subTree)) {
        parent.RemoveSubtree(subTreeIndex);
        var replacementNode = foldedNodes[subTree];
        parent.InsertSubtree(subTreeIndex, replacementNode);
        // exchange key and value 
        foldedNodes.Remove(subTree);
        foldedNodes.Add(replacementNode, subTree);
      }
    }

    private void PaintNodeImpacts() {
      var impacts = nodeImpacts.Values;
      double max = impacts.Max();
      double min = impacts.Min();
      foreach (ISymbolicExpressionTreeNode treeNode in Content.Model.SymbolicExpressionTree.IterateNodesPostfix()) {
        VisualTreeNode<ISymbolicExpressionTreeNode> visualTree = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);

        if (!(treeNode is ConstantTreeNode) && nodeImpacts.ContainsKey(treeNode)) {
          visualTree.ToolTip = visualTree.Content.ToString();
          double impact = nodeImpacts[treeNode];

          // impact = 0 if no change
          // impact < 0 if new solution is better
          // impact > 0 if new solution is worse
          if (impact < 0.0) {
            // min is guaranteed to be < 0
            visualTree.FillColor = Color.FromArgb((int)(impact / min * 255), Color.Red);
          } else if (impact.IsAlmost(0.0)) {
            visualTree.FillColor = Color.White;
          } else {
            // max is guaranteed to be > 0
            visualTree.FillColor = Color.FromArgb((int)(impact / max * 255), Color.Green);
          }
          visualTree.ToolTip += Environment.NewLine + "Node impact: " + impact;
          var constantReplacementNode = foldedNodes[treeNode] as ConstantTreeNode;
          if (constantReplacementNode != null) {
            visualTree.ToolTip += Environment.NewLine + "Replacement value: " + constantReplacementNode.Value;
          }
        }
        if (visualTree != null)
          if (changedNodes.ContainsKey(treeNode)) {
            visualTree.LineColor = Color.DodgerBlue;
          } else if (treeNode is ConstantTreeNode && foldedNodes.ContainsKey(treeNode)) {
            visualTree.LineColor = Color.DarkOrange;
          }
      }
      treeChart.RepaintNodes();
    }

    private void btnSimplify_Click(object sender, EventArgs e) {
      var simplifier = new TreeSimplifier();
      var simplifiedExpressionTree = simplifier.Simplify(Content.Model.SymbolicExpressionTree);
      UpdateModel(simplifiedExpressionTree);
    }

    protected abstract void btnOptimizeConstants_Click(object sender, EventArgs e);
  }
}
