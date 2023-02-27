﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public abstract partial class InteractiveSymbolicDataAnalysisSolutionSimplifierView : AsynchronousContentView {
    private readonly Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> foldedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
    private readonly Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode> changedNodes = new Dictionary<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode>();
    private readonly Dictionary<ISymbolicExpressionTreeNode, Interval> nodeIntervals = new Dictionary<ISymbolicExpressionTreeNode, Interval>();
    private readonly Dictionary<ISymbolicExpressionTreeNode, double> nodeImpacts = new Dictionary<ISymbolicExpressionTreeNode, double>();

    private ISymbolicDataAnalysisSolutionImpactValuesCalculator impactCalculator;

    private readonly Progress progress = new Progress();
    private CancellationTokenSource cancellationTokenSource;

    private enum TreeState { Valid, Invalid }
    private TreeState treeState;

    protected InteractiveSymbolicDataAnalysisSolutionSimplifierView(params ISymbolicDataAnalysisSolutionImpactValuesCalculator[] availableImpactCalculator) {
      InitializeComponent();
      this.Caption = "Interactive Solution Simplifier";
      
      // initialize the tree modifier that will be used to perform edit operations over the tree
      treeChart.ModifyTree = Modify;
      
      // setup impact calculators
      impactCalculator = availableImpactCalculator.FirstOrDefault();
      treeChart.InitializeAvailableImpactCalculators(availableImpactCalculator);
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
    // thus, we enforce that the tree stays valid so that the parameter optimization won't throw an exception
    // by returning 2 as the minimum allowed arity for addition and multiplication symbols
    private readonly Func<ISymbol, int> GetMinArity = symbol => {
      var min = symbol.MinimumArity;
      if (symbol is Multiplication || symbol is Division) return Math.Max(2, min);
      return min;
    };
    private bool IsValid(ISymbolicExpressionTree tree) {
      treeChart.Tree = tree;
      treeChart.Repaint();
      // check if all nodes have a legal arity
      var nodes = tree.IterateNodesPostfix().ToList();
      bool valid = !nodes.Any(node => node.SubtreeCount < GetMinArity(node.Symbol) || node.SubtreeCount > node.Symbol.MaximumArity);

      if (valid) {
        // check if all variables are contained in the dataset
        var variables = new HashSet<string>(Content.ProblemData.Dataset.DoubleVariables);
        valid = nodes.OfType<VariableTreeNode>().All(x => variables.Contains(x.VariableName));
      }

      if (valid) {
        btnOptimizeParameters.Enabled = true;
        btnSimplify.Enabled = true;
        treeStatusValue.Visible = false;
      } else {
        btnOptimizeParameters.Enabled = false;
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
      treeChart.ImpactCalculatorChanged += treeChart_ImpactCalculatorChanged;
      Progress.ShowOnControl(grpSimplify, progress);
      progress.StopRequested += progress_StopRequested;
    }
    
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ModelChanged -= Content_Changed;
      Content.ProblemDataChanged -= Content_Changed;
      treeChart.Repainted -= treeChart_Repainted;
      treeChart.ImpactCalculatorChanged -= treeChart_ImpactCalculatorChanged;
      Progress.HideFromControl(grpSimplify, false);
      progress.StopRequested -= progress_StopRequested;
    }

    private void Content_Changed(object sender, EventArgs e) {
      UpdateView();
      SetEnabledStateOfControls();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      foldedNodes.Clear();
      changedNodes.Clear();
      nodeIntervals.Clear();
      nodeImpacts.Clear();
      UpdateView();
      viewHost.Content = this.Content;
    }

    private void treeChart_Repainted(object sender, EventArgs e) {
      if (nodeImpacts != null && nodeImpacts.Count > 0)
        PaintNodeImpacts();
    }

    private void progress_StopRequested(object sender, EventArgs e) {
      cancellationTokenSource.Cancel();
    }

    private async void UpdateView() {
      if (Content == null || Content.Model == null || Content.ProblemData == null) return;
      var tree = Content.Model.SymbolicExpressionTree;
      treeChart.Tree = tree.Root.SubtreeCount > 1 ? new SymbolicExpressionTree(tree.Root) : new SymbolicExpressionTree(tree.Root.GetSubtree(0).GetSubtree(0));

      progress.Start("Calculate impact and replacement values ...");
      cancellationTokenSource = new CancellationTokenSource();
      progress.CanBeStopped = true;
      try {
        var impactAndReplacementValues = await Task.Run(() => CalculateImpactAndReplacementValues(tree));
        try {
          await Task.Delay(300, cancellationTokenSource.Token); // wait for progressbar to finish animation
        } catch (OperationCanceledException) { }

        var replacementValues = impactAndReplacementValues.ToDictionary(x => x.Key, x => x.Value.Item2);
        foreach (var pair in replacementValues.Where(pair => !(pair.Key is INumericTreeNode))) {
          foldedNodes[pair.Key] = MakeNumberTreeNode(pair.Value);
        }
        
        foreach (var pair in impactAndReplacementValues) {
          nodeImpacts[pair.Key] = pair.Value.Item1;
        }

        if (IntervalInterpreter.IsCompatible(tree)) { 
          var regressionProblemData = Content.ProblemData as IRegressionProblemData;
          if (regressionProblemData != null) {
            var interpreter = new IntervalInterpreter();
            var variableRanges = regressionProblemData.VariableRanges.GetReadonlyDictionary();
            IDictionary<ISymbolicExpressionTreeNode, Interval> intervals;
            interpreter.GetSymbolicExpressionTreeIntervals(tree, variableRanges, out intervals);
            foreach (var kvp in intervals) {
              nodeIntervals[kvp.Key] = kvp.Value;
            }
          }
        }
      } finally {
        progress.Finish();
      }

      progress.CanBeStopped = false;
      PaintNodeImpacts();
    }

    protected virtual Dictionary<ISymbolicExpressionTreeNode, Tuple<double, double>> CalculateImpactAndReplacementValues(ISymbolicExpressionTree tree) {
      var impactAndReplacementValues = new Dictionary<ISymbolicExpressionTreeNode, Tuple<double, double>>();
      foreach (var node in tree.Root.GetSubtree(0).GetSubtree(0).IterateNodesPrefix()) {
        if (progress.ProgressState == ProgressState.StopRequested) continue;
        if (impactCalculator != null) {
          impactCalculator.CalculateImpactAndReplacementValues(Content.Model, node, Content.ProblemData, Content.ProblemData.TrainingIndices,
            out double impactValue, out double replacementValue, out _);
          impactAndReplacementValues.Add(node, new Tuple<double, double>(impactValue, replacementValue));
        } else {
          impactAndReplacementValues.Add(node, new Tuple<double, double>(0, 0));  
        }
        double newProgressValue = progress.ProgressValue + 1.0 / (tree.Length - 2);
        progress.ProgressValue = Math.Min(newProgressValue, 1);
        
      }
      return impactAndReplacementValues;
    }

    protected abstract void UpdateModel(ISymbolicExpressionTree tree);

    protected virtual ISymbolicExpressionTree OptimizeParameters(ISymbolicExpressionTree tree, IProgress progress) {
      return tree;
    }

    private static NumberTreeNode MakeNumberTreeNode(double value) {
      var num = new Number { MinValue = value - 1, MaxValue = value + 1 };
      var numTreeNode = (NumberTreeNode)num.CreateTreeNode();
      numTreeNode.Value = value;
      return numTreeNode;
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

        if (!(treeNode is INumericTreeNode) && nodeImpacts.ContainsKey(treeNode)) {
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
          if (foldedNodes[treeNode] is INumericTreeNode numReplacementNode) {
            visualTree.ToolTip += Environment.NewLine + "Replacement value: " + numReplacementNode.Value;
          }
        }
        if (visualTree != null) {
          if (nodeIntervals.ContainsKey(treeNode))
            visualTree.ToolTip += string.Format($"{Environment.NewLine}Intervals: [{nodeIntervals[treeNode].LowerBound:G5} ... {nodeIntervals[treeNode].UpperBound:G5}]");
          if (changedNodes.ContainsKey(treeNode)) {
            visualTree.LineColor = Color.DodgerBlue;
          } else if (treeNode is INumericTreeNode && foldedNodes.ContainsKey(treeNode)) {
            visualTree.LineColor = Color.DarkOrange;
          }
        }
      }
      treeChart.RepaintNodes();
    }

    private void treeChart_ImpactCalculatorChanged(object sender, EventArgs e) {
      impactCalculator = treeChart.ImpactCalculator;
      UpdateView();
    }
    
    private void btnSimplify_Click(object sender, EventArgs e) {
      var simplifiedExpressionTree = TreeSimplifier.Simplify(Content.Model.SymbolicExpressionTree);
      UpdateModel(simplifiedExpressionTree);
    }

    private async void btnOptimizeParameters_Click(object sender, EventArgs e) {
      progress.Start("Optimizing parameters ...");
      cancellationTokenSource = new CancellationTokenSource();
      progress.CanBeStopped = true;
      try {
        var tree = (ISymbolicExpressionTree)Content.Model.SymbolicExpressionTree.Clone();

        var newTree = await Task.Run(() => OptimizeParameters(tree, progress));
        try {
          await Task.Delay(300, cancellationTokenSource.Token); // wait for progressbar to finish animation
        } catch (OperationCanceledException) { }
        UpdateModel(newTree); // triggers progress.Finish after calculating the node impacts when model is changed
      } catch {
        progress.Finish();
      }
    }
  }
}
