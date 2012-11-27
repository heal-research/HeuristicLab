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
    private Dictionary<ISymbolicExpressionTreeNode, double> originalValues;
    private Dictionary<ISymbolicExpressionTreeNode, string> originalVariableNames;

    public InteractiveSymbolicDataAnalysisSolutionSimplifierView() {
      InitializeComponent();
      replacementNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
      nodeImpacts = new Dictionary<ISymbolicExpressionTreeNode, double>();
      originalValues = new Dictionary<ISymbolicExpressionTreeNode, double>();
      originalVariableNames = new Dictionary<ISymbolicExpressionTreeNode, string>();

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
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= Content_Changed;
      Content.ProblemDataChanged -= Content_Changed;
    }

    private void Content_Changed(object sender, EventArgs e) {
      UpdateView();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateView();
      viewHost.Content = this.Content;
    }

    private void UpdateView() {
      if (Content == null || Content.Model == null || Content.ProblemData == null) return;
      var tree = Content.Model.SymbolicExpressionTree;

      var replacementValues = CalculateReplacementValues(tree);
      foreach (var pair in replacementValues.Where(pair => !(pair.Key is ConstantTreeNode))) {
        replacementNodes[pair.Key] = MakeConstantTreeNode(pair.Value);
      }

      nodeImpacts = CalculateImpactValues(tree);

      var model = Content.Model.SymbolicExpressionTree;
      treeChart.Tree = model.Root.SubtreeCount > 1 ? new SymbolicExpressionTree(model.Root) : new SymbolicExpressionTree(model.Root.GetSubtree(0).GetSubtree(0));
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
      if (!treeChart.TreeValid) return;
      var visualNode = (VisualSymbolicExpressionTreeNode)sender;
      var symbExprTreeNode = (SymbolicExpressionTreeNode)visualNode.SymbolicExpressionTreeNode;
      if (symbExprTreeNode == null) return;
      var tree = Content.Model.SymbolicExpressionTree;

      bool update = false;
      // check if the node value/weight has been altered
      // if so, the first double click will return the node to its original value/weight/variable name
      // the next double click will replace the ConstantNode with the original SymbolicExpressionTreeNode
      if (originalVariableNames.ContainsKey(symbExprTreeNode)) {
        var variable = (VariableTreeNode)symbExprTreeNode;
        variable.VariableName = originalVariableNames[symbExprTreeNode];
        originalVariableNames.Remove(variable);
        update = true;
      } else if (originalValues.ContainsKey(symbExprTreeNode)) {
        double value = originalValues[symbExprTreeNode];
        if (symbExprTreeNode.Symbol is Constant) {
          var constantTreeNode = (ConstantTreeNode)symbExprTreeNode;
          constantTreeNode.Value = value;
        } else if (symbExprTreeNode.Symbol is Variable) {
          var variable = (VariableTreeNode)symbExprTreeNode;
          variable.Weight = value;
        }
        originalValues.Remove(symbExprTreeNode);
        update = true;
      } else if (replacementNodes.ContainsKey(symbExprTreeNode)) {
        foreach (var treeNode in tree.IterateNodesPostfix()) {
          for (int i = 0; i < treeNode.SubtreeCount; i++) {
            var subtree = treeNode.GetSubtree(i);
            if (subtree == symbExprTreeNode) {
              SwitchNodeWithReplacementNode(treeNode, i);
              // show only interesting part of solution 
              treeChart.Tree = tree.Root.SubtreeCount > 1
                                 ? new SymbolicExpressionTree(tree.Root)
                                 : new SymbolicExpressionTree(tree.Root.GetSubtree(0).GetSubtree(0));
              update = true;
            }
          }
          if (update) break;
        }
      }
      if (update) UpdateModel(tree);
    }

    private void treeChart_SymbolicExpressionTreeChanged(object sender, EventArgs e) {
      UpdateModel(Content.Model.SymbolicExpressionTree);
      UpdateView();
    }

    private void treeChart_SymbolicExpressionTreeNodeChanged(object sender, EventArgs e) {
      var dialog = (ValueChangeDialog)sender;
      bool flag1 = false, flag2 = false;
      var node = dialog.Content;

      if (node is VariableTreeNode) {
        var variable = (VariableTreeNode)node;
        var weight = double.Parse(dialog.newValueTextBox.Text);
        var name = (string)dialog.variableNamesCombo.SelectedItem;
        if (!variable.Weight.Equals(weight)) {
          flag1 = true;
          originalValues[variable] = variable.Weight;
          variable.Weight = weight;
        }
        if (!variable.VariableName.Equals(name)) {
          flag2 = true;
          originalVariableNames[variable] = variable.VariableName;
          variable.VariableName = name;
        }
      } else if (node is ConstantTreeNode) {
        var constant = (ConstantTreeNode)node;
        var value = double.Parse(dialog.newValueTextBox.Text);
        if (!constant.Value.Equals(value)) {
          flag1 = true;
          originalValues[constant] = constant.Value;
          constant.Value = value;
        }
      }
      if (flag1 || flag2) {
        UpdateView();
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
        VisualSymbolicExpressionTreeNode visualTree = treeChart.GetVisualSymbolicExpressionTreeNode(treeNode);
        bool flag1 = replacementNodes.ContainsKey(treeNode);
        bool flag2 = originalValues.ContainsKey(treeNode);
        bool flag3 = treeNode is ConstantTreeNode;

        if (flag2) // constant or variable node was changed
          visualTree.ToolTip += Environment.NewLine + "Original value: " + originalValues[treeNode];
        else if (flag1 && flag3) // symbol node was folded to a constant
          visualTree.ToolTip += Environment.NewLine + "Original node: " + replacementNodes[treeNode];

        if (!(treeNode is ConstantTreeNode) && nodeImpacts.ContainsKey(treeNode)) {
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
        bool flag1 = replacementNodes.ContainsKey(treeNode);
        bool flag2 = originalValues.ContainsKey(treeNode);
        if (flag1 && treeNode is ConstantTreeNode) {
          this.treeChart.GetVisualSymbolicExpressionTreeNode(treeNode).LineColor = flag2 ? Color.DarkViolet : Color.DarkOrange;
        } else if (flag2) {
          this.treeChart.GetVisualSymbolicExpressionTreeNode(treeNode).LineColor = Color.DodgerBlue;
        }
      }
    }

    private void btnSimplify_Click(object sender, EventArgs e) {
      var simplifier = new SymbolicDataAnalysisExpressionTreeSimplifier();
      var simplifiedExpressionTree = simplifier.Simplify(Content.Model.SymbolicExpressionTree);
      UpdateModel(simplifiedExpressionTree);
    }

    protected abstract void btnOptimizeConstants_Click(object sender, EventArgs e);
  }
}
