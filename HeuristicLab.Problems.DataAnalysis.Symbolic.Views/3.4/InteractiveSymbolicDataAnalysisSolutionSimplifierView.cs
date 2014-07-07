#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private Dictionary<ISymbolicExpressionTreeNode, double> nodeImpacts;
    private enum TreeState { Valid, Invalid }

    public InteractiveSymbolicDataAnalysisSolutionSimplifierView() {
      InitializeComponent();
      foldedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      nodeImpacts = new Dictionary<ISymbolicExpressionTreeNode, double>();
      this.Caption = "Interactive Solution Simplifier";
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

      var replacementValues = CalculateReplacementValues(tree);
      foreach (var pair in replacementValues.Where(pair => !(pair.Key is ConstantTreeNode))) {
        foldedNodes[pair.Key] = MakeConstantTreeNode(pair.Value);
      }

      nodeImpacts = CalculateImpactValues(tree);
      PaintNodeImpacts();
    }

    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree);
    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree);
    protected abstract void UpdateModel(ISymbolicExpressionTree tree);

    private static ConstantTreeNode MakeConstantTreeNode(double value) {
      var constant = new Constant { MinValue = value - 1, MaxValue = value + 1 };
      var constantTreeNode = (ConstantTreeNode)constant.CreateTreeNode();
      constantTreeNode.Value = value;
      return constantTreeNode;
    }

    private void treeChart_SymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      var visualNode = (VisualTreeNode<ISymbolicExpressionTreeNode>)sender;
      if (visualNode.Content == null) { throw new Exception("Visual node content cannot be null."); }
      var symbExprTreeNode = (SymbolicExpressionTreeNode)visualNode.Content;
      if (!foldedNodes.ContainsKey(symbExprTreeNode)) return; // constant nodes cannot be folded
      var parent = symbExprTreeNode.Parent;
      int indexOfSubtree = parent.IndexOfSubtree(symbExprTreeNode);
      SwitchNodeWithReplacementNode(parent, indexOfSubtree);
      UpdateModel(Content.Model.SymbolicExpressionTree);
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
      foreach (var treeNode in Content.Model.SymbolicExpressionTree.IterateNodesPostfix()) {
        VisualTreeNode<ISymbolicExpressionTreeNode> visualTree = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);

        if (!(treeNode is ConstantTreeNode) && nodeImpacts.ContainsKey(treeNode)) {
          visualTree.ToolTip = visualTree.Content.ToString(); // to avoid duplicate tooltips
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
          if (treeNode is ConstantTreeNode && foldedNodes.ContainsKey(treeNode)) {
            visualTree.LineColor = Color.DarkOrange;
          }
      }
      treeChart.RepaintNodes();
    }

    private void btnSimplify_Click(object sender, EventArgs e) {
      var simplifier = new SymbolicDataAnalysisExpressionTreeSimplifier();
      var simplifiedExpressionTree = simplifier.Simplify(Content.Model.SymbolicExpressionTree);
      UpdateModel(simplifiedExpressionTree);
    }

    protected abstract void btnOptimizeConstants_Click(object sender, EventArgs e);
  }
}
