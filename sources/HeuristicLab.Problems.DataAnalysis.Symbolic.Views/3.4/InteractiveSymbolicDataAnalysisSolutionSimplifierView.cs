#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> replacementNodes;
    private Dictionary<ISymbolicExpressionTreeNode, double> nodeImpacts;
    private bool updateInProgress = false;

    public InteractiveSymbolicDataAnalysisSolutionSimplifierView() {
      InitializeComponent();
      this.replacementNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      this.nodeImpacts = new Dictionary<ISymbolicExpressionTreeNode, double>();
      this.Caption = "Interactive Solution Simplifier";
    }

    public new ISymbolicDataAnalysisSolution Content {
      get { return (ISymbolicDataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ModelChanged += new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= new EventHandler(Content_ModelChanged);
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }

    private void Content_ModelChanged(object sender, EventArgs e) {
      OnModelChanged();
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnProblemDataChanged();
    }

    protected virtual void OnModelChanged() {
      this.CalculateReplacementNodesAndNodeImpacts();
    }

    protected virtual void OnProblemDataChanged() {
      this.CalculateReplacementNodesAndNodeImpacts();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.CalculateReplacementNodesAndNodeImpacts();
      this.viewHost.Content = this.Content;
    }

    private void CalculateReplacementNodesAndNodeImpacts() {
      if (Content != null && Content.Model != null && Content.ProblemData != null) {
        var tree = Content.Model.SymbolicExpressionTree;
        var replacementValues = CalculateReplacementValues(tree);
        foreach (var pair in replacementValues) {
          if (!(pair.Key is ConstantTreeNode)) {
            replacementNodes[pair.Key] = MakeConstantTreeNode(pair.Value);
          }
        }
        nodeImpacts = CalculateImpactValues(Content.Model.SymbolicExpressionTree);

        if (!updateInProgress) {
          // automatically fold all branches with impact = 1
          List<ISymbolicExpressionTreeNode> nodeList = Content.Model.SymbolicExpressionTree.Root.GetSubtree(0).IterateNodesPrefix().ToList();
          foreach (var parent in nodeList) {
            for (int subTreeIndex = 0; subTreeIndex < parent.SubtreeCount; subTreeIndex++) {
              var child = parent.GetSubtree(subTreeIndex);
              if (!(child.Symbol is Constant) && nodeImpacts[child].IsAlmost(0.0)) {
                SwitchNodeWithReplacementNode(parent, subTreeIndex);
              }
            }
          }
        }

        // show only interesting part of solution 
        if (tree.Root.SubtreeCount > 1)
          this.treeChart.Tree = new SymbolicExpressionTree(tree.Root); // RPB + ADFs
        else
          this.treeChart.Tree = new SymbolicExpressionTree(tree.Root.GetSubtree(0).GetSubtree(0)); // 1st child of RPB
        this.PaintNodeImpacts();
      }
    }

    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateReplacementValues(ISymbolicExpressionTree tree);
    protected abstract Dictionary<ISymbolicExpressionTreeNode, double> CalculateImpactValues(ISymbolicExpressionTree tree);
    protected abstract void UpdateModel(ISymbolicExpressionTree tree);

    private ConstantTreeNode MakeConstantTreeNode(double value) {
      Constant constant = new Constant();
      constant.MinValue = value - 1;
      constant.MaxValue = value + 1;
      ConstantTreeNode constantTreeNode = (ConstantTreeNode)constant.CreateTreeNode();
      constantTreeNode.Value = value;
      return constantTreeNode;
    }

    private void treeChart_SymbolicExpressionTreeNodeDoubleClicked(object sender, MouseEventArgs e) {
      VisualSymbolicExpressionTreeNode visualTreeNode = (VisualSymbolicExpressionTreeNode)sender;
      var tree = Content.Model.SymbolicExpressionTree;
      foreach (SymbolicExpressionTreeNode treeNode in tree.IterateNodesPostfix()) {
        for (int i = 0; i < treeNode.SubtreeCount; i++) {
          ISymbolicExpressionTreeNode subTree = treeNode.GetSubtree(i);
          // only allow to replace nodes for which a replacement value is known (replacement value for ADF nodes are not available)
          if (subTree == visualTreeNode.SymbolicExpressionTreeNode && replacementNodes.ContainsKey(subTree)) {
            SwitchNodeWithReplacementNode(treeNode, i);

            // show only interesting part of solution 
            if (tree.Root.SubtreeCount > 1)
              this.treeChart.Tree = new SymbolicExpressionTree(tree.Root); // RPB + ADFs
            else
              this.treeChart.Tree = new SymbolicExpressionTree(tree.Root.GetSubtree(0).GetSubtree(0)); // 1st child of RPB

            updateInProgress = true;
            UpdateModel(tree);
            updateInProgress = false;
            return; // break all loops
          }
        }
      }
    }

    private void SwitchNodeWithReplacementNode(ISymbolicExpressionTreeNode parent, int subTreeIndex) {
      ISymbolicExpressionTreeNode subTree = parent.GetSubtree(subTreeIndex);
      parent.RemoveSubtree(subTreeIndex);
      if (replacementNodes.ContainsKey(subTree)) {
        var replacementNode = replacementNodes[subTree];
        parent.InsertSubtree(subTreeIndex, replacementNode);
        // exchange key and value 
        replacementNodes.Remove(subTree);
        replacementNodes.Add(replacementNode, subTree);
      }
    }

    private void PaintNodeImpacts() {
      var impacts = nodeImpacts.Values;
      double max = impacts.Max();
      double min = impacts.Min();
      foreach (ISymbolicExpressionTreeNode treeNode in Content.Model.SymbolicExpressionTree.IterateNodesPostfix()) {
        if (!(treeNode is ConstantTreeNode) && nodeImpacts.ContainsKey(treeNode)) {
          double impact = nodeImpacts[treeNode];
          VisualSymbolicExpressionTreeNode visualTree = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);

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
          var constantReplacementNode = replacementNodes[treeNode] as ConstantTreeNode;
          if (constantReplacementNode != null) {
            visualTree.ToolTip += Environment.NewLine + "Replacement value: " + constantReplacementNode.Value;
          }
        }
      }
      this.PaintCollapsedNodes();
      this.treeChart.Repaint();
    }

    private void PaintCollapsedNodes() {
      foreach (ISymbolicExpressionTreeNode treeNode in Content.Model.SymbolicExpressionTree.IterateNodesPostfix()) {
        if (treeNode is ConstantTreeNode && replacementNodes.ContainsKey(treeNode))
          this.treeChart.GetVisualSymbolicExpressionTreeNode(treeNode).LineColor = Color.DarkOrange;
        else {
          VisualSymbolicExpressionTreeNode visNode = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);
          if (visNode != null)
            visNode.LineColor = Color.Black;
        }
      }
    }

    private void btnSimplify_Click(object sender, EventArgs e) {
      SymbolicDataAnalysisExpressionTreeSimplifier simplifier = new SymbolicDataAnalysisExpressionTreeSimplifier();
      var simplifiedExpressionTree = simplifier.Simplify(Content.Model.SymbolicExpressionTree);
      UpdateModel(simplifiedExpressionTree);
    }

    protected abstract void btnOptimizeConstants_Click(object sender, EventArgs e);
  }
}
